using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject _hasProgressObject;
    [SerializeField] private Image _barImage;

    private IHasProgress _hasProgress;

    private void Start()
    {
        _hasProgress = _hasProgressObject.GetComponent<IHasProgress>();

        if(_hasProgress != null)
        {
            _hasProgress.OnProgressChanged += IHasProgress_OnProgressChanged;

            _barImage.fillAmount = 0;

            ShowOrHide(false);
        }
    }

    private void IHasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        _barImage.fillAmount = e.progress;

        if (e.progress == 0 || e.progress == 1)
            ShowOrHide(false);
        else
            ShowOrHide(true);
    } 

    private void ShowOrHide(bool value)
    {
        gameObject.SetActive(value);
    }
}
