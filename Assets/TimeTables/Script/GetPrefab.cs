using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject allTablePrefab = (GameObject)Instantiate(Resources.Load("Prefabs/tabetopAll"));
        allTablePrefab.transform.parent = gameObject.transform;
        //GameObject allTablePrefab = (GameObject)Resources.Load("Prefabs/tabetopAll", typeof(GameObject));
        allTablePrefab.transform.localScale = new Vector3(allTablePrefab.transform.localScale.x * 12.2f, allTablePrefab.transform.localScale.y * 12.2f, allTablePrefab.transform.localScale.z * 12.2f);
        allTablePrefab.transform.position = new Vector3(0,-16f, -3.8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
