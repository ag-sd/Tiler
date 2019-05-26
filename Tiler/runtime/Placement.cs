using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tiler.runtime
{
    public class Placement
    {
        private static readonly Dictionary<string, Placement> Placements = INISettings.GetAllPlacements();

        public static Placement ByKey(string key)
        {
            return Placements[key];
        }

        public static readonly IEnumerable<Placement> Values = Placements.Values;

        public Placement(string name, float left, float top, float width, float height)
        {
            Name = name;
            LeftPercent = left;
            TopPercent = top;
            WidthPercent = width;
            HeightPercent = height;
        }

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

        public string Name { get; }

        public float LeftPercent { get; }
        
        public float TopPercent { get; }
        
        public float WidthPercent { get; }
        
        public float HeightPercent { get; }
    }
}