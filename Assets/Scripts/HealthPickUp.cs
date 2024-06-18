using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int healthRestore = 20;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    AudioSource pickUpSource;

    private void Awake()
    {
        pickUpSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable && damageable.Health < damageable.MaxHealth)
        {
            bool wasHealed = damageable.Heal(healthRestore);

            if (wasHealed)
            {
                if (pickUpSource)
                {
                    AudioSource.PlayClipAtPoint(pickUpSource.clip, gameObject.transform.position, pickUpSource.volume);
                }
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
