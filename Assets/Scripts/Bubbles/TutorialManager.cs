using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TutorialManager;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance = null;
    public static TutorialManager Instance => instance;

    [SerializeField] Bubble _messageBubblePrefab;
    [SerializeField] int _poolSize;
    [SerializeField] float _bubbleYOffset = -50;
    [SerializeField] List<Bubble> _bubblePool = new();
    RectTransform _canvasRect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        _canvasRect = GetComponent<RectTransform>();
        _InitPool(_poolSize);
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

    public Bubble ShowBubbleMessage(int playerIndex, Transform objectPosition, int triggerController, string message)
    {

        Bubble bubble = GetPoolBubble().InitText(triggerController, message);
        if (bubble == null)
            return null;
        bubble.InitText(triggerController, message);
        bubble.transform.localPosition = _GetCanvasPos(playerIndex, objectPosition.position);
        bubble.gameObject.SetActive(true);

        StartCoroutine(_WaitToDestroy(bubble));
        return bubble;
    }

    private IEnumerator _WaitToDestroy(Bubble bubble)
    {

        yield return new WaitForSeconds(1f);
        bubble.gameObject.SetActive(false);
    }

    private Vector3 _GetCanvasPos(int playerIndex, Vector3 worldPos)
    {
        Vector2 WorldObject_ScreenPosition;
        Vector2 ViewportPosition;
        switch (GameManager.Instance.PlayerList[playerIndex - 1].PlayerRef.RelativePos)
        {
            default:
            case HubRelativePosition.HUB:
                ViewportPosition = GameManager.Instance.FullCamera.WorldToViewportPoint(worldPos);
                WorldObject_ScreenPosition = new Vector2(
                (ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
                (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
                break;
            case HubRelativePosition.LEFT_WING:
                ViewportPosition = GameManager.Instance.CameraLeft.WorldToViewportPoint(worldPos);
                WorldObject_ScreenPosition = new Vector2(
                (ViewportPosition.x * _canvasRect.sizeDelta.x / 2) - (_canvasRect.sizeDelta.x / 2 * 0.5f) - _canvasRect.sizeDelta.x / 4,
                (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
                break;
            case HubRelativePosition.RIGHT_WING:
                ViewportPosition = GameManager.Instance.CameraRight.WorldToViewportPoint(worldPos);
                WorldObject_ScreenPosition = new Vector2(
                (ViewportPosition.x * _canvasRect.sizeDelta.x / 2) - (_canvasRect.sizeDelta.x / 2 * 0.5f) + _canvasRect.sizeDelta.x/4,
                (ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f) + _bubbleYOffset);
                break;
        }
        return WorldObject_ScreenPosition;
    }
}
