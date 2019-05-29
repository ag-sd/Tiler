using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tiler.runtime
{
    public class Placement
    {
        public static readonly Placement None = new Placement("None", 0, 0, 1f, 1f);
        
        private static readonly Dictionary<string, Placement> Placements = new Dictionary<string, Placement>
        {
            {"None", None},
            {"Left", new Placement("Left", 0, 0, 0.33f, 1f)},
            {"Center", new Placement("Center", .33f, 0, 0.33f, 1f)},
            {"Right", new Placement("Right", .66f, 0, 0.33f, 1f)},
            {"Left Half", new Placement("Left Half", 0, 0, 0.50f, 1f)},
            {"Right Half", new Placement("Right Half", .50f, 0, 0.50f, 1f)},
            {"Left Third", new Placement("Left Third", 0, 0.25f, 0.33f, 0.75f)}
        }
            .Concat(INISettings.GetAllPlacements())
            .GroupBy(e => e.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);
        
        public static readonly IEnumerable<Placement> Values = Placements.Values;

        public static Placement ByKey(string key)
        {
            return Placements[key];
        }
        
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

        public override string ToString() => Name;
//        {
//            return Name;
//        }

        public string Name { get; }

        public float LeftPercent { get; }
        
        public float TopPercent { get; }
        
        public float WidthPercent { get; }
        
        public float HeightPercent { get; }
    }
}