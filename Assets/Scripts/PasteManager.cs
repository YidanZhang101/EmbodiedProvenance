using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasteManager : MonoBehaviour
{
    bool plateHide = false;
    string environmentName = ""; //the current scene's prefab
    GameObject newItem;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        if (!plateHide)
        {
            collider.gameObject.SetActive(false);
            GameObject.Find("environments").SetActive(false);

            GetEnvironmentName();
            LoadScene();



            plateHide = true;
        }        
    }

    private void GetEnvironmentName()
    {
        if (SceneManager.GetSceneByBuildIndex(9).isLoaded)
            environmentName = SceneManager.GetSceneByBuildIndex(9).name;
    }

    public void LoadScene()
    {
        //var currentScene = SceneManager.GetSceneByName(environmentName);
        string newSceneName = gameObject.name;

        //SceneManager.UnloadScene(currentScene);
        GameObject[] itemsInCurrentScene = GameObject.FindGameObjectsWithTag(environmentName);
        foreach(GameObject item in itemsInCurrentScene)
        {
            item.SetActive(false);
        }

        //GameObject.Find("TablesOnTimeline").transform.GetChild(0).gameObject.SetActive(false);
        //GameObject.Find("TablesOnTimeline").transform.GetChild(2).gameObject.SetActive(false);

        SceneManager.LoadScene(newSceneName, LoadSceneMode.Additive);
    }

    public void GetCopiedObjects()
    {
        Dictionary<string, List<string[]>> objectsToCopy = CopyManager.CopiedObjects;
        if (objectsToCopy != null)
        {
            foreach(List<string[]> values in objectsToCopy.Values)
            {
                foreach(string[] itemInfo in values)
                {
                    CreateItem(itemInfo);
                    newItem.transform.position = StringToVector3(itemInfo[1]);
                    newItem.transform.rotation = StringToQuaternion(itemInfo[2]);
                    newItem.transform.localScale = StringToVector3(itemInfo[3]);

                }
            }
        }
    }

    private void CreateItem(string[] itemInfo) //ISSUE: nothing added to AllObjects
    {
        GameObject objectContainerInNewScene = GameObject.Find("AllObjects");

        string name = itemInfo[0];
        if (itemInfo.Length == 5)
        {
            if (name.Contains("Tabletop"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Tabletop"));
                newItem.transform.GetChild(0).GetComponent<TextMeshPro>().text = itemInfo[4];
            }

            if (name.Contains("Layer"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Layer"));
                newItem.transform.GetChild(7).gameObject.GetComponent<TextMeshPro>().text = itemInfo[4];
            }
            if (name.Contains("Block_LTB"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_LTB"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
            }
                
            if (name.Contains("Block_Monash_Sport"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Monash_Sport"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
            }
                
            if (name.Contains("Block_Theatre"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Theatre"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
            }
                
            if (name.Contains("Block_Campus_Centre"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_Campus_Centre"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
            }
                
            if (name.Contains("Block_New_Horizons"))
            {
                newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Block_New_Horizons"));
                newItem.transform.GetChild(2).GetComponent<TextMeshPro>().text = itemInfo[4];
            }               
        }       
            
        if (name.Contains("Pedestal"))
            newItem = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal"));

        newItem.transform.parent = objectContainerInNewScene.transform;
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
