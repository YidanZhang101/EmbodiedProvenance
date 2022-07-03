using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceCheck : MonoBehaviour
{
    GameObject wall;
    bool isInstantiated = false;
    bool isMonthly = false;
    bool isOnWall = false;
    GameObject minimap;

    public bool tableOnWall = false;
    // Start is called before the first frame update
    void Start()
    {
        wall = GameObject.Find("Wall");
        minimap = GameObject.Find("Mini Map (1)");
    }

    // Update is called once per frame
    void Update()
    {
    //    float dist = Vector3.Distance(wall.transform.position, gameObject.transform.GetChild(1).position);
    //    //print("Distance to other: " + dist);
    //    if (dist < 1.5f)
    //    {
    //        Transform tableCenter = gameObject.transform.GetChild(1);
    //        if (!isInstantiated)
    //        {
                
    //            //mapOnWall.transform.GetChild(0).GetComponent<TextMeshPro>().text = "2018";               

    //            //int childrenCount = 0;
    //            var children = gameObject.transform.GetComponentsInChildren<Transform>();
    //            foreach (var c in children)
    //            {
    //                if (c.parent == gameObject.transform)// & c.gameObject != mapOnWall)
    //                {
    //                    c.gameObject.SetActive(false);
    //                    //childrenCount += 1;
    //                }
    //            }
    //            float z = tableCenter.position.z;
    //            float y = tableCenter.position.y;
    //            gameObject.transform.rotation = new Quaternion();               
    //            //GameObject mapOnWall = (GameObject)
    //            Instantiate(Resources.Load("Prefabs/Map on Wall"), new Vector3(minimap.transform.position.x, y-0.3f, z + 0.3f), minimap.transform.rotation, gameObject.transform);
    //            tableOnWall = true;
    //            GameObject moveablePlane = gameObject.transform.GetChild(2).GetChild(0).GetChild(10).gameObject;
    //            GameObject targetLayer = gameObject.transform.GetChild(2).GetChild(5).gameObject;//june
    //            gameObject.transform.GetChild(0).GetChild(6).GetComponent<StretchLayer>().targetLayer = targetLayer;
    //            moveablePlane.GetComponent<ExtractLayer>().layerBase = targetLayer;//.transform.parent.GetChild(5).gameObject;
    //            moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.rotation = targetLayer.transform.rotation;
    //            moveablePlane.GetComponent<ExtractLayer>().selectedLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = targetLayer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
    //            //if (childrenCount == 3)
    //            //{
    //            //    mapOnWall.transform.GetChild(0).GetComponent<TextMeshPro>().text += " Monthly";
    //            //    print("ChildrenCount: " + childrenCount);
    //            //}
    //            //else
    //            //{
    //            //    mapOnWall.transform.GetChild(0).GetComponent<TextMeshPro>().text = "2018";
    //            //}
    //            //for (int i = 0; i < childrenCount - 1; i++)
    //            //{
    //            //    gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //            //} 
    //            isInstantiated = true;
    //        }
    //        GameObject mapOnWall = gameObject.transform.GetChild(3).gameObject;
    //        int backCount = mapOnWall.GetComponent<BuildingInteractor>().tableBackCount;
    //        if (backCount == 1 & !isOnWall)
    //        {
    //            gameObject.transform.GetChild(1).gameObject.SetActive(false);
    //            gameObject.transform.GetChild(2).gameObject.SetActive(false);
    //            mapOnWall.SetActive(true);
    //            mapOnWall.transform.GetChild(10).gameObject.SetActive(true);
    //            float z = tableCenter.position.z;
    //            float y = tableCenter.position.y;
    //            mapOnWall.transform.position = new Vector3(minimap.transform.position.x, y - 0.3f, z + 0.3f);
    //            mapOnWall.transform.rotation = minimap.transform.rotation;
    //            isOnWall = true;
    //        }
    //    }
    }
}
