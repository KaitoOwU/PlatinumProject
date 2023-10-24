using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Portrait : Interactable
{
    [Header("---References---")]
    [SerializeField] private GuessManager _guessManager;
    [SerializeField] private Image _imageComponent;
    [SerializeField] private TMP_Text _textComponent;
    [SerializeField] private GameObject[] _votesIndicators;
    private SuspectData _suspectData;
    public SuspectData SuspectData => _suspectData;


    public void InitPortrait(SuspectData suspect)
    {
        _suspectData = suspect;
        _textComponent.text = suspect.Name;
        _imageComponent.sprite = suspect.Image;
    }
    public void UpdateToVictim()
    {
        _imageComponent.color = Color.red;
        GetComponent<Collider>().enabled = false;
    }

    public void UpdateVote(int playerIndex, bool isVoted) => _votesIndicators[playerIndex].SetActive(isVoted);
    

    protected override void OnInteract(Player player)
    {
        _guessManager.OnChoseSuspect?.Invoke(player, _suspectData);
    }

}
