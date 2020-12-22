using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sandbox.Items
{
    public class Item : SandboxObject
    {
        private int xSize;
        private int ySize;
        public Item() : base()
        {
            Size size = (Size)GetType().GetCustomAttributes(typeof(Size), true)[0];
            xSize = size.GetX();
            ySize = size.GetY();
        }

        public int GetXSize()
        {
            return xSize;
        }

        public int GetYSize()
        {
            return ySize;
        }

        public override string GetModel()
        {
            throw new System.NotImplementedException();
        }
    }
}
