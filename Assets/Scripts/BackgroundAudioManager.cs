using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudioManager : MonoBehaviour
{
    public AudioSource AudioSource;

    public AudioClip[] BackgroundAudios;

    private static BackgroundAudioManager _instance;
    public static BackgroundAudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void SwitchBackgroundSound(string sceneName)
    {
        AudioSource.Stop();

        switch (sceneName)
        {
            case "01_Wild":
                AudioSource.clip = BackgroundAudios[0];
                break;
            case "02_Desert":
                AudioSource.clip = BackgroundAudios[1];
                break;
            case "03_Petrol_pump":
                AudioSource.clip = BackgroundAudios[2];
                break;
            case "04_island":
                AudioSource.clip = BackgroundAudios[3];
                break;
            case "05_Glacier":
                AudioSource.clip = BackgroundAudios[4];
                break;
            case "06_Christmas":
                AudioSource.clip = BackgroundAudios[5];
                break;
        }

        AudioSource.Play();
    }
}
