using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : Counter, IInteractable
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;
    [SerializeField] private int _platesSpawnedAmountMax;
    [SerializeField] private float _spawnPlateTimerMax;
    private float _spawnPlateTimer;
    private int _platesSpawnedAmount;

    private void Update()
    {
        if(_spawnPlateTimer >= _spawnPlateTimerMax)
        {
            _spawnPlateTimer = 0;
            if(_platesSpawnedAmount < _platesSpawnedAmountMax)
            {
                _platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
                //KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, this);
            }
        }
        else
        {
            _spawnPlateTimer += Time.deltaTime;
        }
    }

    public void Interact(ThirdPersonCharacter character)
    {
        if (!character.HasKitchenObject())
        {
            if(_platesSpawnedAmount > 0)
            {
                _platesSpawnedAmount--;
                KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, character);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }

}
