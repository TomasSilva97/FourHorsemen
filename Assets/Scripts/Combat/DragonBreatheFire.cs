
using UnityEngine;

public class DragonBreatheFire : MonoBehaviour
{
    Animator animator;
    private bool isFlying;
    

    private float flyingTimer;

   ParticleSystem fireParticles;
   
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        fireParticles = this.transform.Find("Fire Particle System").GetComponent<ParticleSystem>();
        isFlying = false;
        flyingTimer = 0;
        //currentFlyingTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
        {
            if (isFlying)
            {
                flyingTimer += Time.deltaTime;
                if (flyingTimer > 0.5 && flyingTimer < 5)
                {
                    fireParticles.Emit(5);
                }

            }
            else
            {
                flyingTimer = 0;
                isFlying = true;
            }



        }
        else isFlying = false;


        //else
        //fireParticles.Stop();

    }
}
