
﻿using System.Collections;
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
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float cutJumpSpeed;

    //Field for scene control
    bool canMove;

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

    public float CutJumpSpeed{
        get => this.cutJumpSpeed;
        set => this.cutJumpSpeed = value;
    }

    public Vector2 Gravity {
        get{
            if(velocity.y <= 0)
                return this.gravity * FallMultiplier;
            else
                return this.gravity;
        }
        set => this.gravity = value;
    }

    public float JumpVelocity {
        get => this.jumpVelocity;
        set => this.jumpVelocity = value;
    }

    public float FallMultiplier{
        get => this.fallMultiplier;
        set => this.fallMultiplier = value;
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
    private float minGroundDirection = -0.8f;
    private float slopeNoGravityAngle = 40f;
    private float slopeIsWallAngle = 70f;
    private float distanceToGround = 0;
    private float groundBufferDistance = 0.4f;
    private bool bufferedJump = false;
    private float groundTimer = 0f;
    private float leavePlatformJumpTolerance = 0.1f;
    private bool blockFromBelow = false;
    private GameObject standingPlat;
    private Vector3 oldPlatPlace;


    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();

        contactLayer.useTriggers = false;
        contactLayer.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactLayer.useLayerMask = true;
        standingPlat = this.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            //Get player input
            int playerIn = (int)Input.GetAxisRaw("Horizontal");
            Vector2 nextVelocity = this.velocity;
            if (isGrounded)
            {
                if ((playerIn < 0 && nextVelocity.x > 0) || (playerIn > 0 && nextVelocity.x < 0))
                {
                    faceRight = !faceRight;
                    nextVelocity.x = playerIn * GroundAcceleration * turnAroundMultiplier;
                }
                else
                {
                    if (playerIn != 0)
                        nextVelocity += playerIn * GroundAcceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                    else
                    {
                        if ((Vector2.Dot(nextVelocity, new Vector2(normal.y, -normal.x)) * new Vector2(normal.y, -normal.x)).magnitude < groundDecceleration)
                        {
                            if (Vector2.Angle(normal, Vector2.up) < slopeNoGravityAngle)
                                nextVelocity = Vector2.zero;
                            else
                                nextVelocity.x = 0f;
                        }
                        else
                        {
                            if (nextVelocity.x > 0)
                                nextVelocity -= GroundDecceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                            if (nextVelocity.x < 0)
                                nextVelocity += GroundDecceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                        }
                    }
                }
            }
            else
            {
                if (playerIn < 0)
                    nextVelocity.x -= AirAcceleration * Time.deltaTime;
                if (playerIn > 0)
                    nextVelocity.x += AirAcceleration * Time.deltaTime;
            }

            //Detect for Jump input
            if ((Input.GetButton("Jump") || bufferedJump))
            {
                if (isGrounded || (groundTimer < leavePlatformJumpTolerance && velocity.y < 0))
                {
                    nextVelocity.y = JumpVelocity;
                    this.isGrounded = false;
                    bufferedJump = false;
                }
                else
                {
                    if (distanceToGround < groundBufferDistance && velocity.y < 0)
                        bufferedJump = true;
                }
            }

            if (Input.GetButtonUp("Jump") && nextVelocity.y > cutJumpSpeed)
                nextVelocity.y = cutJumpSpeed;

            targetVelocity = nextVelocity;
        }
    }

    void FixedUpdate(){

        groundTimer += Time.fixedDeltaTime;

        //Determine distance to ground
        int hitCount = playerBody.Cast(Gravity, contactLayer, collisionCheck, Gravity.magnitude);
        float currentDistance = 0;
        for(int i = 0; i < hitCount; i++){
            if(collisionCheck[i].distance > currentDistance)
                currentDistance = collisionCheck[i].distance;
        }
        distanceToGround = currentDistance;

        //Detect if player is grounded
        if(Math.Abs(targetVelocity.x) < MaxSpeed)
            this.velocity = this.targetVelocity;
        else{
            if(targetVelocity.x > 0)
                this.velocity = new Vector2(MaxSpeed, targetVelocity.y);
            if(targetVelocity.x < 0)
                this.velocity = new Vector2(-MaxSpeed, targetVelocity.y);
        }
        Vector2 nextPosition;
        if(isGrounded)
            nextPosition = this.velocity * Time.fixedDeltaTime;
        else
            nextPosition = this.velocity * Time.fixedDeltaTime + 0.5f * this.Gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        Vector2 oldGravity = Gravity;
        this.velocity += 0.5f * (Gravity + oldGravity) * Time.fixedDeltaTime;

        if(velocity.y < Gravity.y / 10 || velocity.y > 0){
            blockFromBelow = false;
            transform.parent = null;
        }

        
        //update player position
        TryMove(nextPosition);
    }

    void TryMove(Vector2 movement){
        if(standingPlat != null){
            playerBody.position += new Vector2(standingPlat.transform.position.x - oldPlatPlace.x, standingPlat.transform.position.y - oldPlatPlace.y);
            oldPlatPlace = standingPlat.transform.position;
        }

        int hitCount = playerBody.Cast(movement, contactLayer, collisionCheck, movement.magnitude);
        float collisionDist = 0f;
        bool findGround = false;
        GameObject newPlat = standingPlat;
        for(int i = 0; i < hitCount; i++){
            Vector2 currentNormal = collisionCheck[i].normal;
            collisionDist = collisionCheck[i].distance;
            if(Vector2.Dot(currentNormal, this.Gravity) < 0 && (isGrounded || collisionDist != 0))
                blockFromBelow = true;
            if(Vector2.Dot(movement, currentNormal) < 0 && (collisionCheck[i].transform.tag != "OneWay" || blockFromBelow)){
                if(Vector2.Dot(currentNormal, this.Gravity) < minGroundDirection && Vector2.Angle(currentNormal, Vector2.up) < slopeIsWallAngle){
                    findGround = true;
                    normal = currentNormal;
                    groundTimer = 0f;
                    newPlat = collisionCheck[i].transform.gameObject;
                }
                this.velocity -= Vector2.Dot(velocity, currentNormal) * currentNormal;
                Vector2 moveInWall = Vector2.Dot(movement, currentNormal) * currentNormal;
                movement -= moveInWall - collisionDist * moveInWall.normalized;
            }
        }
        Debug.Log(standingPlat);
        if(standingPlat != newPlat || isGrounded){
            standingPlat = newPlat;
            oldPlatPlace = standingPlat.transform.position;
        }
        Vector2 finalPosition = movement + playerBody.position;
        this.isGrounded = findGround;
        playerBody.MovePosition(finalPosition);
        
    }

    void Jump(){
  
    }

    public void SetDragMode()
    {
        canMove = false;
    }

    public void SetPlayMode()
    {
        canMove = true;
    }
}

