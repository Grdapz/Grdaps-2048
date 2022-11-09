using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private Button playgameButton;
    [SerializeField] private Button highscorescreenButton;

    //[SerializeField] private GameObject Manager;
    void Start()
    {
        //DontDestroyOnLoad(Manager);
        
        playgameButton.onClick.AddListener(LoadPlayScreen);
        highscorescreenButton.onClick.AddListener(LoadHighScoreScreen);
    }

    private void LoadPlayScreen()
    {
       SceneManager.LoadScene("Game");
    }

    private void LoadHighScoreScreen()
    {
        SceneManager.LoadScene("HighScore");
    }
}
