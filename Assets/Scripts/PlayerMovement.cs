
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSource source;


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
    private Vector2 velocity = Vector2.zero;

    [SerializeField] private AudioClip jump;

    //Field for scene control
    public bool canMove = false;

    //Properties for movement fields
    public float GroundAcceleration {
        get => this.groundAcceleration;
        set => this.groundAcceleration = value;
    }

    //Field for animator
    public Animator animator;
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
    private Vector2 targetVelocity = Vector2.zero;
    private Vector2 normal;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private bool isGrounded = false;
    private bool faceRight = true;
    private RaycastHit2D[] collisionCheck = new RaycastHit2D[8];
    private ContactFilter2D contactLayer;
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
    private bool justJumped = false;
    private bool groundCheckPass = false;


    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("faceRight", true);
        contactLayer.useTriggers = false;
        contactLayer.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactLayer.useLayerMask = true;
        standingPlat = null;
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
                if (playerIn == 0 && (animator.GetBool("faceRight")==false))
                {
                    animator.SetBool("walkLeft", false);
                }

                if ((playerIn < 0 && nextVelocity.x > 0) || (playerIn > 0 && nextVelocity.x < 0))
                {
                    if (playerIn == 0)
                    {  
                        faceRight = !faceRight;
                    }
                    else
                    {

                        nextVelocity.x = playerIn * GroundAcceleration * turnAroundMultiplier;
                    }
                }
                else
                {
                    if (playerIn != 0)
                    {
                       nextVelocity += playerIn * GroundAcceleration * Time.deltaTime * new Vector2(normal.y, -normal.x);

                    }
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
            if ((Input.GetButtonDown("Jump") || bufferedJump))
            {
                if (isGrounded || (groundTimer < leavePlatformJumpTolerance && velocity.y < 0))
                {
                    nextVelocity.y = JumpVelocity;
                    StartCoroutine(playSound(jump));
                    bufferedJump = false;
                    justJumped = true;
                }
                else
                {
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
            Debug.Log(groundCheckPass);
            groundTimer += Time.fixedDeltaTime;

            //Determine distance to ground
            int hitCount = playerBody.Cast(Gravity, contactLayer, collisionCheck, Gravity.magnitude);
            float currentDistance = 0;
            for (int i = 0; i < hitCount; i++) {
                if (collisionCheck[i].distance > currentDistance)
                    currentDistance = collisionCheck[i].distance;
            }
            distanceToGround = currentDistance;

            //Detect if player is grounded
            if (Math.Abs(targetVelocity.x) < MaxSpeed)
                this.velocity = this.targetVelocity;
            else {
                if (targetVelocity.x > 0)
                    this.velocity = new Vector2(MaxSpeed, targetVelocity.y);
                if (targetVelocity.x < 0)
                    this.velocity = new Vector2(-MaxSpeed, targetVelocity.y);
            }
            Vector2 nextPosition;
            if (isGrounded)
                nextPosition = this.velocity * Time.fixedDeltaTime;
            else
                nextPosition = this.velocity * Time.fixedDeltaTime + 0.5f * this.Gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            Vector2 oldGravity = Gravity;
            this.velocity += 0.5f * (Gravity + oldGravity) * Time.fixedDeltaTime;

            if (velocity.y < Gravity.y / 10 || velocity.y > 0) {
                blockFromBelow = false;
                transform.parent = null;
            }


            //update player position
            TryMove(nextPosition);
        }
    }

    void TryMove(Vector2 movement) {
        if (standingPlat != null) {
            playerBody.position += new Vector2(standingPlat.transform.position.x - oldPlatPlace.x, standingPlat.transform.position.y - oldPlatPlace.y);
            oldPlatPlace = standingPlat.transform.position;
        }

        int hitCount = playerBody.Cast(new Vector2(movement.x, movement.y), contactLayer, collisionCheck, movement.magnitude);
        float collisionDist = 0f;
        bool findGround = false;
        GameObject newPlat = standingPlat;
        for (int i = 0; i < hitCount; i++) {
            if(collisionCheck[i].transform.gameObject.GetComponent<BalloonBehavior>() == null){
                Vector2 currentNormal = collisionCheck[i].normal;
                collisionDist = collisionCheck[i].distance;
                if (Vector2.Dot(currentNormal, this.Gravity) < 0 && (collisionCheck[i].transform.gameObject == standingPlat && standingPlat.transform.tag == "OneWay"))
                    blockFromBelow = true;
                if (Vector2.Dot(movement, currentNormal) < 0 && (collisionCheck[i].transform.tag != "OneWay" || blockFromBelow)) {
                    this.velocity -= Vector2.Dot(velocity, currentNormal) * currentNormal;
                    Vector2 moveInWall = Vector2.Dot(movement, currentNormal) * currentNormal;
                    movement -= moveInWall - collisionDist * moveInWall.normalized;
                }
                if (Vector2.Dot(currentNormal, this.Gravity) < minGroundDirection && Vector2.Angle(currentNormal, Vector2.up) < slopeIsWallAngle && (isGrounded || collisionDist != 0 || groundCheckPass) && !justJumped){
                    findGround = true;
                    normal = currentNormal; 
                    groundTimer = 0f;
                    newPlat = collisionCheck[i].transform.gameObject;
                }
                if(groundCheckPass && !justJumped)
                    groundCheckPass = false;
                else{
                    if(justJumped)
                        groundCheckPass = true;
                }
            }
        }
        if (standingPlat != newPlat || isGrounded) {
            standingPlat = newPlat;
            oldPlatPlace = standingPlat.transform.position;
            standingPlat.SendMessage("PlatformTrigger", null, SendMessageOptions.DontRequireReceiver);
        }
        if(justJumped){
            justJumped = false;
        }
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

