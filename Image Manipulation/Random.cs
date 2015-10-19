

namespace Image_Manipulation
{
    public static class Random
    {
        static System.Random random = new System.Random();

        public static int RandomInt(int min, int max) { return random.Next(min, max); }
        public static int RandomInt(int max) { return random.Next(max); }
    }
}
