using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    public GameObject scoreboardPanel;
    public TMP_Text scoreText;

    void Start()
    {
        scoreboardPanel.SetActive(false); // Hide on start

        // Pre-filled scores for display
        scoreText.text = "Jay: 300\nChamika: 290\nKshemendra: 280";
    }

    public void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);
    }

    public void HideScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }
}
