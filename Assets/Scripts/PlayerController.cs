using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject snowParticle;
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float jumpHeight = 0.5f;



    public static PlayerController Instance;

    private CharacterController characterController;
    private Animator animator;
    private float axisDirection = 0f;
    private Vector3 velocity;

    private bool isJumping;

    private PlayerTrack currentTrack = PlayerTrack.Middle;
    private float targetZ;
    private bool isSwitchingTrack = false;
    private bool canMove = true;
    private bool isCheckGroundedCoroutineRunning = false;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerSpeed = GameManager.Instance.playerSpeed;
    }

    void Update()
    {
        float gModifier = 3f;
        float delta = 0.02f * Time.deltaTime;
        if (canMove)
        {
            if (characterController.isGrounded)
            {
                gModifier += delta / 10;
                GameManager.Instance.playerSpeed += delta;
                playerSpeed = GameManager.Instance.playerSpeed;
                HandleMovementInput();
                AlignToGround();
                if (jumpHeight > (delta / 10)) jumpHeight -= delta / 10;
                if (Input.GetKeyDown(KeyCode.Space)) Jump();

                snowParticle.SetActive(true);

            }
            else
            {
                snowParticle.SetActive(false);
                float verticalSpeed = velocity.y;
                verticalSpeed += Physics.gravity.y * gModifier * Time.deltaTime;

                velocity.y = verticalSpeed;
            }

            if (isSwitchingTrack)
            {
                MoveAlongZ();
            }

            characterController.Move(new Vector3(playerSpeed, 0f, axisDirection * turnSpeed) * Time.deltaTime);
            characterController.Move(velocity * Time.deltaTime);

        }


    }

    void HandleMovementInput()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && currentTrack != PlayerTrack.Left)
        {
            SwitchTrack(PlayerTrack.Left);
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && currentTrack != PlayerTrack.Right)
        {
            SwitchTrack(PlayerTrack.Right);
        }

    }

    void SwitchTrack(PlayerTrack targetTrack)
    {
        if (isSwitchingTrack) return;

        switch (targetTrack)
        {
            case PlayerTrack.Left:
                if (currentTrack == PlayerTrack.Middle)
                {
                    currentTrack = PlayerTrack.Left;
                    targetZ = GameManager.Instance.playerTrackLeftZ;
                    isSwitchingTrack = true;
                    animator.SetBool("IsSwitchingLeft", true);
                }
                else if (currentTrack == PlayerTrack.Right)
                {
                    currentTrack = PlayerTrack.Middle;
                    targetZ = GameManager.Instance.playerTrackMiddleZ;
                    isSwitchingTrack = true;
                    animator.SetBool("IsSwitchingLeft", true);
                }

                axisDirection = 1f;
                break;
            case PlayerTrack.Right:
                if (currentTrack == PlayerTrack.Middle)
                {
                    currentTrack = PlayerTrack.Right;
                    targetZ = GameManager.Instance.playerTrackRightZ;
                    isSwitchingTrack = true;
                    animator.SetBool("IsSwitchingRight", true);
                }
                else if (currentTrack == PlayerTrack.Left)
                {
                    currentTrack = PlayerTrack.Middle;
                    targetZ = GameManager.Instance.playerTrackMiddleZ;
                    isSwitchingTrack = true;
                    animator.SetBool("IsSwitchingRight", true);
                }
                axisDirection = -1f;
                break;
        }
    }

    void MoveAlongZ()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        characterController.Move(moveDirection * turnSpeed * Time.deltaTime);
     

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isSwitchingTrack = false;
            axisDirection = 0f;
            animator.SetBool("IsSwitchingLeft", false);
            animator.SetBool("IsSwitchingRight", false);
            //animator.SetBool("IsJumping", false);
        }
    }

    void AlignToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            Vector3 forwardDirection = Vector3.Cross(hit.normal, -transform.right);
            transform.rotation = Quaternion.LookRotation(forwardDirection, hit.normal);
        }
    }
    (string state, bool isColliding) CheckCollisionWithRail()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        float rayLength = characterController.height * 0.5f + 0.1f;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if (hit.transform.CompareTag("Rail"))
            {
                return ("IsOnRail", true);
            }
        }

        return ("IsJumping", false);
    }

    void Jump()
    {

        if (!isJumping)
        {
            animator.SetBool("IsJumping", true);
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        }
    }
    void OnAnimatorMove()
    {
        if (isJumping && !isCheckGroundedCoroutineRunning)
        {
            isCheckGroundedCoroutineRunning = true;
            StartCoroutine(CheckGroundedAfterDelay());
        }
    }

    IEnumerator CheckGroundedAfterDelay()
    {
        if (!characterController.isGrounded) yield return null;

        if (characterController.isGrounded)
        {
            (string state, bool isColliding) collisionInfo = CheckCollisionWithRail();
            if (collisionInfo.state == "IsJumping")
            {
                animator.SetBool(collisionInfo.state, collisionInfo.isColliding);
                isJumping = false;
            }
            else if (collisionInfo.state == "IsOnRail")
            { 
                animator.SetBool(collisionInfo.state, collisionInfo.isColliding);
            }
        }

        isCheckGroundedCoroutineRunning = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Obstacle1Z") || hit.transform.CompareTag("Osbtacle3Z"))
        {
            StartCoroutine(DeathTransition());
        }
    }

    IEnumerator DeathTransition()
    {
        animator.SetTrigger($"Fall{Random.Range(1, 3)}");
        canMove = false;
        SwapPlayerColliders();

        float elapsedTime = 0f;
        float duration = 0.5f; 
        Quaternion startRotation = virtualCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(75f, startRotation.eulerAngles.y, startRotation.eulerAngles.z);

        while (elapsedTime < duration)
        {
            virtualCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.transform.rotation = targetRotation; 

    }


    private void SwapPlayerColliders()
    {
        characterController.enabled = false;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<CapsuleCollider>().enabled = true;
    }
}


public enum PlayerTrack
{
    Left,
    Middle,
    Right
}



