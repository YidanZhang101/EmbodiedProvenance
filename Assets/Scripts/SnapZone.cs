using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZone : MonoBehaviour
{
    public GameObject target;
    public GameObject target_In_SnapZone;
    //public float[] positionInSnapZone = { 0.649f,0.546f,-1.66f};
    //public float[] scaleInSnapZone = {0.96f,0.96f,0.96f };

    public AudioSource AudioSource;

    public bool InSnapZone = false;
    public bool IsGrabbed = false;

    public float x;
    public float y;
    public float z;

    public static Quaternion handleRotation;

    public static string actionReq = "";

    Vector3 orginScale;
    //Quaternion origRot;

    void Start()
    {
        orginScale = target.transform.localScale;
        //Spatula_In_SnapZone = GameObject.Find("Spatula_In_SnapZone");
        //origRot = target.transform.rotation;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if(gameObject.tag == "SnapZone")
            InSnapZone = true;
        else if (!InSnapZone)
        {
            if (gameObject.name.StartsWith("undo"))
                actionReq = "undo";
            if (gameObject.name.StartsWith("redo"))
                actionReq = "redo";
            //target.transform.rotation = origRot;
            ActionRecoverManager.Undo_Redo();
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (gameObject.tag == "SnapZone")
        {
            InSnapZone = true;
            target.transform.localScale = target_In_SnapZone.transform.localScale;
        }
           
    }

    void OnTriggerExit(Collider collider)
    {
        if (gameObject.tag == "SnapZone")
        {
            InSnapZone = false;
            target.transform.localScale = orginScale;

            if (!target.name.Equals("Microphone"))
                AudioSource.Play();
        }
        else
            actionReq = "";
    }

    public void OnManipulation()
    {
        IsGrabbed = true;

        if (target.name.Equals("Microphone"))// && InSnapZone)
        {
            AnnotationRecorder recordAnnotation = target.GetComponent<AnnotationRecorder>();
            recordAnnotation.InitAnnotation(target.transform.position);
        }
    }

    public void EndManipulation()
    {
        if (target.name.Equals("Microphone"))
        {
            AnnotationRecorder recordAnnotation = target.GetComponent<AnnotationRecorder>();
            recordAnnotation.LogAnnotation();
        }
        else // for spatula
        {
            if (InSnapZone)
            {
                target.transform.position = target_In_SnapZone.transform.position;
                target.transform.rotation = target_In_SnapZone.transform.rotation;
                target.transform.localScale = target_In_SnapZone.transform.localScale;
                //if (target.name.Equals("Lever"))
                //{
                //    if (ActionRecoverManager.actionDic == null)
                //        ActionRecoverManager.actionID = 0;
                //    else
                //    {
                //        ActionRecoverManager.actionID = ActionRecoverManager.actionDic.Count - 1;
                //        Debug.Log("action ID: " + ActionRecoverManager.actionID);
                //    }
                //}
                //else if (target.name.Equals("Microphone"))
                //{
                //    AnnotationRecorder recordAnnotation = target.GetComponent<AnnotationRecorder>();
                //    recordAnnotation.LogAnnotation();
                //}
            }
            else
            {
                target.transform.localScale = orginScale;
                target.transform.rotation = new Quaternion(x, y, z, 0);
                //if (target.name.Equals("Lever"))
                //    handleRotation = target.transform.GetChild(1).rotation;
            }
        }
    }
}
