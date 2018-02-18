namespace HisDocProUI.Model
{
    public class RenderLine
    {
        public double X0 { get; private set; }
        public double Y0 { get; private set; }
        public double X1 { get; private set; }
        public double Y1 { get; private set; } 

        public RenderLine(double x0, double y0, double x1, double y1)
        {
            this.X0 = x0;
            this.Y0 = y0;
            this.X1 = x1;
            this.Y1 = y1;
        }
    }
}