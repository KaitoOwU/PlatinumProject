using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _music;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _music;
        //GameManager.Instance.OnEachMinute.AddListener(StartMusic);
        //GameManager.Instance.OnFadeMusic.AddListener(StopMusic);
    }

//    private void StartMusic()
//    {
//        int count = GameManager.Instance.GameData.TimerValues.FirstPhaseTime
//        + GameManager.Instance.GameData.TimerValues.SecondPhaseTime
//        + GameManager.Instance.GameData.TimerValues.ThirdPhaseTime
//        - 60;
//        if (GameManager.Instance.Timer == count)
//        {
//            _audioSource.Play();
//        }
//    }
//    private void StopMusic()
//    {
//        DOTween.To(x => _audioSource.volume = x, 1, 0, 3f);
//    }
}
