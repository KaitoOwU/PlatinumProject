using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    [SerializeField] GameObject[] _playerIconsPrefab;
    [SerializeField] GameObject[] _controllersIconsPrefab;
    [SerializeField] TMP_Text _text;

    public int ControllerIndexRef => controllerIndexRef;
    private int controllerIndexRef;

    public EBubbleType BubbleType => _bubbleType;
    public EBubbleType _bubbleType;

    public enum EBubbleType
    {
        TEXT,
        PLAYER
    }
    private void Start()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public Bubble InitText(int triggerPlayer, string text)
    {
        _text.text = text;
        controllerIndexRef = triggerPlayer;
        _bubbleType = EBubbleType.TEXT;
        return this;
    }
    public Bubble InitPlayer(int targetPlayerIndex, int targetControllerIndex)
    {
        Instantiate(_playerIconsPrefab[targetPlayerIndex], transform);
        _text.text = "";
        controllerIndexRef = targetControllerIndex;
        _bubbleType = EBubbleType.PLAYER;
        return this;
    }
    public Bubble InitController(int targetControllerIndex)
    {
        GetComponent<Image>().enabled = false;
        Instantiate(_controllersIconsPrefab[targetControllerIndex], transform);
        _text.text = "";
        controllerIndexRef = targetControllerIndex;
        _bubbleType = EBubbleType.PLAYER;
        return this;
    }
}


