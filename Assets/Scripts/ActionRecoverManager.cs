using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionRecoverManager : MonoBehaviour
{
    public static Dictionary<int, List<string[]>> actionDic;
    public static int actionID;

    public GameObject undo_controller;
    public int pedestalOrLineCount = 1;

    float rotation = 0;
    Vector3 linePosition;
    Dictionary<int,List<string[]>> DicForRedo = new Dictionary<int, List<string[]>>();
    int redoID = 0;
    

    void Start()
    {
        actionDic = new Dictionary<int, List<string[]>> ();
        actionID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = undo_controller.transform.rotation.y;
    }

    //public void RetRotation()
    //{
        
    //}

    public void Undo_Redo()
    {
        Debug.Log(rotation);
        if(actionDic != null)
        {        
            if (rotation < 0)
            {
                Debug.Log("undo");
                List<string[]> theLastList = actionDic[actionDic.Count - 1];
                foreach (string[] theObjectArray in theLastList)
                {
                    CheckObject(theObjectArray[0], theObjectArray[1],false,true);
                }
                redoID++;
                DicForRedo.Add(redoID, theLastList);
                foreach (KeyValuePair<int, List<string[]>> pair in DicForRedo)
                    Debug.Log(pair.Key.ToString() + ": " + pair.Value[0][0]);
                actionDic.Remove(actionDic.Count - 1);
                pedestalOrLineCount++;
            }
            else if (rotation > 0)
            {
                Debug.Log("redo");
                if (DicForRedo != null)
                {
                    Debug.Log("DicForRedo is not empty");
                    KeyValuePair<int, List<string[]>> lastPair = DicForRedo.Last();
                    foreach (string[] theObjectArray in lastPair.Value)
                    {
                        CheckObject(theObjectArray[0], theObjectArray[1], true, false);
                    }
                    actionDic.Add(actionDic.Count, lastPair.Value);
                    DicForRedo.Remove(lastPair.Key);
                }
                pedestalOrLineCount--;
            }
        }
        undo_controller.transform.rotation = new Quaternion();
        Debug.Log("Reset value: " + rotation);
    }

    public void CheckObject(string name, string boolValue, bool afterAction, bool beforeAction)
    {
        GameObject theObject = GameObject.Find(name);
        if (boolValue.Equals("true"))
        {
            if (name.StartsWith("Block"))
            {
                theObject.GetComponent<Renderer>().enabled = afterAction;
                theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = afterAction;
            }
            if (name.StartsWith("Pedestal"))
            {
                GameObject[] allPedestal = GameObject.FindGameObjectsWithTag("pedestal");
                if(allPedestal.Length >= pedestalOrLineCount)
                {
                    GameObject theLastPedestal = allPedestal[allPedestal.Length - pedestalOrLineCount];
                    theLastPedestal.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = afterAction;
                }
            }
            if (name.StartsWith("Line"))
            {
                GameObject[] allLine = GameObject.FindGameObjectsWithTag("line");
                if (allLine.Length >= pedestalOrLineCount)
                {
                    GameObject theLastLine = allLine[allLine.Length - pedestalOrLineCount];
                    if (!afterAction)
                    {
                        linePosition = theLastLine.transform.position;
                        theLastLine.transform.position = new Vector3(theObject.transform.position.x - 20, theObject.transform.position.y, theObject.transform.position.z);
                    }
                    else
                    {
                        theLastLine.transform.position = linePosition;
                    }
                }
                
                
            }

        }
        else if (boolValue.Equals("false"))
        {
            if (name.StartsWith("Block"))
            {
                theObject.GetComponent<Renderer>().enabled = beforeAction;
                theObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = beforeAction;
            }

        }

    }
}
