using System;
using System.Drawing;

namespace Tiler.runtime
{
    public class Placement
    {
        public static readonly Placement Left = new Placement("Left", 0, 0, 0, 0.33f, 1f);

        public static readonly Placement[] values =
        {
            Left
        };

        public Placement(string name, int desktop, float left, float top, float width, float height)
        {
            Desktop = desktop;
            Name = name;
            LeftPercent = left;
            TopPercent = top;
            WidthPercent = width;
            HeightPercent = height;
        }
        
        //Resolve

        //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen?view=netframework-4.8   
        public Point ResolveLocation(float width, float height)
        {
            return new Point((int)Math.Round(width * LeftPercent), (int)Math.Round(height * TopPercent));
        }

        public Size ResolveSize(float width, float height)
        {
            return new Size((int)Math.Round(width * WidthPercent), (int)Math.Round(height * HeightPercent));
        }
        
        public override string ToString()
        {
            return Name;
        }
        
        public int Desktop { get; }
        
        public string Name { get; }

        public float LeftPercent { get; }
        
        public float TopPercent { get; }
        
        public float WidthPercent { get; }
        
        public float HeightPercent { get; }
    }
}