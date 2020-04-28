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
using SwinGameSDK;

/// <summary>

/// ''' The DeploymentController controls the players actions

/// ''' during the deployment phase.

/// ''' </summary>
namespace battleship {
    public static class DeploymentController {
        private readonly static int SHIPS_TOP = 98;
        private readonly static int SHIPS_LEFT = 20;
        private readonly static int SHIPS_HEIGHT = 90;
        private readonly static int SHIPS_WIDTH = 300;

        private readonly static int TOP_BUTTONS_TOP = 72;
        private readonly static int TOP_BUTTONS_HEIGHT = 46;

        private readonly static int PLAY_BUTTON_LEFT = 693;
        private readonly static int PLAY_BUTTON_WIDTH = 80;

        private readonly static int UP_DOWN_BUTTON_LEFT = 410;
        private readonly static int LEFT_RIGHT_BUTTON_LEFT = 350;

        private readonly static int RANDOM_BUTTON_LEFT = 547;
        private readonly static int RANDOM_BUTTON_WIDTH = 51;

        private readonly static int DIR_BUTTONS_WIDTH = 47;

        private readonly static int TEXT_OFFSET = 5;

        private static Direction _currentDirection = Direction.UpDown;
        private static ShipName _selectedShip = ShipName.Tug;

        /// <summary>
        ///     ''' Handles user input for the Deployment phase of the game.
        ///     ''' </summary>
        ///     ''' <remarks>
        ///     ''' Involves selecting the ships, deloying ships, changing the direction
        ///     ''' of the ships to add, randomising deployment, end then ending
        ///     ''' deployment
        ///     ''' </remarks>
        public static void HandleDeploymentInput() {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
                GameController.AddNewState(GameState.ViewingGameMenu);

            if (SwinGame.KeyTyped(KeyCode.UpKey) || SwinGame.KeyTyped(KeyCode.DownKey))
                _currentDirection = Direction.UpDown;
            if (SwinGame.KeyTyped(KeyCode.LeftKey) || SwinGame.KeyTyped(KeyCode.RightKey))
                _currentDirection = Direction.LeftRight;

            if (SwinGame.KeyTyped(KeyCode.RightKey))
                GameController.HumanPlayer.RandomizeDeployment();

            if (SwinGame.MouseClicked(MouseButton.LeftButton)) {
                ShipName selected;
                selected = GetShipMouseIsOver();
                if (selected != ShipName.None)
                    _selectedShip = selected;
                else
                    DoDeployClick();

                if (GameController.HumanPlayer.ReadyToDeploy && UtilityFunctions.IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                    GameController.EndDeployment();
                else if (UtilityFunctions.IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                    _currentDirection = Direction.UpDown;
                else if (UtilityFunctions.IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                    _currentDirection = Direction.LeftRight;
                else if (UtilityFunctions.IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                    GameController.HumanPlayer.RandomizeDeployment();
            }
        }

        /// <summary>
        ///     ''' The user has clicked somewhere on the screen, check if its is a deployment and deploy
        ///     ''' the current ship if that is the case.
        ///     ''' </summary>
        ///     ''' <remarks>
        ///     ''' If the click is in the grid it deploys to the selected location
        ///     ''' with the indicated direction
        ///     ''' </remarks>
        private static void DoDeployClick() {
            Point2D mouse;

            mouse = SwinGame.MousePosition();

            // Calculate the row/col clicked
            int row, col;
            row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (double)(UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP))); // Did this fix it?
            col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (double)(UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 && row < GameController.HumanPlayer.PlayerGrid.Height) {
                if (col >= 0 && col < GameController.HumanPlayer.PlayerGrid.Width) {
                    // if in the area try to deploy
                    try {
                        GameController.HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection); // Bookmark - ship is being placed wrong. When set to updown, should place from bottom of ship
                    }
                    catch (Exception ex) {
                        GameResources.PlaySound("Error");
                        UtilityFunctions.Message = ex.Message;
                    }
                }
            }
        }

        /// <summary>
        ///     ''' Draws the deployment screen showing the field and the ships
        ///     ''' that the player can deploy.
        ///     ''' </summary>
        public static void DrawDeployment() {
            UtilityFunctions.DrawField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer, true);

            // Draw the Left/Right and Up/Down buttons
            if (_currentDirection == Direction.LeftRight)
                SwinGame.DrawBitmap(GameResources.GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
            else
                SwinGame.DrawBitmap(GameResources.GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);

            // DrawShips
            foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
                int i;
                i = Convert.ToInt32(sn) - 1;
                if (i >= 0) {
                    if (sn == _selectedShip)
                        SwinGame.DrawBitmap(GameResources.GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT);
                }
            }

            if (GameController.HumanPlayer.ReadyToDeploy)
                SwinGame.DrawBitmap(GameResources.GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP);

            SwinGame.DrawBitmap(GameResources.GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP);

            UtilityFunctions.DrawMessage();
        }

        /// <summary>
        ///     ''' Gets the ship that the mouse is currently over in the selection panel.
        ///     ''' </summary>
        ///     ''' <returns>The ship selected or none</returns>
        private static ShipName GetShipMouseIsOver() {
            foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
                int i;
                i = Convert.ToInt32(sn) - 1;

                if (UtilityFunctions.IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT))
                    return sn;
            }

            return ShipName.None;
        }
    }
}