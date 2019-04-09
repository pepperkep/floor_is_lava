using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBehavior : MonoBehaviour
{
    BoxCollider2D balloonCollider;
    Collider2D liftFurniture;
    private CameraController modeSwitch;
    public GameObject furniture;
    public Furniture furnitureScript;
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool canDrag;
    public bool moveBalloon = false;
    private Vector3 originalPosition;
    private RaycastHit2D[] collisionCheck = new RaycastHit2D[8];
    //private Rigidbody2D platformBody;



    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        balloonCollider = GetComponent<BoxCollider2D>();
       // platformBody = GetComponent<Rigidbody2D>();
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
            moveBalloon = false;
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = (gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)));
        }
    }

    void OnMouseUp()
    {
        if (canDrag) { 



        moveBalloon = true;
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
        if (canDrag && moveBalloon && this.transform.position.y < 5)
        {
            for(int i = 0; i < 5; i++)
            {
                this.transform.position = new Vector2(this.transform.position.x, (this.transform.position.y) + .1f);
            }
        }



    }
    void OnCollisionEnter2D(Collision2D myCol)
    {
        if (myCol.gameObject.name == "Wall")
        {

            myCol.gameObject.transform.position = this.transform.position;

        }
    }
}
