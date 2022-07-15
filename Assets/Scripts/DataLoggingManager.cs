using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
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
    public IEnumerable<HandModel> Hands;
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

    float timestamp = 0;
    bool isHandTracked = false;
    bool isLeftTracked = false;
    bool isRightTracked = false;

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

        Hands = FindObjectsOfType<HandModel>();

        //if (!FilePath.EndsWith("/"))
        //    FilePath += "/";
        Invoke("StartLogging", AutomaticLogDelay);
        //SceneManager.sceneLoaded += SceneLoaded;

    }

    //private void OnEnable()
    //{
    //    CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
    //    CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);

    //}

    //private void OnDisable()
    //{
    //    try
    //    {
    //        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
    //        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    //    }
    //    catch { }
    //}

    //void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
    //{
    //    if (eventData.Controller.ControllerHandedness.ToString() == "Left")
    //    {
    //        isLeftTracked = true;
    //    }
    //    if (eventData.Controller.ControllerHandedness.ToString() == "Right")
    //    {
    //        isRightTracked = true;
    //    }

    //}

    ///// <inheritdoc/>
    //void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
    //{
    //    try
    //    {
    //        if (eventData.Controller.ControllerHandedness.ToString() == "Left")
    //        {
    //            isLeftTracked = false;
    //        }
    //        if (eventData.Controller.ControllerHandedness.ToString() == "Right")
    //        {
    //            isRightTracked = false;
    //        }
    //    }
    //    catch { }
    //}

    //private string wristPos = "";
    //private string wristRot = "";
    //private string thumbMetacarpalJoint = "";
    //private string thumbProximalJoint = "";
    //private string thumbDistalJoint = "";
    //private string indexKnuckle = "";
    //private string indexMiddleJoint = "";
    //private string indexDistalJoint = "";
    //private string middleKnuckle = "";
    //private string middleMiddleJoint = "";
    //private string middleDistalJoint = "";
    //private string ringKnuckle = "";
    //private string ringMiddleJoint = "";
    //private string ringDistalJoint = "";
    //private string pinkyKnuckle = "";
    //private string pinkyMiddleJoint = "";
    //private string pinkyDistalJoint = "";

    //private string r_wristPos = "";
    //private string r_wristRot = "";
    //private string r_thumbMetacarpalJoint = "";
    //private string r_thumbProximalJoint = "";
    //private string r_thumbDistalJoint = "";
    //private string r_indexKnuckle = "";
    //private string r_indexMiddleJoint = "";
    //private string r_indexDistalJoint = "";
    //private string r_middleKnuckle = "";
    //private string r_middleMiddleJoint = "";
    //private string r_middleDistalJoint = "";
    //private string r_ringKnuckle = "";
    //private string r_ringMiddleJoint = "";
    //private string r_ringDistalJoint = "";
    //private string r_pinkyKnuckle = "";
    //private string r_pinkyMiddleJoint = "";
    //private string r_pinkyDistalJoint = "";

    //public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    //{
    //    Handedness handedness = eventData.Handedness;

        

    //    if (handedness.ToString() == "Left")
    //    {
    //        //Debug.Log("Left");
    //        //Debug.Log(wristPos);

    //        wristPos = eventData.InputData[TrackedHandJoint.Wrist].Position.ToString(format);
    //        wristRot = eventData.InputData[TrackedHandJoint.Wrist].Rotation.ToString(format);
    //        thumbMetacarpalJoint = eventData.InputData[TrackedHandJoint.ThumbMetacarpalJoint].Rotation.ToString(format);
    //        thumbProximalJoint = eventData.InputData[TrackedHandJoint.ThumbProximalJoint].Rotation.ToString(format);
    //        thumbDistalJoint = eventData.InputData[TrackedHandJoint.ThumbDistalJoint].Rotation.ToString(format);
    //        indexKnuckle = eventData.InputData[TrackedHandJoint.IndexKnuckle].Rotation.ToString(format);
    //        indexMiddleJoint = eventData.InputData[TrackedHandJoint.IndexMiddleJoint].Rotation.ToString(format);
    //        indexDistalJoint = eventData.InputData[TrackedHandJoint.IndexDistalJoint].Rotation.ToString(format);
    //        middleKnuckle = eventData.InputData[TrackedHandJoint.MiddleKnuckle].Rotation.ToString(format);
    //        middleMiddleJoint = eventData.InputData[TrackedHandJoint.MiddleMiddleJoint].Rotation.ToString(format);
    //        middleDistalJoint = eventData.InputData[TrackedHandJoint.MiddleDistalJoint].Rotation.ToString(format);
    //        ringKnuckle = eventData.InputData[TrackedHandJoint.RingKnuckle].Rotation.ToString(format);
    //        ringMiddleJoint = eventData.InputData[TrackedHandJoint.RingMiddleJoint].Rotation.ToString(format);
    //        ringDistalJoint = eventData.InputData[TrackedHandJoint.RingDistalJoint].Rotation.ToString(format);
    //        pinkyKnuckle = eventData.InputData[TrackedHandJoint.PinkyKnuckle].Rotation.ToString(format);
    //        pinkyMiddleJoint = eventData.InputData[TrackedHandJoint.PinkyMiddleJoint].Rotation.ToString(format);
    //        pinkyDistalJoint = eventData.InputData[TrackedHandJoint.PinkyDistalJoint].Rotation.ToString(format);
    //    }
    //    else
    //    {
    //        //Debug.Log("Right");
    //        //Debug.Log(r_wristPos);
    //        //Debug.Log("--------");
    //        r_wristPos = eventData.InputData[TrackedHandJoint.Wrist].Position.ToString(format);
    //        r_wristRot = eventData.InputData[TrackedHandJoint.Wrist].Rotation.ToString(format);
    //        r_thumbMetacarpalJoint = eventData.InputData[TrackedHandJoint.ThumbMetacarpalJoint].Rotation.ToString(format);
    //        r_thumbProximalJoint = eventData.InputData[TrackedHandJoint.ThumbProximalJoint].Rotation.ToString(format);
    //        r_thumbDistalJoint = eventData.InputData[TrackedHandJoint.ThumbDistalJoint].Rotation.ToString(format);
    //        r_indexKnuckle = eventData.InputData[TrackedHandJoint.IndexKnuckle].Rotation.ToString(format);
    //        r_indexMiddleJoint = eventData.InputData[TrackedHandJoint.IndexMiddleJoint].Rotation.ToString(format);
    //        r_indexDistalJoint = eventData.InputData[TrackedHandJoint.IndexDistalJoint].Rotation.ToString(format);
    //        r_middleKnuckle = eventData.InputData[TrackedHandJoint.MiddleKnuckle].Rotation.ToString(format);
    //        r_middleMiddleJoint = eventData.InputData[TrackedHandJoint.MiddleMiddleJoint].Rotation.ToString(format);
    //        r_middleDistalJoint = eventData.InputData[TrackedHandJoint.MiddleDistalJoint].Rotation.ToString(format);
    //        r_ringKnuckle = eventData.InputData[TrackedHandJoint.RingKnuckle].Rotation.ToString(format);
    //        r_ringMiddleJoint = eventData.InputData[TrackedHandJoint.RingMiddleJoint].Rotation.ToString(format);
    //        r_ringDistalJoint = eventData.InputData[TrackedHandJoint.RingDistalJoint].Rotation.ToString(format);
    //        r_pinkyKnuckle = eventData.InputData[TrackedHandJoint.PinkyKnuckle].Rotation.ToString(format);
    //        r_pinkyMiddleJoint = eventData.InputData[TrackedHandJoint.PinkyMiddleJoint].Rotation.ToString(format);
    //        r_pinkyDistalJoint = eventData.InputData[TrackedHandJoint.PinkyDistalJoint].Rotation.ToString(format);
    //    }

        //float currentTime = Time.unscaledTime - logStartTime;

        //Debug.Log(currentTime);

        //Debug.Log(timestamp);

        //if (currentTime == timestamp)
        //{
            //string handTrackedBool = "";
            //if (handedness.ToString() == "Left")
            //    handTrackedBool = isLeftTracked.ToString();
            //else
            //    handTrackedBool = isRightTracked.ToString();

            ////Debug.Log(handTrackedBool);

            //handsStreamWriter.WriteLine(string.Join("\t",
            //        timestamp,
            //        handedness.ToString(),
            //        handTrackedBool,
            //        eventData.InputData[TrackedHandJoint.Wrist].Position.ToString(format),
            //        eventData.InputData[TrackedHandJoint.Wrist].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.ThumbMetacarpalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.ThumbProximalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.ThumbDistalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.IndexKnuckle].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.IndexMiddleJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.IndexDistalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.MiddleKnuckle].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.MiddleMiddleJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.MiddleDistalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.RingKnuckle].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.RingMiddleJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.RingDistalJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.PinkyKnuckle].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.PinkyMiddleJoint].Rotation.ToString(format),
            //        eventData.InputData[TrackedHandJoint.PinkyDistalJoint].Rotation.ToString(format)
            //        ));
        //}
        //handsStreamWriter.Flush();
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
        timestamp = Time.unscaledTime - logStartTime;

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

        //GameObject left_hand = GameObject.Find("Joint Tracker: Left Palm");
        //if (left_hand != null)
        //{
        //    playerStreamWriter.WriteLine(string.Join("\t",
        //        timestamp,
        //        "left hand",
        //        left_hand.transform.position.ToString(format),
        //        left_hand.transform.rotation.ToString(format)
        //    ));
        //}
        //GameObject right_hand = GameObject.Find("Joint Tracker: Right Palm");
        //if (right_hand != null)
        //{
        //    playerStreamWriter.WriteLine(string.Join("\t",
        //        timestamp,
        //        "right hand",
        //        right_hand.transform.position.ToString(format),
        //        right_hand.transform.rotation.ToString(format)
        //    ));
        //}

        playerStreamWriter.Flush();

        //log hands
        //Hands = FindObjectsOfType<NetworkHand>();
        //if (Hands != null)
        //{
        //    foreach (var hand in Hands)
        //    {
        //handsStreamWriter.WriteLine(string.Join("\t",
        //    timestamp,
        //    "Left",
        //    isLeftTracked.ToString(),
        //    wristPos,
        //    wristRot,
        //    thumbMetacarpalJoint,
        //    thumbProximalJoint,
        //    thumbDistalJoint,
        //    indexKnuckle,
        //    indexMiddleJoint,
        //    indexDistalJoint,
        //    middleKnuckle,
        //    middleMiddleJoint,
        //    middleDistalJoint,
        //    ringKnuckle,
        //    ringMiddleJoint,
        //    ringDistalJoint,
        //    pinkyKnuckle,
        //    pinkyMiddleJoint,
        //    pinkyDistalJoint
        //));

        //handsStreamWriter.WriteLine(string.Join("\t",
        //            timestamp,
        //            "Right",
        //            isRightTracked.ToString(),
        //            wristPos,
        //            r_wristRot,
        //            r_thumbMetacarpalJoint,
        //            r_thumbProximalJoint,
        //            r_thumbDistalJoint,
        //            r_indexKnuckle,
        //            r_indexMiddleJoint,
        //            r_indexDistalJoint,
        //            r_middleKnuckle,
        //            r_middleMiddleJoint,
        //            r_middleDistalJoint,
        //            r_ringKnuckle,
        //            r_ringMiddleJoint,
        //            r_ringDistalJoint,
        //            r_pinkyKnuckle,
        //            r_pinkyMiddleJoint,
        //            r_pinkyDistalJoint
        //        ));
        ////    }
        ////}
        foreach (var hand in Hands)
        {
            handsStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                hand.handedness.ToString(),
                hand.isHandTracked.ToString(),
                hand.joints[TrackedHandJoint.Wrist].position.ToString(format),
                hand.joints[TrackedHandJoint.Wrist].rotation.ToString(format),
                hand.joints[TrackedHandJoint.ThumbMetacarpalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.ThumbProximalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.ThumbDistalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.IndexKnuckle].rotation.ToString(format),
                hand.joints[TrackedHandJoint.IndexMiddleJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.IndexDistalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.MiddleKnuckle].rotation.ToString(format),
                hand.joints[TrackedHandJoint.MiddleMiddleJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.MiddleDistalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.RingKnuckle].rotation.ToString(format),
                hand.joints[TrackedHandJoint.RingMiddleJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.RingDistalJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.PinkyKnuckle].rotation.ToString(format),
                hand.joints[TrackedHandJoint.PinkyMiddleJoint].rotation.ToString(format),
                hand.joints[TrackedHandJoint.PinkyDistalJoint].rotation.ToString(format)
            ));
        }
        handsStreamWriter.Flush();
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
