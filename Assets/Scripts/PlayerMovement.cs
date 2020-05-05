using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * WARNING: DO NOT USE "ROOT MOTION" OR THIS WILL BREAK
 *
 */


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    CharacterController controller;
    public LayerMask walkable;

    private float charSpeed;
    public float runSpeed = 4.5f;
    private static Vector3 moveDirection;
    public float walkSpeed = 2;
    public float jumpForce = 12; //9
    public float gravityScale = 4; //2
    public Transform[] checkpoints;
    
    #region MonoBehaviour
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //walkable = LayerMask.NameToLayer("Walkable");
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        charSpeed = walkSpeed;
    }
    
    void Update()
    {
        RunOrWalk();
        Move();
        Animate();
    }

    private void Move()
    {
        // Declare movement vector
        moveDirection = new Vector3(Input.GetAxis("Horizontal") * charSpeed,
            moveDirection.y,
            Input.GetAxis("Vertical") * charSpeed);


        Vector3 rotated = Vector3.zero;


        // Set y-axis movement conditions (jumping, grounded, free-fall)
        #region jumping

        //Debug.Log(controller.isGrounded);
        if (controller.isGrounded == true)
        {
            //Debug.Log("WE ARE GROUNDED!");
            moveDirection.y = 0;
            moveDirection.y = moveDirection.y + Physics.gravity.y * gravityScale * Time.deltaTime;
            // rotated 45 deg to align controls to camera instead of world space:
        }
        else if (controller.isGrounded == false)
        {
            moveDirection.y = moveDirection.y + Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        #endregion

        rotated = Quaternion.Euler(0, -45, 0) * moveDirection;
        rotated *= Time.deltaTime;
        controller.Move(rotated);
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpForce;
            controller.Move(Vector3.up * moveDirection.y * Time.deltaTime);
        }
        
        Vector3 lookDir = new Vector3(rotated.x, 0, rotated.z);

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
        //Debug.Log("downward vel = " + moveDirection.y);
    }

    private void Animate()
    {
        // Set y-axis movement conditions (jumping, grounded, free-fall)
        #region jumping
        if (moveDirection.y > 0 && !controller.isGrounded )
        {
            animator.SetBool("isJumping", true);
        }

        else if (controller.isGrounded == false && moveDirection.y < 0)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        /*
        else if (IsGrounded() == false)
        {
            if (moveDirection.y < 0)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);

            }
        }
        */
        else if (controller.isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        #endregion
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

       
    }

    #endregion

    #region Methods
    /*
    bool IsGrounded()
    {
        if (controller.isGrounded)
        {
            Debug.Log("True");
            return true;
        }

        else
        {
            Debug.Log("False");
            return false;
        }
    }
    */
    void RunOrWalk()
    {
        if (Input.GetButton("Fire3"))
        {
            charSpeed = runSpeed;
            animator.SetBool("isRunning", true);
        }
        else
        {
            charSpeed = walkSpeed;
            animator.SetBool("isRunning", false);
        }
    }
    #endregion
}