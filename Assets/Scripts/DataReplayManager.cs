using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class DataReplayManager : MonoBehaviour
{
    [Header("Data Files")]
    public TextAsset SessionInfoData;
    public TextAsset PlayerData;
    public TextAsset HandData;
    public TextAsset ObjectData;
    public TextAsset BlockData;
    public TextAsset LabelData;
    public TextAsset LayerData;

    [Header("Static Replay Objects")]
    public GameObject Player;
    public GameObject[] Hands;

    [HideInInspector] public List<string> targetObjects = new List<string>();
    [HideInInspector] public List<string> targetBlocks = new List<string>();
    [HideInInspector] public List<string> targetLabels = new List<string>();
    [HideInInspector] public List<string> targetLayers = new List<string>();

    [Range(0, 1)] public float TimeScrubber;
    [Range(0, 10)] public float SpeedMultiplier = 1;
    public float CurrentTime = 0;
    public float CurrentTimestamp = 0;

    //private Dictionary<int, int> playerIDToRoleIDDict;

    public bool isLiveReplayRunning = false;
    private bool isLiveReplayPaused = false;

    private float prevTimeScrubber;
    private float prevSpeedMultiplier;
    //private int actionsIdx;

    private System.Diagnostics.Stopwatch stopwatch;

    private static DataReplayManager _instance;
    public static DataReplayManager Instance { get { return _instance; } }

    Dictionary<float, List<string[]>> playerDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> objectDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> handData = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> blockDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> labelDic = new Dictionary<float, List<string[]>>();
    Dictionary<float, List<string[]>> layerDic = new Dictionary<float, List<string[]>>();

    public float selectedTime;
    float logStartTime;
    string[] playerDataLines;
    GameObject theObject;
    GameObject theBlock;
    GameObject theLabel;
    GameObject theLayer;

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
        playerDataLines = PlayerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        AddDataToDic();
        //SetTransparency();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartLiveReplay()
    {
        GameObject allElements = GameObject.Find("AllElements");
        if (allElements != null)
            allElements.SetActive(false);
        if (!isLiveReplayRunning)
        {
            isLiveReplayPaused = false;
            StartCoroutine(Replay());
        }
        else if (isLiveReplayPaused)
        {
            isLiveReplayPaused = false;
        }
    }

    //public void Play()
    //{
    //    float currentTime = Time.unscaledTime - logStartTime;
    //    Debug.Log(currentTime);
    //    for (int i = 1; i < playerDataLines.Length; i++)
    //    {
    //        string[] lines = playerDataLines[i].Split('\t');
    //        float timestamp = float.Parse(lines[0]);            

    //        if (timestamp >= selectedTime && timestamp <= currentTime)
    //        {
    //            string type = lines[1];
    //            if (type == "player")
    //            {
    //                Transform headTransform = GameObject.Find("Player").transform;
    //                headTransform.position = StringToVector3(lines[2]);
    //                headTransform.rotation = StringToQuaternion(lines[3]);
    //            }
    //            if (type == "left hand")
    //            {
    //                Transform handTransform = GameObject.Find("LeftHand").transform;
    //                handTransform.position = StringToVector3(lines[2]);
    //                handTransform.rotation = StringToQuaternion(lines[3]);
    //            }
    //            if (type == "right hand")
    //            {
    //                Transform handTransform = GameObject.Find("RightHand").transform;
    //                handTransform.position = StringToVector3(lines[2]);
    //                handTransform.rotation = StringToQuaternion(lines[3]);
    //            }
    //        }
    //    }
    //}

    public IEnumerator Replay()
    {
        isLiveReplayRunning = true;

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

            //foreach (string[] values in objectDic[CurrentTimestamp])
            //{
            //    string objectName = values[1];
            //    Debug.Log(objectName);
            //    //if (!objectName.Contains("TimeLabel"))
            //    //{
            //    //    Transform objectTransform = GameObject.Find(objectName).transform;
            //    //    objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, 1);
            //    //}
            //    //else
            //    //{
            //    //    RectTransform objectTransform = GameObject.Find(objectName).GetComponent<RectTransform>();
            //    //    objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, 1);
            //    //}

            //    //GameObject.Find(objectName).SetActive(false);

            //}

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

            // Set objects
            List<GameObject> visitedObjects = new List<GameObject>();
            if (objectDic.ContainsKey(CurrentTimestamp))
            {
                Debug.Log(CurrentTimestamp);
                foreach (string[] values in objectDic[CurrentTimestamp])
                {                   
                    string objectName = values[1];
                    if (!targetObjects.Contains(objectName))
                    {
                        SetObjectWithTag("Tabletop", values);
                        SetObjectWithTag("Pedestal", values);
                        SetObjectWithTag("Line", values);

                        //Debug.Log("Create " + theObject.name);
                        targetObjects.Add(theObject.name);
                        
                    }
                    visitedObjects.Add(theObject);
                    //Debug.Log("visitedObjects: " + theObject.name);
                    theObject.transform.position = StringToVector3(values[3]);
                    theObject.transform.rotation = StringToQuaternion(values[4]);
                    theObject.transform.localScale = StringToVector3(values[5]);
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
                Debug.Log(CurrentTimestamp);
                foreach (string[] values in blockDic[CurrentTimestamp])
                {
                    string objectName = values[1];
                    if (!targetBlocks.Contains(objectName))
                    {
                        SetBlockWithTag("Block_LTB", values);
                        SetBlockWithTag("Block_Monash_Sport", values);
                        SetBlockWithTag("Block_Theatre", values);
                        SetBlockWithTag("Block_Campus_Centre", values);
                        SetBlockWithTag("Block_New_Horizons", values);
                        SetObjectWithTag("TimeLabel", values);

                        targetBlocks.Add(theBlock.name);
                        Debug.Log("Create " + theBlock.name);
                    }
                    //if (!visitedBlocks.Contains(theBlock))
                    visitedBlocks.Add(theBlock.name);
                    Debug.Log("visitedBlocks: " + theBlock.name);
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
                    if (!targetLabels.Contains(objectName))
                    {
                        SetLabelWithTag("TimeLabel", values);

                        targetLabels.Add(theLabel.name);
                        //Debug.Log("Create " + theLabel.name);
                    }
                    visitedLabels.Add(theLabel);
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
                    string objectName = values[1];
                    if (!targetLayers.Contains(objectName))
                    {
                        SetLayerWithTag("Layer", values);

                        targetLayers.Add(theLayer.name);
                        //Debug.Log("Create " + theLabel.name);
                    }
                    visitedLayers.Add(theLayer);
                    //Debug.Log("visitedLabels: " + theLabel.name);
                    theLayer.transform.position = StringToVector3(values[3]);
                    theLayer.transform.rotation = StringToQuaternion(values[4]);
                    theLayer.transform.localScale = StringToVector3(values[5]);
                }
            }
            //Destroy all unused labels
            string[] layersToRemove = targetLayers.Where(x => !visitedLayers.Contains(GameObject.Find(x))).ToArray();
            //foreach (string item in labelsToRemove)
            //{
            //    Debug.Log("labelsToRemove: " + item);
            //}
            foreach (string item in layersToRemove)
            {
                Destroy(GameObject.Find(item));
                targetLayers.Remove(item);
            }

        }
        isLiveReplayRunning = false;
        isLiveReplayPaused = false;
        stopwatch.Reset();
    }

    //public void SetTransparency()
    //{
    //    float[] timestamps = playerDic.Keys.Distinct().OrderBy(x => x).ToArray();
    //    float totalTime = timestamps.Max();
    //    float newTime = TimeScrubber * totalTime;
    //    float newTimestamp = timestamps.Aggregate((x, y) => Math.Abs(x - newTime) < Math.Abs(y - newTime) ? x : y);
    //    foreach (float theTime in objectDic.Keys)
    //    {
    //        if(theTime >= newTimestamp)
    //        {
    //            foreach (string[] values in objectDic[theTime])
    //            {
    //                string objectName = values[1];
    //                if (!objectName.Contains("Tabletop"))
    //                {
    //                    GameObject.Find(objectName).GetComponent<Renderer>().enabled = false;
    //                }                   
    //            }
    //        }
    //    }
    //}

    public void PauseLiveReplay()
    {
        isLiveReplayPaused = true;
    }

    public void SetObjectWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            //Debug.Log("Object to be Instantiated");
            theObject = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            theObject.name = targetName;
            theObject.tag = values[2];
            theObject.transform.position = StringToVector3(values[3]);
            theObject.transform.rotation = StringToQuaternion(values[4]);
            theObject.transform.localScale = StringToVector3(values[5]);
        }
    }

    public void SetBlockWithTag(string prefabName, string[] values)
    {
        string targetName = values[1];
        if (targetName.StartsWith(prefabName))
        {
            Debug.Log("Block to be Instantiated");
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
            //Debug.Log("Label to be Instantiated");
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
            //Debug.Log("Label to be Instantiated");
            theLayer = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            theLayer.name = targetName;
            theLayer.tag = values[2];
            theLayer.transform.position = StringToVector3(values[3]);
            theLayer.transform.rotation = StringToQuaternion(values[4]);
            theLayer.transform.localScale = StringToVector3(values[5]);
            theLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = values[6];
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
