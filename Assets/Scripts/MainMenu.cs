
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void SinglePlayer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(4); //First level scene
    }

    public void MultiPlayer()
    {
        SceneManager.LoadScene(1); //Multiplayer scene
    }

    public void OptionsMenu()
    {
        //fazer options menu
    }

    public void Back()
    {
        SceneManager.LoadScene(0);//Main Menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
