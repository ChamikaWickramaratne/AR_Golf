using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Tooltip("Color to apply to the ball when it hits the hole")]
    public Color hitColor = Color.green;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("GolfBall"))
            FindObjectOfType<GolfGameManager>()?.OnBallInHole();;
    }
}
