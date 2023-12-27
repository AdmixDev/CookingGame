using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;

    private IKitchenObjectParent _selectedKitchenParent;

    public KitchenObjectSO GetKitchenObjectSO => _kitchenObjectSO;
    public IKitchenObjectParent GetKitchenObjectParent => _selectedKitchenParent;

    public void DestroySelf()
    {
        _selectedKitchenParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public void SetKitchenParent(IKitchenObjectParent kitchenParent)
    {
        if(_selectedKitchenParent != null)
        {
            _selectedKitchenParent.ClearKitchenObject();
        }

        _selectedKitchenParent = kitchenParent;

        _selectedKitchenParent.SetKitchenObject(this);

        transform.parent = _selectedKitchenParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public bool TryGetFryingPan(out FryingPanKitchenObject fryingPanKitchenObject)
    {
        if (this is FryingPanKitchenObject)
        {
            fryingPanKitchenObject = this as FryingPanKitchenObject;
            return true;
        }
        else
        {
            fryingPanKitchenObject = null;
            return false;
        }
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenParent(kitchenObjectParent);

        return kitchenObject;
    }
}
