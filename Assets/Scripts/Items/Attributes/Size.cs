using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace Cubvox.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Size : Attribute
    {
        private int x;
        private int y;
        public Size(int x, int y)
        {
            if (x < 1) throw new ArgumentException("X must be greater than 0");
            if (y < 1) throw new ArgumentException("Y must be greater than 0");
            this.x = x;
            this.y = y;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }
}
