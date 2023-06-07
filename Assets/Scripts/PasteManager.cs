using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasteManager : MonoBehaviour
{
    public static bool sceneChanged = false;
    public static Vector3 linePosition;
    bool plateHide = false;
    string environmentName = ""; //the current scene's prefab
 
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

    public void GetEnvironmentName()
    {
        if (SceneManager.GetSceneByBuildIndex(9).isLoaded)
            environmentName = SceneManager.GetSceneByBuildIndex(9).name; // 06_Chirstmas
    }

    public void LoadScene()
    {
        //var currentScene = SceneManager.GetSceneByName(environmentName);
        string newSceneName = gameObject.name;

        //SceneManager.UnloadScene(currentScene);
        GameObject[] itemsInCurrentScene = GameObject.FindGameObjectsWithTag(environmentName);
        foreach(GameObject item in itemsInCurrentScene)
        {
            //item.SetActive(false);
            //Renderer[] _renderer = item.transform.GetComponentsInChildren<Renderer>();
            //foreach (Renderer childRenderer in _renderer)
            //    childRenderer.enabled = false;
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y - 10, item.transform.position.z);
        }

        //GameObject[] links = GameObject.FindGameObjectsWithTag("tableLink");
        //foreach (GameObject link in links)
        //{
        //    linePosition = link.transform.position;
        //    link.transform.position = new Vector3(link.transform.position.x, link.transform.position.y - 20, link.transform.position.z);
        //}

        sceneChanged = true;

        SetNewTimlineTable();

        //GameObject.Find("TablesOnTimeline").transform.GetChild(0).gameObject.SetActive(false);
        //GameObject.Find("TablesOnTimeline").transform.GetChild(2).gameObject.SetActive(false);

        SceneManager.LoadScene(newSceneName, LoadSceneMode.Additive);

        BackgroundAudioManager.Instance.SwitchBackgroundSound(newSceneName);

    }

    public void SetNewTimlineTable()
    {
        GameObject[] selectedLEDs = GameObject.FindGameObjectsWithTag("LED");

        foreach (GameObject led in selectedLEDs)
        {
            led.GetComponent<Renderer>().enabled = false;
            led.GetComponent<LEDNode>().enabled = false;
        }
    }
}
