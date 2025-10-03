using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip.Model
{
    public enum CellState
    {
        Empty,
        Ship,
        Hit,
        Miss
    }

    public sealed class Cell
    {
        public int X { get; }
        public int Y { get; }
        public CellState State { get; private set; }
        public Ship? OccupyingShip { get; private set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            State = CellState.Empty;
        }

        public void PlaceShip(Ship ship)
        {
            OccupyingShip = ship;
            State = CellState.Ship;
        }

        public bool Attack()
        {
            if (State == CellState.Hit || State == CellState.Miss)
            {
                return false;
            }

            if (State == CellState.Ship)
            {
                State = CellState.Hit;
                OccupyingShip?.ReceiveHit(this);
                return true;
            }

            State = CellState.Miss;
            return false;
        }

        public void Clear()
        {
            OccupyingShip = null;
            State = CellState.Empty;
        }

        public void DetachShip()
        {
            OccupyingShip = null;
            if (State == CellState.Ship)
            {
                State = CellState.Empty;
            }
        }
    }

    public class Board
    {
        public int Width { get; }
        public int Height { get; }
        private readonly Cell[,] _grid;
        private readonly List<Ship> _ships = new();

        public IReadOnlyCollection<Ship> Ships => _ships.AsReadOnly();

        public Board(int width = 10, int height = 10)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Board dimensions must be positive");
            }

            Width = width;
            Height = height;
            _grid = new Cell[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    _grid[x, y] = new Cell(x, y);
                }
            }
        }

        public Cell GetCell(int x, int y)
        {
            return _grid[x, y];
        }

        public bool CanPlace(Ship ship, int startX, int startY, bool horizontal)
        {
            var positions = ship.CalculatePositions(startX, startY, horizontal);
            return positions.All(p => InBounds(p.X, p.Y) && _grid[p.X, p.Y].State == CellState.Empty);
        }

        public bool PlaceShip(Ship ship, int startX, int startY, bool horizontal)
        {
            if (!CanPlace(ship, startX, startY, horizontal))
            {
                return false;
            }

            ship.Place(startX, startY, horizontal);
            foreach (var pos in ship.Positions)
            {
                _grid[pos.X, pos.Y].PlaceShip(ship);
            }

            _ships.Add(ship);
            return true;
        }

        public bool Attack(int x, int y)
        {
            return GetCell(x, y).Attack();
        }

        public void ResetShips()
        {
            foreach (var cell in EnumerateCells())
            {
                cell.Clear();
            }
            _ships.Clear();
        }

        public bool RotateShip(Ship ship)
        {
            if (!_ships.Contains(ship))
            {
                return false;
            }

            // Try rotate around first position
            var anchor = ship.Positions.First();
            var newHorizontal = !ship.IsHorizontal;
            var can = CanPlace(ship.WithoutPositionBinding(), anchor.X, anchor.Y, newHorizontal);
            if (!can)
            {
                return false;
            }

            // Clear old cells
            foreach (var pos in ship.Positions)
            {
                var c = _grid[pos.X, pos.Y];
                c.DetachShip();
            }

            // Place rotated
            ship.Place(anchor.X, anchor.Y, newHorizontal);
            foreach (var pos in ship.Positions)
            {
                _grid[pos.X, pos.Y].PlaceShip(ship);
            }

            return true;
        }

        public void RandomizeShips(IReadOnlyCollection<Ship> ships, int? seed = null)
        {
            ResetShips();
            var rng = seed.HasValue ? new Random(seed.Value) : new Random();

            foreach (var ship in ships)
            {
                var placed = false;
                for (var attempts = 0; attempts < 1000 && !placed; attempts++)
                {
                    var horizontal = rng.Next(2) == 0;
                    var x = rng.Next(Width);
                    var y = rng.Next(Height);
                    placed = PlaceShip(ship.WithoutPositionBinding(), x, y, horizontal);
                }
            }
        }

        public IEnumerable<Cell> EnumerateCells()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    yield return _grid[x, y];
                }
            }
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
}


