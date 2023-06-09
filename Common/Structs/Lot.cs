/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System.Runtime.InteropServices;

namespace MnM.GWS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Lot<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public static Lot<T1, T2> Empty = new Lot<T1, T2>();

        public Lot(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override string ToString()
        {
            return "Item1: " + Item1 + ", Item2: " + Item2;
        }
    }
   
    [StructLayout(LayoutKind.Sequential)]
    public struct Lot<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public static Lot<T1, T2, T3> Empty = new Lot<T1, T2, T3>();

        public Lot(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
        public override string ToString()
        {
            return "Item1: " + Item1 + ", Item2: " + Item2 + ", Item3: " + Item3;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Lot<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public static Lot<T1, T2, T3, T4> Empty = new Lot<T1, T2, T3, T4>();

        public Lot(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }
        public override string ToString()
        {
            return "Item1: " + Item1 + ", Item2: " + Item2 + ", Item3: " + Item3 + ", Item4: " + Item4;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Lot<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public static Lot<T1, T2, T3, T4, T5> Empty = new Lot<T1, T2, T3, T4, T5>();

        public Lot(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }
        public override string ToString()
        {
            return "Item1: " + Item1 + ", Item2: " + Item2 + ", Item3: " + Item3 + ", Item4: " + Item4 + ", Item5: " + Item5;
        }
    }
   
    public static class Lot
    {
        public static Lot<T1, T2> Create<T1, T2>(T1 item1 = default(T1), T2 item2 = default(T2)) =>
            new Lot<T1, T2>(item1, item2);
        public static Lot<T1, T2, T3> Create<T1, T2, T3>(T1 item1 = default(T1), T2 item2 = default(T2), T3 item3 = default(T3)) =>
            new Lot<T1, T2, T3>(item1, item2, item3);

        public static Lot<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1 = default(T1), T2 item2 = default(T2), T3 item3 = default(T3), T4 item4 = default(T4)) =>
            new Lot<T1, T2, T3, T4>(item1, item2, item3, item4);

        public static Lot<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>
            (T1 item1 = default(T1), T2 item2 = default(T2), T3 item3 = default(T3), T4 item4 = default(T4), T5 item5 = default(T5)) =>
            new Lot<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
    }
}
