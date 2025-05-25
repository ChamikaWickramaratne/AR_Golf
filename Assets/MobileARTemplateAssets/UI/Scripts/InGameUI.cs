using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public void BackToMenu()
    {
        Debug.Log("sssssssssssssssssssssssss");
        SceneManager.LoadScene("MainMenu");
    }
}
