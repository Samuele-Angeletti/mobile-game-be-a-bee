using System;
using UnityEngine;

[Serializable] public class BackgroundElement
{
    [Tooltip("Mettere il Game Object che contiene lo Sprite Renderer con l'immagine voluta.")]
    [SerializeField] public Material texture;

    [Tooltip("Imposta la velocità relativa dell'oggetto rispetto a quella base.")]
    [Range(0f,2f)]
    [SerializeField] public float relativeSpeed = 1f;

    [Tooltip("Imposta l'ordine della texture se non viene usato quello della lista")]
    [SerializeField] public int order;
}
