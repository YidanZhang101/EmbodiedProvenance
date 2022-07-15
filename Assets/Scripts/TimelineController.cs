using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : MonoBehaviour
{
    public GameObject timeline;
    public GameObject timeline_controller;
    public float gap_per_move;

    float rotation = 0;
    float timeline_position = 0;
    bool timelineStart = false;

    void Start()
    {
        timeline_position = timeline.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = timeline_controller.transform.rotation.y;
        if (timelineStart)
        {
            Debug.Log(rotation);
            if (rotation < 0)
            {
                timeline_position = timeline_position - gap_per_move;
            }
            else if (rotation > 0)
            {
                timeline_position = timeline_position + gap_per_move;
            }
            timeline.transform.position = new Vector3(timeline_position, timeline.transform.position.y, timeline.transform.position.z);
        }        
    }

    public void TimelineStart()
    {
        timelineStart = true;
    }
}
