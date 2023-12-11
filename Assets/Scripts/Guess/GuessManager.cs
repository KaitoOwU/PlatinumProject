using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
using System.Linq;
using TMPro;

public class GuessManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnIndividualVote;
    [HideInInspector] public UnityEvent OnGroupFinalVote;
    [HideInInspector] public UnityEvent OnVoteWrong;
    [HideInInspector] public UnityEvent OnVoteRight;

    public UnityEvent<Player, SuspectData> OnChoseSuspect;

    [Header("---References---")]
    [SerializeField] private Portrait[] _portraitsInfos;

    [SerializeField] private UIEndScreen _endScreen;

    [Header("---Debug---")]
    [SerializeField] private Dictionary<Player, SuspectData> _votes = new(); //vote from each player

    #region Event Suscription
    private void OnEnable()
    {
        OnChoseSuspect.AddListener(Vote);
    }
    private void OnDisable()
    {
        OnChoseSuspect.RemoveListener(Vote);
    }
    #endregion

    private void Start()
    {
        foreach(var v in GameManager.Instance.PlayerList) // init dict
        {
            _votes.Add(v.PlayerRef, null);
        }
        InitPortraits();
    }

    private void InitPortraits()
    {
        for(int i = 0; i < GameManager.Instance.GameData.SuspectsDatas.Length; i++)
        {
            _portraitsInfos[i].InitPortrait(GameManager.Instance.GameData.SuspectsDatas[i]);
        }
        GetPortraitFromData(GameManager.Instance.Victim).UpdateToVictim(); //Disable votes for victim portrait
    }

    private void Vote(Player player, SuspectData suspectData)
    {
        OnIndividualVote?.Invoke();
        if (_votes.ContainsKey(player))
            GetPortraitFromData(_votes[player])?.UpdateVote(player.Index - 1, false); //remove indicator from old vote portrait

        _votes[player] = suspectData;

        GetPortraitFromData(suspectData)?.UpdateVote(player.Index - 1, true); //add indicator to new vote portrait

        //Final Guess if all players have voted
        foreach(var v in _votes.Select(kv => kv.Value).ToList())
        {
            if (v == null)
                return;
        }

        StartCoroutine(GetFinalGuess());
    }

    private IEnumerator GetFinalGuess()
    {
        UIFinalVoteConfirm.instance.Init();
        yield return new WaitUntil(() => UIFinalVoteConfirm.instance.IsValid);
        
        Debug.LogError("Final Guess");
        Dictionary<SuspectData, int> finalVotes = new(); //vote for each suspect
        foreach (var v in GameManager.Instance.GameData.SuspectsDatas) { //init dict
            finalVotes.Add(v, 0); }

        foreach (var vote in _votes) {
            finalVotes[vote.Value] += 1; 
        }
        
        int maxVotes = finalVotes.Values.Max();
        SuspectData[] finalSuspects = finalVotes.Where(vote => vote.Value == maxVotes).Select(kv => kv.Key).ToArray();
        StartCoroutine(CheckFinalGuess(finalSuspects[UnityEngine.Random.Range(0, finalSuspects.Length)]));
    }

    private IEnumerator CheckFinalGuess(SuspectData finalGuess)
    {
        OnGroupFinalVote?.Invoke();

        yield return new WaitForSecondsRealtime(1f);
        
        if (finalGuess == GameManager.Instance.Murderer)
        {
            OnVoteRight?.Invoke();
            yield return StartCoroutine(UIMessageGenerator.instance.Init(false,
                new UIMessageData("The Manor", "No...\nIt can't be...\nYou guessed right?!"),
                new UIMessageData("The Manor", "You were not supposed to succeed! How could this happen!\nWell.. Get out of here! BEGONE! I don't want to ever see you again!")));
            GameManager.Instance.OnWin?.Invoke();
            _endScreen.Init(true);
        }
        else
        {
            OnVoteWrong?.Invoke();
            yield return StartCoroutine(UIMessageGenerator.instance.Init(false,
                new UIMessageData("The Manor", "Obviously, you didn't got it right.\nI don't even know why you tried."),
                new UIMessageData("The Manor", "Don't try to escape, the door is locked..."),
                new UIMessageData("The Manor", "It will be forever.", 0.2f, 1f)));
            GameManager.Instance.OnLose?.Invoke();
            _endScreen.Init(false);
        }
    }

    private Portrait GetPortraitFromData(SuspectData suspectData) => _portraitsInfos.FirstOrDefault(e => e.SuspectData == suspectData);

}
