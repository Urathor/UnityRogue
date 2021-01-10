using RogueSharp;

public class StationaryAttack : IBehavior
{
    public bool Act(Monster monster, CommandSystem commandSystem)
    {
        DungeonMap map = GameManager.DungeonMap;
        Player player = GameManager.Player;
        FieldOfView monsterFoV = new FieldOfView(map);

        if (!monster.TurnsAlerted.HasValue)
        {
            monsterFoV.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
            if (monsterFoV.IsInFov(player.X, player.Y))
            {
                GameManager.MessageLog.AddLog($"{monster.DisplayName} sees {player.DisplayName}.");
                monster.TurnsAlerted = 1;
            }
        }

        if (monster.TurnsAlerted.HasValue)
        {
            map.SetIsWalkable(monster.X, monster.Y, false);

            if(PlayerIsNear(monster, player))
            {
                commandSystem.Attack(monster, player);
            }

            monster.TurnsAlerted++;

            if (monster.TurnsAlerted > 15)
                monster.TurnsAlerted = null;
        }

            return true;
    }

    private bool PlayerIsNear(Monster monster, Player player)
    {
        return ((player.X == monster.X + 1 && player.Y == monster.Y) ||
            (player.X == monster.X && player.Y == monster.Y + 1) ||
            (player.X == monster.X - 1 && player.Y == monster.Y) ||
            (player.X == monster.X && player.Y == monster.Y - 1));
            

        
        
    }
}
