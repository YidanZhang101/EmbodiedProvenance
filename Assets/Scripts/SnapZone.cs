using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZone : MonoBehaviour
{
    public GameObject target;
    //public float[] positionInSnapZone = { 0.649f,0.546f,-1.66f};
    //public float[] scaleInSnapZone = {0.96f,0.96f,0.96f };

    public bool InSnapZone = false;
    public bool IsGrabbed = false;

    Vector3 orginScale;
    GameObject Spatula_In_SnapZone;

    void Start()
    {
        orginScale = target.transform.localScale;
        Spatula_In_SnapZone = GameObject.Find("Spatula_In_SnapZone");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        InSnapZone = true;   
    }

    void OnTriggerStay(Collider collider)
    {       
        InSnapZone = true;
        target.transform.localScale = Spatula_In_SnapZone.transform.localScale;
    }

    void OnTriggerExit(Collider collider)
    {
        InSnapZone = false;
        target.transform.localScale = orginScale;
    }

    public void OnManipulation()
    {
        IsGrabbed = true;
    }

    public void EndManipulation()
    {
        if(InSnapZone)
        {
            target.transform.position = Spatula_In_SnapZone.transform.position;
            target.transform.rotation = Spatula_In_SnapZone.transform.rotation;
            target.transform.localScale = Spatula_In_SnapZone.transform.localScale;
        }
        else
            target.transform.localScale = orginScale;
    }
}
