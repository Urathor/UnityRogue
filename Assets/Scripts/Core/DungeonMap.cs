using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp;
using UnityEngine.Tilemaps;

public class DungeonMap : Map
{
    public List<Monster> Monsters;
    public List<Rectangle> Rooms;
    public List<Door> Doors;
    public List<Loot> Loots;
    public Stairs UpStairs;
    public Stairs DownStairs;

    public DungeonMap()
    {
        GameManager.SchedulingSystem.Clear();
        Monsters = new List<Monster>();
        Rooms = new List<Rectangle>();
        Doors = new List<Door>();
        Loots = new List<Loot>();
    }

    public void SetTilemapTileforCell(Tilemap tilemap, Cell cell)
    {
        if (!cell.IsExplored)
            return;

        if (cell.IsWalkable)
        {
            tilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), GameManager.Instance.Floor);
        }
        else
        {
            tilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), GameManager.Instance.Wall);
        }

        if (IsInFov(cell.X,cell.Y))
        {
            tilemap.SetColor(new Vector3Int(cell.X, cell.Y, 0), Color.white);
        }
        else if (!IsInFov(cell.X,cell.Y))
        {
            tilemap.SetColor(new Vector3Int(cell.X, cell.Y, 0), Color.grey);
        }
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        Cell cell = (Cell)GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    public bool SetActorPosition(Actor actor, int X, int Y, bool canOpenDoors)
    {
        if(GetCell(X,Y).IsWalkable && GetDoorAt(X,Y) == null)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));
            actor.X = X;
            actor.Y = Y;
            actor.go.transform.position = new Vector3(X, Y, 0);
            actor.Draw(this);
            SetIsWalkable(actor.X, actor.Y, false);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));

            if (actor is Player)
                UpdatePlayerFoV();

            return true;
        }
        else if ( GetCell(X,Y).IsWalkable && !GetDoorAt(X,Y).IsOpen && canOpenDoors)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));
            actor.X = X;
            actor.Y = Y;
            actor.go.transform.position = new Vector3(X, Y, 0);
            actor.Draw(this);
            SetIsWalkable(actor.X, actor.Y, false);
            OpenDoor(actor, X, Y);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));

            if (actor is Player)
                UpdatePlayerFoV();

            return true;
        }
        else if (GetCell(X, Y).IsWalkable && GetDoorAt(X, Y).IsOpen)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));
            actor.X = X;
            actor.Y = Y;
            actor.go.transform.position = new Vector3(X, Y, 0);
            actor.Draw(this);
            SetIsWalkable(actor.X, actor.Y, false);
            OpenDoor(actor, X, Y);
            SetTilemapTileforCell(GameManager.BoardHolder, (Cell)GetCell(actor.X, actor.Y));

            if (actor is Player)
                UpdatePlayerFoV();

            return true;
        }

        return false;
    }

    public bool DoesRoomHaveWalkableSpace(Rectangle room)
    {
        for(int x = 1; x <= room.Width - 2; x++)
        {
            for(int y = 1; y <= room.Height - 2; y++)
            {
                if(IsWalkable(x + room.X ,y + room.Y))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Point GetRandomLocation()
    {
        int room = GameManager.Random.Next(0, Rooms.Count - 1);

        if(!DoesRoomHaveWalkableSpace(Rooms[room]))
        {
            GetRandomLocation();
        }

        return GetRandomLocationInRoom(Rooms[room]);
    }

    public Point GetRandomLocationInRoom(Rectangle room)
    {

        int x = GameManager.Random.Next(1, room.Width - 2) + room.X;
        int y = GameManager.Random.Next(1, room.Height - 2) + room.Y;

        if (!IsWalkable(x, y))
        {
            GetRandomLocationInRoom(room);
        }
        return new Point(x, y);

    }

    public Monster GetMonsterAt(int x, int y)
    {
        return Monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    }

    public Door GetDoorAt(int x, int y)
    {
        return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
    }

    public void OpenDoor(Actor actor, int x, int y)
    {
        Door door = GetDoorAt(x, y);

        if(door != null && !door.IsOpen)
        {
            door.Interact();
            var cell = GetCell(x, y);
            SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
        }
    }

    public Loot GetLootAt(int x, int y)
    {
        return Loots.LastOrDefault(d => d.X == x && d.Y == y);
    }

    public void AddLoot(Loot loot)
    {
        Loots.Add(loot);
        SetIsWalkable(loot.X, loot.Y, true);
    }

    public void RemoveLoot(Loot loot)
    {
        Loots.Remove(loot);
        loot.gameObject.SetActive(false);
    }

    public void AddMonster(Monster monster)
    {
        Monsters.Add(monster);
        SetIsWalkable(monster.X, monster.Y,false);
        GameManager.SchedulingSystem.Add(monster);
        
    }

    public void RemoveMonster(Monster monster)
    {
        Monsters.Remove(monster);
        monster.gameObject.SetActive(false);
        SetIsWalkable(monster.X, monster.Y, true);
        GameManager.SchedulingSystem.Remove(monster);
    }

    public void AddPlayer(Player player)
    {
        GameManager.Player = player;
        SetIsWalkable(player.X, player.Y, false);
        UpdatePlayerFoV();
        GameManager.SchedulingSystem.Add(player);
    }

    public void UpdatePlayerFoV()
    {
        Player player = GameManager.Player;

        ComputeFov(player.X, player.Y, player.Awareness, true);

        foreach(Cell cell in GetAllCells())
        {
            if(IsInFov(cell.X,cell.Y))
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
    }
     
    public bool CanMoveDownStairs()
    {
        return DownStairs.X == GameManager.Player.X && DownStairs.Y == GameManager.Player.Y;
    }

    public void Draw( Tilemap tilemap)
    { 
        tilemap.ClearAllTiles();
        foreach ( Cell cell in GetAllCells())
        {
            SetTilemapTileforCell(tilemap, cell);
        }

        foreach (Door door in Doors)
        {
            door.Draw(this, tilemap);
        }

        foreach (Monster monster in Monsters)
        {
            monster.Draw(this);

            if(IsInFov(monster.X,monster.Y))
            {
                monster.DrawStats();
            }
            else
            {
                EnemyPanel.Instance.RemoveMonster(monster);
            }

            if (GetCell(monster.X, monster.Y).IsExplored)
            {
                tilemap.SetTile(new Vector3Int(monster.X, monster.Y, 0), GameManager.Instance.Floor);
            }

            
        }

        foreach(Loot loot in Loots)
        {
            loot.Draw(this, tilemap);
        }

        UpStairs.Draw(this, tilemap);
        DownStairs.Draw(this, tilemap);

        GameManager.Player.Draw(this);
        tilemap.SetTile(new Vector3Int(GameManager.Player.X, GameManager.Player.Y, 0), GameManager.Instance.Floor);
    }
}
