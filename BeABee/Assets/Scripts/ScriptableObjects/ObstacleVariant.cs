using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ObstacleVariant")]
public class ObstacleVariant : ScriptableObject, IScriptableObject
{
    [SerializeField] public EObstacleType ObstacleType;
    [SerializeField] public List<bool> WallsActive;
    [SerializeField] public int BirdsNeededForPassingThrough;
    [SerializeField] public Sprite Sprite;
    [SerializeField] public EMessageType MessageTypeOnPassing;
    [SerializeField] public float Speed;
    [SerializeField] public int ScoreOnPass;
    [SerializeField] public bool IsBoss;
    [SerializeField] public bool ExpandWorldUp;
    [Range(0, 1)]
    [SerializeField] public float ProbabilityToSpawn;
}
