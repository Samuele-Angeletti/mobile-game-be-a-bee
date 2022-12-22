using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Tooltip("Imposta la velocità di scorrimento orizzontale dei backgroud.")]
    [SerializeField] float baseHorizontalSpeed;
    [Tooltip("Imposta la durata del cambio di scenario")]
    [SerializeField] float changeSceneryTime;

    [Tooltip("Imposta il prefab per il background")]
    [SerializeField] GameObject quadPrefab;

    [Tooltip("Imposta il centro dello spawn del background")]
    [SerializeField] Transform centerSpawnPoint;

    [Tooltip("Imposta il tipo dello scenario di partenza")]
    [SerializeField] EScenario startingScenery;
    public EScenario activeScenery { get; private set; }

    [Tooltip("Imposta se la direzione dello scorrimento è verso sinistra o destra")]
    [SerializeField] public bool directionLeft = true;
    [Tooltip("Imposta se lo scorrimento orizzontale dellos cenario è attivo o no")]
    [SerializeField] public bool scrollingIsActive;

    [SerializeField] public List<BackgroundContainer> backgrounds;

    GameObject activeBackgroundContainer;
    GameObject newBackgroundContainer;

    Transform mover;

    float position;

    Vector3 startPosition;
    Vector3 upperSpawnPoint;
    Vector3 bottomSpawnPoint;

    bool isChangingScene;

    void Start()
    {
        ResetMoving();
        activeBackgroundContainer = SpawnBackground(startingScenery, centerSpawnPoint.position);
        CalculateSpawnPoint();

        GameManager.Instance.onGameOver += () => scrollingIsActive = false;
        GameManager.Instance.onGameStart += () => scrollingIsActive = true;
    }

    private void ResetMoving()
    {
        isChangingScene = false;
        position = 0;
        startPosition = Vector3.zero;
    }

    private void CalculateSpawnPoint()
    {
        if(quadPrefab == null)
        {
            Debug.LogError("Non è stato assegnato nessun QuadPrefab, gli SpawnPoint non sono stati calcolati");
            return;
        }

        float dinamicY = quadPrefab.transform.localScale.y;
        upperSpawnPoint = new Vector3(centerSpawnPoint.position.x, dinamicY, centerSpawnPoint.position.z);
        bottomSpawnPoint = new Vector3(centerSpawnPoint.position.x, -dinamicY, centerSpawnPoint.position.z);
    }

    void Update()
    {
        if (isChangingScene)
        {
            MovingScenery();
        }

        // input per test
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    ChangeScenery(EScenario.Mountain, true);
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    ChangeScenery(EScenario.Sky, false);
        //}
    }

    private void MovingScenery()
    {
        if(Vector3.Distance(mover.position, centerSpawnPoint.position) < 0.05)
        {
            mover.position = centerSpawnPoint.position;
            activeBackgroundContainer = newBackgroundContainer;
            activeBackgroundContainer.transform.SetParent(this.gameObject.transform);
            activeScenery = activeBackgroundContainer.GetComponent<BackgroundContainer>().sceneryType;
            Destroy(mover.gameObject);
            ResetMoving();
        }
        else
        {
            position += Time.deltaTime / changeSceneryTime;
            mover.position = Vector3.Lerp(startPosition, centerSpawnPoint.position, position);
        }
    }

    public GameObject SpawnBackground(EScenario sceneryType, Vector3 position)
    {
        BackgroundContainer bgc = backgrounds.FindLast(bg => bg.sceneryType == sceneryType);
        if(bgc == null)
        {
            bgc = backgrounds[0];
            if (bgc == null)
            {
                Debug.LogError("Nessun Background Trovato");
                return null;
            }
        }

        GameObject backgroundContainer = Instantiate(bgc.gameObject, position, Quaternion.identity, this.gameObject.transform);

        int order = 1;

        foreach(BackgroundElement bge in  bgc.backgroundElements)
        {
            Vector3 spawnPosition = new Vector3(position.x, position.y, bgc.followListOrder ? order : bge.order);
            GameObject quad = Instantiate(quadPrefab, spawnPosition, Quaternion.identity, backgroundContainer.transform);
            quad.GetComponent<ScrollingBackground>().speed = baseHorizontalSpeed * bge.relativeSpeed;
            quad.GetComponent<ScrollingBackground>().bgRenderer.material = bge.texture;
            quad.GetComponent<ScrollingBackground>().backgroundManager = this;

            order++;
        }

        return backgroundContainer;
    }

   
    public void ChangeScenery(EScenario newScenery, bool beeGoingUp)
    {
        mover = Instantiate(new GameObject("Mover"), this.gameObject.transform).transform;

        mover.localPosition = beeGoingUp ? upperSpawnPoint : bottomSpawnPoint;
        startPosition = mover.localPosition;

        newBackgroundContainer = SpawnBackground(newScenery, mover.position);

        newBackgroundContainer.transform.SetParent(mover);
        activeBackgroundContainer.transform.SetParent(mover);
        
        isChangingScene = true;
    }




}
