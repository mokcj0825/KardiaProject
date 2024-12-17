using UnityEngine.SceneManagement;

namespace Object
{
    public class GameContext
    {
        
        public static GameContext Instance { get; private set; }
        
        private GameContext()
        {
            
        }
        
        /// <summary>
        /// Initializes the GameContext instance.
        /// </summary>
        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new GameContext();
            }
        }
        
    }
}