using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : Counter, IInteractable
{
    public void Interact(ThirdPersonCharacter character)
    {
        if (character.HasKitchenObject())
        {
            character.GetKitchenObject().DestroySelf();
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }
}
