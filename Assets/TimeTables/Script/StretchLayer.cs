using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class StretchLayer : MonoBehaviour
{
    public GameObject building;
    public Texture2D colorRamp;
    public GameObject targetLayer;
    Vector3 originP;
    Vector3 thisPos;
    float thisY;
    bool objectCreated = false;

    GameObject monthlyLayer;
    GameObject moveablePlane;
    GameObject currentTimeLevel;

    string[] year = new string[] { "2019", "2020" };
    string[] month = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    string[] week = new string[] { "Week 1", "Week 2", "Week 3", "Week 4" };
    string[] day = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    string[] hour = new string[] { "0am - 1am", "1am - 2am", "2am - 3am", "3am - 4am", "4am - 5am", "5am - 6am", "6am - 7am", "7am - 8am", "8am - 9am", "9am - 10am", "10am - 11am", "11am - 12pm", "12pm - 13pm", "13pm - 14pm", "14pm - 15pm", "15pm - 16pm", "16pm - 17pm", "17pm - 18pm", "18pm - 19pm", "19pm - 20pm", "20pm - 21pm", "21pm - 22pm", "22pm - 23pm", "23pm - 24pm" };
    //public int timeLevelCount;

    //public float test = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        thisY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        originP = building.transform.position;
        thisPos = gameObject.transform.position;
    }

    //public void stretchBuilding_above()
    //{
    //    if (thisPos.y > 27.6f)
    //        clone(0.05f, 1);

    //}

    public void stretchBuilding_above()
    {
        Invoke(nameof(showAbove), 1);
        gameObject.transform.parent.parent.gameObject.GetComponent<BuildingInteractor>().grabCount = 0;
    }

    public void stretchLayer()
    {
        Invoke(nameof(showLayer), 1);
    }
    
    public string getTimeLabel(int i)
    {
        string label = "";
        if (CheckTimeLevel(month))
        {
            label = "Week " + i;
        }
        else if (CheckTimeLevel(week))
        {
            switch (i)
            {
                case 1:
                    label = "Monday";
                    break;
                case 2:
                    label = "Tuesday";
                    break;
                case 3:
                    label = "Wednesday";
                    break;
                case 4:
                    label = "Thursday";
                    break;
                case 5:
                    label = "Friday";
                    break;
                case 6:
                    label = "Saturday";
                    break;
                case 7:
                    label = "Sunday";
                    break;
            }
        }
        else if (CheckTimeLevel(day))//(gameObject.transform.parent.parent.parent.gameObject.tag == "Daily")
        {
            if (i < 12)
            {
                label = i-1 + "am - " + i + "am";
            }
            else if(i == 12)
            {
                label = i - 1 + "am - " + i + "pm";
            }
            else
            {
                label = i-1 + "pm - " + i + "pm";
            }
        }
        else
        {
            switch (i)
            {
                case 1:
                    label = "January";
                    break;
                case 2:
                    label = "February";
                    break;
                case 3:
                    label = "March";
                    break;
                case 4:
                    label = "April";
                    break;
                case 5:
                    label = "May";
                    break;
                case 6:
                    label = "June";
                    break;
                case 7:
                    label = "July";
                    break;
                case 8:
                    label = "August";
                    break;
                case 9:
                    label = "September";
                    break;
                case 10:
                    label = "October";
                    break;
                case 11:
                    label = "November";
                    break;
                case 12:
                    label = "December";
                    break;
            }
        }            
        return label;
    }

    public void SetConsumption_Monthly(string month, GameObject block)
    {
        if (month == "January")
            block.GetComponent<BuildingInteractor>().consumption = 20;
        if (month == "February")
            block.GetComponent<BuildingInteractor>().consumption = 50;
        if (month == "March")
            block.GetComponent<BuildingInteractor>().consumption = 60;
        if (month == "April")
            block.GetComponent<BuildingInteractor>().consumption = 45;
        if (month == "May")
            block.GetComponent<BuildingInteractor>().consumption = 70;
        if (month == "June")
            block.GetComponent<BuildingInteractor>().consumption = 30;
        if (month == "July")
            block.GetComponent<BuildingInteractor>().consumption = 40;
        if (month == "August")
            block.GetComponent<BuildingInteractor>().consumption = 60;
        if (month == "September")
            block.GetComponent<BuildingInteractor>().consumption = 80;
        if (month == "October")
            block.GetComponent<BuildingInteractor>().consumption = 70;
        if (month == "November")
            block.GetComponent<BuildingInteractor>().consumption = 40;
        if (month == "December")
            block.GetComponent<BuildingInteractor>().consumption = 20;
    }

    public void SetConsumption_Yearly(string timeLabel, GameObject block)
    {
        if(block.tag == "92")
        {
            if(timeLabel == "2019")
                block.GetComponent<BuildingInteractor>().consumption = 80;
            if (timeLabel == "2020")
                block.GetComponent<BuildingInteractor>().consumption = 50;
        }
        if (block.tag == "Sport")
        {
            if (timeLabel == "2019")
                block.GetComponent<BuildingInteractor>().consumption = 70;
            if (timeLabel == "2020")
                block.GetComponent<BuildingInteractor>().consumption = 40;
        }
    }

    public void SetConsumption(GameObject block, string[] listToAdd, int[] usage_92,int[] usage_sport)
    {
        //if (timeLabel == "Week 1")
        //    block.GetComponent<BuildingInteractor>().consumption = 30;
        //if (timeLabel == "Week 2")
        //    block.GetComponent<BuildingInteractor>().consumption = 50;
        //if (timeLabel == "Week 3")
        //    block.GetComponent<BuildingInteractor>().consumption = 60;
        //if (timeLabel == "Week 4")
        //    block.GetComponent<BuildingInteractor>().consumption = 70;
        //int[] usage = new int[] { 30,50,60,70 };
        //if (CheckTimeLevel(listToCheck))
        //{
        string timeLabel = block.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text;
        for (int i = 0; i < listToAdd.Length; i++)
        {
            if (timeLabel == listToAdd[i] & block.tag == "92")
                block.GetComponent<BuildingInteractor>().consumption = usage_92[i];
            if (timeLabel == listToAdd[i] & block.tag == "Sport")
                block.GetComponent<BuildingInteractor>().consumption = usage_sport[i];
        }
        //}                  
    }

    public Color GetColor(int value)
    {
        //normalise the value
        float normalisedValue = value / 100f;

        //interpolate pixel width
        int w = (int)Mathf.Lerp(0, 1000, normalisedValue);

        //get pixel color
        Color color = colorRamp.GetPixel(w, 0);
        return color;
    }

    public void ChangeColor(GameObject block)
    {
        int consumption = block.GetComponent<BuildingInteractor>().consumption;
        block.GetComponent<Renderer>().material.color = GetColor(consumption);
    }

    public void clone(float num, int i)
    {
        GameObject clone = Instantiate(building);//, transform.position, transform.rotation);
        Transform parent = gameObject.transform.parent;
        Transform buildingLevel = parent.parent;
        Transform layerLevel = buildingLevel.parent;
        Transform monthly = layerLevel.GetChild(6);
        monthly.gameObject.GetComponent<MatchPedestalContent>().timeForPedestal = gameObject.transform.parent.parent.GetChild(2).GetComponent<TextMeshPro>().text;
        clone.transform.parent = monthly;
        clone.transform.position = new Vector3(originP.x, originP.y + num * i, originP.z);
        //GameObject.Find("Simple Line ToolTip").SendMessage("ToolTipText");
        Transform lineTooltip = clone.transform.GetChild(2);
        lineTooltip.gameObject.GetComponent<TextMeshPro>().text = getTimeLabel(i+1);

        //Color red = new Color(0.7264151f, 0.06461674f, 0.05139729f);
        //Color yellow = new Color(0.9333333f, 0.6410894f, 0.06666663f);
        //Color[] colors = new Color[] { red, yellow };
        //if(i >5 & i <10 || i <3 || i == 4)
        //    clone.GetComponent<Renderer>().material.color = colors[0];
        //if (i == 3 || i==1 || i==11)
        //    clone.GetComponent<Renderer>().material.color = colors[1];

    }

    public void cloneTimeBelowMonth(float num, int i, string timeLevel)
    {
        if(i == 0)
        {
            GameObject monthly = GameObject.FindGameObjectWithTag("Monthly"); //gameObject.transform.parent.parent.parent.GetChild(6).gameObject;// 
            currentTimeLevel = new GameObject(timeLevel);
            string timeLabel = gameObject.transform.parent.parent.GetChild(2).GetComponent<TextMeshPro>().text;
            currentTimeLevel.AddComponent<MatchPedestalContent>().timeForPedestal = timeLabel;
            //timeLevelCount = GameObject.FindGameObjectsWithTag("timeLabel").Length;
            currentTimeLevel.tag = timeLevel;
            //if (timeLevel == "Weekly")
            //    currentTimeLevel.transform.parent = monthly.transform;
            //if (timeLevel == "Daily")
            //{
            //    GameObject weekly = GameObject.FindGameObjectWithTag("Weekly");
            //    currentTimeLevel.transform.parent = weekly.transform;
            //}
            if (gameObject.transform.parent.parent.parent.parent.parent.gameObject == GameObject.Find("Tabletop"))
            {
                currentTimeLevel.transform.parent = gameObject.transform.parent.parent.parent.parent; //monthly.transform.parent;// 
                currentTimeLevel.transform.localScale = monthly.transform.localScale;
            }               
            else
                currentTimeLevel.transform.parent = gameObject.transform.parent.parent.parent;
            currentTimeLevel.transform.position = monthly.transform.position;
            currentTimeLevel.transform.rotation = monthly.transform.rotation;
            
        }
        GameObject clone = Instantiate(building);
        clone.transform.parent = currentTimeLevel.transform;
        clone.transform.position = new Vector3(originP.x, originP.y + num * i, originP.z);
        clone.transform.rotation = building.transform.rotation;
        clone.transform.localScale = building.transform.localScale;
        Transform lineTooltip = clone.transform.GetChild(2);
        lineTooltip.gameObject.GetComponent<TextMeshPro>().text = getTimeLabel(i + 1);

        Color red = new Color(0.7264151f, 0.06461674f, 0.05139729f);
        Color yellow = new Color(0.9333333f, 0.6410894f, 0.06666663f);
        Color[] colors = new Color[] { red, yellow };
        if (i >0 & i < 4)
            clone.GetComponent<Renderer>().material.color = colors[0];
        if (i == 5 || i == 6)
            clone.GetComponent<Renderer>().material.color = colors[1];
    }

    public void cloneLayer(float num,int i)
    {
        GameObject nonMovableLayer = GameObject.Find("Layer1");
        GameObject targetLayer = gameObject.transform.parent.gameObject;//.GetChild(10).GetChild(0).gameObject;
        GameObject clonedLayer = Instantiate(nonMovableLayer);
        GameObject quad = targetLayer.transform.GetChild(5).gameObject;
        if (!objectCreated)
        {
            monthlyLayer = new GameObject("LayerMonthly");
            monthlyLayer.AddComponent<ParentTimeLevel>();
            monthlyLayer.GetComponent<ParentTimeLevel>().parentTimeLevel = gameObject.transform.parent.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
            moveablePlane = (GameObject)Instantiate(Resources.Load("Prefabs/MovablePlane"), clonedLayer.transform);
            moveablePlane.tag = "moveablePlane";
            moveablePlane.transform.position = new Vector3(clonedLayer.transform.GetChild(6).position.x, clonedLayer.transform.GetChild(6).position.y, clonedLayer.transform.GetChild(0).position.z);
            moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.position = clonedLayer.transform.position; 
            //moveablePlane.transform.rotation = quad.transform.rotation;
            objectCreated = true;
            //clonedLayer.transform.GetChild(5).gameObject.SetActive(true);
        }      
        //quad.SetActive(false);
        monthlyLayer.transform.parent = gameObject.transform.parent.parent;
        clonedLayer.transform.parent = monthlyLayer.transform;
        clonedLayer.transform.position = new Vector3(targetLayer.transform.position.x, targetLayer.transform.position.y + num * i, targetLayer.transform.position.z);
        clonedLayer.transform.rotation = targetLayer.transform.rotation;
        Transform timeLabel = clonedLayer.transform.GetChild(7);//.GetChild(0);
        timeLabel.gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        clonedLayer.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        clonedLayer.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        clonedLayer.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        clonedLayer.transform.GetChild(4).GetChild(2).gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        clonedLayer.transform.GetChild(5).GetChild(2).gameObject.GetComponent<TextMeshPro>().text = getTimeLabel_Layer(i + 1);
        //clonedLayer.transform.GetChild(6).GetChild(0).gameObject.SetActive(false);
    }

    public bool CheckTimeLevel(string[] time)
    {
        int pos = Array.IndexOf(time, gameObject.transform.parent.parent.GetChild(2).GetComponent<TextMeshPro>().text);
        if (pos > -1)
        {
            return true;
        }
        return false;
    }

    public void showAbove()
    {
        int i = 0;
        int interationNum = 12;
        if (CheckTimeLevel(month))//(gameObject.transform.parent.parent.parent.gameObject.tag == "Monthly")
            interationNum = 4;
        if (CheckTimeLevel(week))
            interationNum = 7;
        if (CheckTimeLevel(day))
            interationNum = 24;
        while (i < interationNum)
        {
            if(gameObject.tag == "Building")
            {
                if(CheckTimeLevel(year))
                {
                    clone(0.04f, i);
                    //Transform parent = gameObject.transform.parent;
                    //Transform buildingLevel = parent.parent;
                    //Transform layerLevel = buildingLevel.parent;
                    //Transform monthly = layerLevel.GetChild(6);
                    //int monthlyChildren = 0;
                    //foreach (Transform c in monthly.GetComponentsInChildren<Transform>())
                    //{
                    //    if (c.parent == monthly)
                    //        monthlyChildren += 1;
                    //}
                    //for (int n = 0; n < monthlyChildren; n++)
                    //{
                    //    SetConsumption(monthly.GetChild(n).gameObject, month, new int[] { 20, 50, 60, 45, 70, 30, 40, 60, 80, 70, 40, 20 }, new int[] { 70, 50, 60, 45, 70, 30, 40, 60, 80, 70, 40, 20 });// new int[] { 80, 50 }, new int[] { 70, 40 });                                                                                                                                                                                             //SetConsumption(timeLabel, clone, month, new int[] { 20,50,60,45,70,30,40,60,80,70,40,20 }, new int[] { 70, 50, 60, 45, 70, 30, 40, 60, 80, 70, 40, 20 });
                    //    ChangeColor(monthly.GetChild(n).gameObject);
                    //}                   
                }
                    
                if (CheckTimeLevel(month))
                    cloneTimeBelowMonth(0.04f, i, "Weekly");
                if (CheckTimeLevel(week))
                    cloneTimeBelowMonth(0.04f, i, "Daily");
                if (CheckTimeLevel(day))
                    cloneTimeBelowMonth(0.04f, i, "Hourly");
                //if (gameObject.transform.parent.parent.parent.gameObject.tag == "Monthly")
                //    cloneTimeBelowMonth(0.04f, i, "Weekly");
                //else if (gameObject.transform.parent.parent.parent.gameObject.tag == "Weekly")
                //    cloneTimeBelowMonth(0.04f, i, "Daily");
                //else if (gameObject.transform.parent.parent.parent.gameObject.tag == "Daily")
                //    cloneTimeBelowMonth(0.04f, i, "Hourly");
                //else
                //    clone(0.04f, i);
            }
            i++;
        }
        if (gameObject.tag == "Building")
        {
            building.SetActive(false);
            //GameObject singleBuildingContainer = new GameObject();
            //singleBuildingContainer.transform.parent = GameObject.Find("Monthly").transform;
        }          
        //if (gameObject.tag == "Layer")
        //{
        //    GameObject june = gameObject.transform.parent.parent.GetChild(2).GetChild(5).gameObject;
        //    moveablePlane.GetComponent<ExtractLayer>().layerBase = june;//.transform.parent.GetChild(5).gameObject;
        //    moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.rotation = june.transform.rotation;
        //    gameObject.transform.parent.gameObject.SetActive(false);
        //}          
    }

    public void showLayer()
    {
        int i = 0;
        int interationNum = 12;
        if (gameObject.transform.parent.parent.gameObject.tag == "Weekly_L")
            interationNum = 4;
        if (gameObject.transform.parent.parent.gameObject.tag == "Daily_L")
            interationNum = 7;
        if (gameObject.transform.parent.parent.gameObject.tag == "Hourly_L")
            interationNum = 24;
        while (i < interationNum)
        {
            cloneLayer(0.05f, i);
            i++;
        }
        if (gameObject.tag == "Layer")
        {
            bool tableIsBack = gameObject.transform.parent.parent.gameObject.GetComponent<DistanceCheck>().tableOnWall;
            print("tableisBack: " + tableIsBack);
            if (!tableIsBack)
                SetLayer(1);//february
            else
                SetLayer(5);//june
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
    public void SetLayer(int ChildNo)
    {
        targetLayer = gameObject.transform.parent.parent.GetChild(2).GetChild(ChildNo).gameObject;
        moveablePlane.GetComponent<ExtractLayer>().layerBase = targetLayer;//.transform.parent.GetChild(5).gameObject;
        moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.rotation = targetLayer.transform.rotation;
        moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = targetLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
    }
    public string getTimeLabel_Layer(int i)
    {
        string label = "";
        if (gameObject.transform.parent.parent.gameObject.tag == "Weekly_L")
        {
            label = "Week " + i;
        }
        else if (gameObject.transform.parent.parent.gameObject.tag == "Daily_L")
        {
            switch (i)
            {
                case 1:
                    label = "Monday";
                    break;
                case 2:
                    label = "Tuesday";
                    break;
                case 3:
                    label = "Wednesday";
                    break;
                case 4:
                    label = "Thursday";
                    break;
                case 5:
                    label = "Friday";
                    break;
                case 6:
                    label = "Saturday";
                    break;
                case 7:
                    label = "Sunday";
                    break;
            }
        }
        else if (gameObject.transform.parent.parent.gameObject.tag == "Hourly_L")
        {
            if (i < 12)
            {
                label = i - 1 + "am - " + i + "am";
            }
            else if (i == 12)
            {
                label = i - 1 + "am - " + i + "pm";
            }
            else
            {
                label = i - 1 + "pm - " + i + "pm";
            }
        }
        else
        {
            switch (i)
            {
                case 1:
                    label = "January";
                    break;
                case 2:
                    label = "February";
                    break;
                case 3:
                    label = "March";
                    break;
                case 4:
                    label = "April";
                    break;
                case 5:
                    label = "May";
                    break;
                case 6:
                    label = "June";
                    break;
                case 7:
                    label = "July";
                    break;
                case 8:
                    label = "August";
                    break;
                case 9:
                    label = "September";
                    break;
                case 10:
                    label = "October";
                    break;
                case 11:
                    label = "November";
                    break;
                case 12:
                    label = "December";
                    break;
            }
        }
        return label;
    }
}
