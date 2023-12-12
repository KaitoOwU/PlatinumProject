using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using static PlayerController;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public int PlayerIndex => _playerIndex;
    public InputManager Inputs => _inputManager;
    public EMoveState MoveState => _moveState;
    public Animator Animator => _animator;
    public Furniture PushedFurniture { get => _pushedFurniture; set => _pushedFurniture = value; }
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }

    private Furniture _pushedFurniture;

    [Header("Parameters")]
    private float _moveSpeed;
    [SerializeField]
    private int _playerIndex;
    [Header("References")]
    [SerializeField]
    private InputManager _inputManager;
    [SerializeField]
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerInput _inputs;
    private float _currentVelocity;
    private EMoveState _moveState;

    public static Dictionary<EButtonType, string> INPUT_NAMES = new()
    { { EButtonType.MOVE, "Move" },
        { EButtonType.INTERACT, "Interact" },
        { EButtonType.PUSH, "Push" },
        { EButtonType.TOOL, "Tool" },
        { EButtonType.PAUSE, "Pause" },
        {EButtonType.ASK_TP_HUB, "AskTPHub"},
        { EButtonType.BACK, "Back"}
    };

    [HideInInspector] public UnityEvent OnMoveStarted;
    [HideInInspector] public UnityEvent OnMoveCanceled;
    [HideInInspector] public UnityEvent OnPause;
    [HideInInspector] public UnityEvent OnBackUI;

    [HideInInspector] public UnityEvent OnPickCharacter1;
    [HideInInspector] public UnityEvent OnPickCharacter2;
    [HideInInspector] public UnityEvent OnPickCharacter3;
    [HideInInspector] public UnityEvent OnPickCharacter4;

    public enum EButtonType
    {
        MOVE,
        INTERACT,
        PUSH,
        TOOL,
        PAUSE,
        ASK_TP_HUB,
        BACK
    }
    public enum EMoveState
    {
        NORMAL,
        PUSH,
        PUSH_BLOCKED,
    }
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

    }
    private void OnDisable() => _CleanUp();

    public bool IsButtonHeld(EButtonType buttonType)
    {
        return _inputManager?.GetInputValue(INPUT_NAMES[buttonType]) > 0;
    }

    #region Set up & Clean up
    public IEnumerator SetUp(InputManager inputManager, PlayerInput inputs, Transform playerController)
    {
        switch (_playerIndex)
        {
            case 1:
                OnPickCharacter1?.Invoke();
                break;
            case 2:
                OnPickCharacter2?.Invoke();
                break;
            case 3:
                OnPickCharacter3?.Invoke();
                break;
            case 4:
                OnPickCharacter4?.Invoke();
                break;

        }
        //Debug.Log("WakeUP");
        _animator.SetTrigger("WakeUp");
        yield return new WaitForSeconds(2f); // Wait for end of animation

        GameManager.Instance.CurrentPlayersCount++;
        _inputManager = inputManager;
        _inputs = inputs;
        playerController.SetParent(transform);


        GetComponent<Player>().Index = _playerIndex;

        _inputManager.OnInteract.AddListener(_Interact);
        _inputManager.OnUseTool.AddListener(_UseTool);
        _inputManager.OnPause.AddListener(_Pause);
        _inputManager.OnBack.AddListener(_Back);

        _inputManager.OnMoveStarted.AddListener(_StartMove);
        _inputManager.OnMoveCanceled.AddListener(_StopMove);

        _moveSpeed = GameManager.Instance.PlayerConstants.NormalMoveSpeed;
        _moveState = EMoveState.NORMAL;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void _CleanUp()
    {
        if (_inputManager != null)
        {
            _inputManager.OnMoveStarted.RemoveListener(_StartMove);
            _inputManager.OnMoveCanceled.RemoveListener(_StopMove);

            _inputManager.OnInteract.RemoveListener(_Interact);
            _inputManager.OnUseTool.RemoveListener(_UseTool);
            _inputManager.OnPause.RemoveListener(_Pause);
            _inputManager.OnBack.RemoveListener(_Back);
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
                _pushedFurniture = null;
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
            if (_inputManager.InputLocked)
                return;
            if (_inputs.actions["Move"].ReadValue<Vector2>() != Vector2.zero)
            {
                if (_moveState == EMoveState.NORMAL)
                    UpdatePlayerRotation(_inputs.actions["Move"].ReadValue<Vector2>());
                _Move(_inputs.actions["Move"].ReadValue<Vector2>());
            }
            else
            {
                _animator.SetBool("IsMoving", false);
            }
        }
    }
    public IEnumerator Fall()
    {
        _animator.SetTrigger("Fall");
        Inputs.InputLocked = true;

        yield return new WaitForSeconds(5.65f);
        Inputs.InputLocked = false;
    }

    #region Player Actions Methods
    private void _Move(Vector3 dir)
    {
        _rigidbody.velocity = new Vector3(dir.x * _moveSpeed, _rigidbody.velocity.y, dir.y * _moveSpeed);
        if (_moveState != EMoveState.NORMAL)
        {
            if (Vector3.Dot(dir, transform.forward) > 0)
            {
                _animator.SetBool("IsPushing", true);
                _animator.SetBool("IsPulling", false);
            }
            else
            {
                _animator.SetBool("IsPulling", true);
                _animator.SetBool("IsPushing", false);
            }
        }
        else
        {
            _animator.SetBool("IsMoving", true);
        }

    }
    private void _StartMove() => OnMoveStarted?.Invoke();
    private void _StopMove() => OnMoveCanceled?.Invoke();
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
        OnPause?.Invoke();
    }   
    private void _Back() => OnBackUI?.Invoke();
    
    #endregion
}
