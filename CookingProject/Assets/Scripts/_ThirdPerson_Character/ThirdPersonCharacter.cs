using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCharacter : Entity, IKitchenObjectParent
{
    public ThirdPersonCamera _cameraController;
    public InteractionsManager interactionsManager;

    [Header("Rotations")]
    [SerializeField] private float _rotationSharpness;

    [Header("Interactions")]
    [SerializeField] private Transform _kitchenObjectHoldPoint;

    private CharacterController _charController;

    private Control _control;
    private bool _lockingEnemy;
    private Animator _myAnimator;
    private KitchenObject _kitchenObject;


    private void Awake()
    {
        _control = new Control(this);
        _charController = GetComponent<CharacterController>();
        _myAnimator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

    }

    private void Update()
    {
        _control.OnUpdate();
    }

    public override void MainAction()
    {
        IInteractable interactable = interactionsManager.CurrentInteractable;
        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }

    public override void AlternativeAction()
    {
        IInteractable interactable = interactionsManager.CurrentInteractable;
        if (interactable != null)
        {
            interactable.AlternativeInteract(this);
        }
    }

    public override void Move(float h, float v)
    {
        _moveVector = GetPlayerMoveNormalized(h, v);
        transform.rotation = GetRotation(_moveVector);

        Vector3 moveMultiplied = _moveVector *= _targetSpeed;

        _charController.Move(moveMultiplied * Time.deltaTime);
    }

    public override void Run()
    {
        if (!_isRunning)
        {
            _targetSpeed = _runSpeed;
            _isRunning = true;
        }
        else if (_isRunning)
        {
            _targetSpeed = _walkSpeed;
            _isRunning = false;
        }
    }

    private Vector3 GetPlayerMoveNormalized(float h, float v)
    {
        Vector3 movementInputs = new Vector3(h, 0, v).normalized;

        // Calculate camera relative directions to move:
        Vector3 _cameraPlanarDirection = _cameraController.CameraPlanarDirection;
        Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);

        Vector3 _moveVectorOriented = _cameraPlanarRotation * movementInputs;

        //Set Move anim
        _myAnimator.SetFloat("Speed", _moveVectorOriented.magnitude);

        return _moveVectorOriented.normalized;
    }

    private Quaternion GetRotation(Vector3 moveNormalized)
    {
        Quaternion rotation;

        if (moveNormalized.magnitude != 0)
        {
            Quaternion _targetRotation = Quaternion.LookRotation(moveNormalized);
            rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationSharpness * Time.deltaTime);
            return rotation;
        }
        else
        {
            return transform.rotation;
        }
    }

    #region IKitchenObjectParent

    public Transform GetKitchenObjectFollowTransform()
    {
        return _kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }

    #endregion
}