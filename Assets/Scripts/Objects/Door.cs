using RogueSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Entity, IInteractable
{
    public SpriteRenderer Renderer;
    public Sprite ClosedDoorSprite;
    public Sprite OpenDoorSprite;
    public bool IsOpen;
    public bool IsLocked;

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        IsOpen = true;
    }

    public void Draw(IMap map, Tilemap tilemap)
    {
        if (!map.GetCell(X, Y).IsExplored)
            return;

        Renderer.sprite = IsOpen ? OpenDoorSprite : ClosedDoorSprite;

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
