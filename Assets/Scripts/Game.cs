using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
public class Game : MonoBehaviour
{   
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject blockFab;
    [SerializeField] private GameObject grid;
    [SerializeField] private Animator animBlock;
    public int[,] matrixArray = new int[4,4]; 
    private bool[,] isMerged = new bool[4,4]; 
    private bool movedNow = false;
    public int[,] beforetemp = new int[4,4];
    private Transform[] thisgrid = new Transform[16];
    public bool isReady = false;
    public float shiftspeed;

    void Start()
    {      
        thisgrid = _gameManager.thisgrid;
        isReady = true; 

        #if UNITY_ANDROID
            shiftspeed = 1.5f;
        #endif

        #if UNITY_EDITOR
            shiftspeed = 5.0f;
        #endif 
    }

    void Update()
    { 

    }

    public void CreateBlock(Vector2Int pos, int value)
    {   
        int index = pos.x * 4 + pos.y;
        Instantiate(blockFab, thisgrid[index].transform);
        thisgrid[index].GetChild(0).name = pos.x*4 + pos.y.ToString(); 
          
        Color ButtonColor;
        ColorUtility.TryParseHtmlString(getColor(value), out ButtonColor);
        thisgrid[index].GetChild(0).GetComponent<Image>().color = ButtonColor;

        if(value == 2)
        {
            animBlock = thisgrid[index].GetChild(0).GetComponent<Animator>();
            animBlock.SetTrigger("TriggerBlock");
            thisgrid[index].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(89,84,77,255);
        }
        else if(value == 4)
        {
            animBlock = thisgrid[index].GetChild(0).GetComponent<Animator>();
            animBlock.SetTrigger("MergeBlock");
            thisgrid[index].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(89,84,77,255);
        }
        else
        {
            thisgrid[index].GetChild(0).GetComponent<Animator>();
            animBlock.SetTrigger("MergeBlock");
            thisgrid[index].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255,252,242,255);
        }

