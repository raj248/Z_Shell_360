using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]

// This Script Lies on the Enemy Sphere
public class EnemySphere : MonoBehaviour
{
    #region Variables
    [Header("Attributes")]
    public float speed = 7;
    public float range = 7;
    public float damage = 10;
    public float damageByPlayer = 15;
    public float dashSpeed = 50;
    
    public GameObject crackedPrefab;

    [Header("Health")]
    public Image healthBar;
    public float currentHealth;
    public float maxHealth = 45;

    [Header("References")]
    Rigidbody rb;
    NavMeshAgent agent;
    GameObject player;
    #endregion

    #region Enemy Behaviour
    void ChasePlayer()
    {
        /*
           Check for the player to be in Attack Range 
           if Not in Attack Range then will move try to move in Range
           and Attack otherwise.
           It is Kinematic while Moving in Range And Non-Kinematic while Attacking
        */
        Vector3 direction = player.transform.position - transform.position;
        float dis = direction.magnitude;
        Vector3 dir = direction.normalized;

        if(dis < range && rb.isKinematic)
        {
            agent.SetDestination(transform.position);
            StartCoroutine( Dash(dir));
        }
        else if( dis > range)
        {
            agent.SetDestination(player.transform.position);
        }
    }
    IEnumerator Dash(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * dashSpeed * 10 * Time.deltaTime, ForceMode.VelocityChange);
        yield return new WaitForSeconds(3);
        rb.isKinematic = true;
    }
    #endregion

    #region Functional Support(Die, CollisionDetection)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Ability>().attacking)
            {
                currentHealth -= damage;
            }
        }
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            currentHealth -= damageByPlayer;
        }
    }
    void Die()
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
    #endregion

    #region EventFunctions
    void Start()
    {
        //GameOnject References
        player = GameObject.FindGameObjectWithTag("Player");

        //Components References
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // Initial Calculations
        rb.isKinematic = true;
        currentHealth = maxHealth;
    }
    void FixedUpdate()
    {
        ChasePlayer();
    }
    void Update()
    {
        Die();
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    #endregion
}
