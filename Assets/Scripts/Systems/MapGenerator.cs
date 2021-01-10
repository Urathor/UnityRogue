using UnityEngine;
using RogueSharp;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using RogueSharp.DiceNotation;
using System.Collections.Generic;

public class MapGenerator
{
    private readonly int width;
    private readonly int height;
    private readonly int maxNumRooms;
    private readonly int maxRoomSize;
    private readonly int minRoomSize;
    private readonly int level;
    private readonly DungeonMap map;
    private LootGenerator LootTable { get; set; }
    private MonsterGenerator MonsterTable { get; set; }


    public MapGenerator( int w, int h, int rooms, int maxSize, int minSize, int lvl)
    {
        width = w;
        height = h;
        maxNumRooms = rooms;
        maxRoomSize = maxSize;
        minRoomSize = minSize;
        level = lvl;

        map = new DungeonMap();

        MonsterTable = new MonsterGenerator();
        SetupMonsterSpawnTable();

        LootTable = new LootGenerator();
        SetupLootTable();

    }

    public void SetupMonsterSpawnTable()
    {
        MonsterTable.MonsterList = new List<MobSpawn>()
        {
            //common
            new MobSpawn("Koball", 12),
            new MobSpawn("Hob Gobbo", 12),
            new MobSpawn("Emu", 12),
            new MobSpawn("Snek",10),
            new MobSpawn("Bat",10),
            new MobSpawn("Krow", 10),
            //uncommon
            new MobSpawn("Fly-Trap",4.5f),
            new MobSpawn("Quagga", 4.5f),
            //rare
            new MobSpawn("Centaur", 1f),
            new MobSpawn("Yeti", 1f),
            //boss
            new MobSpawn("Jabberwok", .25f),
        };

        MonsterTable.SetPairs();
    }

    public void SetupLootTable()
    {
        LootTable.LootList = new List<LootDrop>
        {
            //foods
            new LootDrop ("Simple Bread", ItemType.Consumable, 15),
            new LootDrop ("Fine Aged Cheese", ItemType.Consumable, 8),
            new LootDrop ("Molded Cheese", ItemType.Consumable, 10),
            
            //potions
            new LootDrop ("Minor Heal Potion", ItemType.Consumable, 8),
            new LootDrop ("Swamp Water", ItemType.Consumable, 2),


            //armors
            new LootDrop ("Plain Shirt",ItemType.Equipment,5),
            new LootDrop ("Plain Leather Armor", ItemType.Equipment,5),
            new LootDrop ("Plain Studded Leather Armor", ItemType.Equipment ,5),
            new LootDrop ("Plain Splintmail", ItemType.Equipment, 3),
            new LootDrop ("Plain Ringmail", ItemType.Equipment, 3),
            new LootDrop ("Plain Platemail", ItemType.Equipment, 2),
            
            //weapons
            new LootDrop ("Simple Quarterstaff", ItemType.Equipment, 5),
            new LootDrop ("Simple Dagger", ItemType.Equipment,5),
            new LootDrop ("Simple Mace",ItemType.Equipment,5),
            new LootDrop ("Simple Longsword", ItemType.Equipment,3),
            new LootDrop ("Simple Claymore", ItemType.Equipment, 2)

            //Scrolls

        };
    }

    public void ClearMap()
    {
        for (int i = 0; i < GameManager.ObjectHolder.childCount; i++)
        {
            GameObject.Destroy(GameManager.ObjectHolder.GetChild(i).gameObject);
        }

        GameObject.Find("FloorMap").GetComponent<Tilemap>().ClearAllTiles();

        map.Rooms.Clear();

        map.Doors.Clear();

        map.Monsters.Clear();
    }

    public DungeonMap CreateMap()
    {
        ClearMap();
        // Initialize every cell in the map by
        // setting walkable, transparency, and explored to true
        map.Initialize(width, height);

        CreateRooms();
        CreateCorridors();

        foreach (Rectangle room in map.Rooms)
        {
            DrawRoom(room);
            CreateDoors(room);
        }

        CreateStairs();

        PlacePlayer();

        PlaceMonsters();

        PlaceLoots();

        return map;
    }

