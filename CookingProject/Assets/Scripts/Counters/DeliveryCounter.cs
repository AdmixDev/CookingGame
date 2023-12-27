using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : Counter, IInteractable
{

    public void Interact(ThirdPersonCharacter character)
    {
        if (character.HasKitchenObject())
        {
            if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                character.GetKitchenObject().DestroySelf();
            }
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }
}
