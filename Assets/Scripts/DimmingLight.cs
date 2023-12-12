using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DimmingLight : MonoBehaviour
{
    private float _dim;
    private Volume[] _volumes;
    private LiftGammaGain _gamma;
    private float _timer;
    [SerializeField] AnimationCurve _curve;
    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnEachEndPhase.AddListener(ResetTimer);
        _volumes = FindObjectsOfType<Volume>();
        Debug.Log(_volumes.Length);
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.FIRST_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0f, -0.4f, _curve.Evaluate(_timer / GameManager.Instance.GameData.TimerValues.FirstPhaseTime));
            foreach (Volume v in FindObjectsOfType<Volume>())
            {
                v.profile.TryGet<LiftGammaGain>(out _gamma);
                _gamma.gamma.value = new Vector4(1f, 1f, 1f, dim);
            }
        }
        else if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.SECOND_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0f, -0.4f, _curve.Evaluate(_timer / GameManager.Instance.GameData.TimerValues.SecondPhaseTime));
            foreach (Volume v in FindObjectsOfType<Volume>())
            {
                v.profile.TryGet<LiftGammaGain>(out _gamma);
                _gamma.gamma.value = new Vector4(1f, 1f, 1f, dim);
            }
        }
        else if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.THIRD_PHASE && GameManager.Instance.IsTimerGoing)
        {
            _timer += Time.deltaTime;
            float dim = Mathf.Lerp(0f, -0.4f, _curve.Evaluate(_timer / GameManager.Instance.GameData.TimerValues.ThirdPhaseTime));
            foreach (Volume v in FindObjectsOfType<Volume>())
            {
                v.profile.TryGet<LiftGammaGain>(out _gamma);
                _gamma.gamma.value = new Vector4(1f, 1f, 1f, dim);
            }
        }
    }
    private void ResetTimer()
    {
        _timer = 0;
    }
}
