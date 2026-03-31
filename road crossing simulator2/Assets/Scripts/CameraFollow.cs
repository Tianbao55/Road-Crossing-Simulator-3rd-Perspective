using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -4f); // behind & above
    public float followSpeed = 5f;                      // camera movement smoothness
    public float rotationSmoothTime = 0.1f;            // smooth rotation
    public float flipSmoothTime = 1f;

    [Header("Mouse/Look Settings")]
    public float mouseSensitivity = 2f;
    public float minPitch = -30f;                       // looking down limit
    public float maxPitch = 60f;                        // looking up limit

    private float yaw;                                  // horizontal rotation
    private float pitch;                                // vertical rotation
    private float yawSmooth;
    private float pitchSmooth;
    private float yawVelocity;
    private float pitchVelocity;
    private float targetSmoothTime = 0.1f;

    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. Get mouse input ---
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity; // invert Y if you like
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        rotationSmoothTime = Mathf.Lerp(rotationSmoothTime, targetSmoothTime, Time.deltaTime * 5f);

        // --- 2. Smooth rotation ---
        yawSmooth = Mathf.SmoothDamp(yawSmooth, yaw + target.eulerAngles.y, ref yawVelocity, rotationSmoothTime);
        pitchSmooth = Mathf.SmoothDamp(pitchSmooth, pitch, ref pitchVelocity, rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(pitchSmooth, yawSmooth, 0);

        // --- 3. Desired camera position ---
        Vector3 desiredPosition = target.position + rotation * offset;

        // --- 4. Smooth follow ---
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // --- 5. Look at target (slightly above center) ---
        transform.LookAt(target.position + Vector3.up * 1.2f);


    }

    public IEnumerator FlipSmoothCoroutine(float duration)
    {
        targetSmoothTime = flipSmoothTime;
        yield return new WaitForSeconds(duration);
        targetSmoothTime = 0.1f;
    }
}
