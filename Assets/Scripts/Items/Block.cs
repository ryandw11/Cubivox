﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sandbox.Items
{
    public class Block : SandboxObject
    {
        protected Color color;
        public Block() : base()
        {
            color = new Color(0, 0, 0, 0);
        }

        public override string GetModel()
        {
            throw new System.NotImplementedException();
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }

        public Color GetColor()
        {
            return color;
        }
    }
}