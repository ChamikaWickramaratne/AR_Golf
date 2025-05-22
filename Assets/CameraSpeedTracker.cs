using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSpeedTracker : MonoBehaviour
{
    [Tooltip("Smoothed camera speed (m/s)")]
    public float smoothedSpeed;

    [Tooltip("How quickly to smooth sudden jumps (0 = no smoothing, 1 = infinite smoothing)")]
    [Range(0, 1)] public float smoothing = 0.1f;

    private Vector3 _lastPosition;
    private float _currentSpeed;

    void Start()
    {
        _lastPosition = transform.position;
    }

    void Update()
    {
        // 1) Compute raw speed
        Vector3 delta = transform.position - _lastPosition;
        float rawSpeed = delta.magnitude / Time.deltaTime;

        // 2) Smooth it a bit (optional)
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, 1 - smoothing);

        // 3) Cache for next frame
        _lastPosition = transform.position;
    }
}
