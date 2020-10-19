/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections.Generic;

namespace MnM.GWS
{
#if (GWS || Window)
    #region ISHAPE PARSER
    public interface IShapeParser
    {
        /// <summary>
        /// Get the point join rules of the shape named.
        /// </summary>
        /// <param name="Name">Case sensitive name of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns></returns>
        PointJoin GetStrokeJoin(string Name);

        /// <summary>
        /// Returns the rule for joining close points for standard shapes. 
        /// </summary>
        /// <param name="Name">Case sensitive name of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns>True if Points should not be joined for the shape when they are too close.
        bool DontJoinPointsIfTooClose(string Name);

        /// <summary>
        /// Gets the AfterStroke rule for the Shape named. So that is can be drawn correctly.
        /// </summary>
        /// <param name="Name">Case sensitive name of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns>AfterStroke Enum for Named shape. </returns>
        AfterStroke GetAfterStroke(string Name);

        /// <summary>
        /// Get the rule for line drawing for the given shape.
        /// </summary>
        /// <param name="Name">Case sensitive name of shape as used in IRecognizable e.g. "Bezier".</param>
        /// <returns>Returns the LineDraw enum used to decide how line is drawn.</returns>
        LineCommand GetLineDraw(string Name);

        /// <summary>
        /// Gets lien skip information for outer and inner parameters.
        /// </summary>
        /// <param name="Name">Name of shape which is being rendered.</param>
        /// <param name="mode">Fill mode which shape will be drawn with.</param>
        /// <param name="forData0">Line skip option for outer perimeter.</param>
        /// <param name="forData2">Line skip option for inner perimeter.</param>
        void GetLineSkip(string Name, FillMode mode, out SlopeType forData0, out SlopeType forData2);

        /// <summary>
        /// Return the swap perimeters state. 
        /// Important for besier where inside and outside can swap due to the orientation of the line.!!!!or a bug!!!!
        /// </summary>
        /// <param name="Name">Case sensitive name of shape as used in IRecognizable e.g. "BezierArc".</param>
        /// <returns>True if perimeters do not need to be swapped.</returns>
        bool NoNeedToSwapPerimeters(string Name);

        /// <summary>
        /// Gets the applicable fillmode for a given shape name depending on vakue of stroke
        /// </summary>
        /// <param name="current">Current fill mode</param>
        /// <param name="Name">Name of the shape</param>
        /// <param name="stroke">Value of stroke</param>
        /// <returns>Compitible fill mode to render the shape</returns>
        FillMode GetFillMode(FillMode current, string Name, float stroke);
    }
    #endregion

    #region ISHAPE
    /// <summary>
    /// Represents an object which can rotate, offer perimeters(surround area represented by points in sequential order) and has bounds.
    /// And of course, must also implement IElement - the gateway interface.
    /// </summary>
    public interface IShape : IID, IRenderable, IRecognizable, IEnumerable<VectorF>
    { }
    #endregion

    #region IPOLYGON
    /// <summary>
    /// Represent an object which is a polygon i.e made of a collection of straight lines.
    /// In GWS, having a collection of points arranged in a sequential manner - one after another, 
    /// defines a polygon. Bezier is a curve, but GWS breakes it into the straight lines so for
    /// the drawing purpose it becomes polygon without having close ends i.e first point joins the last one.
    /// All the shapes which offer closed area are in fact have the first point joined with the last.
    /// GWS, does not break the curves except bezier i.e Ellipse, Circle, Pie, Arc in straight lines and 
    /// that is why there is a separate drawing routine for them.
    /// </summary>
    public interface IPolygon : IShape { }
    #endregion

    #region IBEZIER
    /// <summary>
    /// Represents an object which have properties of bezier curve.
    /// For drawing purpose, GWS breakes the curve in straight line segments.
    /// In GWS, a bezier can be drawn by offering minimum 3 points. 
    /// However there isn't any specific number of points required except minimum 3 to draw a curve.
    /// </summary>
    public interface IBezier : IShape
    {
        /// <summary>
        /// Specified which option is used to interpret the points for accumulating bezier points.
        /// We have two options : Cubic (taking a group of 4 points) or Multiple (4, 7, 10, 13 ... so on).
        /// Please not that if only three points are provided then its a Quadratic Bezier.
        /// </summary>
        BezierType Option { get; }
    }
    #endregion

