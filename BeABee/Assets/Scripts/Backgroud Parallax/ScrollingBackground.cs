using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] public float speed;

    [SerializeField] public Renderer bgRenderer;

    [SerializeField] public BackgroundManager backgroundManager;

    bool directionLeft => backgroundManager.directionLeft;
    bool isActive => backgroundManager.scrollingIsActive;

    void Update()
    {
        if(isActive)
            bgRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime * (directionLeft ? 1 : -1), 0);
    }
}
