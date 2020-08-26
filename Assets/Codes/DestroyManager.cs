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
        for (int i = 0; i < childCount - 1; i++)
        {
            Vector2 child1Pos = parentRefObj.transform.GetChild(i).transform.position;
            Vector2 child2Pos = parentRefObj.transform.GetChild(i + 1).transform.position;
            if (math.abs(child1Pos.y - child2Pos.y) > GridManager.gridManagerClass.verticalSize)
            {
                //Childlar sıralıdır ve sıralı 2  child arasındaki y mesafesi verticalSize değerinden büyükse parçalanır.
                CreateGroup(parentRefObj, i);
                return false;
            }
            else if (math.abs(child1Pos.x - child2Pos.x) > GridManager.gridManagerClass.horizontalSize)
            {
                //Childlar sıralıdır ve sıralı 2  child arasındaki x mesafesi horizontalSize değerinden büyükse parçalanır.
                CreateGroup(parentRefObj, i);
                return false;
            }
        }
        return true;
    }
    private void CreateGroup(GameObject parentRefObj, int key)
    {
        //Referans alınan parent objenin ilgili child indisine kadar olan childlar yeni parent objeye taşınır.
        GameObject parentObjCopy = Instantiate(parentObj);
        for(int i = 0; i <= key; i++)
        {
            parentRefObj.transform.GetChild(0).transform.parent = parentObjCopy.transform;
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