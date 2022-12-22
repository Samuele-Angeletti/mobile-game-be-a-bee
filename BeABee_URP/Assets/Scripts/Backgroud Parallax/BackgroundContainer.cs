using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundContainer : MonoBehaviour
{
    [Tooltip("Lo scenario di apaprtenenza della texture")]
    [SerializeField] public EScenario sceneryType;

    [Tooltip("Imposta se seguire l'ordine della lista per la visualizzazione degli elementi (l'ultimo della lista è più sullo sfondo)")]
    [SerializeField] public bool followListOrder = true;

    [Tooltip("Gli elementi che compongono il background vanno messi in questa lista")]
    [SerializeField] public List<BackgroundElement> backgroundElements;
}
