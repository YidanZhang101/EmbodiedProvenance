using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAvatarRoot : MonoBehaviour
{
    public Transform headTransform;

    public float yOffset = 0.18f;

    private void Update()
    {
        if (DataReplayManager.Instance.isLiveReplayRunning)
        {
            transform.eulerAngles = new Vector3(0, 0, 180);

            Vector3 position = headTransform.position;
            position.y = yOffset;
            transform.position = position;
        }
    }
}
