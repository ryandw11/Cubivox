using System;
using System.Reflection;
using CubivoxCore;
using CubivoxCore.UI;
using UnityEngine.UIElements;

namespace CubivoxClient.UI
{
    public class CubiStyles : Styles
    {
        private UnityEngine.UIElements.Label label;

        public CubiStyles(UnityEngine.UIElements.Label label)
        {
            this.label = label;
        }

        public override T GetStyle<T>(string key)
        {
            var properteries = label.style.GetType().GetProperties();
            foreach( var property in properteries)
            {
                if( !string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase) )
                {
                    continue;
                }

                if( !property.CanRead )
                {
                    continue;
                }

                var value = property.GetValue(label.style);
                switch( value )
                {
                    case StyleFloat styleFloat:
                        if( !typeof(T).IsPrimitive && typeof(T) != typeof(string) )
                        {
                            throw new ArgumentException($"Style property {key} is not of type {typeof(T).Name}.");
                        }
                        return (T) Convert.ChangeType(styleFloat.value, typeof(T));
                    case var styleEnum when styleEnum.GetType().IsGenericType && styleEnum.GetType().GetGenericTypeDefinition() == typeof(StyleEnum<>):
                        {
                            var enumType = styleEnum.GetType().GetGenericArguments()[0];
                            if( typeof(T) != typeof(string) )
                            {
                                throw new ArgumentException($"Style property {key} is not of type {typeof(T).Name}.");
                            }

                            var valueProperty = styleEnum.GetType().GetProperty("value");
                            if( valueProperty == null )
                            {
                                throw new ArgumentException("Internal Error: Could not find value property on the StyleEnum");
                            }
                            var valuePropertyValue = valueProperty.GetValue(styleEnum);

                            return (T)Convert.ChangeType(valuePropertyValue.ToString(), typeof(T));
                        }
                    case StyleLength styleLength:
                        if( typeof(T) == typeof(string) )
                        {
                            return (T)Convert.ChangeType(styleLength.value.ToCubivoxLength().ToString(), typeof(T));
                        }
                        else if( typeof(T) == typeof(CubivoxCore.UI.Length) )
                        {
                            return (T)(object)styleLength.value.ToCubivoxLength();
                        }
                        else
                        {
                            throw new ArgumentException($"Style property {key} is not of type {typeof(T).Name}.");
                        }
                    case StyleColor styleColor:
                        if (typeof(T) == typeof(string))
                        {
                            return (T)Convert.ChangeType(styleColor.value.ToCubivoxColor().ToString(), typeof(T));
                        }
                        else if (typeof(T) == typeof(CubivoxCore.Color))
                        {
                            return (T)(object)styleColor.value.ToCubivoxColor();
                        }
                        else
                        {
                            throw new ArgumentException($"Style property {key} is not of type {typeof(T).Name}.");
                        }
                }
                return default(T);
            }

            return default(T);
        }

        public override void SetStyle(string key, float value)
        {
            var property = GetProperty(key);
            var propValue = property.GetValue(label.style);
            if( propValue.GetType() != typeof(StyleFloat) )
            {
                throw new ArgumentException($"Style property {key} is not of type float.");
            }
            var styleFloat = (StyleFloat)propValue;

            var newStyleFloat = new StyleFloat();
            newStyleFloat.keyword = styleFloat.keyword;
            newStyleFloat.value = value;
            property.SetValue(label.style, newStyleFloat);
        }

        public override void SetStyle(string key, CubivoxCore.UI.Length value)
        {
            var property = GetProperty(key);
            var propValue = property.GetValue(label.style);
            if (propValue.GetType() != typeof(StyleLength))
            {
                throw new ArgumentException($"Style property {key} is not of type Length.");
            }
            var length = (StyleLength)propValue;

            var styleLength = new StyleLength();
            styleLength.keyword = length.keyword;
            styleLength.value = value.ToUnityLength();
            property.SetValue(label.style, styleLength);
        }

        public override void SetStyle(string key, Color value)
        {
            var property = GetProperty(key);
            var propValue = property.GetValue(label.style);
            if (propValue.GetType() != typeof(StyleColor))
            {
                throw new ArgumentException($"Style property {key} is not of type Color.");
            }
            var color = (StyleColor)propValue;

            var styleColor = new StyleColor();
            styleColor.keyword = color.keyword;
            styleColor.value = value.ToUnityColor();
            property.SetValue(label.style, styleColor);
        }

        public override void SetStyle(string key, string value)
        {
            var property = GetProperty(key);
            var propValue = property.GetValue(label.style);

            switch (propValue)
            {
                case StyleFloat styleFloat:
                    {
                        var newStyleFloat = new StyleFloat();
                        newStyleFloat.keyword = styleFloat.keyword;
                        newStyleFloat.value = float.Parse(value);
                        property.SetValue(label.style, newStyleFloat);
                        break;
                    }
                case var styleEnum when styleEnum.GetType().IsGenericType && styleEnum.GetType().GetGenericTypeDefinition() == typeof(StyleEnum<>):
                    {
                        var enumType = styleEnum.GetType().GetGenericArguments()[0];
                        var enumValue = Enum.Parse(enumType, value, ignoreCase: true);

                        var styleEnumInstance = Activator.CreateInstance(property.PropertyType, enumValue);

                        var valueProperty = styleEnumInstance.GetType().GetProperty("value");
                        valueProperty.SetValue(styleEnumInstance, enumValue);

                        property.SetValue(label.style, styleEnumInstance);
                        break;
                    }
                case StyleLength styleLength:
                    {
                        var newStyleLength = new StyleLength();
                        newStyleLength.keyword = styleLength.keyword;

                        newStyleLength.value = CubivoxCore.UI.Length.FromString(value).ToUnityLength();
                        property.SetValue(label.style, newStyleLength);
                    }
                    break;
                case StyleColor styleColor:
                    {
                        var newStyleColor = new StyleColor();
                        newStyleColor.keyword = styleColor.keyword;
                        newStyleColor.value = Color.FromString(value).ToUnityColor();
                        property.SetValue(label.style, newStyleColor);
                    }
                    break;
            }
        }

        private PropertyInfo GetProperty(string key)
        {
            var properteries = typeof(IStyle).GetProperties();
            foreach (var property in properteries)
            {
                if (!string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }
                return property;
            }

            throw new ArgumentException($"Cannot find style property with name {key}.");
        }
    }
}
