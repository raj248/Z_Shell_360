using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public RectTransform healthBar;

    [SerializeField] 
    float maxHealth = 100;
    [SerializeField]
    float currentHealth;

    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        float y = (currentHealth / maxHealth);
        float health = Mathf.Clamp(y, 0f, 1f);
        healthBar.localScale = new Vector3(healthBar.localScale.x, health, healthBar.localScale.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        ReflectBounce();
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Damage(10);
        }
    }
    public void Damage(float _damage)
    {
        currentHealth -= _damage;
    }
    public void ReflectBounce()
    {
        Vector3 vel = rb.velocity;
        vel += vel / 10;
        rb.velocity = vel;
    }
}
