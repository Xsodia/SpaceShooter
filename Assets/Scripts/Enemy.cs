using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    [SerializeField] private float speed;
    [SerializeField] private float bottomBound;
    
    private bool isDestroyed;

    private PlayerController player;

    [SerializeField] private int scoreMin;
    [SerializeField] private int scoreMax;

    private Animator animator;

    private BoxCollider2D boxCollider2D;

    private AudioSource audioSource;

    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        isDestroyed = false;

        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player's script not found");
            Debug.LogException(new System.NullReferenceException());
            Debug.LogError(System.Environment.StackTrace);
        }

        spawnManager = FindObjectOfType<SpawnManager>();
        if(spawnManager == null)
        {
            Debug.LogError("Spawn Manager not found");
            Debug.LogException(new System.NullReferenceException());
            Debug.LogError(System.Environment.StackTrace);
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found");
            Debug.LogException(new System.NullReferenceException());
            Debug.LogError(System.Environment.StackTrace);
        }

        boxCollider2D = GetComponent<BoxCollider2D>();
        if (animator == null)
        {
            Debug.LogError("Collider2D not found");
            Debug.LogException(new System.NullReferenceException());
            Debug.LogError(System.Environment.StackTrace);
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("Couldn't find AudioSource Manager.");
            Debug.LogError(System.Environment.StackTrace);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < bottomBound)
        {
            if (isDestroyed)
                Destroy(gameObject);
            else
                transform.position = spawnManager.RandomSpawnPosition();
        }
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Projectile") && !isDestroyed)
        {
            if(player != null)
                player.AddScore(Random.Range(scoreMin, scoreMax));
            Destroy(other.gameObject);
            DestroySequence();
        }

        if (other.gameObject.CompareTag("Player"))
        {
            player.ReceiveDamage(damage);
            DestroySequence();
        }
    }

    void DestroySequence()
    {
        audioSource.Play();
        animator.SetTrigger("OnEnemyDeath");
        isDestroyed = true;
        StartCoroutine(ShrinkColliderRoutine(2.26f));
    }

    IEnumerator ShrinkColliderRoutine(float time)
    {
        while(boxCollider2D.size.x > 0.1f && boxCollider2D.size.y > 0.1f && time > 0)
        {
            float timeToSub = Time.deltaTime * 2;
            time -= timeToSub;
            boxCollider2D.size = new Vector3(boxCollider2D.size.x - timeToSub, boxCollider2D.size.y - timeToSub, 0);
            yield return new WaitForFixedUpdate();
        }
        boxCollider2D.size = Vector3.zero;
    }

}
