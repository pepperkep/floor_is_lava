﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDeath : MonoBehaviour
{
    public AudioSource source;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject hudCanvas;
    BoxCollider2D m_ObjectCollider;
    Collision myCol;
    [SerializeField] private AudioClip clip;


    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<BoxCollider2D>();
        gameOverCanvas.SetActive(false);
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
            gameOverCanvas.SetActive(true);
            hudCanvas.SetActive(false);
            source.Stop();
            source.clip = clip;
            source.loop = false;
            source.PlayOneShot(clip);
        }
    }

}