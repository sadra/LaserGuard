using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //Configuration Parameters
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.7f;
    [SerializeField] float upPadding = 3f;
    [SerializeField] int health = 500;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.4f;
    [SerializeField] AudioClip hitSound;
    [SerializeField] [Range(0, 1)] float hitSoundVolume = 1f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;


    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 2f;

    [Header("Control")]
    [SerializeField] bool activeJoyStick = true;


    protected Joystick joystick;


    Coroutine firingCoroutine;

    //States
    float xMin;
    float xMax;
    float yMin;
    float yMax;


    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        joystick = FindObjectOfType<Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }


    //Fire
    private void Fire()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Shoot"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    public void FirepointerDown()
    {
        firingCoroutine = StartCoroutine(FireContinuously());
    }


    public void FirepointerUp()
    {
        StopCoroutine(firingCoroutine);
    }


    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }


    //Movement
    private void Move()
    {
        var deltaX = (joystick.Horizontal + Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed;
        var deltaY = (joystick.Vertical + Input.GetAxis("Vertical")) * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }


    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - upPadding;
    }



    //Health
    public int GetHealth()
    {
        return health;
    }

    private void OnTriggerEnter2D(Collider2D collisionOther)
    {
        DamageDealer damageDealer = collisionOther.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        PeocessHit(damageDealer);
    }

    private void PeocessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position, hitSoundVolume);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        FindObjectOfType<Level>().LoadGameOver();
    }

}
