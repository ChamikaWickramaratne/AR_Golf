using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfGameManager : MonoBehaviour
{
    public static GolfGameManager Instance { get; private set; }
    public int playerCount = 2;
    public GameObject golfBallPrefab;
    public Text currentPlayerText;
    public Text[] strokeCountTexts;

    private List<GameObject> _balls = new List<GameObject>();
    private int[] _strokes;
    private int _currentPlayerIndex = 0;
    private ARPathBuilder _pathBuilder;
    private Vector3 _teePosition;

    void Awake()
    {
        //set initial values
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerCount = GameSettings.PlayerCount;
        _strokes = new int[playerCount];
        _pathBuilder = FindObjectOfType<ARPathBuilder>();
    }

    public void SetupHole(Vector3 teePosition)
    {
        //start game. teePosition = ball position
        _teePosition = teePosition;
        foreach (var b in _balls)
            Destroy(b);
        _balls.Clear();
        _strokes = new int[playerCount];
        _currentPlayerIndex = 0;

        //initiate balls for each player
        for (int i = 0; i < playerCount; i++)
        {
            var ball = Instantiate(golfBallPrefab, teePosition, Quaternion.identity);
            ball.name = $"Player{i+1}_Ball";
            _balls.Add(ball);
            //only activate player 1
            ball.SetActive(i == _currentPlayerIndex);
            var rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep(); 
            }
        }

        UpdateUI();
    }

    public void RegisterStroke()
    {
        //get strike for score calculation
        _strokes[_currentPlayerIndex]++; 
        UpdateUI();
    }

    public void OnBallInHole()
    {
        //destroy ball when its in hole and remove that player from active player list
        Destroy(_balls[_currentPlayerIndex]);
        _balls[_currentPlayerIndex] = null;
        int nextIndex = -1;
        for (int i = _currentPlayerIndex + 1; i < playerCount; i++)
        {
            if (_balls[i] != null)
            {
                nextIndex = i;
                break;
            }
        }
        //if everyone has made it in hole, end game
        if (nextIndex < 0)
        {
            if (currentPlayerText != null)
                currentPlayerText.text = "Game Complete!";
            return;
        }
        //change player turn
        _currentPlayerIndex = nextIndex;
        var nextBall = _balls[_currentPlayerIndex];
        nextBall.transform.position = _teePosition;
        nextBall.transform.rotation = Quaternion.identity;
        nextBall.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {  
        //change UI with score and player turn
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
        //change turn after strike
        _balls[_currentPlayerIndex].SetActive(false);
        _currentPlayerIndex = (_currentPlayerIndex + 1) % playerCount;
        var next = _balls[_currentPlayerIndex];
        next.transform.position = _balls[_currentPlayerIndex].transform.position;
        next.transform.rotation = Quaternion.identity;
        next.SetActive(true);
        UpdateUI();
    }

    public void ResetGame()
    {
        //reset everything to starting value
        foreach (var ball in _balls)
            if (ball != null) Destroy(ball);
        _balls.Clear();
        _strokes = new int[playerCount];
        _currentPlayerIndex = 0;
        UpdateUI();
    }

}
