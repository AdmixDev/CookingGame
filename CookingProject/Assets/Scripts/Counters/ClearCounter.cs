using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : Counter, IInteractable
{
    public void Interact(ThirdPersonCharacter character)
    {
        if (!HasKitchenObject())
        {
            if (character.HasKitchenObject())
            {
                character.GetKitchenObject().SetKitchenParent(this);
            }
        }
        else
        {
            if (character.HasKitchenObject())
            {
                if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO))
                        GetKitchenObject().DestroySelf();
                }
                else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(character.GetKitchenObject().GetKitchenObjectSO))
                        {
                            character.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
                _kitchenObject.SetKitchenParent(character);
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }
}