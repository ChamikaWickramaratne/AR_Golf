using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    public void LoadSinglePlayer()
    {
        SceneManager.LoadScene("testTouch");
    }

    public void LoadMultiplayer2Player()
    {
        GameSettings.PlayerCount = 2;
        SceneManager.LoadScene("SampleScene");
    }
    public void LoadMultiplayer3Player()
    {
        GameSettings.PlayerCount = 3;
        SceneManager.LoadScene("SampleScene");
    }
    public void LoadMultiplayer4Player()
    {
        GameSettings.PlayerCount = 4;
        SceneManager.LoadScene("SampleScene");
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
