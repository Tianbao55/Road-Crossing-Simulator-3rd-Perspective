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
    public GameParameter gameParameter;
    private int previousFacing = 1; // 1 = +X, -1 = -X
    public float spawnTriggerX = -300f;     // trigger point
    private bool hasTriggered = false;   // trigger once
    public float elapsedTime = 0f;
    private bool timerStarted = false;

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

            if (!hasTriggered && transform.position.x >= spawnTriggerX)
            {
                hasTriggered = true;
                timerStarted = true;

                if (gameParameter != null)
                {
                    int indexToUse = gameParameter.directionDropdown != null
                                     ? gameParameter.directionDropdown.value
                                     : 0;
                    switch (indexToUse)
                    {
                        case 0:
                            gameParameter.currentDirection = "Left";
                            if (gameParameter.carSpawner != null)
                                gameParameter.carSpawner.StartSpawning();
                            break;
                        case 1:
                            gameParameter.currentDirection = "Right";
                            if (gameParameter.carSpawner2 != null)
                                gameParameter.carSpawner2.StartSpawning();
                            break;
                        case 2:
                            gameParameter.currentDirection = "Dual";
                            if (gameParameter.carSpawner != null)
                                gameParameter.carSpawner.StartSpawning();
                            if (gameParameter.carSpawner2 != null)
                                gameParameter.carSpawner2.StartSpawning();
                            break;
                        case 3:
                            gameParameter.currentDirection = "Random";
                            int rand = Random.Range(0, 3); // 0 = Left, 1 = Right, 2 = Both

                            if (rand == 0)
                            {
                                if (gameParameter.carSpawner != null)
                                    gameParameter.carSpawner.StartSpawning();
                            }
                            else if (rand == 1)
                            {
                                if (gameParameter.carSpawner2 != null)
                                    gameParameter.carSpawner2.StartSpawning();
                            }
                            else // rand == 2
                            {
                                if (gameParameter.carSpawner != null)
                                    gameParameter.carSpawner.StartSpawning();
                                if (gameParameter.carSpawner2 != null)
                                    gameParameter.carSpawner2.StartSpawning();
                            }
                            break;
                    }

                    gameParameter.OnDirectionChanged(indexToUse);
                    Debug.Log("Triggered spawner with index " + indexToUse
                              + " at player x = " + transform.position.x);
                }


            }

        }
        if (timerStarted)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;

        if (other.CompareTag("Final"))
        {
            // Trigger the win condition in the GameManager
            GameManager.instance.StopGame(true, "win");
            Debug.Log("You Win!");
        }
        else if (other.CompareTag("Car"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false, "hit");
            Debug.Log("You Lose! Hit by right car.");
        }
        else if (other.CompareTag("CarLeft"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false, "hitl");
            Debug.Log("You Lose! Hit by left car.");
        }

        canMove = false;
        animator.SetBool("Walk", false);
    }
}
