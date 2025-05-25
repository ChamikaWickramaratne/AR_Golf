using UnityEngine;

public class ClubToggle : MonoBehaviour
{
    [Tooltip("Drag your club GameObject here (the child of AR Camera)")]
    public GameObject club;

    [Tooltip("The club GameObject (child of the AR camera)")]
    public Transform clubTransform;

    [Tooltip("Default local Z offset when slider is at 0")]
    public float defaultZOffset = 0.3f;

    /// <summary>
    /// Call this from your UI Button's OnClick.
    /// </summary>
    public void ToggleClubActive()
    {
        if (club == null)
        {
            Debug.LogWarning("ClubToggle: no club assigned.");
            return;
        }

        bool isActive = club.activeSelf;
        club.SetActive(!isActive);
        Debug.Log($"Club is now {(!isActive ? "ENABLED" : "DISABLED")}");
    }

    /// <summary>
    /// This gets called by the UI Slider's OnValueChanged(float)
    /// </summary>
    public void OnDepthSliderChanged(float sliderValue)
    {
        if (clubTransform == null) return;

        Vector3 localPos = clubTransform.localPosition;
        localPos.z = defaultZOffset + sliderValue;
        clubTransform.localPosition = localPos;
        Debug.Log("Slider value: " + clubTransform.localPosition + " (" + sliderValue + ")");
    }
}
