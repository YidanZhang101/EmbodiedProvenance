using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ActionRecover : MonoBehaviour
{
    public enum ActionType
    {
        Grab,
        Create,
        Duplicate,
        Destroy
    }

    public enum LogTargetType
    {
        Block,
        layer,
        Tabletop,
        Line,
        Pedestal,
        Camera        
    }

    public enum LogStatus
    {
        Start,
        End,
        Discrete,
        All
    }

    private static ActionRecover instance;
    public static ActionRecover getinstance
    {
        get
        {
            if (instance == null)
            {
                instance = new ActionRecover();
            }
            return instance;
        }
    }

    [Header("Logging Settings")]
    public string FilePath = "C:/Users/Yidan/Desktop/LoggingData/";
    //public string GroupID;
    public float TimeBetweenLogs = 0.05f;
    public float AutomaticLogDelay = 0.25f;
    public string TestMessage;

    [HideInInspector]
    public bool IsLogging = false;

    public StreamWriter actionStreamWriter;
    public Dictionary<float, List<string[]>> logRecords = new Dictionary<float, List<string[]>>();
    public List<string[]> targetList = new List<string[]>();

    private float logStartTime;
    private float timer = 0f;
    private const string format = "F3";
    private string scene = "";
    private string sceneNo = "";

    void Start()
    {
        Invoke("StartLogging", AutomaticLogDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLogging()
    {
        //GetStaticReferences();
        TestMessage = "StartLogging";
        scene = "06_Christmas";
        sceneNo = "1";
        string path = string.Format("{0}{1}_Scene{2}_Actions.txt", FilePath, scene, sceneNo);
        actionStreamWriter = new StreamWriter(path, true);
        actionStreamWriter.WriteLine("Timestamp\tTargetName\tTargetType\tActionType\tStatus\tPosition");

        IsLogging = true;
        logStartTime = Time.unscaledTime;

        TestMessage = "Logging started for Actions";
    }

    //public void test()
    //{
    //    GameObject.Find("Valve_Prefab").SetActive(false);
    //    Vector3 a = new Vector3(0, 0, 0);
    //    LogActionData("Block", "Block", "Ctreate", "Start", a);
    //    Debug.Log("ActionList: " + actionList[-1]);
    //}

    public void LogActionData(string targetName,string targetType, string actionType, string status, Vector3 position)
    {
        //actionStreamWriter.WriteLine("Timestamp\tBlock\tBlock\tCreate\tStart\t(0,0,0)");
        //TestMessage = "Logging data";

        if (IsLogging)
        {
            Debug.Log("within isLogging");
            float timestamp = Time.unscaledTime - logStartTime;
            actionStreamWriter.WriteLine(string.Join("\t",
                timestamp,
                targetName,
                targetType,
                actionType,
                status,
                position.ToString(format)
            ));
            actionStreamWriter.Flush();
        }
    }

    public void StopLogging()
    {
        IsLogging = false;
        timer = 0;

        actionStreamWriter.Close();

        TestMessage = "Logging stopped for Actions";
    }

    public void OnApplicationQuit()
    {
        if (IsLogging)
            StopLogging();
    }

    public void AddToDictionary()
    {
        float timestamp = Time.unscaledTime - logStartTime;
        AddToList("tabletop"); // add Tabletop
        AddToList("pedestal"); // add Pedestal
        AddToList("line"); // add Line
        // add Block & TimeLabel
        AddToList("year");
        AddToList("month");
        AddToList("week");
        AddToList("day");
        AddToList("hour");

        logRecords.Add(timestamp, targetList);
    }

    public void AddToList(string targetTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        if (targets != null)
        {
            foreach (GameObject target in targets)
            {
                string objectName = target.name;
                string objectPosition = target.transform.position.ToString(format);
                string objectRotation = target.transform.rotation.ToString(format);
                string objectScale = target.transform.localScale.ToString(format);
                string[] theTarget = {objectName, objectPosition, objectRotation, objectScale};
                targetList.Add(theTarget);
            }
        }
    }
}
