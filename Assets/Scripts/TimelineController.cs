using Microsoft.MixedReality.Toolkit.UI;
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
            //Debug.Log(rotation);
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

        if (GameObject.Find("Yearly_Table").GetComponent<Interactable>().ClickCount > 0)
        {
            int last = GameObject.FindGameObjectsWithTag("timeline_table").Length - 1;
            float startPoint = GameObject.Find("Yearly_Table").transform.position.x;
            float endPoint = GameObject.Find("Timeline_Table_" + last).transform.position.x;
            float totalLength = endPoint - startPoint;
            float gap = DataReplayManager.TimeScrubber * totalLength;
            float timeline_position_x = timeline_position - gap;
            timeline.transform.position = new Vector3(timeline_position_x, timeline.transform.position.y, timeline.transform.position.z);
        }
    }

    public void TimelineStart()
    {
        timelineStart = true;
    }

    public void TimeLineStop()
    {
        timelineStart = false;
    }
}
