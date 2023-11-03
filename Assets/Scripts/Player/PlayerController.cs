using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using static PlayerController;

public class PlayerController : MonoBehaviour
{
    public int PlayerIndex => _playerIndex;
    public InputManager Inputs => _inputManager;

    [Header("Parameters")]
    private float _moveSpeed;
    [SerializeField]
    private int _playerIndex;
    private Rigidbody _rigidbody;
    [SerializeField]
    private InputManager _inputManager;
    private PlayerInput _inputs;
    private float _currentVelocity;
    private EMoveState _moveState;

    public enum EButtonType
    {
        MOVE,
        INTERACT,
        PUSH,
        TOOL,
        PAUSE
    }
    public enum EMoveState
    {
        NORMAL,
        PUSH,
        PUSH_BLOCKED,
    }
    private Vector3 _constraint;
    Dictionary<EButtonType, string> _inputNames = new()
    { { EButtonType.MOVE, "Move" },
    { EButtonType.INTERACT, "Interact" },
    { EButtonType.PUSH, "Push" },
    { EButtonType.TOOL, "Tool" },
    { EButtonType.PAUSE, "Pause" }};

    private void OnDisable() => _CleanUp();
    RigidbodyConstraints _normalConstraints;

    public bool IsButtonHeld(EButtonType buttonType)
    {
        return _inputManager.GetInputValue<bool>(_inputNames[buttonType]);
    }

    #region Set up & Clean up
    public void SetUp(InputManager inputManager, PlayerInput inputs, Transform playerController)
    {
        _inputManager = inputManager;
        _inputs = inputs;
        playerController.SetParent(transform);

        _rigidbody = GetComponent<Rigidbody>();

        GetComponent<Player>().Index = _playerIndex;

        _inputManager.OnInteract.AddListener(_Interact);
        _inputManager.OnUseTool.AddListener(_UseTool);
        _inputManager.OnPause.AddListener(_Pause);

        _moveSpeed = GameManager.Instance.PlayerConstants.NormalMoveSpeed;
        _moveState = EMoveState.NORMAL;
        _normalConstraints = _rigidbody.constraints;
    }
    private void _CleanUp()
    {
        if (_inputManager != null)
        {
            _inputManager.OnInteract.RemoveListener(_Interact);
            _inputManager.OnUseTool.RemoveListener(_UseTool);
            _inputManager.OnPause.RemoveListener(_Pause);
        }
    }
    #endregion

    public void SwitchMoveState(EMoveState newMoveState, Vector3 constraint = new()) 
    {
        _moveState = newMoveState;
        switch (newMoveState)
        {
            case EMoveState.NORMAL:
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
                _moveSpeed = GameManager.Instance.PlayerConstants.NormalMoveSpeed; break;
            case EMoveState.PUSH:
                if (constraint.x != 0)
                    _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                else if (constraint.z != 0)
                    _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                _moveSpeed = GameManager.Instance.PlayerConstants.PushMoveSpeed; break;
            case EMoveState.PUSH_BLOCKED:
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                _moveSpeed = 0; break;
        }
    }

    private void FixedUpdate()
    {
        if (_inputs != null)
        {
            if (_inputs.actions["Move"].ReadValue<Vector2>() != Vector2.zero)
            {
                if (_moveState == EMoveState.NORMAL)
                    UpdatePlayerRotation(_inputs.actions["Move"].ReadValue<Vector2>());
                _Move(_inputs.actions["Move"].ReadValue<Vector2>());
            }
        }
    }

    #region Player Actions Methods
    private void _Move(Vector3 dir)
    {
        _rigidbody.velocity = new Vector3(dir.x * _moveSpeed, _rigidbody.velocity.y, dir.y * _moveSpeed);
    }
    private void UpdatePlayerRotation(Vector3 dir)
    {
        var targetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg, ref _currentVelocity, 0.05f);
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }
    private void _Interact(Player player) 
    {

    }

    private void _Push(Player player)
    {

    }

    private void _UseTool()
    {
        
    }

    private void _Pause()
    {
        
    }
    #endregion
}
