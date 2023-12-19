using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UIFinalVoteConfirm : MonoBehaviour, IInputAwaiterReactive
{
    public static UIFinalVoteConfirm instance;
    [SerializeField] private UIValidateInputs _validator;
    [SerializeField] private CanvasGroup _group;

    public bool IsValid { get; private set; } = false;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public void Init()
    {
        IsValid = false;
        _validator.Setup(PlayerController.EButtonType.BACK, "B", GameManager.Instance.PlayerList.ToArray());
        _group.DOFade(1f, 1.5f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        yield return _group.DOFade(0f, 1.5f);
        IsValid = true;
    }
}
