using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : Interactable
{
    [SerializeField] private Image _image;
    private ClueData _data;
    protected override void OnInteract(Player player)
    {
        UIObtainedClue.instance.Init(_data);
        
    }

    public void Init(ClueData item)
    {
        _data = item;
        _image.sprite = item.Sprite;
    }
}
