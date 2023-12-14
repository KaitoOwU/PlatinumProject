using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Light _light;
    [SerializeField]AnimationCurve _curve;
    [SerializeField]AnimationCurve _curveC;
    float _timer;
    bool _flick;
    bool _comp;
    float _intensity;
    float _range;
    [SerializeField] float  _completeIntiensity;
    // Start is called before the first frame update
    private void Awake()
    {
        _light = GetComponentInChildren<Light>();

    }
    void Start()
    {
        GameManager.Instance.OnEachMinute.AddListener(Flickers);
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_flick)
        {
            _light.intensity = Mathf.Lerp(_intensity, 0.04f, _curve.Evaluate(_timer));
        }
        if (_comp)
        {
            _light.range = Mathf.Lerp(_range, _range + 20,_curveC.Evaluate(_timer));
            _light.intensity = Mathf.Lerp(_intensity, _completeIntiensity*10, _curveC.Evaluate(_timer/2));
            _light.color = new Color(245 / 255f, 212f / 255f, 102f / 255f);
        }
        if (_timer > 1)
        {
            _flick = false;
        }
        if (_timer > 2)
        {
            _comp = false;
        }
    }
    // Update is called once per frame
    void Flickers()
    {
        _intensity = _light.intensity;
        _timer = 0;
        _flick = true;
    }
    public void Complete()
    {
        _range = _light.range;
        _intensity = _light.intensity;
        _timer = 0;
        _comp = true;
    }
}
