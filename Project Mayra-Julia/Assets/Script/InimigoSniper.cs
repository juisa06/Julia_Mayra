using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class InimigoSniper : MonoBehaviour
{
    public GameObject Ak;
    public GameObject blood;
    public bool isDead;
    public float moveSpeed = 3f;
    public float visionRange = 5f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int life = 10;
    private Animator anim;
    public bool isfire;
    private bool weaponDropped = false;

    private Transform player;
    private bool playerInSight = false;
    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 patrolDirection;
    private AudioSource Snipersound;

    private EnemyManager enemyManager;

    void Start()
    {
        Snipersound = GetComponent<AudioSource>();
        enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.AddEnemy(gameObject);
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        SetRandomPatrolDirection();
    }

    void Update()
    {
        if (isDead == false)
        {
            dead();
            if (Vector2.Distance(transform.position, player.position) <= visionRange)
            {
                playerInSight = true;
                Vector2 direction = (player.position - transform.position).normalized;
                transform.up = direction;
                if (Time.time >= nextFireTime)
                {
                    StartCoroutine("Tiro");
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                playerInSight = false;
            }
            Patrol();
        }
    }

    void Patrol()
    {
        if (!playerInSight)
        {
            RaycastHit2D selfHit = Physics2D.Raycast(transform.position, patrolDirection, 1f);
            if (selfHit.collider != null && selfHit.collider.CompareTag("Inimigo"))
            {
                SetRandomPatrolDirection();
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 1f);
            if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Inimigo")))
            {
                SetRandomPatrolDirection();
            }
            else
            {
                rb.MovePosition(rb.position + patrolDirection * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void SetRandomPatrolDirection()
    {
        patrolDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    IEnumerator Tiro()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, transform.up, visionRange);
        if (hit.collider == null || !hit.collider.CompareTag("Wall"))
        {
            if (isfire == false)
            {
                isfire = true;
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                Snipersound.Play();
                bulletRb.velocity = transform.up * 20f;
                yield return new WaitForSeconds(4f);
                Destroy(bullet, 2.0f);
                isfire = false;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Inimigo"))
        {
            SetRandomPatrolDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            anim.SetTrigger("Hit");
            life--;
            Destroy(col.gameObject);
        }

        if (col.gameObject.layer == 8)
        {
            life = 0;
            Destroy(col.gameObject);
        }
        if (col.gameObject.layer == 9)
        {
            anim.SetTrigger("Hit");
            life -= 5;
            Destroy(col.gameObject);
        }
    }

    void dead()
    {
        if (life <= 0 && !isDead)
        {
            if (!weaponDropped)
            {
                GameObject Akobj = Instantiate(Ak, gameObject.transform.position, gameObject.transform.rotation);
                weaponDropped = true;
                GameObject Blood = Instantiate(blood, gameObject.transform.position, gameObject.transform.rotation);
            }
            enemyManager.EnemyDied(gameObject);
            isDead = true;
            anim.SetBool("Morreu", true);
            Destroy(gameObject, 1.5f);
        }
    }
}
