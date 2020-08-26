using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] private int levelLimit = 100;
    [SerializeField] private int pointPerBlock = 1;
    [SerializeField] private Slider levelSlider = null;
    [SerializeField] private Text PointT = null;
    [SerializeField] private Text LimitPointT = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private GameObject nextLevelMenu = null;
    [SerializeField] private GameObject gameOverMenu = null;
    [SerializeField] private GameObject stopMenu = null;
    [SerializeField] private Animator gameOverAnimator = null;
    [SerializeField] private Animator nextLevelAnimator = null;
    [SerializeField] private AudioSource gameAudio = null;
    [SerializeField] private AudioClip destroyAudio = null;
    [SerializeField] private AudioClip gameOverAudio = null;
    [SerializeField] private AudioClip levelCompletedAudio = null;
    private int level, levelPoint;
    public bool gameOver;
    public static LevelManager levelManagerClass;
    private void Start()
    {
        levelManagerClass = this;

        //Slider'a tıklanılmaması için kullanılır.
        levelSlider.enabled = false;

        //Referans alınan menü objeleri start anında aktif edilmez.
        gameOver = false;
        gameOverMenu.SetActive(false);
        stopMenu.SetActive(false);
        nextLevelMenu.SetActive(false);

        //Slider'a ve level'a ait değişkenler düzenlenir. Gerekli metotlar çağrılır.
        levelPoint = 0;
        SetSliderMaxValue();

        level = PlayerPrefs.GetInt("level");
        SetLevel();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Oyun durduruldu
            Time.timeScale = 0;
            stopMenu.SetActive(true);
        }
        if (gameOver)
        {
            //Başka bir script tarafından bu bool değişken oyunu kaybettiğimiz anda true edilir.
            gameOver = false;
            gameOverMenu.SetActive(true);
            gameAudio.clip = gameOverAudio;
            gameAudio.Play();
            gameOverAnimator.Play("GameOverAnimation");
            gameOverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
    private void SetSliderMaxValue()
    {
        //Slider'a ve Txtlere ait değerler düzenlenir.
        levelSlider.maxValue = levelLimit;
        levelSlider.value = levelPoint;

        LimitPointT.text = levelLimit.ToString();
        PointT.text = levelSlider.value.ToString();
    }
    public void SetSlider(int column)
    {
        gameAudio.clip = destroyAudio;
        gameAudio.Play();
        levelPoint += pointPerBlock * column;
        levelSlider.value = levelPoint;

        PointT.text = levelSlider.value.ToString();

        if (levelPoint >= levelLimit)
        {
            //Puan levelLimit puanından fazla ise oyun kazanılır.
            Time.timeScale = 0;
            nextLevelMenu.SetActive(true);
            gameAudio.clip = levelCompletedAudio;
            gameAudio.Play();
            nextLevelAnimator.Play("LevelCompletedAnimation");
            nextLevelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
    private void SetLevel()
    {
        //Hafızadaki level değişkeni güncellenir.
        PlayerPrefs.SetInt("level", level);
        levelText.text = PlayerPrefs.GetInt("level").ToString();
    }
    public void Continue()
    {
        //Durdurulan oyun devam etmeye başladı.
        stopMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void NextLevel()
    {
        //Oyunu kazandığımızda karşımıza çıkan Next level butonu için bir metot.
        Debug.Log("Next levels are not ready yet");
        level++;
        SetLevel();
        SceneManager.LoadScene("GameScene");
    }
    public void Replay()
    {
        //Kaybettiğimizde tekrar oynamamız için butona tanımlanmış bir metot.
        SceneManager.LoadScene("GameScene");
    }
    public void MainMenu()
    {
        //Menüye dönmek için butona tanımlanmış bir metot.
        SceneManager.LoadScene("MainMenu");
    }
}