    private void SpawnMonster(int x, int y)
    {
        MonsterData newMonster = MonsterTable.GetMonsterData();

        if(newMonster != null)
        {
            GameObject monsterObject = GameObject.Instantiate(Res.Instance.Monster, new Vector2(x, y), Quaternion.identity);
            Monster monster = monsterObject.GetComponent<Monster>();
            monster.Data = newMonster;
            monster.GetComponent<SpriteRenderer>().sprite = newMonster.Icon;
            monster.DisplayName = newMonster.Name;
            monster.X = x;
            monster.Y = y;
            monster.go = monsterObject;
            monster.go.name = newMonster.Name;
            monster.transform.SetParent(GameManager.ObjectHolder);
            monster.Init();
            map.AddMonster(monster);
        }
    }

    private void PlaceMonsters()
    {
        foreach(var room in map.Rooms)
        {
            var numMonsters = Dice.Roll("1d2");
                
            for (int i = 0; i < numMonsters; i++)
            {
                if (map.DoesRoomHaveWalkableSpace(room))
                {
                    Point randomLocation = (Point)map.GetRandomLocationInRoom(room);

                    if (randomLocation != null)
                    {
                        SpawnMonster(randomLocation.X, randomLocation.Y);
                    }
                }
            }
            
        }
    }

    private void DropLoot(int x , int y)
    {
        Item item = LootTable.GetDrop();

        if (item != null)
        {
            GameObject lootObject = GameObject.Instantiate(Res.Instance.Loot, new Vector2(x, y), Quaternion.identity);
            Loot loot = lootObject.GetComponent<Loot>();
            loot.ItemName = item.IdentifiedName;
            loot.Type = item.Type;
            loot.Renderer.sprite = item.ItemIcon;
            loot.X = x;
            loot.Y = y;
            loot.go = lootObject;
            lootObject.transform.SetParent(GameManager.ObjectHolder);

            map.AddLoot(loot);
        }
    }

    private void PlaceLoots()
    {
        foreach (var room in map.Rooms)
        {
            int chance = Dice.Roll("1D10");

            var numLoots = Dice.Roll("1d2");

            if (chance > 5)
            {
                for (int i = 0; i < numLoots; i++)
                {
                    if (map.DoesRoomHaveWalkableSpace(room))
                    {
                        Point randomLocation = (Point)map.GetRandomLocationInRoom(room);
                        if (randomLocation != null)
                        {
                            DropLoot(randomLocation.X, randomLocation.Y);
                        }
                    }
                }
            }
            
        }
    }

    private void PlacePlayer()
    {
        Player player = GameManager.Player;
        
        if (player == null)
        {

            player = GameObject.Instantiate(Res.Instance.PlayerPrefab).GetComponent<Player>();
            player.Init();
            player.go = player.gameObject;
            GameManager.Player = player;
        }
        player.X = map.Rooms[0].Center.X;
        player.Y = map.Rooms[0].Center.Y;
        map.SetActorPosition(player, player.X, player.Y,true);
        map.AddPlayer(player);
    }

    private void CreateRooms()
    {
        for (int r = maxNumRooms; r > 0; r--)
        {
            int roomWidth = GameManager.Random.Next(minRoomSize, maxRoomSize);
            int roomHeight = GameManager.Random.Next(minRoomSize, maxRoomSize);
            int roomX = GameManager.Random.Next(0, width - roomWidth - 1);
            int roomY = GameManager.Random.Next(0, height - roomHeight - 1);

            Rectangle newRoom = new Rectangle(roomX, roomY, roomWidth, roomHeight);

            bool newRoomIntersects = map.Rooms.Any(room => newRoom.Intersects(room));

            if (!newRoomIntersects)
                map.Rooms.Add(newRoom);
        }
    }

    private void DrawRoom(Rectangle room)
    {
        for (int x = room.Left + 1; x < room.Right; x++)
        {
            for(int y = room.Top + 1; y < room.Bottom; y++ )
            {
                map.SetCellProperties(x, y, true, true);
            }
        }
    }

