using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineTableHighlighter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name.Equals("Yearly_Table"))
        {
            GameObject yearly_table = GameObject.Find("Tabletop (1)");
            GameObject tableHighlighter = GameObject.Find("TableHighlighter");
            tableHighlighter.transform.position = new Vector3(yearly_table.transform.position.x, tableHighlighter.transform.position.y, tableHighlighter.transform.position.z);            
        }
    }
}
