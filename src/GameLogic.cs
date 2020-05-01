using SwinGameSDK;

namespace battleship {
    public class GameLogic {
        public static void Main() {
            // Opens a new Graphics Window
            SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

            // Load Resources
            GameResources.LoadResources();

            GameController.Boot();

            // Game Loop
            do {
                GameResources.PlayMusic("Background");
                GameController.HandleUserInput();
                GameController.DrawScreen();
            }
            while (!SwinGame.WindowCloseRequested() == true || GameController.CurrentState == GameState.Quitting);

            SwinGame.StopMusic();

            // Free Resources and Close Audio, to end the program.
            GameResources.FreeResources();
        }
    }
}
