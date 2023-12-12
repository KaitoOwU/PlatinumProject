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
    
    private List<int> _validatedControllers;
    public Coroutine messages;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public IEnumerator Init(bool skippable, params UIMessageData[] messages)
    {
        _narrator.text = messages[0].narrator;
        _message.text = string.Empty;

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
