
using System.Text;
using RogueSharp;
using RogueSharp.DiceNotation;

public class CommandSystem
{
    public bool IsPlayerTurn { get; set; }

    public static void ResolveDeath(Actor defender)
    {
        if(defender is Player)
        {
            GameManager.MessageLog.AddLog($"{defender.DisplayName} was killed, GAME OVER!!!! Please try again....");
        }
        else if(defender is Monster)
        {
            GameManager.DungeonMap.RemoveMonster((Monster)defender);
            GameManager.Player.GrantExp((defender as Monster).ExpValue);
            GameManager.Player.GiveGold(defender.Gold);
            GameManager.MessageLog.AddLog($"{defender.DisplayName} has died and dropped {defender.Gold} gold pieces.");
            
        }

        defender.OnDeath();
    }

    private static void ResolveDamage(Actor defender, int damage)
    {
        if(damage > 0)
        {

            //normal damage
            defender.CurrentHealth -= damage;
            GameManager.MessageLog.AddLog($"{defender.DisplayName} was hit for {damage} damage.");

            if (defender.CurrentHealth < 1)
            {
                ResolveDeath(defender);
            }
        }
        else
        {
            GameManager.MessageLog.AddLog($"{defender.DisplayName} dodged all damage.");
        }
    }

    private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder sb, bool ranged = false)
    {
        int hits = 0;

        sb.AppendFormat("{0} attacks {1} and rolls ", attacker.DisplayName, defender.DisplayName);

        DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 20);
        DiceResult attackResult = attackDice.Roll();

        foreach(TermResult result in attackResult.Results)
        {
            sb.Append(result.Value + attacker.HitBonus + ", ");
            if(result.Value + attacker.HitBonus + 1 >= (20 - ( -defender.Defense + 11) - attacker.Level) )
            {
                hits++;
            }
        }

        return hits;
    }

    public void Attack(Actor attacker, Actor defender)
    {
        StringBuilder attackMessage = new StringBuilder();
        StringBuilder defenseMessage = new StringBuilder();

        int hits = ResolveAttack(attacker, defender, attackMessage);


        GameManager.MessageLog.AddLog(attackMessage.ToString());
        if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
        {
            GameManager.MessageLog.AddLog(defenseMessage.ToString());
        }

        int damage = 0;

        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                damage += UnityEngine.Random.Range(1,attacker.Damage);
            }
        }
        //normal attacks
        ResolveDamage(defender, damage);
    }

    public void EndPlayerTurn()
    {
        IsPlayerTurn = false;
    }

    public void AdvanceSchedule()
    {
        IScheduleable scheduleable = GameManager.SchedulingSystem.Get();
        if(scheduleable is Player)
        {
            IsPlayerTurn = true;
            GameManager.SchedulingSystem.Add(GameManager.Player);
        }
        else if( scheduleable is Monster)
        {
            Monster monster = scheduleable as Monster;

            if(monster != null)
            {
                monster.PerformAction(this);
                GameManager.SchedulingSystem.Add(monster);
            }
        }
        else if (scheduleable is Condition)
        {
            Condition condition = scheduleable as Condition;

            if (condition != null)
            {
                condition.PerformAction(this);

                if (condition.Duration > 0)
                    GameManager.SchedulingSystem.Add(condition);
                else
                    GameManager.SchedulingSystem.Remove(condition);
            }
        }
    }

    public void MoveMonster(Actor monster, Cell cell)
    {
        if (monster.CanMove)
        {
            bool canOpendoors = monster.GetComponent<Monster>().CanOpenDoors;

            if (!GameManager.DungeonMap.SetActorPosition(monster, cell.X, cell.Y,canOpendoors))
            {
                if (GameManager.Player.X == cell.X && GameManager.Player.Y == cell.Y)
                {
                    Attack(monster, GameManager.Player);
                }
            }
        }
    }

    public bool MovePlayer(Direction dir)
    {

        int x = GameManager.Player.X;
        int y = GameManager.Player.Y;


        switch(dir)
        {
            case Direction.Down: { y = GameManager.Player.Y - 1; break; }
            case Direction.Up: { y = GameManager.Player.Y + 1; break; }
            case Direction.Right: { x = GameManager.Player.X + 1; break; }
            case Direction.Left: { x = GameManager.Player.X - 1; break; }
            default: { return false; }
        }


        if (GameManager.DungeonMap.SetActorPosition(GameManager.Player, x, y,true))
        {
            GameManager.Player.Regenerate();
            GameManager.Player.ReduceHunger();
            return true;
        }


        Monster monster = GameManager.DungeonMap.GetMonsterAt(x, y);
        if(monster != null)
        {
            Attack(GameManager.Player, monster);
            return true;
        }

        return false;
    }

}
