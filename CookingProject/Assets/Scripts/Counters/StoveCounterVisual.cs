using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    [SerializeField] private GameObject _stoveParticles;
    [SerializeField] private GameObject _stoveObject;

    private void Start()
    {
        _stoveCounter.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(StoveCounter.State state)
    {
        bool showVisual = state == StoveCounter.State.Frying || state == StoveCounter.State.Fried;
        _stoveParticles.SetActive(showVisual);
        _stoveObject.SetActive(showVisual);
    }
}
