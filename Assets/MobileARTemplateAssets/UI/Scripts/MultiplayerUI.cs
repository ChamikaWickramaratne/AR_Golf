using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerUI : MonoBehaviour
{
    public void BackToMainMenu()
    {
        Debug.Log("tttttttttttttttttttttttt");
        SceneManager.LoadScene("MainMenu");
    }
}