    #region TRIANGLE
    /// <summary>
    /// Represent an object which has three points to offer.
    /// This object must have collection of three points.
    /// </summary>
    public interface ITriangle : IShape
    {
        /// <summary>
        /// 1st point of triangle.
        /// </summary>
        VectorF P1 { get; }
        /// <summary>
        /// 2nd point of triangle.
        /// </summary>
        VectorF P2 { get; }
        /// <summary>
        /// 3rd point of triangle.
        /// </summary>
        VectorF P3 { get; }

        /// <summary>
        /// A one among the several others center of this object.
        /// Center of a tringle is a tricky thing. We just picekd one way to calculate -
        /// i.e taking an average of x and y cordinates of all three points.
        /// Center = new PointF((a.X + b.X + c.X)/3f, (a.Y + b.Y + c.Y)/3f);
        /// Math is fun right!
        /// </summary>
        VectorF Centre { get; }

        float Area { get; }

        bool Contains(float x, float y);
        bool Contains(VectorF p);
    }
    #endregion

    #region ITETRAGON
    /// <summary>
    /// Represents a closed object (Quardilateral) which has four sides.
    /// This defination is in accordance with the British English and not the US one.
    /// </summary>
    public interface ITetragon : IShape
    {
        QuadType Type { get; }
    }

    /// <summary>
    /// Represents an object which has four sides just as IBox but has all corners rounded to a certain radius.
    /// </summary>
    public interface IRoundBox : ITetragon
    {
        /// <summary>
        /// Radius of a circle at all four corner.
        /// </summary>
        float CornerRadius { get; }
    }
    #endregion

    #region ICURVATURE
    public interface ICurvature
    {
        /// <summary>
        /// Start angle from where a curve start.
        /// </summary>
        float StartAngle { get; }

        /// <summary>
        /// End Angle where a curve stops.
        /// </summary>
        float EndAngle { get; }

        /// <summary>
        /// X co-ordinate of the center of this ellipse.
        /// </summary>
        int Cx { get; }

        /// <summary>
        /// Y co-ordinate of the center of this ellipse.
        /// </summary>
        int Cy { get; }

        /// <summary>
        /// X axis radius of this ellipse.
        /// </summary>
        float Rx { get; }

        /// <summary>
        /// Y axis radius of this ellipse.
        /// </summary>
        float Ry { get; }
    }
    #endregion

    #region ICONIC
    /// <summary>
    /// Defines conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0 where A, B, C, D, E and F are constants. 
    /// </summary>
    public interface IConic: ICurvature, IRotatable, IDrawable, IID
    {
        /// <summary>
        /// Gets original tilt angle of this object. 
        /// Only exists when this object is created throught points and not bounds.
        /// </summary>
        float TiltAngle { get; }
        
        /// <summary>
        /// Indicates if this object is a valid conic or not.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// Gets the maximum position from where scan lines must be requested to fill or draw the curve. 
        /// Value returned depends on whether operation is for drawing or filling. Loop is Boundary ----> 0.
        /// </summary>
        /// <param name="horizontal">Direction of scanning if true then vertically otherwise horizontally.</param>
        /// <param name="forDrawingOnly">If true, value returned is appropriate for drawing end pixels otherwise it is appropriated for filling operation.</param>
        /// <returns>Maximum position in curve.</returns>
        int GetBoundary(bool horizontal, bool forDrawingOnly = false);

        /// <summary>
        /// Gets two axis line positions for given position.
        /// If queried for boundary position, it gives minimum and maximum value of bounds.
        /// </summary>
        /// <param name="position">Position (in relation to YMax if horizontal otherwise XMax).</param>
        /// <param name="horizontal">If true horizontal scan of curve is done oterwise vertical scan is performed.</param>
        /// <param name="axis1">First axis line.</param>
        /// <param name="axis2">Second axis line.</param>
        void GetAxis(int position, bool horizontal, out int axis1, out int axis2);

