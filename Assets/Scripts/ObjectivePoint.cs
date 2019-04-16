using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePoint : MonoBehaviour
{
    public AudioClip clip;

    private bool isActive = false;

    public bool IsActive{
        get => isActive;
        set{
            if (value)
                activeParticles.Play();
            else
            {
                SoundManager.manager.PlaySingle(clip);
                activeParticles.Stop();
            }
            isActive = value;
        }
    }
    
    private ParticleSystem activeParticles;

    void Awake(){
        activeParticles = GetComponent<ParticleSystem>();  
        activeParticles.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {

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
