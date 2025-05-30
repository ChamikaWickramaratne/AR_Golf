using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ClubHit : MonoBehaviour
{
    public float baseHitMultiplier = 1.0f;
    public float stopThreshold = 0.01f;
    private CameraSpeedTracker _speedTracker;
    private Collider _clubCollider;
    private bool _hasStruck = false;
    private Rigidbody _ballRb;

    void Awake()
    {
        _speedTracker = GetComponentInParent<CameraSpeedTracker>();
        _clubCollider = GetComponent<Collider>();
        _clubCollider.isTrigger = false;
    }

    void Update()
    {
        //on hit 
        if (_hasStruck && _ballRb != null)
        {
            if (_ballRb.velocity.sqrMagnitude < stopThreshold * stopThreshold)
            {
                //make ball active when it stops
                _clubCollider.enabled = true;
                GolfGameManager.Instance.AdvanceTurn();
                _hasStruck = false;
                _ballRb = null;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
{
    if (_hasStruck) return;  

    if (collision.collider.CompareTag("GolfBall"))
    {
        //calculate force with camera speed and hit direction
        _ballRb = collision.collider.attachedRigidbody;
        if (_ballRb == null) return;
        Vector3 contactNormal = collision.GetContact(0).normal;
        Vector3 hitDir = -contactNormal;
        float camSpeed = _speedTracker != null ? _speedTracker.smoothedSpeed : 1f;
        float strength = baseHitMultiplier * camSpeed;
        _ballRb.AddForce(hitDir * strength, ForceMode.Impulse);
        _hasStruck = true;
        //disable collider when first hit
        _clubCollider.enabled = false;
        FindObjectOfType<GolfGameManager>()?.RegisterStroke();
    }
}

}
