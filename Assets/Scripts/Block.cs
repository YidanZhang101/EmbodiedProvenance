using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Month { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec}
public enum Week { Week1, Week2, Week3, Week4}
public enum Day { Mon, Tue, Wed, Thu, Fri, Sat, Sun}

public class Block : MonoBehaviour
{
    public GameObject theBlock;
    public float gapBetweenBlocks = 0.04f;
    int grabCount = 0;
    bool pedestalAdded = false;
    //Vector3 centerPos;
    GameObject block;
    ActionRecover actionRecover = ActionRecover.getinstance;

    private static Block instance;
    public static Block getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new Block();
            }
            return instance;
        }
    }

    void Start()
    {
        //centerPos = theBlock.transform.GetChild(0).position;
        //if (theBlock.transform.parent != null) //.gameObject.tag == "extractedLayer")
        //{
        //    Transform lineTooltip = theBlock.transform.GetChild(2);
        //    lineTooltip.gameObject.GetComponent<TextMeshPro>().text = theBlock.transform.parent.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void IntantiateBlock(GameObject newBlock)
    //{
    //    if (theBlock.transform.parent != null)
    //    {
    //        newBlock = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent.parent);
    //    }
    //    else
    //    {
    //        newBlock = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent);
    //    }
    //}

    public void DuplicateBlock()
    {
        if (grabCount == 0)
        {
            block = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent);
            //IntantiateBlock(block);
            block.transform.localScale = gameObject.transform.localScale;
            grabCount++;
            //actionRecover.test();
            
            //actionRecover.LogActionData(block.name, Enum.GetName(typeof(ActionRecover.LogTargetType), ActionRecover.LogTargetType.Block),
            //    Enum.GetName(typeof(ActionRecover.ActionType), ActionRecover.ActionType.Create), Enum.GetName(typeof(ActionRecover.LogStatus), ActionRecover.LogStatus.Start), block.transform.position);
        }
    }

    private void UpdateText(int timeCount, GameObject block)
    {
        string label = "";
        if (block.tag == "month")
        {
            foreach (var value in Enum.GetValues(typeof(Month)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (block.tag == "week")
        {
            label = "Week " + (timeCount+1);
        }
        if (block.tag == "day")
        {
            foreach (var value in Enum.GetValues(typeof(Day)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (block.tag == "hour")
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
        Transform lineTooltip = block.transform.GetChild(2);
        lineTooltip.gameObject.GetComponent<TextMeshPro>().text = label;
    }

    private void CreateBlocks(int maxTimeCount, string tagName)
    {
        int timeCount = 0;
        while (timeCount < maxTimeCount)
        {
            GameObject thisBlock = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent);
            //GameObject thisBlock = new GameObject();
            //IntantiateBlock(thisBlock);
            thisBlock.tag = tagName;
            UpdateText(timeCount, thisBlock);
            thisBlock.transform.localScale = theBlock.transform.localScale;
            thisBlock.transform.position = new Vector3(theBlock.transform.position.x, theBlock.transform.position.y + gapBetweenBlocks * timeCount, theBlock.transform.position.z);
            timeCount++;          
        }
        DestroyBlock();
    }
    public void StretchBlock()
    {
        Invoke(nameof(ShowBlocks), 1);
        grabCount = 0;
    }

    public void ShowBlocks()
    {       
        if (theBlock.tag == "year")
        {
            CreateBlocks(12, "month");
        }
        if (theBlock.tag == "month")
        {
            CreateBlocks(4, "week");
        }
        if (theBlock.tag == "week")
        {
            CreateBlocks(7, "day");
        }
        if (theBlock.tag == "day")
        {
            CreateBlocks(24, "hour");
        }
    }

    public void DestroyBlock()
    {
        Destroy(this.gameObject);
    }

    public void SetColour()
    {

    }

    public void AddPedestal()
    {
        if (!pedestalAdded)
        {
            GameObject clonePedestal = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal"));
            //clonePedestal.GetComponent<RemovePedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            //clonePedestal.tag = "pedestal";
            //clonePedestal.transform.parent = gameObject.transform.parent;
            clonePedestal.transform.position = theBlock.transform.GetChild(0).position;
            pedestalAdded = true;
            GameObject line = (GameObject)Instantiate(Resources.Load("Prefabs/Line"));
            //line.AddComponent<MatchPedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            //line.transform.parent = gameObject.transform.parent;
            line.transform.GetChild(0).position = block.transform.GetChild(0).position;
            line.transform.GetChild(1).position = gameObject.transform.GetChild(0).position;

            //ActionRecover.getinstance.LogActionData(clonePedestal.name, Enum.GetName(typeof(ActionRecover.LogTargetType), ActionRecover.LogTargetType.Pedestal),
            //    Enum.GetName(typeof(ActionRecover.ActionType), ActionRecover.ActionType.Create), Enum.GetName(typeof(ActionRecover.LogStatus), ActionRecover.LogStatus.Start), clonePedestal.transform.position);
            //ActionRecover.getinstance.LogActionData(line.name, Enum.GetName(typeof(ActionRecover.LogTargetType), ActionRecover.LogTargetType.Line),
            //    Enum.GetName(typeof(ActionRecover.ActionType), ActionRecover.ActionType.Create), Enum.GetName(typeof(ActionRecover.LogStatus), ActionRecover.LogStatus.Start), line.transform.position);
        }
    }
}
