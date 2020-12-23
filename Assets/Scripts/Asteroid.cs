using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] private float torqueForce;

    [SerializeField] private int health = 10;

    private SpawnManager spawnManager;

    [SerializeField] private GameObject explosionPrefab;


    private bool isDestroyed;
    // Start is called before the first frame update
    void Start()
    {
        isDestroyed = false;

        spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("'SpawnManager not found.");
            Debug.LogError(System.Environment.StackTrace);
        }

        rb2d = GetComponent<Rigidbody2D>();
        if(rb2d == null)
        {
            Debug.LogError("RigidBody2D not found.");
            Debug.LogError(System.Environment.StackTrace);
        }


        rb2d.AddTorque(torqueForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Projectile") && !isDestroyed)
        {
            ReceiveDamage();
            Destroy(other.gameObject); 
        }
    }

    void ReceiveDamage()
    {
        if (health > 1)
            health--;
        else
        {
            isDestroyed = true;
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            spawnManager.Invoke(nameof(spawnManager.OnAsteroidDestroy), 3);
            Destroy(explosionInstance, 2.38f);
            Destroy(gameObject, 0.25f);
        }
    }

}
