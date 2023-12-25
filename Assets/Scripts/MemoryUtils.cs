using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MemoryUtils
{
    public static void Fill3DArray<T>(ref T[,,] array, T value, int totalLength) where T : unmanaged
    {
        unsafe
        {
            fixed (T* start = &array[0, 0, 0])
            {
                T* current = start;
                var span = new Span<T>(current, totalLength);
                span.Fill(value);
            }
        }
    }

    public static T[,,] OneDArrayTo3DArray<T>(ref T[] array, int componentLength) where T : unmanaged
    {
        int totalLength = componentLength * componentLength * componentLength;
        T[,,] result = new T[componentLength, componentLength, componentLength];
        unsafe
        {
            fixed (T* start = &array[0])
            {
                T* current = start;
                var span = new Span<T>(current, totalLength);
                fixed (T* resultStart = &result[0, 0, 0])
                {
                    T* resultCurrent = resultStart;
                    var resultSpan = new Span<T>(resultCurrent, totalLength);
                    span.CopyTo(resultSpan);
                }
            }
        }
        return result;
    }
}