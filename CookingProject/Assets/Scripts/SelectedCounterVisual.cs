using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject _selectedObj;
    private Counter _myCounter;

    private void Awake()
    {
        _myCounter = GetComponent<Counter>();
    }

    private void Start()
    {
        InteractionsManager.OnSelectedChanged += OnSelectedChanged;
    }

    private void OnSelectedChanged(Counter counter)
    {
        if (_myCounter == counter)
        {
            _selectedObj.SetActive(true);
        }
        else
        {
            _selectedObj.SetActive(false);
        }
    }
}
