using System;

namespace XnaCraft
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XnaCraftGame game = new XnaCraftGame())
            {
                game.Run();
            }
        }
    }
#endif
}

