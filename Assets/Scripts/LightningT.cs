using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This Script Lies on the Lightning Tower
public class LightningT : MonoBehaviour
{
    #region Variables
    [Header("Attributes")]
    public float bulletSpeed = 25;
    public float range = 30;
    public float fireRate = 1;
    private float nextFire;
    private bool sawPlayer;

    [Header("Health")]
    public float maxHealth = 40;
    public float currentHealth;
    private float damageByPlayer = 20;

    [Header("References")]
    public GameObject player;
    public GameObject bullet;
    public Image healthBar;
    public GameObject crackedPrefab;
    Transform eBall;

    #endregion

    #region EnemyBehaviour
    private void Attack()
    {
        // Detect Player with it's Tag and Attack if Player is in Range
        Vector3 direction = player.transform.position - eBall.position;
        float dis = direction.magnitude;
        Vector3 dir;

        if (dis < range && Time.time > nextFire)
        {
            dir = (player.transform.position - eBall.position).normalized;
            GameObject blt = Instantiate(bullet, eBall.position + dir , Quaternion.identity) as GameObject;
            blt.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.VelocityChange);
            Destroy(blt,2);
            nextFire = Time.time + 1 / fireRate;
        }
    }
    private void Die()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            GameObject crack = Instantiate(crackedPrefab, transform.position, crackedPrefab.transform.rotation);
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
            currentHealth -= damageByPlayer;
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
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        eBall = transform.Find("EBall");
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,player.transform.position - transform.position, out hit, range))
        {
            Debug.DrawRay(transform.position,player.transform.position - transform.position,Color.blue);
            if (hit.collider.gameObject.CompareTag("Player")) sawPlayer = true;
        }
        else sawPlayer = false;

        if (sawPlayer) Attack();
        Die();
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    #endregion 

}
