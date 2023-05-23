using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public enum Month { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec}
public enum Week { Week1, Week2, Week3, Week4}
public enum Day { Mon, Tue, Wed, Thu, Fri, Sat, Sun}

public class Block : MonoBehaviour
{
    public static GameObject objectContainerInNewScene;
    public GameObject theBlock;
    public float gapBetweenBlocks = 0.04f;
    //public TextAsset BlockConsumptionData;
    //public Texture2D colorRamp;

    public static int duplicatedBlockCount = 0;

    public static Dictionary<int, List<string[]>> pedestalsDicForCopy = new Dictionary<int, List<string[]>>();
    public int dicKey = 0;
    List<string[]> pedestalForCopy = new List<string[]>();

    int grabCount = 0;
    bool pedestalAdded = false;
    GameObject block;

    List<string> blockDataLines = new List<string>();
    Texture2D colorRamp; 

    private static Block instance;
    public static Block getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new Block();
            }
            return instance;
        }
    }

    void Start()
    {
        GetConsumptionData();
        ReadColorRamp();

        if (!gameObject.name.Contains("Clone"))
        {
            gameObject.GetComponent<Renderer>().material.color = GetColor(gameObject.transform, gameObject.transform.parent);
        }

        objectContainerInNewScene = GameObject.Find("AllElements");
        if (PasteManager.sceneChanged)
        {
            objectContainerInNewScene = GameObject.Find("AllObjects");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DuplicateBlock()
    {
        if (!theBlock.name.Contains("Copy"))
        {
            block = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent);
            duplicatedBlockCount++;
            block.name = theBlock.name + " Copy_" + duplicatedBlockCount.ToString();
            block.transform.localScale = gameObject.transform.localScale;
            //grabCount++;
        }
    }

    private void UpdateText(int timeCount, GameObject block)
    {
        string label = "";
        if (block.tag == "month")
        {
            foreach (var value in Enum.GetValues(typeof(Month)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (block.tag == "week")
        {
            label = "Week " + (timeCount+1);
        }
        if (block.tag == "day")
        {
            foreach (var value in Enum.GetValues(typeof(Day)))
            {
                if ((int)value == timeCount)
                {
                    label = value.ToString();
                }
            }
        }
        if (block.tag == "hour")
        {
            if (timeCount < 12)
            {
                label = timeCount + "am - " + (timeCount + 1) + "am";
            }
            else if (timeCount == 12)
            {
                label = timeCount + "am - " + (timeCount + 1) + "pm";
            }
            else
            {
                label = timeCount + "pm - " + (timeCount + 1) + "pm";
            }
        }
        Transform lineTooltip = block.transform.GetChild(2);
        lineTooltip.gameObject.GetComponent<TextMeshPro>().text = label;
    }

    public void GetConsumptionData()
    {
        string filepath = Application.streamingAssetsPath + "/ConsumptionData.csv";//Application.streamingAssetsPath + "/latin_square_small_vs_medium.csv"; //latin_square
        StreamReader strReader = new StreamReader(filepath);
        //StreamReader strReader = new StreamReader("Assets\\TaskText.csv"); 
        bool endOfFile = false;
        while (!endOfFile)
        {
            string data_String = strReader.ReadLine();
            if (data_String == null)
            {
                endOfFile = true;
                break;
            }
            blockDataLines.Add(data_String);
        }
    }

    public void ReadColorRamp()
    {
        colorRamp = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
        string filename = Application.streamingAssetsPath + "/continous colour.png";
        var rawData = File.ReadAllBytes(filename);
        //Texture2D colorRamp = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
        colorRamp.LoadImage(rawData);

        //colorRamp.LoadRawTextureData(rawData);
        //colorRamp.Apply();
    }

    public Color GetColor(Transform blockName, Transform parent)//(string blockName,Transform parent)
    {
        string[] nameChar = blockName.name.Split('_');
        string keyName = nameChar[1].Split()[0];

        int consumption = 0;

        //string[] blockDataLines = BlockConsumptionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < blockDataLines.Count; i++)
        {
            //string[] lines = blockDataLines[i].Split('\t');
            //if (blockName.Equals(lines[0]) & (parent.name.Equals("Layer_" + lines[1]) | parent.name.Equals(lines[1])))
            //{
            //    consumption = int.Parse(lines[2]);
            //    break;
            //}

            string[] lines = blockDataLines[i].Split(',');
            if (parent.name.Equals("AllElements") | parent.name.Contains("AllObjects"))
            {
                if (blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals(lines[1]) & keyName.Equals(lines[2]))
                {
                    consumption = int.Parse(lines[3]);
                    break;
                }
            }
            else if (blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals("2019") | blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals("2020"))
            {
                string TableInfo = GameObject.Find("Tabletop (1)").transform.GetChild(0).GetComponent<TextMeshPro>().text;
                if (TableInfo.Equals(lines[0]) & blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals(lines[1]) & keyName.Equals(lines[2]))
                {
                    consumption = int.Parse(lines[3]);
                    break;
                }
            }
            //else if (parent.name.Equals("Layer_1"))
            //{
            //    string TableInfo = "";
            //    if(GameObject.Find("Tabletop_1") == null)
            //        TableInfo = GameObject.Find("Tabletop (1)").transform.GetChild(0).GetComponent<TextMeshPro>().text;
            //    else
            //        TableInfo = GameObject.Find("Tabletop_1").transform.GetChild(0).GetComponent<TextMeshPro>().text;
            //    if (TableInfo.Equals(lines[0]) & blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals(lines[1]) & keyName.Equals(lines[2]))
            //    {
            //        consumption = int.Parse(lines[3]);
            //        break;
            //    }    
            //}
            else
            {
                string commonNumber = parent.name.Split('_')[1];
                string TableInfo = "";
                string table_currentLevel = "Tabletop_" + commonNumber;
                int preNum = int.Parse(commonNumber) - 1;
                string table_preLevel = "Tabletop_" + preNum.ToString();
                if (GameObject.Find(table_currentLevel) == null)
                    TableInfo = GameObject.Find(table_preLevel).transform.GetChild(0).GetComponent<TextMeshPro>().text;
                else
                    TableInfo = GameObject.Find(table_currentLevel).transform.GetChild(0).GetComponent<TextMeshPro>().text;
                if (TableInfo.Contains(lines[0]) & blockName.GetChild(2).GetComponent<TextMeshPro>().text.Equals(lines[1]) & keyName.Equals(lines[2]))
                {
                    consumption = int.Parse(lines[3]);
                    break;
                }
            }
        }
        //normalise the value
        float normalisedValue = consumption / 100f;

        //interpolate pixel width
        int w = (int)Mathf.Lerp(0, 1000f, normalisedValue);

        //get pixel color
        Color color = colorRamp.GetPixel(w, 0);
        return color;
    }

    private void CreateBlocks(int maxTimeCount, string tagName)
    {
        List<string[]> blocksForStretch = new List<string[]>();

        int timeCount = 0;
        while (timeCount < maxTimeCount)
        {
            GameObject thisBlock = Instantiate(theBlock, transform.position, transform.rotation, theBlock.transform.parent);
            //GameObject thisBlock = new GameObject();
            //IntantiateBlock(thisBlock);
            thisBlock.name = theBlock.name.Split()[0] + " Clone_" + duplicatedBlockCount.ToString() + "_" + tagName + "_" + timeCount.ToString();
            thisBlock.tag = tagName;
            UpdateText(timeCount, thisBlock);
            thisBlock.transform.localScale = theBlock.transform.localScale;
            thisBlock.transform.position = new Vector3(theBlock.transform.position.x, theBlock.transform.position.y + gapBetweenBlocks * timeCount, theBlock.transform.position.z);
            timeCount++;

            //set block color
            thisBlock.GetComponent<Renderer>().material.color = GetColor(thisBlock.transform, thisBlock.transform.parent);

            //undo/redo
            string[] addedBlock = { thisBlock.name, "true" };
            blocksForStretch.Add(addedBlock);

            //add to dictionary for copy&paste
            string[] blockArray = { thisBlock.name, thisBlock.transform.position.ToString("F4"), thisBlock.transform.rotation.ToString(),
                thisBlock.transform.localScale.ToString("F4"), thisBlock.transform.GetChild(2).GetComponent<TextMeshPro>().text, thisBlock.tag};
            pedestalForCopy.Add(blockArray);
        }
        pedestalsDicForCopy[dicKey - 1] = pedestalForCopy;

        DestroyBlock();
        //undo/redo
        //if (!gameObject.transform.parent.gameObject.name.Contains("Layer"))
        //{
        //    string[] hiddenBlock = { gameObject.name, "false" };
        //    blocksForStretch.Add(hiddenBlock);
        //    int key = ActionRecoverManager.actionID++;
        //    ActionRecoverManager.actionDic.Add(key, blocksForStretch);
        //    //foreach (KeyValuePair<int, List<string[]>> pair in ActionRecoverManager.actionDic)
        //    //    Debug.Log(pair.Key.ToString() + ": " + pair.Value[0][0]);
        //}        
        string[] hiddenBlock = { gameObject.name, "false" };
        blocksForStretch.Add(hiddenBlock);
        //int key = ActionRecoverManager.actionID++;

        int key = 0;
        if (ActionRecoverManager.actionDic != null)
            key = ActionRecoverManager.actionDic.Count;
        ActionRecoverManager.actionDic.Add(key, blocksForStretch);
    }
    public void StretchBlock()
    {
        Invoke(nameof(ShowBlocks), 0.5f);
        grabCount = 0;
    }

    public void ShowBlocks()
    {       
        if (theBlock.tag == "year")
        {
            CreateBlocks(12, "month");
        }
        if (theBlock.tag == "month")
        {
            CreateBlocks(4, "week");
        }
        if (theBlock.tag == "week")
        {
            CreateBlocks(7, "day");
        }
        if (theBlock.tag == "day")
        {
            CreateBlocks(24, "hour");
        }
    }

    public void DestroyBlock()
    {
        //Destroy(this.gameObject);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void AddPedestal()
    {
        if (!theBlock.name.Contains("Copy"))
        {
            //objectContainerInNewScene = GameObject.Find("AllElements");
            //if (PasteManager.sceneChanged)
            //{
            //    objectContainerInNewScene = GameObject.Find("AllObjects");
            //}              

            GameObject clonePedestal = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal"));
            //clonePedestal.GetComponent<RemovePedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            //clonePedestal.tag = "pedestal";
            //clonePedestal.transform.parent = gameObject.transform.parent;
            int pedestalCount = GameObject.FindGameObjectsWithTag("pedestal").Length - 1;
            clonePedestal.name = "Pedestal_" + pedestalCount.ToString();
            clonePedestal.transform.position = theBlock.transform.GetChild(0).position;
            clonePedestal.transform.parent = objectContainerInNewScene.transform;
            pedestalAdded = true;
            GameObject line = (GameObject)Instantiate(Resources.Load("Prefabs/Line"));
            //line.AddComponent<MatchPedestalContent>().timeForPedestal = gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            //line.transform.parent = gameObject.transform.parent;
            int lineCount = GameObject.FindGameObjectsWithTag("line").Length -1;
            line.name = "Line_" + lineCount.ToString();
            line.transform.GetChild(0).position = block.transform.GetChild(0).position;
            line.transform.GetChild(1).position = gameObject.transform.GetChild(0).position;
            line.transform.parent = objectContainerInNewScene.transform;

            //switch the original block and the cloned block
            Vector3 initialPos = block.transform.position;
            block.transform.position = theBlock.transform.position;
            theBlock.transform.position = initialPos;

            Quaternion intialRot = block.transform.rotation;
            block.transform.rotation = theBlock.transform.rotation;
            theBlock.transform.rotation = intialRot;

            //add pedestal in Timeline
            //int pedestalCount = GameObject.FindGameObjectsWithTag("pedestal").Length -1;
            GameObject yearlyTable = GameObject.Find("Yearly_Table");
            string blockName = theBlock.name.Split()[0];
            GameObject timelinePedestal = (GameObject)Instantiate(Resources.Load("Prefabs/Pedestal_" + blockName));
            timelinePedestal.name = "Pedestal_" + blockName + "_" + pedestalCount;
            timelinePedestal.transform.parent = yearlyTable.transform.parent;
            timelinePedestal.transform.position = new Vector3(yearlyTable.transform.position.x + 0.386f + 0.05f * pedestalCount, yearlyTable.transform.position.y - 0.0587f, yearlyTable.transform.position.z - 0.045f);
            timelinePedestal.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = block.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text;
            LayerManager.AddForClippingBox(timelinePedestal);

            //add pedestal for copy&paste
            string[] pedestalArray = { clonePedestal.name, clonePedestal.transform.position.ToString("F4"), clonePedestal.transform.rotation.ToString(), clonePedestal.transform.localScale.ToString("F4") };
            string[] blockArray = { block.name, block.transform.position.ToString("F4"), block.transform.rotation.ToString(),
                block.transform.localScale.ToString("F4"), block.transform.GetChild(2).GetComponent<TextMeshPro>().text,block.tag};
            pedestalForCopy.Add(blockArray);
            pedestalForCopy.Add(pedestalArray);
            dicKey = pedestalCount;
            pedestalsDicForCopy.Add(dicKey, pedestalForCopy);

            //Add to dictionary for Undo/Redo

            //int key = ActionRecoverManager.actionID++;
            //Debug.Log("key for new Block: " + ActionRecoverManager.actionID);
            string[] newblockForAction = { block.name, "true" };
            string[] pedestalForAction = { clonePedestal.name, "true" };
            string[] lineForAction = { line.name, "true" };

            List<string[]> listForAction = new List<string[]>();
            listForAction.Add(newblockForAction);
            listForAction.Add(pedestalForAction);
            listForAction.Add(lineForAction);
            int key = 0;
            if (ActionRecoverManager.actionDic != null)
                key = ActionRecoverManager.actionDic.Count;
            ActionRecoverManager.actionDic.Add(key, listForAction);
        }
    }
}
