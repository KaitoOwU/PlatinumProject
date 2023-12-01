using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEndScreen : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Button _button;

    public void Init(bool isWon)
    {
        _title.text = isWon ? "You Won !" : "You Lost...";
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_button.gameObject);
    }
}
