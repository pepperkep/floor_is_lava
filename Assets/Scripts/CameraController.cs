using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject targetObject1;
    public GameObject targetObject2;
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
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (dragMode)
            {
                SetPlayMode();
                targetObject1.SendMessage("SetPlayMode");
                targetObject2.SendMessage("SetPlayMode");
            }
            else
            {
                SetDragMode();
                targetObject1.SendMessage("SetDragMode");
                targetObject2.SendMessage("SetDragMode");
            }
                
        }
    }
 

}