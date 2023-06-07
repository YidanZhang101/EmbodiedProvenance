using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemovePedestalContent : MonoBehaviour
{
    public string timeForPedestal;
    int pedestalCount;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {
        MatchPedestalContent[] pedestalItems = FindObjectsOfType(typeof(MatchPedestalContent)) as MatchPedestalContent[];
        int index = 0;
        List<GameObject> matchedItems = new List<GameObject>();
        for (int i = 0; i < pedestalItems.Length;i++)
        {
            if (timeForPedestal == pedestalItems[i].timeForPedestal)
            {
                matchedItems.Add(pedestalItems[i].gameObject);               
            }            
        }
        foreach(GameObject item in matchedItems)
        {
            if (item.name == "Monthly")
            {
                pedestalCount = GameObject.FindGameObjectsWithTag("pedestal").Length;
                //print("pedestalCount: " + pedestalCount);
                Transform parentRoot = item.transform.parent;
                for (int c = 1; c < pedestalCount; c++)
                {
                    parentRoot.GetChild(parentRoot.childCount - c).gameObject.SetActive(false);
                }
            }
            if (item.transform.GetChild(0).GetComponent<BuildingInteractor>() != null & item.name != "Monthly")
            {
                int itemIndex = item.transform.GetSiblingIndex();
                item.transform.parent.GetChild(itemIndex + 1).gameObject.SetActive(false);
            }
            item.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
