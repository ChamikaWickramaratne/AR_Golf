using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ClubHit : MonoBehaviour
{
    [Tooltip("Base multiplier for hit strength")]
    public float baseHitMultiplier = 1.0f;

    [Tooltip("Velocity threshold below which ball is considered stopped")]
    public float stopThreshold = 0.01f;

    private CameraSpeedTracker _speedTracker;
    private Collider _clubCollider;
    private bool _hasStruck = false;
    private Rigidbody _ballRb;

    void Awake()
    {
        _speedTracker = GetComponentInParent<CameraSpeedTracker>();
        if (_speedTracker == null)
            Debug.LogWarning("ClubHit: no CameraSpeedTracker found in parent hierarchy.");

        _clubCollider = GetComponent<Collider>();
        _clubCollider.isTrigger = false;
    }

    void Update()
    {
        // After first strike, watch the ball until it stops
        if (_hasStruck && _ballRb != null)
        {
            if (_ballRb.velocity.sqrMagnitude < stopThreshold * stopThreshold)
            {
                // Ball has essentially stopped: reset for next swing
                _clubCollider.enabled = true;
                GolfGameManager.Instance.AdvanceTurn();
                _hasStruck = false;
                _ballRb = null;
                Debug.Log("ClubHit: Ball stopped — collider re-enabled");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
{
    if (_hasStruck) return;  // only first strike

    if (collision.collider.CompareTag("GolfBall"))
    {
        _ballRb = collision.collider.attachedRigidbody;
        if (_ballRb == null) return;

        // Get the first contact point normal (points into the Club)
        Vector3 contactNormal = collision.GetContact(0).normal;

        // Fire the ball away from the club surface
        Vector3 hitDir = -contactNormal;

        float camSpeed = _speedTracker != null ? _speedTracker.smoothedSpeed : 1f;
        float strength = baseHitMultiplier * camSpeed;

        _ballRb.AddForce(hitDir * strength, ForceMode.Impulse);
        Debug.Log($"ClubHit: First strike. camSpeed={camSpeed:F2}, strength={strength:F2}, dir={hitDir}");

        _hasStruck = true;
        _clubCollider.enabled = false;
        FindObjectOfType<GolfGameManager>()?.RegisterStroke();
    }
}

}
