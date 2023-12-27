using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected float _walkSpeed = 7;
    [SerializeField] protected float _runSpeed = 10;
    [SerializeField] protected float _gravity = 9.8f;

    protected float _initialWalkSpeed;
    protected float _targetSpeed;
    protected Vector3 _moveVector = Vector3.zero;
    protected bool _isRunning = false;

    public bool IsRunning => _isRunning;

    public abstract void Move(float h, float v);
    public abstract void Run();

    public virtual void Move() { }
    public virtual void Jump() { }
    public virtual void Crouch() { }
    public virtual void MainAction() { }
    public virtual void AlternativeAction() { }

    protected virtual void Start()
    {
        _initialWalkSpeed = _walkSpeed;
        _targetSpeed = _walkSpeed;
    }
}
