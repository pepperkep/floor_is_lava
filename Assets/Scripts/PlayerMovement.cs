
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSource source;


    [SerializeField] private float groundAcceleration;      //Acceleration when player is grounded. Should be fairly fast.
    [SerializeField] private float groundDecceleration;     //Deceleration on ground with no input. Should be very fast.
    [SerializeField] private float maxSpeed;                //Maximum horizontal speed. This is the speed the player the player will usually travel at.
    [SerializeField] private float turnAroundMultiplier;    //Value normal ground acceleration is multiplied by when player inputs opposite direction of movement.
    [SerializeField] private float airAcceleration;         //Acceleration when player has jumped or is falling. Should be slower than ground acceleration.
    [SerializeField] private Vector2 gravity;               //Gravity force player constantly feels. Determines which direction is considered the ground.
    [SerializeField] private float jumpVelocity;            //When the player puts in a jump their y velocity will be set to this for two frames.
    [SerializeField] private float fallMultiplier;          //Value gravity is multiplied by when they are falling to improve game feel.
    [SerializeField] private float cutJumpSpeed;            //Value y velocity will be set to if the player lets go of jump early

    [SerializeField] private AudioClip jump;

    //Field for scene control
    public bool canMove = false;
    //Field for animator
    public Animator animator;

    //Properties for movement fields
    public float GroundAcceleration {
        get => this.groundAcceleration;
        set => this.groundAcceleration = value;
    }

    public float GroundDecceleration {
        get => this.groundDecceleration;
        set => this.groundDecceleration = value;
    }

    public float MaxSpeed {
        get => this.maxSpeed;
        set => this.maxSpeed = value;
    }

    public float TurnAroundMultiplier {
        get => this.turnAroundMultiplier;
        set => this.turnAroundMultiplier = value;
    }

    public float AirAcceleration {
        get => this.airAcceleration;
        set => this.airAcceleration = value;
    }

    public float CutJumpSpeed {
        get => this.cutJumpSpeed;
        set => this.cutJumpSpeed = value;
    }

    public Vector2 Gravity {
        get {
            if (velocity.y <= 0)
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

    public float FallMultiplier {
        get => this.fallMultiplier;
        set => this.fallMultiplier = value;
    }

    public Vector2 Velocity{
        get => this.velocity;
        set => this.velocity = value;
    }

    //Private movementData
    private Vector2 velocity = Vector2.zero;            //The player's current velocity
    private Vector2 targetVelocity = Vector2.zero;      //The velocity from player input that will be fed into the physics update
    private Vector2 normal;                             //Normal of the platform the player is currently standing on
    private Rigidbody2D playerBody;                     //Player's rigid body for collision detection
    private SpriteRenderer playerSprite;                //Sprite to change with orientation
    private bool isGrounded = false;                    //Whether the player is grounded. Determines if player can jump
    private RaycastHit2D[] collisionCheck = new RaycastHit2D[8];    //Raycast results for collision
    private ContactFilter2D contactLayer;               //Layer player should collide with
    private float minGroundDirection = -0.8f;           //Boundary for direction that is considered ground  (determined by dot product of normal with gravity)
    private float slopeNoGravityAngle = 40f;            //Threshold for slopes where the player does not slide down
    private float slopeIsWallAngle = 70f;               //Angle where slope is treated as a wall and not ground
    private float distanceToGround = 0;                 //Player's current distance to ground (for jump buffering)
    private float groundBufferDistance = 0.4f;          //Distance from platform where player can buffer a jump
    private bool bufferedJump = false;                  //Whether a jump is currently buffered. Will trigger jump when player lands
    private float groundTimer = 0f;                     //Timer since player left ground. Player can jump if this value is low enough even if not grounded
    private float leavePlatformJumpTolerance = 0.1f;    //Value timer is checked against to determine if player can jump
    private bool blockFromBelow = false;                //Whether the player currently collides with one way platforms
    private GameObject standingPlat;                    //Platform player is currently standing on
    private Vector3 oldPlatPlace;                       //Old location of platform players is on. Used to move player with platforms.
    private bool justJumped = false;                    //Bool to make player jump another frame after they jump. Quick fix for jumps being eaten that is here to stay.


    // Start is called before the first frame update
    void Start()
    {
        //Fill component fields
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //Initialize animator
        animator.SetBool("faceRight", true);

        //Initialize contact layer
        contactLayer.useTriggers = false;
        contactLayer.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactLayer.useLayerMask = true;

        //Initalize standing plat
        standingPlat = null;

        //Initaialize audio
        source = GameObject.Find("SFX Manager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        //Cannot move if in drag mode
        if (canMove)
        {
            //Get player input
            int playerIn = (int)Input.GetAxisRaw("Horizontal");
            Vector2 nextVelocity = this.velocity;
            if (isGrounded)
            {

                //Determine animation state from player input
                if (playerIn < 0)
                {
                    animator.SetBool("faceRight", false);
                    animator.SetBool("walkRight", false);
                    animator.SetBool("walkLeft", true);
                   
                }

                if (playerIn > 0)
                {
                    animator.SetBool("faceRight", true);
                    animator.SetBool("walkRight", true);
                    animator.SetBool("walkLeft", false);

                }

                if (playerIn == 0 && animator.GetBool("faceRight")) { 
                    animator.SetBool("walkRight", false);
                }
                if (playerIn == 0 && !animator.GetBool("faceRight"))
                {
                    animator.SetBool("walkLeft", false);
                }

                //Movment if player is turning
                if ((playerIn < 0 && nextVelocity.x > 0) || (playerIn > 0 && nextVelocity.x < 0))
                {
                   nextVelocity.x = playerIn * GroundAcceleration * turnAroundMultiplier;
                }
                else
                {
                    //Acceleration if player continues in same direction
                    if (playerIn != 0)
                    {
                       nextVelocity += playerIn * GroundAcceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                    }
                    else
                    {
                        //Set velocity to zero if player is on a gradual slope
                        if ((Vector2.Dot(nextVelocity, new Vector2(normal.y, -normal.x)) * new Vector2(normal.y, -normal.x)).magnitude < groundDecceleration && nextVelocity.y < 0)
                        {
                            if (Vector2.Angle(normal, Vector2.up) < slopeNoGravityAngle)
                                nextVelocity = Vector2.zero;
                            else
                                nextVelocity.x = 0f;
                        }
                        else
                        {
                            //Deceleration for if player stops on flat surface (should be quick)
                            if (nextVelocity.x > 0)
                                nextVelocity -= GroundDecceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                            if (nextVelocity.x < 0)
                                nextVelocity += GroundDecceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);
                        }
                    }
                }
            }
            //air movment
            else
            {
                if (playerIn < 0)
                    nextVelocity.x -= AirAcceleration * Time.deltaTime;
                if (playerIn > 0)
                    nextVelocity.x += AirAcceleration * Time.deltaTime;
            }

            //Detect for Jump input or buffered jump
            if (Input.GetButtonDown("Jump") || bufferedJump || justJumped)
            {
                //jump if grounded or just off platform
                if (isGrounded || (groundTimer < leavePlatformJumpTolerance && velocity.y < 0) || justJumped)
                {
                    nextVelocity.y = JumpVelocity;
                    StartCoroutine(playSound(jump));
                    bufferedJump = false;
                    if(!justJumped)
                        justJumped = true;
                    else
                        justJumped = false;
                }
                else
                {
                    //buffer jump if close to ground
                    if (distanceToGround < groundBufferDistance && velocity.y < 0){
                        bufferedJump = true;
                    }
                }
            }

            if (Input.GetButtonUp("Jump") && nextVelocity.y > cutJumpSpeed)
                nextVelocity.y = cutJumpSpeed;

            targetVelocity = nextVelocity;
        }
    }


    void FixedUpdate() {

        if(canMove){
            groundTimer += Time.fixedDeltaTime;

            //Determine distance to ground
            int hitCount = playerBody.Cast(Gravity, contactLayer, collisionCheck, this.targetVelocity.magnitude);
            float currentDistance = 0;
            for (int i = 0; i < hitCount; i++) {
                if (collisionCheck[i].distance > currentDistance)
                    currentDistance = collisionCheck[i].distance;
            }
            distanceToGround = currentDistance;

            //If target x velocity is over max speed change it to the max speed
            if (Math.Abs(targetVelocity.x) < MaxSpeed)
                this.velocity = this.targetVelocity;
            else {
                if (targetVelocity.x > 0)
                    this.velocity = new Vector2(MaxSpeed, targetVelocity.y);
                if (targetVelocity.x < 0)
                    this.velocity = new Vector2(-MaxSpeed, targetVelocity.y);
            }

            //Determine next position from velocity and add in acceleration from gravity
            Vector2 nextPosition;
            if (isGrounded)
                nextPosition = this.velocity * Time.fixedDeltaTime;
            else
                nextPosition = this.velocity * Time.fixedDeltaTime + 0.5f * this.Gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            Vector2 oldGravity = Gravity;
            this.velocity += 0.5f * (Gravity + oldGravity) * Time.fixedDeltaTime;

            //Determine whether to block pass through platforms
            if (velocity.y < Gravity.y / 10 || velocity.y > 0) {
                blockFromBelow = false;
                transform.parent = null;
            }


            //update player position
            TryMove(nextPosition);
        }
    }

    void TryMove(Vector2 movement) {

        //Move players with platform they are on
        if (standingPlat != null) {
            playerBody.position += new Vector2(standingPlat.transform.position.x - oldPlatPlace.x, standingPlat.transform.position.y - oldPlatPlace.y);
            oldPlatPlace = standingPlat.transform.position;
        }

        //Cast rigid body to find collisions
        int hitCount = playerBody.Cast(new Vector2(movement.x, movement.y), contactLayer, collisionCheck, movement.magnitude);
        float collisionDist = 0f;
        bool findGround = false;
        GameObject newPlat = standingPlat;

        //Check each collisison
        for (int i = 0; i < hitCount; i++) {
            if(collisionCheck[i].transform.gameObject.GetComponent<BalloonBehavior>() == null){
                Vector2 currentNormal = collisionCheck[i].normal;
                collisionDist = collisionCheck[i].distance;

                //Block one way platforms if moving down
                if (Vector2.Dot(currentNormal, this.Gravity) < 0 && (collisionCheck[i].transform.gameObject == standingPlat && standingPlat.transform.tag == "OneWay"))
                    blockFromBelow = true;

                //Move player the projection of the intended position parallel to collision
                if (Vector2.Dot(movement, currentNormal) < 0 && (collisionCheck[i].transform.tag != "OneWay" || blockFromBelow)) {
                    this.velocity -= Vector2.Dot(velocity, currentNormal) * currentNormal;
                    Vector2 moveInWall = Vector2.Dot(movement, currentNormal) * currentNormal;
                    movement -= moveInWall - collisionDist * moveInWall.normalized;
                }

                //Determine if the player is grounded and set fields relating to platform they are on
                if (Vector2.Dot(currentNormal, this.Gravity) < minGroundDirection && Vector2.Angle(currentNormal, Vector2.up) < slopeIsWallAngle && (isGrounded || collisionDist != 0) ){
                    findGround = true;
                    normal = currentNormal; 
                    groundTimer = 0f;
                    newPlat = collisionCheck[i].transform.gameObject;
                }
            }
        }

        //Update platform player is on and send message to platform
        if (standingPlat != newPlat || isGrounded) {
            standingPlat = newPlat;
            oldPlatPlace = standingPlat.transform.position;
            standingPlat.SendMessage("PlatformTrigger", null, SendMessageOptions.DontRequireReceiver);
        }

        //Update player position
        Vector2 finalPosition = movement + playerBody.position;
        this.isGrounded = findGround;
        playerBody.MovePosition(finalPosition);

    }

    public void SetDragMode()
    {
        canMove = false;
    }

    public void SetPlayMode()
    {
        canMove = true;
    }
        
    IEnumerator playSound(AudioClip clip)
    {
        source.clip = clip;
        source.PlayOneShot(clip);
        yield return null;
    }
}

