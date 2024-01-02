using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool isEmpty = true;
    public RecipeSO recipeSO;

    public void GetRecipe()
    {
        recipeSO = DeliveryManager.Instance.GetRecipe();
    }
}
