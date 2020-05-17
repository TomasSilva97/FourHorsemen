
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    
    public float attackSpeed;
    public int attackDamage;
    private Animator animator; 
    private float attackTimer;
    
    private Transform goal;
    
    // Start is called before the first frame update
    void Start()
    {
        attackTimer = 0;
        goal = GetComponent<EnemyChasing>().goal;
        animator = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(animator.GetBool("IsAttacking"));
        if (animator.GetBool("IsAttacking"))
        {
            if (attackTimer >= attackSpeed)
            { 
               
                PlayerHealth playerHP = goal.GetComponentInParent<PlayerHealth>();
                playerHP.TakeDamage(attackDamage);
               // Debug.Log("Attack from enemy");
                attackTimer=0;
                
            }
            else attackTimer += Time.deltaTime;
        }
    }
}
