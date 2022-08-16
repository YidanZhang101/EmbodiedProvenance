using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUndoRedoGesture : MonoBehaviour, IMixedRealityHandJointHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        Handedness handedness = eventData.Handedness;
        Debug.Log("OnHandJointsUpdated");
        Debug.Log("ThumbFingerCurl: " + HandPoseUtils.ThumbFingerCurl(handedness));
        if (HandPoseUtils.ThumbFingerCurl(handedness) > 0.7f &&
            HandPoseUtils.IndexFingerCurl(handedness) > 0.7f &&
            HandPoseUtils.MiddleFingerCurl(handedness) > 0.7f &&
            HandPoseUtils.PinkyFingerCurl(handedness) > 0.7f &&
            HandPoseUtils.RingFingerCurl(handedness) > 0.7f)
        {
            Vector3 position = eventData.InputData[TrackedHandJoint.IndexMiddleJoint].Position;
            Vector3 direction = eventData.InputData[TrackedHandJoint.IndexMiddleJoint].Up;

            Debug.Log("direction: " + direction);
        }
    }
}
