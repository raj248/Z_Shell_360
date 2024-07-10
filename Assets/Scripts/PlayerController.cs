using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script lies of a refrence object which will find a GameObject named Ball to Take Control of It's Rigidbody


public class PlayerController : MonoBehaviour
{
    private bool isOnGround;

//    public Joystick joystick;
  //  public Joystick rJoystick;
    Rigidbody ballRb;

    public enum States { RollMode, FlightMode, Rage }
    [HideInInspector] public States states;

    [SerializeField] private float rollSpeed = 10;

    private void Awake()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
    }
    private void Start()
    {
        states = States.RollMode;
        isOnGround = true;
        //rJoystick.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        transform.position = ballRb.transform.position;
        //Roll();
    }
    //private void Roll()
    //{
    //    if (isOnGround)
    //    {
    //        rJoystick.gameObject.SetActive(true);
    //        states = States.RollMode;
    //        Vector3 xAxis = joystick.Horizontal * transform.right;
    //        Vector3 zAxis = joystick.Vertical * transform.forward;
    //        Vector3 dir = (xAxis + zAxis).normalized;

    //        float y = rJoystick.Horizontal;
    //        transform.localEulerAngles += (new Vector3(0, y * rollSpeed / 5, 0));

    //        ballRb.AddForce(dir * rollSpeed * Time.unscaledDeltaTime, ForceMode.VelocityChange);
    //    }
    //}


}
