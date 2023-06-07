using UnityEngine;
using System.Collections;

public class ResetPosition : MonoBehaviour 

{
  private Vector3 originalPos;
  private Quaternion originalRot;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void GetOriPos()
    {
        if (gameObject.activeSelf)
        {
            //Get the original position
            //originalPos = transform.localPosition;
            originalRot = transform.localRotation;
            Debug.Log("original pos get");
        }       
    }

    public void ResetPos()
    {
        //transform.position = originalPos;
        transform.localRotation = originalRot;
    }

    public void BackToIniPos() //back to player start pos
    {
        GameObject mrtkRig = GameObject.FindGameObjectWithTag("MRTKRig");
        mrtkRig.transform.position = new Vector3();
    }
}