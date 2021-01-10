using RogueSharp;

public class StandardAttackAndMove : IBehavior
{
    public bool Act(Monster monster, CommandSystem commandSystem)
    {
        DungeonMap map = GameManager.DungeonMap;
        Player player = GameManager.Player;
        FieldOfView monsterFoV = new FieldOfView(map);

        if(!monster.TurnsAlerted.HasValue)
        {
            monsterFoV.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
            if(monsterFoV.IsInFov(player.X,player.Y))
            {
                GameManager.MessageLog.AddLog($"{monster.DisplayName} sees {player.DisplayName}.");
                monster.TurnsAlerted = 1;
            }
        }

        if(monster.TurnsAlerted.HasValue)
        {
            map.SetIsWalkable(monster.X, monster.Y,true);
            map.SetIsWalkable(player.X, player.Y,true);

            PathFinder pathFinder = new PathFinder(map);
            Path path = null;

            try
            {
                path = pathFinder.ShortestPath(map.GetCell(monster.X, monster.Y), map.GetCell(player.X, player.Y));
            }
            catch (PathNotFoundException)
            {
                GameManager.MessageLog.AddLog($"{monster.DisplayName} waits impatiently.");
            }

            map.SetIsWalkable(monster.X, monster.Y, false);
            map.SetIsWalkable(player.X, player.Y, false);

            if (path != null)
            {
                try
                {
                    commandSystem.MoveMonster(monster, (Cell)path.StepForward());
                }
                catch(NoMoreStepsException)
                {
                    GameManager.MessageLog.AddLog($"{monster.DisplayName} growls in frustration as it cannot reach {player.DisplayName}");
                }
            }

            monster.TurnsAlerted++;

            if (monster.TurnsAlerted > 15)
                monster.TurnsAlerted = null;
        }

        return true;
    }
}
