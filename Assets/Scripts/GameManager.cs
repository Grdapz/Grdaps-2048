using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] Game _game;

    [Header("GameObjects")]
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject UI;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI textmainScore;
    [SerializeField] private TextMeshProUGUI texthighScore;
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI textmoveCounter;
    [SerializeField] private TextMeshProUGUI textGameover;
    [SerializeField] private TextMeshProUGUI textanimsScore;
    
    [Header("Buttons")]
    [SerializeField] private Button newgameButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button playagainButton;
    [SerializeField] private Button challangeButton;

    [Header("Animations")]
    [SerializeField] private Animator animScore;
    
    private bool isPaused = false;
    private bool timerIsRunning = false;
    private bool gameStarted = false;
    private bool usedUndo = false;
    private int[,] tempmatrixArray = new int[4,4];
    private int highScore;
    private int score = 0;
    private int minutes;
    private int seconds;
    private int movecounter = 0;
    public int tempscore;
    private float timer = 0;
    public Transform[] thisgrid = new Transform[16];
    
    IEnumerator Start()
    {   
        for (int i = 0; i < grid.transform.childCount; i++)
        {
           thisgrid[i] = grid.transform.GetChild(i);
        }
  
        yield return new WaitUntil(()=> _game.isReady);

        if(File.Exists(Path.Combine(Application.persistentDataPath,"veri.json")))
        {
            gameStarted = true;
            LoadSave();
        }      
        else
        {
            textmainScore.text = "0";
            _game.RandomCreateBlock();
            _game.RandomCreateBlock();
            usedUndo = true;
            gameStarted = true;
        }

        if(!PlayerPrefs.HasKey("highscoredata"))
            PlayerPrefs.SetInt("highscoredata",score);
        else
           highScore = PlayerPrefs.GetInt("highscoredata");

        timerIsRunning = true;
        texthighScore.text = highScore.ToString();

        newgameButton.onClick.AddListener(NewGame);
        playagainButton.onClick.AddListener(NewGame);
        undoButton.onClick.AddListener(Undo);
        menuButton.onClick.AddListener(BackToMenu);
        challangeButton.onClick.AddListener(Challenge);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timer += Time.deltaTime;
            minutes = Mathf.FloorToInt(timer / 60F);
            seconds = Mathf.FloorToInt(timer - minutes * 60);
            textTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    private void LoadSave()
    {
        string readjson = File.ReadAllText(Path.Combine( Application.persistentDataPath, "veri.json" ));
        tempmatrixArray = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>(readjson);

        for (int i = 0; i < tempmatrixArray.GetLength(0); i++)
            {
                for (int j = 0; j < tempmatrixArray.GetLength(1); j++)
                {
                    if(tempmatrixArray[i,j] > 0)
                    {
                        _game.CreateBlock(new Vector2Int(i,j), tempmatrixArray[i,j]);
                        tempmatrixArray[i,j] = 0;
                    }
                }
            }

            if(PlayerPrefs.GetInt("undoused") == 0)
            {
                usedUndo = false;
                System.Array.Clear(tempmatrixArray,0,tempmatrixArray.Length);

                if(File.Exists(Path.Combine(Application.persistentDataPath,"undo.json")))
                    {
                        string readjson2 = File.ReadAllText(Path.Combine( Application.persistentDataPath, "undo.json" ));
                        tempmatrixArray = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>(readjson2);
                    }
            }
            else if(PlayerPrefs.GetInt("undoused") == 1)
            {
                usedUndo = true;
            }

            tempscore = PlayerPrefs.GetInt("undotempscore");
        
        //matrixArray = Copy2DArray(tempmatrixArray);     
            
        score = PlayerPrefs.GetInt("tempscore");
        movecounter = PlayerPrefs.GetInt("tempmoves");
        timer = PlayerPrefs.GetFloat("temptimer");
        textmainScore.text = score.ToString();
        textmoveCounter.text = movecounter.ToString() + " moves";
        textTimer.text = timer.ToString();
    }

    private void NewGame()
    {
        gameWinPanel.SetActive(false);
        gameoverPanel.SetActive(false);
        UI.SetActive(true);
    
        System.Array.Clear(_game.matrixArray,0,_game.matrixArray.Length);
        System.Array.Clear(tempmatrixArray,0,tempmatrixArray.Length);

        for (int i = 0; i < thisgrid.Length; i++)
        {
            if(thisgrid[i].childCount > 0)
                DestroyImmediate(thisgrid[i].GetChild(0).gameObject);
        }

        score = 0;
        textmainScore.text = score.ToString();
        movecounter = 0;
        textmoveCounter.text = movecounter.ToString() + " moves";
        timer = 0;
        textTimer.text = timer.ToString();

        _game.RandomCreateBlock();
        _game.RandomCreateBlock();

        usedUndo = true;
        timerIsRunning = true;
    }

    private void Undo()
    {
        //int[,] thismatrixArray = new int[4,4];
        //thismatrixArray =_game.matrixArray;
        if(!usedUndo)
        {
            usedUndo = true;
            movecounter -= 1;
            textmoveCounter.text = movecounter.ToString() + " moves";;

            for (int i = 0; i < thisgrid.Length; i++)
            {
                if(thisgrid[i].childCount > 0)
                    DestroyImmediate(thisgrid[i].GetChild(0).gameObject);
            }

            System.Array.Clear(_game.matrixArray,0,_game.matrixArray.Length);
            //_game.matrixArray = thismatrixArray;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if(tempmatrixArray[i,j] > 0)
                    {
                        _game.CreateBlock(new Vector2Int(i,j), tempmatrixArray[i,j]);
                        tempmatrixArray[i,j] = 0;
                    }
                }
            }
        }
        score = tempscore;
        textmainScore.text = score.ToString();
    }

    private void UndoSave(int[,] temp)
    { 
        GetSave(temp);
    }

    private void GetSave(int[,] temp)
    {
        for (int i = 0; i < temp.GetLength(0); i++)
        {
            for(int j = 0; j < temp.GetLength(1); j++)
            {
                tempmatrixArray[i,j] = 0;
                if(temp[i,j] > 0)
                    tempmatrixArray[i,j] = temp[i,j];                 
            } 
        }
        usedUndo = false;
    }
    
    private void MatrixSavetoJson()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(tempmatrixArray);
        string jsonKonum = Path.Combine( Application.persistentDataPath, "veri.json" );
        File.WriteAllText( jsonKonum, json );   
    }

    private void UndoSavetoJson()
    {
        PlayerPrefs.SetInt("undotempscore",tempscore);
        string jsonundo = Newtonsoft.Json.JsonConvert.SerializeObject(tempmatrixArray);
        string jsonKonumundo = Path.Combine( Application.persistentDataPath, "undo.json");
        File.WriteAllText( jsonKonumundo, jsonundo );

        if(usedUndo)
            PlayerPrefs.SetInt("undoused",1);
        else
            PlayerPrefs.SetInt("undoused",0);    
    }

    private void BackToMenu()
    {
        timerIsRunning = false;
        PlayerPrefs.SetFloat("temptimer",timer);
        PlayerPrefs.SetInt("tempscore",score);
        PlayerPrefs.SetInt("tempmoves",movecounter);
        UndoSavetoJson();
        GetSave(_game.matrixArray);
        MatrixSavetoJson();

        SceneManager.LoadScene("Menu");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
        timerIsRunning = true;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
        if(gameStarted)
        {
            timerIsRunning = false;
            PlayerPrefs.SetFloat("temptimer",timer);
            PlayerPrefs.SetInt("tempscore",score);
            PlayerPrefs.SetInt("tempmoves",movecounter);
            UndoSavetoJson();
            GetSave(_game.matrixArray);
            MatrixSavetoJson();
        }
    }
    
    public void UndoCheck(bool isDiff)
    {
        if(!isDiff)
        {
            int[,] test = new int[4,4];
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    test[i,j] = tempmatrixArray[i,j];                 
                }
            }   
            UndoSave(test);
            //Debug.Log("farklılık yok");
        }
        else
        {
            tempscore = score;
            UndoSave(_game.beforetemp);
        }
    }

    public void MoveCounter()
    {
        movecounter += 1;
        textmoveCounter.text = movecounter.ToString() + " moves";
    }

    public void GetScore(int matrisvalue)
    {
        textanimsScore.text = "+" + matrisvalue;
        animScore.SetTrigger("AddScore");
        score = score + matrisvalue;
        textmainScore.text = score.ToString(); 
                    
        if(score > highScore)
        {
            highScore = score;
            texthighScore.text = highScore.ToString();
            PlayerPrefs.SetInt("highscoredata",highScore);
        }
    }

    public void GameWin()
    {
        timerIsRunning = false;
        gameWinPanel.SetActive(true);
    }

    private void Challenge()
    {
        gameWinPanel.SetActive(false);
        timerIsRunning = false;
    }

    public void GameLoseCheck()
    {
        int trycounter = 0;

        trycounter += _game.ShiftControl(Directions.right,0,1,1,true);
        trycounter += _game.ShiftControl(Directions.left,0,-1,-1,true);
        trycounter += _game.ShiftControl(Directions.down,1,0,4,true);
        trycounter += _game.ShiftControl(Directions.up,-1,0,-4,true);

        if(trycounter == 0)
            GameOver();
            
    }

    private void GameOver()
    {
        timerIsRunning = false;
        string templatetime = string.Format("{0:0}:{1:00}", minutes, seconds);
        textGameover.text = "You earned " + textmainScore.text + " points with " + movecounter + " moves in " + templatetime;

        UI.SetActive(false);
        gameoverPanel.SetActive(true);

        // System.Array.Clear(tempmatrixArray,0,tempmatrixArray.Length);
        // File.Delete(Path.Combine( Application.persistentDataPath, "undo.json"));
        // File.Delete(Path.Combine( Application.persistentDataPath, "veri.json"));
    }  
}
