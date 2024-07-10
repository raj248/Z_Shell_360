using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float  sensitivity = 8;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        FixLookRotation(MouseLookAround());
    }
    public Vector3 MouseLookAround()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        Vector3 input = new Vector3(x, y, 0);
        return new Vector3(y * -sensitivity * 10, x * sensitivity * 10, 0) * Time.unscaledDeltaTime;
    }
    void FixLookRotation(Vector3 input)
    {
        // x --> -60 to 70
        if ((transform.localEulerAngles.x > 70) && input.x > 0)
        {
            transform.localEulerAngles += new Vector3(0, input.y, 0);
        }
        else if ((transform.localEulerAngles.x > 70) && input.x < 0)
        {
            transform.localEulerAngles += new Vector3(input.x, input.y, 0);
        }
        if ((transform.localEulerAngles.x <= -60) && input.x < 0)
        {
            transform.localEulerAngles += new Vector3(0, input.y, 0);
        }
        else if ((transform.localEulerAngles.x <= -60) && input.x > 0)
        {
            transform.localEulerAngles += new Vector3(input.x, input.y, 0);
        }
        else
        {
            transform.localEulerAngles += input;
        }
    }


}
