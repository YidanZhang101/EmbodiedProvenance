using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEraser : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMiniMap()
    {
        Invoke(nameof(Show), 1);
    }

    public void Show()
    {
        GameObject leftController = GameObject.FindGameObjectWithTag("leftController");
        leftController.transform.GetChild(12).gameObject.SetActive(true);
        //leftController.transform.GetChild(0).GetChild(2).GetChild(2).gameObject.SetActive(true); //hand
    }
}
