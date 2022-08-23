using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public GameObject timeLabel;

    public static Dictionary<string, Vector3> positionForTimelineTables = new Dictionary<string, Vector3>();
    public static Dictionary<string, Vector3> positionForMiniTables = new Dictionary<string, Vector3>();
    public static Dictionary<string, List<Vector3>> positionForMiniTableLinks = new Dictionary<string, List<Vector3>>();

    public static Dictionary<int, List<string[]>> objectsDicForCopy = new Dictionary<int, List<string[]>>();
    public int key = 1;
    List<string[]> listForCopy = new List<string[]>();

    GameObject newTable;
    GameObject newMiniTable;
    int grabCount = 0;
    string timeDescription;
    string textAdded;
    string tableForDescName;

    string tableLinkName;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CountGrab()
    {
        grabCount += 1;
    }

    public int TableMoveIn_x_Side(Transform allElements)
    {
        int x_index = 0;
        if (transform.parent != null & transform.parent == allElements)
        {
            x_index = 1; // year level
            timeDescription = transform.GetChild(7).GetComponent<TextMeshPro>().text;
        }   
        else
        {
            if (transform.parent.parent != null & transform.parent.parent == allElements)
            {
                x_index = 2; // month level
                timeDescription = transform.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + transform.GetChild(7).GetComponent<TextMeshPro>().text;
            }
            else
            {
                if (transform.parent.parent.parent != null & transform.parent.parent.parent == allElements)
                {
                    x_index = 3; // week level
                    timeDescription = transform.parent.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + 
                        transform.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + transform.GetChild(7).GetComponent<TextMeshPro>().text;
                }
                else
                {
                    x_index = 4; // day level
                    timeDescription = transform.parent.parent.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + transform.parent.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + 
                        transform.parent.GetChild(7).GetComponent<TextMeshPro>().text + " " + transform.GetChild(7).GetComponent<TextMeshPro>().text;
                }
            }
        }
        return x_index;
    }

    public int TableMoveIn_z_Side()
    {
        int z_index = 0;
        LayerManager[] children = transform.parent.GetComponentsInChildren<LayerManager>();
        List<Transform> childrenInSameLevel = new List<Transform>();
        foreach (LayerManager child in children)
        {
            if (child.transform.parent == transform.parent)
                childrenInSameLevel.Add(child.transform);
            foreach (Transform item in childrenInSameLevel)
            {
                if (item.name == transform.name)
                    z_index = childrenInSameLevel.IndexOf(item);
            }
        }
        return z_index + 1;
    }

    private void SetTablePos(int tableCount,int tableCounter,int counterInNewScene, TeleportManager[] allMiniTables, Transform firstMiniTable)
    {
        GameObject base_layer = GameObject.Find("Base_Layer");
        Transform allElements = GameObject.Find("AllElements").transform;
        int x_move_counter = TableMoveIn_x_Side(allElements);
        int z_move_counter = TableMoveIn_z_Side();
        if (allElements.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().isVisible == true)
        {
            GameObject tableLink = (GameObject)Instantiate(Resources.Load("Prefabs/LinkForTables"));
            int linkCount = GameObject.FindGameObjectsWithTag("tableLink").Length - 1;
            tableLinkName = "TableLink_" + linkCount.ToString();
            tableLink.name = tableLinkName;
            tableLink.transform.parent = transform.parent;

            //GameObject linkForMiniTable = (GameObject)Instantiate(Resources.Load("Prefabs/LinkForMiniTable"));
            //linkForMiniTable.name = "MiniTableLink_" + linkCount.ToString();
            GameObject hand_menu = GameObject.Find("Hand_Menu");
            Transform yearlyMiniTable = hand_menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
            //linkForMiniTable.transform.parent = yearlyMiniTable.parent.parent;
            List<Vector3> startEndPoints = new List<Vector3>();
            if (x_move_counter == 1)
            {
                newTable.transform.position = new Vector3(newTable.transform.position.x + 2f,
                        newTable.transform.position.y, newTable.transform.position.z + 2f * (z_move_counter - 1));
                transform.position = new Vector3(base_layer.transform.position.x + 0.002f + 2f,
                    base_layer.transform.position.y, base_layer.transform.position.z - 0.016f + 2f * (z_move_counter - 1));
                
                //set link start and end point
                tableLink.transform.GetChild(0).position = GameObject.Find("Tabletop (1)").transform.GetChild(1).position;
                tableLink.transform.GetChild(1).position = newTable.transform.GetChild(2).position;

                newMiniTable.transform.localPosition = new Vector3(newMiniTable.transform.localPosition.x - 1.6f,
                        firstMiniTable.localPosition.y, newMiniTable.transform.localPosition.z - 1.72f * (z_move_counter - 1));

                //link for mini tables
                //linkForMiniTable.transform.localRotation = yearlyMiniTable.localRotation;
                //linkForMiniTable.transform.localScale = yearlyMiniTable.localScale;
                //linkForMiniTable.transform.GetChild(0).localPosition = yearlyMiniTable.GetChild(1).localPosition;
                //linkForMiniTable.transform.GetChild(1).localPosition = newMiniTable.transform.GetChild(2).localPosition;
                //startEndPoints.Add(yearlyMiniTable.GetChild(1).localPosition);
                //startEndPoints.Add(newMiniTable.transform.GetChild(2).localPosition);
                //positionForMiniTableLinks.Add(linkForMiniTable.name, startEndPoints);
            }
            else
            {
                string[] tableInfo = timeDescription.Split();
                string infoToCheck = "";

                int infoJoinCounter = 0;
                if (x_move_counter < 4)
                    infoJoinCounter = x_move_counter - 1;
                else
                    infoJoinCounter = x_move_counter;

                for (int i = 0; i < infoJoinCounter; i++)
                {
                    infoToCheck += " " + tableInfo[i];
                }

                infoToCheck = infoToCheck.Trim();
                string sourceTableName = "";
                Vector3 sourceTablePos = new Vector3();
                GameObject[] existingTables = GameObject.FindGameObjectsWithTag("tabletop");
                foreach(GameObject table in existingTables)
                {
                    if (table.transform.GetChild(0).GetComponent<TextMeshPro>().text.Contains(infoToCheck))
                    {
                        sourceTablePos = table.transform.position;
                        sourceTableName = table.name;
                        Debug.Log("sourceTableName: " + sourceTableName);
                        //set link start point
                        tableLink.transform.GetChild(0).position = table.transform.GetChild(1).position;
                        break;
                    }
                }
                string sourceMiniMapName = "MiniTable_" + sourceTableName.Split('_')[1];
                Vector3 sourceMiniMapPos = new Vector3();
                Vector3 sourcePosForLink = new Vector3();
                foreach (TeleportManager aMiniTable in allMiniTables)
                {
                    if (aMiniTable.transform.name.Equals(sourceMiniMapName))
                    {
                        sourceMiniMapPos = aMiniTable.transform.localPosition;
                        sourcePosForLink = aMiniTable.transform.GetChild(1).localPosition;
                        //linkForMiniTable.transform.GetChild(0).localPosition = sourcePosForLink;
                    }          
                }
                if (tableInfo[0].Equals("2020"))
                {
                    newTable.transform.position = new Vector3(sourceTablePos.x + 2f,
                        newTable.transform.position.y, sourceTablePos.z + 2f * (z_move_counter - 1));
                    transform.position = new Vector3(sourceTablePos.x + 0.208f + 2f,
                    base_layer.transform.position.y, sourceTablePos.z + 0.514f + 2f * (z_move_counter - 1));

                    newMiniTable.transform.localPosition = new Vector3(sourceMiniMapPos.x - 1.6f,
                        sourceMiniMapPos.y, sourceMiniMapPos.z - 1.72f * (z_move_counter - 1));
                }
                else
                {
                    newTable.transform.position = new Vector3(sourceTablePos.x + 2f,
                        newTable.transform.position.y, sourceTablePos.z - 2f * (z_move_counter - 1));
                    transform.position = new Vector3(sourceTablePos.x + 0.208f + 2f,
                    base_layer.transform.position.y, sourceTablePos.z + 0.514f - 2f * (z_move_counter - 1));

                    newMiniTable.transform.localPosition = new Vector3(sourceMiniMapPos.x - 1.6f,
                        sourceMiniMapPos.y, sourceMiniMapPos.z + 1.72f * (z_move_counter - 1));
                }
                //set link end point
                tableLink.transform.GetChild(1).position = newTable.transform.GetChild(2).position;

                //linkForMiniTable.transform.GetChild(1).localPosition = newMiniTable.transform.GetChild(2).localPosition;
                startEndPoints.Add(sourcePosForLink);
                startEndPoints.Add(newMiniTable.transform.GetChild(2).localPosition);
                //positionForMiniTableLinks.Add(linkForMiniTable.name, startEndPoints);
            }

            //player on mini map
            Transform playerOnMiniMap = hand_menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            playerOnMiniMap.localPosition = new Vector3(newMiniTable.transform.localPosition.x, newMiniTable.transform.localPosition.y + 0.7f,
                newMiniTable.transform.localPosition.z + 0.2f);

            //if (tableCount <= 13)
            //{
            //    newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * tableCounter, newTable.transform.position.y, newTable.transform.position.z);
            //    transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * tableCounter, base_layer.transform.position.y, base_layer.transform.position.z);
            //}
            //else if (tableCount > 13 && tableCount < 28)
            //{
            //    newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * (27 - tableCount), newTable.transform.position.y, newTable.transform.position.z - 2.5f);
            //    transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * (27 - tableCount), base_layer.transform.position.y, base_layer.transform.position.z - 2.5f);
            //}
        }
        else
        {
            if (tableCount <= 13)
            {
                newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * counterInNewScene, newTable.transform.position.y, newTable.transform.position.z);
                transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * counterInNewScene, base_layer.transform.position.y, base_layer.transform.position.z);
            }
            else if (tableCount > 13 && tableCount < 28)
            {
                newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * (28 - counterInNewScene), newTable.transform.position.y, newTable.transform.position.z - 2.5f);
                transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * (28 - counterInNewScene), base_layer.transform.position.y, base_layer.transform.position.z - 2.5f);
            }
            counterInNewScene++;
        }
    }

    public void AddTableforLayer()
    {
        GameObject base_layer = GameObject.Find("Base_Layer");
        int tableCounter = GameObject.FindGameObjectsWithTag("tabletop").Length;
        if (grabCount == 1)
        {
            //add real table
            newTable = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
            int tableCount = GameObject.FindGameObjectsWithTag("tabletop").Length - 1;
            newTable.name = "Tabletop_" + tableCount.ToString();
            newTable.transform.parent = base_layer.transform.parent;
            tableForDescName = newTable.name;

            // add table on mini map
            TeleportManager[] allMiniTables = FindObjectsOfType<TeleportManager>(true);
            List<Transform> firstMiniTable = new List<Transform>();
            foreach (TeleportManager aMiniTable in allMiniTables)
            {
                if (aMiniTable.transform.name.Equals("MiniTable"))
                    firstMiniTable.Add(aMiniTable.transform);
            }
            newMiniTable = (GameObject)Instantiate(Resources.Load("Prefabs/MiniTable"), firstMiniTable[0].parent);
            newMiniTable.name = "MiniTable_" + tableCount.ToString();            
            newMiniTable.transform.rotation = firstMiniTable[0].transform.rotation;

            SetTablePos(tableCount, tableCounter, 1, allMiniTables, firstMiniTable[0]);

            positionForMiniTables.Add(newMiniTable.name, newMiniTable.transform.localPosition);

            transform.rotation = new Quaternion();
            newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;

            GameObject mrtkRig = GameObject.FindGameObjectWithTag("MRTKRig");
            mrtkRig.transform.position = new Vector3(newTable.transform.position.x, newTable.transform.position.y, newTable.transform.position.z);

            //add table in Timeline
            GameObject yearlyTable = GameObject.Find("Yearly_Table");
            GameObject timelineTable = (GameObject)Instantiate(Resources.Load("Prefabs/Timeline_Table"), yearlyTable.transform.parent);
            timelineTable.name = "Timeline_Table_" + tableCounter;
            //timelineTable.transform.parent = yearlyTable.transform.parent;
            timelineTable.transform.localPosition = new Vector3(yearlyTable.transform.localPosition.x + 0.17f * tableCounter, yearlyTable.transform.localPosition.y, yearlyTable.transform.localPosition.z);
            timelineTable.transform.rotation = yearlyTable.transform.rotation;
            timelineTable.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = timeDescription;
            AddForClippingBox(timelineTable);
            positionForTimelineTables.Add(timelineTable.name, timelineTable.transform.localPosition);

            //add to dictionary for copy&paste
            string[] tableArray = {newTable.name, newTable.transform.position.ToString("F4"), newTable.transform.rotation.ToString(), newTable.transform.localScale.ToString("F4"), timeDescription};
            string[] layerArray = {gameObject.name, gameObject.transform.position.ToString("F4"), gameObject.transform.rotation.ToString(), gameObject.transform.localScale.ToString("F4"), timeDescription };
            listForCopy.Add(tableArray);
            listForCopy.Add(layerArray);
            key = GameObject.FindGameObjectsWithTag("tabletop").Length - 1;
            objectsDicForCopy.Add(key, listForCopy);

            //Add to dictionary for Undo/Redo

            //int ActionKey = ActionRecoverManager.actionID++;
            string[] newTableForAction = { newTable.name, "true" };
            string[] newTableLinkForAction = { tableLinkName, "true" };
            
            List<string[]> listForAction = new List<string[]>();
            listForAction.Add(newTableForAction);
            listForAction.Add(newTableLinkForAction);

            int ActionKey = 0;
            if (ActionRecoverManager.actionDic != null)
                ActionKey = ActionRecoverManager.actionDic.Count;
            ActionRecoverManager.actionDic.Add(ActionKey, listForAction);

            //layerFinalPos = new Vector3(gameObject.transform.position.x, 1.58f);
            //layerFinalRot = gameObject.transform.rotation;
            //gameObject.transform.position = layerFinalPos;
            //Vector3 tableBase = new Vector3(gameObject.transform.position.x, 1.48f, gameObject.transform.position.z);
            //GameObject clonedTable = (GameObject)Instantiate(Resources.Load("Prefabs/TableCenter"), tableBase, gameObject.transform.rotation, gameObject.transform.parent);//layerBase.transform.parent.GetChild(0).gameObject;
        }
    }

    public static void AddForClippingBox(GameObject target)
    {
        GameObject clippingBox = GameObject.Find("ClippingBox");
        Renderer[] _renderer = target.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer childRenderer in _renderer)
            clippingBox.GetComponent<ClippingBox>().AddRenderer(childRenderer);
    }

    //public void TableDescription(GameObject table)
    //{
    //    if (isStretched && infoUpdated == 0)
    //    {
    //        timeDescription += textAdded;
    //        infoUpdated = 1;
    //    }
    //    table.transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;
    //}

    public void StretchLayer()
    {
        Invoke(nameof(ShowAllBlocks), 1);
        //DestroyLayer();
    }

    public void ShowAllBlocks()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject thisBlock = transform.GetChild(i).gameObject;
            thisBlock.GetComponent<Block>().ShowBlocks();
            //add to dictionary for copy&paste
            string[] blockArray = { thisBlock.name, thisBlock.transform.position.ToString("F4"), thisBlock.transform.rotation.ToString(),
                thisBlock.transform.localScale.ToString("F4"), thisBlock.transform.GetChild(2).GetComponent<TextMeshPro>().text,thisBlock.tag};
            listForCopy.Add(blockArray);
        }
        ShowTimeLabels();
        objectsDicForCopy[key - 1] = listForCopy;
        //undo/redo
        string[] theLayer = { gameObject.name, "true" };
        List<string[]> layerForStretch = new List<string[]>();
        layerForStretch.Add(theLayer);
        //int layerKey = ActionRecoverManager.actionID++;
        int layerKey = 0;
        if (ActionRecoverManager.actionDic != null)
            layerKey = ActionRecoverManager.actionDic.Count;
        ActionRecoverManager.actionDic.Add(layerKey, layerForStretch);
    }

    private void CreateLabels(int maxTimeCount, string tagName)
    {
        //timeLabel = transform.GetChild(7).gameObject;
        int timeCount = 0;
        while (timeCount < maxTimeCount)
        {
            GameObject thisLabel = Instantiate(timeLabel, timeLabel.transform.position, timeLabel.transform.rotation, timeLabel.transform.parent);
            int labelCount = FindObjectsOfType<Layer>().Length;
            thisLabel.tag = tagName;
            thisLabel.name = "TimeLabel_" + labelCount.ToString() + "_" + tagName + "_" + timeCount.ToString();
            UpdateText(timeCount, thisLabel);
            //thisLabel.transform.localScale = thisLabel.transform.localScale;
            thisLabel.transform.position = new Vector3(thisLabel.transform.position.x, thisLabel.transform.position.y + Block.getInstance.gapBetweenBlocks * timeCount, thisLabel.transform.position.z);
            timeCount++;

            //add label to dictionary for copy&paste
            string[] labelArray = { thisLabel.name, thisLabel.transform.position.ToString(), thisLabel.transform.rotation.ToString(),
                thisLabel.transform.localScale.ToString(), thisLabel.GetComponent<TextMeshPro>().text,thisLabel.tag};
            listForCopy.Add(labelArray);
        }
        //Destroy(timeLabel);
        timeLabel.GetComponent<Renderer>().enabled = false;
    }

    public void ShowTimeLabels()
    {
        if (timeLabel.tag == "year")
        {
            CreateLabels(12, "month");
            textAdded = " Monthly";
        }
        if (timeLabel.tag == "month")
        {
            CreateLabels(4, "week");
            textAdded = " Weekly";
        }
        if (timeLabel.tag == "week")
        {
            CreateLabels(7, "day");
            textAdded = " Daily";
        }
        if (timeLabel.tag == "day")
        {
            CreateLabels(24, "hour");
            textAdded = " Hourly";
        }
        timeDescription += textAdded;
        if(GameObject.Find(tableForDescName) != null)
            GameObject.Find(tableForDescName).transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;

        string tableInNewScene = tableForDescName + "_newScene";
        if (GameObject.Find(tableInNewScene) != null)
            GameObject.Find(tableInNewScene).transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;
    }

    private void UpdateText(int timeCount, GameObject timeLabel)
    {
        string label = "";
        if (timeLabel.tag == "month")
        {
            foreach (var value in Enum.GetValues(typeof(Month)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (timeLabel.tag == "week")
        {
            label = "Week " + (timeCount + 1);
        }
        if (timeLabel.tag == "day")
        {
            foreach (var value in Enum.GetValues(typeof(Day)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (timeLabel.tag == "hour")
        {
            if (timeCount < 12)
            {
                label = timeCount + "am - " + (timeCount + 1) + "am";
            }
            else if (timeCount == 12)
            {
                label = timeCount + "am - " + (timeCount + 1) + "pm";
            }
            else
            {
                label = timeCount + "pm - " + (timeCount + 1) + "pm";
            }
        }
        //Transform lineTooltip = transform.GetChild(7);
        timeLabel.GetComponent<TextMeshPro>().text = label;
    }

    public void DestroyLayer()
    {
        Destroy(this.gameObject);
    }
}
