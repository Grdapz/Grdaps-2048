using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentLoaded;
    void Start()
    {
        Application.targetFrameRate = 60;

        StartCoroutine(LoadMainPageScene());
        percentLoaded.text = "0%";
        progressBar.value = 0.004f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadMainPageScene()
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Menu");
        while(!loadingOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.3f);
            percentLoaded.text = Mathf.Round(progressValue * 100) + "%";
            progressBar.value = Mathf.Clamp01(loadingOperation.progress / 0.3f);
            yield return null;
        }
    }
}
