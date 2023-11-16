using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private GameObject snowParticle1;
    [SerializeField] private GameObject snowParticle2;
    [SerializeField] private float jumpHeight;

    private CharacterController characterController;
    private Animator animator;
    private float axisDirection = 0f;
    private float currentMovement = 0.5f;
    private float targetMovement = 0.5f;
    private float movementChangeSpeed = 1f;
    private int jumpAnim;
    [SerializeField] private float rotationSpeed = 50f; 
    private Vector3 velocity;

    private bool isJumping;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerSpeed = GameManager.Instance.playerSpeed;
    }

    void Update()
    {
        GameManager.Instance.playerSpeed += 0.02f * Time.deltaTime;
        playerSpeed = GameManager.Instance.playerSpeed;
        

        if (characterController.isGrounded)
        {
            HandleMovementInput();
            AlignToGround();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            // Rotate the player based on the current rotation value
            //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
        }

        // Apply gravity using Unity's built-in gravity
        float verticalSpeed = velocity.y;
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        velocity.y = verticalSpeed;

        // Move the character using CharacterController
        characterController.Move(new Vector3(playerSpeed, 0f, axisDirection * turnSpeed) * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            targetMovement = 1f;
            axisDirection = Mathf.MoveTowards(axisDirection, 1f, turnSpeed * Time.deltaTime);

            if (transform.eulerAngles.y > 60f)
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }

        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            targetMovement = 0f;
            axisDirection = Mathf.MoveTowards(axisDirection, -1f, turnSpeed * Time.deltaTime);

            if (transform.eulerAngles.y < 120f)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

        }
        else
        {
            targetMovement = 0.5f;
            axisDirection = Mathf.MoveTowards(axisDirection, 0f, turnSpeed * Time.deltaTime);
            if (transform.eulerAngles.y < 90f)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else if (transform.eulerAngles.y > 90f)
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }

        }
        currentMovement = Mathf.MoveTowards(currentMovement, targetMovement, movementChangeSpeed * Time.deltaTime);
        animator.SetFloat("Movement", currentMovement);
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
            animator.SetBool($"IsJumping{jumpAnim}", true);
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        }
    }

    // Called by Unity after processing animation movements
    void OnAnimatorMove()
    {
        if (isJumping && characterController.isGrounded)
        {
            animator.SetBool($"IsJumping{jumpAnim}", false);
            isJumping = false;
        }
    }
}
