
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public int attackDamage;
    public int attackRange;

    private float attackCooldown;
    private float attackTimer;
    private Ray attackRay;
    private RaycastHit attackHit;
    
    
    // Start is called before the first frame update
    void Start()
    {
        attackCooldown = 1.0f;
        attackTimer = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        if(Input.GetButton ("Fire1") && attackTimer >= attackCooldown)
        {
           
            Attack();
        }

    }

    void Attack()
    {
        attackTimer = 0f;
        attackRay.origin = transform.position;
        attackRay.direction = transform.forward;
        if (Physics.Raycast(attackRay, out attackHit, attackRange))
        {
            if (attackHit.collider.tag == "Enemy")
            {
                EnemyHealth enemyHP = attackHit.collider.GetComponent<EnemyHealth>();
                
                    enemyHP.TakeDamage(attackDamage,attackHit.point);
                
            }
        }
    }
}
