using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public Color hitColor = Color.green;

    void OnTriggerEnter(Collider other)
    {
        //call onBallInHole in game manager when the golf ball is in the hole
        if (other.CompareTag("GolfBall"))
            FindObjectOfType<GolfGameManager>()?.OnBallInHole();;
    }
}
