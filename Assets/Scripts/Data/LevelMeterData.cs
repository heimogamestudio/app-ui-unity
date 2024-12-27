using Unity.Properties;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// Represents the level data used for the level meter, which tracks the player's total levels 
    /// and provides the corresponding rank using data binding.
    /// </summary>
    public class LevelMeterData
    {
        // Stores the player's total level count
        int m_TotalLevels;

        /// <summary>
        /// The total number of levels accumulated. This property is used with data binding.
        /// </summary>
        [CreateProperty]
        public int TotalLevels { get => m_TotalLevels; set => m_TotalLevels = value; }

        /// <summary>
        /// Constructor initializes the LevelMeterData class with the specified total levels.
        /// </summary>
        public LevelMeterData(int totalLevels)
        {
            m_TotalLevels = Mathf.Clamp(totalLevels, 0, 100);
        }
        
        /// <summary>
        /// Maps the player's total level to a rank string.
        /// </summary>
        /// <param name="level">The player's total level.</param>
        public static string GetRankFromLevel(int level)
        {
            if (level >= 100) return "Ultimate Champion";
            if (level >= 95) return "Supreme Champion";
            if (level >= 90) return "Royal Champion";
            if (level >= 85) return "Elite Champion";
            if (level >= 80) return "Grand Champion";
            if (level >= 75) return "Veteran Champion";
            if (level >= 70) return "Champion";
            if (level >= 65) return "Master IV";
            if (level >= 60) return "Master III";
            if (level >= 55) return "Master II";
            if (level >= 50) return "Master I";
            if (level >= 45) return "Challenger IV";
            if (level >= 40) return "Challenger III";
            if (level >= 35) return "Challenger II";
            if (level >= 30) return "Challenger I";
            if (level >= 27) return "Acolyte III";            
            if (level >= 23) return "Acolyte II";   
            if (level >= 20) return "Acolyte I";
            if (level >= 15) return "Explorer III";
            if (level >= 12) return "Explorer II";
            if (level >= 10) return "Explorer I";
            if (level >= 8) return "Apprentice II";
            if (level >= 6) return "Apprentice I";
            if (level >= 4) return "Initiate III";
            if (level >= 3) return "Initiate II";
            if (level >= 1) return "Initiate I";
            return "Unranked";
        }
    }
    

}
