using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private List<int> _validatedControllers;
    private bool _isInputPressed = false;
    private bool _skippable;
    public Coroutine messages;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    private void Update()
    {
        if (_skippable)
        {
            if (_isInputPressed)
            {
                _state.fillAmount = Mathf.Clamp01(_state.fillAmount + Time.deltaTime);
                if (_state.fillAmount >= 1f)
                {
                    StartCoroutine(SkipMessage());
                }
            }
            else
            {
                _state.fillAmount = Mathf.Clamp01(_state.fillAmount - Time.deltaTime);
            }
        }
    }

    private void Start()
    {
        if (_skippable)
        {
            _input.started += SetActionValidatedTrue;
            _input.canceled += SetActionValidatedFalse;

            _input.Enable();
        }
    }

    private void SetActionValidatedTrue(InputAction.CallbackContext obj)
    {
        _isInputPressed = true;
        _skipGroup.DOFade(1f, 1f);
    }

    private void SetActionValidatedFalse(InputAction.CallbackContext obj)
    {
        _isInputPressed = false;
        _skipGroup.DOFade(0f, 1f);
    }

    public IEnumerator Init(bool skippable, params UIMessageData[] messages)
    {
        _narrator.text = messages[0].narrator;
        _message.text = string.Empty;
        _skippable = skippable;

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
    }

    private IEnumerator SkipMessage()
    {
        StopCoroutine(messages);
        yield return _group.DOFade(0f, 1f).WaitForCompletion();
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
