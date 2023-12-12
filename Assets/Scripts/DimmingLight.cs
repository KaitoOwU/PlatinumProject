using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimmingLight : MonoBehaviour
{
    private float _dim;
    private Light _light;
    private float _timer;
    [SerializeField] AnimationCurve curve;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnEachEndPhase.AddListener(ResetTimer);
        _light = GetComponent<Light>();
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.FIRST_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0.3f, 0.25f, curve.Evaluate( _timer / GameManager.Instance.GameData.TimerValues.FirstPhaseTime));
            _light.intensity = dim;
        }
        else if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.SECOND_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0.25f, 0.175f, curve.Evaluate(_timer / GameManager.Instance.GameData.TimerValues.SecondPhaseTime));
            _light.intensity = dim;
        }
        else if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.THIRD_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0.175f, 0f, _timer / GameManager.Instance.GameData.TimerValues.ThirdPhaseTime);
            _light.intensity = dim;
        }
    }
    private void ResetTimer()
    {
        _timer = 0;
    }
}
