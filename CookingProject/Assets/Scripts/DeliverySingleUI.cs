using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliverySingleUI : MonoBehaviour
{
    public System.Action<RecipeSO> OnTimerEnd;

    [SerializeField] private Slider _recipeSlider;
    [SerializeField] private TextMeshProUGUI _recipeNameTxt;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private Transform _iconTemplate;

    [SerializeField] private float _timeMax = 7;
    private float _timer;

    private RecipeSO _myRecipeSO;

    public RecipeSO GetRecipeSO => _myRecipeSO;

    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _timer = _timeMax;
        _recipeSlider.value = _timer / _timeMax;
    }

    private void Update()
    {
        if (_timer <= 0)
        {
            //_timer = _timeMax;
            OnTimerEnd?.Invoke(_myRecipeSO);
        }
        else
        {
            _timer -= Time.deltaTime;
            _recipeSlider.value = _timer / _timeMax;
        }
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        _myRecipeSO = recipeSO;
        _recipeNameTxt.text = _myRecipeSO.RecipeName;

        foreach (Transform child in _iconContainer)
        {
            if (child == _iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (var item in recipeSO.kitchenObjectsSO)
        {
            Transform kitchenTransform = Instantiate(_iconTemplate, _iconContainer);
            kitchenTransform.gameObject.SetActive(true);
            kitchenTransform.GetComponent<Image>().sprite = item.sprite;
        }
    }
}
