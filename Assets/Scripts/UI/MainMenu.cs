using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadClassic()
    {
        SceneManager.LoadScene("Classic");
    }

    public void LoadModern()
    {
        SceneManager.LoadScene("Modern");
    }
}