using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlRoomManager : MonoBehaviour
{
    private int grabCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnEnvir()
    {
        if (grabCount == 0 && gameObject.transform.parent == GameObject.Find("six_places").transform)
        {
            GameObject clone = Instantiate(gameObject, transform.position, transform.rotation, gameObject.transform.parent.parent);
            clone.transform.localScale = gameObject.transform.localScale;
            grabCount++;
        }
    }

    public void EnterEnvironment(GameObject theEnvir)
    {      
        SceneManager.LoadScene("DefaultManagerScene", LoadSceneMode.Single);
        SceneManager.LoadScene(theEnvir.name, LoadSceneMode.Additive);
    }

    public void BackToControlRoom()
    {
        SceneManager.LoadScene("DefaultManagerScene", LoadSceneMode.Single);
        SceneManager.LoadScene("Control_Room", LoadSceneMode.Additive);
    }
}
