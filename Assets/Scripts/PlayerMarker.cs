using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMarker : MonoBehaviour
{

    public GameObject jumpButton;
    //public Joystick joystick;
    ////public Joystick rJoystick;
    private TimeManager timeManager;
    private LineRenderer lineVisual;

    [SerializeField] public Rigidbody ballRb;
    [SerializeField] private GameObject ball;
    [HideInInspector] public States states;
    public enum States { RollMode, FlightMode ,Rage}
    private bool isOnGround;
    [HideInInspector] public bool isLaunchAllowed;
    public LayerMask floor;

    [Header("Player States")]
    [SerializeField] private float orbitSpeed = 4;
    [SerializeField] private float rollSpeed = 10;
    [SerializeField] private int lineSegments = 10;
    [SerializeField] private float maxVelocity = 1200;

    private float lineLength = 15;
    private float floorCheckRadius = 1.5f;
    private float x; private float y;
    private void Awake()
    {
        ball = GameObject.Find("Ball");
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        lineVisual = GetComponent<LineRenderer>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
    }
    void Start()
    {
        states = States.RollMode;
        isOnGround = false;
        ////rJoystick.gameObject.SetActive(false);
    }
    void Update()
    {
        transform.position = ball.transform.position;
    }
    private void FixedUpdate()
    {
        //Rotate();
        //Roll();
        CheckFloor();
    }
    //public void Rotate()
    //{
    //    if (!isOnGround)
    //    {

    //        states = States.FlightMode;
    //        float xAxis = joystick.Vertical;
    //        float yAxis = joystick.Horizontal;
    //        Vector3 dir = new Vector3(xAxis, yAxis).normalized;

    //        x += xAxis;
    //        y -= yAxis;
    //        Vector3 rot = new Vector3(x, y);
    //        transform.localRotation = Quaternion.Euler(rot * orbitSpeed);
    //        SetLaunchPermission(dir);
    //    }
    //}
    //private void Roll()
    //{
    //    if (isOnGround)
    //    {
    //        //rJoystick.gameObject.SetActive(true);
    //        states = States.RollMode;
    //        Vector3 xAxis = joystick.Horizontal * transform.right;
    //        Vector3 zAxis = joystick.Vertical * transform.forward;
    //        Vector3 dir = (xAxis+  zAxis).normalized;

    //        //float y = rJoystick.Horizontal;
    //        transform.localEulerAngles += (new Vector3(0, y * rollSpeed /5, 0));

    //        ballRb.AddForce(dir * rollSpeed * Time.unscaledDeltaTime, ForceMode.VelocityChange);
    //    }
    //}
    public void Jump()
    {
        ballRb.AddForce(transform.up * 200 * Time.deltaTime, ForceMode.VelocityChange);
    }
    private void CheckFloor()  // Mark an Object Layermask and Tag both as floor to be able to detect it.
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, floorCheckRadius,floor);

        if (colliders.Length != 0)
        {
            rJoystick.gameObject.SetActive(true);
            isOnGround = true;
            isLaunchAllowed = false;
            jumpButton.SetActive(true);
            RollMode();
            if(Time.timeScale < .5) timeManager.FastUp();
        }
        else { isOnGround = false; jumpButton.SetActive(false); rJoystick.gameObject.SetActive(false); }
    }
    private void SetLaunchPermission(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            FlightMode();
            ballRb.velocity =ballRb.velocity.normalized* 120f ;
            isLaunchAllowed = true;
            timeManager.SlowDown();
            ballRb.mass = 5;
        }

        if (dir == Vector3.zero && isLaunchAllowed)
        {
            Launch();
            isLaunchAllowed = false;
            timeManager.FastUp();
            ballRb.mass = 1;
        }

    }
    public void Launch()
    {
        ballRb.velocity = Vector3.zero;
        ballRb.AddForce(transform.forward * Time.unscaledDeltaTime * maxVelocity, ForceMode.VelocityChange);
        lineVisual.enabled = false;
    }
    public void FlightMode()
    {
        ballRb.useGravity = false;
        ballRb.drag = 0f;
    }
    public void RollMode()
    {
        ballRb.useGravity = true;
        ballRb.drag = 0;
    }

    // Path Prediction 
    public void Visualize()
    {
        lineVisual.enabled = true;
        for (int i = 0; i < lineSegments; i++)
        {
            Vector3 pos = CalculatePositionInTime( i / (float)lineSegments);
            lineVisual.SetPosition(i, pos);
        }
    }  // Setting Line Renderer Positions
    public Vector3 CalculatePositionInTime( float time)
    {
        float vXz = ballRb.velocity.magnitude * lineLength;
        Vector3 result = transform.position + transform.forward *vXz * time;
        return result;
    }  // Calculating Position for Line Renderer

}
