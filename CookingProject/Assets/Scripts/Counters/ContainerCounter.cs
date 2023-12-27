using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : Counter, IInteractable
{
    [SerializeField] protected KitchenObjectSO _kitchenObjectSO;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer.sprite = _kitchenObjectSO.sprite;
    }

    public void Interact(ThirdPersonCharacter character)
    {
        if (!character.HasKitchenObject())
        {
            _myAnimator.SetTrigger("OpenClose");

            KitchenObject.SpawnKitchenObject(_kitchenObjectSO, character);
        }
        else
        {
            if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                if (plateKitchenObject.TryAddIngredient(_kitchenObjectSO))
                {
                    Debug.Log("Added to plate");
                }
            }
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }
}