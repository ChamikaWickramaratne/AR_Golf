using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    public void LoadSinglePlayer()
    {
        SceneManager.LoadScene("SinglePlayerGame");
    }

    public void LoadMultiplayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }
    public void BackToMenu()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
        SceneManager.LoadScene("MainMenu");
    }
    public void test()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
    }

}
