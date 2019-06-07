using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tiler.runtime
{
    public class Placement : IEquatable<Placement>
    {
        public static readonly Placement None = new Placement("None", 0, 0, 1f, 1f, false);
        
        private static readonly Dictionary<string, Placement> Placements = new Dictionary<string, Placement>
        {
            {"None", None},
            {"Left", new Placement("Left", 0, 0, 0.33f, 1f, false)},
            {"Center", new Placement("Center", .33f, 0, 0.33f, 1f, false)},
            {"Right", new Placement("Right", .66f, 0, 0.33f, 1f, false)},
            {"Left Half", new Placement("Left Half", 0, 0, 0.50f, 1f, false)},
            {"Right Half", new Placement("Right Half", .50f, 0, 0.50f, 1f, false)},
            {"Left Third", new Placement("Left Third", 0, 0.25f, 0.33f, 0.75f, false)}
        }
            .Concat(INISettings.GetAllPlacements())
            .GroupBy(e => e.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);
        
        public static readonly IEnumerable<Placement> Values = Placements.Values;

        public static Placement ByKey(string key)
        {
            return Placements[key];
        }
        
        public Placement(string name, float left, float top, float width, float height, bool isCustom)
        {
            Name = name;
            LeftPercent = left;
            TopPercent = top;
            WidthPercent = width;
            HeightPercent = height;
            IsCustom = isCustom;
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

        public bool SizeMatches(Placement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return new RectangleF(LeftPercent, TopPercent, WidthPercent, HeightPercent)
                .Equals(new RectangleF(other.LeftPercent, other.TopPercent, other.WidthPercent, other.HeightPercent));
        }

        public bool Equals(Placement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && 
                   LeftPercent.Equals(other.LeftPercent) && 
                   TopPercent.Equals(other.TopPercent) && 
                   WidthPercent.Equals(other.WidthPercent) && 
                   HeightPercent.Equals(other.HeightPercent) && 
                   IsCustom == other.IsCustom;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Placement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LeftPercent.GetHashCode();
                hashCode = (hashCode * 397) ^ TopPercent.GetHashCode();
                hashCode = (hashCode * 397) ^ WidthPercent.GetHashCode();
                hashCode = (hashCode * 397) ^ HeightPercent.GetHashCode();
                hashCode = (hashCode * 397) ^ IsCustom.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Placement left, Placement right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Placement left, Placement right)
        {
            return !Equals(left, right);
        }

        public string AsINISetting() =>
            $"{LeftPercent},{TopPercent},{WidthPercent},{HeightPercent}";


        public string Name { get; }

        public float LeftPercent { get; }
        
        public float TopPercent { get; }
        
        public float WidthPercent { get; }
        
        public float HeightPercent { get; }
        
        public bool IsCustom { get; }
    }
}