using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : Interactable
{
    [SerializeField] private Image _image;
    [SerializeField] private Transform _vfx;
    private ClueData _data;
    private Material _outlineMat;
    [SerializeField] private Color _outlineColor;
    [SerializeField] private float _outlineFadeDuration = 0.1f;
    private bool _isOutlined;
    private Coroutine _currentFade;

    private void Start()
    {
        _outlineMat = _image.material;
        _outlineMat.SetFloat("_BaseContourSize", 0f);
        _outlineMat.SetColor("_Color", Color.white);
        _isOutlined = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(!_isOutlined && _currentFade == null)
        {
            _currentFade = StartCoroutine(AddOutline(_outlineColor));   
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (_isOutlined == true)
        {
            if(_currentFade != null)
            {
                StopCoroutine(_currentFade);
                _currentFade = null;
            }
            _currentFade = StartCoroutine(RemoveOutline());
        }
    }
    protected override void OnInteract(Player player)
    {
        UIObtainedClue.instance.Init(_data);        
    }

    public void Init(ClueData item)
    {
        _data = item;
        _image.sprite = item.Sprite;
        _vfx.localRotation = Quaternion.Euler(0, 0, Random.Range(-90f, 90f));
    }
    private IEnumerator AddOutline(Color newColor)
    {
        DOTween.To(x => _outlineMat.SetFloat("_BaseContourSize",x), 0f, 0.01f, _outlineFadeDuration);
        yield return _outlineMat.DOColor(newColor, "_Color", _outlineFadeDuration).WaitForCompletion();
        _isOutlined = true;
        _currentFade = null;
        yield return null;
    }
    private IEnumerator RemoveOutline()
    {
        DOTween.To(x => _outlineMat.SetFloat("_BaseContourSize", x), 0.01f, 0f, _outlineFadeDuration);
        yield return _outlineMat.DOColor(Color.white, "_Color", _outlineFadeDuration).WaitForCompletion();
        _isOutlined = false;
        _currentFade = null;
        yield return null;
    }
}
