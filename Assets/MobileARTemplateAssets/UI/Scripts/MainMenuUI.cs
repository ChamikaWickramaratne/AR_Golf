using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    
    public AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private Slider audioSlider;

    public void PlaySinglePlayer()
    {

        PlayClickSound();
        SceneManager.LoadScene("SinglePlayerGame");
    }

    public void PlayMultiplayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void PlayGameModeSelection()
    {
        PlayClickSound();
        SceneManager.LoadScene("GameModeSelect");
    }
    public void BackToMenu()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
        SceneManager.LoadScene("MainMenu");
    }
    
    


    public void QuitGame()
    {

        PlayClickSound();
        Application.Quit();
    }

    void PlayClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }
}
