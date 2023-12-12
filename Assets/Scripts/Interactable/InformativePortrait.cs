using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformativePortrait : Interactable
{

    [SerializeField] private Image _sprite;
    [SerializeField] private SuspectData _suspect;

    protected override void Awake()
    {
        _sprite.sprite = _suspect.Image;
    }

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        UIPortrait.instance.Init(_suspect);
    }
}
