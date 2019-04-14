using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBehavior : MonoBehaviour
{
    BoxCollider2D balloonCollider;
    Collider2D liftFurniture;
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool canDrag;
    public bool moveBalloon = false;
    private Vector3 originalPosition;
    private Rigidbody2D platformBody;
    public GameObject floor;
    public static float floorHeight;
    private bool setPosition = false;
    private float spacingAmount = 6f;



    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        balloonCollider = GetComponent<BoxCollider2D>();
        platformBody = GetComponent<Rigidbody2D>();
        floor = GameObject.Find("Floor");
        Vector2 size = floor.GetComponent<BoxCollider2D>().bounds.size;
        floorHeight = size.y;
    }
    void OnMouseDrag()
    {
        if (canDrag && !setPosition)
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
            moveBalloon = false;
            setPosition = false;
            this.transform.parent = null;
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = (gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)));
        }
    }

    void OnMouseUp()
    {
        if (canDrag && !setPosition)
        {
            if (this.transform.position.y > floorHeight-.2)
            {
                moveBalloon = true;
            }
            else
            {
                transform.position = originalPosition;
            }

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
    // Update is called once per frame
    void Update()
    {
        if (canDrag && moveBalloon && platformBody.transform.position.y < 5)
        {
            for(int i = 0; i < 5; i++)
            {
                this.transform.position = new Vector2(this.transform.position.x, (this.transform.position.y) + .1f);
            }
        }



    }
    void OnCollisionEnter2D(Collision2D myCol) {
        DragDropManager attachTo = myCol.gameObject.GetComponent<DragDropManager>(); 
        if (attachTo != null && attachTo.isGrounded)
        {
            this.transform.parent = myCol.transform;
            int balloonNumber = myCol.transform.childCount;
            if(balloonNumber == 0)
                this.transform.position = new Vector3(myCol.transform.position.x, myCol.transform.position.y + myCol.collider.bounds.size.y / 1.2f, 0);
            else{
                if(balloonNumber % 2 == 0)
                    this.transform.position = new Vector3(myCol.transform.position.x - myCol.collider.bounds.size.x * (balloonNumber / 2) / spacingAmount, myCol.transform.position.y + myCol.collider.bounds.size.y / 1.2f, 0);
                else
                    this.transform.position = new Vector3(myCol.transform.position.x + myCol.collider.bounds.size.x * (balloonNumber / 2) / spacingAmount, myCol.transform.position.y + myCol.collider.bounds.size.y / 1.2f, 0);
            }    
            setPosition = true;
        }
    }
}
