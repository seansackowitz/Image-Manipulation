namespace Image_Manipulation
{
    public struct Color
    {
        public int A;
        public int R;
        public int G;
        public int B;

        public Color(int a = 255, int r = 255, int g = 255, int b = 255)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public static implicit operator Color(System.Drawing.Color c) { return new Color(c.A, c.R, c.G, c.B); }
        
        public static implicit operator System.Drawing.Color(Color c) { return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B); }

        public override string ToString()
        {
            return string.Format("A: {0}, R: {1}, G: {2}, B: {3}", A, R, G, B);
        }

        public float Compare(Color c)
        {
            return (float)System.Math.Sqrt(((A - c.A) * (A - c.A)) + ((R - c.R) * (R - c.R)) + ((G - c.G) * (G - c.G)) + ((B - c.B) * (B - c.B))) / 510f;
        }
    }
}
