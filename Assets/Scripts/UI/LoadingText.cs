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
    [SerializeField] private Image _img, _textZone;
    [SerializeField] private TextMeshProUGUI _text, _pressA;
    [SerializeField] private float _timeBetweenTexts, _timeCharacterPrinting;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private List<TextData> _loadingTexts = new();

    private bool _allTextPrinted = false;
    private bool _sceneMustLoad = false;

    private void Start()
    {
        StartCoroutine(PrintTexts());
        StartCoroutine(LoadGameScene());
    }

    private void OnEnable()
    {
        _action.Enable();
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Proto1");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            _loadingBar.DOValue(operation.progress / 0.9f, 0.5f);
            yield return null;
        }
        
        yield return _loadingBar.DOValue(1f, 0.5f).WaitForCompletion();
        _pressA.DOColor(new Color(1, 1, 1, 1), 0.5f);

        yield return new WaitUntil(() => _allTextPrinted || _action.ReadValue<float>() > 0);
        
        _text.DOColor(new Color(0, 0, 0, 1), 1f);
        _loadingBar.fillRect.GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1f);
        _pressA.DOKill();
        _pressA.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() => _sceneMustLoad = true);

        yield return new WaitUntil(() => _sceneMustLoad);
        
        operation.allowSceneActivation = true;
    }

    IEnumerator PrintTexts()
    {
        yield return _textZone.DOFade(1f, 1f).WaitForCompletion();
        
        for (var index = 0; index < _loadingTexts.Count; index++)
        {
            TextData text = _loadingTexts[index];
            if (text.Sprite != null)
            {
                _img.sprite = text.Sprite;
            }
            else
            {
                _img.sprite = null;
                _img.color = new Color(0, 0, 0, 0);
            }

            _img.DOFade(1f, 1.5f);
            _text.text = string.Empty;
            yield return _text.DOText(text.Text, text.Text.Length * _timeCharacterPrinting).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSecondsRealtime(_timeBetweenTexts);
            
            _img.DOFade(0f, 1.5f).WaitForCompletion();
            yield return _text.DOColor(new(1, 1, 1, 0), _timeBetweenTexts).WaitForCompletion();
            _text.text = "";
            _text.color = new Color(1, 1, 1, 1);
        }

        _text.DOColor(new Color(1, 1, 1, 0), 1f);
        yield return _textZone.DOFade(0f, 1f).WaitForCompletion();

        _allTextPrinted = true;
    }
}

[Serializable]
public struct TextData
{
    [TextArea] public string Text;
    public Sprite Sprite;
}
