using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip click;
    // Start is called before the first frame update
    void Start()
    {
        source = GameObject.Find("SFXManager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        source.clip = click;
        source.Stop();
        source.PlayOneShot(click);
    }

    void OnMouseUp()
    {
        source.clip = click;
        source.Stop();
        source.PlayOneShot(click);
    }
}
