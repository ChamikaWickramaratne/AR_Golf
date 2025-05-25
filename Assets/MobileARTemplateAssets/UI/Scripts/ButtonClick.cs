using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public static ButtonClick Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🎯 Makes this object persistent
        }
        else
        {
            Destroy(gameObject); // 🗑 Prevent duplicates
        }
    }
}
