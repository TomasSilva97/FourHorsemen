using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class EnemyChasing : MonoBehaviour
{

    public Transform goal;
    Vector3 previousPosition;
    Animator animator;

    public NavMeshAgent agent;
    public float attackRange;

    [SerializeField]
    public float range;
    EnemyHealth enemyHealth;  


    void Start()
    {
        if (goal == null)
            AssignGoal();
        previousPosition = transform.position;
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent <EnemyHealth> ();
    }

    void Update()
    {
        // Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"));
        /*

        Quaternion targetRotation = Quaternion.LookRotation(goal.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
        transform.position += transform.forward * 1f * Time.deltaTime;
        if (previousPosition != transform.position)*/
        if(enemyHealth.currentHealth > 0){
        agent.stoppingDistance = attackRange;
      
        float distance = Vector3.Distance(goal.position, transform.position);

        if(distance < range){
            agent.SetDestination(goal.position);
            agent.isStopped = false;
            //animator.SetBool("IsWalking",true);
            float dist=agent.remainingDistance;
            //Debug.Log(agent.remainingDistance);
            if (agent.remainingDistance<=attackRange) {
                animator.SetBool("IsWalking",false);
                animator.SetBool("IsAttacking",true);
                
                       
                //Debug.Log("Reached destination");
                      
                       

            }

            else{
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsAttacking",false);
                //agent.isStopped = true;
            }
        }
        else
        {
            animator.SetBool("IsWalking",false);
            agent.Stop();
            
            
        }
             

        
        }

       

        
        /*  if(previousPosition != transform.position){
               animator.SetBool("IsWalking", true);
          }
          else{
             animator.SetBool("isWalking", false);
          }
          previousPosition = transform.position;*/

    }

    void AssignGoal()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        
        
        Random randomPlayer = new Random();
        int index = randomPlayer.Next(0,players.Length);
        Debug.Log(index);
        goal = players[index].transform;
    }


}
