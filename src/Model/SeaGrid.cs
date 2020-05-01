﻿/// <summary>
/// The SeaGrid is the grid upon which the ships are deployed.
/// </summary>
/// <remarks>
/// The grid is viewable via the ISeaGrid interface as a read only
/// grid. This can be used in conjuncture with the SeaGridAdapter to
/// mask the position of the ships.
/// </remarks>
using System;
using System.Collections.Generic;
//using System.Xml.Linq;

namespace battleship {
    public class SeaGrid : ISeaGrid {
        private static int _WIDTH = 10;
        private static int _HEIGHT = 10;
        private Tile[,] _GameTiles;
        private Dictionary<ShipName, Ship> _Ships;
        private int _ShipsKilled = 0;

        /// <summary>
        /// SeaGrid constructor, a seagrid has a number of tiles stored in an array
        /// </summary>
        public SeaGrid(Dictionary<ShipName, Ship> ships) {
            _Ships = ships;
            _GameTiles = new Tile[Width, Height];

            // fill array with empty Tiles
            int i;
            var loopTo = Width - 1;
            for (i = 0; i <= loopTo; i++) {
                for (int j = 0, loopTo1 = Height - 1; j <= loopTo1; j++)
                    _GameTiles[i, j] = new Tile(i, j, null);
            }
        }

        /// <summary>
        /// The sea grid has changed and should be redrawn.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// The width of the sea grid.
        /// </summary>
        /// <value>The width of the sea grid.</value>
        /// <returns>The width of the sea grid.</returns>
        public int Width {
            get {
                return _WIDTH;
            }
        }

        /// <summary>
        /// The height of the sea grid
        /// </summary>
        /// <value>The height of the sea grid</value>
        /// <returns>The height of the sea grid</returns>
        public int Height {
            get {
                return _HEIGHT;
            }
        }

        /// <summary>
        /// ShipsKilled returns the number of ships killed
        /// </summary>
        public int ShipsKilled {
            get {
                return _ShipsKilled;
            }
        }

        /// <summary>
        /// Show the tile view
        /// </summary>
        /// <param name="x">x coordinate of the tile</param>
        /// <param name="y">y coordiante of the tile</param>
        /// <returns></returns>
        public TileView Item(int row, int col) {
            return _GameTiles[row, col].View;
        }

        /// <summary>
        /// AllDeployed checks if all the ships are deployed
        /// </summary>
        public bool AllDeployed {
            get {
                foreach (Ship s in _Ships.Values) {
                    if (!s.IsDeployed) {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// MoveShips allows for ships to be placed on the seagrid
        /// </summary>
        /// <param name="row">the row selected</param>
        /// <param name="col">the column selected</param>
        /// <param name="ship">the ship selected</param>
        /// <param name="direction">the direction the ship is going</param>
        public void MoveShip(int row, int col, ShipName ship, Direction direction) {
            var newShip = _Ships[ship];
            newShip.Remove();
            AddShip(row, col, direction, newShip);
        }

        /// <summary>
        /// AddShip add a ship to the SeaGrid
        /// </summary>
        /// <param name="row">row coordinate</param>
        /// <param name="col">col coordinate</param>
        /// <param name="direction">direction of ship</param>
        /// <param name="newShip">the ship</param>
        private void AddShip(int row, int col, Direction direction, Ship newShip) {
            try {
                int size = newShip.Size;
                int currentRow = row;
                int currentCol = col;
                int dRow, dCol;
                if (direction == Direction.LeftRight) {
                    dRow = 0;
                    dCol = 1;
                }
                else {
                    dRow = 1;
                    dCol = 0;
                }

                // place ship's tiles in array and into ship object
                int i;
                var loopTo = size - 1;
                for (i = 0; i <= loopTo; i++) {
                    if (currentRow < 0 || currentRow >= Width || currentCol < 0 || currentCol >= Height) {
                        throw new InvalidOperationException("Ship can't fit on the board");
                    }

                    _GameTiles[currentRow, currentCol].Ship = newShip;
                    currentCol += dCol;
                    currentRow -= dRow;
                }

                newShip.Deployed(direction, row, col);
            }
            catch (Exception e) {
                newShip.Remove(); // if fails remove the ship
                throw new ApplicationException(e.Message);
            }
            finally {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// HitTile hits a tile at a row/col, and whatever tile has been hit, a
        /// result will be displayed.
        /// </summary>
        /// <param name="row">the row at which is being shot</param>
        /// <param name="col">the cloumn at which is being shot</param>
        /// <returns>An attackresult (hit, miss, sunk, shotalready)</returns>
        public AttackResult HitTile(int row, int col) {
            try {
                // tile is already hit
                if (_GameTiles[row, col].Shot) {
                    return new AttackResult(ResultOfAttack.ShotAlready, "have already attacked [" + col + "," + row + "]!", row, col);
                }

                _GameTiles[row, col].Shoot();

                // there is no ship on the tile
                if (_GameTiles[row, col].Ship is null) {
                    return new AttackResult(ResultOfAttack.Miss, "missed", row, col);
                }

                // all ship's tiles have been destroyed
                if (_GameTiles[row, col].Ship.IsDestroyed) {
                    _GameTiles[row, col].Shot = true;
                    _ShipsKilled += 1;
                    return new AttackResult(ResultOfAttack.Destroyed, _GameTiles[row, col].Ship, "destroyed the enemy's", row, col);
                }

                // else hit but not destroyed
                return new AttackResult(ResultOfAttack.Hit, "hit something!", row, col);
            }
            finally {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}