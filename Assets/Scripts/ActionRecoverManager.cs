using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ActionRecoverManager : MonoBehaviour
{
    public static Dictionary<int, List<string[]>> actionDic;
    public static int actionID;

    public static GameObject undo_controller;
    public static int pedestalOrLineCount = 1;

    float rotation = 0;
    static Vector3 linePosition;
    static Dictionary<int,List<string[]>> DicForRedo = new Dictionary<int, List<string[]>>();
    //int redoID = 0;
    //GameObject[] timeline_pedestals;

    private static ActionRecoverManager _instance;
    public static ActionRecoverManager Instance { get { return _instance; } }

    void Start()
    {
        actionDic = new Dictionary<int, List<string[]>> ();
        actionID = 0;
        //timeline_pedestals = GameObject.FindGameObjectsWithTag("timeline_pedestal");
    }

    // Update is called once per frame
    void Update()
    {
        //rotation = undo_controller.transform.rotation.y;
    }

    public static void Undo_Redo()
    {
        //Debug.Log("default rotation: " + rotation);  //0.5000001
        if (actionDic != null)
        {
            if (SnapZone.actionReq == "undo")
            {
                //Debug.Log("undo");
                //Debug.Log(rotation);
                if(actionDic.ContainsKey(actionDic.Count - 1))
                {
                    List<string[]> theLastList = actionDic[actionDic.Count - 1];
                    foreach (string[] theObjectArray in theLastList)
                    {
                        CheckObject(theObjectArray[0], theObjectArray[1], false, true);
                    }
                    //redoID++;
                    int redoID = 0;
                    if (DicForRedo != null)
                        redoID = DicForRedo.Count;
                    DicForRedo.Add(redoID, theLastList);
                    //foreach (KeyValuePair<int, List<string[]>> pair in DicForRedo)
                    //    Debug.Log(pair.Key.ToString() + ": " + pair.Value[0][0]);
                    actionDic.Remove(actionDic.Count - 1);
                    pedestalOrLineCount++;
                }
            }
            //else if (rotation < 0.45)
            else if (SnapZone.actionReq == "redo") //(rotation < 0.45)
            {
                //Debug.Log("redo");
                //Debug.Log(rotation);
                if (DicForRedo != null)
                {
                    //Debug.Log("DicForRedo is not empty");
                    KeyValuePair<int, List<string[]>> lastPair = DicForRedo.Last();
                    foreach (string[] theObjectArray in lastPair.Value)
                    {
                        CheckObject(theObjectArray[0], theObjectArray[1], true, false);
                    }
                    actionDic.Add(actionDic.Count, lastPair.Value);
                    DicForRedo.Remove(lastPair.Key);
                }
                pedestalOrLineCount--;
            }
        }

        //undo_controller.transform.rotation = SnapZone.handleRotation;
        //Debug.Log("Reset value: " + rotation);
    }

    public static void CheckObject(string name, string boolValue, bool afterAction, bool beforeAction)
    {
        GameObject theObject = GameObject.Find(name);
        if (boolValue.Equals("true"))
        {
            if (name.StartsWith("Block"))
            {
                theObject.GetComponent<Renderer>().enabled = afterAction;
                theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = afterAction;
            }
            if (name.StartsWith("Pedestal"))
            {
                theObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;
                int pedestalNo = int.Parse(name.Split('_')[1]);
                GameObject[] timeline_pedestals = GameObject.FindGameObjectsWithTag("timeline_pedestal");
                Transform pedestalOnTimeline = timeline_pedestals[pedestalNo].transform;
                pedestalOnTimeline.GetChild(0).GetChild(1).gameObject.GetComponent<Renderer>().enabled = afterAction;
                pedestalOnTimeline.GetChild(1).GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;
                pedestalOnTimeline.GetChild(2).GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;
            }
            if (name.StartsWith("Line"))
            {
                if (!afterAction)
                {
                    linePosition = theObject.transform.position;
                    theObject.transform.position = new Vector3(theObject.transform.position.x - 20, theObject.transform.position.y, theObject.transform.position.z);
                }
                else
                {
                    theObject.transform.position = linePosition;
                }
            }
            if (name.StartsWith("TableLink"))
            {
                if (!afterAction)
                {
                    linePosition = theObject.transform.position;
                    theObject.transform.position = new Vector3(theObject.transform.position.x, theObject.transform.position.y - 20, theObject.transform.position.z);
                }
                else
                {
                    theObject.transform.position = linePosition;
                }
            }
            if (name.StartsWith("Layer"))
            {
                Renderer[] _renderer = theObject.transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer childRenderer in _renderer)
                {
                    if (childRenderer.gameObject.GetComponent<Interactable>() == null | childRenderer.gameObject.tag == "timelabelCube")
                        childRenderer.enabled = afterAction;
                }
                theObject.transform.GetChild(7).gameObject.GetComponent<Renderer>().enabled = beforeAction;
                theObject.transform.GetChild(7).GetChild(0).gameObject.GetComponent<Renderer>().enabled = beforeAction;// for timelabel cube
                theObject.transform.GetChild(6).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().enabled = beforeAction;
                if (!afterAction)
                {
                    for(int i=0; i < 5; i++)
                    {
                        theObject.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = beforeAction;
                        theObject.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Renderer>().enabled = beforeAction;
                    }
                    theObject.transform.GetChild(5).gameObject.GetComponent<Renderer>().enabled = beforeAction;                   
                }
                else
                {
                    LayerManager[] embededLayers = theObject.transform.GetComponentsInChildren<LayerManager>();
                    for(int i = 1; i < embededLayers.Length; i++)
                    {
                        Renderer[] embededLayer_renderer = embededLayers[i].transform.GetComponentsInChildren<Renderer>();
                        foreach (Renderer childRenderer in embededLayer_renderer)
                            childRenderer.enabled = beforeAction;
                    }
                }
            }
            if (name.StartsWith("Tabletop"))
            {
                string tableNo = name.Split('_')[1];
                GameObject layerOnTable = GameObject.Find("Layer_" + tableNo);
                Renderer[] layer_renderer = layerOnTable.transform.GetComponentsInChildren<Renderer>();

                string tableName = theObject.name;
                Vector3 position = theObject.transform.position;
                Transform parent = theObject.transform.parent;
                string tableInfo = theObject.transform.GetChild(0).GetComponent<TextMeshPro>().text;
                if (!afterAction)
                {
                    Renderer[] _renderer = theObject.transform.GetComponentsInChildren<Renderer>();
                    foreach (Renderer childRenderer in _renderer)
                    {
                        childRenderer.enabled = afterAction;
                    }

                    foreach (Renderer childRenderer in layer_renderer)
                    {
                        childRenderer.enabled = afterAction;
                    }
                }
                else
                {
                    GameObject newTable = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
                    newTable.name = tableName;
                    newTable.transform.parent = parent;
                    newTable.transform.position = position;
                    newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = tableInfo;
                    theObject.Destroy();

                    for (int i = 0; i < 5; i++)
                    {
                        layerOnTable.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = afterAction;
                        layerOnTable.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Renderer>().enabled = afterAction;
                    }
                    layerOnTable.transform.GetChild(5).gameObject.GetComponent<Renderer>().enabled = afterAction;
                    layerOnTable.transform.GetChild(6).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;
                    layerOnTable.transform.GetChild(7).gameObject.GetComponent<Renderer>().enabled = afterAction;
                    layerOnTable.transform.GetChild(7).GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;// timelabel cube

                    //foreach (Renderer childRenderer in layer_renderer)
                    //{
                    //    if(childRenderer.gameObject.GetComponent<Interactable>() == null)
                    //        childRenderer.enabled = afterAction;
                    //}
                }

                GameObject[] timeline_tables = GameObject.FindGameObjectsWithTag("timeline_table");
                Transform tableOnTimeline = timeline_tables[int.Parse(tableNo)].transform;
                Renderer[] timelineTable_renderer = tableOnTimeline.GetComponentsInChildren<Renderer>();
                foreach (Renderer childRenderer in timelineTable_renderer)
                {
                    childRenderer.enabled = afterAction;
                }

                GameObject hand_menu = GameObject.Find("Hand_Menu");
                Transform miniTable = hand_menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(int.Parse(tableNo));
                Renderer[] miniTable_renderer = miniTable.GetComponentsInChildren<Renderer>();
                foreach (Renderer childRenderer in miniTable_renderer)
                {
                    childRenderer.enabled = afterAction;
                }
            }

        }
        else if (boolValue.Equals("false"))
        {
            if (name.StartsWith("Block"))
            {
                theObject.GetComponent<Renderer>().enabled = beforeAction;
                theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = beforeAction;
            }

        }

    }
}
