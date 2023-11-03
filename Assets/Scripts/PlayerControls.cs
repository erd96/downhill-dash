//using UnityEngine;

//public class PlayerControls : MonoBehaviour
//{
//    public float speed;
//    private float yVelocity;
//    public CharacterController player;
//    public float jumpForce = 10.0f;
//    public float moveForce = 5.0f;
//    public float gravity = 1.0f;

//    private void Start()
//    {
//        player = GetComponent<CharacterController>();
//    }

//    void Update()
//    {
//        Vector3 direction = new Vector3(1, 0, 0);
//        Vector3 velocity = direction * speed;

//        // Add left/right movement
//        if (Input.GetKey(KeyCode.LeftArrow))
//        {
//            velocity += transform.TransformDirection(Vector3.left) * moveForce;
//        }
//        else if (Input.GetKey(KeyCode.RightArrow))
//        {
//            velocity += transform.TransformDirection(Vector3.right) * moveForce;
//        }

//        if (player.isGrounded)
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//                yVelocity = jumpForce;
//            }
//        }
//        else
//        {
//            yVelocity -= gravity;
//        }

//        velocity.y = yVelocity;
//        player.Move(velocity * Time.deltaTime);
//    }
//}

using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed;
    private float yVelocity;
    public CharacterController player;
    public float jumpForce = 10.0f;
    public float moveForce = 1.0f;
    public float gravity = 1.0f;

    private bool movingLeft;
    private bool movingRight;

    private void Start()
    {
        player = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 direction = new Vector3(1, 0, 0);
        Vector3 velocity = direction * speed;

        if (movingLeft)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movingLeft = false;
                movingRight = true;
            }
            else
            {
                velocity += transform.TransformDirection(Vector3.left) * moveForce;
            }
        }
        else if (movingRight)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movingRight = false;
                movingLeft = true;
            }
            else
            {
                velocity += transform.TransformDirection(Vector3.right) * moveForce;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movingLeft = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movingRight = true;
            }
        }

        if (player.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = jumpForce;
            }
        }
        else
        {
            yVelocity -= gravity;
        }

        velocity.y = yVelocity;
        player.Move(velocity * Time.deltaTime);
    }
}