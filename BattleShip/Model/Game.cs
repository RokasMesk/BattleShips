using System;
using System.Collections.Generic;

namespace BattleShip.Model
{
    public sealed class Session
    {
        private static Session? _instance;
        public static Session Current => _instance ??= new Session();

        public Guid Id { get; } = Guid.NewGuid();
        public Player? PlayerOne { get; private set; }
        public Player? PlayerTwo { get; private set; }
        public Chat Chat { get; } = new Chat();
        public bool IsStarted { get; private set; }
        public Player? CurrentTurn { get; private set; }
        public IReadOnlyList<Action> Actions => _actions;

        private readonly List<Action> _actions = new();

        private Session()
        {
        }

        public void Create(Player playerOne, Player playerTwo)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            IsStarted = false;
            CurrentTurn = playerOne;
            _actions.Clear();
        }

        public void Start()
        {
            if (PlayerOne == null || PlayerTwo == null)
            {
                throw new InvalidOperationException("Both players must be set before starting the session.");
            }
            IsStarted = true;
        }

        public AttackAction Attack(Player attacker, int x, int y)
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Game not started");
            }

            if (CurrentTurn != attacker)
            {
                throw new InvalidOperationException("Not your turn");
            }

            var defender = attacker == PlayerOne ? PlayerTwo! : PlayerOne!;
            var wasHit = attacker.Attack(defender, x, y);
            var act = new AttackAction(attacker, defender, x, y, wasHit);
            _actions.Add(act);
            CurrentTurn = defender;
            return act;
        }

        public void Surrender(Player player)
        {
            IsStarted = false;
        }

        public void RequestDraw(Player player)
        {
        }
    }

    public sealed class Game
    {
        public Session CreateSessionAndBoard(Player playerOne, Player playerTwo)
        {
            var session = Session.Current;
            session.Create(playerOne, playerTwo);
            return session;
        }

        public void RandomizeShipPlacements(Player player)
        {
            var fleet = ShipFactory.CreateStandardFleet();
            player.Board.RandomizeShips(fleet);
        }

        public bool CreateAndPlaceShip(Player player, string shipName, int x, int y, bool horizontal)
        {
            var ship = ShipFactory.CreateByName(shipName);
            return player.PlaceShip(ship, x, y, horizontal);
        }
    }
}


