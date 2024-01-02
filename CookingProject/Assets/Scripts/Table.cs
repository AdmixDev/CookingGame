using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Seat
{
    public Transform position;
    public bool isEmpty;
}

public class Table : MonoBehaviour
{
    [SerializeField] private List<Seat> _seats;
    private bool _isEmpty = true;

    public RecipeSO recipeSO;

    public bool CanTakeTable()
    {
        if (_isEmpty)
        {
            return true;
        }

        return false;
    }

    public Transform GetSeatPosition()
    {
        Transform seatPosition = transform;
        return seatPosition;
    }
}