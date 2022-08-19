using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static string nameForLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TeleportToTimeline()
    {
        GameObject mrtkRig = GameObject.FindGameObjectWithTag("MRTKRig");
        Vector3 originalPosition = mrtkRig.transform.position;
        mrtkRig.transform.position = new Vector3(3.1f, 2.65f, -4.26f);
    }
}
