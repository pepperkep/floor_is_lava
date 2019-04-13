using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour

{

    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject lavaLevel;
    [SerializeField] private GameObject playerPrefab;

    private int currentObjective = 0;
    private CameraController modeSwitch;
    private bool lavaSwitch = false;
    public GameObject lava;
    public GameObject floor;
    public GameObject player;
    public static float floorWidth;
    public static float floorHeight;
    public static Vector2 floorPosition;
    public SpriteRenderer floorSprite;
    public Sprite lavaSprite;
    public float lavaSizeMultiplier;
    private WaterArea lavaArea;
    private Vector3 originalPlayerPosition;
    [SerializeField] private GameObject deathUI;

    public void EndLevel()
    {

    }

    public void BeginLevel(){
        deathUI.SetActive(false);

        lava.transform.position = floor.transform.position;
        this.lavaArea = lava.GetComponent<WaterArea>();
        lavaArea.size = new Vector2(floor.transform.localScale.x, floor.transform.localScale.y);
        lavaLevel.transform.position = new Vector3(lava.transform.position.x - lavaArea.size.x / 2, lava.transform.position.y + lavaArea.size.y * lavaSizeMultiplier / 2, lava.transform.position.z);
        lava.SetActive(true);
        lavaLevel.SetActive(false);
        lavaArea.AdjustComponentSizes();
        lavaArea.RecomputeMesh();
        floor.SetActive(false);

        objectiveList[currentObjective].IsActive = false;
        currentObjective = 0;
        objectiveList[currentObjective].IsActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        deathUI.SetActive(false);
        floor.SetActive(true);
        lava.SetActive(false);
        modeSwitch = GetComponent<CameraController>();
        floor = GameObject.Find("Floor");
        player = GameObject.Find("Player");
        floorSprite = floor.GetComponent<SpriteRenderer>();
        
        originalPlayerPosition = player.transform.position;
        objectiveList[currentObjective].IsActive = true;

        Vector2 size = floor.GetComponent<BoxCollider2D>().bounds.size;
        floorPosition = floor.transform.position;
        floorWidth = size.x;
        floorHeight = size.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!modeSwitch.dragMode && !lavaSwitch)
        {
            BeginLevel();
            lavaSwitch = true;
        }

        if (!objectiveList[currentObjective].IsActive)
        {
            if (currentObjective < objectiveList.Length - 1)
            {
                lavaArea.size = new Vector2(lavaArea.size.x, lavaArea.size.y * lavaSizeMultiplier);
                lavaArea.AdjustComponentSizes();
                lavaArea.RecomputeMesh();
                currentObjective++;
                objectiveList[currentObjective].IsActive = true;
            }

            else
            {
                EndLevel();
            }
        }
    }

    public void RestartLevel(bool toDragMode){
        if(toDragMode)
            SceneManager.LoadScene("Level 1");
        else{
            player = (GameObject) Instantiate(playerPrefab);
            player.transform.position = originalPlayerPosition;
            player.name = "Player";
            PlayerMovement playerProperties = player.GetComponent<PlayerMovement>();
            playerProperties.canMove = true;
            BeginLevel();
        }
    }
}
