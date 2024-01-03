using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Booleans")]
    public bool IsUMP;
    public bool IsAK;
    public bool IsAWP;
    public bool IsGLOCK;
    public bool IsEAGLE;
    private bool isfire;
    
    [Header("GameObjects")]
    public GameObject UMP;
    public GameObject AK;
    public GameObject AWP;
    public GameObject Glock;
    public GameObject Eagle;
    public GameObject bulletUMP;
    public GameObject bulletAK;
    public GameObject bulletAWP;
    public GameObject bulletGLOCK;
    public GameObject bulletEAGLE;
    public GameObject gameover;
    
    [Header("Components")]    
    public Transform FirepointAWP;
    public Transform firePoint;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDirection;
    public Text vidaText;
    public Text textbalas;
    
    [Header("Floats and Ints")]
    public float moveSpeed = 5f;
    public float bulletSpeed = 5f;
    public int vida = 15;
    public int balas = 60;
    public int WeaponNumber;

    [Header("Sons")] 
    public AudioClip[] Audios;
    public AudioSource Source;
    
    
    void Start()
    {
        WeaponNumber = 3;
        Source = GetComponent<AudioSource>();
        textbalas.text = balas.ToString();
        anim = GetComponent<Animator>();
        vidaText.text = vida.ToString();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (balas <=0)
        {
            WeaponNumber = 5;
            IsAK = false;
            IsUMP = false;
            IsAWP = false;
            IsGLOCK = false;
            IsEAGLE = false;
        }
        textbalas.text = balas.ToString();
        vidaText.text = vida.ToString();
        dead();
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;
        if (Input.GetButtonDown("Fire1") && balas > 0)
        {
            StartCoroutine("Tiro");
        }
        else if (Input.GetButtonDown("Fire1") && balas <= 0)
        {
            StartCoroutine("Sembala");
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Tiro()
    {

        if (isfire == false && IsAK == true)
        {
            for (int i = 0; i <= 3; i++)
            {
                PlaySoundWeapons(WeaponNumber);
                isfire = true;
                GameObject bullet = Instantiate(bulletAK, firePoint.position, firePoint.rotation);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.velocity = transform.up * bulletSpeed; 
                yield return new WaitForSeconds(0.2f);
                Destroy(bullet, 2.0f);
                balas--;
                if (balas <= 0)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(1.60f);
            isfire = false;
        }
        else if (isfire == false && IsGLOCK == true)
        {
            isfire = true;
            for (int i = 0; i <= 2; i++)
            {
                if (isfire == true)
                {
                    PlaySoundWeapons(WeaponNumber);
                    GameObject bullet = Instantiate(bulletGLOCK, firePoint.position, firePoint.rotation);
                    Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                    bulletRb.velocity = transform.up * bulletSpeed; 
                    yield return new WaitForSeconds(0.2f);
                    balas--;
                    if (balas <= 0)
                    {
                        break;
                    }
                    Destroy(bullet, 2.0f);
                }
            }
            yield return new WaitForSeconds(1.60f);
            isfire = false;
        }
        else if (isfire == false && IsAWP == true)
        {
            PlaySoundWeapons(WeaponNumber);
            isfire = true;
            GameObject bullet = Instantiate(bulletAWP, FirepointAWP.position, FirepointAWP.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = transform.up * 20f;
            yield return new WaitForSeconds(2.5f);
            Destroy(bullet, 2.5f);
            isfire = false;
            balas--;
            
        }
        else if (isfire == false && IsEAGLE == true)
        {
             PlaySoundWeapons(WeaponNumber);
            isfire = true;
            GameObject bullet = Instantiate(bulletEAGLE, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = transform.up * bulletSpeed;
            yield return new WaitForSeconds(1.5f);
            Destroy(bullet, 2.5f);
            isfire = false;
            balas--;
           
        }else if (isfire == false && IsUMP == true)
        {
             PlaySoundWeapons(WeaponNumber);
            isfire = true;
            GameObject bullet = Instantiate(bulletUMP, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = transform.up * bulletSpeed;
            yield return new WaitForSeconds(0.7f);
            Destroy(bullet, 2.0f);
            isfire = false;
            balas--;
        }
    }

    IEnumerator Sembala()
    {
        if (isfire == false && WeaponNumber == 5)
        {
            isfire = true;
            PlaySoundWeapons(WeaponNumber);
            yield return new WaitForSeconds(0.5f);
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
    private void PlaySoundWeapons(int n)
    {
        Source.clip = Audios[n];
        Source.Play();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            anim.SetTrigger("HIT");
            vida--;
            Destroy(col.gameObject);
        }
        if (col.gameObject.layer == 12)
        {
            anim.SetTrigger("HIT");
            Destroy(col.gameObject);
            vida-=5;
        }
        if (col.gameObject.layer == 11)
        {
            anim.SetTrigger("HIT");
            Destroy(col.gameObject);
            vida-= 2;
        }
        
        if (col.gameObject.tag == "AK")
        {
            WeaponNumber = 0;
            IsAK = true;
            IsUMP = false;
            IsAWP = false;
            IsGLOCK = false;
            IsEAGLE = false;
            
            AK.SetActive(true);
            UMP.SetActive(false);
            AWP.SetActive(false);
            Glock.SetActive(false);
            Eagle.SetActive(false);
            anim.SetBool("Pistol", false);
            Destroy(col.gameObject);
            balas = 50;

        }
        if (col.gameObject.tag == "UMP")
        {
            WeaponNumber = 1;
            IsAK = false;
            IsUMP = true;
            IsAWP = false;
            IsGLOCK = false;
            IsEAGLE = false;
            
            AK.SetActive(false);
            UMP.SetActive(true);
            AWP.SetActive(false);
            Glock.SetActive(false);
            Eagle.SetActive(false);
            anim.SetBool("Pistol", false);
            Destroy(col.gameObject);
            balas = 25;
        }
        if (col.gameObject.tag == "AWP")
        {
            WeaponNumber = 2;
            IsAK = false;
            IsUMP = false;
            IsAWP = true;
            IsGLOCK = false;
            IsEAGLE = false;
            
            AK.SetActive(false);
            UMP.SetActive(false);
            AWP.SetActive(true);
            Glock.SetActive(false);
            Eagle.SetActive(false);
            anim.SetBool("Pistol", false);
            Destroy(col.gameObject);
            balas = 15;
        }
        if (col.gameObject.tag == "GLOCK")
        {
            WeaponNumber = 3;
            IsAK = false;
            IsUMP = false;
            IsAWP = false;
            IsGLOCK = true;
            IsEAGLE = false;
            
            AK.SetActive(false);
            UMP.SetActive(false);
            AWP.SetActive(false);
            Glock.SetActive(true);
            Eagle.SetActive(false);
            anim.SetBool("Pistol", true);
            Destroy(col.gameObject);
            balas = 60;
        }
        if (col.gameObject.tag == "EAGLE")
        {
            WeaponNumber = 4;
            IsAK = false;
            IsUMP = false;
            IsAWP = false;
            IsGLOCK = false;
            IsEAGLE = true;
            
            AK.SetActive(false);
            UMP.SetActive(false);
            AWP.SetActive(false);
            Glock.SetActive(false);
            Eagle.SetActive(true);
            anim.SetBool("Pistol", true);
            Destroy(col.gameObject);
            balas = 25;
        }
    }
}