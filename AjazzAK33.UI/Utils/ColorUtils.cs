﻿using Avalonia.Media;
using System;

namespace AjazzAK33.UI
{
    public static class ColorUtils
    {
        public static void ToHsv(Color color, out double hue, out double saturation, out double value)
        {
            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            var delta = max - min;

            hue = 0d;
            if (delta != 0)
            {
                if (color.R == max) hue = (color.G - color.B) / (double)delta;
                else if (color.G == max) hue = 2d + (color.B - color.R) / (double)delta;
                else if (color.B == max) hue = 4d + (color.R - color.G) / (double)delta;
            }

            hue *= 60;
            if (hue < 0.0) hue += 360;

            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color FromHsv(double hue, double saturation, double value)
        {
            saturation = Math.Max(Math.Min(saturation, 1), 0);
            value = Math.Max(Math.Min(value, 1), 0);

            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            var v = (byte)(value);
            var p = (byte)(value * (1 - saturation));
            var q = (byte)(value * (1 - f * saturation));
            var t = (byte)(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromRgb(v, t, p);
                case 1: return Color.FromRgb(q, v, p);
                case 2: return Color.FromRgb(p, v, t);
                case 3: return Color.FromRgb(p, q, v);
                case 4: return Color.FromRgb(t, p, v);
                default: return Color.FromRgb(v, p, q);
            }
        }

        public static Color ChangeHue(Color color, double offset)
        {
            if (offset == 0)
                return color;

            ToHsv(color, out var hue, out var saturation, out var value);

            hue += offset;

            while (hue > 360) hue -= 360;
            while (hue < 0) hue += 360;

            return FromHsv(hue, saturation, value);
        }

        public static System.Drawing.Color ToDrawingClr(this Color clr)
            => System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

        public static Color GetFontColorFromBackground(this Color c)
        {
            if ((c.R * 0.299 + c.G * 0.587 + c.B * 0.114) > 186)
                return Colors.Black;
            else
                return Colors.White;
        }
    }
}
