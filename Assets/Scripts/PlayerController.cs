using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private float _moveSpeed = 1;

    private InputManager _inputManager;
    private Rigidbody _rigidbody;

    private void Start() => SetUp();
    private void OnDisable() => CleanUp();

    #region Set up & Clean up
    private void SetUp()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
        _inputManager._OnInteract.AddListener(Interact);
        _inputManager._OnUseTool.AddListener(UseTool);
        _inputManager._OnPause.AddListener(Pause);
    }    
    private void CleanUp()
    {
        _inputManager._OnInteract.RemoveListener(Interact);
        _inputManager._OnUseTool.RemoveListener(UseTool);
        _inputManager._OnPause.RemoveListener(Pause);
    }
    #endregion

    private void FixedUpdate()
    {
        if (_inputManager.Inputs.Gameplay.Move.ReadValue<Vector2>() != Vector2.zero)
            Move(_inputManager.Inputs.Gameplay.Move.ReadValue<Vector2>());
    }

    #region Player Actions Methods
    public void Move(Vector3 dir)
    {
        _rigidbody.AddForce((Quaternion.AngleAxis(90, Vector3.right) * dir) * _moveSpeed);
    }

    public void Interact()
    {
        
    }

    public void UseTool()
    {
        
    }  
    
    public void Pause()
    {
        
    }
    #endregion
}
