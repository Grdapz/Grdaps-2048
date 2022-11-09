using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Highscore : MonoBehaviour
{
    [SerializeField] private Button ResetScoreButton;
    [SerializeField] private TextMeshProUGUI highscoretext;

    [SerializeField] private GameObject youSure;

    [SerializeField] private Button BackToMenu;

    void Start()
    {
        ResetScoreButton.onClick.AddListener(AreYouSure);
        highscoretext.text = PlayerPrefs.GetInt("highscoredata").ToString();

        BackToMenu.onClick.AddListener(BackMenu);
    }

    private void AreYouSure()
    {
        youSure.SetActive(true);
        youSure.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(ResetScore);
        youSure.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(Cancel);
    }

    private void ResetScore()
    {
        PlayerPrefs.SetInt("highscoredata",0);
        highscoretext.text = PlayerPrefs.GetInt("highscoredata").ToString();
        youSure.SetActive(false);
    }

    private void Cancel()
    {
        youSure.SetActive(false);
    }
    private void BackMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
