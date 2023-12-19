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
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] GameObject _shortBackground;
    [SerializeField] GameObject _longBackground;
    [SerializeField] GameObject _lockIcon;
    [SerializeField] GameObject _AIcon;
    [SerializeField] GameObject _RTIcon;

    public CanvasGroup CanvasGroup { get => _canvasGroup; set => _canvasGroup = value; }
    public GameObject ShortBackground { get => _shortBackground; set => _shortBackground = value; }
    public GameObject LongBackground { get => _longBackground; set => _longBackground = value; }
    public TMP_Text Text => _text;


    public int ControllerIndexRef => controllerIndexRef;
    private int controllerIndexRef;

    public EBubbleType BubbleType => _bubbleType;
    public EBubbleType _bubbleType;

    public enum EBubbleType
    {
        TEXT,
        PLAYER,
        LOCK
    }
    private void Start()
    {
        //transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public Bubble InitText(int triggerPlayer, string text)
    {
        controllerIndexRef = triggerPlayer;
        _bubbleType = EBubbleType.TEXT;
        _lockIcon.gameObject.SetActive(false);
        _text.text = text;

        if (text == "A")
        {
            _AIcon.SetActive(true);
            _RTIcon.SetActive(false);
            _text.gameObject.SetActive(false);
        }
        else if (text == "RT")
        {
            _AIcon.SetActive(false);
            _RTIcon.SetActive(true);
            _text.gameObject.SetActive(false);
        }
        else
        {
            _AIcon.SetActive(false);
            _text.gameObject.SetActive(true);
            _RTIcon.SetActive(false);
        }

        return this;
    }
    public Bubble InitLock(int triggerPlayer)
    {
        controllerIndexRef = triggerPlayer;
        _bubbleType = EBubbleType.LOCK;

        _text.gameObject.SetActive(false);
        _AIcon.SetActive(false);
        _RTIcon.SetActive(false);

        _lockIcon.gameObject.SetActive(true);
        _shortBackground.SetActive(true);
        _longBackground.SetActive(false);

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


