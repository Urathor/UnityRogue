using System.Collections;
using System.Collections.Generic;
using RogueSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Loot : Entity,IInteractable
{
    public SpriteRenderer Renderer;
    public string ItemName;
    public ItemType Type;

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }
    public void Draw(IMap map, Tilemap tilemap)
    {
        if (!map.GetCell(X, Y).IsExplored)
        {
            if (Renderer.enabled)
                Renderer.enabled = false;
            return;
        }
        else
        {
            Renderer.enabled = true;
        }
        if (map.IsInFov(X, Y))
        {
            Renderer.color = Color.white;
        }
        else if (!map.IsInFov(X, Y))
        {
            Renderer.color = Color.gray;
        }
    }

    public void Interact()
    {
        GameManager.InventorySystem.AddItem(ItemName, Type);
        GameManager.DungeonMap.RemoveLoot(this);
    }
}
