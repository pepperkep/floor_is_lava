using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera PlayCamera;
    public Camera DragCamera;

    bool editMode;

    public void ShowEditMode()
    {
        editMode = true;
        PlayCamera.enabled = false;
        DragCamera.enabled = true;
    }

    public void ShowPlatformMode()
    {
        editMode = false;
        PlayCamera.enabled = true;
        DragCamera.enabled = false;
    }

    void Start()
    {
        ShowPlatformMode();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (editMode)
            {
                ShowPlatformMode();
            }
            else
                ShowEditMode();
        }
    }
}