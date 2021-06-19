using System;

namespace Cubivox.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Name : Attribute
    {
        private string name;
        public Name(string name)
        {
            this.name = name;
        }

        public string GetValue()
        {
            return name;
        }

    }
}
