﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject FlowingLavaPrefab;
    [SerializeField] private GameObject Floor;

    private int currentObjective = 0;
    public  GameObject lava;
    public  GameObject floor;
    public static float floorWidth;
    public static float floorHeight;
    public static Vector2 floorPosition;
    public SpriteRenderer floorSprite;
     public Sprite lavaSprite;
   
   public void BeginLevel(){
        Debug.Log("Level has begun!");

    }

    public void EndLevel(){
<<<<<<< HEAD
=======
        Debug.Log("Level has ended!");
>>>>>>> e13d2550124633a9f5db1f893afbaa1de40a03a6

    }

    // Start is called before the first frame update
    void Start()
    {
        objectiveList[currentObjective].IsActive = true;
        BeginLevel();
        floor = GameObject.Find("Floor");
        floorSprite=floor.GetComponent<SpriteRenderer>();
        floorSprite.enabled = false;
        floor.GetComponent<SpriteRenderer>().sprite = lavaSprite;
        floorSprite.enabled = true;

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
        if(!objectiveList[currentObjective].IsActive){
            if(currentObjective < objectiveList.Length - 1){
                currentObjective++;
                objectiveList[currentObjective].IsActive = true;


            }
            else
                EndLevel();
        }
    }
}
