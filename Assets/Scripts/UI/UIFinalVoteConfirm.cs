using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIFinalVoteConfirm : MonoBehaviour, IInputAwaiterReactive
{
    public static UIFinalVoteConfirm instance;
    [SerializeField] private UIValidateInputs _validator;

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
        _validator.Setup(PlayerController.EButtonType.TOOL, "B", GameManager.Instance.PlayerList.ToArray());
        transform.DOLocalMoveY(190f, 1.5f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        yield return transform.DOLocalMoveY(-500f, 1.5f).WaitForCompletion();
        IsValid = true;
    }
}
