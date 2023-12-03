using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private GameObject snowParticle;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float idleOscillationSpeed = 0.01f;
    [SerializeField] private float maxOscillationDistance = 0.2f;

    private CharacterController characterController;
    private Animator animator;
    private float axisDirection = 0f;
    private Vector3 velocity;

    private bool isJumping;

    private PlayerTrack currentTrack = PlayerTrack.Middle;
    private float targetZ;
    private bool isSwitchingTrack = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerSpeed = GameManager.Instance.playerSpeed;
    }

    void Update()
    {

        if (characterController.isGrounded)
        {
            GameManager.Instance.playerSpeed += 0.02f * Time.deltaTime;
            playerSpeed = GameManager.Instance.playerSpeed;
            HandleMovementInput();
            AlignToGround();
            if (Input.GetKeyDown(KeyCode.Space) && !isSwitchingTrack)
            {
                Jump();
            }
            snowParticle.SetActive(true);
            if (isSwitchingTrack)
            {
                MoveAlongZ();
            }
            else
            {
                if (targetZ == 0) targetZ = GameManager.Instance.playerTrackMiddleZ;
                OscillateIdleParameter();
            }

        }
        else
        {
            snowParticle.SetActive(false);
            float verticalSpeed = velocity.y;
            verticalSpeed += Physics.gravity.y * 3 * Time.deltaTime;

            velocity.y = verticalSpeed;
        }

        characterController.Move(new Vector3(playerSpeed, 0f, axisDirection * turnSpeed) * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);

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

        if (Vector3.Distance(transform.position, targetPosition) < 0.3f)
        {
            isSwitchingTrack = false;
            axisDirection = 0f;
            animator.SetBool("IsSwitchingLeft", false);
            animator.SetBool("IsSwitchingRight", false);
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
        if (isJumping)
        {
            StartCoroutine(CheckGroundedAfterDelay());
        }
    }

    IEnumerator CheckGroundedAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        if (characterController.isGrounded)
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
        }
    }

    void OscillateIdleParameter()
    {
        float delta = 0.2f;
        float frequency = 1f;

        float oscValue = Mathf.PingPong(Time.time * frequency, 1.0f) * 2.0f - 1.0f;
        float targetPositionZ = Mathf.Lerp((targetZ - delta), (targetZ + delta), (oscValue + 1f) / 2f);

        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetPositionZ);
        characterController.Move((targetPosition - transform.position) * Time.deltaTime * 5f);

        animator.SetFloat("SkiingOscillation", (oscValue + 1f) / 2f);
    }


}


public enum PlayerTrack
{
    Left,
    Middle,
    Right
}




