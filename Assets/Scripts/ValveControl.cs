using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveControl : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Undo_Redo()
    {
        //if (transform.rotation.z < 0) // undo
        //{
        //    GameObject.Find("Block_LTB (1)").GetComponentInChildren<Renderer>().enabled = false;
        //    GameObject.Find("Pedestal(Clone)").GetComponentInChildren<Renderer>().enabled = false;
        //    GameObject.Find("Line(Clone)").SetActive(false);
        //}
        //if (transform.rotation.z > 0) // undo
        //{
        //    GameObject.Find("Block_LTB (1)").GetComponentInChildren<Renderer>().enabled = true;
        //    GameObject.Find("Pedestal(Clone)").GetComponentInChildren<Renderer>().enabled = true;
        //    //GameObject.Find("Line(Clone)").GetComponentInChildren<Renderer>().enabled = true;
        //}
    }

    public void RetRotation()
    {
        transform.rotation = new Quaternion();
    }
}
