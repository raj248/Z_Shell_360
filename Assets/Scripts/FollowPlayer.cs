using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It'll Lerp to a empty GameObject (Offset)  
 * That offset is the child of the player so that it can rotate with player
 * or playerMarker (if it is a ball ) 
 * Dont't forget to mark your player with a player tag
 */
public class FollowPlayer : MonoBehaviour
{
    #region Variables
    [Header("Camera Settings")]
    public Vector3 offsetLenght;
    public float smoothFactor = .5f;
    public LayerMask player;

    [Header("Miscelleneous")]
    [HideInInspector]
    public bool sniping;
    Vector3 velocity;

    [Header("References")]
    GameObject offset;
    Transform playerTransform;
    Transform follower;
    Ability ability;
    Roll rollComponent;
    #endregion

    #region Camera Movements
    void SmoothShift()
    {
        if (rollComponent.states!=Roll.States.Sniper) SetLookAtPlayer(); else SetLookAtForward();
        if (rollComponent.states != Roll.States.Dash)
        {
            Vector3 desiredPosition = offset.transform.position;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothFactor/10);
            transform.position = smoothPosition;
            if (rollComponent.states != Roll.States.Sniper)
            {
                transform.rotation = Quaternion.LookRotation(playerTransform.position - transform.position);
            }
        }
        else
        {
            Damp();
        }
    }
    private void Damp()
    {
        Vector3 desiredPosition = offset.transform.position;
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothFactor);
        transform.position = smoothPosition;
        transform.rotation = Quaternion.LookRotation(playerTransform.position - transform.position);
    }
    #endregion

    #region SetLookAt
    public void SetLookAtForward()
    {
        transform.SetParent(follower);
        FixLookRotation(rollComponent.MouseLookAround()); //Replace with LookAround for Joystick
    }
    public void SetLookAtPlayer()
    {
        follower.DetachChildren();
        offset.transform.SetParent(follower);
    }
    public void SetLookAtEnemy(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }
    #endregion

    #region Fixing( Angle, Rotations )
    void FixAngle()
    {
        if((rollComponent.states != Roll.States.Sniper)) { 
        RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (!hit.collider.gameObject.CompareTag("Player"))
                {
                    follower.transform.localEulerAngles += (new Vector3(0, 7 / 5, 0));
                }
            }
        }
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
    #endregion
    private void Awake()
    {
        // GameObject References
        offset = new GameObject("Offset");
        follower = GameObject.Find("Follower").transform;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Component References
        rollComponent = follower.GetComponent<Roll>();
        ability = GameObject.Find("Ball").GetComponent<Ability>();
        offset.transform.SetParent(follower);
        offset.transform.position = playerTransform.position + offsetLenght;
    }
    private void FixedUpdate()
    {
        SmoothShift();
        FixAngle();
    }

}
