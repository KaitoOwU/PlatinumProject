using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    private MainMenuButton[] _buttons;
    [SerializeField] private Button _buttonCredit;
    [SerializeField] private Image _background, _transition, _selection, _logo;
    [SerializeField] private RectTransform _firstSelected;
    [SerializeField] private EventSystem _events;
    [SerializeField] private CanvasGroup _mainMenu, _credits;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<MainMenuButton>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        _events.gameObject.SetActive(false);

        _transition.color = new Color(0, 0, 0, 0);
        _background.DOColor(Color.white, 4f);
        yield return _logo.DOColor(new(1, 1, 1, 1), 4f).WaitForCompletion();

        foreach (MainMenuButton button in _buttons)
        {
            button.GetComponent<TextMeshProUGUI>().DOColor(new(1, 1, 1, 1), 3f);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return _selection.DOColor(new(1, 1, 1, 1), 0.5f).WaitForCompletion();
        _events.gameObject.SetActive(true);
        _events.SetSelectedGameObject(_firstSelected.gameObject);
    }

    public void Select(MainMenuButton button)
    {
        _selection.transform.DOMove(button.transform.position, 0.5f);
    }

    public void LoadLoadingScene()
    {
        _events.gameObject.SetActive(false);
        _transition.DOColor(new Color(0, 0, 0, 1), 1.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Loading");
        });
    }

    public void LoadCredits(bool s)
    {
        if (s)
        {
            StartCoroutine(CR_LoadCredits());
        }
        else
        {
            StartCoroutine(CR_UnloadCredits());
        }
    }

    IEnumerator CR_LoadCredits()
    {
        _background.transform.DOScale(1.4f, 5f).SetEase(Ease.OutExpo);
        _events.SetSelectedGameObject(null);
        yield return _mainMenu.DOFade(0f, 1f).WaitForCompletion();
        
        _events.SetSelectedGameObject(_buttonCredit.gameObject);
        _credits.DOFade(1f, 1f);
    }
    
    IEnumerator CR_UnloadCredits()
    {
        _background.transform.DOScale(1f, 5f).SetEase(Ease.OutExpo);
        _events.SetSelectedGameObject(null);
        yield return _credits.DOFade(0f, 1f).WaitForCompletion();
        
        _events.SetSelectedGameObject(_buttons[1].gameObject);
        _mainMenu.DOFade(1f, 1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
