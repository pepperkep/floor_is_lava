using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private int numberBalloons;

    public int NumberBallons{
        get => numberBalloons;
        set{
            risingHeight = originalHeight + value * heightBalloonMultiplier;
            numberBalloons = value;
        }
    }

    private float originalHeight;
    private float risingHeight;
    private float heightBalloonMultiplier = 0.8f;
    private float risingVelocity = 4f;
    private float loweringVelocity = 15f;

    void Awake(){
        originalHeight = transform.position.y;
        risingHeight = originalHeight + numberBalloons * heightBalloonMultiplier;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        risingHeight = originalHeight + numberBalloons * heightBalloonMultiplier;
        if(transform.position.y < risingHeight){
            float newHeight = transform.position.y + risingVelocity * Time.fixedDeltaTime;
            if(newHeight > risingHeight)
                transform.position = new Vector2(transform.position.x, risingHeight);
            else
                transform.position = new Vector2(transform.position.x, newHeight);
        }
        if(transform.position.y > risingHeight){
            float newHeight = transform.position.y - loweringVelocity * Time.fixedDeltaTime;
            if(newHeight < risingHeight)
                transform.position = new Vector2(transform.position.x, risingHeight);
            else
                transform.position = new Vector2(transform.position.x, newHeight);
        }
        
    }
}
