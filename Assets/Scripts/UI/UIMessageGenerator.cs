using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIMessageGenerator : MonoBehaviour
{
    public static UIMessageGenerator instance;
    [SerializeField] private TextMeshProUGUI _narrator, _message;
    [SerializeField] private Image _state;
    [SerializeField] private CanvasGroup _group, _skipGroup;
    [SerializeField] private InputAction _input;

    public CanvasGroup Group => _group;
    
    private int _inputSkip;
    private bool _skipUI;
    private const float _TIME_TO_VALIDATE = 1.75f;
    
    private List<int> _validatedControllers;
    public bool Skip { get; private set; }

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    private void Update()
    {
        if (_inputSkip >= 1)
        {
            if (!_skipUI)
            {
                _skipGroup.DOFade(1f, 0.5f);
                _skipUI = true;
            }
            
            _state.fillAmount = Mathf.Clamp01(_state.fillAmount + Time.deltaTime / _TIME_TO_VALIDATE);
            if (_state.fillAmount >= 1f)
                Skip = true;
        }
        else if (!Skip)
        {
            _state.fillAmount = Mathf.Clamp01(_state.fillAmount - Time.deltaTime / _TIME_TO_VALIDATE);
            if (_skipUI && _state.fillAmount <= 0f)
            {
                _skipGroup.DOFade(0f, 0.5f);
                _skipUI = false;
            }
        }
    }

    public IEnumerator Init(bool skippable, params UIMessageData[] messages)
    {
        Skip = false;
        _state.fillAmount = 0f;
        _skipGroup.alpha = 0f;
        _narrator.text = messages[0].narrator;
        _message.text = string.Empty;

        if (skippable)
        {
            _input.started += InputValidate;
            _input.canceled += InputCanceled;
            _input.Enable();
        }
        else
        {
            _input.started -= InputValidate;
            _input.canceled -= InputCanceled;
            _input.Disable();
        }

        yield return _group.DOFade(1f, 1f).WaitForCompletion();
        foreach (UIMessageData message in messages)
        {
            _narrator.text = message.narrator;
            _message.text = string.Empty;
            _message.color = Color.white;

            yield return _message.DOText(message.text, message.printDurationPerLetter * message.text.Length)
                .SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSecondsRealtime(message.stayDuration);
            yield return _message.DOColor(new Color(1, 1, 1, 0), 1f).WaitForCompletion();
        }

        yield return _group.DOFade(0f, 1f).WaitForCompletion();
        Skip = true;
    }

    private void InputValidate(InputAction.CallbackContext obj)
    {
        _inputSkip++;
    }

    private void InputCanceled(InputAction.CallbackContext obj)
    {
        _inputSkip--;
    }
}

public struct UIMessageData
{
    public readonly string narrator;
    public readonly string text;
    public readonly float printDurationPerLetter;
    public readonly float stayDuration;

    public UIMessageData(string narrator, string text, float printDurationPerLetter = 0.1f, float stayDuration = 3f)
    {
        this.narrator = narrator;
        this.text = text;
        this.printDurationPerLetter = printDurationPerLetter;
        this.stayDuration = stayDuration;
    }
}
