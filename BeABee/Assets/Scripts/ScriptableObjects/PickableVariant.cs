using System;
using System.Collections.Generic;
using UnityEngine;
using PubSub;

[CreateAssetMenu(menuName = "SO/PickableVariant")]

public class PickableVariant : ScriptableObject, IScriptableObject
{
    [Header("Pickable Settings")]
    [SerializeField] public float YOffsetMin;
    [SerializeField] public float YOffsetMax;
    [SerializeField] public float Speed;
    [SerializeField] public ParticleSystem OnPickUpEffect;
    [SerializeField] public Sprite Sprite;
    [SerializeField] public Vector3 InitialScale;
    [Range(0,1)]
    [SerializeField] public float ProbabilityToSpawn;

    [Header("Pickable Life Reason")]
    [SerializeField] public EPickableType PickableType;
    [SerializeField] public EMessageType MessageType;
    [SerializeField] public EBirdType BirdType;
    [SerializeField] public int Score;

    public float RandomPosition()
    {
        return UnityEngine.Random.Range(YOffsetMin, YOffsetMax);
    }

}
