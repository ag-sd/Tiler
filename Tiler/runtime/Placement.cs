using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tiler.runtime
{
    public class Placement
    {
        public static readonly Placement None = new Placement("None", 0, 0, 0, 1f, 1f);
        public static readonly Placement Left = new Placement("Left", 0, 0, 0, 0.33f, 1f);
        public static readonly Placement Center = new Placement("Center", 0, .33f, 0, 0.33f, 1f);
        public static readonly Placement Right = new Placement("Right", 0, .66f, 0, 0.33f, 1f);
        public static readonly Placement LeftHalf = new Placement("Left Half", 0, 0, 0, 0.50f, 1f);
        public static readonly Placement RightHalf = new Placement("Right Half", 0, .50f, 0, 0.50f, 1f);
        public static readonly Placement LeftThird = new Placement("Left Third", 0, 0, 0.25f, 0.33f, 0.75f);
        
        private static readonly Dictionary<string, Placement> Placements = new Dictionary<string, Placement>()
        {
            {"None", None},
            {"Left", Left},
            {"Center", Center},
            {"Right", Right},
            {"Left Half", LeftHalf},
            {"Right Half", RightHalf},
            {"Left Third", LeftThird}
        };

        public static Placement ByKey(string key)
        {
            return Placements[key];
        }

        public static readonly IEnumerable<Placement> Values = Placements.Values;

        private Placement(string name, int desktop, float left, float top, float width, float height)
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