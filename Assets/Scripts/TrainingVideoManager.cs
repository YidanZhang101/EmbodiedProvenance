using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TrainingVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] trainingVideos;

    public GameObject videoTitle;

    private static TrainingVideoManager _instance;
    public static TrainingVideoManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void SwitchTrainingVideo(GameObject text)
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture.Release();
        string videoName = text.GetComponent<Text>().text;

        videoTitle.GetComponent<TextMeshPro>().text = videoName;

        switch (videoName)
        {
            case "Basic interactions for exploration":
                videoPlayer.clip = trainingVideos[0];
                break;
            case "Teleportation":
                videoPlayer.clip = trainingVideos[1];
                break;
            case "Replay-It":
                videoPlayer.clip = trainingVideos[2];
                break;
            case "Note-Down-Mic":
                videoPlayer.clip = trainingVideos[3];
                break;
            case "Do-It-Lever":
                videoPlayer.clip = trainingVideos[4];
                break;
            case "Copy-It-Spatula":
                videoPlayer.clip = trainingVideos[5];
                break;
        }

        videoPlayer.targetTexture.Create();
        videoPlayer.Play();
    }
}
