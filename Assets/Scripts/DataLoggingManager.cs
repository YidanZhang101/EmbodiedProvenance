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

    [Header("Logged Objects")]
    public IEnumerable<NetworkHand> Hands;
    //public IEnumerable<Clipboard> Clipboards;
    //public IEnumerable<Card> Cards;
    //public IEnumerable<CardLine> Lines;
    //public IEnumerable<Relation> Relations;
    //public IEnumerable<PlaneContainer> Planes;

    [HideInInspector]
    public bool IsLogging = false;

    private StreamWriter objectsStreamWriter;
    private StreamWriter handsStreamWriter;
    private StreamWriter playerStreamWriter;

    private float logStartTime;
    private float timer = 0f;
    private const string format = "F3";
    private string scene = "";
    private string sceneNo = "";

    public static List<string> existingObjects = new List<string>();

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
        objectsStreamWriter = new StreamWriter(path, false);
        objectsStreamWriter.WriteLine("Timestamp\tObjectName\tTagName\tPosition\tRotation\tScale");//tParentName\

        path = string.Format("{0}{1}_Scene{2}_Player.txt", FilePath, scene, sceneNo);
        playerStreamWriter = new StreamWriter(path, false);
        playerStreamWriter.WriteLine("Timestamp\tType\tPosition\tRotation");

        // Hands
        path = string.Format("{0}{1}_Scene{2}_Hands.txt", FilePath, scene, sceneNo);
        handsStreamWriter = new StreamWriter(path, false);
        handsStreamWriter.WriteLine("Timestamp\tHandedness\tIsVisible\tj1\tj2\tj3\tj4\tj5\tj6\tj7\tj8\tj9\tj10\tj\tj12\tj13\tj14\tj15\tj16\tj17");

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

        //log objects
        AddToFile(timestamp,"tabletop"); // add Tabletop
        AddToFile(timestamp, "extractedLayer"); // add Layer
        AddToFile(timestamp, "pedestal"); // add Pedestal
        AddToFile(timestamp, "line"); // add Line
        // add Block & TimeLabel
        AddToFile(timestamp, "year");
        AddToFile(timestamp, "month");
        AddToFile(timestamp, "week");
        AddToFile(timestamp, "day");
        AddToFile(timestamp, "hour");

        objectsStreamWriter.Flush();

        //log player
        GameObject player = GameObject.Find("Real Player");
        playerStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                "player",
                player.transform.position.ToString(format),
                player.transform.rotation.ToString(format)
            ));

        GameObject left_hand = GameObject.Find("Joint Tracker: Left Palm");
        if (left_hand != null)
        {
            playerStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                "left hand",
                left_hand.transform.position.ToString(format),
                left_hand.transform.rotation.ToString(format)
            ));
        }
        GameObject right_hand = GameObject.Find("Joint Tracker: Right Palm");
        if (right_hand != null)
        {
            playerStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                "right hand",
                right_hand.transform.position.ToString(format),
                right_hand.transform.rotation.ToString(format)
            ));
        }

        playerStreamWriter.Flush();

        //log hands
        //Hands = FindObjectsOfType<NetworkHand>();
        //if (Hands != null)
        //{
        //    foreach (var hand in Hands)
        //    {
        //        handsStreamWriter.WriteLine(string.Join("\t",
        //            timestamp,
        //            hand.handedness.ToString(),
        //            hand.isHandTracked.ToString(),
        //            hand.joints[TrackedHandJoint.Wrist].position.ToString(format),
        //            hand.joints[TrackedHandJoint.Wrist].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.ThumbMetacarpalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.ThumbProximalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.ThumbDistalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.IndexKnuckle].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.IndexMiddleJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.IndexDistalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.MiddleKnuckle].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.MiddleMiddleJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.MiddleDistalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.RingKnuckle].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.RingMiddleJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.RingDistalJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.PinkyKnuckle].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.PinkyMiddleJoint].rotation.ToString(format),
        //            hand.joints[TrackedHandJoint.PinkyDistalJoint].rotation.ToString(format)
        //        ));
        //    }
        //}
        //handsStreamWriter.Flush();
    }

    public void AddToFile(float timestamp, string targetTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        bool objectExist = false;
        if (targets != null)
        {
            foreach (GameObject target in targets)
            {
                string objectName = target.name;               
                string objectPosition = target.transform.position.ToString(format);
                string objectRotation = target.transform.rotation.ToString(format);
                string objectScale = target.transform.localScale.ToString(format);
                if (existingObjects != null)
                {
                    foreach (string name in existingObjects)
                    {
                        if (name.Equals(objectName))
                        {
                            objectExist = true;
                        }
                    }
                }

                if (!objectExist)
                {
                    existingObjects.Add(objectName);
                    objectsStreamWriter.WriteLine(string.Join("\t",
                    timestamp,
                    objectName,
                    targetTag,
                    objectPosition,
                    objectRotation,
                    objectScale
                    ));
                }
            }
        }
    }

    //public static List<Transform> FindAllChildren(Transform transform)
    //{
    //    var transforms = new List<Transform>();
    //    foreach (Transform t in transform.GetComponentsInChildren<Transform>())
    //    {
    //        transforms.Add(t);
    //    }
    //    return transforms;
    //}

    public void StopLogging()
    {
        IsLogging = false;
        timer = 0;

        objectsStreamWriter.Close();
        handsStreamWriter.Close();
        playerStreamWriter.Close();

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
