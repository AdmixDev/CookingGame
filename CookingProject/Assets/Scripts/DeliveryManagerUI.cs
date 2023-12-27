using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _recipeTemplate;

    private List<DeliverySingleUI> _spawnedRecipes = new List<DeliverySingleUI>();

    private void Awake()
    {
        _recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += UpdateVisual;
        DeliveryManager.Instance.OnRecipeCompleted += RemoveCorrectVisual;
    }

    private void UpdateVisual(RecipeSO recipeSO)
    {
        Transform recipeTransform = Instantiate(_recipeTemplate, _container);
        recipeTransform.gameObject.SetActive(true);
        recipeTransform.GetComponent<DeliverySingleUI>().SetRecipeSO(recipeSO);
        recipeTransform.GetComponent<DeliverySingleUI>().OnTimerEnd += RemoveOnTimeEnd;

        _spawnedRecipes.Add(recipeTransform.GetComponent<DeliverySingleUI>());
    }

    private void RemoveCorrectVisual(RecipeSO recipeSO)
    {
        for (int i = 0; i < _spawnedRecipes.Count; i++)
        {
            if (_spawnedRecipes[i].GetRecipeSO == recipeSO)
            {
                Destroy(_spawnedRecipes[i].gameObject);
                _spawnedRecipes.RemoveAt(i);
                Debug.Log("Remove this");
                break;
            }
        }
    }

    private void RemoveOnTimeEnd(RecipeSO recipeSO)
    {
        Debug.Log("Time is up");
        DeliveryManager.Instance.GetWaitingRecipeSOList().RemoveAt(0);
        Destroy(_spawnedRecipes[0].gameObject);
        _spawnedRecipes.RemoveAt(0);
    }
}
