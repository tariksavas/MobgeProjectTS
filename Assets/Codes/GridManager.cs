using UnityEngine;
public class GridManager : MonoBehaviour
{
    [SerializeField] private int column = 10;
    [SerializeField] private int row = 18;
    [SerializeField] private int offset = 2;
    [SerializeField] private int yellowBlockCount = 6;
    [SerializeField] private int yellowAreaMinWidth = 2;
    [SerializeField] private int deadLineRowHeight = 15;
    [SerializeField] private int deadLineColumnStart = 0;
    [SerializeField] private int deadLineColumnFinish = 0;
    [SerializeField] private GameObject blockObject = null;
    [SerializeField] private GameObject deadLineObject = null;
    [SerializeField] private GameObject yellowBlockObject = null;
    [SerializeField] private GameObject gridObjectsParent = null;
    [SerializeField] private Color highlightAreaColor = new Color();
    private int availableAreaStart, availableAreaFinish;
    private int yellowPosXStart, yellowPosXFinish, yellowBlockCounter;
    public int[][] grid;
    public int[][] deadLine;
    public float vertical, horizontal, verticalSize, horizontalSize;
    public static GridManager gridManagerClass;
    private void Awake()
    {
        //Kameranın ölçüleri alınarak gerekli grid boyutlandırması yapılır.
        gridManagerClass = this;

        float orthographicSizeY = (float)Camera.main.orthographicSize;
        float orthographicSizeX = orthographicSizeY * ((float)Screen.width / (float)Screen.height);

        vertical = orthographicSizeY * 0.9f;
        verticalSize = vertical / ((float)row / 2);

        horizontal = orthographicSizeX;
        horizontalSize = horizontal / ((float)column / 2);

        //grid dizisi tanımlanır. Dizideki tüm elemanlar row ve column deişkenine göre 0 değerini alır.
        //Fakat girilen row değişkeninin bir üst satırına 1 değeri atanır.
        grid = new int[row + 1][];
        deadLine = new int[row + 1][];
        for(int i = 0; i < row + 1; i++)
        {
            grid[i] = new int[column];
            deadLine[i] = new int[column];
            for (int j = 0; j < column; j++)
            {
                if (i == row)
                {
                    grid[i][j] = 1;
                }
                else
                {
                    grid[i][j] = 0;
                }
            }
        }
    }
    private void Start()
    {
        //Sahne başladığında grid oluşturulur.
        Time.timeScale = 1;

        CreateDeadLine();
        Refresh();
    }
    public void Refresh()
    {
        //Her blok gönderiminden sonra bu metot çağrılır.
        RemoveGrid();
        CalculateAvailableArea();
        CalculateYellowSpawnArea();
        CreateGrid();
    }
    private void CreateDeadLine()
    {
        //Dead-line oluşturmak için kullanılan metottur.
        //Editörden alınan değerlere göre grid üzerinde bir yerlerde bloklar arasında oluşturulur.
        GameObject deadLineObjectCopy = Instantiate(deadLineObject);
        float yPos = (deadLineRowHeight * verticalSize) - vertical;
        float xPos = (((float)(deadLineColumnFinish - deadLineColumnStart + 1) / 2) * horizontalSize) - horizontal;
        deadLineObjectCopy.transform.position = new Vector2(xPos, yPos);
        deadLineObjectCopy.transform.localScale = new Vector2((deadLineColumnFinish - deadLineColumnStart + 1) * horizontalSize, verticalSize / 10);
        for(int i = 0; i < deadLineRowHeight; i++)
        {
            for(int j = deadLineColumnStart; j <= deadLineColumnFinish; j++)
            {
                //Dizinin bu indislerine blok yerleşirse oyun kaybedilir.
                deadLine[i][j] = 2;
            }
        }
    }
    private void CalculateAvailableArea()
    {
        //Available-area hesaplaması mevcut column sayısından offset sayısı çıkarılarak bir bütün halinde oluşturulur.
        availableAreaStart = Random.Range(0, column);
        while (availableAreaStart > offset)
        {
            availableAreaStart = Random.Range(0, column);
        }
        availableAreaFinish = availableAreaStart + (column - offset);
    }
    private void CalculateYellowSpawnArea()
    {
        //Available-area içerisinde rastgele oluşturulur.
        yellowPosXStart = Random.Range(availableAreaStart, availableAreaFinish - yellowAreaMinWidth + 1);
        int maxYellowPosXFinish = yellowPosXStart + yellowBlockCount;
        if (maxYellowPosXFinish < availableAreaFinish + 1)
        {
            yellowPosXFinish = Random.Range(yellowPosXStart + yellowAreaMinWidth, maxYellowPosXFinish);
        }
        else
        {
            yellowPosXFinish = Random.Range(yellowPosXStart + yellowAreaMinWidth, availableAreaFinish + 1);
        }
        //Editörden alınan değere kadar sayan sayıcımız her random düzenlemede 0'lanır.
        yellowBlockCounter = 0;
    }
    private void CreateYellowBlocks(Transform refObjectTransform)
    {
        //Referans alınan transform değerine göre, sarı blokları, tıklanabilmesi için 1 br önümüzde oluşturur.
        GameObject yellowBlockObjectCopy = Instantiate(yellowBlockObject);
        yellowBlockObjectCopy.transform.position = new Vector3(refObjectTransform.position.x, refObjectTransform.position.y, refObjectTransform.position.z - 1);
        yellowBlockObjectCopy.transform.localScale = refObjectTransform.localScale;
        //Her sarı blok oluşturulduktan sonra sayıcımızın değeri 1 artar. Ta ki Editörden alınan sarı blok sayısına kadar.
        yellowBlockCounter++;
    }
    private void CreateGrid()
    {
        //İlk olarak parent clone oluşturulur ve altına bloklar oluşturulur.
        GameObject backGroundObjectsParentCopy = Instantiate(gridObjectsParent);
        for (int rowCounter = 0; rowCounter < row; rowCounter++)
        {
            for(int columnCounter = 0; columnCounter < column; columnCounter++)
            {
                //Satır ve sütun değerine göre bloklar oluşturulur. Konum ve scale ayarlamaları yapılır.
                GameObject blockObjectCopy = Instantiate(blockObject);
                blockObjectCopy.transform.parent = backGroundObjectsParentCopy.transform;
                blockObjectCopy.transform.position = new Vector2((columnCounter * horizontalSize) - horizontal + (horizontal / column), (rowCounter * verticalSize) - vertical + (vertical / row));
                blockObjectCopy.transform.localScale = new Vector2(horizontalSize, verticalSize);

                if (columnCounter >= yellowPosXStart && columnCounter < yellowPosXFinish)
                {
                    //Sarı blokların hizasında isek blok rengi ayarlanır.
                    blockObjectCopy.GetComponent<SpriteRenderer>().color = highlightAreaColor;

                    if (yellowBlockCounter < yellowBlockCount)
                    {
                        //Sarı blok sayısına gelene dek sarı blok oluşturulur.
                        CreateYellowBlocks(blockObjectCopy.transform);
                    }
                }
            }
        }
    }
    public int FindColumn(float posX)
    {
        //Referans alınan x pozisyonuna göre grid üzerindeki sütun değeri bulunur
        return (int)System.Math.Round(((posX - (horizontalSize / 2) + horizontal)) / horizontalSize);
    }
    public int FindRow(float posY)
    {
        //Referans alınan y pozisyonuna göre grid üzerindeki satir değeri bulunur
        return (int)System.Math.Round(((posY - (verticalSize / 2) + vertical)) / verticalSize);
    }
    public GameObject FindGameObject(int row, int column)
    {
        //Referans alınan satır ve sütun değerine göre obje döndürülür.
        GameObject[] obj = GameObject.FindGameObjectsWithTag("arrivedObjects");
        for(int i = 0; i < obj.Length; i++)
        {
            int objRow = FindRow(obj[i].transform.position.y);
            int objColumn = FindColumn(obj[i].transform.position.x);
            if (row == objRow && column == objColumn)
            {
                return obj[i];
            }
        }
        return null;
    }
    private void RemoveGrid()
    {
        //Blok gönderiminden sonra çağrılan metottur.
        GameObject[] removableObjects = GameObject.FindGameObjectsWithTag("gridObjectsParent");
        for(int i =0;i < removableObjects.Length; i++)
        {
            Destroy(removableObjects[i]);
        }
        GameObject[] selectableObjects = GameObject.FindGameObjectsWithTag("selectableObjects");
        for (int i = 0; i < selectableObjects.Length; i++)
        {
            Destroy(selectableObjects[i]);
        }
    }
}