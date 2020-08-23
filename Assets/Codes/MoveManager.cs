using UnityEngine;
using UnityEngine.UI;
public class MoveManager : MonoBehaviour
{
    [SerializeField] private int minimumBlockLimit = 3;
    [SerializeField] private float warningTime = 2f;
    [SerializeField] private Text warning = null;
    [SerializeField] private GameObject parentObj = null;
    [SerializeField] private Color[] objectsColor = null;
    private float warningTimer;
    public static MoveManager moveManagerClass;
    private void Start()
    {
        //Uyarı veren txt'yi start anında görünmez yapar.
        moveManagerClass = this;
        warning.enabled = false;
    }
    private void Update()
    {
        if (warning.enabled)
        {
            //Text'i görünür olması durumunda süre işlemeye başlar.
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0)
            {
                warning.enabled = false;
            }
        }
    }
    public void OnMouseExit()
    {
        //Ekranda bulunan gizli bir butona atanmıştır.
        //Bu metot elimizi objelerden çektiğimiz anda devreye girmektedir.
        GameObject[] selectedObjects = GameObject.FindGameObjectsWithTag("selectedObjects");
        if(selectedObjects.Length > 0 && selectedObjects.Length < minimumBlockLimit)
        {
            //Seçilen obje sayısı minimmum block limitinden küçük olması durumunda uyarı verir.
            warning.enabled = true;
            warning.text = "Please select minimum " + minimumBlockLimit + " blocks";
            warningTimer = warningTime;

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                selectedObjects[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 0.4f);
                selectedObjects[i].tag = "selectableObjects";
            }
        }
        else if(selectedObjects.Length >= minimumBlockLimit)
        {
            //Seçilen objeler yukarı gönderilmeye hazırlanır.
            GameObject[] selectableObjects = GameObject.FindGameObjectsWithTag("selectableObjects");
            for(int i = 0; i < selectableObjects.Length; i++)
            {
                Destroy(selectableObjects[i]);
            }

            //Her gönderimde parent obje oluşturulur ve gönderilen objeler tek parent obje altında gönderilir.
            GameObject parentObjCopy = Instantiate(parentObj);
            Color parentObjColor = RandomColor();
            for (int j = 0; j < selectedObjects.Length; j++)
            {
                selectedObjects[j].transform.parent = parentObjCopy.transform;
                selectedObjects[j].GetComponent<SpriteRenderer>().color = parentObjColor;
            }
            DestroyManager.destroyManagerClass.groupCount = 1;
            //Parent objede bulunan Scriptteki bool değişkeni true olarak atanır ve objeler hareket etmeye başlar.
            selectedObjects[0].GetComponentInParent<MoveGroup>().moveGroup = true;
        }
    }
    private Color RandomColor()
    {
        //Editörden alınan random renklere göre renk seçip döndürür.
        return objectsColor[Random.Range(0, objectsColor.Length)];
    }
}