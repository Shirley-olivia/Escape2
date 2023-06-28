using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSUI : MonoBehaviour
{

    public TextMeshProUGUI FPS_Text;
    private float m_UpdateShowDeltaTime;
    private int m_FrameUpdate = 0;
    private float m_FPS = 0;

    private void Update()
    {
        m_FrameUpdate++;
        m_UpdateShowDeltaTime += Time.deltaTime;
        if (m_UpdateShowDeltaTime >= 0.2)
        {
            m_FPS = m_FrameUpdate / m_UpdateShowDeltaTime;
            m_UpdateShowDeltaTime = 0;
            m_FrameUpdate = 0;
            FPS_Text.text = "FPS: " + m_FPS.ToString("00.00");
        }
    }
}
