using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject targetObject1;
    public GameObject targetObject2;
    public GameObject targetObject3;
    public GameObject targetObject4;
    public GameObject targetObject5;
    public GameObject player;
    public Rigidbody2D playerBody;
    //public GameObject targetObject6;
    bool dragMode;

    public void SetDragMode()
    {
        dragMode = true;
        PlayCamera.enabled = false;
        DragCamera.enabled = true;
    }

    public void SetPlayMode()
    {
        dragMode = false;
        PlayCamera.enabled = true;
        DragCamera.enabled = false;
    }

    void Start()
    {
        SetPlayMode();
        player = GameObject.Find("Player");
        playerBody = player.GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            if (dragMode)
            {
                SetPlayMode();
              //  playerBody.WakeUp();
                targetObject1.SendMessage("SetPlayMode");
                targetObject2.SendMessage("SetPlayMode");
                targetObject3.SendMessage("SetPlayMode");
                targetObject4.SendMessage("SetPlayMode");
                targetObject5.SendMessage("SetPlayMode");
               // targetObject6.SendMessage("SetPlayMode");
            }
            else
            {
                SetDragMode();
               // playerBody.Sleep();
               // playerBody.isKinematic = true;
                targetObject1.SendMessage("SetDragMode");
                targetObject2.SendMessage("SetDragMode");
                targetObject3.SendMessage("SetDragMode");
                targetObject4.SendMessage("SetDragMode");
                targetObject5.SendMessage("SetDragMode");
               // targetObject6.SendMessage("SetDragMode");
            }
                
        }
    }
 

}