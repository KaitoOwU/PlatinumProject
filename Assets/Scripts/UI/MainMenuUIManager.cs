using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    private MainMenuButton[] _buttons;
    [SerializeField] private Image _transition;
    [SerializeField] private EventSystem _events;

private void Awake()
    {
        _buttons = GetComponentsInChildren<MainMenuButton>();
    }

    public void DeselectAll()
    {
        _buttons.ToList().ForEach((button) =>
        {
            DOTween.Kill(button.SelectImg.transform);
            button.SelectImg.transform.DOScaleX(0, 0.2f).SetEase(Ease.OutExpo);
        });
    }

    public void LoadLoadingScene()
    {
        _events.gameObject.SetActive(false);
        _transition.DOColor(new Color(0, 0, 0, 1), 2f).OnComplete(() =>
        {
            SceneManager.LoadScene("Loading");
        });
    }
}
