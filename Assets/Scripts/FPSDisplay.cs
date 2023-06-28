using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    private TMP_Text text;
    public float interval = 0.1f;
    private float lastUpdate;
    private int cnt;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        lastUpdate = Time.time;
        cnt = 0;
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        cnt++;
        if (Time.time - lastUpdate >= interval)
        {
            text.SetText("FPS: " + (cnt / (Time.time - lastUpdate)).ToString("F2"));
            lastUpdate = Time.time;
            cnt = 0;
        }
    }
}
