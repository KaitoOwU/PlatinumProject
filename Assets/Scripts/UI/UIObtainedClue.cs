using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIObtainedClue : MonoBehaviour, IInputAwaiterReactive
{
    public static UIObtainedClue instance;
    
    [SerializeField] private UIValidateInputs _inputValidator;
    [SerializeField] private TextMeshProUGUI _textOnSprite, _description;
    [SerializeField] private Image _sprite;
    [SerializeField] private CanvasGroup _group;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void Init(ClueData clue)
    {
        _textOnSprite.text = clue.Content;
        _description.text = clue.Description;
        _sprite.sprite = clue.Sprite;

        _group.DOFade(1f, 1f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        yield return _group.DOFade(0f, 1f).WaitForCompletion();
    }
}
