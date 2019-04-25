
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private AudioSource sourceSFX;
    [SerializeField] private AudioSource sourceMusic;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject hudCanvas;
    // Start is called before the first frame update
    public void MusicVolume()
    {
        sourceMusic.volume = sliderMusic.value;
    }

    public void SFXVolume()
    {
        sourceSFX.volume = sliderSFX.value;
    }

    void Start()
    {
        sliderMusic.value = sliderMusic.value;
    }

    public void Open()
    {
        settingsCanvas.SetActive(true);
        hudCanvas.SetActive(false);
    }
    public void Close()
    {
        settingsCanvas.SetActive(false);
        hudCanvas.SetActive(true);
    }

}