/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Runtime.CompilerServices;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    public interface IBoundary : IUpdateArea, IPolygonal, IAccumulative, ICloneable<IBoundary>, IEnumHolder<BoundaryKind>,
       IConvertible<IUpdateArea, IType>, IConvertible<IUpdateArea>, INotToBeImplementedOutsideGWS
    {
        /// <summary>
        /// Gets far right X co-ordinate of recently drawn area of this object.
        /// </summary>
        int MaxX { get; }

        /// <summary>
        /// Gets far bottom Y co-ordinate of recently drawn area of this object.
        /// </summary>
        int MaxY { get; }
    }
    internal interface IExBoundary: IBoundary, IExUpdatable<int[], bool>, IExClearable<bool>,
        IArrayHolder<int>, IExEnumHolder<BoundaryKind>, IExUpdatable<IBounds, bool> 
    {
        /// <summary>
        /// Gets or sets GWS assigned type of the object for the purpose of handling rendering operation.
        /// </summary>
        new ObjType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        new IExBoundary Clone();
    }

    partial class Factory 
    {
        sealed class Boundary : IExBoundary
        {
            #region VARIABLES
            int[] Data;
            BoundaryKind kind = BoundaryKind.Boundary;
            ObjType type;
            //X = Data[0]
            //Y = Data[1]
            //MaxX = Data[2]
            //MaxY = Data[3]

            //XOfMinY = Data[4]
            //YOfMinX = Data[5]
            //XOfMaxY = Data[6]
            //YOfMaxX = Data[7]

            static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
            #endregion

            #region CONSTRUCTOR
            public Boundary(BoundaryKind kind = BoundaryKind.Boundary)
            {
                this.kind = kind;
                //XOfMinY = Data[4]
                //YOfMinX = Data[5]
                //XOfMaxY = Data[6]
                //YOfMaxX = Data[7]

                switch (kind)
                {
                    case BoundaryKind.Boundary:
                    case BoundaryKind.AbsBoundary:
                    case BoundaryKind.ItemBoundary:
                    default:
                        Data = new int[] { int.MaxValue, int.MaxValue, 0, 0 };
                        break;
                    case BoundaryKind.RotatedBoundary:
                    case BoundaryKind.RotatedItemBoundary:
                        Data = new int[] { int.MaxValue, int.MaxValue, 0, 0, int.MaxValue, int.MaxValue, 0, 0 };
                        break;
                }
            }
            public Boundary(int x, int y, int w, int h, BoundaryKind kind = BoundaryKind.Boundary) :
                this(kind)
            {
                var Data = this.Data;
                Data[0] = x;
                Data[1] = y;
                Data[2] = x + w;
                Data[3] = y + h;

                if (Data.Length > 7)
                {
                    Data[4] = x;
                    Data[5] = y;
                    Data[6] = x + w;
                    Data[7] = y + h;
                }
            }
            public Boundary(IBounds parameter, BoundaryKind kind = BoundaryKind.Boundary) :
                this(kind)
            {
                int minX, minY, maxX, maxY;
                parameter.GetBounds(out minX, out minY, out maxX, out maxY);
                maxX += minX;
                maxY += minY;
                var Data = this.Data;

                if (maxX == 0 && maxY == 0)
                {
                    Data[0] = Data[1] = int.MaxValue;
                    Data[2] = Data[3] = 0;
                    return;
                }

                Data[0] = minX;
                Data[1] = minY;
                Data[2] = maxX;
                Data[3] = maxY;

                if (Data.Length > 7)
                {
                    Data[4] = minX;
                    Data[5] = minY;
                    Data[6] = maxX;
                    Data[7] = maxY;
                    switch (kind)
                    {
                        case BoundaryKind.Boundary:
                        case BoundaryKind.RotatedBoundary:
                            this.kind = BoundaryKind.RotatedBoundary;
                            break;
                        case BoundaryKind.ItemBoundary:
                        case BoundaryKind.RotatedItemBoundary:
                            this.kind = BoundaryKind.RotatedBoundary;
                            break;
                        default:
                            break;
                    }
                }
                /*
                    XOfMinY = Data[4]
                    YOfMinX = Data[5]
                    XOfMaxY = Data[6]
                    YOfMaxX = Data[7]
                */           
            }
            public Boundary(ObjType type, BoundaryKind kind = BoundaryKind.Boundary) :
                this(kind)
            {
                this.type = type;
            }
            public Boundary(ObjType type, int[] boundary, BoundaryKind kind = BoundaryKind.Boundary) :
                this(boundary, kind)
            {
                this.type = type;
            }
            public Boundary(int[] boundary, BoundaryKind kind = BoundaryKind.Boundary) :
                this(kind)
            {
                
                if (boundary != null)
                    Array.Copy(boundary, Data, Math.Min(boundary.Length, Data.Length));
            }
            #endregion

            #region PROPERTIES
            /// <summary>
            /// Far left X co-ordinate of recently drawn area of this object.
            /// </summary>
            public int X => Data[0];

            /// <summary>
            /// Far left Y co-ordinate of recently drawn area of this object.
            /// </summary>
            public int Y => Data[1];

            /// <summary>
            /// Far right X co-ordinate of recently drawn area of this object.
            /// </summary>
            public int MaxX => Data[2];

            /// <summary>
            /// Far bottom Y co-ordinate of recently drawn area of this object.
            /// </summary>
            public int MaxY => Data[3];

            public bool Valid => Data[2] > 0 && Data[3] > 0;
            public int Width => Data[2] - Data[0];
            public int Height => Data[3] - Data[1];
            public bool Accumulative { get; set; }
            public ObjType Type => type;
            public BoundaryKind Kind => kind;
            #endregion

            #region IMPLICIT INTERFACE PROPERTY IMPLEMENTATION
            BoundaryKind IExEnumHolder<BoundaryKind>.Kind
            {
                get => Kind;
                set
                {
                    var k = value;
                    if (k == kind)
                        return;
                    kind = k;
                    //XOfMinY = Data[4]
                    //YOfMinX = Data[5]
                    //XOfMaxY = Data[6]
                    //YOfMaxX = Data[7]

                    switch (kind)
                    {
                        case BoundaryKind.Boundary:
                        case BoundaryKind.AbsBoundary:
                        case BoundaryKind.ItemBoundary:
                        default:
                            Data = new int[] { int.MaxValue, int.MaxValue, 0, 0 };
                            break;
                        case BoundaryKind.RotatedBoundary:
                        case BoundaryKind.RotatedItemBoundary:
                            Data = new int[] { int.MaxValue, int.MaxValue, 0, 0, int.MaxValue, int.MaxValue, 0, 0 };
                            break;
                    }
                }
            }
            int IPoint.X => Data[0] == int.MaxValue ? 0 : Data[0];
            int IPoint.Y => Data[1] == int.MaxValue ? 0 : Data[1];
            ObjType IType.Type => Type;
            int IBoundary.MaxX => Data[2];
            int IBoundary.MaxY => Data[3];
            BoundaryKind IEnumHolder<BoundaryKind>.Kind => kind;
            ObjType IExBoundary.Type { get => type; set => type = value; }
            int[] IArrayHolder<int>.Data => Data;
            int ICount.Count
            {
                get
                {
                    switch (kind)
                    {
                        case BoundaryKind.RotatedBoundary:
                        case BoundaryKind.RotatedItemBoundary:
                            return 4;
                        default:
                            return 8;
                    }
                }
            }
            #endregion

            #region UPDATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IExUpdatable<IBounds, bool>.Update(IBounds value, bool parameter)
            {
                if (value == null)
                    return;
                if(value is IArrayHolder<int>)
                {
                    ((IExUpdatable<int[], bool>)this).Update(((IArrayHolder<int>)value).Data);
                    return;
                }
                int minX, minY, maxX, maxY;
                value.GetBounds(out minX, out minY, out maxX, out maxY);
                maxX += minX;
                maxY += minY;
                var Data = this.Data;

                if (maxX == 0 && maxY == 0)
                {
                    Data[0] = Data[1] = int.MaxValue;
                    Data[2] = Data[3] = 0;
                    return;
                }
                if (minX < Data[0])
                    Data[0] = minX;

                if (minY < Data[1])
                    Data[1] = minY;

                if (maxX > Data[2])
                    Data[2] = maxX;
                if (maxY > Data[3])
                    Data[3] = maxY;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IExUpdatable<int[], bool>.Update(int[] boundary, bool byForce)
            {
                var Data = this.Data;
                if (boundary == null)
                {
                    Data[0] = Data[1] = int.MaxValue;
                    Data[2] = Data[3] = 0;
                    return;
                }
                if (boundary.Length < 4)
                    return;

                if (boundary[2] == 0 && boundary[3] == 0)
                {
                    Data[0] = Data[1] = int.MaxValue;
                    Data[2] = Data[3] = 0;
                    return;
                }
                if (boundary[0] < Data[0])
                    Data[0] = boundary[0];
                if (boundary[1] < Data[1])
                    Data[1] = boundary[1];

                if (boundary[2] > Data[2])
                    Data[2] = boundary[2];
                if (boundary[3] > Data[3])
                    Data[3] = boundary[3];

                if (Data.Length < 8 || boundary.Length < 8)
                    return;

                //XOfMinY = Data[4]
                //YOfMinX = Data[5]
                //XOfMaxY = Data[6]
                //YOfMaxX = Data[7]

                Data[4] = boundary[4];
                Data[5] = boundary[5];
                Data[6] = boundary[6];
                Data[7] = boundary[7];

                var pts = new Point[]
                {
                new Point(Data[4], Data[1]),
                new Point(Data[2], Data[7]),
                new Point(Data[6], Data[3]),
                new Point(Data[0], Data[5])
                };

                this.CorrectBoundary(pts, ref Data[4], ref Data[5], ref Data[6], ref Data[7]);
            }
            #endregion

            #region CLEAR
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IExClearable<bool>.Clear(bool byForce)
            {
                if (Accumulative && !byForce)
                    return;
                var Data = this.Data;
                Data[0] = Data[1] = int.MaxValue;
                Data[2] = Data[3] = 0;
                if (Data.Length < 8)
                    return;
                Data[4] = Data[5] = int.MaxValue;
                Data[6] = Data[7] = 0;
            }
            #endregion

            #region GET BOUNDS
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetBounds(out int x, out int y, out int w, out int h)
            {
                var Data = this.Data;
                x = Data[0];
                y = Data[1];

                w = Data[2] - Data[0];
                h = Data[3] - Data[1];

                if (Data[2] == 0 || Data[3] == 0 || w <= 0 || h <= 0)
                {
                    x = y = w = h = 0;
                    return;
                }
            }
            #endregion

            #region CONTAINS
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(float x, float y)
            {
                var Data = this.Data;

                if (Data[2] == 0 || Data[3] == 0 || x < Data[0] || y < Data[1] || x > Data[2] || y > Data[3])
                    return false;

                switch (Kind)
                {   case BoundaryKind.RotatedItemBoundary:
                    case BoundaryKind.RotatedBoundary:
                        return (
                        Vectors.InTriangle(x, y, Data[4], Data[1], Data[2], Data[7], Data[6], Data[3]) ||
                        Vectors.InTriangle(x, y, Data[6], Data[3], Data[0], Data[5], Data[4], Data[1]));
                    default:
                        return true;
                }
                //XOfMinY = Data[4]
                //YOfMinX = Data[5]
                //XOfMaxY = Data[6]
                //YOfMaxX = Data[7]

            }
            #endregion

            #region GET POINTS
            Point[] IPolygonal<Point>.GetPoints()
            {
                var Data = this.Data;
                if (Data.Length < 8)
                    return null;

                return new Point[]
                {
                new Point(Data[4], Data[1]),
                new Point(Data[2], Data[7]),
                new Point(Data[6], Data[3]),
                new Point(Data[0], Data[5])
                };
            }
            #endregion

            #region CONVERT
            public IUpdateArea Convert() =>
                 new UpdateArea(this);
            public IUpdateArea Convert(IType argument)=>
                new UpdateArea(this, argument.Type);
            #endregion

            #region IS CONFINED
            public bool IsConfined(int maxWidth, int maxHeight)
            {
                var Data = this.Data;
                return
                    Data[2] > 0 &&
                    Data[3] > 0 &&
                    Data[0] > -2 &&
                    Data[1] > -2 &&
                    Data[2] <= maxWidth &&
                    Data[3] <= maxHeight;
            }
            #endregion

            #region CLONE
            IExBoundary IExBoundary.Clone()
            {
                var RESULT = new Boundary();
                RESULT.kind = kind;
                RESULT.type = type;
                RESULT.Data = new int[Data.Length];
                System.Array.Copy(Data, 0, RESULT.Data, 0, Data.Length);
                return RESULT;
            }
            public IBoundary Clone() =>
                ((IExBoundary)this).Clone();
            object ICloneable.Clone() =>
                ((IExBoundary)this).Clone();
            #endregion

            public override string ToString()
            {
                return string.Format(description, X, Y, MaxX, MaxY);
            }
        }
    }
}