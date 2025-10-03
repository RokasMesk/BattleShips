using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip.Model
{
    public readonly struct GridPosition
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public abstract class Ship
    {
        public string Name { get; protected set; } = string.Empty;
        public int Length { get; protected set; }
        public bool IsHorizontal { get; private set; }
        public IReadOnlyList<GridPosition> Positions => _positions;
        private readonly List<GridPosition> _positions = new();
        private readonly HashSet<(int x, int y)> _hits = new();

        protected Ship()
        {
        }

        public virtual void Place(int startX, int startY, bool horizontal)
        {
            IsHorizontal = horizontal;
            _positions.Clear();
            _positions.AddRange(CalculatePositions(startX, startY, horizontal));
        }

        public IEnumerable<GridPosition> CalculatePositions(int startX, int startY, bool horizontal)
        {
            for (var i = 0; i < Length; i++)
            {
                var x = horizontal ? startX + i : startX;
                var y = horizontal ? startY : startY + i;
                yield return new GridPosition(x, y);
            }
        }

        public void ReceiveHit(Cell cell)
        {
            _hits.Add((cell.X, cell.Y));
        }

        public bool IsSunk()
        {
            return _hits.Count >= Length;
        }

        public Ship WithoutPositionBinding()
        {
            var clone = (Ship)Activator.CreateInstance(GetType())!;
            return clone;
        }
    }

    public sealed class Cruiser : Ship
    {
        public Cruiser()
        {
            Name = "Cruiser";
            Length = 3;
        }
    }

    public sealed class Battleship : Ship
    {
        public Battleship()
        {
            Name = "Battleship";
            Length = 4;
        }
    }

    public sealed class Submarine : Ship
    {
        public Submarine()
        {
            Name = "Submarine";
            Length = 3;
        }
    }

    public sealed class Destroyer : Ship
    {
        public Destroyer()
        {
            Name = "Destroyer";
            Length = 2;
        }
    }

    public sealed class PatrolBoat : Ship
    {
        public PatrolBoat()
        {
            Name = "Patrol Boat";
            Length = 1;
        }
    }

    public static class ShipFactory
    {
        public static Ship Create<TShip>() where TShip : Ship, new()
        {
            return new TShip();
        }

        public static Ship CreateByName(string name)
        {
            return name.ToLowerInvariant() switch
            {
                "cruiser" => new Cruiser(),
                "battleship" => new Battleship(),
                "submarine" => new Submarine(),
                "destroyer" => new Destroyer(),
                _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown ship type")
            };
        }

        public static IReadOnlyCollection<Ship> CreateStandardFleet()
        {
            return new Ship[] { new Battleship(), new Cruiser(), new Cruiser(), new Submarine(), new Destroyer(), new Destroyer() };
        }

        // Classic layout requested: 1x4, 2x3, 3x2, 4x1
        public static IReadOnlyCollection<Ship> CreateFleetClassic()
        {
            return new Ship[]
            {
                new Battleship(),
                new Cruiser(), new Submarine(),
                new Destroyer(), new Destroyer(), new Destroyer(),
                new PatrolBoat(), new PatrolBoat(), new PatrolBoat(), new PatrolBoat()
            };
        }
    }
}


