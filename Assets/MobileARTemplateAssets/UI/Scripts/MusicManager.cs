using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

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
