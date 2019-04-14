using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private int numberBalloons;

    public int NumberBalloons{
        get => numberBalloons;
        set{
            risingHeight = value * heightBalloonMultiplier;
            numberBalloons = value;
        }
    }

    private float heightChange = 0f;
    private float risingHeight;
    private float heightBalloonMultiplier = 0.8f;
    private float risingVelocity = 4f;
    private float loweringVelocity = 10f;

    void Awake(){
        risingHeight = numberBalloons * heightBalloonMultiplier;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter2D(Collision2D myCol)
    {
        if (myCol.gameObject.name == "Balloon")
        {

            this.NumberBalloons+=1;

        }
    }
    void FixedUpdate(){
        risingHeight = numberBalloons * heightBalloonMultiplier;
        if(heightChange < risingHeight){
            float newHeight = risingVelocity * Time.fixedDeltaTime;
            float checkValue = heightChange + newHeight;
            if(checkValue > risingHeight){
                transform.position += new Vector3(0, risingHeight - heightChange, 0);
                heightChange = risingHeight;
            }
            else{
                transform.position += new Vector3(0, newHeight, 0);
                heightChange += newHeight;
            }
        }
        if(heightChange > risingHeight){
            float newHeight = loweringVelocity * Time.fixedDeltaTime;
            float checkValue = heightChange - newHeight;
            if(checkValue < risingHeight){
                transform.position -= new Vector3(0, heightChange - risingHeight, 0);
                heightChange = risingHeight;
            }
            else{
                transform.position -= new Vector3(0, newHeight, 0);
                heightChange -= newHeight;
            }
        }
    }
}
