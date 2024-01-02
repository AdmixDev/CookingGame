using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private List<Seat> _seats;
    private bool _isEmpty = true;

    public bool CanTakeTable()
    {
        if (_isEmpty)
        {
            return true;
        }

        return false;
    }

    public Seat GetSeatPosition()
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
}