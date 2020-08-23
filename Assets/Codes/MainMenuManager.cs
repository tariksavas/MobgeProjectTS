using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadGameButton = null;
    private void Start()
    {
        Time.timeScale = 1;
        if (!PlayerPrefs.HasKey("level"))
        {
            //Hafızada level değişkenine ait veri yok ise loadGame butonu aktif edilmez.
            loadGameButton.SetActive(false);
        }
        else
        {
            //Hafızada level değişkenine ait veri var ise loadGame butonu aktif edilir.
            loadGameButton.SetActive(true);
        }
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
        //Hafızaya level değişkeninin değeri 1 olarak kaydedilir.
        PlayerPrefs.SetInt("level", 1);
    }
    public void HelpMenu()
    {
        SceneManager.LoadScene("HelpScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}