        /// <summary>
        /// Get a collections of length 2 which is of scan line fragments for a given position (in relation to YMax if horizontal otherwise XMax).
        /// </summary>
        /// <param name="position">Position (in relation to YMax if horizontal otherwise XMax).</param>
        /// <param name="horizontal">If true horizontal scan of curve is done oterwise vertical scan is performed.</param>
        /// <param name="forDrawingOnly">If true only end points are obtained.</param>
        /// <param name="axis1">First value for Y if horizontal otherwise X.</param>
        /// <param name="axis2">Second value for Y if horizontal otherwise X.</param>
        /// <returns></returns>
        IList<float>[] GetDataAt(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2);

        /// <summary>
        /// Solves standard equation for this conic and returns four quardrants and two axis values.
        /// i.e. 2 axis lines either horizontal or vertical.
        /// </summary>
        /// <param name="position">Poistion to query data for. Use GetBoundary function to get max position and iterate from that in descending order by 1 step at a time.
        /// For example, var max = GetBoundary(true, false); for i=max; i>=0; i-=1 { Solvequation (i.....) } </param>
        /// <param name="horizontal">If true, a pair of two horizontal axis lines is returned otherwise vertical.</param>
        /// <param name="forDrawingOnly">If true, only one point from each line is returned.
        /// While drawing curve like ellipse, top - bottom and left - right halves are processed and so it is necessary to avoid overlapped pixels.</param>
        /// <param name="v1">Value of first quardrant.</param>
        /// <param name="v2">Value of second quardrant.
        /// Will be Nan if fordrawingOnly is true.</param>
        /// <param name="v3">Value of third quardrant.</param>
        /// <param name="v4">Value of fourth quardrant.
        /// Will be Nan if fordrawingOnly is true.</param>
        /// <param name="a1">Value of first axis</param>
        /// <param name="a2">Value of second axis</param>
        /// <returns></returns>
        bool SolveEquation(int position, bool horizontal, bool forDrawingOnly, out float v1, out float v2, out float v3, out float v4, out int a1, out int a2);
    }
    #endregion

    #region ICURVE
    /// <summary>
    /// Represents an object which has a  one continious segment of curved line which does not form a closed loop, 
    /// where the sum of the distances from two points (foci) to every point on the line is constant.
    /// </summary>
    public interface ICurve: ICut, IConic
    {
        /// <summary>
        /// Indicates if this object is in-fact forms a closed loop i.e an ellipse.
        /// This property indicates true if no start and end angle is provided i.e both of them are zero or one of them is 0 and othe one is 360 degree.
        /// </summary>
        bool Full { get; }

        /// <summary>
        /// Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it.
        /// </summary>
        CurveType Type { get; }

        /// <summary>
        /// Indicates a straight line between start and end points of this curve.
        /// </summary>
        ILine ArcLine { get; }

        /// <summary>
        /// Gets a collection of three points in the following order:
        /// a start point of curve,
        /// center point of the curve,
        /// an end point of the curve.
        /// </summary>
        VectorF[] PieTriangle { get; }

        /// <summary>
        /// Indicates if any child curve is attached to this object or not.
        /// GWS uses this property to apply strokes to the original curve.
        /// </summary>
        ICurve AttachedCurve { get; set; }
               
        /// <summary>
        /// Gets a collection of lines necessary to draw closing cut for this curve.
        /// </summary>
        /// <returns>
        /// If this curve is an arc and is is not closed then ..
        /// If it has no stroke then no lines returned.
        /// If it has stroke than two lines each obtained from joining correspoinding start and end points of inner and outer curve will be returned.
        /// If this curve is a pie then...
        /// If it has no stroke then two pie lines i.e one from start point to the center and another from end point to the center will be returned.
        /// If it has stroke than four pie lines from each curves  consists of inner and outer curve will be returned.
        /// </returns>
        IList<ILine> GetClosingLines();
    }
    #endregion

