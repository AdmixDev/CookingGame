using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : Counter, IInteractable, IHasProgress
{
    public System.Action<State> OnStateChanged;

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    [SerializeField] private FryingPanKitchenObject _fryingPanKitchenObject;
    [SerializeField] private GameObject _progressBar;

    [SerializeField] private List<FryingRecipeSO> _fryingRecipesSO = new List<FryingRecipeSO>();
    [SerializeField] private List<BurningRecipeSO> _burningRecipesSO = new List<BurningRecipeSO>();

    private float _fryingTimer;
    private float _burningTimer;
    private State _currentState;
    private FryingRecipeSO _currentFryingRecipeSO;
    private BurningRecipeSO _currentBurningRecipeSO;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    private void Start()
    {
        _currentState = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (_currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:

                    _fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = _fryingTimer / _currentFryingRecipeSO.fryingTimerMax });


                    if (_fryingTimer > _currentFryingRecipeSO.fryingTimerMax)
                    {
                        _fryingTimer = 0;

                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_currentFryingRecipeSO.output, this);
                        _currentState = State.Fried;

                        _currentBurningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO);
                        _burningTimer = 0;

                        OnStateChanged?.Invoke(_currentState);
                    }

                    break;
                case State.Fried:

                    _burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = _burningTimer / _currentBurningRecipeSO.burningTimerMax });

                    if (_burningTimer > _currentBurningRecipeSO.burningTimerMax)
                    {
                        _burningTimer = 0;

                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_currentBurningRecipeSO.output, this);
                        _currentState = State.Burned;

                        OnStateChanged?.Invoke(_currentState);

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = 0 });
                    }


                    break;
                case State.Burned:
                    break;
                default:
                    break;
            }
        }
    }

    public void Interact(ThirdPersonCharacter character)
    {
        if (!HasKitchenObject())
        {
            if (!character.HasKitchenObject()) return;
            else if (!HasRecipeWithInput(character.GetKitchenObject().GetKitchenObjectSO)) return;

            _currentFryingRecipeSO = GetFryingRecipeSOWithInput(character.GetKitchenObject().GetKitchenObjectSO);

            character.GetKitchenObject().SetKitchenParent(this);

            _currentState = State.Frying;
            _fryingTimer = 0;

            OnStateChanged?.Invoke(_currentState);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = _fryingTimer / _currentFryingRecipeSO.fryingTimerMax });
        }
        else
        {
            if (character.HasKitchenObject())
            {
                if (character.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO))
                    {
                        GetKitchenObject().DestroySelf();

                        _currentState = State.Idle;

                        _currentFryingRecipeSO = null;
                        _currentBurningRecipeSO = null;

                        OnStateChanged?.Invoke(_currentState);
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = 0 });
                    }
                }
            }
            else
            {
                _kitchenObject.SetKitchenParent(character);

                _currentState = State.Idle;

                _currentFryingRecipeSO = null;
                _currentBurningRecipeSO = null;

                OnStateChanged?.Invoke(_currentState);
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progress = 0 });
            }
        }
    }

    public void AlternativeInteract(ThirdPersonCharacter character)
    {

    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenSO);

        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenSO);

        if (fryingRecipeSO != null) return fryingRecipeSO.output;
        else return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var item in _fryingRecipesSO)
        {
            if (item.input == inputKitchenObjectSO)
            {
                return item;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var item in _burningRecipesSO)
        {
            if (item.input == inputKitchenObjectSO)
            {
                return item;
            }
        }

        return null;
    }
}
