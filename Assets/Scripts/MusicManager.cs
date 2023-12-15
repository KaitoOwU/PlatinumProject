using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioClip _endMusic;

    void Start()
    {
        UIPauseMenu.instance.OnMusicVolumeChange.AddListener(UpdateVolume);
        _audioSource = GetComponent<AudioSource>();
        _audioSourceMusic.clip = _endMusic;
        _audioSourceMusic.Stop();
        GameManager.Instance.OnEachMinute.AddListener(AnalyseMinute);
    }

    void UpdateVolume(float volume)
    {
        _audioSource.volume = volume;
        _audioSourceMusic.volume = volume;
    }

    private void AnalyseMinute()
    {
        int count = GameManager.Instance.GameData.TimerValues.FirstPhaseTime
        + GameManager.Instance.GameData.TimerValues.SecondPhaseTime
        + GameManager.Instance.GameData.TimerValues.ThirdPhaseTime;

        if (GameManager.Instance.Timer == 60
            || GameManager.Instance.Timer == GameManager.Instance.GameData.TimerValues.FirstPhaseTime + 60
            || GameManager.Instance.Timer == GameManager.Instance.GameData.TimerValues.FirstPhaseTime + GameManager.Instance.GameData.TimerValues.SecondPhaseTime + 60
            )
        {
            StartMusic();
        }
    }

    private void StartMusic()
    {
        _audioSourceMusic.Play();
        DOTween.To(x => _audioSourceMusic.volume = x, 0, 1, 2f);
    }
}
