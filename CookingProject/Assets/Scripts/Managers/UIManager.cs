using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _timeIsUpScreen;

    private void Start()
    {
        EventManager.Subscribe(EventManager.EventType.GameOver, OnGameOver);
    }

    private void OnGameOver(params object[] parameters)
    {
        _timeIsUpScreen.SetActive(true);
        var timeIsUpBtn = _timeIsUpScreen.transform.Find("Content").Find("Button").GetComponent<Button>();
        if (timeIsUpBtn) timeIsUpBtn.onClick.AddListener(delegate { EventManager.Trigger(EventManager.EventType.NewGame); });
    }
}