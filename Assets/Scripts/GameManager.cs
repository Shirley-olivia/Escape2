using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    private GameInputAction inputAction;
    private InputAction pauseAction;
    private InputAction debugAction;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject FPSText;
    public GameObject Mode;
    public EnemyManager enemyManager;
    public GameObject flashLight;
    public AudioSource retiredAudio;
    public AudioSource winAudio;

    private float totalTime = 600.0f;
    public float remainingTime;
    public TextMeshProUGUI currentTime;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        enemyManager= GetComponent<EnemyManager>();
        ChangeScene();
        Application.targetFrameRate = 60;
        remainingTime = totalTime;
        inputAction = new GameInputAction();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        inputAction.Enable();
        pauseAction = inputAction.Game.Pause;
        pauseAction.performed += GamePause;
        debugAction = inputAction.Game.Debug;
        debugAction.performed += GameDebug;
    }

    void Update()
    {
        currentTime.text = remainingTime.ToString("000.00");


        if (remainingTime > 0.0f)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            GameOver(false);
        }
        //timeScore.text = Time.timeSinceLevelLoad.ToString("00");
        //Debug.Log(Static.GameMode);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Start");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public static void GameOver(bool win)
    {
        if (win && !instance.winPanel.activeSelf)
        {
            instance.winAudio.Play();
            instance.winPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if (!win && !instance.gameOverPanel.activeSelf)
        {
            instance.retiredAudio.Play();
            instance.gameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Map");
        Time.timeScale = 1;
    }

    public void ChoseMode()
    {
        Mode.SetActive(true);
    }

    public void ChoseEasy()
    {
        Static.GameMode = 0;
        StartGame();
    }

    public void ChoseNormal()
    {
        Static.GameMode = 1;
        StartGame();
    }

    public void ChoseHard()
    {
        Static.GameMode = 2;
        StartGame();
    }

    public void ChangeScene()
    {
        if (enemyManager == null)
        {
            return ;
        }
        if(Static.GameMode == 0)
        {
            totalTime = 300;
            RenderSettings.ambientIntensity = 1.0f;
            RenderSettings.reflectionIntensity = 1.0f;
            flashLight.SetActive(false);
        }
        else if (Static.GameMode == 1)
        {
            totalTime = 600;
            enemyManager.generate(2);
            RenderSettings.ambientIntensity = 1.0f;
            RenderSettings.reflectionIntensity = 1.0f;
            flashLight.SetActive(false);
        }
        else if (Static.GameMode == 2)
        {
            totalTime = 360;
            enemyManager.generate(4);
            RenderSettings.ambientIntensity = 0.4f;
            RenderSettings.reflectionIntensity = 0.1f;
            flashLight.SetActive(true);
        }
    }

    void GamePause(InputAction.CallbackContext context)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void GameDebug(InputAction.CallbackContext context)
    {
        if (FPSText != null)
        {
            FPSText.SetActive(!FPSText.activeInHierarchy);
        }
    }
}
