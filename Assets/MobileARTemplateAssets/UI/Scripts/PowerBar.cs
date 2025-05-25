using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Slider powerSlider;
    public float maxPower = 10f;
    public bool charging = false;

    void Update()
    {
        if (charging)
        {
            powerSlider.value += Time.deltaTime * 3;
            if (powerSlider.value >= powerSlider.maxValue)
                powerSlider.value = 0;
        }

        if (Input.GetMouseButtonDown(0)) charging = true;
        if (Input.GetMouseButtonUp(0))
        {
            charging = false;
            float shootPower = powerSlider.value / powerSlider.maxValue * maxPower;
            Debug.Log("Shoot with power: " + shootPower);
            powerSlider.value = 0;
        }
    }
}
