using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public GameObject timeLabel;
    //public GameObject tabletop;

    [Header("Position Gap")]
    public float gap_x = 0.654f;
    public float gap_y = 0.18f;
    public float gap_z = 0.163f;

    [Header("New Table Position Gap")]
    public float add_x = 1.19f;

    int grabCount = 0;
    public Vector3 layerInitialPos;

    private static Layer instance;
    public static Layer getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new Layer();
            }
            return instance;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DuplicateLayer()
    {
        GameObject layer = (GameObject)Instantiate(Resources.Load("Prefabs/Layer"));
        layer.transform.parent = timeLabel.transform.parent;
        layer.transform.GetChild(7).gameObject.tag = timeLabel.tag;
        int layerCount = FindObjectsOfType<LayerManager>().Length -1;
        //string name_extension = timeLabel.GetComponent<TextMeshPro>().text + "_";
        //TeleportManager.nameForLayer += name_extension;
        layer.name = "Layer_" + layerCount.ToString(); //+ TeleportManager.nameForLayer + layerCount.ToString();
        //string label = layer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text;
        layer.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = timeLabel.GetComponent<TextMeshPro>().text;
        for (int i = 0; i < 5; i++)
        {
            GameObject thisBlock = layer.transform.GetChild(i).gameObject;
            thisBlock.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text = timeLabel.GetComponent<TextMeshPro>().text;
            thisBlock.tag = timeLabel.tag;
        }
        //layer.transform.position = new Vector3(tabletop.transform.position.x, timeLabel.transform.position.y, tabletop.transform.position.z);
        layer.transform.position = new Vector3(timeLabel.transform.position.x + gap_x, timeLabel.transform.position.y + gap_y, timeLabel.transform.position.z + gap_z);
        layerInitialPos = layer.transform.position;
        layer.SetActive(true);
        //timeDescription = timeLabel.GetComponent<TextMeshPro>().text;
    }

}
