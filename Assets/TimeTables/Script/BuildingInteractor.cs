using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.SceneManagement;

public class BuildingInteractor : MonoBehaviour
{   
    public int grabCount = 0;
    Vector3 originP;
    Quaternion originR;
    Vector3 centerPos;
    bool isOrigin = false;
    GameObject cloneTable;
    bool pedestalAdded = false;

    public GameObject table;
    public float xNew;
    public int consumption;
    public int tableBackCount = 0;
    //public Transform tabletop;
    //public Transform largemap;

    // Start is called before the first frame update
    void Start()
    {
        originP = gameObject.transform.position;
        originR = gameObject.transform.rotation;
        centerPos = gameObject.transform.GetChild(0).position;
        //GameObject parentObject = GameObject.Find("Layer1");
        //if (gameObject.transform.parent == parentObject)
        //    isOrigin = true;

        //layerPos = selectedLayer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();

    //private struct PointerData
    //{
    //    public IMixedRealityPointer pointer;
    //    private Vector3 initialGrabPointInPointer;

    //    public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
    //    {
    //        this.pointer = pointer;
    //        this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
    //    }

    //    public bool IsNearPointer => pointer is IMixedRealityNearPointer;

    //    /// Returns the grab point on the manipulated object in world space
    //    public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
    //}

    public void DuplicateLayer()
    {
        originP = gameObject.transform.position;
        if (grabCount == 0)
        {
            GameObject clone = Instantiate(gameObject, transform.position, transform.rotation, gameObject.transform.parent);
            clone.transform.localScale = gameObject.transform.localScale;
            //Transform parent = gameObject.transform.parent;
            //clone.transform.parent = parent;
            //clone.transform.position = originP;
            //if (isOrigin)
            //    ResetPosition();
            grabCount++;
        }
    }

    public void ResetPosition()
    {
        gameObject.transform.rotation = originR;
        gameObject.transform.position = originP;
    }

    public void AddPedestal()
    {
        if (!pedestalAdded)
        {
            GameObject clonePedestal = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal"));
            clonePedestal.GetComponent<RemovePedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            clonePedestal.tag = "pedestal";
            clonePedestal.transform.parent = gameObject.transform.parent;
            clonePedestal.transform.position = gameObject.transform.GetChild(0).position;
            pedestalAdded = true;
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x, clonePedestal.transform.position.y, gameObject.transform.position.z);
            GameObject line = (GameObject)Instantiate(Resources.Load("Prefabs/Line"));
            line.AddComponent<MatchPedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            line.transform.parent = gameObject.transform.parent;
            line.transform.GetChild(0).position = centerPos;
            line.transform.GetChild(1).position = gameObject.transform.GetChild(0).position;
        }
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, -0.264f, gameObject.transform.position.z);
    }


    public void DragfromWall()
    {
        cloneTable = (GameObject)Instantiate(Resources.Load("Prefabs/TableCenter"));
        //cloneTable = Instantiate(table); 
        GameObject tabletop = GameObject.Find("Tabletop");
        if (tabletop.activeSelf)
        {
            //Transform parent = tabletop.parent;
            cloneTable.transform.parent = tabletop.transform;
        }
        //GameObject largemap = GameObject.Find("largeMap");
        //if (largemap.activeSelf)
        //{
        //    //Transform parent = tabletop.parent;
        //    cloneTable.transform.parent = largemap.transform;
        //}
        cloneTable.SetActive(false);
        xNew = gameObject.transform.eulerAngles.x;
        //xNew = -0.5f;
        gameObject.transform.eulerAngles = new Vector3(xNew, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
    }

    public void SetTable()
    {
        originP = gameObject.transform.position;
        //originR = gameObject.transform.rotation;
        cloneTable.transform.position = new Vector3(gameObject.transform.position.x, 1.48f, gameObject.transform.position.z);
        //cloneTable.transform.position = originP;
        //cloneTable.transform.rotation = originR;
        cloneTable.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PutTableBack()
    {
        originP = gameObject.transform.position;
        //originR = gameObject.transform.rotation;
        Transform tableContainer = gameObject.transform.parent;
        //Vector3 tablePos = gameObject.transform.parent.GetChild(2).GetChild(0).GetChild(9).gameObject.GetComponent<ExtractLayer>().tablePos;
        tableContainer.position = new Vector3(0.5f,0,0.14f);// tablePos;
        Transform tableCenter = gameObject.transform.parent.GetChild(1);
        tableCenter.position = new Vector3(gameObject.transform.position.x, 1.48f, gameObject.transform.position.z);
        tableCenter.gameObject.SetActive(true);
        //Transform layer = gameObject.transform.parent.GetChild(0);
        //layer.gameObject.SetActive(true);
        //layer.position = new Vector3(gameObject.transform.position.x, 1.6f, gameObject.transform.position.z);
        Transform layer = gameObject.transform.parent.GetChild(2);
        layer.gameObject.SetActive(true);
        Transform[] children = layer.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length;i++)
        {
            if(children[i].parent == layer)
                children[i].position = new Vector3(gameObject.transform.position.x, 1.6f + 0.0032f * i, gameObject.transform.position.z);
        }
        //int childrenCount = 0;
        //var children = gameObject.transform.parent.GetComponentsInChildren<Transform>();
        //foreach (var c in children)
        //{
        //    if (c.parent == gameObject.transform.parent)
        //    {
        //        childrenCount += 1;
        //        print("current object: " + c.gameObject.name + " / childrenCount: " + childrenCount);
        //    }
        //}

        //if (childrenCount > 3)
        //    Destroy(gameObject.transform.parent.GetChild(2).gameObject);
        //Destroy(gameObject.transform.parent.GetChild(2).gameObject);
        //gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "2018";
        tableBackCount += 1;
        gameObject.SetActive(false);
    }
}
