using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class EraseLargeBuildings : MonoBehaviour
{
    GameObject[] targetBuildings;
    //Transform controlBox;
    // Start is called before the first frame update
    void Start()
    {
        //controlBox = gameObject.transform.parent;

        targetBuildings = GameObject.FindGameObjectsWithTag("clipping");
        foreach (GameObject building in targetBuildings)
        {
            Renderer renderer = building.GetComponent<Renderer>();
            gameObject.GetComponent<ClippingBox>().AddRenderer(renderer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.position = new Vector3(controlBox.position.x, controlBox.position.y * 5f, controlBox.position.z);
    }
}
