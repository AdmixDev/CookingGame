using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance{get; private set;}

    public System.Action<RecipeSO> OnRecipeSpawned;
    public System.Action<RecipeSO> OnRecipeCompleted;
    public System.Action OnRecipeFailed;

    public RecipeListSO _recipes;
    [SerializeField] private float _spawnRecipeTimerMax = 4;
    [SerializeField] private int _waitingRecipesMax = 4;

    private List<RecipeSO> _waitingRecipeSO = new List<RecipeSO>();

    private float _spawnRecipeTimer;
    private bool _isDelivering;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        EventManager.Subscribe(EventManager.EventType.NewGame, NewGame);
        EventManager.Subscribe(EventManager.EventType.GameOver, GameOver);
    }

    private void Update()
    {
        if(_isDelivering)
        {
            if (_spawnRecipeTimer <= 0)
            {
                if (_waitingRecipeSO.Count < _waitingRecipesMax)
                {
                    SpawnRecipe();
                }
            }
            else
            {
                _spawnRecipeTimer -= Time.deltaTime;
            }
        }
    }

    private void NewGame(object[] parameters)
    {
        _isDelivering = true;
    }

    private void GameOver(object[] parameters)
    {
        _isDelivering = false;
        _spawnRecipeTimer = _spawnRecipeTimerMax;
    }

    private void SpawnRecipe()
    {
        _spawnRecipeTimer = _spawnRecipeTimerMax;

        RecipeSO waitingRecipe = _recipes.recipesSOList[Random.Range(0, _recipes.recipesSOList.Count)];

        _waitingRecipeSO.Add(waitingRecipe);
        OnRecipeSpawned?.Invoke(waitingRecipe);

        Debug.Log(waitingRecipe.RecipeName);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        RecipeSO correctRecipeSO = null;
        for (int i = 0; i < _waitingRecipeSO.Count; i++)
        {
            RecipeSO waitingRecipeSO = _waitingRecipeSO[i];

            if (waitingRecipeSO.kitchenObjectsSO.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Tiene la misma cantidad de ingredientes
                bool plateContentsMatchesRecipe = true;
                foreach (var item in waitingRecipeSO.kitchenObjectsSO)
                {
                    bool ingredientFound = false;
                    foreach (var plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == item)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    //
                    if (!ingredientFound)
                    {
                        //This recipe ingredient was not found on the plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Player delivered the correct recipe!
                    Debug.Log("Player delivered the correct recipe!");
                    correctRecipeSO = _waitingRecipeSO[i];
                    OnRecipeCompleted?.Invoke(correctRecipeSO);
                    _waitingRecipeSO.RemoveAt(i);
                    return;
                }
            }
        }

        //No matches found!
        //Player did not deliver a correct recipe
        Debug.Log("Player did not deliver a correct recipe!");
        OnRecipeFailed?.Invoke();
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return _waitingRecipeSO;
    }
}