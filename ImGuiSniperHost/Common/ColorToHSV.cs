using System.Drawing;
using System.Numerics;

namespace ImGuiSniperHost.Common
{
    public static class ColorToHSV
    {
        public static Vector4 ToImGuiVec4(this Color color)
        {
            return new Vector4((float) color.R / 100, (float) color.G / 100, (float) color.B / 100, 1);
        }
    }
}