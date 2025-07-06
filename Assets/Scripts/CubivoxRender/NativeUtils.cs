using System.Collections.Generic;
using Unity.Collections;

namespace CubivoxRender
{
    public static class NativeUtils
    {
        public delegate T NativeSelector<T, S>(S input);
        public static List<T> Select<T, S>(this NativeList<S> nativeList, NativeSelector<T, S> transformFunc ) where S : unmanaged
        {
            List<T> output = new List<T>(nativeList.Length);
            foreach( var item in nativeList )
            {
                output.Add(transformFunc(item));
            }

            return output;
        }

        public static T[] AsSharpArray<T>(this NativeList<T> nativeList) where T : unmanaged
        {
            T[] output = new T[nativeList.Length];
            for( int i = 0; i < nativeList.Length; ++i )
            {
                output[i] = nativeList[i];
            }

            return output;
        }
    }
}