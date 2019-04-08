using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject FlowingLavaPrefab;
    [SerializeField] private GameObject Floor;

    private int currentObjective = 0; //Survive
    private CameraController modeSwitch;
    public  GameObject lava;
    public  GameObject floor;
    public GameObject player;
    public static float floorWidth;
    public static float floorHeight;
    public static Vector2 floorPosition;
    public SpriteRenderer floorSprite;
     public Sprite lavaSprite;

    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject[] targetObjects;
    public Rigidbody2D playerBody;
    public PlayerMovement playerScript;

    public bool dragMode;



    public void BeginLevel(){
        Debug.Log("Level has begun!");

    }

    public void EndLevel(){


    }

    void Awake(){
        lava.SetActive(false);  
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerBody = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<PlayerMovement>();
        SetDragMode();

        modeSwitch = GetComponent<CameraController>();
        objectiveList[currentObjective].IsActive = true;
        BeginLevel();
        floor = GameObject.Find("Floor");
        floorSprite =floor.GetComponent<SpriteRenderer>();

        Vector2 size = floor.GetComponent<BoxCollider2D>().bounds.size;
        floorPosition = floor.transform.position;
        Debug.Log("position is" + floorPosition);
        floorWidth = size.x;
         floorHeight = size.y;
        Debug.Log("width is" + floorWidth);
        Debug.Log("height is" + floorHeight);


    }

    // Update is called once per frame
    void Update()
    {
        if(modeSwitch.dragMode == false){
            floorSprite.sprite = lavaSprite;
            lava.SetActive(true);
        }
        if(!objectiveList[currentObjective].IsActive){
            if (currentObjective < objectiveList.Length - 1)
            {
                currentObjective++;
                objectiveList[currentObjective].IsActive = true;
            }
            else
            {
                EndLevel();
            }
        }
    }

    void OnMouseDown()
    {
        SetPlayMode();
    }


    public void SetPlayMode()
    {
        dragMode = false;
        PlayCamera.enabled = true;
        DragCamera.enabled = false;
        playerScript.SetPlayMode();
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].SendMessage("SetPlayMode");
        }
    }

    public void SetDragMode()
    {
        dragMode = true;
        PlayCamera.enabled = false;
        DragCamera.enabled = true;
        playerScript.SetDragMode();
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].SendMessage("SetDragMode");
        }

    }


}
