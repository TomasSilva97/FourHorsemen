
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;

public class PlayerHealth : MonoBehaviour
{
    public Image hp_bar;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead;
    public int blocking_damage;
    private float regenCooldown;
    private float regenTimer;
    public PlayerController controller;
    private PlayerManager player_manager;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        regenCooldown = 3;
        regenTimer = 0;
        controller = GetComponentInParent<PlayerController>();
        player_manager = GetComponentInParent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp_bar == null)
            GetHpBar();
        if (isDead)
            return;
        if (regenCooldown >= 3)
        {
            if (regenTimer >= 0.4)
            {
                if (currentHealth > maxHealth - 5)
                {
                    currentHealth = 100;
                    if (hp_bar != null)
                        hp_bar.fillAmount = 1;
                    regenTimer = 0;
                }
                else
                {
                    currentHealth += 5;
                    if (hp_bar != null)
                        hp_bar.fillAmount += 0.05f;
                    regenTimer = 0;
                }
            }
            else
            {
                regenTimer += Time.deltaTime;
            }
        }
        else
        {
            regenCooldown += Time.deltaTime;
        }
    }

    private void GetHpBar()
    {
        var parent = this.transform.root;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).tag == "Status")
            {
                hp_bar = parent.GetChild(i).transform.Find("Simple").transform.Find("Bars").Find("Healthbar")
                    .GetComponent<Image>();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;
        //Debug.Log("Enemy Attacked for "+amount);
        //Debug.Log("Enemy hp: "+currentHealth);
        if (!Photon.Pun.PhotonNetwork.IsConnected)
        {
            if (controller.GetAnimator().GetBool("IsBlocking"))
            {
                amount -= blocking_damage;
                if (amount < 0) amount = 0;
                controller.GetAnimator().SetTrigger("DamagedBlocked");
            }
        }
        else
        {
            if (player_manager.GetAnimator().GetBool("IsBlocking"))
            {
                amount -= blocking_damage;
                if (amount < 0) amount = 0;
                controller.GetAnimator().SetTrigger("DamagedBlocked");
            }
        }

        currentHealth -= amount;
        if (hp_bar != null)
            hp_bar.fillAmount -= ((float)amount) / 100;
        regenCooldown = 0;
        if (!Photon.Pun.PhotonNetwork.IsConnected)
            controller.DamageAnimation();
        else
            player_manager.DamageAnimation();

        if (currentHealth <= 0)
        {
            // ... the enemy is dead.
            Death();
        }
    }



    void Death()
    {
        isDead = true;
        if (!Photon.Pun.PhotonNetwork.IsConnected)
            controller.StopCharacter(false, isDead);
        else
            player_manager.StopCharacter(false, isDead);

        Debug.Log("Player Dead");
        Photon.Pun.PhotonNetwork.Disconnect();

    }
}
