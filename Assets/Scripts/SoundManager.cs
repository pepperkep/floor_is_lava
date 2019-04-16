using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource source;

    public static SoundManager manager;
    
    public void PlaySingle(AudioClip clip)
    {
        source.clip = clip;
        source.PlayOneShot(clip);
    }




}
