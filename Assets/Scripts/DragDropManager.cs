using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool canDrag;

    void OnMouseDrag()
    {
        if (canDrag)
        {
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            transform.position = cursorPosition;
        }
    }

    void OnMouseDown()
    {
        if (canDrag)
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = (gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)));
        }
    }

    public void SetDragMode()
    {
        canDrag = true;
    }

    public void SetPlayMode()
    {
        canDrag = false;
    }

}
