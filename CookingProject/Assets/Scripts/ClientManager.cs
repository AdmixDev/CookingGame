using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;

    [SerializeField] private GameObject _clientPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _spawnRecipeTimerMax = 4;
    private float _spawnRecipeTimer;

    [SerializeField] private List<Table> _tables = new List<Table>();

    private bool _isDelivering;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        EventManager.Subscribe(EventManager.EventType.NewGame, NewGame);
        EventManager.Subscribe(EventManager.EventType.GameOver, GameOver);
    }

    private void Update()
    {
        if (_isDelivering)
        {
            if (_spawnRecipeTimer <= 0)
            {
                if (DeliveryManager.Instance.CanSpawnClient())
                {
                    SpawnClient();
                }
            }
            else
            {
                _spawnRecipeTimer -= Time.deltaTime;
            }
        }
    }

    private void SpawnClient()
    {
        _spawnRecipeTimer = _spawnRecipeTimerMax;
        var client = Instantiate(_clientPrefab, _spawnPoint);
    }

    public Table GetTable()
    {
        int random = Random.Range(0, _tables.Count);
        Table table = _tables[random];

        if (table.CanTakeTable())
        {
            return table;
        }

        return null;
    }

    private void NewGame(object[] parameters)
    {
        _isDelivering = true;
    }

    private void GameOver(object[] parameters)
    {
        _isDelivering = false;
        _spawnRecipeTimer = _spawnRecipeTimerMax;
    }
}
