using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject FlowingLavaPrefab;
    [SerializeField] private GameObject Floor;

    private int currentObjective = 0;
    private CameraController modeSwitch;
    public  GameObject lava;
    public  GameObject floor;
    public GameObject player;
    public static float floorWidth;
    public static float floorHeight;
    public static Vector2 floorPosition;
    public SpriteRenderer floorSprite;
     public Sprite lavaSprite;
   
   public void BeginLevel(){
        Debug.Log("Level has begun!");

    }

    public void EndLevel(){


    }

    void Awake(){
        floor.SetActive(true);
        lava.SetActive(false);  
    }

    // Start is called before the first frame update
    void Start()
    {
        modeSwitch = GetComponent<CameraController>();
        objectiveList[currentObjective].IsActive = true;
        BeginLevel();
        floor = GameObject.Find("Floor");
        player = GameObject.Find("Player");
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
            lava.SetActive(true);
            lava.transform.position = floor.transform.position;
            WaterArea lavaArea = lava.GetComponent<WaterArea>();
            lavaArea.size = new Vector2(floor.transform.localScale.x, floor.transform.localScale.y);
            floor.SetActive(false);
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
}
