using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform followTarget;
    [SerializeField] private PlayerMovement velocityCheck;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.05f;
    public float adjustDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(followTarget != null && (Math.Abs(transform.position.x - followTarget.position.x) > adjustDistance) && Math.Abs(velocityCheck.Velocity.x) != 0){
            Vector3 targetPosition = new Vector3(followTarget.position.x, transform.position.y, transform.position.z);
            if((targetPosition.x > transform.position.x && velocityCheck.Velocity.x >= 0) || (targetPosition.x < transform.position.x && velocityCheck.Velocity.x <= 0))
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        if(followTarget == null){
            GameObject player = GameObject.Find("Player");
            if(player != null){
                followTarget = player.transform;
                velocityCheck = player.GetComponent<PlayerMovement>();
                transform.position = new Vector3(followTarget.position.x, transform.position.y, transform.position.z);
            }
        }
    }
}
