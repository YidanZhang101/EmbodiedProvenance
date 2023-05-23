using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAfterReplay : MonoBehaviour
{
    bool isReset = false;
    Vector3 originPos;

    void Start()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (DataReplayManager.isLiveReplayStopped & !isReset)
        {
            transform.position = originPos;// new Vector3(4.345f, 3.56f, -6.33f);
            Debug.Log("TableOnTimeline pos x: " + transform.position.x);
            isReset = true;
        }            
    }

    public void ResetReplayPlayer()
    {
        Transform playerPos = GameObject.Find("Player").transform;
        playerPos.position = new Vector3(playerPos.position.x, playerPos.position.y - 10f, playerPos.position.z);
        Vector3 timelinePos = GameObject.Find("TablesOnTimeline").transform.localPosition;
        timelinePos = new Vector3(4.345f, 3.56f, -6.33f);
    }
}
