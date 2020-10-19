/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
    public class ShapeParser : IShapeParser
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
                default:
                    return PointJoin.CircularJoin;
            }
        }

        public virtual bool DontJoinPointsIfTooClose(string Name)
        {
            switch (Name)
            {
                case "Arc":
                case "ClosedArc":
                case "Pie":
                case "Bezier":
                case "BezierArc":
                case "Ellipse":
                case "Circle":
                case "RoundBox":
                    return true;
                default:
                    return false;
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
                case "BezierPie":
                case "Box":
                case "BoxF":
                case "ClosedArc":
                default:
                    return AfterStroke.Reset1st;
                case "BezierArc":
                case "Arc":
                case "Bezier":
                case "Line":

                    return AfterStroke.JoinEnds;
                case "Ellipse":
                case "Circle":
                    return 0;
            }
        }

        public virtual LineCommand GetLineDraw(string Name)
        {
            switch (Name)
            {
                case "Bezier":
                    //if (RenderSettings.Stroke > 0 && RenderSettings.FillMode != FillMode.Original)
                    //    return 0;
                    return LineCommand.Distinct;
                case "Arc":
                case "Pie":
                case "Circle":
                case "Ellipse":
                case "RoundBox":
                    return LineCommand.Distinct;
                default:
                    return 0;
            }
        }

        public virtual void GetLineSkip(string Name, FillMode mode, out SlopeType forData0, out SlopeType forData2)
        {
            forData0 = forData2 = 0;
            return;
            //switch (mode)
            //{
            //    case FillMode.Original:
            //    case FillMode.Inner:
            //    case FillMode.Outer:
            //        switch (Name)
            //        {
            //            case "Bezier":
            //            case "BezierArc":
            //            case "Arc":
            //                break;
            //            default:
            //                forData0 = forData2 = SlopeType.Steep;
            //                break;
            //        }
            //        break;
            //    case FillMode.FillOutLine:
            //        if (Stroke != 0)
            //        {
            //            forData0 = forData2 = SlopeType.Steep;
            //        }
            //        break;
            //    case FillMode.ExceptOutLine:
            //        forData2 = SlopeType.Steep;
            //        break;
            //    default:
            //        break;
            //}
        }

        public virtual bool NoNeedToSwapPerimeters(string Name)
        {
            switch (Name)
            {
                case "Bezier":
                    return true;
                default:
                    return false;
            }
        }

        public virtual FillMode GetFillMode(FillMode current, string Name, float stroke)
        {
            if (stroke != 0 && Name == "Line")
            {
                if (current != FillMode.FillOutLine)
                    return FillMode.DrawOutLine;
            }

            if (stroke == 0 && (Name == "Beizer" || Name == "BezierArc"))
            {
                switch (current)
                {
                    case FillMode.Original:
                    case FillMode.Inner:
                    case FillMode.FillOutLine:
                    case FillMode.ExceptOutLine:
                    case FillMode.Outer:
                        return FillMode.DrawOutLine;
                    default:
                        return current;
                }
            }
            return current;
        }
    }
}
