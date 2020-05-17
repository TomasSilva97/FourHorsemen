
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    private bool isDead;
    //private bool gameIsPaused;
    public GameObject character;
    public GameObject deathMenu;

    private PlayerController controller;

    private PlayerHealth playerHP;
    // Start is called before the first frame update
    void Start()
    {
        //gameIsPaused = false;
        isDead = false;
        controller = character.GetComponent<PlayerController>();
        deathMenu.SetActive(false);
        playerHP = character.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isDead && playerHP.isDead)
        {
            Die();
        }
    }

    void Die()
    {
        deathMenu.SetActive(true);

        isDead = true;
        controller.StopCharacter(false, true);
        //Time.timeScale = 0.00001f;
    }

    public void ResumeGame()
    {
        deathMenu.SetActive(false);
        isDead = false;
        Time.timeScale = 1f;
        controller.StopCharacter(true, false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(0);//Main Menu
    }
}
