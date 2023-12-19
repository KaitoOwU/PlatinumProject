using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHubTpManager : MonoBehaviour, IInputAwaiterReactive
{
    public static UIHubTpManager instance;
    
    [SerializeField] private UIValidateInputs _inputValidator;
    [SerializeField] private Image _timer;
    
    private bool _isActive;
    private int _count;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public void PrintUI(Player player, float seconds, string input = "X")
    {
        if (_isActive)
            return;

        if (player.RelativePos == HubRelativePosition.HUB)
            return;

        _isActive = true;
        _timer.color = Color.white;
        _timer.fillAmount = 1f;
        transform.DOMoveY(190f, 1.5f).SetEase(Ease.OutExpo);
        _inputValidator.Setup(PlayerController.EButtonType.ASK_TP_HUB, input, GameManager.Instance.PlayerList.ToArray());

        _timer.DOFillAmount(0f, seconds).SetEase(Ease.Linear).OnComplete(() =>
        {
            _timer.color = new Color(1, 1, 1, 0);
            _timer.fillAmount = 1f;
            _timer.DOColor(new Color(.7f, 0, 0, .4f), .5f);
            StartCoroutine(PlayUncompleteAnimation());
        });
    }

    public IEnumerator AwaiterCompleted()
    {
        _inputValidator.InputAwaiters.ToList().ForEach(awaiter => awaiter.UnsetupInputs(PlayerController.EButtonType.ASK_TP_HUB));
        _timer.DOKill();
        _timer.DOColor(new Color(0, 0.7f, 0, 0.4f), .5f);

        yield return new WaitForSecondsRealtime(1.5f);

        GameManager.Instance.TPAllPlayersToHub();
        
        yield return transform.DOMoveY(-190f, .5f).SetEase(Ease.InExpo).WaitForCompletion();
        _inputValidator.InputAwaiters.ToList().ForEach(awaiter => awaiter.ResetVFX());
        _isActive = false;
    }


    private IEnumerator PlayUncompleteAnimation()
    {
        _inputValidator.Unsetup(PlayerController.EButtonType.ASK_TP_HUB);
        _timer.DOKill();
        _timer.DOColor(new Color(0.7f, 0, 0, 0.4f), .5f);

        yield return new WaitForSecondsRealtime(1.5f);
        yield return transform.DOMoveY(-190f, .25f).SetEase(Ease.InExpo).WaitForCompletion();
        _inputValidator.InputAwaiters.ToList().ForEach(awaiter => awaiter.ResetVFX());
        _isActive = false;
    }
}
