namespace CubivoxClient.UI
{
    /// <summary>
    /// A utility class to add extension methods convert Unity types to CubivoxCore types and vice versa.
    /// </summary>
    public static class StyleUtils
    {
        public static CubivoxCore.UI.Length ToCubivoxLength(this UnityEngine.UIElements.Length length)
        {
            var cubiLength = new CubivoxCore.UI.Length();
            cubiLength.Value = length.value;
            switch(length.unit)
            {
                case UnityEngine.UIElements.LengthUnit.Pixel:
                    cubiLength.Unit = CubivoxCore.UI.LengthUnit.Pixel;
                    break;
                case UnityEngine.UIElements.LengthUnit.Percent:
                    cubiLength.Unit = CubivoxCore.UI.LengthUnit.Percent;
                    break;
            }

            return cubiLength;
        }

        public static UnityEngine.UIElements.Length ToUnityLength(this CubivoxCore.UI.Length length)
        {
            var uniLength = new UnityEngine.UIElements.Length();
            uniLength.value = length.Value;
            switch (length.Unit)
            {
                case CubivoxCore.UI.LengthUnit.Pixel:
                    uniLength.unit = UnityEngine.UIElements.LengthUnit.Pixel;
                    break;
                case CubivoxCore.UI.LengthUnit.Percent:
                    uniLength.unit = UnityEngine.UIElements.LengthUnit.Percent;
                    break;
            }

            return uniLength;
        }

        public static CubivoxCore.Color ToCubivoxColor(this UnityEngine.Color color)
        {
            return new CubivoxCore.Color(color.r, color.g, color.b, color.a);
        }

        public static UnityEngine.Color ToUnityColor(this CubivoxCore.Color color)
        {
            return new UnityEngine.Color(color.R, color.G, color.B, color.A);
        }
    }
}