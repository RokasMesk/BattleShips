using System;

namespace BattleShip.Model
{
    public abstract class Action
    {
        public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
        public abstract string Describe();
    }

    public sealed class AttackAction : Action
    {
        public Player Attacker { get; }
        public Player Defender { get; }
        public int X { get; }
        public int Y { get; }
        public bool WasHit { get; }

        public AttackAction(Player attacker, Player defender, int x, int y, bool wasHit)
        {
            Attacker = attacker;
            Defender = defender;
            X = x;
            Y = y;
            WasHit = wasHit;
        }

        public override string Describe()
        {
            return $"{Attacker.DisplayName} attacked ({X},{Y}) and {(WasHit ? "hit" : "missed")}.";
        }
    }

    public abstract class Ability
    {
        public string Name { get; protected set; } = string.Empty;
        public abstract void Use(Player player, Session session);
    }
}


