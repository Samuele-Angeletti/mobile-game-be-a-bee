using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BirdVariant")]

public class BirdVariant : ScriptableObject
{
    [Header("Bird Setup")]
    [SerializeField] public Sprite MainSprite;
    [SerializeField] public EBirdType BirdType;
    [SerializeField] public float JumpForce;
    [SerializeField] public int StartScale;
    [SerializeField] public float Gravity;
    [SerializeField] public float MaxSpeed;
    [Tooltip("This value is to decelerate the jump. Higher is this value and shorter is the jump")]
    [SerializeField] public float MinDelta;
    [SerializeField] public float JumpHeigth;
    [SerializeField] public float XSpeed;
    [Header("Follow the Leader Settings")]
    [SerializeField] public float FollowSpeed;
    [SerializeField] public float SlowDownDistance;
}
