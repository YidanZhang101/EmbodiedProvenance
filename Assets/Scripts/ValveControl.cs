using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValveControl : MonoBehaviour
{
    public TextAsset ObjectData;

    List<GameObject> undoneObjects = new List<GameObject>();
    GameObject previousObject;
    public bool undo = false;

    float rotation = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation = transform.rotation.z;
    }

    public void Undo_Redo()
    {
        //if (transform.rotation.z < 0) // undo
        //{
        //    GameObject.Find("Block_LTB (1)").GetComponentInChildren<Renderer>().enabled = false;
        //    GameObject.Find("Pedestal(Clone)").GetComponentInChildren<Renderer>().enabled = false;
        //    GameObject.Find("Line(Clone)").SetActive(false);
        //}
        //if (transform.rotation.z > 0) // undo
        //{
        //    GameObject.Find("Block_LTB (1)").GetComponentInChildren<Renderer>().enabled = true;
        //    GameObject.Find("Pedestal(Clone)").GetComponentInChildren<Renderer>().enabled = true;
        //    //GameObject.Find("Line(Clone)").GetComponentInChildren<Renderer>().enabled = true;
        //}
        string[] objectDataLines = ObjectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] lastLine = objectDataLines[objectDataLines.Length - 1].Split('\t');
        float lastTimestamp = float.Parse(lastLine[0]);
        Debug.Log("last item name: "+ lastLine[1]);
        if (rotation < -7E-08) // undo
        {
            for (int i = 1; i < objectDataLines.Length; i++)
            {
                string[] lines = objectDataLines[i].Split('\t');
                float timestamp = float.Parse(lines[0]);

                if (lastTimestamp == timestamp)
                {
                    string previousObjectName = GetPrivousObject(lines[1], lines[2]);
                    if (!previousObjectName.Equals("") && previousObjectName.StartsWith("Block"))
                    {
                        previousObject = GameObject.Find(previousObjectName);
                        previousObject.GetComponent<Renderer>().enabled = true;
                        previousObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;
                    }
                    GameObject theObject = GameObject.Find(lines[1]);
                    Debug.Log("theObject: " + theObject.name);
                    undoneObjects.Add(theObject);
                    theObject.GetComponent<Renderer>().enabled = false;
                    theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
                    //Destroy(theObject);
                    //DataLoggingManager.existingObjects.Remove(lines[1]);
                }
            }
            undo = true;
        }
        Debug.Log(rotation);
        if (rotation > -7E-08)// && undo) // redo
        {
            Debug.Log("previous object: " + previousObject.name);
            previousObject.GetComponent<Renderer>().enabled = false;
            previousObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
            Debug.Log("undoObject count: " + undoneObjects.Count);
            foreach (GameObject undoObject in undoneObjects)
            {
                Debug.Log("undoObject: " + undoObject.name);
                undoObject.GetComponent<Renderer>().enabled = true;
                undoObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;
            }
            undo = false;
            undoneObjects.Clear();
            //for (int i = 1; i < objectDataLines.Length; i++)
            //{
            //    string[] lines = objectDataLines[i].Split('\t');
            //    float timestamp = float.Parse(lines[0]);

            //    if (lastTimestamp == timestamp)
            //    {
            //        string previousObjectName = GetPrivousObject(lines[1], lines[2]);
            //        if (!previousObjectName.Equals("") && previousObjectName.StartsWith("Block"))
            //        {
            //            previousObject = GameObject.Find(previousObjectName);
            //            previousObject.GetComponent<Renderer>().enabled = false;
            //            previousObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
            //        }
            //        GameObject theObject = GameObject.Find(lines[1]);
            //        undoneObjects.Add(theObject);
            //        theObject.GetComponent<Renderer>().enabled = true;
            //        theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;
            //        //Destroy(theObject);
            //        //DataLoggingManager.existingObjects.Remove(lines[1]);
            //    }
            //}

        }
    }

    private string GetPrivousObject(string name, string tagName)
    {
        string previousObjectName = "";
        string[] nameItems = name.Split('_');
        int count = nameItems.Length - 1;
        if (count > 1)
        {
            int index = Array.IndexOf(nameItems, tagName);
            List<string> previousObjects = new List<string>();
            for (int i = 0; i < index; i++)
            {
                previousObjects.Add(nameItems[i]);
            }
            if (previousObjects.Count > 0)
                previousObjectName = previousObjects.Aggregate((a, b) => a + '_' + b);
        }
        return previousObjectName;
    }

    public void RetRotation()
    {
        transform.rotation = new Quaternion();
    }
}
