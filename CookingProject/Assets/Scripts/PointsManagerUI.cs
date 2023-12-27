using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManagerUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    private void Start()
    {
        GameManager.Instance.OnTimeUpdated += OnTimeUpdated;
    }

    private void OnTimeUpdated(float timeNormalized)
    {
        _fillImage.fillAmount = timeNormalized;
    }
}