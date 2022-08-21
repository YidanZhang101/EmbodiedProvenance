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
using TMPro;
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
    public IEnumerable<Block> Blocks;
    public IEnumerable<Layer> TimeLabels;
    public GameObject player;
    public GameObject[] layers;

    [HideInInspector]
    public static bool IsLogging = false;

    public static StreamWriter objectsStreamWriter;
    public static StreamWriter handsStreamWriter;
    public static StreamWriter playerStreamWriter;

    public static StreamWriter blockWriter;
    public static StreamWriter labelWriter;
    public static StreamWriter layerWriter;

    public static Dictionary<float, List<string[]>> linesDic = new Dictionary<float, List<string[]>>();

    private float logStartTime;
    public static float timer = 0f;
    private const string format = "F3";
    private string scene = "";
    private string sceneNo = "";

    public static List<string> existingObjects = new List<string>();

    //private Player centralMachinePlayer;

    public bool HasRejoiner = false;

    public float timestamp = 0;
    bool isHandTracked = false;
    bool isLeftTracked = false;
    bool isRightTracked = false;

    List<GameObject> targetObjects = new List<GameObject>();

    //GameObject[] tables;
    //GameObject[] layers;
    //GameObject[] pedestals;
    //GameObject[] lines;
    //GameObject[] block_year;
    //GameObject[] block_month;
    //GameObject[] block_week;
    //GameObject[] block_day;
    //GameObject[] block_hour;

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
        player = GameObject.Find("Real Player");

        Invoke("StartLogging", AutomaticLogDelay);
        //SceneManager.sceneLoaded += SceneLoaded;

    }

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

        //Block
        path = string.Format("{0}{1}_Scene{2}_Blocks.txt", FilePath, scene, sceneNo);
        blockWriter = new StreamWriter(path, false);
        blockWriter.WriteLine("Timestamp\tObjectName\tTagName\tPosition\tRotation\tScale\tLabel");

        //TimeLabel
        path = string.Format("{0}{1}_Scene{2}_Labels.txt", FilePath, scene, sceneNo);
        labelWriter = new StreamWriter(path, false);
        labelWriter.WriteLine("Timestamp\tObjectName\tTagName\tPosition\tRotation\tScale\tLabel");

        //Layer
        path = string.Format("{0}{1}_Scene{2}_Layers.txt", FilePath, scene, sceneNo);
        layerWriter = new StreamWriter(path, false);
        layerWriter.WriteLine("Timestamp\tObjectName\tTagName\tPosition\tRotation\tScale\tLabel");

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

    private void GetDynamicObjects()
    {
        targetObjects.Clear();
        AddObjectsToList("tabletop"); // add Tabletop
        //AddObjectsToList("extractedLayer"); // add Layer
        AddObjectsToList("pedestal"); // add Pedestal
        AddObjectsToList("line"); // add Line
        // add Block & TimeLabel
        //AddObjectsToList("year");
        //AddObjectsToList("month");
        //AddObjectsToList("week");
        //AddObjectsToList("day");
        //AddObjectsToList("hour");
        Blocks = FindObjectsOfType<Block>();
        TimeLabels = FindObjectsOfType<Layer>();

        layers = GameObject.FindGameObjectsWithTag("extractedLayer");
    }

    private void LogData()
    {
        timestamp = Time.unscaledTime - logStartTime;

        GetDynamicObjects();

        //log lines
        List<string[]> linesInfo = new List<string[]>();
        GameObject[] lines = GameObject.FindGameObjectsWithTag("line");
        foreach(GameObject line in lines)
        {
            string[] lineInfo = { line.name, line.transform.position.ToString(format) };
            linesInfo.Add(lineInfo);
        }
        linesDic.Add(timestamp, linesInfo);

        //log player

        playerStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                "player",
                player.transform.position.ToString(format),
                player.transform.rotation.ToString(format)
            ));

        playerStreamWriter.Flush();

        //log hands
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

        //log objects
        List<string[]> timestampLines = new List<string[]>();
        foreach (GameObject target in targetObjects)
        {
            try
            {
                objectsStreamWriter.WriteLine(string.Join("\t",
            timestamp,
            target.name,
            target.tag,
            target.transform.position.ToString(format),
            target.transform.rotation.ToString(format),
            target.transform.localScale.ToString(format)
            ));
                //string[] objectInfo = { target.name, target.tag, target.transform.position.ToString(format), target.transform.rotation.ToString(format), target.transform.localScale.ToString(format) };
                //timestampLines.Add(objectInfo);
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogError(e.ToString());
            }
        }
        //objectsDic[timestamp] = timestampLines;
        objectsStreamWriter.Flush();

        //log blocks and labels
        foreach (var block in Blocks)
        {
            try
            {
                blockWriter.WriteLine(string.Join("\t",
                    timestamp,
                    block.name,
                    block.tag,
                    block.transform.position.ToString(format),
                    block.transform.rotation.ToString(format),
                    block.transform.localScale.ToString(format),
                    block.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text
            ));
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogError(e.ToString());
            }
        }
        blockWriter.Flush();

        foreach (var label in TimeLabels)
        {
            try
            {
                labelWriter.WriteLine(string.Join("\t",
                    timestamp,
                    label.name,
                    label.tag,
                    label.transform.position.ToString(format),
                    label.transform.rotation.ToString(format),
                    label.transform.localScale.ToString(format),
                    label.GetComponent<TextMeshPro>().text
            ));
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogError(e.ToString());
            }
        }
        labelWriter.Flush();

        foreach (var layer in layers)
        {
            try
            {
                layerWriter.WriteLine(string.Join("\t",
                    timestamp,
                    layer.name,
                    layer.tag,
                    layer.transform.position.ToString(format),
                    layer.transform.rotation.ToString(format),
                    layer.transform.localScale.ToString(format),
                    layer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text
            ));
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogError(e.ToString());
            }
        }
        layerWriter.Flush();
    }

    private void AddObjectsToList(string targetTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject target in targets)
        {
            targetObjects.Add(target);
        }
    }

    public static void StopLogging()
    {
        IsLogging = false;
        timer = 0;

        objectsStreamWriter.Close();
        handsStreamWriter.Close();
        playerStreamWriter.Close();

        blockWriter.Close();
        labelWriter.Close();
        layerWriter.Close();

        Debug.Log("Logging stopped for 06_Christmas");
    }

    public void OnApplicationQuit()
    {
        if (IsLogging)
            StopLogging();
    }
}
