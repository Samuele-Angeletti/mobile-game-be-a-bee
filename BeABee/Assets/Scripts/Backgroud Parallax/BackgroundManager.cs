using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Tooltip("Imposta la velocit� di base dei backgroud.")]
    [SerializeField] float baseSpeed;

    [SerializeField] List<BackgroundContainer> Backgrounds;

    BackgroundContainer activeBackground;

    bool isMoving;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }




}
