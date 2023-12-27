using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Counter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform _counterTopPoint;

    protected Animator _myAnimator;

    protected KitchenObject _kitchenObject;

    protected virtual void Awake()
    {
        _myAnimator = GetComponent<Animator>();
    }

    public Transform GetKitchenObjectFollowTransform() 
    { 
        return _counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
}