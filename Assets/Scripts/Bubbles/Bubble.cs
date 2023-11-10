using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bubble : MonoBehaviour
{
    [SerializeField] GameObject[] _playerIconsPrefab;
    [SerializeField] TMP_Text _text;

    public Player PlayerRef => _playerRef;
    private Player _playerRef;

    public EBubbleType BubbleType => _bubbleType;
    public EBubbleType _bubbleType;

    public enum EBubbleType
    {
        TEXT,
        PLAYER
    }


    public Bubble Init(Player triggerPlayer, string text)
    {
        _text.text = text;
        _playerRef = triggerPlayer;
        _bubbleType = EBubbleType.TEXT;
        return this;
    }
    public Bubble Init(Player targetPlayer)
    {
        Instantiate(_playerIconsPrefab[targetPlayer.Index - 1], transform);
        _text.text = "";
        _playerRef = targetPlayer;
        _bubbleType = EBubbleType.PLAYER;
        return this;
    }
}


