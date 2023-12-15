using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        _inputValidator.Setup(PlayerController.EButtonType.INTERACT, "A", GameManager.Instance.PlayerList.ToArray());
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
        {
            p.PlayerController.Inputs.InputLocked = true;
            p.PlayerController.Animator.SetBool("IsMoving", false);
        });

        _group.DOFade(1f, 1f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        _inputValidator.Unsetup(PlayerController.EButtonType.INTERACT);
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
        yield return _group.DOFade(0f, 1f).WaitForCompletion();
        _inputValidator.InputAwaiters.ToList().ForEach(awaiter => awaiter.ResetVFX());
    }
}
