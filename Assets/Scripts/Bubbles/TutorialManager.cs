using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static TutorialManager;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance = null;
    public static TutorialManager Instance => instance;

    [SerializeField] Bubble _messageBubblePrefab;
    [SerializeField] float _messageDisplayTime;
    [SerializeField] int _poolSize;
    [SerializeField] float _bubbleYOffset = -50;
    [SerializeField] List<Bubble> _bubblePool = new();
    [SerializeField] AnimationCurve _elasticIn;
    RectTransform _canvasRect;

    public enum E_DisplayStyle
    {
        STAY,
        FADE,
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        _canvasRect = GetComponent<RectTransform>();
        _InitPool(_poolSize);
    }
    private void Start()
    {
        GameManager.Instance.OnTPToHub.AddListener(_HideAllBubbles);    
        
    }

    private void _InitPool(int poolSize)
    {
        for(int i  = 0; i < poolSize; i++)
        {
            Bubble bubble = Instantiate(_messageBubblePrefab, Vector3.zero, Quaternion.identity, transform);
            bubble.gameObject.SetActive(false);
            _bubblePool.Add(bubble);
        }
    }
    public Bubble GetPoolBubble()
    {
        foreach(Bubble bubble in _bubblePool)
        {
            if (!bubble.gameObject.activeSelf)
            {
                return bubble;
            }
        }
        return null;
    }

    public Bubble ShowBubbleMessage(int playerIndex, Transform objectPosition, int triggerController, string message, E_DisplayStyle displayStyle, Vector3 offset = default)
    {
        Bubble bubble = GetPoolBubble();
        if (bubble == null)
            return null;
        bubble.InitText(triggerController, message);
        bubble.transform.localPosition = _GetCanvasPos(playerIndex, objectPosition.position) + offset;
        if(message.Count() <= 2) //if message short
        {
            bubble.ShortBackground.SetActive(true);
            bubble.LongBackground.SetActive(false);
            _ShowShortMessage(bubble);
        }
        else //if message long
        {
            bubble.ShortBackground.SetActive(false);
            bubble.LongBackground.SetActive(true);
            _ShowLongMessage(bubble);
        }
        if (displayStyle == E_DisplayStyle.FADE)
            HideBubble(bubble, 1f);
        return bubble;
    }
    public Bubble ShowLockedBubble(int playerIndex, Transform objectPosition, int triggerController, E_DisplayStyle displayStyle)
    {
        Bubble bubble = GetPoolBubble();
        if (bubble == null)
            return null;
        bubble.InitLock(triggerController);
        bubble.transform.localPosition = _GetCanvasPos(playerIndex, objectPosition.position);

        _ShowShortMessage(bubble);
        if (displayStyle == E_DisplayStyle.FADE)
            HideBubble(bubble, 1f);
        return bubble;
    }

    private void _ShowLongMessage(Bubble bubble)
    {
        bubble.transform.localScale = new Vector3(0, 0, 0);
        bubble.CanvasGroup.alpha = 0;
        bubble.gameObject.SetActive(true);
        bubble.CanvasGroup.DOFade(1, 0.5f);
        bubble.transform.DOScale(0.8f, 0.5f).SetEase(Ease.OutCirc);

    }
    private void _ShowShortMessage(Bubble bubble)
    {
        bubble.transform.localScale = new Vector3(0,0,0);
        bubble.CanvasGroup.alpha = 0;
        bubble.gameObject.SetActive(true); 
        bubble.CanvasGroup.DOFade(1, 0.1f);
        bubble.transform.DOScale(0.8f, 0.5f).SetEase(_elasticIn);
    }
    public void HideBubble(Bubble bubble, float waitTime)
    {
        StartCoroutine(_HideBubbleCoroutine(bubble, waitTime));
    }
    private IEnumerator _HideBubbleCoroutine(Bubble bubble, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(bubble.Text.text.Count() <= 2 || bubble.BubbleType == Bubble.EBubbleType.LOCK)
            bubble.transform.DOScale(0, 0.5f).SetEase(Ease.InExpo);
        yield return bubble.CanvasGroup.DOFade(0, 0.5f).WaitForCompletion();
        bubble.gameObject.SetActive(false);
    }

    private void _HideAllBubbles()
    {
        foreach (Bubble bubble in _bubblePool)
        {
            if (bubble.gameObject.activeSelf)
            {
                HideBubble(bubble, 0);
            }
        }
    }

    private Vector3 _GetCanvasPos(int playerIndex, Vector3 worldPos)
    {
        Vector2 WorldObject_ScreenPosition = new();
        Vector2 ViewportPosition = new();
        Player p = GameManager.Instance.PlayerList[playerIndex - 1].PlayerRef;
        switch (p.RelativePos)
        {
            default:
            case HubRelativePosition.HUB:
                if(GameManager.Instance.CurrentCameraState == GameManager.CameraState.FULL)
                {
                    SetFullUIPosition(ref ViewportPosition, ref WorldObject_ScreenPosition, worldPos);

                }
                else
                {
                    Player otherSidePlayer = GameManager.Instance.PlayerList.FirstOrDefault(p => p.PlayerRef.RelativePos != HubRelativePosition.HUB).PlayerRef;
                    if (otherSidePlayer is null)
                        return Vector3.zero;
                    if(otherSidePlayer.RelativePos == HubRelativePosition.LEFT_WING)
                    {
                        SetLeftUIPosition(ref ViewportPosition, ref WorldObject_ScreenPosition, worldPos);
                    }
                    else
                    {
                        SetRightUIPosition(ref ViewportPosition, ref WorldObject_ScreenPosition, worldPos);
                    }
                }
                break;
            case HubRelativePosition.LEFT_WING:
                SetLeftUIPosition(ref ViewportPosition, ref WorldObject_ScreenPosition, worldPos);
                break;
            case HubRelativePosition.RIGHT_WING:
                SetRightUIPosition(ref ViewportPosition, ref WorldObject_ScreenPosition, worldPos);
                break;
        }
        return WorldObject_ScreenPosition;
    }

    void SetLeftUIPosition(ref Vector2 ViewportPosition, ref Vector2 WorldObject_ScreenPosition, Vector3 worldPos)
    {
        ViewportPosition = GameManager.Instance.CameraLeft.WorldToViewportPoint(worldPos);
        WorldObject_ScreenPosition = new Vector2(
        (ViewportPosition.x * _canvasRect.sizeDelta.x / 2) - (_canvasRect.sizeDelta.x / 2 * 0.5f) - _canvasRect.sizeDelta.x / 4,
        (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
    }
    void SetRightUIPosition(ref Vector2 ViewportPosition, ref Vector2 WorldObject_ScreenPosition, Vector3 worldPos)
    {
        ViewportPosition = GameManager.Instance.CameraRight.WorldToViewportPoint(worldPos);
        WorldObject_ScreenPosition = new Vector2(
        (ViewportPosition.x * _canvasRect.sizeDelta.x / 2) - (_canvasRect.sizeDelta.x / 2 * 0.5f) + _canvasRect.sizeDelta.x / 4,
        (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
    }
    void SetFullUIPosition(ref Vector2 ViewportPosition, ref Vector2 WorldObject_ScreenPosition, Vector3 worldPos)
    {
        ViewportPosition = GameManager.Instance.FullCamera.WorldToViewportPoint(worldPos);
        WorldObject_ScreenPosition = new Vector2(
        (ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
        (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
    }
}
