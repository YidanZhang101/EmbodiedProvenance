using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLoggingManager : MonoBehaviour
{
    [Header("Logging Settings")]
    public string FilePath;
    //public string GroupID;
    public float TimeBetweenLogs = 0.05f;
    public float AutomaticLogDelay = 0.25f;
    public string TestMessage;

    //[Header("Logged Objects")]
    //public IEnumerable<PhotonAvatar> Players;
    //public IEnumerable<DesktopAvatar> DesktopPlayers;
    //public IEnumerable<NetworkHand> Hands;
    //public IEnumerable<Clipboard> Clipboards;
    //public IEnumerable<Card> Cards;
    //public IEnumerable<CardLine> Lines;
    //public IEnumerable<Relation> Relations;
    //public IEnumerable<PlaneContainer> Planes;

    [HideInInspector]
    public bool IsLogging = false;

    private StreamWriter objectsStreamWriter;

    private float logStartTime;
    private float timer = 0f;
    private const string format = "F3";
    private string scene = "";
    private string sceneNo = "";

    //private Player centralMachinePlayer;

    public bool HasRejoiner = false;

    private static DataLoggingManager _instance;
    public static DataLoggingManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        //if (!FilePath.EndsWith("/"))
        //    FilePath += "/";
        Invoke("StartLogging", AutomaticLogDelay);
        //SceneManager.sceneLoaded += SceneLoaded;

    }

    //private void SceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name == "06_Christmas")// && NetworkManager.Instance.IsCentralMachine)
    //    {
    //        Invoke("StartLogging", AutomaticLogDelay);
    //    }
    //}

    public void StartLogging()
    {
        //GetStaticReferences();
        TestMessage = "StartLogging";
        scene = "06_Christmas";
        sceneNo = "1";
        //Tabletops (include everything inside)
        string path = string.Format("{0}{1}_Scene{2}_Objects.txt", FilePath, scene, sceneNo);
        objectsStreamWriter = new StreamWriter(path, true);
        objectsStreamWriter.WriteLine("Timestamp\tObjectName\tPosition\tRotation\tScale");//tParentName\

        IsLogging = true;
        logStartTime = Time.unscaledTime;

        TestMessage = "Logging started for 06_Christmas";

    }

    public void LateUpdate()
    {
        if (IsLogging)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= TimeBetweenLogs)
            {
                timer = 0;
                LogData();
            }
        }
    }

    private void LogData()
    {
        float timestamp = Time.unscaledTime - logStartTime;
        Transform parentObject = GameObject.Find("Tabletop").transform;
        foreach (Transform child in FindAllChildren(parentObject))
        {
            objectsStreamWriter.WriteLine(string.Join("\t",
                    timestamp,
                    child.gameObject.name,
                    //child.parent.name,
                    child.position.ToString(format),
                    child.rotation.ToString(format),
                    child.localScale.ToString(format)
                    ));
        }
        objectsStreamWriter.Flush();
    }

    public static List<Transform> FindAllChildren(Transform transform)
    {
        var transforms = new List<Transform>();
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            transforms.Add(t);
        }
        return transforms;
        //var transforms = new List<Transform>();
        //var queue = new Queue<Transform>();

        //queue.Enqueue(transform);

        //while (queue.Count > 0)
        //{
        //    var t = queue.Dequeue();
        //    if (transform.childCount > 0)
        //    {
        //        foreach (Transform child in t.GetComponentsInChildren<Transform>())
        //        {
        //            queue.Enqueue(child);
        //        }
        //    }
        //    else
        //    {
        //        transforms.Add(t);
        //    }
        //}
        //return transforms;
    }

    public void StopLogging()
    {
        IsLogging = false;
        timer = 0;

        objectsStreamWriter.Close();

        TestMessage = "Logging stopped for 06_Christmas";
    }

    public void OnApplicationQuit()
    {
        if (IsLogging)
            StopLogging();
    }

    ///// <summary>
    ///// Populates the tracked object arrays for static references (i.e., gameobjects that do not get instantiated/destroyed at runtime)
    ///// </summary>
    //private void GetStaticReferences()
    //{
    //    Players = FindObjectsOfType<PhotonAvatar>();
    //    DesktopPlayers = FindObjectsOfType<DesktopAvatar>();
    //    Clipboards = FindObjectsOfType<Clipboard>();
    //    Hands = FindObjectsOfType<NetworkHand>();

    //    // If any of the Players don't have properly initialised references to their transforms, we force them to update here
    //    if (condition == "VR")
    //    {
    //        foreach (var player in Players)
    //        {
    //            if (player.ChestTransform == null || player.HeadTransform == null)
    //            {
    //                player.ReinitialiseAvatarTransforms();
    //            }
    //        }
    //    }
    //}
}
