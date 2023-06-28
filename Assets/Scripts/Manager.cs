using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public GameObject Mode;
    int GameMode = 0;

    // Update is called once per frame
    void Update()
    {
        //timeScore.text = Time.timeSinceLevelLoad.ToString("00");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Map");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChoseMode()
    {
        Mode.SetActive(true);
    }

    public void ChoseEasy()
    {
        GameMode = 0;
    }

    public void ChoseNormal()
    {
        GameMode = 1;
    }

    public void ChoseHard()
    {
        GameMode = 2;
    }
}
