using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Private variables
    bool _isGrounded = true, actions = true;
    Transform _groundChecker;
    bool canMove;//, in_the_air
    int comboIndex = 0;
    float resetTimer, reduce_speed; // used to reduce speed when attacking or after hitting the ground
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    Animator m_Animator;
    Rigidbody rb;
    CharacterController cc; // controls the Character

    // Public variables
    public Transform cam;
    public float speed = 3.0f; // Chacater speed
    public float turnSpeed = 8f; // Chacater rotation speed
    public float jumpSpeed = 4.8f; // Character jump speed
    public float gravity = 30.0f; // gravity applied to the Character
    public float fireRate = 1.0f; // rate of the attack
    public int forceConst = 50; // force applied
    public string[] comboParams;
    public LayerMask Ground;
    public float GroundDistance = 0.2f;
    public GameObject showFps;


    void Start()
    {
        // Private vars
        _groundChecker = transform.GetChild(0);
        m_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        canMove = true;
        reduce_speed = 1.0f;

        // Handles the combos for the attacks
        if (comboParams == null || (comboParams != null && comboParams.Length == 0))
            comboParams = new string[] { "Attack1", "Attack2", "Attack3" };
    }

    void FixedUpdate()
    {
        if (actions)
            Move();
    }

    void Update()
    {
        if (actions)
        {
            Roll();
            AttackCombo();
            Block();
        }
    }

    //------------------------------------Fighting-------------------------------------//
    void AttackCombo()
    {
        if (Input.GetButtonDown("Fire1") && comboIndex < comboParams.Length)
        {
            reduce_speed = 20.0f;
            m_Animator.SetTrigger(comboParams[comboIndex]);

            // If combo must not loop
            comboIndex++;

            // If combo can loop
            //comboIndex = (comboIndex + 1) % comboParams.Length ;
            resetTimer = 0f;
        }

        // Reset combo if the user has not clicked quickly enough
        if (comboIndex > 0)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > fireRate)
            {
                comboIndex = 0;
            }
        }
    }

    void Block()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            m_Animator.SetBool("IsBlocking", true);
            canMove = false;
        }
        else
        {
            m_Animator.SetBool("IsBlocking", false);
            canMove = true;
        }
    }

    //-----------------------------------------------Movement---------------------------------------//

    /*Handles the movement and jump*/
    void Move()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        
        // makes the falling animation when is not grounded and isnt jumping
        bool isWalking = false;
        m_Animator.SetBool("IsFalling", !cc.isGrounded && !_isGrounded);

        if (_isGrounded || cc.isGrounded)
        {
            jumpSpeed = 1.0f;
            m_Animator.SetBool("IsJumping", false);
            // We are grounded, so recalculate
            // move direction directly from axes
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Movement = new Vector3(horizontal, 0.0f, vertical);
            m_Movement *= speed;

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();

            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

            isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);

            Vector3 current = processWalking();

            if (Input.GetButton("Jump"))
            {
                jumpSpeed = 4.8f;
                m_Animator.SetBool("IsJumping", true);
                reduce_speed = 5.0f;
                speedOnJump(current);
                m_Movement.y = jumpSpeed;
            }

            Vector3 NextDir = Vector3.RotateTowards(current, m_Movement, turnSpeed * Time.deltaTime, 0f).normalized;

            if (NextDir != Vector3.zero)
            {
                if (checkRotation(current))
                    rotateCharacter(current);
                else
                    cc.transform.rotation = Quaternion.LookRotation(NextDir);
            }

            if (isWalking && canMove)
                cc.Move(NextDir / 8 / reduce_speed);

        }
        else
        {
            m_Animator.SetBool("IsWalking", false);
            // Apply gravity
            m_Movement.y -= gravity * Time.deltaTime;

            // Move the controller
            if (canMove)
                cc.Move(m_Movement * Time.deltaTime * jumpSpeed);
        }

    }
    /** changes the current depending of the key pressed(W,A,S,D)
    */
    Vector3 processWalking()
    {
        // Follows where the camera is pointing
        Vector3 camF = cam.forward;
        Vector3 camR = cam.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;

        if (Input.GetKey(KeyCode.W))
            return camF;
        else if (Input.GetKey(KeyCode.A))
            return camR * -1;
        else if (Input.GetKey(KeyCode.S))
            return camF * -1;
        else if (Input.GetKey(KeyCode.D))
            return camR;

        return Vector3.zero;
    }

    void rotateCharacter(Vector3 current)
    {
        Debug.Log(Vector3.Dot(Vector3.right, m_Movement));
        bool can_move_side_ways = Vector3.Dot(Vector3.right, m_Movement) > 0.1;
        //aqui ver como impossibilitar o caracter de mover estranhamente para direita ou esquerda
        if (Vector3.Dot(Vector3.forward, m_Movement) >= 0)
        {
            if (Input.GetKey(KeyCode.A))
                cc.transform.Rotate(Vector3.up * turnSpeed * -30.0f * Time.deltaTime);
            else if (Input.GetKey(KeyCode.D))
                cc.transform.Rotate(Vector3.up * turnSpeed * 30.0f * Time.deltaTime);

            processRightOrLeft();
        }
        else if (Vector3.Dot(Vector3.forward, m_Movement) < 0)
        {
            if (Input.GetKey(KeyCode.A))
                cc.transform.Rotate(Vector3.up * turnSpeed * 30.0f * Time.deltaTime);
            else if (Input.GetKey(KeyCode.D))
                cc.transform.Rotate(Vector3.up * turnSpeed * -30.0f * Time.deltaTime);

            processRightOrLeft();
        }

    }

    void processRightOrLeft()
    {
        if (Vector3.Dot(Vector3.right, m_Movement) >= 0)
        {// moving right
            if (Input.GetKey(KeyCode.W))
                cc.transform.Rotate(Vector3.up * turnSpeed * -30.0f * Time.deltaTime);
            else if (Input.GetKey(KeyCode.S))
                cc.transform.Rotate(Vector3.up * turnSpeed * 30.0f * Time.deltaTime);
        }
        else if (Vector3.Dot(Vector3.right, m_Movement) < 0)
        {// moving left
            if (Input.GetKey(KeyCode.W))
                cc.transform.Rotate(Vector3.up * turnSpeed * 30.0f * Time.deltaTime);
            else if (Input.GetKey(KeyCode.S))
                cc.transform.Rotate(Vector3.up * turnSpeed * -30.0f * Time.deltaTime);
        }
    }

    bool checkRotation(Vector3 current)
    {
        //checks if cc is walking backwards or frontwards
        bool limit_rotation_front = Mathf.Abs(Vector3.Dot(m_Movement, Vector3.right)) == 0;
        //checks if cc is walking sideways
        bool limit_rotation_sideways = Mathf.Abs(Vector3.Dot(m_Movement, Vector3.forward)) == 0;
        if (!limit_rotation_sideways && !limit_rotation_front)
            return true;
        return false;
    }

    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            m_Animator.SetTrigger("Roll");
    }

    void speedOnJump(Vector3 current)
    {
        if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
        {
            m_Movement = Vector3.RotateTowards(current, m_Movement, turnSpeed * Time.deltaTime, 0f).normalized;
        }
    }

    public void DamageAnimation(){
        m_Animator.SetTrigger("Damaged");
    }

    public Animator GetAnimator(){
        return m_Animator;
    }

    public void StopCharacter(bool isActive, bool isDead)
    {
        actions = isActive;
        if(isDead){
            m_Animator.SetBool("IsWalking",false);
            m_Animator.SetTrigger("Dead");
        }
    }

}