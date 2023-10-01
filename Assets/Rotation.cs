using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Rotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(50 * Time.deltaTime, 50 * Time.deltaTime, 50 * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime);
        //transform.localRotation = Quaternion.Euler(transform.localRotation.x + Time.deltaTime, transform.localRotation.y + Time.deltaTime, transform.localRotation.z + Time.deltaTime);
    }
}
