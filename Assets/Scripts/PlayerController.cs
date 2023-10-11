using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int PlayerIndex => _playerIndex;

    [Header("Parameters")]
    [SerializeField]
    private float _moveSpeed = 1;
    [SerializeField]
    private int _playerIndex;
    private Rigidbody _rigidbody;
    private InputManager _inputManager;
    private PlayerInput _inputs;

    private void OnDisable() => _CleanUp();

    #region Set up & Clean up
    public void SetUp(InputManager inputManager, PlayerInput inputs, Transform playerController)
    {
        _inputManager = inputManager;
        _inputs = inputs;
        playerController.SetParent(transform); 

        _rigidbody = GetComponent<Rigidbody>();

        _inputManager._OnInteract.AddListener(_Interact);
        _inputManager._OnUseTool.AddListener(_UseTool);
        _inputManager._OnPause.AddListener(_Pause);
    }
    private void _CleanUp()
    {
        if(_inputManager != null)
        {
            _inputManager._OnInteract.RemoveListener(_Interact);
            _inputManager._OnUseTool.RemoveListener(_UseTool);
            _inputManager._OnPause.RemoveListener(_Pause);
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
        _rigidbody.AddForce((Quaternion.AngleAxis(90, Vector3.right) * dir) * _moveSpeed);
    }

    public void _Interact()
    {
        
    }

    public void _UseTool()
    {
        
    }

    public void _Pause()
    {
        
    }
    #endregion
}
