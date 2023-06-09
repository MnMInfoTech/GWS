/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
namespace MnM.GWS
{
    #region ISHAPE PARSER
    public interface IShapeParser
    {
        /// <summary>
        /// Suggests the point join rules of the for the given type of shape.
        /// </summary>
        /// <param name="TypeName">Case sensitive type of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns></returns>
        PointJoin GetStrokeJoin(string TypeName);

        /// <summary>
        /// Suggests the AfterStroke rule for the given type of shape. 
        /// </summary>
        /// <param name="TypeName">Case sensitive type of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns>AfterStroke Enum for Named shape. </returns>
        AfterStroke GetAfterStroke(string TypeName);

        /// <summary>
        /// Suggests the applicable fillmode for for the given type of shape. 
        /// </summary>
        /// <param name="current">Current fill mode</param>
        /// <param name="TypeName">Name of the shape</param>
        /// <param name="stroke">Value of stroke</param>
        void ResetFillOptions(ref FillCommand current, string TypeName, float stroke);
    }
    #endregion

#if DevSupport
    public
#else
    internal
#endif
    class ShapeParser : IShapeParser
    {
        public virtual PointJoin GetStrokeJoin(string Name)
        {
            switch (Name)
            {
                case "Polygon":
                    return PointJoin.PolygonJoin;
                case "BezierArc":
                case "Arc":
                    return PointJoin.ArcJoin;
                case "ClosedArc":
                    return PointJoin.CloseArcJoin;
                case "Pie":
                case "BezierPie":
                    return PointJoin.PieJoin;
                case "Bezier":
                case "Line":
                    return PointJoin.ConnectEach;
                case "Glyph":
                    return PointJoin.CircularJoinOpen;
                default:
                    return PointJoin.CircularJoin;
            }
        }

        public virtual AfterStroke GetAfterStroke(string Name)
        {
            switch (Name)
            {
                case "Triangle":
                case "Trapazoid":
                case "Rhombus":
                case "Trapezium":
                case "Polygon":
                case "Pie":
                case "Box":
                case "BoxF":
                case "ClosedArc":
                default:
                    return AfterStroke.Reset1st;
                case "Arc":
                case "Bezier":
                case "Line":

                    return AfterStroke.JoinEnds;
                case "Ellipse":
                case "Circle":
                    return 0;
            }
        }

        public virtual void ResetFillOptions(ref FillCommand mode, string Name, float stroke)
        {
            if (stroke != 0)
            {
                switch (Name)
                {
                    case "Bezier":
                    case "Line":
                        mode &= ~FillCommand.XORFill;
                        if ((mode & FillCommand.FillOddLines) == FillCommand.FillOddLines ||
                            (mode & FillCommand.FillOddLines) == FillCommand.FillOddLines)
                        {
                            mode &= ~FillCommand.FillOddLines;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (Name)
                {
                    case "Bezier":
                    case "Arc":
                        mode &= ~FillCommand.XORFill;
                        if ((mode & FillCommand.FillOddLines) == FillCommand.FillOddLines)
                        {
                            mode &= ~FillCommand.FillOddLines;
                            return;
                        }
                        mode = FillCommand.DrawOutLines;
                        return;
                    default:
                        if ((mode & FillCommand.FillOddLines) == FillCommand.FillOddLines)
                        {
                            mode &= ~FillCommand.FillOddLines;
                            return;
                        }
                        break;
                }
            }
        }
    }
}
#endif