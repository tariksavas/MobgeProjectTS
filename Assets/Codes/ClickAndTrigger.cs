using UnityEngine;
public class ClickAndTrigger : MonoBehaviour
{
    private void OnMouseOver()
    {
        //Bu objeye dokunulduğunda çalışır.
        if (Input.GetMouseButton(0) && CompareTag("selectableObjects"))
        {
            //Eğer objeye tıklandıysa ve tıklanabilir bir objeyle bu if içerisine girer.
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0);
            tag = "selectedObjects";
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("selectableObjects"))
        {
            //Objelerin ekranı doldurması durumunda veya dead-line'a çarpması durumunda oyun kaybedilir.
            Time.timeScale = 0;
            LevelManager.levelManagerClass.gameOver = true;
        }
    }
}