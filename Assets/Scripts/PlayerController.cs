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
    private int jumpAnim;
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
            verticalSpeed += Physics.gravity.y * Time.deltaTime;

            velocity.y = verticalSpeed;
        }

     
        print(velocity.y);
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
                }
                else if (currentTrack == PlayerTrack.Right)
                {
                    currentTrack = PlayerTrack.Middle;
                    targetZ = GameManager.Instance.playerTrackMiddleZ;
                    isSwitchingTrack = true;
                }
                axisDirection = 1f;
                break;
            case PlayerTrack.Right:
                if (currentTrack == PlayerTrack.Middle)
                {
                    currentTrack = PlayerTrack.Right;
                    targetZ = GameManager.Instance.playerTrackRightZ;
                    isSwitchingTrack = true;
                }
                else if (currentTrack == PlayerTrack.Left)
                {
                    currentTrack = PlayerTrack.Middle;
                    targetZ = GameManager.Instance.playerTrackMiddleZ;
                    isSwitchingTrack = true;
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
        jumpAnim = Random.Range(1, 3);

        if (!isJumping)
        {
            print("Velocity y before jump" + velocity.y);
            animator.SetBool($"IsJumping{jumpAnim}", true);
            isJumping = true;
            playerSpeed = 0f;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
            print("Velocity y after jump: "+velocity.y);
        }
    }

    void OnAnimatorMove()
    {
        if (isJumping && characterController.isGrounded)
        {
            animator.SetBool($"IsJumping{jumpAnim}", false);
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

        animator.SetFloat("SkiingOscillation", (oscValue + 1f) / 2f); // Normalize the sine wave to [0, 1]
    }

}

public enum PlayerTrack
{
    Left,
    Middle,
    Right
}
