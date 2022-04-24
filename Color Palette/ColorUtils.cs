using System;
using System.Windows.Media;

namespace OokiiTsuki.Palette
{
    public static class ColorUtils
    {
        private const int MIN_ALPHA_SEARCH_MAX_ITERATIONS = 10;
        private const int MIN_ALPHA_SEARCH_PRECISION = 1;
        private const float MIN_CONTRAST_TITLE_TEXT = 3.0f;
        private const float MIN_CONTRAST_BODY_TEXT = 4.5f;
        public static float CalculateXyzLuma(this int color)
        {
            Color c = color.ToColor();
            return (0.2126f * c.R +
                    0.7152f * c.G +
                    0.0722f * c.B / 255f);
        }
        public static float CalculateContrast(this int color1, int color2)
        {
            return Math.Abs(color1.CalculateXyzLuma() - color2.CalculateXyzLuma());
        }
        public static float[] RGBtoHSL(int r, int g, int b)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float deltaMaxMin = max - min;
            float h, s;
            float l = (max + min) / 2f;
            float[] hsl = new float[3];
            if (max == min)
            {
                // Monochromatic
                h = s = 0f;
            }
            else
            {
                if (max == rf)
                {
                    h = ((gf - bf) / deltaMaxMin) % 6f;
                }
                else if (max == gf)
                {
                    h = ((bf - rf) / deltaMaxMin) + 2f;
                }
                else
                {
                    h = ((rf - gf) / deltaMaxMin) + 4f;
                }
                s = deltaMaxMin / (1f - Math.Abs(2f * l - 1f));
            }
            hsl[0] = (h * 60f) % 360f;
            hsl[1] = s;
            hsl[2] = l;
            return hsl;
        }
        public static int HSLtoRGB(float[] hsl)
        {
            float h = hsl[0];
            float s = hsl[1];
            float l = hsl[2];
            float c = (1f - Math.Abs(2 * l - 1f)) * s;
            float m = l - 0.5f * c;
            float x = c * (1f - Math.Abs((h / 60f % 2f) - 1f));
            int hueSegment = (int)h / 60;
            int r = 0, g = 0, b = 0;
            switch (hueSegment)
            {
                case 0:
                    r = (int)Math.Round(255 * (c + m));
                    g = (int)Math.Round(255 * (x + m));
                    b = (int)Math.Round(255 * m);
                    break;
                case 1:
                    r = (int)Math.Round(255 * (x + m));
                    g = (int)Math.Round(255 * (c + m));
                    b = (int)Math.Round(255 * m);
                    break;
                case 2:
                    r = (int)Math.Round(255 * m);
                    g = (int)Math.Round(255 * (c + m));
                    b = (int)Math.Round(255 * (x + m));
                    break;
                case 3:
                    r = (int)Math.Round(255 * m);
                    g = (int)Math.Round(255 * (x + m));
                    b = (int)Math.Round(255 * (c + m));
                    break;
                case 4:
                    r = (int)Math.Round(255 * (x + m));
                    g = (int)Math.Round(255 * m);
                    b = (int)Math.Round(255 * (c + m));
                    break;
                case 5:
                case 6:
                    r = (int)Math.Round(255 * (c + m));
                    g = (int)Math.Round(255 * m);
                    b = (int)Math.Round(255 * (x + m));
                    break;
            }
            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b).ToInt();
        }
        public static float[] RGBToXYZ(byte r, byte g, byte b)
        {
            float[] outXyz = new float[3];
            float sr = r / 255.0f;
            sr = sr < 0.04045f ? sr / 12.92f : (float)Math.Pow((sr + 0.055f) / 1.055f, 2.4f);
            float sg = g / 255.0f;
            sg = sg < 0.04045f ? sg / 12.92f : (float)Math.Pow((sg + 0.055f) / 1.055f, 2.4f);
            float sb = b / 255.0f;
            sb = sb < 0.04045f ? sb / 12.92f : (float)Math.Pow((sb + 0.055f) / 1.055f, 2.4f);

            outXyz[0] = 100 * (sr * 0.4124f + sg * 0.3576f + sb * 0.1805f);
            outXyz[1] = 100 * (sr * 0.2126f + sg * 0.7152f + sb * 0.0722f);
            outXyz[2] = 100 * (sr * 0.0193f + sg * 0.1192f + sb * 0.9505f);
            return outXyz;
        }
        public static Color ToColor(this int color)
        {
            Color c = new Color();
            c.B = (byte)(color & 0xFF);
            c.G = (byte)((color >> 8) & 0xFF);
            c.R = (byte)((color >> 16) & 0xFF);
            c.A = (byte)((color >> 24) & 0xFF);
            return c;
        }
        public static int ToInt(this Color color)
        {
            return (color.A & 0xFF) << 24 | (color.R & 0xFF) << 16 | (color.G & 0xFF) << 8 | (color.B & 0xFF);
        }
        public static int Red(this int color) => (color >> 16) & 0xFF;
        public static int Green(this int color) => (color >> 8) & 0xFF;
        public static int Blue(this int color) => color & 0xFF;

        ///<summary>Returns an appropriate color to use for any 'title' text which is displayed over this <c>color</c>.
        ///This color is guaranteed to have sufficient contrast.</summary>
        ///<returns>An appropriate color</returns>
        public static Color GetTitleTextColor(this Color color)
        {
            int lightTitleAlpha = CalculateMinimumAlpha(Colors.White, color, MIN_CONTRAST_TITLE_TEXT);
            if (lightTitleAlpha != -1)
            {
                // If we found valid light values, use them and return
                return Color.FromArgb((byte)lightTitleAlpha, 255, 255, 255);
            }
            int darkTitleAlpha = CalculateMinimumAlpha(Colors.Black, color, MIN_CONTRAST_TITLE_TEXT);
            if (darkTitleAlpha != -1)
            {
                // If we found valid dark values, use them and return
                return Color.FromArgb((byte)darkTitleAlpha, 0, 0, 0);
            }


            return lightTitleAlpha != -1
                ? Color.FromArgb((byte)lightTitleAlpha, 255, 255, 255)
                : Color.FromArgb((byte)darkTitleAlpha, 0, 0, 0);
        }

        ///<summary>Returns an appropriate color to use for any 'body' text which is displayed over this <c>color</c>.
        ///This color is guaranteed to have sufficient contrast.</summary>
        ///<returns>An appropriate color</returns>
        public static Color GetBodyTextColor(this Color color)
        {
            int lightBodyAlpha = CalculateMinimumAlpha(Colors.White, color, MIN_CONTRAST_BODY_TEXT);
            if (lightBodyAlpha != -1)
            {
                // If we found valid light values, use them and return
                return Color.FromArgb((byte)lightBodyAlpha, 255, 255, 255);
            }
            int darkBodyAlpha = CalculateMinimumAlpha(Colors.Black, color, MIN_CONTRAST_BODY_TEXT);
            if (darkBodyAlpha != -1)
            {
                // If we found valid dark values, use them and return
                return Color.FromArgb((byte)darkBodyAlpha, 0, 0, 0);
            }
            return lightBodyAlpha != -1
                ? Color.FromArgb((byte)lightBodyAlpha, 255, 255, 255)
                : Color.FromArgb((byte)darkBodyAlpha, 0, 0, 0);
        }
        public static int CalculateMinimumAlpha(this Color foreground, Color background, float minContrastRatio)
        {
            if (background.A != 255)
                throw new ArgumentException("background can not be translucent: #" + background.ToInt().ToString("X"));

            // First lets check that a fully opaque foreground has sufficient contrast
            Color testForeground = Color.FromArgb(255, foreground.R, foreground.G, foreground.B);
            float testRatio = CalculateContrast(testForeground, background);
            if (testRatio < minContrastRatio)
            {
                // Fully opaque foreground does not have sufficient contrast, return error
                return -1;
            }
            // Binary search to find a value with the minimum value which provides sufficient contrast
            int numIterations = 0;
            int minAlpha = 0;
            int maxAlpha = 255;

            while (numIterations <= MIN_ALPHA_SEARCH_MAX_ITERATIONS
                && (maxAlpha - minAlpha) > MIN_ALPHA_SEARCH_PRECISION)
            {
                int testAlpha = (minAlpha + maxAlpha) / 2;

                testForeground = Color.FromArgb((byte)testAlpha, foreground.R, foreground.G, foreground.B);
                testRatio = CalculateContrast(testForeground, background);

                if (testRatio < minContrastRatio)
                {
                    minAlpha = testAlpha;
                }
                else
                {
                    maxAlpha = testAlpha;
                }

                numIterations++;
            }

            // Conservatively return the max of the range of possible alphas, which is known to pass.
            return maxAlpha;
        }
        public static float CalculateContrast(this Color foreground, Color background)
        {
            if (background.A != 255)
                throw new ArgumentException("background can not be translucent: #" + background.ToInt().ToString("X"));
            if (foreground.A != 255)
            {
                // If the foreground is translucent, composite the foreground over the background
                foreground = CompositeColors(foreground, background);
            }

            float luminance1 = CalculateLuminance(foreground) + 0.05f;
            float luminance2 = CalculateLuminance(background) + 0.05f;

            // Now return the lighter luminance divided by the darker luminance
            return Math.Max((float)luminance1, (float)luminance2) / Math.Min((float)luminance1, (float)luminance2);
        }
        public static Color CompositeColors(this Color foreground, Color background)
        {
            byte bgAlpha = background.A;
            byte fgAlpha = foreground.A;
            byte a = CompositeAlpha(fgAlpha, bgAlpha);

            byte r = CompositeComponent(foreground.R, fgAlpha, background.R, bgAlpha, a);
            byte g = CompositeComponent(foreground.G, fgAlpha, background.G, bgAlpha, a);
            byte b = CompositeComponent(foreground.B, fgAlpha, background.B, bgAlpha, a);

            return Color.FromArgb(a, r, g, b);
        }
        private static byte CompositeAlpha(this byte foregroundAlpha, byte backgroundAlpha)
        {
            return (byte)(0xFF - (((0xFF - backgroundAlpha) * (0xFF - foregroundAlpha)) / 0xFF));
        }
        private static byte CompositeComponent(int fgC, int fgA, int bgC, int bgA, int a)
        {
            if (a == 0)
                return 0;
            return (byte)(((0xFF * fgC * fgA) + (bgC * bgA * (0xFF - fgA))) / (a * 0xFF));
        }
        public static float CalculateLuminance(Color color)
        {
            // Luminance is the Y component
            return RGBToXYZ(color.R, color.G, color.B)[1] / 100;
        }
    }
}