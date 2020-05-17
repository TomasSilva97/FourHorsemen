
using UnityEngine;

public class Testing : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float rotateSpeed = 8.0F;

    Quaternion m_Rotation = Quaternion.identity;

    private Vector3 moveDirection = Vector3.zero;
    Animator m_Animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            moveDirection.Normalize();

            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

            bool isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                m_Animator.SetTrigger("Jump");
            }
            Vector3 NextDir = Vector3.RotateTowards(transform.forward, moveDirection, rotateSpeed * Time.deltaTime, 0f);

            if (NextDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(NextDir);

            if (isWalking)
            {
                characterController.Move(NextDir / 8);
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        //Tratar do bug em que está andar enquanto cai.
    }


}
