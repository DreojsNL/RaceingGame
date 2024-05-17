using UnityEngine.SceneManagement;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exiting game");
    }
}
