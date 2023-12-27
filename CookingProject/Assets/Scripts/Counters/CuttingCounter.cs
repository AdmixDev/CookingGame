using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : Counter, IInteractable, IHasProgress
{
    [SerializeField] private List<CuttingRecipeSO> _cuttingRecipesSO = new List<CuttingRecipeSO>();

    private float _cuttingProgress;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public void Interact(ThirdPersonCharacter character)
    {
        if (!HasKitchenObject())
        {
            if (!character.HasKitchenObject()) return;
            else if (!HasRecipeWithInput(character.GetKitchenObject().GetKitchenObjectSO)) return;

            character.GetKitchenObject().SetKitchenParent(this);
            _cuttingProgress = 0;
        }
        else
        {
            if (_cuttingProgress > 0) return;

            if (character.HasKitchenObject())
            {
                if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO))
                        GetKitchenObject().DestroySelf();
                }
            }
            else
            {
                _kitchenObject.SetKitchenParent(character);
            }

        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO))
        {
            _cuttingProgress++;
            _myAnimator.SetTrigger("Cut");

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = _cuttingProgress / cuttingRecipeSO.cuttingProgressMax });

            if (_cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outPutKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO);

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outPutKitchenObjectSO, this);

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = _cuttingProgress / cuttingRecipeSO.cuttingProgressMax });

                _cuttingProgress = 0;
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenSO);

        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenSO);

        if (cuttingRecipeSO != null) return cuttingRecipeSO.output;
        else return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var item in _cuttingRecipesSO)
        {
            if (item.input == inputKitchenObjectSO)
            {
                return item;
            }
        }

        return null;
    }
}