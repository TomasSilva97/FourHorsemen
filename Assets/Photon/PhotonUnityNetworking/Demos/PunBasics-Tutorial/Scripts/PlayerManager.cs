// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;


namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {

        // Private variables
        bool _isGrounded = true, actions = true;
        Transform _groundChecker;
        bool canMove;
        int comboIndex = 0;
        float resetTimer, reduce_speed; // used to reduce speed when attacking or after hitting the ground
        Vector3 m_Movement;
        Quaternion m_Rotation = Quaternion.identity;
        Animator m_Animator;
        Rigidbody rb;
        CharacterController cc; // controls the Character

        // Public variables
        Camera cam;
        public float speed = 3.0f; // Chacater speed
        public float turnSpeed = 8f; // Chacater rotation speed
        public float jumpSpeed = 4.8f; // Character jump speed
        public float gravity = 30.0f; // gravity applied to the Character
        public float fireRate = 1.0f; // rate of the attack
        public int forceConst = 50; // force applied
        public string[] comboParams;
        public LayerMask Ground;
        public float GroundDistance = 0.2f;
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 100f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            // Private vars
            cam = Camera.main;
            _groundChecker = transform.GetChild(0);
            m_Animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            cc = GetComponent<CharacterController>();
            canMove = true;
            reduce_speed = 1.0f;

            // Handles the combos for the attacks
            if (comboParams == null || (comboParams != null && comboParams.Length == 0))
                comboParams = new string[] { "Attack1", "Attack2", "Attack3" };

            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            // Create the UI
            if (this.playerUiPrefab != null)
            {
                if (photonView.IsMine)
                {
                    GameObject _uiGo = Instantiate(this.playerUiPrefab);
                    _uiGo.transform.parent = this.transform;
                    //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                }
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }


        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        public void FixedUpdate()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
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

            Vector3 camF = cam.transform.forward;
            Vector3 camR = cam.transform.right;
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

        public void DamageAnimation()
        {
            m_Animator.SetTrigger("Damaged");
        }


        public Animator GetAnimator()
        {
            return m_Animator;
        }

        public void StopCharacter(bool isActive, bool isDead)
        {
            actions = isActive;
            if (isDead)
            {
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetTrigger("Dead");
            }
        }

        /*public Quaternion TargetRotation()
        {
            return m_Rotation;
        }*/

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }


            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            this.Health -= 0.1f;
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are interesting the player
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            this.Health -= 0.1f * Time.deltaTime;
        }


#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(-5f, 10f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.transform.parent = this.transform;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (actions)
            {
                Move();
                Roll();
                AttackCombo();
                Block();
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }



        #endregion
    }
}