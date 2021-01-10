using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using RogueSharp;


public class Stairs : Entity, IInteractable
{
    public SpriteRenderer Renderer;
    public Sprite UpSprite;
    public Sprite DownSprite;

    public bool IsUp;
    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {

    }

    public void Draw(IMap map, Tilemap tilemap)
    {
        if (!map.GetCell(X, Y).IsExplored)
            return;


        Renderer.sprite = IsUp ? UpSprite : DownSprite;

        if (map.IsInFov(X, Y))
        {
            Renderer.color = Color.white;
        }
        else if (!map.IsInFov(X, Y))
        {
            Renderer.color = Color.gray;
        }
    }
}
