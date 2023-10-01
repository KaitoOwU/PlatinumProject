using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMov : MonoBehaviour
{
    // Start is called before the first frame update
    public float startMove;
    public float endMove;
    public float sinValue;
    public float newValue;
    void Start()
    {
        startMove = transform.localPosition.y;
        endMove = 0.015f;

        //sinValue = Mathf.Sin(Time.time * 0.1f);
        //newValue = Mathf.Lerp(startMove, endMove, sinValue * sinValue);
        //transform.position = new Vector3(transform.position.x, newValue, transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        sinValue = Mathf.Sin(Time.time);
        newValue = Mathf.Lerp(startMove, endMove, sinValue * sinValue);
        transform.localPosition = new Vector3(transform.localPosition.x, newValue, transform.localPosition.z);
    }
}
