using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class LoadScene : MonoBehaviour
{
    public GameObject tabletopContents;
    Vector3 orginScale;
    Vector3 orginPos;
    Transform thisTable;
    Vector3 tableScale;
    Vector3 tablePos;
    //public Transform loadingBar;
    //public GameObject loadingText;
    //public GameObject progress;
    // Start is called before the first frame update
    void Start()
    {
        orginScale = tabletopContents.transform.localScale;
        orginPos = tabletopContents.transform.position;
        thisTable = GameObject.Find("Tabletop").transform;//gameObject.transform.parent;
        tableScale = thisTable.localScale;
        tablePos = thisTable.position;
    }

    // Update is called once per frame
    void Update()
    {
        //IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        //if (sceneSystem.SceneOperationInProgress)
        //{
        //    DisplayProgressIndicator(sceneSystem.SceneOperationProgress);
        //}
        //else
        //{
        //    HideProgressIndicator();
        //}
    }

    //private void HideProgressIndicator()
    //{
    //    loadingBar.gameObject.SetActive(false);
    //}

    //private void DisplayProgressIndicator(float sceneOperationProgress)
    //{
    //    loadingBar.gameObject.SetActive(true);
    //    //loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "jumping into the table";
    //    //progress.GetComponent<TMPro.TextMeshProUGUI>().text = sceneOperationProgress / 1 * 100 + "%";
    //}

    public async void loadContentAsync()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        // Additively load a single content scene
        await sceneSystem.LoadContent("TestScene");

        // Additively load a set of content scenes
        //await sceneSystem.LoadContent(new string[] { "MyContentScene1", "MyContentScene2", "MyContentScene3" });

        //// ContentScene1, ContentScene2 and ContentScene3 will be loaded additively
        //await sceneSystem.LoadContent("ContentScene1");
        //await sceneSystem.LoadContent("ContentScene2");
        //await sceneSystem.LoadContent("ContentScene3");

        //// ContentScene1, ContentScene2 and ContentScene3 will be unloaded
        //// SingleContentScene will be loaded additively
        //await sceneSystem.LoadContent("SingleContentScene", LoadSceneMode.Single);
    }

    public async void loadLargeMapAsync()
    {
        //IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        //await sceneSystem.LoadContent("SampleScene 1", LoadSceneMode.Single);
        ////await sceneSystem.UnloadContent("TestScene");
        ////await sceneSystem.LoadContent("SampleScene 1");

        tabletopContents.transform.localScale = new Vector3(tabletopContents.transform.localScale.x * 12.2f, tabletopContents.transform.localScale.y * 12.2f, tabletopContents.transform.localScale.z * 12.2f);
        tabletopContents.transform.position = new Vector3(30f, -16.2f, 0);// (-1.4f, -16f, -30f);
        thisTable.localScale = new Vector3(thisTable.transform.localScale.x * 12.2f, thisTable.transform.localScale.y * 12.2f, thisTable.transform.localScale.z * 12.2f);
        thisTable.position = new Vector3(-1.4f, -16.2f, -7f);
        GameObject map = thisTable.GetChild(0).GetChild(0).gameObject;
        map.GetComponent<ClipController>().width = 17.5f;
        map.GetComponent<ClipController>().height = 13.5f;
        GameObject.Find("LayerPlane_base").SetActive(false);
        //change the building for eraser
        gameObject.transform.parent.GetChild(3).GetChild(2).gameObject.SetActive(false);
        gameObject.transform.parent.GetChild(3).GetChild(9).gameObject.SetActive(true);
        gameObject.transform.parent.GetChild(3).GetChild(0).gameObject.SetActive(false);
        gameObject.transform.parent.GetChild(3).GetChild(8).gameObject.SetActive(true);
        gameObject.transform.parent.GetChild(4).GetChild(2).gameObject.SetActive(false);
        gameObject.transform.parent.GetChild(4).GetChild(9).gameObject.SetActive(true);
        gameObject.transform.parent.GetChild(4).GetChild(0).gameObject.SetActive(false);
        gameObject.transform.parent.GetChild(4).GetChild(8).gameObject.SetActive(true);
    }

    public void BackToTable()
    {
        tabletopContents.transform.localScale = orginScale;
        tabletopContents.transform.position = orginPos;
        thisTable.localScale = tableScale;
        thisTable.position = tablePos;
        GameObject map = thisTable.GetChild(0).GetChild(0).gameObject;
        map.GetComponent<ClipController>().width = 1.4f;
        map.GetComponent<ClipController>().height = 1.1f;
        //GameObject.Find("LayerPlane_base").SetActive(true);
        gameObject.transform.parent.parent.GetChild(1).gameObject.SetActive(true); // activate LayerPlane_base
        //change the building for eraser
        gameObject.transform.parent.parent.GetChild(3).GetChild(2).gameObject.SetActive(true);
        gameObject.transform.parent.parent.GetChild(3).GetChild(9).gameObject.SetActive(false);
        gameObject.transform.parent.parent.GetChild(4).GetChild(2).gameObject.SetActive(true);
        gameObject.transform.parent.parent.GetChild(4).GetChild(9).gameObject.SetActive(false);
        gameObject.transform.parent.parent.GetChild(3).GetChild(0).gameObject.SetActive(true);
        gameObject.transform.parent.parent.GetChild(3).GetChild(8).gameObject.SetActive(false);
        gameObject.transform.parent.parent.GetChild(4).GetChild(0).gameObject.SetActive(true);
        gameObject.transform.parent.parent.GetChild(4).GetChild(8).gameObject.SetActive(false);
    }
}
