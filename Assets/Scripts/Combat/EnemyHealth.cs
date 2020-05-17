
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;
    public int maxHealth;
    public int currentHealth;
    private bool isDead;
    ParticleSystem hitParticles;
    BoxCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        isDead = false;
        hitParticles = this.transform.Find("Hit Particles").GetComponent<ParticleSystem>();
        collider = GetComponent<BoxCollider>();
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead)
            return;
        //Debug.Log("Enemy Attacked for "+amount);
        //Debug.Log("Enemy hp: "+currentHealth);
        currentHealth -= amount;
        hitParticles.transform.position = hitPoint;
        hitParticles.Emit(15);

        if (currentHealth <= 0)
        {
            // ... the enemy is dead.
            Death();
        }
    }

    void Death()
    {
        Debug.Log("Enemy Dead");
        isDead = true;
        collider.isTrigger = true;
        GetComponent<NavMeshAgent>().enabled = false;
        animator.Play("Death");
        Destroy(gameObject, 2f);
    }
}