        thisgrid[index].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        matrixArray[pos.x,pos.y] = value;
        isMerged[pos.x,pos.y] = false;
    }

    public void RandomCreateBlock()
    {
        int randomBlockX = Random.Range(0,4);
        int randomBlockY = Random.Range(0,4); 
            
        if(matrixArray[randomBlockX,randomBlockY] == 0)
        {
            CreateBlock(new Vector2Int(randomBlockX,randomBlockY),2);    
        }
        else
        {
            RandomCreateBlock();
        }
    }

    IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToReachTarget)
    {
        var currentPos = transform.localPosition;
        var t = 0f;
        //isMoved = true;
        while (t < 1)
        {
            if(transform != null)
            {
                t += Time.deltaTime / timeToReachTarget;
                transform.localPosition = Vector3.Lerp(currentPos, position, t);
                yield return new WaitForSeconds(shiftspeed * Time.deltaTime);
            }else{
                break;
            }
        }
        //isMoved = false;
    }

    private string getColor(int index)
    {
        switch (index)
        {
            case 2:
                return "#eee4da";
            case 4:
                return "#ede0c8";
            case 8:
                return "#f2b179";
            case 16:
                return "#f59563";
            case 32:
                return "#f67c5f";
            case 64:
                return "#f65e3b"; 
            case 2048:
                _gameManager.GameWin();
                return "#edcf72";          
            default:
                return "#edcf72";
        }
    }

    private int[,] Copy2DArray(int[,] array)
    {
        int[,] tempArray = new int[array.GetLength(0),array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                tempArray[i,j] = array[i,j];
            }
        }
        return tempArray;
    }

    public int ShiftControl(Directions dir,int x, int y, int bdir, bool isSimulation = false) 
    {   
        int simulationMove = 0;
        
        beforetemp = Copy2DArray(matrixArray);
        switch(dir)
        {
            case Directions.right:
                for(int k=0; k < 3; k++)
                    for (int i = 0; i <= 3; i++)
                    {
                        for (int j = 3; j >= 0 ; j--)
                        {   
                            if(j == 3)
                                continue;
                            simulationMove += Shift(0,1,1,i,j,isSimulation);
                        }
                    }
            break;

            case Directions.left:
                for(int k=0; k < 3; k++)
                    for (int i = 0; i <= 3; i++)
                    {
                        for (int j = 0; j <= 3 ; j++)
                        {
                            if(j == 0)
                                continue;
                            simulationMove += Shift(0,-1,-1,i,j,isSimulation);
                        }
                    }
            break;

            case Directions.down:
                for(int k=0; k < 3; k++)
                    for (int i = 3; i >= 0; i--)
                    {
                        for (int j = 0; j <= 3 ; j++)
                        {
                            if(i == 3)
                                continue;
                            simulationMove += Shift(1,0,4,i,j,isSimulation);         
                        }
                    }
            break;

            case Directions.up:
                for(int k=0; k < 3; k++)
                   for (int i = 0; i <= 3; i++)
                    {
                        for (int j = 0; j <= 3 ; j++)
                        {
                                if(i == 0)
                                    continue;
                                simulationMove += Shift(-1,0,-4,i,j,isSimulation);         
                        }
                    }
            break;
        }

        if(simulationMove > 0 && isSimulation)
            return 1;
        else if(simulationMove == 0 && isSimulation)
            return 0;

        System.Array.Clear(isMerged, 0, isMerged.Length);

        if(movedNow)
        {
            RandomCreateBlock();
            movedNow = false;
            _gameManager.MoveCounter();
        }

        var aftertemp = Copy2DArray(matrixArray);
        #region DiffCheck
        bool isDiff = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if(aftertemp[i,j] != beforetemp[i,j])
                {
                    isDiff = true;
                    break;
                }
            }
        }
        _gameManager.UndoCheck(isDiff);
        #endregion
    
        return 1;
   }

    Coroutine shift; 
    private int Shift(int x, int y, int bdir,int i,int j, bool isSimulation = false)
    {
        if (matrixArray[i,j] == 0 || (matrixArray[i + x,j + y] !=0 && matrixArray[i,j] != matrixArray[i + x,j + y]) || isMerged[i,j] || isMerged[i + x,j + y]) //this block is empty ? || near block is full and same number || once merged 
        {
            return 0;
        }

        if(isSimulation)
            return 1;

        int index = i*4 + j;   
        if(matrixArray[i,j] > 0 && matrixArray[i,j] == matrixArray[i + x,j + y]) //if true, merge it.
        {  
            var oldt = thisgrid[index].GetChild(0).gameObject.GetComponent<RectTransform>().localPosition;
            thisgrid[index].GetChild(0).SetParent(thisgrid[index+bdir].transform);
            //grid.transform.GetChild((i*4 + j)+bdir).GetChild(0).GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(oldt, Vector3.zero, 100 * Time.deltaTime);
        
            StartCoroutine(MoveToPosition(thisgrid[index+bdir].GetChild(0).gameObject.transform,Vector3.zero, shiftspeed * Time.deltaTime));
            
            DestroyImmediate(thisgrid[index+bdir].GetChild(1).gameObject);
            DestroyImmediate(thisgrid[index+bdir].GetChild(0).gameObject); 

            matrixArray[i + x,j + y] = matrixArray[i + x,j + y] + matrixArray[i,j];
            CreateBlock(new Vector2Int(i + x,j + y),matrixArray[i + x,j + y]);  
            matrixArray[i,j] = 0;            
            _gameManager.GetScore(matrixArray[i + x,j + y]);
            movedNow = true;
            isMerged[i + x,j + y] = true;
        }
        else //just shift
        {
            var oldt = thisgrid[index].GetChild(0).gameObject.GetComponent<RectTransform>().localPosition;
            thisgrid[index].GetChild(0).SetParent(thisgrid[index+bdir].transform);
            //grid.transform.GetChild((i*4 + j)+bdir).GetChild(0).GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(oldt, Vector3.zero, 100 * Time.deltaTime);
          
            StartCoroutine(MoveToPosition(thisgrid[index+bdir].GetChild(0).gameObject.transform,Vector3.zero, shiftspeed * Time.deltaTime));
            
            matrixArray[i + x,j + y] = matrixArray[i,j];
            matrixArray[i,j] = 0;
            movedNow = true;    
        }

        return 1;
    }
}