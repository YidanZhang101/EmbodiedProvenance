using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColliderEvents : MonoBehaviour
{
    public GameObject x;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void onTriggerEnter(Collider other)
    {
        //if (string.Equals("Building_Alexander_Theatre_(7)_(CL48Exh) (1)", other.gameObject.name))
        //{
        //    other.gameObject.SetActive(false);
        //}
        if (string.Equals("Layer2", other.gameObject.name))
        {
            x.SetActive(false);
        }

        Debug.Log("Trigger Entered");
    }

}
