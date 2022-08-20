using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlRoomManager : MonoBehaviour
{
    private int grabCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)// & !miniTableSettled)
        {
            foreach (KeyValuePair<string, Vector3> pair in LayerManager.positionForMiniTables)
            {
                GameObject theMiniTable = GameObject.Find(pair.Key);
                theMiniTable.transform.localPosition = new Vector3(pair.Value.x, pair.Value.y, pair.Value.z);
            }

            foreach (KeyValuePair<string, List<Vector3>> pair in LayerManager.positionForMiniTableLinks)
            {
                GameObject theLink = GameObject.Find(pair.Key);
                theLink.transform.GetChild(0).localPosition = pair.Value[0];
                theLink.transform.GetChild(1).localPosition = pair.Value[1];
            }
        }
    }

    public void CreateAnEnvir()
    {
        if (grabCount == 0 && gameObject.transform.parent == GameObject.Find("six_places").transform)
        {
            GameObject clone = Instantiate(gameObject, transform.position, transform.rotation, gameObject.transform.parent.parent);
            clone.transform.localScale = gameObject.transform.localScale;
            grabCount++;
            
        }
    }

    public void EnterEnvironment(GameObject theEnvir)
    {      
        SceneManager.LoadScene("DefaultManagerScene", LoadSceneMode.Single);
        SceneManager.LoadScene(theEnvir.name, LoadSceneMode.Additive);
    }

    public void BackToControlRoom()
    {
        SceneManager.LoadScene("DefaultManagerScene", LoadSceneMode.Single);
        SceneManager.LoadScene("Control_Room", LoadSceneMode.Additive);
        //GameObject Control_Table = GameObject.Find("Control_Table");
        //Control_Table.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
    }
}
