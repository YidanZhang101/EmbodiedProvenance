using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayManager : MonoBehaviour
{
    GameObject mrtkRig;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FollowCamera()
    {
        mrtkRig = GameObject.FindGameObjectWithTag("MRTKRig");
        Vector3 cameraPosition = mrtkRig.transform.position;
        transform.position = new Vector3(cameraPosition.x + 0.5f, cameraPosition.y + 1.2f, cameraPosition.z);
    }
}
