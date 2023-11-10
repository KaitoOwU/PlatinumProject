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
    [SerializeField] private Image _transition, _selection, _logo;
    [SerializeField] private RectTransform _firstSelected;
    [SerializeField] private EventSystem _events;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<MainMenuButton>();
    }

    private void Start()
    {
        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        _events.gameObject.SetActive(false);
        
        yield return new WaitForSecondsRealtime(2f);
        yield return _logo.DOColor(new(1, 1, 1, 1), 4f).WaitForCompletion();

        foreach (MainMenuButton button in _buttons)
        {
            button.GetComponent<TextMeshProUGUI>().DOColor(new(1, 1, 1, 1), 3f);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return new WaitForSecondsRealtime(1.5f);
        yield return _selection.DOColor(new(1, 1, 1, 1), 1f).WaitForCompletion();
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
}
