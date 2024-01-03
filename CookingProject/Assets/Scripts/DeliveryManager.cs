using System.Linq;
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
    [SerializeField] private int _waitingRecipesMax = 4;

    [SerializeField] private List<Table> _tables = new List<Table>();

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

    private void NewGame(object[] parameters)
    {
        _isDelivering = true;
    }

    private void GameOver(object[] parameters)
    {
        _isDelivering = false;
    }

    public RecipeSO GetRecipe()
    {
        RecipeSO waitingRecipe = _recipes.recipesSOList[Random.Range(0, _recipes.recipesSOList.Count)];

        _waitingRecipeSO.Add(waitingRecipe);
        OnRecipeSpawned?.Invoke(waitingRecipe);

        Debug.Log(waitingRecipe.RecipeName);

        return waitingRecipe;
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

    public void DeliverRecipeOnTable(PlateKitchenObject plateKitchenObject, Table table)
    {
        RecipeSO correctRecipeSO = null;

        for (int i = 0; i < _waitingRecipeSO.Count; i++)
        {
            RecipeSO waitingRecipeSO = _waitingRecipeSO[i];

            //Tiene la misma cantidad de ingredientes
            if (waitingRecipeSO.kitchenObjectsSO.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
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

                    correctRecipeSO = _waitingRecipeSO[i];
                    Seat correctSeat = table.CheckPlateOnTable(correctRecipeSO);

                    if (correctSeat)
                    {
                        Debug.Log("Player delivered the correct recipe!");
                        OnRecipeCompleted?.Invoke(correctRecipeSO);
                        _waitingRecipeSO.RemoveAt(i);
                        return;
                    }
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

    public bool CanSpawnClient()
    {
        return _waitingRecipeSO.Count < _waitingRecipesMax && _tables.Any(x => x.CanSeatOnTable());
    }

    public Table GetTable()
    {
        int random = Random.Range(0, _tables.Count);
        Table table = _tables[random];

        if (table.CanTakeTable())
        {
            return table;
        }

        return null;
    }

}