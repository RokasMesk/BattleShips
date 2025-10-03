using System;

namespace BattleShip.Model
{
    public abstract class User
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string DisplayName { get; protected set; } = string.Empty;

        protected User(string displayName)
        {
            DisplayName = displayName;
        }
    }

    public sealed class Player : User
    {
        public Board Board { get; }

        public Player(string displayName, int width = 10, int height = 10) : base(displayName)
        {
            Board = new Board(width, height);
        }

        public bool PlaceShip(Ship ship, int x, int y, bool horizontal)
        {
            return Board.PlaceShip(ship, x, y, horizontal);
        }

        public bool Attack(Player enemy, int x, int y)
        {
            return enemy.Board.Attack(x, y);
        }
    }

    public sealed class Enemy : User
    {
        public Enemy(string displayName) : base(displayName)
        {
        }
    }
}


