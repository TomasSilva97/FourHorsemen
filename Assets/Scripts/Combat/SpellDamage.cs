
using System.Collections.Generic;
using UnityEngine;

public class SpellDamage : MonoBehaviour
{

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
        //Debug.Log("colision");
        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);
        foreach (ParticleCollisionEvent collision in collisionEvents)
        {
            Debug.Log(collision.colliderComponent);
            if (collision.colliderComponent.tag == "Enemy")
            {
                EnemyHealth enemyHP = collision.colliderComponent.GetComponent<EnemyHealth>();
                
                enemyHP.TakeDamage(spellDamage,collision.intersection);
            }
        }
    }
}
