using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIValidateInputs : MonoBehaviour
{
    [SerializeField] private InputAwaiter[] _inputAwaiters;
    [SerializeField] private GameObject _reactive;

    public IInputAwaiterReactive Reactive => _reactive.GetComponent<IInputAwaiterReactive>();
    public InputAwaiter[] InputAwaiters => _inputAwaiters;

    public void Setup(PlayerController.EButtonType type, string input, params PlayerInfo[] players)
    {
        for (var index = 0; index < GameManager.Instance.PlayerList.Count; index++)
        {
            PlayerInfo p = GameManager.Instance.PlayerList[index];
            if (players.ToList().Contains(p) && p.PlayerController.Inputs != null)
            {
                _inputAwaiters[index].SetActive(true);
                _inputAwaiters[index].Setup(this, input, type, p.PlayerRef);
            }
            else
            {
                _inputAwaiters[index].SetActive(false);
            }
        }
    }

    public void ActivateChange()
    {
        if (_inputAwaiters.ToList().FindAll(awaiter => awaiter.Activated).Count ==
            _inputAwaiters.ToList().FindAll(awaiter => awaiter.IsActiveInHierarchy).Count)
        {
            StartCoroutine(Reactive.AwaiterCompleted());
        }
    }

    public void Unsetup(PlayerController.EButtonType input)
    {
        _inputAwaiters.ToList().ForEach(awaiter => awaiter.UnsetupInputs(input));
    }
}

[Serializable]
public class InputAwaiter
{
    public int index;
    public TextMeshProUGUI inputTxt;
    public Image surround;
    public Image validator;
    public Image button;

    private UIValidateInputs _inputAwaiter;
    private bool _activated;

    public bool Activated => _activated;
    public bool IsActiveInHierarchy => surround.transform.parent.gameObject.activeInHierarchy;

    public void Setup(UIValidateInputs inputAwaiter, string input, PlayerController.EButtonType buttonType, Player player)
    {
        _inputAwaiter = inputAwaiter;
        inputTxt.text = input;
        player.PlayerController.Inputs.Map.actions[PlayerController.INPUT_NAMES[buttonType]].started += InputValidate;
        player.PlayerController.Inputs.Map.actions[PlayerController.INPUT_NAMES[buttonType]].canceled += InputCanceled;
    }
    
    internal void UnsetupInputs(PlayerController.EButtonType buttonType)
    {
        foreach (PlayerInfo p in GameManager.Instance.PlayerList) if (p.PlayerRef.PlayerController.Inputs != null)
        {
            p.PlayerRef.PlayerController.Inputs.Map.actions[PlayerController.INPUT_NAMES[buttonType]].started -= InputValidate;
            p.PlayerRef.PlayerController.Inputs.Map.actions[PlayerController.INPUT_NAMES[buttonType]].canceled -= InputCanceled;
        }
    }

    internal void ResetVFX()
    {
        inputTxt.color = new Color(0, 0, 0, 1);
        surround.color = new Color(1, 1, 1, 1);
        button.color = new Color(1, 1, 1, 1);
        validator.color = new Color(1, 1, 1, 0);
    }

    public void SetActive(bool state) => surround.transform.parent.gameObject.SetActive(state);

    private void InputValidate(InputAction.CallbackContext ctx)
    {
        _activated = true;
        SetInputValidation(true, GameManager.Instance.PlayerList[index].PlayerRef.AssociatedColor);
        
        _inputAwaiter.ActivateChange();
    }

    private void InputCanceled(InputAction.CallbackContext ctx)
    {
        _activated = false;
        SetInputValidation(false, GameManager.Instance.PlayerList[index].PlayerRef.AssociatedColor);
    }

    public void SetInputValidation(bool state, Color pColor)
    {
        if (state)
        {
            inputTxt.DOColor(new Color(0, 0, 0, 0), .25f);
            surround.DOColor(new Color(1, 1, 1, 0), .25f);
            button.DOColor(new Color(1, 1, 1, 0), .25f);
            validator.DOColor(pColor, .25f);
        }
        else
        {
            inputTxt.DOColor(new Color(0, 0, 0, 1), .25f);
            surround.DOColor(new Color(1, 1, 1, 1), .25f);
            button.DOColor(new Color(1, 1, 1, 1), .25f);
            validator.DOColor(new Color(pColor.r, pColor.g, pColor.b, 0), .25f);
        }
    }
}

public interface IInputAwaiterReactive
{
    public IEnumerator AwaiterCompleted();
}
