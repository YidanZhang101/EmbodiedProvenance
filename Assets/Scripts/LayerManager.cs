using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public GameObject timeLabel;

    public static Dictionary<int, List<string[]>> objectsDicForCopy = new Dictionary<int, List<string[]>>();
    public int key = 1;
    List<string[]> listForCopy = new List<string[]>();

    GameObject newTable;
    int grabCount = 0;
    string timeDescription;
    string textAdded;
    string tableForDescName;

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
            if (tableCount <= 13)
            {
                newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * tableCounter, newTable.transform.position.y, newTable.transform.position.z);
                transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * tableCounter, base_layer.transform.position.y, base_layer.transform.position.z);
            }
            else if(tableCount > 13 && tableCount < 28)
            {
                newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * (27 - tableCount), newTable.transform.position.y, newTable.transform.position.z - 2.5f);
                transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * (27 - tableCount), base_layer.transform.position.y, base_layer.transform.position.z - 2.5f);
            }
            transform.rotation = new Quaternion();
            timeDescription = timeLabel.GetComponent<TextMeshPro>().text;
            newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;

            //add table in Timeline
            GameObject yearlyTable = GameObject.Find("Yearly_Table");
            GameObject timelineTable = (GameObject)Instantiate(Resources.Load("Prefabs/Timeline_Table"));
            timelineTable.name = "Timeline_Table_" + tableCounter;
            timelineTable.transform.parent = yearlyTable.transform.parent;
            timelineTable.transform.position = new Vector3(yearlyTable.transform.position.x + 0.17f * tableCounter, yearlyTable.transform.position.y, yearlyTable.transform.position.z);
            timelineTable.transform.rotation = yearlyTable.transform.rotation;
            timelineTable.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = timeDescription;
            AddForClippingBox(timelineTable);

            //add to dictionary for copy&paste
            string[] tableArray = {newTable.name, newTable.transform.position.ToString("F4"), newTable.transform.rotation.ToString(), newTable.transform.localScale.ToString("F4"), timeDescription};
            string[] layerArray = {gameObject.name, gameObject.transform.position.ToString("F4"), gameObject.transform.rotation.ToString(), gameObject.transform.localScale.ToString("F4"), timeDescription };
            listForCopy.Add(tableArray);
            listForCopy.Add(layerArray);
            key = GameObject.FindGameObjectsWithTag("tabletop").Length - 1;
            objectsDicForCopy.Add(key, listForCopy);

            //Add to dictionary for Undo/Redo
            int ActionKey = ActionRecoverManager.actionID++;
            string[] newTableForAction = { newTable.name, "true" };

            List<string[]> listForAction = new List<string[]>();
            listForAction.Add(newTableForAction);
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
                thisBlock.transform.localScale.ToString("F4"), thisBlock.transform.GetChild(2).GetComponent<TextMeshPro>().text};
            listForCopy.Add(blockArray);
        }
        ShowTimeLabels();
        objectsDicForCopy[key - 1] = listForCopy;
        //undo/redo
        string[] theLayer = { gameObject.name, "true" };
        List<string[]> layerForStretch = new List<string[]>();
        layerForStretch.Add(theLayer);
        int layerKey = ActionRecoverManager.actionID++;
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
                thisLabel.transform.localScale.ToString(), thisLabel.GetComponent<TextMeshPro>().text};
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
        }
        if (timeLabel.tag == "day")
        {
            CreateLabels(24, "hour");
        }
        timeDescription += textAdded;
        GameObject.Find(tableForDescName).transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;
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
