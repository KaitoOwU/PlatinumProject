using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Light _light;
    [SerializeField]AnimationCurve _curve;
    float _timer;
    bool _flick;
    float _intensity;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnEachMinute.AddListener(Flickers);
        _light = GetComponentInChildren<Light>();
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_flick)
        {
            _light.intensity = Mathf.Lerp(_intensity, 0.13f, _curve.Evaluate(_timer));

        }
        if (_timer > 1)
        {
            _flick = false;
        }
    }
    // Update is called once per frame
    void Flickers()
    {
        _intensity = _light.intensity;
        _timer = 0;
        _flick = true;
    }

}
