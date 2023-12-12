using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Statue : Puzzle, IResettable
{
        [HideInInspector] public UnityEvent OnRepaired;

    private bool _isRepaired;
    [SerializeField]
    protected string _onRangeWithArmMessage;
    [SerializeField] private GameObject _statueArm;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;

        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        p.Inputs.OnInteract.AddListener(OnInteract);

        _onPlayerEnterRange?.Invoke();

        if (_isRepaired)
            return;

        if ((_message == null || !_message.gameObject.activeSelf))
        {
            if(GameManager.Instance.PlayerList[p.PlayerIndex-1].PlayerRef.HeldPickable == null && _onRangeMessage != "")
                _message = TutorialManager.Instance.ShowBubbleMessage(p.PlayerIndex, transform, p.Inputs.ControllerIndex, _onRangeMessage, TutorialManager.E_DisplayStyle.STAY);
            else if(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef.HeldPickable != null && _onRangeWithArmMessage != "")
                _message = TutorialManager.Instance.ShowBubbleMessage(p.PlayerIndex, transform, p.Inputs.ControllerIndex, _onRangeWithArmMessage, TutorialManager.E_DisplayStyle.STAY);

        }
    }

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        if (player.HeldPickable == null)
            return;
        
        if (!_isRepaired && player.HeldPickable.ID == 10) // BRAS DE STATUE
        {
            OnRepaired?.Invoke();
            player.HeldPickable = null;
            _statueArm.SetActive(true);
            _isRepaired = true;
            Reactive.PuzzleCompleted();
        }
    }


    public void ResetAsDefault()
    {
        _statueArm.SetActive(false);
        _isRepaired = false;
    }
}
