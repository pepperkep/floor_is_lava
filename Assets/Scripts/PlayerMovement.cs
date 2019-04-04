using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{

    //Private movement fields
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float groundDecceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnAroundMultiplier;
    [SerializeField] private float airAcceleration;
    [SerializeField] private Vector2 gravity;
    [SerializeField] private float jumpVelocity;

    //Properties for movement fields
    public float GroundAcceleration {
        get => this.groundAcceleration;
        set => this.groundAcceleration = value;
    }

    public float GroundDecceleration {
        get => this.groundDecceleration;
        set => this.groundDecceleration = value;
    }

    public float MaxSpeed{
        get => this.maxSpeed;
        set => this.maxSpeed = value;
    }

    public float TurnAroundMultiplier{
        get => this.turnAroundMultiplier;
        set => this.turnAroundMultiplier = value;
    }

    public float AirAcceleration {
        get => this.airAcceleration;
        set => this.airAcceleration = value;
    }

    public Vector2 Gravity {
        get => this.gravity;
        set => this.gravity = value;
    }

    public float JumpVelocity {
        get => this.jumpVelocity;
        set => this.jumpVelocity = value;
    }

    //Private movementData
    private Vector2 velocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Vector2 normal;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private bool isGrounded = true;
    private bool faceRight = true;
    private RaycastHit2D[] collisionCheck = new RaycastHit2D[8];
    private ContactFilter2D contactLayer;
    private float minDistanceCheck = 0.01f;
    private float minGroundDirection = -0.4f;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();

        contactLayer.useTriggers = false;
        contactLayer.SetLayerMask(Physics2D.GetLayerCollisionMask (gameObject.layer));
        contactLayer.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Get player input
        int playerIn = (int)Input.GetAxisRaw("Horizontal");
        Vector2 nextVelocity = this.velocity;
        if(isGrounded){
            if((playerIn < 0 && nextVelocity.x > 0) || (playerIn > 0 && nextVelocity.x < 0)) {
                faceRight = !faceRight;
                nextVelocity.x = playerIn * GroundAcceleration * turnAroundMultiplier;
            }
            else{
                if(playerIn != 0)
                    nextVelocity.x += playerIn * GroundAcceleration * Time.deltaTime;
                else{
                    if(nextVelocity.x < groundDecceleration)
                        nextVelocity.x = 0;
                    else{
                        if(nextVelocity.x > 0)
                            nextVelocity.x -= GroundDecceleration * Time.deltaTime;
                        if(nextVelocity.x < 0)
                            nextVelocity.x += GroundDecceleration * Time.deltaTime;
                    }
                }
            }
        }
        else{
            if(playerIn < 0)
                nextVelocity.x -= AirAcceleration * Time.deltaTime;
            if(playerIn > 0)
                nextVelocity.x += AirAcceleration * Time.deltaTime;
        }

        //Detect for Jump input
        if(Input.GetButton("Jump") && isGrounded){
            nextVelocity.y = JumpVelocity;
            this.isGrounded = false;
        }

        targetVelocity = nextVelocity;
    }

    void FixedUpdate(){
        //Detect if player is grounded
        if(Math.Abs(targetVelocity.x) < MaxSpeed)
            this.velocity = this.targetVelocity;
        else{
            if(targetVelocity.x > 0)
                this.velocity = new Vector2(MaxSpeed, targetVelocity.y);
            if(targetVelocity.x < 0)
                this.velocity = new Vector2(-MaxSpeed, targetVelocity.y);
        }
        this.velocity += this.Gravity * Time.fixedDeltaTime;
        Vector2 nextPosition = this.velocity * Time.fixedDeltaTime;
        //update player position
        TryMove(nextPosition);
    }

    void TryMove(Vector2 movement){
        
        if(movement.magnitude > minDistanceCheck){
            
            int hitCount = playerBody.Cast(movement, contactLayer, collisionCheck, movement.magnitude);
            float groundDist = 0f;
            bool findGround = false;
            for(int i = 0; i < hitCount; i++){
                if(Vector2.Dot(collisionCheck[i].normal, this.Gravity) < minGroundDirection){
                    groundDist = collisionCheck[i].distance;
                    findGround = true;
                    this.velocity.y = 0;
                    normal = collisionCheck[i].normal;
                    if(movement.y < 0) movement.y = -groundDist;
                }
            }
            Vector2 finalPosition = movement;
            Debug.Log(movement);
            if(isGrounded) finalPosition = movement - Vector2.Dot(movement, normal) * normal;
            finalPosition += playerBody.position;
            this.isGrounded = findGround;
            playerBody.MovePosition(finalPosition);
        }

        
    }

    void Jump(){
  
    }
}
