using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    public AudioSource DoorOpenAudio;
    // Start is called before the first frame update
    void Start()
    {
        DoorOpenAudio.Play();
        DoorOpenAudio.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio()
    {
        DoorOpenAudio.UnPause();
        // DoorOpenAudio.Play();
    }

    public void StopAudio()
    {
        DoorOpenAudio.Pause();
    }
}
