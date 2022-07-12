using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DataReplayManager : MonoBehaviour
{
    [Header("Data Files")]
    public TextAsset SessionInfoData;
    public TextAsset PlayerData;
    public TextAsset ObjectData;

    [Header("Static Replay Objects")]
    public GameObject[] Player;

    [Range(0, 1)] public float TimeScrubber;
    [Range(0, 10)] public float SpeedMultiplier = 1;
    public float CurrentTime = 0;
    public float CurrentTimestamp = 0;

    private Dictionary<int, int> playerIDToRoleIDDict;

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

    public float selectedTime;
    float logStartTime;
    string[] playerDataLines;


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

        float[] timestamps = objectDic.Keys.Distinct().OrderBy(x => x).ToArray();
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

            // Set player and hands
            foreach (string[] values in playerDic[CurrentTimestamp])
            {
                string type = values[1];
                if(type == "player")
                {
                    Transform headTransform = GameObject.Find("Player").transform;
                    headTransform.position = StringToVector3(values[2]);
                    headTransform.rotation = StringToQuaternion(values[3]);
                }
                if (type == "left hand")
                {
                    Transform handTransform = GameObject.Find("LeftHand").transform;
                    handTransform.position = StringToVector3(values[2]);
                    handTransform.rotation = StringToQuaternion(values[3]);
                }
                if (type == "right hand")
                {
                    Transform handTransform = GameObject.Find("RightHand").transform;
                    handTransform.position = StringToVector3(values[2]);
                    handTransform.rotation = StringToQuaternion(values[3]);
                }
            }

            foreach (string[] values in objectDic[CurrentTimestamp])
            {
                string objectName = values[1];
                Debug.Log(objectName);
                //if (!objectName.Contains("TimeLabel"))
                //{
                //    Transform objectTransform = GameObject.Find(objectName).transform;
                //    objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, 1);
                //}
                //else
                //{
                //    RectTransform objectTransform = GameObject.Find(objectName).GetComponent<RectTransform>();
                //    objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, 1);
                //}
                
                //GameObject.Find(objectName).SetActive(false);

            }

            // Set objects
            //foreach (string[] values in objectDic[CurrentTimestamp])
            //{
            //    SetObjectWithTag("Tabletop", values);
            //    SetObjectWithTag("Block_LTB", values);
            //    SetObjectWithTag("Block_Monash_Sport", values);
            //    SetObjectWithTag("Block_Theatre", values);
            //    SetObjectWithTag("Block_Campus_Centre", values);
            //    SetObjectWithTag("Block_New_Horizons", values);
            //    SetObjectWithTag("TimeLabel", values);
            //    SetObjectWithTag("Pedestal", values);
            //    SetObjectWithTag("Line", values);
            //}
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
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/" + prefabName));
            newObject.name = targetName;
            newObject.tag = values[2];
            newObject.transform.position = StringToVector3(values[3]);
            newObject.transform.rotation = StringToQuaternion(values[4]);
            newObject.transform.localScale = StringToVector3(values[5]);
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
