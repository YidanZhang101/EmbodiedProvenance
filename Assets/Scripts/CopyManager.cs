using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    public Material table;
    public bool tableSelected = false;
    public static Dictionary<string, List<string[]>> CopiedObjects = new Dictionary<string, List<string[]>>();
    //public float height = 0.005f;

    List<string[]> CopiedList = new List<string[]>();
    GameObject[] targets;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        int index = 0;
       
        if (!tableSelected)
        {
            if (gameObject.name.Contains("Table"))
            {
                index = FindIndex("timeline_table");
                if (index == 0)
                {
                    AddYearlyTable();
                }
                else
                    CopiedObjects.Add("table_" + index, LayerManager.objectsDicForCopy[index]);
                AddToPlate("big");
            }
            else
            {
                index = FindIndex("timeline_pedestal");
                CopiedObjects.Add("pedestal_" + index, Block.pedestalsDicForCopy[index]);
                AddToPlate("small");
            }

            tableSelected = true;
            
        }

    }

    private void AddToPlate(string name)
    {
        GameObject bigCube = GameObject.Find(name);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = bigCube.transform.parent;
        cube.tag = "plateItem";
        int counter = GameObject.FindGameObjectsWithTag("plateItem").Length - 1;
        cube.transform.position = new Vector3(bigCube.transform.position.x, bigCube.transform.position.y + 0.005f * counter, bigCube.transform.position.z);
        cube.transform.rotation = bigCube.transform.rotation;
        cube.transform.localScale = bigCube.transform.localScale;
        cube.GetComponent<Renderer>().material = table;
    }

    private void AddYearlyTable()
    {
        Transform allElements = GameObject.Find("AllElements").transform;
        Transform tabletop = allElements.GetChild(0);
        string[] tabletopInfo = { tabletop.name, tabletop.position.ToString("F4"), tabletop.rotation.ToString(), tabletop.localScale.ToString("F4"), tabletop.GetChild(0).GetComponent<TextMeshPro>().text };
        CopiedList.Add(tabletopInfo);
        for (int i = 1; i < 6; i++)
        {
            Transform item = allElements.GetChild(i);
            string[] itemArray = { item.name, item.position.ToString("F4"), item.rotation.ToString(), item.localScale.ToString("F4"), item.GetChild(2).GetComponent<TextMeshPro>().text};
            CopiedList.Add(itemArray);
        }
        for (int i = 7; i < 12; i++)
        {
            Transform item = allElements.GetChild(i);
            string[] itemArray = { item.name, item.position.ToString("F4"), item.rotation.ToString(), item.localScale.ToString("F4"), item.GetChild(2).GetComponent<TextMeshPro>().text };
            CopiedList.Add(itemArray);
        }

        Transform label_2019 = allElements.GetChild(6);
        string[] labelInfo = { label_2019.name, label_2019.position.ToString("F4"), label_2019.rotation.ToString(), label_2019.localScale.ToString("F4"), label_2019.GetComponent<TextMeshPro>().text};
        CopiedList.Add(labelInfo);

        Transform label_2020 = allElements.GetChild(12);
        string[] SecondLabelInfo = { label_2020.name, label_2020.position.ToString("F4"), label_2020.rotation.ToString(), label_2020.localScale.ToString("F4"), label_2020.GetComponent<TextMeshPro>().text };
        CopiedList.Add(SecondLabelInfo);

        Transform baseLayer = allElements.GetChild(13);
        string[] baseLayerInfo = { baseLayer.name, baseLayer.position.ToString("F4"), baseLayer.rotation.ToString(), baseLayer.localScale.ToString("F4") };
        CopiedList.Add(baseLayerInfo);

        CopiedObjects.Add("table_0", CopiedList);
    }

    private int FindIndex(string tagName)
    {
        int index = 0;
        targets = GameObject.FindGameObjectsWithTag(tagName);

        if (targets.Length == 0)
        {
            Debug.Log("No game objects are tagged with " + tagName);
        }
        else
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].name.Equals(gameObject.name))
                    index = i;
            }
        }
        return index;
    }


}
