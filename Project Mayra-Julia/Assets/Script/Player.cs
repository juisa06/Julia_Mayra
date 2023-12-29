using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public Transform firePoint; 
    private bool isfire;
    public int vida = 15;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    public GameObject gameover;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        dead();
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine("Tiro");
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Tiro()
    {
        if (isfire == false)
        {
            isfire = true;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = transform.up * bulletSpeed;
            yield return new WaitForSeconds(1f);
            Destroy(bullet, 2.0f);
            isfire = false;
        }
    }

    void dead()
    {
        if (vida <=0)
        {
            Destroy(gameObject);
            gameover.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            vida--;
        }
    }
}