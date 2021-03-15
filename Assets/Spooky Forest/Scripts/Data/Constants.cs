namespace Spooky_Forest.Scripts.Data
{
    public static class Constants
    {
        public struct Tags
        {
            public const string Player = "Player";
            public const string Collider = "collider";
            public const string Water = "water";
            public const string Boat = "boat";
        }
        
        public static readonly string[] _PlatformTags = {"grave", "grass", "water", "rails", "spider", "skeleton", "ghost"};
        public static readonly float[] _PlatformHeights = {0f, 0f, -0.15f, 0.05f, 0f, 0f, 0f};
    }
}