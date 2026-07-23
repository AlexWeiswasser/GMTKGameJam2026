using UnityEngine;
using System;

public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    void Awake()
    {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        spr.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}
