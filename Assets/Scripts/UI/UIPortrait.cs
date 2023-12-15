using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPortrait : MonoBehaviour, IInputAwaiterReactive
{
    public static UIPortrait instance;
    [SerializeField] private UIValidateInputs _validator;

    [SerializeField] private Image _portrait, _blood;
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
        if (GameManager.Instance.Victim == suspect)
        {
            _blood.gameObject.SetActive(true);
            _portrait.color = Color.gray;
            _description.text = suspect.Description + "<br><color=red><size=50><b>Found Dead.";
        }
        else
        {
            _blood.gameObject.SetActive(false);
            _portrait.color = Color.white;
        }

        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
        {
            p.PlayerController.Inputs.InputLocked = true;
            p.PlayerController.Animator.SetBool("IsMoving", false);
        });
        _validator.Setup(PlayerController.EButtonType.INTERACT, "A", GameManager.Instance.PlayerList.ToArray());
        _group.DOFade(1f, 1.5f);
    }
    
    public IEnumerator AwaiterCompleted()
    {
        _validator.Unsetup(PlayerController.EButtonType.INTERACT);
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
        yield return new WaitForSecondsRealtime(1f);
        yield return _group.DOFade(0f, 1.5f).WaitForCompletion();
        _validator.InputAwaiters.ToList().ForEach(awaiter => awaiter.ResetVFX());
    }
}
