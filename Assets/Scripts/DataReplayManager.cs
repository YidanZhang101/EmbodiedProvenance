using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class DataReplayManager : MonoBehaviour
{
    //public string FilePath;

    [Header("Data Files")]
    public TextAsset SessionInfoData;
    public TextAsset PlayerData;
    public TextAsset HandData;
    public TextAsset ObjectData;
    public TextAsset BlockData;
    public TextAsset LabelData;
    public TextAsset LayerData;
    public TextAsset AnnotationData;
    //public TextAsset TableData;

    [Header("Static Replay Objects")]
    public GameObject Player;
    public GameObject[] Hands;

    [HideInInspector] public List<string> targetObjects = new List<string>();
    [HideInInspector] public List<string> targetBlocks = new List<string>();
    [HideInInspector] public List<string> targetLabels = new List<string>();
    [HideInInspector] public List<string> targetLayers = new List<string>();

    [Range(0, 1)] public static float TimeScrubber;
    [Range(0, 10)] public float SpeedMultiplier = 1;
    public float CurrentTime = 0;
    public float CurrentTimestamp = 0;

    //private Dictionary<int, int> playerIDToRoleIDDict;

    //public bool StartReplay = false;

    public bool isLiveReplayRunning = false;
    private bool isLiveReplayPaused = false;
    public static bool isLiveReplayStopped = false;

    private float prevTimeScrubber;
    private float prevSpeedMultiplier;
    //private int actionsIdx;

    private System.Diagnostics.Stopwatch stopwatch;

    private static DataReplayManager _instance;
    public static DataReplayManager Instance { get { return _instance; } }

    Dictionary<float, List<string[]>> playerDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> objectDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> tableDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> handData = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> blockDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> labelDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> layerDic = new Dictionary<float, List<string[]>>();

    private Dictionary<float, List<string[]>> annotationData = new Dictionary<float, List<string[]>>();
    private Dictionary<int, AnnotationManager> Annotations;

    public float selectedTime;
    float logStartTime;
    //string[] playerDataLines;
    GameObject theObject;
    GameObject theBlock;
    GameObject theLabel;
    GameObject theLayer;

    float pausedTime = 0;

    bool logStopped = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        logStartTime = Time.unscaledTime;
        //playerDataLines = PlayerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //AddDataToDic();
        //SetTransparency();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopDataLog()
    {
        if (DataLoggingManager.IsLogging)
        {
            DataLoggingManager.StopLogging();
            logStopped = true;
            Debug.Log("logStopped");
        }
    }

    public void StartLiveReplay()
    {
        //StopDataLog();
        //if (logStopped)
        //{
        //    AddDataToDic();
        //    if (!isLiveReplayRunning)
        //    {
        //        isLiveReplayPaused = false;
        //        StartCoroutine("Replay");
        //    }
        //    else if (isLiveReplayPaused)
        //    {
        //        isLiveReplayPaused = false;
        //    }
        //}
        AddDataToDic();
        if (!isLiveReplayRunning)
        {
            isLiveReplayPaused = false;
            StartCoroutine("Replay");
        }
        else if (isLiveReplayPaused)
        {
            isLiveReplayPaused = false;
        }
    }

    public IEnumerator Replay()
    {
        isLiveReplayRunning = true;

        Transform allElements = GameObject.Find("AllElements").transform;
        Renderer[] _renderer = allElements.GetComponentsInChildren<Renderer>();
        foreach (Renderer childRenderer in _renderer)
        {
            if (childRenderer.gameObject.GetComponent<Interactable>() == null | childRenderer.gameObject.tag == "timelabelCube")
                childRenderer.enabled = false;
        }

        GameObject[] lines = GameObject.FindGameObjectsWithTag("line");
        foreach (GameObject line in lines)
            line.transform.position = new Vector3(line.transform.position.x, line.transform.position.y - 20f, line.transform.position.z);

        GameObject[] links = GameObject.FindGameObjectsWithTag("tableLink");
        foreach (GameObject link in links)
            link.transform.position = new Vector3(link.transform.position.x, link.transform.position.y - 20f, link.transform.position.z);

        Annotations = new Dictionary<int, AnnotationManager>();

        float[] timestamps = playerDic.Keys.Distinct().OrderBy(x => x).ToArray();
        //float[] timestamps = playerDic.Keys.Distinct().OrderBy(x => x).ToArray();
        float totalTime = timestamps.Max();

        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < timestamps.Length; i++)
        {
            // If the time scrubber has been manually changed
            if (TimeScrubber != prevTimeScrubber)
            {
                // Find the nearest timestamp that matches the new time
                float newTime = TimeScrubber * totalTime;
                float newTimestamp = timestamps.Aggregate((x, y) => Math.Abs(x - newTime) < Math.Abs(y - newTime) ? x : y);
                // Set this timestamp as our new video time, and set i as its index
                i = Array.IndexOf(timestamps, newTimestamp);

                //// Find the new actionsIdx
                //for (int j = 0; j < actionData.Keys.Count; j++)
                //{
                //    actionsIdx = actionData.Count - 1;
                //    if (actionData.Keys.ElementAt(j) > newTimestamp)
                //    {
                //        actionsIdx = j;
                //        break;
                //    }
                //}

                stopwatch.Restart();
                prevTimeScrubber = TimeScrubber;
            }

            // If the live replay is paused, skip this frame
            else if (isLiveReplayPaused)
            {
                i--;
                pausedTime = TimeScrubber;
                stopwatch.Restart();
                yield return null;
                continue;
            }

            // The current real time is the previous timestamp plus the time that has passed since then
            CurrentTime = timestamps[Mathf.Max(0, i - 1)] + ((stopwatch.ElapsedMilliseconds / 1000f) * SpeedMultiplier);

            // If the speed multiplier has changed since the previous frame, update the pitch of the audio sources
            if (prevSpeedMultiplier != SpeedMultiplier)
            {
                prevSpeedMultiplier = SpeedMultiplier;
            }

            // If not enough time has elapsed between the two timestamps, skip this frame
            //if ((stopwatch.ElapsedMilliseconds / 1000f) * SpeedMultiplier < timestampDelta)
            if (CurrentTime < timestamps[i])
            {
                i--;
                yield return null;
                continue;
            }

            stopwatch.Restart();
            CurrentTimestamp = timestamps[i];
            TimeScrubber = CurrentTimestamp / totalTime;
            prevTimeScrubber = TimeScrubber;

            // Set player
            foreach (string[] values in playerDic[CurrentTimestamp])
            {
                string type = values[1];
                if(type == "player")
                {
                    Transform headTransform = Player.transform;
                    headTransform.position = StringToVector3(values[2]);
                    headTransform.rotation = StringToQuaternion(values[3]);
                }
            }

            //Set hands
            foreach (string[] values in handData[CurrentTimestamp])
            {
                GameObject handGo;
                if (values[1] == "Left")
                    handGo = Hands[0];
                else
                    handGo = Hands[1];

                HandModel hand = handGo.GetComponent<HandModel>();
                

                hand.handRenderer.enabled = bool.Parse(values[2]);
                // Set joint positions
                hand.joints[TrackedHandJoint.Wrist].position = StringToVector3(values[3]);
                hand.joints[TrackedHandJoint.Wrist].rotation = StringToQuaternion(values[4]);
                hand.joints[TrackedHandJoint.ThumbMetacarpalJoint].rotation = StringToQuaternion(values[5]);
                hand.joints[TrackedHandJoint.ThumbProximalJoint].rotation = StringToQuaternion(values[6]);
                hand.joints[TrackedHandJoint.ThumbDistalJoint].rotation = StringToQuaternion(values[7]);
                hand.joints[TrackedHandJoint.IndexKnuckle].rotation = StringToQuaternion(values[8]);
                hand.joints[TrackedHandJoint.IndexMiddleJoint].rotation = StringToQuaternion(values[9]);
                hand.joints[TrackedHandJoint.IndexDistalJoint].rotation = StringToQuaternion(values[10]);
                hand.joints[TrackedHandJoint.MiddleKnuckle].rotation = StringToQuaternion(values[11]);
                hand.joints[TrackedHandJoint.MiddleMiddleJoint].rotation = StringToQuaternion(values[12]);
                hand.joints[TrackedHandJoint.MiddleDistalJoint].rotation = StringToQuaternion(values[13]);
                hand.joints[TrackedHandJoint.RingKnuckle].rotation = StringToQuaternion(values[14]);
                hand.joints[TrackedHandJoint.RingMiddleJoint].rotation = StringToQuaternion(values[15]);
                hand.joints[TrackedHandJoint.RingDistalJoint].rotation = StringToQuaternion(values[16]);
                hand.joints[TrackedHandJoint.PinkyKnuckle].rotation = StringToQuaternion(values[17]);
                hand.joints[TrackedHandJoint.PinkyMiddleJoint].rotation = StringToQuaternion(values[18]);
                hand.joints[TrackedHandJoint.PinkyDistalJoint].rotation = StringToQuaternion(values[19]);
            }

            //targetObjects = new List<GameObject>();

            // Set tables
            if (tableDic.ContainsKey(CurrentTimestamp))
            {
                //Debug.Log(CurrentTimestamp);
                foreach (string[] values in tableDic[CurrentTimestamp])
                {
                    string objectName = values[1];
                    if (!targetObjects.Contains(objectName))
                    {
                        SetObjectWithTag("Tabletop", values);
                        targetObjects.Add(theObject.name);
                    }
                }
            }

            // Set objects
            List<GameObject> visitedObjects = new List<GameObject>();
            if (objectDic.ContainsKey(CurrentTimestamp))
            {
                //Debug.Log(CurrentTimestamp);
                foreach (string[] values in objectDic[CurrentTimestamp])
                {                    
                    //ReplayObjects("Tabletop", values);
                    ReplayObjects("Pedestal", values);
                    ReplayObjects("Line", values);
                    ReplayObjects("TableLink", values);
                }
            }
            //Destroy all unused objects
            //string[] objectsToRemove = targetObjects.Where(x => !visitedObjects.Contains(GameObject.Find(x))).ToArray();
            //foreach (string item in objectsToRemove)
            //{
            //    Debug.Log("objectsToRemove: " + item);
            //}
            //foreach (string item in objectsToRemove)
            //{
            //    Destroy(GameObject.Find(item));
            //    targetObjects.Remove(item);
            //}

            // Set blocks
            List<string> visitedBlocks = new List<string>();
            if (blockDic.ContainsKey(CurrentTimestamp))
            {
                //Debug.Log(CurrentTimestamp);
                foreach (string[] values in blockDic[CurrentTimestamp])
                {
                    string objectName = values[1];
                    if (objectName.StartsWith("Block"))
                    {
                        GameObject block = GameObject.Find(objectName);
                        block.GetComponent<Renderer>().enabled = true;
                        block.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;

                        block.transform.position = StringToVector3(values[3]);
                        block.transform.rotation = StringToQuaternion(values[4]);
                        block.transform.localScale = StringToVector3(values[5]);
                    }
                    //if (!targetBlocks.Contains(objectName))
                    //{
                    //    SetBlockWithTag("Block_LTB", values);
                    //    SetBlockWithTag("Block_Monash_Sport", values);
                    //    SetBlockWithTag("Block_Theatre", values);
                    //    SetBlockWithTag("Block_Campus_Centre", values);
                    //    SetBlockWithTag("Block_New_Horizons", values);
                    //    SetObjectWithTag("TimeLabel", values);

                    //    targetBlocks.Add(theBlock.name);
                    //    Debug.Log("Create " + theBlock.name);
                    //}
                    ////if (!visitedBlocks.Contains(theBlock))
                    //visitedBlocks.Add(theBlock.name);
                    //Debug.Log("visitedBlocks: " + theBlock.name);

                    //if (theBlock.name.Contains("Clone"))
                    //{
                    //    int endExcluded = theBlock.name.Length - 7;
                    //    string blockName = theBlock.name.Substring(0, endExcluded);
                    //    GameObject block = GameObject.Find(blockName);
                    //    block.transform.position = StringToVector3(values[3]);
                    //    block.transform.rotation = StringToQuaternion(values[4]);
                    //    block.transform.localScale = StringToVector3(values[5]);
                    //}

                }
            }
            //Destroy all unused blocks
            //string[] blocksToRemove = targetBlocks.Where(x => !visitedBlocks.Contains(x)).ToArray();
            //foreach (string item in blocksToRemove)
            //{
            //    Debug.Log("blocksToRemove: " + item);
            //}
            //foreach (string item in blocksToRemove)
            //{
            //    Destroy(GameObject.Find(item));
            //    targetBlocks.Remove(item);
            //}
            //GameObject ltb = GameObject.Find("Block_LTB (1)");
            //Destroy(ltb);
            //targetBlocks.Remove("Block_LTB (1)");

            //Set label
            List<GameObject> visitedLabels = new List<GameObject>();
            if (labelDic.ContainsKey(CurrentTimestamp))
            {
                foreach (string[] values in labelDic[CurrentTimestamp])
                {
                    string objectName = values[1];
                    GameObject label = GameObject.Find(objectName);
                    if (label != null)
                    {
                        label.GetComponent<Renderer>().enabled = true;
                        label.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true; // enable render for the cube
                        //Renderer[] label_renderer = label.GetComponentsInChildren<Renderer>();
                        //foreach (Renderer childRenderer in label_renderer)
                        //    childRenderer.enabled = true;
                        label.transform.position = StringToVector3(values[3]);
                        label.transform.rotation = StringToQuaternion(values[4]);
                        label.transform.localScale = StringToVector3(values[5]);
                    }
                    //if (!targetLabels.Contains(objectName))
                    //{
                    //    SetLabelWithTag("TimeLabel", values);

                    //    targetLabels.Add(theLabel.name);
                    //    //Debug.Log("Create " + theLabel.name);
                    //}
                    //visitedLabels.Add(theLabel);

                    //Debug.Log("visitedLabels: " + theLabel.name);
                    //theLabel.transform.position = StringToVector3(values[3]);
                    //theLabel.transform.rotation = StringToQuaternion(values[4]);
                    //theLabel.transform.localScale = StringToVector3(values[5]);
                }
            }
            //Destroy all unused labels
            //string[] labelsToRemove = targetLabels.Where(x => !visitedLabels.Contains(GameObject.Find(x))).ToArray();
            ////foreach (string item in labelsToRemove)
            ////{
            ////    Debug.Log("labelsToRemove: " + item);
            ////}
            //foreach (string item in labelsToRemove)
            //{
            //    Destroy(GameObject.Find(item));
            //    targetLabels.Remove(item);
            //}

            ////Set layer
            List<GameObject> visitedLayers = new List<GameObject>();
            if (layerDic.ContainsKey(CurrentTimestamp))
            {
                foreach (string[] values in layerDic[CurrentTimestamp])
                {
                    SetLayerWithTag("Layer", values);
                    //string objectName = values[1];
                    //if (!targetLayers.Contains(objectName))
                    //{
                    //    SetLayerWithTag("Layer", values);

                    //    targetLayers.Add(theLayer.name);
                    //    //Debug.Log("Create " + theLabel.name);
                    //}
                    //visitedLayers.Add(theLayer);
                    ////Debug.Log("visitedLabels: " + theLabel.name);
                    //theLayer.transform.position = StringToVector3(values[3]);
                    //theLayer.transform.rotation = StringToQuaternion(values[4]);
                    //theLayer.transform.localScale = StringToVector3(values[5]);
                }
            }
            ////Destroy all unused labels
            //string[] layersToRemove = targetLayers.Where(x => !visitedLayers.Contains(GameObject.Find(x))).ToArray();
            ////foreach (string item in labelsToRemove)
            ////{
            ////    Debug.Log("labelsToRemove: " + item);
            ////}
            //foreach (string item in layersToRemove)
            //{
            //    Destroy(GameObject.Find(item));
            //    targetLayers.Remove(item);
            //}

            // Set annotations
            if (annotationData.ContainsKey(CurrentTimestamp))
            {
                foreach (string[] values in annotationData[CurrentTimestamp])
                {
                    string annotationName = values[1];
                    GameObject annotationObject = GameObject.Find(annotationName);
                    AnnotationManager annotation = annotationObject.GetComponent<AnnotationManager>();
                    Renderer[] object_renderer = annotationObject.transform.GetComponentsInChildren<Renderer>();
                    foreach (Renderer childRenderer in object_renderer)
                        childRenderer.enabled = true;
                    annotation.SetTitle(values[2]);
                    annotation.SetContent(values[3]);
                    annotation.transform.position = StringToVector3(values[4]);
                    annotation.transform.rotation = StringToQuaternion(values[5]);
                }
            }
            //List<AnnotationManager> visitedAnnotations = new List<AnnotationManager>();
            //if (annotationData.ContainsKey(CurrentTimestamp))
            //{
            //    foreach (string[] values in annotationData[CurrentTimestamp])
            //    {
            //        AnnotationManager annotation;
            //        int id = int.Parse(values[1]);
            //        // If the index of this annotation does not already exist, instantiate a new one
            //        if (!Annotations.TryGetValue(id, out annotation))
            //        {
            //            annotation = ((GameObject)Instantiate(Resources.Load("Prefabs/SpeechBubble"), Block.objectContainerInNewScene.transform)).GetComponent<AnnotationManager>();
            //            annotation.SetTitle(values[1]);
            //            annotation.SetContent(values[2]);

            //            Annotations[id] = annotation;
            //        }
            //        visitedAnnotations.Add(annotation);

            //        annotation.transform.position = StringToVector3(values[3]);
            //        annotation.transform.rotation = StringToQuaternion(values[4]);
            //    }
            //}
            //// Destroy all unused annotations
            //var annotationsToRemove = Annotations.Where(x => !visitedAnnotations.Contains(x.Value)).ToArray();
            //foreach (var item in annotationsToRemove)
            //{
            //    Destroy(item.Value.gameObject);
            //    Annotations.Remove(item.Key);
            //}
        }
        isLiveReplayRunning = false;
        isLiveReplayPaused = false;
        stopwatch.Reset();

        Transform playerPos = GameObject.Find("Player").transform;
        if (!isLiveReplayStopped)
        {
            playerPos.position = new Vector3(playerPos.position.x, playerPos.position.y + 1.2f, playerPos.position.z);
            Transform playerBase = playerPos.GetChild(3);
            playerBase.localRotation = new Quaternion(0, 0, 0, 0);
            playerBase.localPosition = new Vector3(0, playerBase.localPosition.y, 0);
        }
        else
        {
            playerPos.position = new Vector3(playerPos.position.x, playerPos.position.y - 10f, playerPos.position.z);
            //GameObject.Find("TablesOnTimeline").transform.position = new Vector3(4.345f, 3.56f, -6.33f);
            //Debug.Log("TableOnTimeline pos x: " + GameObject.Find("TablesOnTimeline").transform.position.x);
        }

        //allElements.gameObject.SetActive(true);
        //_renderer = allElements.GetComponentsInChildren<Renderer>();
        //foreach (Renderer childRenderer in _renderer) //isue with tabletops
        //{
        //    if (childRenderer.gameObject.GetComponent<Interactable>() == null)
        //        childRenderer.enabled = true;
        //}
    }

    public void PauseLiveReplay()
    {
        GameObject PauseButton = GameObject.Find("PauseReplayButton");
        Transform ButtonContent = PauseButton.transform.GetChild(0);
        int pressedCount = PauseButton.GetComponent<Interactable>().ClickCount;
        if (pressedCount % 2 == 0)
        {
            isLiveReplayPaused = true;
            ButtonContent.GetChild(0).gameObject.SetActive(false);
            ButtonContent.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            isLiveReplayPaused = false;
            TimeScrubber = pausedTime;
            ButtonContent.GetChild(0).gameObject.SetActive(true);
            ButtonContent.GetChild(1).gameObject.SetActive(false);
        }
        //if(pressedCount > 0)
        //{

        //}       
    }

    public void StopLiveReplay()
    {
        isLiveReplayPaused = false;
        TimeScrubber = 1;
        isLiveReplayStopped = true;
        Transform playerPos = GameObject.Find("Player").transform;
        playerPos.position = new Vector3(playerPos.position.x, playerPos.position.y - 10f, playerPos.position.z);
        GameObject.Find("TablesOnTimeline").transform.position = new Vector3(4.345f,3.56f, -6.33f);
    }

    public void ReplayObjects(string startName, string[] values)
    {
        string objectName = values[1];
        if (objectName.StartsWith(startName))
        {
            GameObject objectToReplay = GameObject.Find(objectName);
            if(objectToReplay != null)
            {
                if (startName.Equals("Pedestal"))
                {
                    Renderer[] object_renderer = objectToReplay.transform.GetComponentsInChildren<Renderer>();
                    foreach (Renderer childRenderer in object_renderer)
                        childRenderer.enabled = true;
                }

                objectToReplay.transform.position = StringToVector3(values[3]);
                objectToReplay.transform.rotation = StringToQuaternion(values[4]);
                objectToReplay.transform.localScale = StringToVector3(values[5]);
            } 
        }
    }

    public void SetObjectWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            GameObject tableFound = GameObject.Find(targetName);
            Destroy(tableFound);
            theObject = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            theObject.name = targetName;
            theObject.transform.parent = GameObject.Find("AllElements").transform;
            theObject.tag = values[2];
            theObject.transform.position = StringToVector3(values[3]);
            theObject.transform.rotation = StringToQuaternion(values[4]);
            theObject.transform.localScale = StringToVector3(values[5]);
            theObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = values[6];
        }
    }

    public void SetBlockWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            //Debug.Log("Block to be Instantiated");
            theBlock = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            theBlock.name = targetName;
            theBlock.tag = values[2];
            theBlock.transform.position = StringToVector3(values[3]);
            theBlock.transform.rotation = StringToQuaternion(values[4]);
            theBlock.transform.localScale = StringToVector3(values[5]);
            theBlock.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text = values[6];
        }
    }

    public void SetLabelWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            theLabel = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            theLabel.name = targetName;
            theLabel.tag = values[2];
            theLabel.transform.position = StringToVector3(values[3]);
            theLabel.transform.rotation = StringToQuaternion(values[4]);
            theLabel.transform.localScale = StringToVector3(values[5]);
            theLabel.GetComponent<TextMeshPro>().text = values[6];
        }
    }

    public void SetLayerWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            GameObject layer = GameObject.Find(targetName);
            for(int i = 0; i < 6; i++)
                layer.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = true;
            layer.transform.GetChild(7).gameObject.GetComponent<Renderer>().enabled = true;
            layer.transform.GetChild(7).GetChild(0).gameObject.GetComponent<Renderer>().enabled = true; // render for timelabel cube
            layer.transform.GetChild(6).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
            layer.transform.position = StringToVector3(values[3]);
            layer.transform.rotation = StringToQuaternion(values[4]);
            layer.transform.localScale = StringToVector3(values[5]);
            //theLayer = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            //theLayer.name = targetName;
            //theLayer.transform.parent = GameObject.Find("AllElements").transform;
            //theLayer.tag = values[2];
            //theLayer.transform.position = StringToVector3(values[3]);
            //theLayer.transform.rotation = StringToQuaternion(values[4]);
            //theLayer.transform.localScale = StringToVector3(values[5]);
            //theLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = values[6];
        }
    }

    public void AddDataToDic()
    {
        // Player data
        string[] playerDataLines = PlayerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < playerDataLines.Length; i++)
        {
            string[] lines = playerDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!playerDic.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                playerDic[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        // Hand data
        string[] handDataLines = HandData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < handDataLines.Length; i++)
        {
            string[] lines = handDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!handData.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                handData[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        // Objects data
        string[] objectDataLines = ObjectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //string objectFileName = FilePath + ObjectData.name + ".txt";
        //string[] objectDataLines = File.ReadAllLines(objectFileName);
        for (int i = 1; i < objectDataLines.Length; i++)
        {
            string[] lines = objectDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!objectDic.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                objectDic[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        // Tables data
        TextAsset TableData = Resources.Load("Replay/06_Christmas_Scene1_Tables") as TextAsset;
        string[] tableDataLines = TableData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < tableDataLines.Length; i++)
        {
            string[] lines = tableDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!tableDic.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                tableDic[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        // Blocks data
        TextToDic(BlockData, blockDic);
        //string[] blockDataLines = BlockData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //for (int i = 1; i < blockDataLines.Length; i++)
        //{
        //    string[] lines = blockDataLines[i].Split('\t');
        //    float timestamp = float.Parse(lines[0]);

        //    List<string[]> timestampLines;
        //    if (!blockDic.TryGetValue(timestamp, out timestampLines))
        //    {
        //        timestampLines = new List<string[]>();
        //        blockDic[timestamp] = timestampLines;
        //    }

        //    timestampLines.Add(lines);
        //}

        //Label data
        TextToDic(LabelData, labelDic);

        //layer data
        TextToDic(LayerData, layerDic);

        // Annotation data
        string[] annotationDataLines = AnnotationData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        annotationData = new Dictionary<float, List<string[]>>();
        for (int i = 1; i < annotationDataLines.Length; i++)
        {
            string[] lines = annotationDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!annotationData.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                annotationData[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }
    }

    private void TextToDic(TextAsset theData, Dictionary<float, List<string[]>> theDic)
    {
        string[] blockDataLines = theData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < blockDataLines.Length; i++)
        {
            string[] lines = blockDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!theDic.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                theDic[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }
    }

    private Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    private Quaternion StringToQuaternion(string sQuaternion)
    {
        // Remove the parentheses
        if (sQuaternion.StartsWith("(") && sQuaternion.EndsWith(")"))
        {
            sQuaternion = sQuaternion.Substring(1, sQuaternion.Length - 2);
        }

        // split the items
        string[] sArray = sQuaternion.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }
}
