using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Light _light;
    [SerializeField]AnimationCurve _curve;
    float _timer;
    bool _flick;
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
            _light.intensity = Mathf.Lerp(0.43f, 0.13f, _curve.Evaluate(_timer));

        }
        if (_timer > 1)
        {
            _flick = false;
        }
    }
    // Update is called once per frame
    void Flickers()
    {
        _timer = 0;
        _flick = true;
    }

}
