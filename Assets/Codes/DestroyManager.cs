using Unity.Mathematics;
using UnityEngine;
public class DestroyManager : MonoBehaviour
{
    [SerializeField] private GameObject parentObj = null;
    private bool destroyed;
    public int groupCount;
    public static DestroyManager destroyManagerClass;
    private void Start()
    {
        //İlgili değişkenler start anında atanır.
        destroyManagerClass = this;
        destroyed = false;
        groupCount = -1;
    }
    private void Update()
    {
        if(groupCount == 0)
        {
            //Tüm parentlar hareketini tamamladıktan sonra çalışır.
            ControlArrivedObjects();
        }
    }
    private void ControlArrivedObjects()
    {
        //Bu metot grid üzerinde kontrolleri yapmaktadır.
        groupCount = -1;
        for (int i = GridManager.gridManagerClass.grid.Length - 2; i >= 0 ; i--)
        {
            //grid dizisinin uzunluğu bizim satır sayımızdan 1 fazladır.
            //Bunun sebebi grid'i tanımlarken [row + 1] olarak tanımlamamızdır.
            //Bu sebepten gridi [grid.Length -2]. indisten başlatırız ve yukarıdan aşağı kontrol ederiz.
            for(int j = 0; j < GridManager.gridManagerClass.grid[i].Length; j++)
            {
                if (GridManager.gridManagerClass.grid[i][j] == 0)
                {
                    //grid'in ilgili satırında, ilgili sütun 0'a eşitse o satırdaki diğer sütunlar kontrol edilmez.
                    break;
                }
                else if(j == GridManager.gridManagerClass.grid[i].Length - 1)
                {
                    //Döngüdeki j sayısı bizim sütun sayımıza eşitse, ilgili satır silinir.
                    destroyed = true;
                    for(int k = 0; k < j + 1; k++)
                    {
                        GameObject refObject = GridManager.gridManagerClass.FindGameObject(i, k);
                        DestroyObjects(refObject, i, k);
                    }
                    //Puan güncellemesi yapılır.
                    LevelManager.levelManagerClass.SetSlider(GridManager.gridManagerClass.grid[i].Length);

                    MoveObjects(i, j + 1);
                    i = 0;
                }
            }
        }
        if (!destroyed)
        {
            //Eğer silme işlemi olmadıysa grid yenilenir. Ve bu metot silme işlemi olmayana kadar çağrılır.
            GridManager.gridManagerClass.Refresh();
        }
        destroyed = false;
    }
    private void DestroyObjects(GameObject refObject, int row, int column)
    {
        //Referans alınan objenin tagı değiştirilip silinir.
        refObject.tag = "destroyedObjects";

        if (refObject.transform.parent.childCount == 1)
        {
            //Eğer ki silinecek obje, parentının son childı ise parentı da silinir.
            Destroy(refObject.transform.parent.gameObject);
            refObject.transform.parent.tag = "destroyedObjects";
        }
        refObject.transform.parent = null;
        Destroy(refObject);
        GridManager.gridManagerClass.grid[row][column] = 0;
    }
    private bool GroupControl(GameObject parentRefObj)
    {
        //Bu metotta satır silindiği zaman childlar arası bağlantılar kontrol edilir.
        int childCount = parentRefObj.transform.childCount;
        float[] sortChilds = new float[childCount];
        for(int i = 0; i < childCount; i++)
        {
            //Referans parenta ait tüm childların posX değerleri sırasıyla bu dizide toplanır.
            //posX değerlerini almamızın sebebi eğer childlar arası kopma olursa posX değerine göre kopma olur.
            //Yani satır silindiği için sütunlara arası kopma olabilir. Satırlar arası kopma olamaz.
            sortChilds[i] = parentRefObj.transform.GetChild(i).transform.position.x;
        }
        for (int i = 0; i < childCount - 1; i++)
        {
            for (int j = 0; j < childCount - 1; j++)
            {
                if(sortChilds[j] > sortChilds[j + 1])
                {
                    //Bubble Sort sıralama algoritması kullanılarak dizi küçükten büyüğe sıralanıyor.
                    float temp = sortChilds[j];
                    sortChilds[j] = sortChilds[j + 1];
                    sortChilds[j + 1] = temp;
                }
            }
        }
        for(int i = 0; i < childCount - 1; i++)
        {
            if(math.abs(sortChilds[i] - sortChilds[i + 1]) > GridManager.gridManagerClass.horizontalSize)
            {
                //Sıralanmış dizideki indisler sonraki indis değeriyle karşılaştırılıyor.
                CreateGroup(parentRefObj, sortChilds, i);
                return false;
            }
        }
        //Dizideki childlar arası kopma yok.
        return true;
    }
    private void CreateGroup(GameObject parentRefObj, float[] sortChilds, int key)
    {
        //Referans alınan parent objenin ilgili child indisine kadar olan childlar yeni parent objeye taşınır.
        GameObject parentObjCopy = Instantiate(parentObj);
        Transform[] childObjects = new Transform[parentRefObj.transform.childCount];
        for (int i = 0; i < parentRefObj.transform.childCount; i++)
        {
            //Referans parenta ait tüm childlar bu diziye aktarılıyor.
            childObjects[i] = parentRefObj.transform.GetChild(i);
        }
        for (int i = 0; i <= key; i++)
        {
            for(int j = 0; j < childObjects.Length; j++)
            {
                if (childObjects[j].position.x <= sortChilds[i])
                {
                    //İlgili child değerinin posX değeri başka parenta aktarılacak child ile eşleşiyorsa taşınır.
                    childObjects[j].parent = parentObjCopy.transform;
                }
            }            
        }
    }
    private void MoveObjects(int row, int column)
    {
        //Satır silindikten sonra çağırlır. 
        for (int i = row; i > 0; i--)
        {
            //Silinen satırdan aşağısı, bir satır yukarı taşınır.
            for(int j = 0; j < column; j++)
            {
                if(GridManager.gridManagerClass.grid[i - 1][j] == 1)
                {
                    //İlgili indislerdeki tüm objeler bir satır yukarı taşınır.
                    GameObject refObject = GridManager.gridManagerClass.FindGameObject(i - 1, j);
                    Vector3 pos = refObject.transform.position;
                    refObject.transform.localPosition = new Vector3(pos.x, pos.y + GridManager.gridManagerClass.verticalSize, pos.z);
                }
                GridManager.gridManagerClass.grid[i][j] = GridManager.gridManagerClass.grid[i - 1][j];
            }
        }

        GameObject[] movableParentObjects = GameObject.FindGameObjectsWithTag("movableParentObjects");
        for (int i = 0; i < movableParentObjects.Length; i++)
        {
            //Tüm parentlar kontrol edilir. Parentların childları arasında kopma olduysa yeni parent atamaları gerçekleşir.
            while (!GroupControl(movableParentObjects[i])) ;
        }

        //Toplam parent sayısı "groupCount" değişkenine atanır. Tek tek tüm parentların hareket etmesi tetiklenir.
        movableParentObjects = GameObject.FindGameObjectsWithTag("movableParentObjects");
        groupCount = movableParentObjects.Length;
        for (int i = 0; i < movableParentObjects.Length; i++)
        {
            movableParentObjects[i].GetComponent<MoveGroup>().moveGroup = true;
        }
    }
}