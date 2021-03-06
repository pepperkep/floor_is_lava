using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelController : MonoBehaviour
{


    [SerializeField] private ObjectivePoint[] objectiveList;
    [SerializeField] private GameObject lavaLevel;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject dragUI;

    [SerializeField] private AudioSource sourceSFX;
    [SerializeField] private AudioSource sourceMusic;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip music;


    private int currentObjective = 0;
    private bool lavaSwitch = false;
    public GameObject lava;
    public GameObject floor;
    public GameObject player;
    public float lavaSizeMultiplier;
    public int nextSceneBuildNumber;
    private WaterArea lavaArea;
    private Vector3 originalPlayerPosition;

    public Camera PlayCamera;
    public Camera DragCamera;
    public GameObject[] targetObjects;
    public Rigidbody2D playerBody;
    public PlayerMovement playerScript;
    private bool dragMode;

    public void EndLevel()
    {
        if(!winUI.activeSelf){
            winUI.SetActive(true);
            playerScript.canMove = false;
            TextMeshProUGUI textMesh = winUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            textMesh.text = textMesh.text + (int)Time.timeSinceLevelLoad / 60 + " minutes " + (int)Time.timeSinceLevelLoad % 60 + " seconds";
        }
    }

    public void StartNextLevel(){
        if (nextSceneBuildNumber != -1)
            SceneManager.LoadScene(nextSceneBuildNumber);
    }

    public void BeginLevel(bool restart, bool toDragMode)
    {
        settingsUI.SetActive(false);
        hudUI.SetActive(true);
        deathUI.SetActive(false);

        lava.transform.position = floor.transform.position;
        this.lavaArea = lava.GetComponent<WaterArea>();
        lavaArea.size = new Vector2(floor.transform.localScale.x, floor.transform.localScale.y);
        lava.SetActive(true);
        lavaLevel.SetActive(false);
        lavaArea.AdjustComponentSizes();
        lavaArea.RecomputeMesh();
        floor.SetActive(false);

        for(int i = 0; i < targetObjects.Length; i++){
            if(restart || (targetObjects[i].GetComponent<Furniture>() != null && targetObjects[i].GetComponent<Furniture>().destroyedOnLanding))
                targetObjects[i].SendMessage("OnLavaReset", null, SendMessageOptions.DontRequireReceiver);
            targetObjects[i].SetActive(true);
        }

        if(toDragMode){
            deathUI.SetActive(false);
            settingsUI.SetActive(false);
            hudUI.SetActive(true);
            floor.SetActive(true);
            lava.SetActive(false);
            dragUI.SetActive(true);
            settingsUI.transform.Find("Same Layout Button").gameObject.SetActive(false);
            playerScript.canMove = false;
            lavaLevel.SetActive(true);
            lavaLevel.transform.position = new Vector3(lavaLevel.transform.position.x, lava.transform.position.y + (lavaArea.size.y * lavaSizeMultiplier / 2) * lava.transform.localScale.y, lavaLevel.transform.position.z);
	        lavaSwitch = false;
            SetDragMode(); 
        }

        objectiveList[currentObjective].IsActive = false;
        currentObjective = 0;
        objectiveList[currentObjective].IsActive = true;

        sourceMusic.clip = music;
        sourceMusic.loop = true;
        sourceMusic.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        floor = GameObject.Find("Floor");
        player = GameObject.Find("Player");

        lava.transform.position = floor.transform.position;
        this.lavaArea = lava.GetComponent<WaterArea>();
        lavaArea.size = new Vector2(floor.transform.localScale.x, floor.transform.localScale.y);
        this.lavaArea = lava.GetComponent<WaterArea>();
        lavaLevel = GameObject.Find("Lava line");
        lavaLevel.SetActive(true);
        lavaLevel.transform.position = new Vector3(lavaLevel.transform.position.x, lava.transform.position.y + (lavaArea.size.y * lavaSizeMultiplier / 2) * lava.transform.localScale.y, lavaLevel.transform.position.z);

        deathUI.SetActive(false);
        settingsUI.SetActive(false);
        hudUI.SetActive(true);
        winUI.SetActive(false);
        floor.SetActive(true);
        lava.SetActive(false);

        settingsUI.transform.Find("Same Layout Button").gameObject.SetActive(false);

        originalPlayerPosition = player.transform.position;
        objectiveList[currentObjective].IsActive = true;

        Vector2 size = floor.GetComponent<BoxCollider2D>().bounds.size;

        playerBody = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<PlayerMovement>();
        SetDragMode();

        sourceMusic.clip = music;
        sourceMusic.loop = true;
        sourceMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dragMode && !lavaSwitch)
        {
            BeginLevel(false, false);
            lavaSwitch = true;
            settingsUI.transform.Find("Same Layout Button").gameObject.SetActive(true);
        }

        if (!objectiveList[currentObjective].IsActive)
        {
            if (currentObjective < objectiveList.Length - 1)
            {
                lavaArea.size = new Vector2(lavaArea.size.x, lavaArea.size.y * lavaSizeMultiplier);
                lavaArea.AdjustComponentSizes();
                lavaArea.RecomputeMesh();
                currentObjective++;
                objectiveList[currentObjective].IsActive = true;
                for (int i = 0; i < targetObjects.Length; i++)
                {
                    targetObjects[i].SendMessage("OnLavaRise", lava.transform.position.y + (lavaArea.size.y / 2) * lava.transform.localScale.y, SendMessageOptions.DontRequireReceiver);
                }
            }

            else
            {
                EndLevel();
            }
        }
    }

    public void RestartLevel(bool toDragMode){
        player = (GameObject)Instantiate(playerPrefab);
        player.transform.position = originalPlayerPosition;
        player.name = "Player";
        playerScript = player.GetComponent<PlayerMovement>();
        playerScript.canMove = true;
        if(!toDragMode){
            if(currentObjective > 0)
                BeginLevel(true, false);
            else
                BeginLevel(false, false);
        }
        else{
            SetDragMode();
            if(currentObjective > 0)
                BeginLevel(true, true);
            else
                BeginLevel(false, true);
        }
    }

    public void SetDragMode()
    {
        dragMode = true;
        PlayCamera.enabled = false;
        PlayCamera.gameObject.SetActive(false);
        DragCamera.enabled = true;
        DragCamera.gameObject.SetActive(true);
        playerScript.SetDragMode();
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].SendMessage("SetDragMode", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SetPlayMode()
    {
        dragMode = false;
        dragUI.SetActive(false);
        PlayCamera.enabled = true;
        PlayCamera.gameObject.SetActive(true);
        DragCamera.enabled = false;
        DragCamera.gameObject.SetActive(false);
        playerScript.SetPlayMode();
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].SendMessage("SetPlayMode", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void playClick()
    {
        float vol = sourceSFX.volume;
        if (vol > 0.3)
            sourceSFX.volume = vol - 0.3f;
        else
            sourceSFX.volume = 0.1f;
        sourceSFX.clip = click;
        sourceSFX.Stop();
        sourceSFX.PlayOneShot(click);
        sourceSFX.volume = vol;
    }
}
