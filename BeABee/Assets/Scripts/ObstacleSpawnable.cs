using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnable : Spawnable
{
    [SerializeField] EObstacleType obstacleType;
    [SerializeField] BoxCollider2D upCollider;
    [SerializeField] BoxCollider2D middleCollider;
    [SerializeField] BoxCollider2D lowCollider;

    public override void Initialize(Vector3 deathPosition)
    {
        base.Initialize(deathPosition);
        
        ObstacleType = obstacleType;

        switch (obstacleType)
        {
            case EObstacleType.UpOnly:
                upCollider.gameObject.SetActive(true);
                middleCollider.gameObject.SetActive(false);
                lowCollider.gameObject.SetActive(false);
                break;
            case EObstacleType.MiddleOnly:
                upCollider.gameObject.SetActive(false);
                middleCollider.gameObject.SetActive(true);
                lowCollider.gameObject.SetActive(false);
                break;
            case EObstacleType.LowOnly:
                upCollider.gameObject.SetActive(false);
                middleCollider.gameObject.SetActive(false);
                lowCollider.gameObject.SetActive(true);
                break;
            case EObstacleType.UpMiddle:
                upCollider.gameObject.SetActive(true);
                middleCollider.gameObject.SetActive(true);
                lowCollider.gameObject.SetActive(false);
                break;
            case EObstacleType.LowMiddle:
                upCollider.gameObject.SetActive(false);
                middleCollider.gameObject.SetActive(true);
                lowCollider.gameObject.SetActive(true);
                break;
            case EObstacleType.UpLow:
                upCollider.gameObject.SetActive(true);
                middleCollider.gameObject.SetActive(false);
                lowCollider.gameObject.SetActive(true);
                break;
        }
    }
}

public enum EObstacleType
{
    UpOnly,
    MiddleOnly,
    LowOnly,
    UpMiddle,
    LowMiddle,
    UpLow
}