    // Carve a tunnel out of the map parallel to the x-axis
    private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
    {
        for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
        {
            map.SetCellProperties(x, yPosition, true, true);
        }
    }

    // Carve a tunnel out of the map parallel to the y-axis
    private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
    {
        for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
        {
            map.SetCellProperties(xPosition, y, true, true);
        }
    }

    private void CreateCorridors()
    {
        // Iterate through each room that was generated
        // Don't do anything with the first room, so start at r = 1 instead of r = 0
        for (int r = 1; r < map.Rooms.Count; r++)
        {
            // For all remaing rooms get the center of the room and the previous room
            int previousRoomCenterX = map.Rooms[r - 1].Center.X;
            int previousRoomCenterY = map.Rooms[r - 1].Center.Y;
            int currentRoomCenterX = map.Rooms[r].Center.X;
            int currentRoomCenterY = map.Rooms[r].Center.Y;

            // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
            if (GameManager.Random.Next(1, 2) == 1)
            {
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
            }
            else
            {
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
            }
        }
    }

    private bool IsPotentialDoor(Cell cell)
    {
        if (!cell.IsWalkable)
            return false;

        Cell right = (Cell) map.GetCell(cell.X + 1, cell.Y);
        Cell left = (Cell) map.GetCell(cell.X - 1, cell.Y);
        Cell bottom = (Cell) map.GetCell(cell.X, cell.Y + 1);
        Cell top = (Cell) map.GetCell(cell.X, cell.Y - 1);

        if(map.GetDoorAt(cell.X,cell.Y) != null || 
            map.GetDoorAt(right.X,right.Y) != null ||
            map.GetDoorAt(left.X, left.Y) != null ||
            map.GetDoorAt(top.X, top.Y )!= null ||
            map.GetDoorAt(bottom.X, bottom.Y )!= null)
        {
            return false;
        }

        if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            return true;

        if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            return true;

        return false;
    }

    private void CreateDoors(Rectangle room)
    {
        int xMin = room.Left;
        int xMax = room.Right;
        int yMin = room.Top;
        int yMax = room.Bottom;

        List<ICell> borderCells = map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
        borderCells.AddRange(map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
        borderCells.AddRange(map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
        borderCells.AddRange(map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

        foreach(var cell in borderCells)
        {
            if(IsPotentialDoor((Cell)cell))
            {
                map.SetCellProperties(cell.X, cell.Y, false, true);
                GameObject doorObject = GameObject.Instantiate(Res.Instance.DoorObject, new Vector2(cell.X,cell.Y),Quaternion.identity);
                Door door = doorObject.GetComponent<Door>();
                door.X = cell.X;
                door.Y = cell.Y;
                door.IsOpen = false;
                door.go = doorObject;
                doorObject.transform.SetParent(GameManager.ObjectHolder);
                
                map.Doors.Add(door);

            }
        }
    }

    private void CreateStairs()
    {
        int upX = map.Rooms.First().Center.X + 1;
        int upY = map.Rooms.First().Center.Y;

        GameObject stairsUpObject = GameObject.Instantiate(Res.Instance.StairsObject, new Vector3(upX, upY, 0f), Quaternion.identity);
        map.UpStairs = stairsUpObject.GetComponent<Stairs>();
        map.UpStairs.X = upX;
        map.UpStairs.Y = upY;
        map.UpStairs.IsUp = true;
        map.UpStairs.go = stairsUpObject;
        stairsUpObject.transform.SetParent(GameManager.ObjectHolder);

        int downX = map.Rooms.Last().Center.X;
        int downY = map.Rooms.Last().Center.Y;

        GameObject stairsDownObject = GameObject.Instantiate(Res.Instance.StairsObject, new Vector3(downX, downY, 0f), Quaternion.identity);
        map.DownStairs = stairsDownObject.GetComponent<Stairs>();
        map.DownStairs.X = downX;
        map.DownStairs.Y = downY;
        map.DownStairs.IsUp = false;
        map.DownStairs.go = stairsDownObject;
        stairsDownObject.transform.SetParent(GameManager.ObjectHolder);
    }
}
