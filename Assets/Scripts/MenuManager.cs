
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private bool gameIsPaused;
    public GameObject optionsMenu;
    public GameObject showFps;
    public PlayerController character;

    // Start is called before the first frame update
    void Start()
    {
        gameIsPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        showHideFps();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        Debug.Log("entrou");
        optionsMenu.SetActive(false);
        gameIsPaused = false;
        Time.timeScale = 1f;
        character.StopCharacter(true, false);
    }

    void PauseGame()
    {
        optionsMenu.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 0.00001f;
        character.StopCharacter(false, false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }

    public void showHideFps()
    {
        if (Input.GetKeyDown(KeyCode.F))
            showFps.SetActive(!showFps.activeSelf);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);//Main Menu
    }
}
