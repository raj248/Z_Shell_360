using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float slowdownFactor = .05f;
    public void SlowDown()
    {

        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void FastUp()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
