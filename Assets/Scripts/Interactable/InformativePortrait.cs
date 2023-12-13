using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformativePortrait : Interactable
{

    [SerializeField] private Image _sprite, _blood;
    [SerializeField] private SuspectData _suspect;

    protected override void Awake()
    {
        _sprite.sprite = _suspect.Image;
    }

    private void Start()
    {
        if (GameManager.Instance.Victim == _suspect)
        {
            _blood.gameObject.SetActive(true);
            _sprite.color = Color.gray;
        }
    }

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        UIPortrait.instance.Init(_suspect);
    }
}
