using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZone : MonoBehaviour
{
    public GameObject target;
    public GameObject target_In_SnapZone;
    //public float[] positionInSnapZone = { 0.649f,0.546f,-1.66f};
    //public float[] scaleInSnapZone = {0.96f,0.96f,0.96f };

    public bool InSnapZone = false;
    public bool IsGrabbed = false;

    public float x;
    public float y;
    public float z;

    public static Quaternion handleRotation;

    Vector3 orginScale;

    void Start()
    {
        orginScale = target.transform.localScale;
        //Spatula_In_SnapZone = GameObject.Find("Spatula_In_SnapZone");
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
        target.transform.localScale = target_In_SnapZone.transform.localScale;
    }

    void OnTriggerExit(Collider collider)
    {
        InSnapZone = false;
        target.transform.localScale = orginScale;
    }

    public void OnManipulation()
    {
        IsGrabbed = true;

        if (target.name.Equals("Microphone") && InSnapZone)
        {
            AnnotationRecorder recordAnnotation = target.GetComponent<AnnotationRecorder>();
            recordAnnotation.InitAnnotation(target.transform.position);
        }
    }

    public void EndManipulation()
    {
        if (InSnapZone)
        {
            target.transform.position = target_In_SnapZone.transform.position;
            target.transform.rotation = target_In_SnapZone.transform.rotation;
            target.transform.localScale = target_In_SnapZone.transform.localScale;
            if (target.name.Equals("Lever"))
            {
                if (ActionRecoverManager.actionDic == null)
                    ActionRecoverManager.actionID = 0;
                else
                {
                    ActionRecoverManager.actionID = ActionRecoverManager.actionDic.Count - 1;
                    Debug.Log("action ID: " + ActionRecoverManager.actionID);
                }
            }
            else if (target.name.Equals("Microphone"))
            {
                AnnotationRecorder recordAnnotation = target.GetComponent<AnnotationRecorder>();
                recordAnnotation.LogAnnotation();
            }
        }
        else
        {
            target.transform.localScale = orginScale;
            target.transform.rotation = new Quaternion(x, y, z, 0);
            if (target.name.Equals("Lever"))
                handleRotation = target.transform.GetChild(1).rotation;
        }
    }
}
