using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem particles;
    public int spellDamage;
    private List<ParticleCollisionEvent> collisionEvents;
    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("colision");
        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);
        foreach (ParticleCollisionEvent collision in collisionEvents)
        {
            Debug.Log(collision.colliderComponent.name);
            if (collision.colliderComponent.tag == "Player")
            {
                PlayerHealth playerHP = collision.colliderComponent.GetComponent<PlayerHealth>();
                
                playerHP.TakeDamage(spellDamage);
            }
        }
    }
}
