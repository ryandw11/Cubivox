using CubivoxCore.UI;
using UnityEngine;

namespace CubivoxClient.UI
{
    public class CubiLabel : UnityEngine.UIElements.Label, Label
    {
        private CubiStyles cubiStyles;
        public CubiLabel()
        {
            cubiStyles = new CubiStyles(this);
        }

        public string Text { get => text; set => text = value; }
        public bool SupportsRichText { get => enableRichText; set => enableRichText = value; }

        public string Name { get => name; set => name = value; }

        public Styles Style => cubiStyles;

        public void Add(Element element)
        {
            Add((UnityEngine.UIElements.VisualElement)element);
        }
    }
}
