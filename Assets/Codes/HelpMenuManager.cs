using UnityEngine;
using UnityEngine.SceneManagement;
public class HelpMenuManager : MonoBehaviour
{    public void BackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}