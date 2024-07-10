using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    private GameObject player;
    private GameObject camOffset;
    private GameObject rageCam;
    private PlayerMarker marker;

    public Vector3 velocity = Vector3.one;
    public Vector3 offset;
    public float smoothFactor = 1;


    private void Awake()
    {
        player = GameObject.Find("PlayerMarker");
        camOffset = GameObject.Find("Offset");
        marker = GameObject.Find("PlayerMarker").GetComponent<PlayerMarker>();
        rageCam = GameObject.Find("RageCamera");
    }
    void Start()
    {
        offset = transform.position;
    }
    private void FixedUpdate()
    {
        offset = camOffset.transform.position - player.transform.position;
        
            SmoothShift();
        if (marker.isLaunchAllowed )
        {
            transform.SetParent(camOffset.transform);
            marker.Visualize();
        }
    }
    public void SmoothShift()
    {
        camOffset.transform.DetachChildren();
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothFactor ,1000,Time.unscaledDeltaTime);
        transform.position = smoothedPosition;

        if (marker.ballRb.velocity.magnitude < 150)
        {
            transform.LookAt(player.transform);
        }
        else { SetLook(); }
    }
    public void SetLook()
    {
        transform.position = Vector3.Slerp(transform.position, rageCam.transform.position, .1f );
        transform.localRotation = rageCam.transform.localRotation;
    }
}
