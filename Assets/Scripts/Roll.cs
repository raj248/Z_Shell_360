using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script lies on a refrence object which will find a GameObject named Ball to Take Control of It's Rigidbody
// -------------- Player Movement Behaviour ----------------- //
public class Roll : MonoBehaviour
{
    #region Variables
    //[Header("Joystick")]
    //public Joystick moveJoystick;
    //public Joystick rotateJoystick;
    public enum States { Dash , Sniper }

    [Header("Essentials")]
    [HideInInspector]
    public float rollSpeed = 7;
    [HideInInspector]
    public Vector3 offsetDistance = Vector3.zero;
    [HideInInspector] public States states;

    [Header("References")]
    public GameObject trail;
    Rigidbody ballRb;
    Ability ability;
    #endregion

    #region Movements
/*    private void RollAround()
    {
        // Joystick Controller
        // MoveJoystick for movement & RotateJoystick for Rotation
        // Can Rotate only about Y-Axis When on ground
        // Limits player speed if it's not in Dash 

        
        
            Vector3 xAxis = moveJoystick.Horizontal * transform.right;
            Vector3 zAxis = moveJoystick.Vertical * transform.forward;
            Vector3 dir = (xAxis + zAxis).normalized;

            float y = rotateJoystick.Horizontal;
            transform.localEulerAngles += (new Vector3(0, y * rollSpeed / 5, 0));

            ballRb.AddForce(dir * rollSpeed * 5 * Time.unscaledDeltaTime, ForceMode.VelocityChange);
            if (states != States.Dash)
                ballRb.velocity = ballRb.velocity.normalized * Mathf.Clamp(ballRb.velocity.magnitude, 0, 15);
        
    }
*/
    private void KeyRollAround()
    {
        // KeyBoard Controller
        // (WASD || Arrow Keys) for movement && (Q and W || PageUp and PageDown) for Rotation
        // Can Rotate only about Y-Axis When on ground
        // Limits player speed if it's not in Dash 

            // Taking Inputs
            Vector3 xAxis = Input.GetAxis("Horizontal") * transform.right;
            Vector3 zAxis = Input.GetAxis("Vertical") * transform.forward;
            Vector3 dir = (xAxis + zAxis).normalized;

            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageUp))
                transform.localEulerAngles -= (new Vector3(0, rollSpeed / 5, 0));
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageDown))

                // Using Inputs
                transform.localEulerAngles += (new Vector3(0, rollSpeed / 5, 0));
            ballRb.AddForce(dir * rollSpeed * 5 * Time.unscaledDeltaTime, ForceMode.VelocityChange);

            // Limiting Speed if Not in Dash
            if (states != States.Dash)
                ballRb.velocity = ballRb.velocity.normalized * Mathf.Clamp(ballRb.velocity.magnitude, 0, 15);

    }
    #endregion

    #region Functional Support
  /*
    public Vector3 LookAround() // Joystick Version 
    {
        float x = rotateJoystick.Vertical;
        float y = rotateJoystick.Horizontal;
        return new Vector3(x * -rollSpeed * 5, y * rollSpeed * 5, 0) * Time.unscaledDeltaTime;
    }
*/
    public Vector3 MouseLookAround()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        Vector3 input = new Vector3(x, y, 0);
        return new Vector3(y * -rollSpeed * 10, x * rollSpeed * 10, 0) * Time.unscaledDeltaTime;
    }
    public void SetOffset(Vector3 offset)
    {
        offsetDistance = offset;
    }
    #endregion

    #region EventFunctions
    private void Awake()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        ability = GameObject.Find("Ball").GetComponent<Ability>();
        trail = GameObject.Find("Trail");
        transform.position = ballRb.transform.position;
    }
    private void Start()
    {
        states = States.Dash;
    }
    private void FixedUpdate()
    {
        transform.position = ballRb.transform.position + offsetDistance;
        trail.transform.position = transform.position + new Vector3(0,-.5f);
        KeyRollAround();  // Keyboard Controllers
        //RollAround();     //Joystick Controllers
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            ability.Snipe();
            ability.AimAt();
        }
        if (Input.GetKeyDown(KeyCode.R) && states == States.Sniper) ability.AimAt();
        //Cursor.visible = false;
        if (Input.GetKeyDown(KeyCode.G)) ability.SlingShot();
        if (Input.GetKey(KeyCode.Tab)) ability.Visualize();
        if (Input.GetKeyDown(KeyCode.Space) && states != States.Sniper ) ability.Attack();
        if (Input.GetKeyDown(KeyCode.Space) && states == States.Sniper) ability.SnipeFire();
    }
    #endregion
}
