using UnityEngine;
using UnityEngine.Tilemaps;
using RogueSharp;

public interface IInteractable
{
    void Interact();
    void Draw(IMap map, Tilemap tilemap);
}

