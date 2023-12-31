using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIClue : MonoBehaviour, IInputAwaiterReactive
{
    public static UIClue right, left;
    
    [SerializeField] private UIValidateInputs _inputValidator;
    [SerializeField] private HubRelativePosition _position;
    [SerializeField] private Image _clueVisual;
    [SerializeField] private TextMeshProUGUI _clueText, _clueDescription;
    [SerializeField] private CanvasGroup _group;
    
    public bool IsActive { get; private set; }
    public Image ClueVisual => _clueVisual;
    public TextMeshProUGUI ClueText => _clueText;
    public TextMeshProUGUI ClueDescription => _clueDescription;

    private void Awake()
    {
        if (right == null && _position == HubRelativePosition.RIGHT_WING)
            right = this;
        else if (left == null && _position == HubRelativePosition.LEFT_WING)
            left = this;
        else Destroy(gameObject);
    }

    public void Init(Sprite clueVisual, string clueDesc, string clueText = "")
    {
        _clueVisual.sprite = clueVisual;
        _clueDescription.text = clueDesc;
        _clueText.text = clueText;

        InitUI();
    }
    
    public void Init(ClueData clue)
    {
        _clueDescription.text = clue.Description ?? string.Empty;
        _clueText.text = clue.Content ?? string.Empty;
        _clueVisual.sprite = clue.Sprite ?? null;

        InitUI();
    }

    private void InitUI()
    {
        IsActive = true;
        GameManager.Instance.OnEachEndPhase.AddListener(ClearUI);
        ClearUI();
        if (_position == HubRelativePosition.LEFT_WING)
        {
            _inputValidator.Setup(PlayerController.EButtonType.INTERACT, "A", GameManager.Instance.LeftPlayers.ToArray());
        } else if (_position == HubRelativePosition.RIGHT_WING)
        {
            _inputValidator.Setup(PlayerController.EButtonType.INTERACT, "A", GameManager.Instance.RightPlayers.ToArray());
        }

        _group.DOFade(1f, 1.5f);
    }
    
    private void ClearUI()
    {
        _group.DOFade(0f, 1.5f);
    }

    public IEnumerator AwaiterCompleted()
    {
        _inputValidator.InputAwaiters.ToList().ForEach(awaiter => awaiter.Unsetup(PlayerController.EButtonType.INTERACT));
        yield return new WaitForSecondsRealtime(1.5f);
        transform.DOLocalMoveY(-1131, 1.5f);
    }
}
