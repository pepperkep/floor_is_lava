using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject[] targetObjects;
    public GameObject player;
    public Rigidbody2D playerBody;
    public PlayerMovement playerScript;
  
    public bool dragMode;

    public void SetDragMode()
    {
        dragMode = true;
        PlayCamera.enabled = false;
        DragCamera.enabled = true;
        playerScript.SetDragMode();
        for(int i = 0; i < targetObjects.Length; i++){
            targetObjects[i].SendMessage("SetDragMode");
        }
        
    }

    public void SetPlayMode()
    {
        dragMode = false;
        PlayCamera.enabled = true;
        DragCamera.enabled = false;
        playerScript.SetPlayMode();
        for(int i = 0; i < targetObjects.Length; i++){
            targetObjects[i].SendMessage("SetPlayMode");
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");
        playerBody = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<PlayerMovement>();
        SetDragMode();

    }

    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            if (dragMode)
            {
              SetPlayMode();
            }    
        }
    }
 

}