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

    private float originalHeight;
    private float heightChange = 0f;
    private float risingHeight;
    private float heightBalloonMultiplier = 0.8f;
    private float risingVelocity = 4f;
    private float loweringVelocity = 10f;
    public bool doesFloat = false;
    private float riseRate = 0.08f;
    public bool destroyedOnLanding = false;
    private float destroyTime = 1f;
    private Rigidbody2D furnitureBody;

    void Awake(){
        risingHeight = numberBalloons * heightBalloonMultiplier;
        furnitureBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
        void Update()
    {
        NumberBalloons = this.transform.childCount;
    }
    void OnCollisionEnter2D(Collision2D myCol)
    {
        if (myCol.gameObject.name == "Balloon")
        {

            this.NumberBalloons += 1;

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

    public void OnLavaRise(float risePosition){
        originalHeight = transform.position.y;
        if(doesFloat)
            StartCoroutine(NewLavaLevel(risePosition));
    }

    public void OnLavaReset(){
        if(doesFloat)
            transform.position = new Vector3(transform.position.x, originalHeight, transform.position.z);
    }

    private IEnumerator NewLavaLevel(float risePosition){
        for(float f = transform.position.y; f < risePosition; f += riseRate){
            transform.position = new Vector3(transform.position.x, f, transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }
    } 

    public void PlatformTrigger(){
        if(destroyedOnLanding)
            StartCoroutine(DestroyFurniture());
    }

    private IEnumerator DestroyFurniture(){
        float translateAmount = 0.01f;
        int changeDirectionInterval = 5;
        Vector2 oldPosition = furnitureBody.position;
        for(int i = 0; i < destroyTime * 100; i++){
            if(i % changeDirectionInterval == 0)
                translateAmount = -translateAmount;
            oldPosition = oldPosition + new Vector2(0, translateAmount);
            furnitureBody.position = oldPosition;
            yield return new WaitForSeconds(0.01f);
        }
        gameObject.SetActive(false);
    }
}
