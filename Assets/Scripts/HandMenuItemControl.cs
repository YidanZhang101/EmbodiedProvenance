using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuItemControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ControlItem()
    {
        if (gameObject.activeSelf)
        {
            //if (gameObject.name.Equals("Microphone"))
            //{
            //    AnnotationRecorder recordAnnotation = gameObject.GetComponent<AnnotationRecorder>();
            //    recordAnnotation.LogAnnotation();
            //}
            gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(true);
        
    }
}
