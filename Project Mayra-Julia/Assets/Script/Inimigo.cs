using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inimigo : MonoBehaviour
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
    private bool weaponDropped = false;

    private Transform player;
    private bool playerInSight = false;
    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 patrolDirection;
    private int BalaArma;
    public float Recarga;
    private int balaAK = 2;
    private int balaUMP = 1;
    private int balaEAGLE = 1;
    private int balaAKGLOCK =3;

    public bool EnemyAK;
    public bool EnemyUMP;
    public bool EnemyEAGLE;
    public bool EnemyGLOCK;

    public AudioClip[] Audios;
    private AudioSource Source;
    public int weaponsound;

    private EnemyManager enemyManager;
    void Start()
    {
        Source = GetComponent<AudioSource>();
        enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        enemyManager.AddEnemy(gameObject);
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        SetRandomPatrolDirection();
    }

    private void Awake()
    {
        if (EnemyAK == true)
        {
            BalaArma = balaAK;
            weaponsound = 0;
        }
        else if (EnemyUMP == true)
        {
            weaponsound = 1;
            BalaArma = balaUMP;
        }
        else if (EnemyEAGLE == true)
        {
            weaponsound = 2;
            BalaArma = balaEAGLE;
        }
        else if (EnemyGLOCK == true)
        {
            weaponsound = 3;
            BalaArma = balaAKGLOCK;
        }
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 1f); 
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
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
            if (BalaArma > 0)
            {
                while (BalaArma >= 0)
                {
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                    PlaySoundWeapons(weaponsound);
                    bulletRb.velocity = transform.up * 15f;
                    yield return new WaitForSeconds(0.2f);
                    BalaArma--;
                    Destroy(bullet, 2.0f);
                    if (BalaArma <= 0)
                    {
                        yield return new WaitForSeconds(Recarga);
                    }
                }
            }

            if (EnemyAK == true)
            {
                BalaArma = balaAK;
            }
            else if (EnemyUMP == true)
            {
                BalaArma = balaUMP;
            }
            else if (EnemyEAGLE == true)
            {
                BalaArma = balaEAGLE;
            }
            else if (EnemyGLOCK == true)
            {
                BalaArma = balaAKGLOCK;
            }
            
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
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
        }
        if (col.gameObject.layer == 9)
        {
            anim.SetTrigger("Hit");
            life -= 3;
            Destroy(col.gameObject);
        }
    }
    private void PlaySoundWeapons(int n)
    {
        Source.clip = Audios[n];
        Source.Play();
    }
    void dead()
    {
        if (life <= 0 && !isDead)
        {
            StopCoroutine("Tiro");
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
