

using RogueSharp;

public class FleeFromPlayer : IBehavior
{
    public bool Act(Monster monster, CommandSystem comSystem)
    {


        DungeonMap dungeonMap = GameManager.DungeonMap;
        Player player = GameManager.Player;
        FieldOfView monsterFov = new FieldOfView(dungeonMap);


        if (!monster.TurnsAlerted.HasValue)
        {
            monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);

            if (monsterFov.IsInFov(player.X, player.Y))
            {
                GameManager.MessageLog.AddLog($"{monster.DisplayName} looks at {player.DisplayName} in terror.");
                monster.TurnsAlerted = 1;

            }
        }
        if (monster.TurnsAlerted.HasValue)
        {

            dungeonMap.SetIsWalkable(monster.X, monster.Y, true);

            PathFinder pathFinder = new PathFinder(dungeonMap);
            Path path = null;

            //get a random cell that is away from the player
            int randIndex = GameManager.Random.Next(0, dungeonMap.Rooms.Count - 1);
            Rectangle randRoom = dungeonMap.Rooms[randIndex];
            Point randLocation = (Point)dungeonMap.GetRandomLocationInRoom(randRoom);

            try
            {
                path = pathFinder.ShortestPath(dungeonMap.GetCell(monster.X, monster.Y), dungeonMap.GetCell(randLocation.X, randLocation.Y));
            }
            catch (PathNotFoundException)
            {
                GameManager.MessageLog.AddLog($"{monster.DisplayName} cowers in place.");
            }

            dungeonMap.SetIsWalkable(monster.X, monster.Y, false);

            if (path != null)
            {
                try
                {
                    comSystem.MoveMonster(monster, (Cell)path.StepForward());
                }
                catch (NoMoreStepsException)
                {
                    GameManager.MessageLog.AddLog($"{monster.DisplayName} flees in fear.");
                }
            }

            monster.TurnsAlerted++;

            if (monster.TurnsAlerted > 15)
            {
                monster.TurnsAlerted = null;
            }
        }


        return true;
    }
}
