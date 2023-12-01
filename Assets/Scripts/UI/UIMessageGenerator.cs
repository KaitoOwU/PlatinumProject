using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIMessageGenerator : MonoBehaviour
{
    public static UIMessageGenerator instance;
    [SerializeField] private TextMeshProUGUI _narrator, _message;
    [SerializeField] private CanvasGroup _group;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public IEnumerator Init(params UIMessageData[] messages)
    {
        _narrator.text = messages[0].narrator;
        _message.text = string.Empty;
        
        yield return _group.DOFade(1f, 1f).WaitForCompletion();
        foreach (UIMessageData message in messages)
        {
            _narrator.text = message.narrator;
            _message.text = string.Empty;
            _message.color = Color.white;

            yield return _message.DOText(message.text, message.printDurationPerLetter * message.text.Length).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSecondsRealtime(message.stayDuration);
            yield return _message.DOColor(new Color(1, 1, 1, 0), 1f).WaitForCompletion();
        }

        yield return _group.DOFade(0f, 1f);
    }
}

public struct UIMessageData
{
    public string narrator;
    public string text;
    public float printDurationPerLetter;
    public float stayDuration;

    public UIMessageData(string narrator, string text, float printDurationPerLetter = 0.1f, float stayDuration = 5f)
    {
        this.narrator = narrator;
        this.text = text;
        this.printDurationPerLetter = printDurationPerLetter;
        this.stayDuration = stayDuration;
    }
}
