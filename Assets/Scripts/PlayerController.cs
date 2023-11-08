using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float carveAngle;
    private float currentRotation = 0f; // Track the current rotation
    private float axisDirection;
    private TurningDirection previousDirection;

    private enum TurningDirection { None, Left, Right };
    private TurningDirection turningDirection;
    private Vector3 moveDirection;
    private Vector3 velocity; // Keep track of gravity/jumping.

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    private CharacterController characterController;
    private Animator animator;
    private float previousY; // Track previous position for calculating vertical speed

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        turningDirection = TurningDirection.None; //Set no rotation on start up e.g. go forward.
        previousDirection = TurningDirection.None;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        AdaptiveForce();

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                axisDirection = 1f; // Left & Right returns -1 or 1 if left or right is pressed
                turningDirection = TurningDirection.Left;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                axisDirection = -1f; // Left & Right returns -1 or 1 if left or right is pressed
                turningDirection = TurningDirection.Right;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            switch (turningDirection)
            {
                case TurningDirection.Left:
                    moveDirection = new Vector3(0, 0, axisDirection) * turnSpeed;
                    TurnLeft();
                    break;
                case TurningDirection.Right:
                    moveDirection = new Vector3(0, 0, axisDirection) * turnSpeed;
                    TurnRight();
                    break;
                default:
                    break;
            }

            previousDirection = turningDirection;
        }

        // Update the vertical speed based on the change in position
        float currentY = transform.position.y;
        float verticalSpeed = (currentY - previousY) / Time.deltaTime;
        verticalSpeed = Mathf.Clamp01(verticalSpeed);
        previousY = currentY;

        // Update the verticalSpeed parameter in the Animator
        animator.SetFloat("VerticalSpeed", verticalSpeed);
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void AdaptiveForce()
    {
        characterController.Move(new Vector3(playerSpeed, 0f, 0f) * Time.deltaTime); // Move the player down the slope. 
    }

    void TurnLeft()
    {
        if (turningDirection != previousDirection)
        {
            currentRotation = 0f; // Reset current rotation if changing direction
        }

        Quaternion targetRotation = Quaternion.Euler(carveAngle, 0f, 0f);

        // Gradually increase the rotation until it reaches 8 degrees
        if (currentRotation < 8f)
        {
            currentRotation += turnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, currentRotation / 8f);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void TurnRight()
    {
        if (turningDirection != previousDirection)
        {
            currentRotation = 0f; // Reset current rotation if changing direction
        }

        Quaternion targetRotation = Quaternion.Euler(-carveAngle, 0f, 0f);
        // Gradually increase the rotation until it reaches -8 degrees
        if (currentRotation > -8f)
        {
            currentRotation -= turnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Abs(currentRotation) / 8f);
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
    }
}
