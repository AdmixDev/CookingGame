using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsManager : MonoBehaviour
{
    public static System.Action<Counter> OnSelectedChanged;

    [Header("Interactions")]
    [SerializeField] private LayerMask _interactLayer;

    private readonly HashSet<Counter> _interactables = new HashSet<Counter>();

    public IInteractable CurrentInteractable { get; private set; }

    private void OnDestroy()
    {
        OnSelectedChanged = null;
    }

    private void FixedUpdate()
    {
        IInteractable closest = TryGetClosestInteractable();

        // nothing has changed
        if (closest == CurrentInteractable) { return; }

        // something has changed (maybe null)
        CurrentInteractable = closest;

        // togglesOn only when there is a interactable near
        OnSelectedChanged?.Invoke(closest as Counter);
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();

        if (interactable == null) return;

        if (_interactables.Contains(interactable as Counter))
        {
            Debug.LogWarning($"[InteractableController] TriggerEnter on a preexisting collider {other.gameObject.name}");
            return;
        }

        _interactables.Add(interactable as Counter);
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            _interactables.Remove(interactable as Counter);
        }
    }

    public void Remove(IInteractable interactable)
    {
        _interactables.Remove(interactable as Counter);
    }

    private IInteractable TryGetClosestInteractable()
    {
        var minDistance = float.MaxValue;
        IInteractable closest = null;
        foreach (var interactable in _interactables)
        {
            var distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance > minDistance) continue;
            minDistance = distance;
            closest = interactable as IInteractable;
        }

        return closest;
    }
}