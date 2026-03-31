using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.0f;
    public float minX = -1.3f; // Minimum X position
    public float maxX = 10f;  // Maximum X position

    public Animator animator;

    private bool canMove = true;
    public CameraFollow cameraFollow;
    private int previousFacing = 1; // 1 = +X, -1 = -X

    void Start()
    {
        // Get the Animator component attached to the character
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Initial animation state: Idle
        animator.SetBool("Walk", false);
    }

    void Update()
    {
        if (!canMove || !GameManager.instance.GameStart) return;
        float inputX = 0f;
        if (Input.GetKey(KeyCode.W))
            inputX = 1f;
        else if (Input.GetKey(KeyCode.S))
            inputX = -1f;

        // Check whether the character is moving
        bool isMoving = Mathf.Abs(inputX) > 0.01f;

        // Update animation state
        animator.SetBool("Walk", isMoving);

        if (isMoving)
        {
            int currentFacing = 0;

            if (inputX > 0)
                currentFacing = 1;   // Facing +X
            else if (inputX < 0)
                currentFacing = -1;  // Facing -X

            if (currentFacing != 0 && currentFacing != previousFacing)
            {
                cameraFollow.StartCoroutine(cameraFollow.FlipSmoothCoroutine(1f));
                previousFacing = currentFacing;
            }



            // Instantly rotate the character to face the movement direction
            if (inputX > 0)
            {
                // Face positive X direction
                transform.rotation = Quaternion.Euler(0, 0f, 0);
            }
            else if (inputX < 0)
            {
                // Face negative X direction
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            // Move the character along the X axis
            Vector3 movement = new Vector3(inputX, 0, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;

            // Clamp X position to limit left/right movement
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;

        if (other.CompareTag("Final"))
        {
            // Trigger the win condition in the GameManager
            GameManager.instance.StopGame(true);
            Debug.Log("You Win!");
        }
        else if (other.CompareTag("Car"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false);
            Debug.Log("You Lose!");
        }

        canMove = false;
        animator.SetBool("Walk", false);
    }
}
