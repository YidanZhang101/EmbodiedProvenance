using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public GameObject timeLabel;

    GameObject newTable;
    int grabCount = 0;
    string timeDescription;
    string textAdded;

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
            newTable = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
            newTable.transform.position = new Vector3(newTable.transform.position.x + Layer.getInstance.add_x * tableCounter, newTable.transform.position.y, newTable.transform.position.z);            
            transform.position = new Vector3(base_layer.transform.position.x + 0.016f + Layer.getInstance.add_x * tableCounter, base_layer.transform.position.y, base_layer.transform.position.z);
            transform.rotation = new Quaternion();
            timeDescription = timeLabel.GetComponent<TextMeshPro>().text;
            newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;
            //layerFinalPos = new Vector3(gameObject.transform.position.x, 1.58f);
            //layerFinalRot = gameObject.transform.rotation;
            //gameObject.transform.position = layerFinalPos;
            //Vector3 tableBase = new Vector3(gameObject.transform.position.x, 1.48f, gameObject.transform.position.z);
            //GameObject clonedTable = (GameObject)Instantiate(Resources.Load("Prefabs/TableCenter"), tableBase, gameObject.transform.rotation, gameObject.transform.parent);//layerBase.transform.parent.GetChild(0).gameObject;
        }
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
        }
        ShowTimeLabels();
    }

    private void CreateLabels(int maxTimeCount, string tagName)
    {
        //timeLabel = transform.GetChild(7).gameObject;
        int timeCount = 0;
        while (timeCount < maxTimeCount)
        {
            GameObject thisLabel = Instantiate(timeLabel, timeLabel.transform.position, timeLabel.transform.rotation, timeLabel.transform.parent);
            thisLabel.tag = tagName;
            UpdateText(timeCount, thisLabel);
            //thisLabel.transform.localScale = thisLabel.transform.localScale;
            thisLabel.transform.position = new Vector3(thisLabel.transform.position.x, thisLabel.transform.position.y + Block.getInstance.gapBetweenBlocks * timeCount, thisLabel.transform.position.z);
            timeCount++;
        }
        Destroy(timeLabel);
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
        newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = timeDescription;
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
