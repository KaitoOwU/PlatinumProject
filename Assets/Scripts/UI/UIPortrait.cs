using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPortrait : MonoBehaviour, IInputAwaiterReactive
{
    public static UIPortrait instance;
    [SerializeField] private UIValidateInputs _validator;

    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _name, _description;
    [SerializeField] private CanvasGroup _group;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public void Init(SuspectData suspect)
    {
        _portrait.sprite = suspect.Image;
        _name.text = suspect.Name;
        _description.text = suspect.Description;

        _validator.Setup(PlayerController.EButtonType.INTERACT, "A", GameManager.Instance.PlayerList.ToArray());
        _group.DOFade(1f, 1.5f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        _validator.Unsetup(PlayerController.EButtonType.INTERACT);
        yield return new WaitForSecondsRealtime(1f);
        yield return _group.DOFade(0f, 1.5f).WaitForCompletion();
    }
}
