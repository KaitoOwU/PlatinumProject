using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private InputAction _action;
    [SerializeField] private TextMeshProUGUI _text, _pressA;
    [SerializeField] private float _timeBetweenTexts, _timeCharacterPrinting;
    [SerializeField] private Slider _loadingBar;
    [SerializeField, TextArea] private List<string> _loadingTexts = new();

    private bool _loadable = false, _allTextPrinted = false;
    private bool _sceneMustLoad = false;

    private void Start()
    {
        StartCoroutine(PrintTexts());
        StartCoroutine(LoadGameScene());
    }

    private void OnEnable()
    {
        _action.performed += SkipCutsceneEvent;
        _action.Enable();
    }

    private void OnDisable()
    {
        _action.performed -= SkipCutsceneEvent;
        _action.Disable();
    }

    private void SkipCutsceneEvent(InputAction.CallbackContext obj)
    {
        SkipCutscene();
    }

    private void SkipCutscene()
    {
        if (_loadable)
        {
            _text.DOColor(new Color(0, 0, 0, 1), 1f);
            _loadingBar.fillRect.GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1f);
            _pressA.DOKill();
            _pressA.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() => _sceneMustLoad = true);
        }
    }

    IEnumerator LoadGameScene()
    {
        yield return SceneManager.LoadSceneAsync("Proto1");
    }

    IEnumerator PrintTexts()
    {
        yield return new WaitForSecondsRealtime(_timeBetweenTexts);

        for (var index = 0; index < _loadingTexts.Count; index++)
        {
            var text = _loadingTexts[index];
            string[] splitedTexts = text.Split("<stop>");
            foreach (string splitedText in splitedTexts)
            {
                string printableSplitedText = splitedText.Replace("<stop>", "");
                yield return _text.DOText(_text.text + printableSplitedText, _timeCharacterPrinting * printableSplitedText.Length).WaitForCompletion();
                yield return new WaitForSecondsRealtime(_timeBetweenTexts);
            }

            if (index < _loadingTexts.Count - 1)
            {
                yield return _text.DOColor(new(0, 0, 0, 1), _timeBetweenTexts).WaitForCompletion();
                _text.text = "";
                _text.color = new Color(1, 1, 1, 1);
            }
        }

        yield return new WaitUntil(() => _loadable);
        SkipCutscene();
    }
}
