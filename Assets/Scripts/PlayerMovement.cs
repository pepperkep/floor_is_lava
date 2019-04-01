using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpAmount;

    private Rigidbody2D playerBody;
    private CapsuleCollider2D collisionBox;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        collisionBox = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMove = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 totalMove = new Vector2(xMove, playerBody.velocity.y);
        playerBody.velocity = totalMove;

        Vector2 bot = collisionBox.bounds.min;
        Vector2 groundCheckLeft = new Vector2(bot.x - 0.4f, bot.y - 0.1f);
        Vector2 groundCheckRight = new Vector2(bot.x + 0.4f, bot.y - 0.1f);
        Collider2D hit = Physics2D.OverlapArea(groundCheckLeft, groundCheckRight);
        isGrounded = hit == null ? false : true;

        if(isGrounded && Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
    }

    void Jump(){
        playerBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
    }
}
