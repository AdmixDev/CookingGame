using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Framing")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _followTransform = null;
    private Vector2 _framingNormal = new Vector2(0, 0);

    [Header("Distance - Zoom")]
    [SerializeField] private bool _useZoom;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _defaultDistance = 5f;
    [SerializeField] private float _minDistance = 0f;
    [SerializeField] private float _maxDistance = 10f;

    [Header("Rotation")]
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] private bool _invertX = false;
    [SerializeField] private bool _invertY = false;
    [SerializeField] private float _rotationSharpness = 25f;
    [SerializeField] private float _defaultVerticalAngle = 20f;
    [SerializeField] [Range(-90, 90)] private float _minVerticalAngle = -90;
    [SerializeField] [Range(-90, 90)] private float _maxVerticalAngle = 90;

    [Header("Obstructions")]
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _obstructionLayers = -1;
    private List<Collider> _ignoreColliders = new List<Collider>();

    [Header("Lock On")]
    [SerializeField] private float _lockOnLossTime = 1f;
    [SerializeField] private float _lockOnDistance = 15f;
    [SerializeField] private LayerMask _lockOnLayers = -1;
    [SerializeField] private Vector3 _lockOnFraming = Vector3.zero;
    [SerializeField, Range(1, 179)] private float _lockOnFOV = 40f;

    private ITargetable _target;
    private bool _lockedOn;
    private float _lockOnLossTimeCurrent;

    public ITargetable Target { get => _target; }
    public Vector3 CameraPlanarDirection { get => _planarDirection; }
    public bool LockedOn { get => _lockedOn; }

    //Privates
    private float _fovNormal;
    private float _framingLerp;
    private Vector3 _planarDirection;   //Cameras forward on the x,z plane
    private float _targetDistance;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _targetVerticalAngle;
    float _zoom;

    private Vector3 _newPosition;
    private Quaternion _newRotation;

    public Camera GetCamera => _camera;

    private float _zoomFov;

    private bool _isZoomed;

    private void OnValidate()
    {
        _defaultDistance = Mathf.Clamp(_defaultDistance, _minDistance, _maxDistance);
        _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
    }

    private void Start()
    {
        if (!_camera)
            _camera = Camera.main;

        //Ignore the players colliders
        _ignoreColliders.AddRange(GetComponentsInChildren<Collider>());

        //Important
        _fovNormal = _camera.fieldOfView;
        _planarDirection = _followTransform.forward;

        //Calculate Targets
        _targetDistance = _defaultDistance;
        _targetVerticalAngle = _defaultVerticalAngle;
        _targetRotation = Quaternion.LookRotation(_planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
        _targetPosition = _followTransform.position - (_targetRotation * Vector3.forward) * _targetDistance;

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        //if (Cursor.lockState != CursorLockMode.Locked)
        //    return;

        //Handle Inputs
        if (_useZoom)
            _zoom = -Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;

        float _mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float _mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        if (_invertX) { _mouseX *= -1f; }
        if (_invertY) { _mouseY *= -1f; }

        //Framing
        Vector3 _framing = Vector3.Lerp(_framingNormal, _lockOnFraming, _framingLerp);

        Vector3 _focusPosition = _followTransform.position + _followTransform.TransformDirection(_framing);
        float _fov = Mathf.Lerp(_fovNormal, _lockOnFOV, _framingLerp);
        _camera.fieldOfView = _fov;

        if (_lockedOn && _target != null && _target.TargetTransform != null)
        {
            Vector3 _camToTarget = _target.TargetTransform.position - _camera.transform.position;

            Vector3 _planarCamToTarget = Vector3.ProjectOnPlane(_camToTarget, Vector3.up);

            // Assumption line
            Quaternion _lookRotation = Quaternion.LookRotation(_camToTarget, Vector3.up);

            _framingLerp = Mathf.Clamp01(_framingLerp + Time.deltaTime);
            _planarDirection = _planarCamToTarget != Vector3.zero ? _planarCamToTarget.normalized : _planarDirection;
            _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance);

            _targetVerticalAngle = _lookRotation.eulerAngles.x;

            //if (_targetVerticalAngle >= 80)
            //{
            //    _lockedOn = false;
            //}
        }
        else
        {
            if (_isZoomed)
            {
                _framingLerp = Mathf.Clamp01(_framingLerp + Time.deltaTime);
            }
            else
            {
                _framingLerp = Mathf.Clamp01(_framingLerp - Time.deltaTime);
            }

            _planarDirection = Quaternion.Euler(0, _mouseX, 0) * _planarDirection;
            _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle + _mouseY, _minVerticalAngle, _maxVerticalAngle);
        }

        Debug.DrawLine(_camera.transform.position, _camera.transform.position + _planarDirection, Color.red);

        //Handle Obstructions (affects target distance)
        float _smallestDistance = _targetDistance;
        RaycastHit[] _hits = Physics.SphereCastAll(_focusPosition, _checkRadius, _targetRotation * -Vector3.forward, _targetDistance, _obstructionLayers);
        if (_hits.Length != 0)
            foreach (RaycastHit hit in _hits)
                if (!_ignoreColliders.Contains(hit.collider))
                    if (hit.distance < _smallestDistance)
                        _smallestDistance = hit.distance;

        //Final Targets
        _targetRotation = Quaternion.LookRotation(_planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
        _targetPosition = _focusPosition - (_targetRotation * Vector3.forward) * _smallestDistance;

        ////Handle Smoothing
        _newRotation = Quaternion.Slerp(_camera.transform.rotation, _targetRotation, _rotationSharpness);
        _newPosition = Vector3.Lerp(_camera.transform.position, _targetPosition, _rotationSharpness);

        //Apply
        _camera.transform.rotation = _newRotation;
        _camera.transform.position = _newPosition;

        if (_lockedOn && _target != null && _target.TargetTransform != null)
        {
            bool _valid = _target.Targetable && InDistance(_target) && InScreen(_target) && NotBlocked(_target);

            if (_valid) { _lockOnLossTimeCurrent = 0; }
            else { _lockOnLossTimeCurrent = Mathf.Clamp(_lockOnLossTimeCurrent + Time.deltaTime, 0, _lockOnLossTime); }

            if (_lockOnLossTimeCurrent == _lockOnLossTime)
                _lockedOn = false;
        }
    }

    public void ToggleLockOn(bool toggle)
    {
        if (toggle == _lockedOn)
            return;

        _lockedOn = !_lockedOn;

        if (_lockedOn)
        {
            //Filter targetables
            List<ITargetable> _targetables = new List<ITargetable>();
            Collider[] _colliders = Physics.OverlapSphere(transform.position, _lockOnDistance, _lockOnLayers);

            foreach (Collider _col in _colliders)
            {
                ITargetable _targetable = _col.GetComponent<ITargetable>();

                if (_targetable != null)
                    if (_targetable.Targetable)
                        if (InScreen(_targetable))
                            if (NotBlocked(_targetable))
                                _targetables.Add(_targetable);
            }

            //Find closest targetable to screen center
            float _hypotenuse;
            float _smallestHypotenuse = Mathf.Infinity;
            ITargetable _closestTargetable = null;

            foreach (ITargetable targetable in _targetables)
            {
                _hypotenuse = CalculateHypotenuse(targetable.TargetTransform.position);
                if (_smallestHypotenuse > _hypotenuse)
                {
                    _closestTargetable = targetable;
                    _smallestHypotenuse = _hypotenuse;
                }
            }

            //Final
            _target = _closestTargetable;
            _lockedOn = _closestTargetable != null;
        }
    }

    public void SetTargetNull()
    {
        _target = null;
    }

    private bool InDistance(ITargetable targetable)
    {
        float _distance = Vector3.Distance(transform.position, targetable.TargetTransform.position);

        return _distance <= _lockOnDistance;
    }

    private bool InScreen(ITargetable targetable)
    {
        Vector3 _viewPortPosition = _camera.WorldToViewportPoint(targetable.TargetTransform.position);

        if (!(_viewPortPosition.x > 0) || !(_viewPortPosition.x < 1)) { return false; }
        if (!(_viewPortPosition.y > 0) || !(_viewPortPosition.y < 1)) { return false; }
        if (!(_viewPortPosition.z > 0)) { return false; }

        return true;
    }

    private bool NotBlocked(ITargetable targetable)
    {
        Vector3 _origin = _camera.transform.position;
        Vector3 _direction = targetable.TargetTransform.position - _origin;

        float _radius = 0.15f;
        float _distance = _direction.magnitude;
        bool _notBlocked = !Physics.SphereCast(_origin, _radius, _direction, out RaycastHit _hit, _distance, _obstructionLayers);

        return _notBlocked;
    }

    private float CalculateHypotenuse(Vector3 position)
    {
        float _screenCenterX = _camera.pixelWidth / 2;
        float _screenCenterY = _camera.pixelHeight / 2;

        Vector3 _screenPosition = _camera.WorldToScreenPoint(position);
        float _xDelta = _screenCenterX - _screenPosition.x;
        float _yDelta = _screenCenterX - _screenPosition.y;
        float _hypotenuse = Mathf.Sqrt(Mathf.Pow(_xDelta, 2) + Mathf.Pow(_yDelta, 2));

        return _hypotenuse;
    }
}
