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
}