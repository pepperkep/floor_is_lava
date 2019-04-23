﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject title;

    private bool isCredits;

    private void Start()
    {
        levelSelect.SetActive(false);
        credits.SetActive(false);
        title.SetActive(true);
        isCredits = false;
        source.volume = 0.4f;
    }

    public void Play(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);
        title.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LevelSelect()
    {
        levelSelect.SetActive(true);
        title.SetActive(false);
    }

    public void Credits()
    {
        credits.SetActive(true);
        title.SetActive(false);
        isCredits = true;
    }

    public void Close()
    {
        if (isCredits) {
            credits.SetActive(false);
            isCredits = false;
        }
        else
            levelSelect.SetActive(false);
        title.SetActive(true);
    }

    public void playClick() { 
        source.Play();
    }
}
