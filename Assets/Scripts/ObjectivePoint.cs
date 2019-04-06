using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePoint : MonoBehaviour
{

    private bool isActive = false;

    public bool IsActive{
        get => isActive;
        set{
            if(value)
                activeParticles.Play();
            else
                activeParticles.Stop();
            isActive = value;
        }
    }
    
    private ParticleSystem activeParticles;

    // Start is called before the first frame update
    void Start()
    {
        activeParticles = GetComponent<ParticleSystem>();  
        activeParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.name == "Player")
            IsActive = false;
    }
}
