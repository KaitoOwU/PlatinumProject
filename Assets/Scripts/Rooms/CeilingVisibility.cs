using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CeilingVisibility : MonoBehaviour
{
    private List<Player> _playersInRange = new();
    private Coroutine _coroutine;
    [SerializeField] private float _lerpTime;
    [SerializeField]
    Material _material;
    Material _materialInstance;
    MeshRenderer _meshRenderer;
    void Start()
    {
        _materialInstance = new Material(_material);
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _materialInstance;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;

        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(SwitchOpacity(0.2f));
            _materialInstance.SetFloat("_IsTextureMoving", 1);
            _meshRenderer.material = _materialInstance;

        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;

        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        if (_playersInRange.Count == 0)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(SwitchOpacity(1));        
        }
    }

    IEnumerator SwitchOpacity(float targetOpacity)
    {
        float _startOpacity = _materialInstance.GetFloat("_Opacity");
        float _index = 0;
        while(_index < 1)
        {
            _materialInstance.SetFloat("_Opacity", Mathf.Lerp(_startOpacity, targetOpacity, _index));
            _meshRenderer.material = _materialInstance;

            _index += Time.deltaTime / _lerpTime;
            yield return null;
        }
        _materialInstance.SetFloat("_Opacity", targetOpacity);
        _meshRenderer.material = _materialInstance;


        if (targetOpacity == 1)
        {
            _materialInstance.SetFloat("_IsTextureMoving", 0);
            _meshRenderer.material = _materialInstance;

        }
        _coroutine = null;
    }
}
