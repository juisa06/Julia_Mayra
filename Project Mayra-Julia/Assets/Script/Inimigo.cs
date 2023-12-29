using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inimigo : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float visionRange = 5f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int life = 10;

    private Transform player;
    private bool playerInSight = false;
    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 patrolDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        SetRandomPatrolDirection();
    }

    void Update()
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

    void Patrol()
    {
        if (!playerInSight) // Verifica se o jogador está à vista
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 1f); // Verifica se há obstáculos à frente
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                SetRandomPatrolDirection(); // Se houver uma parede à frente, escolhe uma nova direção aleatória
            }
            else
            {
                rb.MovePosition(rb.position + patrolDirection * moveSpeed * Time.fixedDeltaTime); // Move-se na direção atual
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
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = transform.up * 15f;
            yield return new WaitForSeconds(0.2f);
            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb2 = bullet2.GetComponent<Rigidbody2D>();
            bulletRb2.velocity = transform.up * 15f;
            yield return new WaitForSeconds(1f);
            Destroy(bullet, 2.0f);
            Destroy(bullet2, 2.0f);
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
        if (col.gameObject.tag == "Bala")
        {
            life--;
        }
    }

    void dead()
    {
        if (life <=0 )
        {
            Destroy(gameObject);
        }
    }
}
