using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    private static BubbleManager instance = null;
    public static BubbleManager Instance => instance;

    [SerializeField] Camera _cam;
    [SerializeField] Bubble _bubblePrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public enum EBubblePos
    {
        LEFT,
        MIDDLE_LEFT,
        MIDDLE_RIGHT,
        RIGHT,
    }

    public Bubble ShowBubble(Transform objectPosition, Player triggerPlayer, string message, EBubblePos bubblePos)
    {
        Bubble bubble = Instantiate(_bubblePrefab, Vector3.zero, Quaternion.identity, transform).Init(triggerPlayer, message);
        bubble.GetComponent<RectTransform>().anchoredPosition = GetCanvasPos(objectPosition.position, bubblePos);
        return bubble;
    }

    public Bubble ShowPlayerIcon(Transform objectPosition, Player targetPlayer, EBubblePos bubblePos)
    {
        Bubble bubble = Instantiate(_bubblePrefab, Vector3.zero, Quaternion.identity, transform).Init(targetPlayer);
        bubble.GetComponent<RectTransform>().anchoredPosition = GetCanvasPos(objectPosition.position, bubblePos);
        return bubble;
    }

    Vector3 GetCanvasPos(Vector3 worldPos, EBubblePos bubblePos)
    {
        RectTransform CanvasRect = this.GetComponent<RectTransform>();

        Vector2 ViewportPosition = _cam.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        (((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f))) + (-75 + (50 * (int)bubblePos)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) - 50);

        return WorldObject_ScreenPosition;
    }
}
