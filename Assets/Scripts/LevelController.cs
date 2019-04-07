using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField] private ObjectivePoint[] objectiveList;
    private int currentObjective = 0;

    public void BeginLevel(){
        Debug.Log("Level has begun!");
    }

    public void EndLevel(){

    }

    // Start is called before the first frame update
    void Start()
    {
        objectiveList[currentObjective].IsActive = true;
        BeginLevel();
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
