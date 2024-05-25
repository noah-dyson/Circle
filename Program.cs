using System;

namespace Circle{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1(new MainGameState()))
                game.Run();
        }
    }
}
