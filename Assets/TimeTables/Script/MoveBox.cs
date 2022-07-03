using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBox : MonoBehaviour
{
    public float speed = 2f;
    GameObject box;
    GameObject target;
    GameObject leave;
    //Vector3 originalPos;
    bool readyToLeave = false;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        box = GameObject.Find("Box");
        target = GameObject.Find("BoxTarget");
        leave = GameObject.Find("BoxLeave");
        //originalPos = box.transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {        
        float step = speed * Time.deltaTime; // calculate distance to move
        if (!readyToLeave)
            box.transform.position = Vector3.MoveTowards(box.transform.position, target.transform.position, step);
        if (Vector3.Distance(box.transform.position, target.transform.position) < 0.001f)
            readyToLeave = true;
        if (readyToLeave & Time.time - startTime > 10f)
            box.transform.position = Vector3.MoveTowards(box.transform.position, leave.transform.position, step);
    }

    public void moveBox()
    {
        
    }
}
