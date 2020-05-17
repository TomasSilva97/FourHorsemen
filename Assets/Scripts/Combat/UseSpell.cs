
using UnityEngine;
using UnityEngine.UI;


public class UseSpell : MonoBehaviour
{
    // Start is called before the first frame update
    public Image mana_bar;
    public int startingMana = 100;
    private int currentMana;
    private ParticleSystem particles;
    private KeyCode spellKey;
    private float refillTimer;
   
    
    void Start()
    {
        
        currentMana = startingMana;
        particles = this.transform.Find("Spell Particles").GetComponent<ParticleSystem>();
        spellKey = KeyCode.E;
        refillTimer=0;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (mana_bar == null)
        {
            GetManaBar();
        }
        if (currentMana > 0 && Input.GetKey(spellKey))
        {
            
            particles.Emit(1);
            if(mana_bar!=null)
            mana_bar.fillAmount -= 0.01f;
            currentMana--;
            
        }
        else if (currentMana < 100)
        {
            refillTimer += Time.deltaTime;
            if (refillTimer > 1)
            {
                if (currentMana > startingMana-10)
                {
                    if(mana_bar!=null)
                    mana_bar.fillAmount = 1;
                    currentMana = startingMana;
                }
                else
                {
                    if(mana_bar!=null)
                    mana_bar.fillAmount += 0.1f;
                    currentMana+=10;
                    
                }


                refillTimer = 0;
            }
            
        }
        //Debug.Log(currentMana);
    }

    void GetManaBar()
    {
        var parent = this.transform.root;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).tag == "Status")
            {
                mana_bar = parent.GetChild(i).transform.Find("Simple").transform.Find("Bars").Find("Manabar")
                    .GetComponent<Image>();
            }
        }
        
    }

    

    
}
