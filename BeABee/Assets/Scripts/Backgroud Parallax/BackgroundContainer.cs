using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class BackgroundElement
{
    [Tooltip("Mettere il Game Object che contiene lo Sprite Renderer con l'immagine voluta.")]
    [SerializeField] public GameObject image;

    [Tooltip("Imposta la velocità relativa dell'oggetto rispetto a quella base.")]
    [Range(0.1f,2f)]
    [SerializeField] public float relativeSpeed = 1f;
}

public class BackgroundContainer : MonoBehaviour
{
    [Tooltip("Gli elementi che compongono il background vanno messi in questa lista")]
    [SerializeField] List<BackgroundElement> backgroundElements;

    [Tooltip("Imposta se seguire l'ordine della lista per la visualizzazione degli elmenti (quello più in alto più indietro)")]
    [SerializeField] bool followListOrder = true;

    [SerializeField] bool directionLeft = true;

    //La velocità di base del background;
    float baseSpeed;

    private void MoveElement(BackgroundElement element)
    {
        float direction;
        if (directionLeft)
            direction = 1;
        else
            direction = -1;

        element.image.transform.Translate(Vector3.left * direction * element.relativeSpeed * baseSpeed * Time.deltaTime);
    }

}
