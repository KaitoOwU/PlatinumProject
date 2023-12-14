using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndScreen : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Button _button;
    [SerializeField] private Image _selection;
    [SerializeField] private CanvasGroup _group;

    public void Init(bool isWon)
    {
        _group.DOFade(1f, 1.5f);
        _title.text = isWon ? "You Escaped" : "Eternal Prisoner";
        EventSystem.current.SetSelectedGameObject(_button.gameObject);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
