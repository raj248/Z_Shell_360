using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

// This Script Lies on the EnemyCube
public class Enemy : MonoBehaviour
{
    #region Variables
    [Header("Attributes")]
    public float bulletSpeed = 25;
    public float range = 30;
    private float nextFire;
    public float fireRate = 1;

    [Header("Health")]
    public float maxHealth = 60;
    public float currentHealth;
    private float damageByPlayer = 20;

    [Header("References")]
    NavMeshAgent agent;
    Rigidbody rb;
    GameObject playert;
    public GameObject bullet;
    public ParticleSystem blastEffect;
    public Image healthBar;
    public GameObject crackedPrefab;
    #endregion

    #region EnemyBehaviour
    private void Attack()
    {
        // Attack Player if in Range 
        // Move in Range if Out of Range And is Reachable
        float dis = (transform.position - playert.transform.position).magnitude;
        if (dis < range && Time.time > nextFire)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(playert.transform);
            Vector3 dir = (transform.position - playert.transform.position).normalized;
            GameObject blt = Instantiate(bullet, transform.position - dir, Quaternion.identity) as GameObject;
            blt.GetComponent<Rigidbody>().AddForce(-dir * bulletSpeed, ForceMode.VelocityChange);
            Destroy(blt, 2);
            nextFire = Time.time + 1 / fireRate;
        }
        else if (dis > range)
        { 
            agent.SetDestination(playert.transform.position);
        }

    }
    private void Die()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            GameObject crack = Instantiate(crackedPrefab, transform.position, transform.rotation);
            for (int i = 0; i < crack.transform.childCount; i++)
            {
                Rigidbody rb = crack.transform.GetChild(i).GetComponent<Rigidbody>();
                rb.AddExplosionForce(50, transform.position, 5);
            }
            Destroy(crack, 3);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Ability>().attacking)
            {
                currentHealth -= damageByPlayer;
            }
        }
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            currentHealth -= damageByPlayer;
        }

    }
    #endregion

    #region EventFunctions
    void Start()
    {
        playert = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        rb.isKinematic = true;
        currentHealth = maxHealth;

    }
    void Update()
    {
        Attack();
        Die();
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    #endregion

}
