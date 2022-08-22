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
            Vector3 rotation = headTransform.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            transform.eulerAngles = rotation;

            Vector3 position = headTransform.position;
            position.y = yOffset;
            transform.position = position;
        }
    }
}
