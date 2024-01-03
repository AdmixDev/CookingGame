using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : Counter, IInteractable
{
    [SerializeField] private List<Seat> _seats;
    private bool _isEmpty = true;

    private void Start()
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            bool randomBlock = Random.Range(0, 6) < 2 ? true : false;

            if (randomBlock)
            {
                _seats[i].isEmpty = false;
            }
        }
    }

    public Seat CheckPlateOnTable(RecipeSO recipeSO)
    {
        foreach (var item in _seats)
        {
            if (item.recipeSO == recipeSO)
            {
                return item;
            }
        }

        return default;
    }

    public bool CanSeatOnTable()
    {
        foreach (var item in _seats)
        {
            if (item.isEmpty) return true;
        }

        return false;
    }

    public bool CanTakeTable()
    {
        return CanSeatOnTable();
    }

    public Seat GetEmptySeatPosition()
    {
        foreach (var item in _seats)
        {
            if (item.isEmpty)
            {
                item.isEmpty = false;
                return item;
            }
        }

        return null;
    }

    public void Interact(ThirdPersonCharacter character)
    {
        if (character.HasKitchenObject())
        {
            if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipeOnTable(plateKitchenObject, this);
                character.GetKitchenObject().DestroySelf();
            }
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }
}