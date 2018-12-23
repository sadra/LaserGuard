using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Configuration Parameters
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.7f;
    [SerializeField] float upPadding = 3f;
    [SerializeField] float health = 200;
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
    [SerializeField] float projectileFiringPeriod = 0.1f;



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
        //StartCoroutine(PrintAndWait());
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
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(Firecontinously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
     
    IEnumerator Firecontinously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
        }
    }


    //Movement
    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

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
