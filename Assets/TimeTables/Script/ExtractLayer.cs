using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;

public class ExtractLayer : MonoBehaviour
{
    public GameObject selectedLayer;
    public GameObject layerBase;
    //public Vector3 tablePos;

    string[] year = new string[] { "2019", "2020" };
    string[] month = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    string[] week = new string[] { "Week 1", "Week 2", "Week 3", "Week 4" };
    string[] day = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};

    GameObject tableContainer;
    Vector3 layerFinalPos;
    Quaternion layerFinalRot;
    Transform allTables;
    Vector3 originP;
    float timePressed;
    float durationThreshold = 2f;
    bool isPointerDown = false;
    bool longPressTriggered = false;
    int grabCount = 0;
    bool tableAdded = false;
    //List<Vector3> itemPos;

    // Start is called before the first frame update
    void Start()
    {
        originP = layerBase.transform.position;
        //GameObject[] layerItems = selectedLayer.GetComponentsInChildren<GameObject>();
        //itemPos = new List<Vector3>();
        //foreach (GameObject item in layerItems)
        //{
        //    itemPos.Add(item.transform.position);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (isPointerDown & !longPressTriggered)// & isOrigin)
        {
            if (Time.time - timePressed > durationThreshold)
            {
                longPressTriggered = true;
                GameObject clone = Instantiate(selectedLayer);
                allTables = GameObject.Find("Tabletop").transform;//layerBase.transform.parent;
                tableContainer = new GameObject("TableContainer");
                AddComponents(tableContainer);
                string label = clone.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
                AddTag(year, label, tableContainer, "Monthly_L");
                AddTag(month, label, tableContainer, "Weekly_L");
                AddTag(week, label, tableContainer, "Daily_L");
                AddTag(day, label, tableContainer, "Hourly_L");
                SyncCounsumption(clone);
                tableContainer.transform.parent = allTables;
                clone.transform.parent = tableContainer.transform;
                clone.transform.position = originP;
                clone.SetActive(true);
                if (gameObject.transform.parent.gameObject != GameObject.Find("Tabletop"))// & gameObject.transform.parent.parent.parent.gameObject.GetComponent<DistanceCheck>().tableOnWall)
                {
                    Transform june = gameObject.transform.parent.parent.GetChild(5);
                    //Transform container = gameObject.transform.parent.parent.parent;
                    clone.transform.position = june.position;// new Vector3(june.position.x + container.position.x, june.position.y, june.position.z + container.position.z);// targetLayer.transform.position;
                }
                //tablePos = tableContainer.transform.position;
                //selectedLayer.SetActive(false);
                //if (grabCount == 0)
                //{
                //    GameObject clone = Instantiate(selectedLayer);
                //    clone.transform.position = originP;
                //    grabCount++;
                //    //extract the whole layer
                //}
            }
        }

        //if(tableAdded)
        //{
        //    gameObject.transform.position = layerFinalPos;
        //    gameObject.transform.rotation = layerFinalRot;
        //}           
    }

    //public void SetLayer(int ChildNo)
    //{
    //    GameObject targetLayer = gameObject.transform.parent.parent.GetChild(ChildNo).gameObject;
    //    layerBase = targetLayer;//.transform.parent.GetChild(5).gameObject;
    //    selectedLayer.transform.rotation = targetLayer.transform.rotation;
    //    selectedLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = targetLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
    //}

    public void AddTag(string[] time, string label, GameObject tableForTag, string timeTag)
    {
        int pos = Array.IndexOf(time, label);
        if (pos > -1)
        {
            tableForTag.tag = timeTag;
        }
    }

    public void SyncCounsumption(GameObject clone)
    {
        if (gameObject.tag == "moveablePlane")
        {
            GameObject targetLayer = gameObject.transform.parent.parent.parent.GetChild(0).GetChild(6).GetComponent<StretchLayer>().targetLayer;
            clone.transform.GetChild(0).gameObject.GetComponent<BuildingInteractor>().consumption = targetLayer.transform.GetChild(0).gameObject.GetComponent<BuildingInteractor>().consumption;
            clone.transform.GetChild(1).gameObject.GetComponent<BuildingInteractor>().consumption = targetLayer.transform.GetChild(1).gameObject.GetComponent<BuildingInteractor>().consumption;
            clone.transform.GetChild(2).gameObject.GetComponent<BuildingInteractor>().consumption = targetLayer.transform.GetChild(2).gameObject.GetComponent<BuildingInteractor>().consumption;
            clone.transform.GetChild(3).gameObject.GetComponent<BuildingInteractor>().consumption = targetLayer.transform.GetChild(4).gameObject.GetComponent<BuildingInteractor>().consumption;
            clone.transform.GetChild(4).gameObject.GetComponent<BuildingInteractor>().consumption = targetLayer.transform.GetChild(5).gameObject.GetComponent<BuildingInteractor>().consumption;
        }
    }

    public void AddComponents(GameObject tableContainer)
    {
        tableContainer.AddComponent<NearInteractionGrabbable>();
        BoxCollider tableRi = tableContainer.AddComponent<BoxCollider>();
        tableRi.isTrigger = true;
        //tableContainer.AddComponent<Rigidbody>();
        tableContainer.AddComponent<ObjectManipulator>();
        //tableContainer.AddComponent<DistanceCheck>();      
    }

    public void OnPointerDown()
    {
        timePressed = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
        //if (grabCount == 0)
        //{
        //    GameObject clone = Instantiate(selectedLayer);
        //    clone.transform.position = originP;
        //    GameObject[] clonedItems = clone.GetComponentsInChildren<GameObject>();
        //    for (int i = 0; i < clonedItems.Length; i++)
        //    {
        //        clonedItems[i].transform.position = itemPos[i];
        //    }
        //    grabCount++;
        //    extract the whole layer
        //}
    }

    public void OnPointerUpOrExit()
    {
        isPointerDown = false;
    }

    public void CountGrab()
    {
        grabCount += 1;
    }

    public void AddTableforLayer()
    {
        if (grabCount == 1)
        {
            layerFinalPos = new Vector3(gameObject.transform.position.x, 1.58f);
            layerFinalRot = gameObject.transform.rotation;
            gameObject.transform.position = layerFinalPos;
            Vector3 tableBase = new Vector3(gameObject.transform.position.x, 1.48f, gameObject.transform.position.z);
            GameObject clonedTable = (GameObject)Instantiate(Resources.Load("Prefabs/TableCenter"), tableBase, gameObject.transform.rotation, gameObject.transform.parent);//layerBase.transform.parent.GetChild(0).gameObject;
            //GameObject clonedTable = Instantiate(tableBase, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            //clonedTable.transform.parent = gameObject.transform;
            //clonedTable.transform.rotation = new Quaternion(gameObject.transform.rotation.x,gameObject.transform.rotation.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
            //clonedTable.transform.localPosition = gameObject.GetComponent<Renderer>().bounds.center;
            tableAdded = true;
        }
    }
}
