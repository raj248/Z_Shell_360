using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ability : MonoBehaviour
{
    #region Variables

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;

    [Header("Camera Settings")]
    public float dampTime = .01f;
    [HideInInspector]
    public bool damp;
    [HideInInspector]
    public bool attacking = false;

    [Header("Projectile Settings")]
    [SerializeField]
    float height;
    float gravity = -18f;
    float time;

    [Header("Attributes")]
    public float dashSpeed = 120;
    public float boostTime = 5f;
    public float drag = 2f;
    public float angularDrag = 1;
    public float dashRange = 7;
    public float bulletSpeed = 70;
    public float snipingHeight = 12;
    public float rollSpeed = 8;
    public LayerMask enemyLayer;
    public LayerMask wallLayerMask;
    public LayerMask floorLayerMask;

    [Header("References")]
    public GameObject bullet;
    public ParticleSystem flares;
    LineRenderer lineRenderer;
    GameObject crossBow;
    Rigidbody rb;
    GameObject follower;
    GameObject offset;
    FollowPlayer cam;
    Roll rollComponent;
    Image healthBar;
    TimeManager timeManager;
    TextMeshProUGUI timeText;


    [Header("Miscelleneous Variables")]
    Vector3 offsetLenght;
    Vector3 launchVelocity;
    float realTime;
    bool visual;
    Queue<Collider> queue = new Queue<Collider>();
    #endregion

    #region   Jump , Dash , Snipe , Attack
    public void Jump()
    {
        rb.velocity += Vector3.up * 5;
    }
    public void Boost()
    {
        //rb.velocity = Vector3.zero;
        rb.AddForce(rb.velocity.normalized * dashSpeed * 10 * Time.fixedUnscaledDeltaTime, ForceMode.Impulse);

    }
    public void Dash(Vector3 _dir)
    {

        Vector3 dir = _dir;
        if (dir == Vector3.zero)
        {
            rb.drag = 0;
            rb.angularDrag = 0;
            StartCoroutine(ResetDrag());
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * dashSpeed * 7 * Time.fixedUnscaledDeltaTime, ForceMode.VelocityChange);
        }
    }
    public void Snipe()
    {
        if ((rollComponent.states != Roll.States.Sniper))
        {
            StartSnipe();
        }
        else
        {
            ResetSnipe();
        }
    }

    private void StartSnipe()
    {
        rollComponent.trail.SetActive(false);
        crossBow.SetActive(true);
        rollComponent.states = Roll.States.Sniper;
        rollComponent.SetOffset(Vector3.up * snipingHeight);
        offset.transform.localPosition = Vector3.zero - follower.transform.forward;
        timeManager.SlowDown();
        cam.SetLookAtForward();
    }
    private void ResetSnipe()
    {
        rollComponent.trail.SetActive(true);
        crossBow.SetActive(false);
        rollComponent.states = Roll.States.Dash;
        rollComponent.SetOffset(Vector3.zero);
        offset.transform.localPosition = offsetLenght;
        timeManager.FastUp();
        cam.SetLookAtPlayer();
        offset.transform.SetParent(follower.transform);
    }
    public void Attack()
    {
        Dash(NearestEnemy());
    }
    public void SnipeFire()
    {
        GameObject blt = Instantiate(bullet, cam.transform.position + Camera.main.transform.forward, Quaternion.identity) as GameObject;
        blt.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * bulletSpeed * 500 * Time.fixedUnscaledDeltaTime, ForceMode.VelocityChange);
        Destroy(blt, 2);
    }
    public void AimAt()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50, enemyLayer);

        //Calculate Rotation And pass in into FixRotation
        Quaternion rotation;
        if (queue.Count == 0) queue = AllEnemy();
        else
        {
            Collider target = queue.Dequeue();
            rotation = Quaternion.LookRotation(target.transform.position - cam.transform.position);
            cam.SetLookAtEnemy(rotation);
        }
    }
    public void WallClimb()
    {
        Collider[] colliders = (Physics.OverlapSphere(transform.position, 1.2f, wallLayerMask));
        if (colliders.Length != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (colliders[0].transform.position - transform.position), out hit))
            {
                Vector3 dir = (hit.normal + Vector3.up).normalized;
                // Adding Upward Force Say WallJmpp/Climbing 
                Dash(dir);
            }
        }

    }
    public void SlingShot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 300))
        {
            Vector3 xz = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);
            float y = hit.point.y - transform.position.y;
            if (!(y < 0))
            {
                height = y + .25f * y;
                launchVelocity = CalculateProjectileVelocity(hit.point);
                ResetSnipe();
                rb.drag = 0;
                rb.angularDrag = 0;
                rb.velocity = launchVelocity;
                lineRenderer.enabled = false;
            }
            else
            {
                ResetSnipe();
                rb.drag = 0;
                rb.angularDrag = 0;
                rb.velocity = (hit.point - transform.position).normalized * 50;
                lineRenderer.enabled = false;
            }
        }

    }
    Vector3 PointInSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 300))
        {
            float y = hit.point.y - transform.position.y;
            height = y + .2f * y;
            if (y > 0)
                return hit.point;
            else return Vector3.zero;
        }
        else
        {
            return Vector3.zero;

        }
    }
    public void Visualize()
    {
        lineRenderer.enabled = true;
        Vector3 vel = CalculateProjectileVelocity(PointInSpace());
        Vector3 prePosition = transform.position;
        for (int i = 0; i < 30; i++)
        {
            float time = i / (float)30 * this.time;
            Vector3 s = (vel * time) + Vector3.up * gravity * time * time / 2;
            lineRenderer.SetPosition(i, s + transform.position);
            Debug.DrawLine(prePosition, s + transform.position, Color.red, 5);
            prePosition = transform.position + s;
        }
    }

    #endregion

    #region Returnables( NearestEnemy, AllEnemy )
    public Vector3 NearestEnemy()
    {
        // If Found Enemy in Range then  
        // will return direction toward them 
        // Camera won't use Vector3.SmoothDamp for transformation 
        // Camera will use Vector3.Lerp
        // And player will be in Attack Mode
        Collider[] enemies = Physics.OverlapSphere(transform.position, dashRange, enemyLayer);
        if (enemies.Length != 0)
        {
            damp = false; attacking = true; rollComponent.states = Roll.States.Dash; StartCoroutine(ResetAttack());
            return (enemies[0].transform.position - transform.position).normalized;
        }

        // If not found then
        // will return forward
        // Camera will use Vector3.SmoothDamp for transformation 
        // Player will be in Dash Mode

        else
        {
            StartCoroutine(Timer());
            return Vector3.zero;
        }
    }
    public Queue<Collider> AllEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 50, enemyLayer);
        if (enemies.Length != 0)
        {
            foreach (Collider enemy in enemies)
            {
                queue.Enqueue(enemy);
            }
        }
        return queue;
    }
    Vector3 CalculateProjectileVelocity(Vector3 target)
    {
        float yDisplacement = target.y - transform.position.y;
        Vector3 xzDisplacement = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);

        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (yDisplacement - height) / gravity);
        this.time = time;

        Vector3 velocityXZ = xzDisplacement / time;
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);

        return velocityXZ + velocityY;
    }


    #endregion

    #region Timers 
    IEnumerator ResetAttack()
    {
        // Reset Attacking bool only in Damp Time 
        yield return new WaitForSeconds(dampTime);
        attacking = false;
    }
    IEnumerator ResetDrag()
    {
        yield return new WaitForSeconds(boostTime);
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
    void ResetDragNow()
    {
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
    IEnumerator Timer()
    {
        //Set Damp to true for DampTime 
        damp = true;
        rollComponent.states = Roll.States.Dash;
        yield return new WaitForSeconds(dampTime);
        damp = false;
    }
    #endregion

    #region Stats Modifier
    public void Damage(float _damage)
    {
        // currentHealth -= _damage;
        currentHealth = Mathf.SmoothStep(currentHealth, currentHealth - _damage, .5f);
        healthBar.fillAmount = (currentHealth / maxHealth);
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bullet")) { Damage(10); flares.Play(); }
        if (collision.gameObject.CompareTag("Wall")) WallClimb();
        if (collision.gameObject.CompareTag("Floor"))
        {
            ResetDragNow();
            rollComponent.states = Roll.States.Dash;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            Destroy(other.gameObject);
        }
    }
    #endregion

    private void Start()
    {
        // GameObject References
        follower = GameObject.Find("Follower");
        timeText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        offset = GameObject.Find("Offset");
        crossBow = GameObject.Find("CrossBow");

        // Component References
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        healthBar = GameObject.Find("Health").GetComponent<Image>();
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        cam = Camera.main.GetComponent<FollowPlayer>();
        rollComponent = GameObject.Find("Follower").GetComponent<Roll>();

        // Initial Calculations
        offsetLenght = offset.transform.position - transform.position;
        currentHealth = maxHealth;
        crossBow.SetActive(false);
        Physics.gravity = Vector3.up * gravity;
        rollComponent.rollSpeed = rollSpeed;
    }
    private void Update()
    {
    }
}
