using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSpeedTracker : MonoBehaviour
{
    public float smoothedSpeed;
    [Range(0, 1)] public float smoothing = 0.1f;

    private Vector3 _lastPosition;
    private float _currentSpeed;

    void Start()
    {
        //set last position
        _lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - _lastPosition;
        float rawSpeed = delta.magnitude / Time.deltaTime;
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, 1 - smoothing);
        _lastPosition = transform.position;
    }
}
