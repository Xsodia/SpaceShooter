using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float bottomBoundLimit = -6f;
    [SerializeField] private float powerUpDuration = 10f;

    [SerializeField] private int powerupId;

    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player's script not found");
            Debug.LogException(new System.NullReferenceException());
            Debug.LogError(System.Environment.StackTrace);
        }
    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < bottomBoundLimit)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (player == null)
                Debug.LogException(new System.NullReferenceException());
            else
            {
                player.PlayPowerupSound();
                switch (powerupId)
                {
                    case 0:
                        player.TripleShotPowerUp(powerUpDuration);
                        break;
                    case 1:
                        player.SpeedPowerup(powerUpDuration);
                        break;
                    case 2:
                        player.ShieldPowerUp();
                        break;
                    default:
                        Debug.LogError("Powerup ID is set to a wrong number");
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
