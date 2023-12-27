using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        ChangeIcon(kitchenObjectSO.sprite);
    }

    private void ChangeIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }
}
