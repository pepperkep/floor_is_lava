using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalSoundScript : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip goal;
    bool isPlayMode;
    bool canPlay;


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && isPlayMode && canPlay)
        {
            StartCoroutine(playSound());
        }
    }


    public void SetPlayMode()
    {
        isPlayMode = true;
        canPlay = true;
    }

    public void SetDragMode()
    {
        isPlayMode = false;
    }

    IEnumerator playSound()
    {
        canPlay = false;
        source.clip = goal;
        source.PlayOneShot(goal);
        yield return new WaitForSeconds(10f);
        canPlay = true;
    }
}
