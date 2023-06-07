using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static string nameForLayer;

    GameObject mrtkRig;

    void Start()
    {
        mrtkRig = GameObject.FindGameObjectWithTag("MRTKRig"); //the camera
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject.Find("hand_right_renderPart_0").GetComponent<Renderer>().enabled = false;
    }

    public void TeleportToTimeline()
    {
        Vector3 originalPosition = mrtkRig.transform.position;
        mrtkRig.transform.position = new Vector3(4.3f, 2.8f, -6.7f);
    }

    public void TeleportAmongTables()
    {
        if (transform.name.Equals("MiniTable"))
        {
            Vector3 yearlyTablePos = GameObject.Find("Tabletop (1)").transform.position;
            mrtkRig.transform.position = yearlyTablePos;
        }
        else
        {
            string corespondingTableName = "Tabletop_" + transform.name.Split('_')[1];
            Vector3 corespondingTablePos = GameObject.Find(corespondingTableName).transform.position;
            mrtkRig.transform.position = corespondingTablePos;
        }

        GameObject playerOnMiniMap = GameObject.Find("PlayerOnMiniMap");
        playerOnMiniMap.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.7f,
                transform.localPosition.z + 0.2f);
    }
}
