using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Seat
{
    public bool isEmpty = true;
    public Transform seatPosition;
}

public class Table : MonoBehaviour
{
    [SerializeField] private List<Seat> _seats;
    public RecipeSO recipeSO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
