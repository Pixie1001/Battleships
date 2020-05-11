using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace battleship
{
    public class AIEasyPlayer : AIPlayer
    {

        private enum AIStates
        {
            Searching,
            TargetingShip
        }

        private AIStates _CurrentState = AIStates.Searching;
        private Stack<Location> _Targets = new Stack<Location>();

        public AIEasyPlayer(BattleShipsGame controller) : base(controller)
        {
        }

        /// <summary>
        ///     ''' GenerateCoordinates should generate random shooting coordinates
        ///     ''' only when it has not found a ship, or has destroyed a ship and 
        ///     ''' needs new shooting coordinates
        ///     ''' </summary>
        ///     ''' <param name="row">the generated row</param>
        ///     ''' <param name="column">the generated column</param>
        protected override void GenerateCoords(ref int row, ref int column)
        {
            _Random = new Random();
            row = _Random.Next(0, EnemyGrid.Height);
            column = _Random.Next(0, EnemyGrid.Width);
        }

        /// <summary>
        ///     ''' TargetCoords is used when a ship has been hit and it will try and destroy
        ///     ''' this ship
        ///     ''' </summary>
        ///     ''' <param name="row">row generated around the hit tile</param>
        ///     ''' <param name="column">column generated around the hit tile</param>


        /// <summary>
        ///     ''' SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
        ///     ''' </summary>
        ///     ''' <param name="row">the generated row</param>
        ///     ''' <param name="column">the generated column</param>
        private void SearchCoords(ref int row, ref int column)
        {
            row = _Random.Next(0, EnemyGrid.Height);
            column = _Random.Next(0, EnemyGrid.Width);
        }

        /// <summary>
        ///     ''' ProcessShot will be called uppon when a ship is found.
        ///     ''' It will create a stack with targets it will try to hit. These targets
        ///     ''' will be around the tile that has been hit.
        ///     ''' </summary>
        ///     ''' <param name="row">the row it needs to process</param>
        ///     ''' <param name="col">the column it needs to process</param>
        ///     ''' <param name="result">the result og the last shot (should be hit)</param>
        protected override void ProcessShot(int row, int col, AttackResult result)
        {

        }

    }

}

