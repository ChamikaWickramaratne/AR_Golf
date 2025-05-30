using UnityEngine;

public class ClubToggle : MonoBehaviour
{
    public GameObject club;
    public Transform clubTransform;
    public float defaultZOffset = 0.3f;

    public void ToggleClubActive()
    {
        if (club == null)
        {
            return;
        }
        //get if club is active or not and set the opposite value
        bool isActive = club.activeSelf;
        club.SetActive(!isActive);
    }

    public void OnDepthSliderChanged(float sliderValue)
    {
        //change club z position on slider change
        if (clubTransform == null) return;

        Vector3 localPos = clubTransform.localPosition;
        localPos.z = defaultZOffset + sliderValue;
        clubTransform.localPosition = localPos;
    }
}
