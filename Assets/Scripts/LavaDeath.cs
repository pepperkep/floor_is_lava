using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDeath : MonoBehaviour
{
    BoxCollider2D m_ObjectCollider;
    Collision myCol;
    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
       
       

    }

    void OnTriggerEnter2D(Collider2D myCol)
    {
        if (myCol.gameObject.name == "Player")
        {
            Destroy(myCol.gameObject);
        }
    }

}
