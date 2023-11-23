using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _music;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _music;
        GameManager.Instance.OnEachMinute.AddListener(StartMusic);
        GameManager.Instance.OnFadeMusic.AddListener(StopMusic);
    }

    private void StartMusic()
    {
        if(GameManager.Instance.Timer < 2)
        {
            Debug.Log("Start Music");
            _audioSource.Play();
        }
    }
    private void StopMusic()
    {
        Debug.Log("Start stop Music");
        DOTween.To(x => _audioSource.volume = x, 1, 0, 3f);
    }
}
