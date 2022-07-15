using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject LeftHandPrefab;
    public GameObject RightHandPrefab;

    public HandModel LeftNetworkHand;
    public HandModel RightNetworkHand;

    private void Start()
    {
        LeftNetworkHand = Instantiate(LeftHandPrefab).GetComponent<HandModel>();
        RightNetworkHand = Instantiate(RightHandPrefab).GetComponent<HandModel>();
    }
}
