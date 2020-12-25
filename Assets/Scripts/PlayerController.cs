﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private int lifePoints = 3;
    private int score = 0;

    [SerializeField] private float speed;
    [SerializeField] private float thrustersBooster = 0.5f;
    [SerializeField] private float boundLimitTop;
    [SerializeField] private float boundLimitBottom;
    [SerializeField] private float boundLimitX;
    //this variable is necessary to not mess with the actual speed variable which already has a ratio link to coroutine
    //for example if we don't use this variable, setting back the speed to normal value may cause unwanted behavior if a speedUp Coroutine is already running
    private float thrusterBonusSpeed = 0;

    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject tripleShotPrefab;
    [SerializeField] private float laserOffset = 0;
    [SerializeField] private float fireRate = 2;
    private float canFire;

    private bool isTripleShotActive = false;
    private Coroutine tripleShotPowerUpCoroutine;

    [SerializeField] private float speedPowerup = 2;
    private bool isSpeedActive = false;
    private Coroutine speedPowerUpCoroutine;

    private int shieldHP = 0;
    private GameObject shieldGameObject;
    private SpriteRenderer shieldSprite;


    private readonly GameObject[] engines = new GameObject[2];

    private SpawnManager spawnManager;
    private UIManager ui;

    private AudioSource audioSource;
    [SerializeField] private AudioClip laserClip;
    [SerializeField] private AudioClip powerupClip;

    /*----------------------- Unity Overrides starts here -----------------------*/
    private void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if(spawnManager == null)
        {
            Debug.LogError(nameof(spawnManager) + " is null.");
        }

        shieldGameObject = transform.GetChild(0).gameObject;

        if(shieldGameObject == null)
        {
            Debug.LogError("Shield GameObject not found.");
            Debug.LogError(System.Environment.StackTrace);
        }
        else
        {
            shieldSprite = shieldGameObject.GetComponent<SpriteRenderer>();
            if(shieldSprite == null)
            {
                Debug.LogError("Shield sprite renderer not found. \n" + StackTraceUtility.ExtractStackTrace());
            }
        }

        
        for(int i = 0; i < 2; i++ )
        {
            engines[i] = transform.GetChild(i+2).gameObject;

            if (engines[i] == null)
            {
                Debug.LogError("Engine " + i + " GameObject not found.");
                Debug.LogError(System.Environment.StackTrace);
            }
        }


        ui = FindObjectOfType<UIManager>();

        if (ui == null)
        {
            Debug.LogError("Couldn't find UI Manager.");
            Debug.LogError(System.Environment.StackTrace);
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("Couldn't find AudioSource Manager.");
            Debug.LogError(System.Environment.StackTrace);
        }
    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && canFire <= Time.time)
            FireLaser();
    }

    /*----------------------- Unity Overrides starts here -----------------------*/

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        
        if(ui != null)
        {
            ui.UpdateScore(score);
        }
    }

    /*----------------------- Control Section (Moves) -----------------------*/
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        //Thrusters Booster
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //Acceleration per second
            thrusterBonusSpeed += thrustersBooster * Time.deltaTime;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            thrusterBonusSpeed = 0;
        }

        transform.Translate(direction *  ( speed + thrusterBonusSpeed) * Time.deltaTime);


        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, boundLimitBottom, boundLimitTop), 0);

        /* this is the alternative code of the line above
        if (transform.position.y >= boundLimitTop)
            transform.position = new Vector3(transform.position.x, boundLimitTop, 0);
        else if (transform.position.y <= boundLimitBottom)
            transform.position = new Vector3(transform.position.x, boundLimitBottom, 0);
        */
        if (transform.position.x > boundLimitX)
            transform.position = new Vector3(-boundLimitX, transform.position.y, 0);
        else if (transform.position.x < -boundLimitX)
            transform.position = new Vector3(boundLimitX, transform.position.y, 0);
    }
    /*----------------------- Control Section (Moves) ends here -----------------------*/

    /*----------------------- Combat Section  -----------------------*/
    void FireLaser()
    {
        canFire = Time.time + 1 / fireRate;
        if(isTripleShotActive)
        {
            Instantiate(tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Vector3 laserSpawnPosition = new Vector3(transform.position.x, transform.position.y + laserOffset, 0);
            Instantiate(laserPrefab, laserSpawnPosition, Quaternion.identity);
        }
        audioSource.PlayOneShot(laserClip);
    }

    public void ReceiveDamage(int amount)
    {
        if(shieldHP > 0)
        {
            switch(shieldHP)
            {
                case 3:
                    shieldHP--;
                    shieldSprite.color = Color.yellow;
                    break;
                case 2:
                    shieldHP--;
                    shieldSprite.color = Color.red;
                    break;
                case 1:
                    shieldHP--;
                    shieldGameObject.SetActive(false);
                    break;
                default:
                    Debug.LogWarning("Couldn't process Shield HP.\n" + StackTraceUtility.ExtractStackTrace());
                    break;
            }

            return;
        }

        lifePoints -= amount;
        ui.UpdateLives(lifePoints);

        if (lifePoints < 1)
        {
            spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
        else
        {
            DamageEngine();
        }
    }

    private void DamageEngine()
    {
        int engineToDamage = Random.Range(0, 2);
        
        //If random engine is not active, then SetActive
        if (!engines[engineToDamage].activeInHierarchy)
            engines[engineToDamage].SetActive(true);
        else
        {
            // Else set the other one active, since we have only 2 objects represented by 1 and 0
            // The Absolute Value of 1 - 0 is the inverse of 1
            // The Absolute value of 0 - 1 is the inverse of 0
            engines[Mathf.Abs(engineToDamage - 1)].SetActive(true);
        }

        // We do no further checks because the method called only 2 times from ReceiveDamage, at life == 2, and life == 1
    }

    /*----------------------- Combat Section ends here -----------------------*/


    /*----------------------- PowerUps Section  -----------------------*/

    public void PlayPowerupSound()
    {
        audioSource.PlayOneShot(powerupClip);
    }
    public void TripleShotPowerUp(float powerUpDuration)
    {
        if(isTripleShotActive && tripleShotPowerUpCoroutine != null)
        {
            StopCoroutine(tripleShotPowerUpCoroutine);
        }
        tripleShotPowerUpCoroutine = StartCoroutine(TripleShotPowerUpRoutine(powerUpDuration));
    }

    IEnumerator TripleShotPowerUpRoutine(float powerUpDuration)
    {
        isTripleShotActive = true;
        yield return new WaitForSeconds(powerUpDuration);
        isTripleShotActive = false;
    }

    public void SpeedPowerup(float powerUpDuration)
    {
        if (isSpeedActive && speedPowerUpCoroutine != null)
        {
            StopCoroutine(speedPowerUpCoroutine);
        }
        speedPowerUpCoroutine = StartCoroutine(SpeedPowerUpRoutine(powerUpDuration));
    }

    IEnumerator SpeedPowerUpRoutine(float powerUpDuration)
    {
        isSpeedActive = true;
        speed *= speedPowerup;
        yield return new WaitForSeconds(powerUpDuration);
        speed /= speedPowerup;
        isSpeedActive = false;
    }

    public void ShieldPowerUp()
    {
        shieldHP = 3;
        shieldSprite.color = Color.white; // original
        shieldGameObject.SetActive(true);
    }
    /*----------------------- PowerUps Section ends here -----------------------*/
}
