using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int PlayerIndex => _playerIndex;
    public bool IsInteractHeld => _isInteractHeld;
    public InputManager Inputs => _inputManager;

    [Header("Parameters")]
    [SerializeField]
    private float _moveSpeed = 1;
    [SerializeField]
    private int _playerIndex;
    private Rigidbody _rigidbody;
    [SerializeField]
    private InputManager _inputManager;
    private PlayerInput _inputs;
    private bool _isInteractHeld;

    private void OnDisable() => _CleanUp();

    #region Set up & Clean up
    public void SetUp(InputManager inputManager, PlayerInput inputs, Transform playerController)
    {
        _inputManager = inputManager;
        _inputs = inputs;
        playerController.SetParent(transform); 

        _rigidbody = GetComponent<Rigidbody>();

        GetComponent<Player>().Index = _playerIndex;

        _inputManager.OnInteractStarted.AddListener(_Interact);
        _inputManager.OnInteractCanceled.AddListener(_StopInteract);
        _inputManager.OnUseTool.AddListener(_UseTool);
        _inputManager.OnPause.AddListener(_Pause);
    }
    private void _CleanUp()
    {
        if(_inputManager != null)
        {
            _inputManager.OnInteractStarted.RemoveListener(_Interact);
            _inputManager.OnInteractCanceled.RemoveListener(_StopInteract);
            _inputManager.OnUseTool.RemoveListener(_UseTool);
            _inputManager.OnPause.RemoveListener(_Pause);
        }
    }
    #endregion

    private void FixedUpdate()
    {
        if(_inputs != null)
        {
            if (_inputs.actions["Move"].ReadValue<Vector2>() != Vector2.zero)
                _Move(_inputs.actions["Move"].ReadValue<Vector2>());
        }        
    }

    #region Player Actions Methods
    private void _Move(Vector3 dir)
    {
        //_rigidbody.AddForce((Quaternion.AngleAxis(90, Vector3.right) * dir) * _moveSpeed);
        _rigidbody.velocity = new Vector3(dir.x * _moveSpeed, _rigidbody.velocity.y, dir.y * _moveSpeed);
    }

    private void _Interact(Player player) => _isInteractHeld = true;
    private void _StopInteract(Player player) => _isInteractHeld = false;

    private void _UseTool()
    {
        
    }

    private void _Pause()
    {
        
    }
    #endregion
}
