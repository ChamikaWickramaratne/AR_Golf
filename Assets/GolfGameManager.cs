using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfGameManager : MonoBehaviour
{
    public static GolfGameManager Instance { get; private set; }

    [Header("Game Settings")]
    [Tooltip("Number of players / balls")]
    public int playerCount = 2;

    [Tooltip("Prefab for the golf ball (must have Rigidbody, Collider, Tag = \"GolfBall\")")]
    public GameObject golfBallPrefab;

    [Header("UI (optional)")]
    [Tooltip("UI Text showing which player's turn it is")]
    public Text currentPlayerText;
    [Tooltip("One Text element per player to show their stroke count")]
    public Text[] strokeCountTexts;

    // Internal state
    private List<GameObject> _balls = new List<GameObject>();
    private int[] _strokes;
    private int _currentPlayerIndex = 0;
    private ARPathBuilder _pathBuilder;
    private Vector3 _teePosition;

    void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerCount = GameSettings.PlayerCount;

        // Initialize stroke counts array
        _strokes = new int[playerCount];
        _pathBuilder = FindObjectOfType<ARPathBuilder>();
    }

    public void SetupHole(Vector3 teePosition)
    {
        _teePosition = teePosition;
        // Clean up any existing balls
        foreach (var b in _balls)
            Destroy(b);
        _balls.Clear();

        // Reset stroke counts and player index
        _strokes = new int[playerCount];
        _currentPlayerIndex = 0;

        // Spawn balls
        for (int i = 0; i < playerCount; i++)
        {
            var ball = Instantiate(golfBallPrefab, teePosition, Quaternion.identity);
            ball.name = $"Player{i+1}_Ball";
            _balls.Add(ball);

            // Only the current player's ball is active at a time
            ball.SetActive(i == _currentPlayerIndex);
            var rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();  // optional: force it to sleep until a wake-up event
            }
        }

        UpdateUI();
    }

    public void RegisterStroke()
    {
        _strokes[_currentPlayerIndex]++; 
        UpdateUI();
    }

    public void OnBallInHole()
    {
        // Destroy current player's ball
        // Destroy(_balls[_currentPlayerIndex]);
        Destroy(_balls[_currentPlayerIndex]);
        _balls[_currentPlayerIndex] = null;

        // Find next player with a ball
        int nextIndex = -1;
        for (int i = _currentPlayerIndex + 1; i < playerCount; i++)
        {
            if (_balls[i] != null)
            {
                nextIndex = i;
                break;
            }
        }

        // No more players => game over
        if (nextIndex < 0)
        {
            Debug.Log("All players have finished. Game over.");
            if (currentPlayerText != null)
                currentPlayerText.text = "Game Complete!";
            return;
        }

        // Advance to next player
        _currentPlayerIndex = nextIndex;
        var nextBall = _balls[_currentPlayerIndex];
        nextBall.transform.position = _teePosition;
        nextBall.transform.rotation = Quaternion.identity;
        nextBall.SetActive(true);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentPlayerText != null)
        {
            currentPlayerText.text = $"Player {_currentPlayerIndex + 1}'s Turn";
        }

        if (strokeCountTexts != null)
        {
            for (int i = 0; i < strokeCountTexts.Length && i < playerCount; i++)
            {
                strokeCountTexts[i].text = $"P{i + 1}: {_strokes[i]}";
            }
        }
    }

    public void AdvanceTurn()
    {
        // Hide current ball
        _balls[_currentPlayerIndex].SetActive(false);

        // Next player (wrap if needed)
        _currentPlayerIndex = (_currentPlayerIndex + 1) % playerCount;

        // Place next ball back at tee
        var next = _balls[_currentPlayerIndex];
        next.transform.position = _balls[_currentPlayerIndex].transform.position;
        next.transform.rotation = Quaternion.identity;
        next.SetActive(true);

        UpdateUI();
    }

    public void ResetGame()
    {
        // 1) Clear & hide all balls
        foreach (var ball in _balls)
            if (ball != null) Destroy(ball);
        _balls.Clear();

        // 3) Reset strokes & UI
        _strokes = new int[playerCount];
        _currentPlayerIndex = 0;
        UpdateUI();
    }

}
