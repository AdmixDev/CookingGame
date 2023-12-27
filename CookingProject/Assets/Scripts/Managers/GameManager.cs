using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public System.Action<float> OnTimeUpdated;

    [Tooltip("Seconds")]
    [SerializeField] private float _gameDuration;
    [SerializeField] private GAME_STATE _currentGameState = GAME_STATE.Paused;

    private float _gameTimer;

    public GAME_STATE GAME_STATE;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        EventManager.Subscribe(EventManager.EventType.NewGame, NewGame);
        EventManager.Subscribe(EventManager.EventType.GameOver, OnGameOver);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EventManager.Trigger(EventManager.EventType.NewGame);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.Trigger(EventManager.EventType.GameOver);
        }

        if (_currentGameState == GAME_STATE.Playing)
        {
            if (_gameTimer < 0)
            {
                Debug.Log("Game Finished");
                EventManager.Trigger(EventManager.EventType.GameOver);
            }
            else
            {
                _gameTimer -= Time.deltaTime;
                OnTimeUpdated?.Invoke(_gameTimer / _gameDuration);
            }
        }
    }

    private void NewGame(params object[] parameters)
    {
        _currentGameState = GAME_STATE.Playing;
        _gameTimer = _gameDuration;
    }

    private void OnGameOver(params object[] parameters)
    {
        Time.timeScale = 0;
        _currentGameState = GAME_STATE.Finished;
    }
}

public enum GAME_STATE
{
    Playing,
    Paused,
    Finished,
}