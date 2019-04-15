﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LevelController : MonoBehaviour
{



    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject lavaLevel;
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject deathUI;



    private int currentObjective = 0;
    private bool lavaSwitch = false;
    public GameObject lava;
    public GameObject floor;
    public GameObject player;
    public float lavaSizeMultiplier;
    private WaterArea lavaArea;
    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject[] targetObjects;
    public Rigidbody2D playerBody;
    public PlayerMovement playerScript;
    private bool dragMode;
    private Vector3 originalPlayerPosition;

    public void EndLevel()
    {

        Debug.Log("Level has begun!");



    }


    public void BeginLevel(bool restart){
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

        for(int i = 0; i < targetObjects.Length; i++){
            if(restart)
                targetObjects[i].SendMessage("OnLavaReset", null, SendMessageOptions.DontRequireReceiver);
        }

        objectiveList[currentObjective].IsActive = false;
        currentObjective = 0;
        objectiveList[currentObjective].IsActive = true;
    }



    // Start is called before the first frame update

    void Start()

    {

        lavaArea = lava.GetComponent<WaterArea>();

        floor.SetActive(true);

        lava.SetActive(false);
        floor = GameObject.Find("Floor");

        player = GameObject.Find("Player");
        
        originalPlayerPosition = player.transform.position;
        objectiveList[currentObjective].IsActive = true;

        Vector2 size = floor.GetComponent<BoxCollider2D>().bounds.size;

        playerBody = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<PlayerMovement>();
        SetDragMode();
    }



    // Update is called once per frame

    void Update()

    {
        if (!dragMode && !lavaSwitch)
        {
            BeginLevel(false);
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
                for(int i = 0; i < targetObjects.Length; i++){
                    targetObjects[i].SendMessage("OnLavaRise", lava.transform.position.y + lavaArea.size.y / 2, SendMessageOptions.DontRequireReceiver);
                }
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
            BeginLevel(true);
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

}