    #region ILINE
    /// <summary>
    /// Represents an object which defines a line segment and its properties.
    /// </summary>
    public interface ILine : IShape, IDrawable
    {
        /// <summary>
        /// X cordinate of start point.
        /// </summary>
        float X1 { get; }
        
        /// <summary>
        /// Y cordinate of start point.
        /// </summary>
        float Y1 { get; }
        
        /// <summary>
        /// X cordinate of end point.
        /// </summary>
        float X2 { get; }
        
        /// <summary>
        /// Y cordinate of end point.
        /// </summary>
        float Y2 { get; }

        /// <summary>
        /// Value of M as in y = M * x + C.
        /// </summary>
        float M { get; }

        /// <summary>
        /// Value of C as in y = M * x + C.
        /// </summary>
        float C { get; }

        /// <summary>
        /// Indicates if the segment is valid i.e has X1, X2, Y1, Y2 all are finite real numbers.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// Gets the type of this line i.e. horizontal or vertical or a point.
        /// </summary>
        LineType Type { get; }
    }
    #endregion

    #region ICUT
    /// <summary>
    /// Represents an object which has a capability to apply a cut to any axial line in order to fragement and omit an unwanted portion.
    /// </summary>
    public interface ICut 
    {
        /// <summary>
        /// Indicates if this object can not cut anything.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the currently associated function to check if certain point is ok with in the context of this object.
        /// </summary>    
        /// <param name="val">Position of reading on axis i.e X coordinate if horizontal otherwise Y</param>
        /// <param name="axis">Position of reading on axis i.e Y coordinate if horizontal otherwise X</param>
        /// <param name="horizontal">Direction of reading: if true horizontally otherwise vertically</param>
        /// <returns>True if conditional logic validates to true otherwise false.</returns>
        bool CheckPixel(float val, int axis, bool horizontal);

        /// <summary>
        /// Performs ray tracing on cutlines and adds result values to the list.
        /// </summary>
        /// <param name="axis">Position of reading on axis i.e Y coordinate if horizontal otherwise X</param>
        /// <param name="horizontal">Direction of reading: if true horizontally otherwise vertically</param>
        /// <param name="list"></param>
        void AddValsSafe(int axis, bool horizontal, ICollection<float> list);
    }
    #endregion

    #region ITEXT
    /// <summary>
    /// Represents an object which represents a text string in a drawing context.
    /// This also has a colleciton of glyphs.
    /// </summary>
    public interface IText : IGlyphs, IID
    {
        /// <summary>
        /// Gets or sets current font of this object.
        /// </summary>
        IFont Font { get; set; }

        /// <summary>
        /// Gets or sets current text value of this object.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets the draw style attached to this object.
        /// </summary>
        TextDrawStyle DrawStyle { get; }
        
        /// <summary>
        /// Indicates if any of the vital parameters such as font or text is changed in this object or not.
        /// </summary>
        bool Changed { get; }

        /// <summary>
        /// Measures current text string and accumulates all the glyphs in an internal collection.
        /// </summary>
        /// <returns></returns>
        IText Measure();

        /// <summary>
        /// Measures current text string with a goven draw style.
        /// If no draws tyle provided or is null then the current draw style will be used.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        RectangleF MeasureText(TextDrawStyle style = null);

        /// <summary>
        /// Gets the kerning information available with font object for a character at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetKerning(int index);

        /// <summary>
        /// Chnages internal draw style object with new draw style.
        /// </summary>
        /// <param name="drawStyle"></param>
        /// <param name="temporary"></param>
        void ChangeDrawStyle(TextDrawStyle newDrawStyle, bool temporary = true);

        /// <summary>
        /// Restores the current draw style to revious default .
        /// </summary>
        void RestoreDrawStyle();

        void SetDrawXY(int? drawX = null, int? drawY = null);
    }
    #endregion

    #region ISEARCH
    public interface ISearch : IRenderable, IBackground, IForeground
    { }
    #endregion

#if Advanced
    #region IFOCUSRECT
    public interface IFocusRect 
    {
        void Show(int x, int y, int w, int h);
        void Hide();
    }
    #endregion
#endif

#endif
}
