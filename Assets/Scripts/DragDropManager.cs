using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool canDrag;
    public bool isGrounded = false;
    public bool hitBalloon = false;
    private Vector3 originalPosition;
    private Rigidbody2D platformBody;
    private RaycastHit2D[] collisionCheck = new RaycastHit2D[8];
    private float floorCheckDistance = 15f;

    void Start(){
        originalPosition = transform.position;  
        platformBody = GetComponent<Rigidbody2D>();
    }

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

    void OnMouseUp(){
        if(canDrag)
            SnapToGround();
    }


    public void SnapToGround()
    {
        string obj;
        if (canDrag)
            obj = "Floor";
        else
            obj = "Floor1";
        if (canDrag && collisionCheck[0] != null)
        {
            int hitNum = platformBody.Cast(Vector2.down, collisionCheck, floorCheckDistance);
            bool foundFloor = false;
            for (int i = 0; i < hitNum; i++)
            {
                if ((collisionCheck[i].transform.name == obj || collisionCheck[i].transform.name == "Balloon") && collisionCheck[i].distance != 0)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - collisionCheck[i].distance, transform.position.z);
                    foundFloor = true;
                }
            }
            if (!foundFloor && !hitBalloon)
                transform.position = originalPosition;
            isGrounded = foundFloor;
        }
    }
    
    void OnCollisionEnter2D(Collision2D myCol)
    {
        if (myCol.gameObject.name == "Balloon")
        {

            hitBalloon=true;
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
