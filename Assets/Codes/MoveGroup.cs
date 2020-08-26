using UnityEngine;
public class MoveGroup : MonoBehaviour
{
    private float translateTimer, translateTime = 0.1f;
    public bool moveGroup;
    void Start()
    {
        translateTimer = -1;
    }
    private bool MoveControl(int row, int column)
    {
        //Gelen satır ve sütun değerine göre bir adım yukarısı kontrol edilir
        if (GridManager.gridManagerClass.grid[row + 1][column] == 0)
            return true;
        else
        {
            GameObject childNow = GridManager.gridManagerClass.FindGameObject(row, column);
            GameObject childForward = GridManager.gridManagerClass.FindGameObject(row + 1, column);
            if(childNow != null && childForward != null && childNow.transform.parent == childForward.transform.parent)
            {
                //Gelen satır ve sütun değerine göre bir adım yukarıdaki objeyle aynı Parent'a sahiplerse yine true döndürür.
                return true;
            }
            //Bir adım yukarısı dolu, hareket edemez.
            return false;
        }
    }
    private bool MoveGroupControl()
    {
        //Parent'ın bir yukarıdaki adımı için bütün childlar tek tek kontrol edilir.
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 childPos = transform.GetChild(i).position;
            int gridRow = (int)GridManager.gridManagerClass.FindRow(childPos.y);
            int gridColumn = (int)GridManager.gridManagerClass.FindColumn(childPos.x);
            if (!MoveControl(gridRow, gridColumn))
            {
                //Çağrılan metoda gönderilen satır ve sütun değerlerine göre false değeri 
                //dönerse bu metot da false değeri döndürür ve hareket engellenir.
                return false;
            }
        }
        //Tüm childlar kontrol edilmiş olur ve hepsinin bir yukarıdaki adımı müsaittir.
        return true;
    }
    private void FillGrid()
    {
        //Hareket tamamlanmış olur ve tüm childların bulunduğu grid değerleri doldurulur.
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).tag = "arrivedObjects";
            Vector3 childPos = transform.GetChild(i).position;
            int gridRow = (int)GridManager.gridManagerClass.FindRow(childPos.y);
            int gridColumn = (int)GridManager.gridManagerClass.FindColumn(childPos.x);
            GridManager.gridManagerClass.grid[gridRow][gridColumn] = 1;
            if (GridManager.gridManagerClass.deadLine[gridRow][gridColumn] == 2)
            {
                Time.timeScale = 0;
                LevelManager.levelManagerClass.gameOver = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if (moveGroup)
        {
            translateTimer -= Time.deltaTime;
            if (translateTimer < 0)
            {
                if (MoveGroupControl())
                {
                    //Bool metotların kontrolüyle bir yukarıdaki grid boş ise hareket başlar.
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Vector3 childPos = transform.GetChild(i).position;
                        transform.GetChild(i).localPosition = new Vector3(childPos.x, childPos.y + GridManager.gridManagerClass.verticalSize, childPos.z);

                        //Tekrar hareket eden tüm childlar yer değiştirirken grid'teki değerler de bir üst satıra taşınır.
                        int gridRow = (int)GridManager.gridManagerClass.FindRow(childPos.y);
                        int gridColumn = (int)GridManager.gridManagerClass.FindColumn(childPos.x);
                        GridManager.gridManagerClass.grid[gridRow + 1][gridColumn] = GridManager.gridManagerClass.grid[gridRow][gridColumn];
                        GridManager.gridManagerClass.grid[gridRow][gridColumn] = 0;
                    }
                }
                else
                {
                    //Bool metotların kontrolüyle bir yukarıdaki grid dolu ise hareket sağlanmaz.
                    FillGrid();
                    moveGroup = false;
                    if (!LevelManager.levelManagerClass.gameOver)
                    {
                        DestroyManager.destroyManagerClass.groupCount--;
                    }
                }
                translateTimer = translateTime;
            }
        }
    }
}