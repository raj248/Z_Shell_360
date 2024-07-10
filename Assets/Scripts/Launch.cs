using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public float gravity = -18f;
    public float height = 25f;

    public Rigidbody ball;
    public Transform target;
    void Start()
    {
        ball.useGravity = false;
        Physics.gravity = Vector3.up * gravity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Throw();
    }
    Vector3 CalculateProjectileVelocity()
    {
        Vector3 xzDisplacement = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        float yDisplacement = target.position.y - transform.position.y;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);

        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (yDisplacement - height) / gravity);

        Vector3 velocityXZ = xzDisplacement / time;

        Debug.Log(velocityXZ + velocityY);
        return velocityXZ + velocityY;
    }
    public void Throw()
    {
        ball.useGravity = true;
        ball.velocity = CalculateProjectileVelocity();
    }

}
