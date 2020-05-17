
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void Quit()
    {
        SceneManager.LoadScene(1);
    }

}
