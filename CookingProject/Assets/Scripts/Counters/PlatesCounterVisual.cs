using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter _platesCounter;
    [SerializeField] private Transform _counterTopPoint;
    [SerializeField] private Transform _plateVisualPrefab;

    private List<GameObject> _plateVisualGameObjects = new List<GameObject>();

    private void Start()
    {
        _platesCounter.OnPlateSpawned += _platesCounter_OnPlateSpawned;
        _platesCounter.OnPlateRemoved += _platesCounter_OnPlateRemoved;
    }

    private void _platesCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject plateGameObject = _plateVisualGameObjects[_plateVisualGameObjects.Count - 1];
        _plateVisualGameObjects.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void _platesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(_plateVisualPrefab, _counterTopPoint);

        float plateOffsetY = .1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * _plateVisualGameObjects.Count, 0);
        _plateVisualGameObjects.Add(plateVisualTransform.gameObject);
    }
}
