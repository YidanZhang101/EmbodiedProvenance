using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasteInNewScene : MonoBehaviour
{
    //GameObject newItem;

    void Start()
    {
        GetCopiedObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCopiedObjects()
    {
        Dictionary<string, List<string[]>> objectsToCopy = CopyManager.CopiedObjects;
        
        if (objectsToCopy != null)
        {
            foreach (List<string[]> values in objectsToCopy.Values)
            {
                foreach (string[] itemInfo in values)
                {
                    CreateItem(itemInfo);
                    

                }
            }
        }
    }

    public void CreateItem(string[] itemInfo) //ISSUE: nothing added to AllObjects
    {
        //Transform objectContainerInNewScene = GameObject.Find("AllObjects").transform;

        string name = itemInfo[0];
        if (itemInfo.Length == 5)
        {
            if (name.Contains("Tabletop"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
                newItem.transform.GetChild(0).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("TimeLabel"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/TimeLabel"));
                newItem.transform.GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("Layer"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Layer"));
                newItem.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }
            if (name.Contains("Block_LTB"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_LTB"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("Block_Monash_Sport"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Monash_Sport"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("Block_Theatre"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Theatre"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("Block_Campus_Centre"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Campus_Centre"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }

            if (name.Contains("Block_New_Horizons"))
            {
                GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_New_Horizons"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
                SetTransform(newItem, itemInfo);
            }
        }

        if (name.Contains("Pedestal"))
        {
            GameObject newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal"));
            SetTransform(newItem, itemInfo);
        }
       
    }

    private void SetTransform(GameObject newItem, string[] itemInfo)
    {
        newItem.name = itemInfo[0] + "_newScene";
        newItem.transform.parent = gameObject.transform;
        newItem.transform.position = StringToVector3(itemInfo[1]);
        newItem.transform.rotation = StringToQuaternion(itemInfo[2]);
        newItem.transform.localScale = StringToVector3(itemInfo[3]);
    }

    public void ReturnToSourceScene()
    {
        GameObject[] itemsInSourceScene = GameObject.FindGameObjectsWithTag("06_Christmas");
        foreach (GameObject item in itemsInSourceScene)
        {
            if (item.name.Contains("Tabletop"))
            {
                GameObject newTable = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
                newTable.name = item.name;
                newTable.transform.parent = item.transform.parent;
                newTable.transform.position = item.transform.position;
                newTable.transform.GetChild(0).GetComponent<TextMeshPro>().text = item.transform.GetChild(0).GetComponent<TextMeshPro>().text;
                item.Destroy();
            }
            else
            {
                Renderer[] _renderer = item.transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer childRenderer in _renderer)
                {
                    if (childRenderer.gameObject.GetComponent<Interactable>() == null)
                        childRenderer.enabled = true;
                }
            }          
        }

        GameObject[] itemsInCurrentScene = GameObject.FindGameObjectsWithTag("01_Wild");
        foreach (GameObject item in itemsInCurrentScene)
        {
            Renderer[] _renderer = item.transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer childRenderer in _renderer)
                childRenderer.enabled = false;
        }
    }

    private Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    private Quaternion StringToQuaternion(string sQuaternion)
    {
        // Remove the parentheses
        if (sQuaternion.StartsWith("(") && sQuaternion.EndsWith(")"))
        {
            sQuaternion = sQuaternion.Substring(1, sQuaternion.Length - 2);
        }

        // split the items
        string[] sArray = sQuaternion.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }
}
