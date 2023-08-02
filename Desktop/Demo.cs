/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if MS && (GWS || Window)
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Linq;
using System.Windows.Forms;

namespace MnM.GWS.MS
{
    public partial class Demo : System.Windows.Forms.Form, IShowable, IHideable
    {
        #region VARIABLES
        string manualFontPath;
        internal float x, y, w, h;
        float startA, endA;
        int[] drawPoints;
        Command pse;
        CurveType curveType = 0;
        IFont gwsFont;
        ITextureBrush textureBrush;
        static Demo instance;
        static IDesktop Window;
        System.Drawing.Font MsFont;
        System.Drawing.Font dfMsFont = new System.Drawing.Font("Tahoma", 12);
        System.Drawing.Brush MsBrush;
        bool Original;
        //IEnumerable<IParameter> Parameters;
        IBoundary Boundary = Factory.newBoundary();
        ITypedBounds UpdateRect = new UpdateArea();
        Command bresenham =0 ;
        #endregion

        #region CONSTRUCTORS
        Demo()
        {
            InitializeComponent();
            MsFont = dfMsFont;
            Window = Factory.newDesktop();
        }
        #endregion

        #region PROPERTIES
        public static Demo Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Demo();
                }
                return instance;
            }
        }

        public VoidMethod GwsMethod { get; private set; }
        public VoidMethod MsMethod { get; private set; }
        bool Destroyed => Window == null || Window.IsDisposed;
        #endregion

        #region LOAD
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            var shapes = typeof(ShapeTypes).ListConstansts<uint>().Select(x => (object)x.Name).ToArray();
            cmbShape.Items.AddRange(shapes);

            cmbStrokeMode.Items.Add(Command.Default);
            cmbStrokeMode.Items.Add(Command.StrokeOuter);
            cmbStrokeMode.Items.Add(Command.StrokeInner);

            var brushTypes = typeof(BrushType).ListConstansts<sbyte>().Select(x => (object)x).ToArray();
            cmbGradient.Items.AddRange(brushTypes);

            var copts = Enum.GetValues(typeof(CurveType));
            foreach (var item in copts)
            {
                chkLstCurveOption.Items.Add(item);
            }

            var zrotdir = Enum.GetValues(typeof(SkewType));
            foreach (var item in zrotdir)
            {
                cmbZRotateDirection.Items.Add(item);
            }
            var rbo = Enum.GetValues(typeof(RoundBoxOption));
            foreach (var item in rbo)
            {
                chkRoundBoxOption.Items.Add(item);
            }
            chkRoundBoxOption.SelectedIndex = 0;

            var linePatterns =
                new object[] { 
                    Command.Bresenham,
                    Command.DottedLine,
                    Command.DashedLine,
                    Command.DashDotDashLine
                };

            chkLstLinePattern.Items.AddRange(linePatterns);
            chkLstLinePattern.SelectedIndex = 0;

            cmbBezier.Items.Add(new KeyValuePair<string, BezierType>("Cubic", BezierType.Cubic));
            cmbBezier.Items.Add(new KeyValuePair<string, BezierType>("Quadratic", BezierType.Quadratric));
            cmbBezier.Items.Add(new KeyValuePair<string, BezierType>("Multiple", BezierType.Multiple));


            cmbStroke.Items.Add(new KeyValuePair<string, Command>("Default", Command.Default));
            cmbStroke.Items.Add(new KeyValuePair<string, Command>("Normal", Command.OriginalFill));
            cmbStroke.Items.Add(new KeyValuePair<string, Command>("Outer", Command.StrokeOuter));
            cmbStroke.Items.Add(new KeyValuePair<string, Command>("Inner", Command.StrokeInner));
            cmbStroke.Items.Add(new KeyValuePair<string, Command>("DrawOutLine", Command.DrawOutLines));
            cmbStroke.Items.Add(new KeyValuePair<string, Command>("ExceptOutLine", Command.FillOddLines));

            cmbZRotateDirection.SelectedIndex = 0;
            cmbStrokeMode.SelectedIndex = 0;

            cmbBezier.DisplayMember = "Key";
            cmbBezier.SelectedIndex = 0;

            cmbStroke.DisplayMember = "Key";
            cmbStroke.SelectedIndex = 0;

            cmbGradient.SelectedIndex = 1;
            cmbShape.SelectedIndex = 0;


            lstColors.Items.Add(System.Drawing.Color.Red.ToArgb());
            lstColors.Items.Add(System.Drawing.Color.Green.ToArgb());
            lstColors.Items.Add(System.Drawing.Color.DodgerBlue.ToArgb());
            lstColors.Items.Add(System.Drawing.Color.DarkViolet.ToArgb());


            var Colors = Enum.GetNames(typeof(System.Drawing.KnownColor)).Select(x => (object)System.Drawing.Color.FromName(x).ToArgb()).Reverse().ToArray();
            lstKnownColors.Items.AddRange(Colors);
            lstKnownColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            lstColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

            BindEvents();
        }
        #endregion

        #region DRAW
        private void Draw(object sender, System.EventArgs e)
        {
            if (Destroyed)
                BindGwsDisplay();

            var description = cmbShape.Text;

            SetDrawingParams();
            SetGwsMethod();

            if (Original)
                description = "Entire Image Rotation ";

            if (chkCompare.Checked)
            {
                MsDisplay.Screen.Refresh(cmbShape.Text + "", SetAngle());
                MsMethod = null;
            }
            Window.Clear(UpdateRect, (Command.SkipDisplayUpdate| Command.Screen).Add());

            if (GwsMethod != null)
            {
                VoidMethod method = () => Text = description + 
                Benchmarks.Execute(GwsMethod, description, MSBridge.BUnit);
                method.Invoke();
                var rc = Boundary.Convert();
                var hybrid = rc.Hybrid(UpdateRect);
                Window.Update(hybrid, UpdateCommand.UpdateScreenOnly);
                UpdateRect = rc;
            }
            if (Original)
            {
                Window.Update(new Rectangle(0, 0, Window.Width, Window.Height), UpdateCommand.UpdateScreenOnly);
                Original = false;
            }
        }
        #endregion

        #region CHANGE SHAPE
        private void ChangeShape(object sender, System.EventArgs e)
        {
            //txtPts.Clear();
            Window.Clear(new Area(0, 0, Window.Width, Window.Height, Purpose.Erase));
            pnlArcS.Visible = false;
            pnlArcE.Visible = false;
            pnlBezier.Visible = false;
            grpXY.Visible = true;
            grpFonts.Visible = false;
            grpXY.Visible = false;
            pnlTrapezium.Visible = false;
            //grpPts.Visible = false;
            grpArcPie.Visible = false;
            grpXY.Text = "Enter " + cmbShape.Text + " paramters";
            var shape = cmbShape.SelectedItem + "";
            btnOpenFont.Visible = false;
            pnlRoundRC.Visible = false;
            grpPts.Text = "Click Gws Drawing Area to select point";
            switch (shape)
            {
                case "Trapezium":
                    pnlTrapezium.Visible = true;
                    pnlTrapezium.BringToFront();
                    grpXY.Visible = true;
                    pnlXy.Visible = false;
                    break;
                case "Circle":
                case "Ellipse":
                    grpXY.Visible = true;
                    pnlXy.Visible = true;
                    grpArcPie.Visible = true;
                    lblArcs.Text = "Angle";
                    if (cmbShape.Text == "Circle")
                        numH.Visible = false;
                    else
                        numH.Visible = true;
                    lblH.Visible = numH.Visible;
                    break;
                case "Square":
                case "Rectangle":
                case "Rhombus":
                case "Capsule":
                case "RoundedArea":
                    grpXY.Visible = true;
                    grpPts.Visible = false;
                    pnlXy.Visible = true;
                    if (cmbShape.Text == "Sqaure")
                        numH.Visible = false;
                    else
                        numH.Visible = true;
                    if (cmbShape.Text == "RoundedArea" || cmbShape.Text == "Capsule")
                    {
                        pnlRoundRC.Visible = true;
                    }
                    else if (cmbShape.Text == "Rhombus")
                    {
                        grpXY.Visible = true;
                        pnlXy.Visible = true;
                        grpPts.Visible = true;
                    }
                    break;
                case "Arc":
                case "Pie":
                case "Curve":
                    grpArcPie.Visible = true;
                    grpPts.Visible = true;
                    grpXY.Visible = true;
                    pnlXy.Visible = true;
                    //grpPts.Visible = false;
                    numH.Visible = true;
                    if (cmbShape.Text == "Arc")
                    {
                        lblArcs.Text = "Arc Start";
                        lblArcs.Text = "Arc End";
                    }
                    if (cmbShape.Text == "Pie")
                    {
                        lblArcs.Text = "Pie Start";
                        lblArcs.Text = "Pie End";
                    }
                    if (cmbShape.Text == "Curve")
                    {
                        lblArcs.Text = "Curve Start";
                        lblArcs.Text = "Curve End";
                    }
                    pnlArcS.Visible = true;
                    pnlArcE.Visible = true;

                    lblH.Visible = numH.Visible;
                    int i = -1;
                    int j = -1;
                    i = chkLstCurveOption.Items.IndexOf(CurveType.Arc);
                    j = chkLstCurveOption.Items.IndexOf(CurveType.Pie);
                    if (cmbShape.Text == "Arc")
                    {
                        if (i != -1)
                            chkLstCurveOption.SetItemChecked(i, true);
                        if (j != -1)
                            chkLstCurveOption.SetItemChecked(j, false);

                    }
                    else
                    {
                        if (i != -1)
                            chkLstCurveOption.SetItemChecked(i, false);
                        if (j != -1)
                            chkLstCurveOption.SetItemChecked(j, true);
                    }
                    break;
                case "Triangle":
                case "Polygon":
                case "Bezier":
                case "Line":
                    pnlArcS.Visible = false;
                    pnlArcE.Visible = false;

                    grpXY.Visible = false;
                    grpPts.Visible = true;
                    if (shape == "Bezier")
                        pnlBezier.Visible = true;
                    txtPts.Clear();
                    break;
                case "Text":
                    grpPts.Visible = true;
                    grpPts.Text = "Type text in here and click Font button to select a font file.";
                    grpXY.Visible = false;
                    grpFonts.Visible = true;
                    grpFonts.BringToFront();
                    btnOpenFont.Visible = true;
                    break;
                default:

                    break;
            }
        }
        #endregion

        #region BIND EVENTS
        void BindEvents()
        {
            numScale.ValueChanged += Draw;
            numArcS.ValueChanged += Draw;
            numArcE.ValueChanged += Draw;
            numX.ValueChanged += Draw;
            numY.ValueChanged += Draw;
            numW.ValueChanged += Draw;
            numH.ValueChanged += Draw;
            numRotate.ValueChanged += Draw;
            numStroke.ValueChanged += Draw;
            numSize.ValueChanged += Draw;
            numSizeDiff.ValueChanged += Draw;
            numCornerRadius.ValueChanged += Draw;
            numSkewRotate.ValueChanged += Draw;
            numFontSize.ValueChanged += SetFont;

            cmbShape.SelectedIndexChanged += ChangeShape;
            lstKnownColors.SelectedIndexChanged += KnownColorIndexChanged;
            cmbGradient.SelectedIndexChanged += Draw;
            cmbStroke.SelectedIndexChanged += Draw;
            cmbStrokeMode.SelectedIndexChanged += Draw;
            chkLstLinePattern.SelectedIndexChanged += Draw;
            cmbZRotateDirection.SelectedIndexChanged += Draw;

            chkLstCurveOption.ItemCheck += CurveOptionCheck;
            chkLstLinePattern.ItemCheck += LineOptionCheck;
            chkRoundBoxOption.Click += Draw;

            chkCenter.Click += ShapeCenterCheck;
            chkCompare.CheckedChanged += ShowCompareForm;
            chkFloodFill.CheckedChanged += Draw;
            chkImageOperation.CheckedChanged += (s, e) =>
              {
                  if (!chkImageOperation.Checked)
                      Original = false;
              };
            btnTextureBrush.Click += SelectTextureBrush;
            btnClear.Click += ClearGraphics;
            btnPlot.Click += PlotPoints;
            btnSave.Click += SaveGraphics;
            btnOpenFont.Click += SelectFont;
            btnDraw.Click += Draw;

            lstKnownColors.DrawItem += KnownColorsItemDraw;
            lstColors.DrawItem += KnownColorsItemDraw;
            lstColors.DoubleClick += ColorListDoubleClick;
            BindGwsDisplay();
            FormClosed += Close;
        }

        private void Demo_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        void BindGwsDisplay()
        {
            Window.MouseDown += Window_MouseUp;
            Window.MouseMove += Window_MouseMove;
        }

        private void Window_MouseMove(object sender, IMouseEventArgs e)
        {
            ShowCoordinates(this, e);
        }

        private void Window_MouseUp(object sender, IMouseEventArgs e)
        {
            DrawCoordinates(this, e);
        }

        private void SelectFont(object sender, System.EventArgs e)
        {
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                manualFontPath = openFileDialog1.FileName;
                SetFont(this, e);
            }
        }
        private void SetFont(object sender, System.EventArgs e)
        {

            if (manualFontPath != null)
            {
                gwsFont = Factory.newFont(manualFontPath, (int)numFontSize.Value);
                if (gwsFont == null)
                    System.Windows.Forms.MessageBox.Show("This font can not be read!");
            }
            else
            {
                gwsFont = Factory.SystemFont;

            }
            if (gwsFont != null)
            {
                gwsFont.Size = (int)numFontSize.Value;
                try
                {
                    MsFont = new System.Drawing.Font(gwsFont.Info.FullName, (float)numFontSize.Value);
                }
                catch
                {
                    MsFont = dfMsFont;
                }
            }
            Draw(sender, e);
        }

        private void ShapeCenterCheck(object sender, System.EventArgs e)
        {
            if (chkCenter.Checked)
            {
                txtShapeCenter.Clear();
                txtShapeCenter.Visible = false;
                chkCenter.Text = "On Center";
            }
            else
            {
                txtShapeCenter.Visible = true;
                chkCenter.Text = "Center XY: ";
            }
        }
        private void ColorListDoubleClick(object sender, System.EventArgs e)
        {
            lstColors.Items.Remove(lstColors.SelectedItem);

        }
        private void KnownColorIndexChanged(object sender, System.EventArgs e)
        {
            if (lstKnownColors.SelectedIndex != -1)
                lstColors.Items.Add(lstKnownColors.SelectedItem);
        }
        private void KnownColorsItemDraw(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            var c = Color.FromArgb((int)(sender as ListBox).Items[e.Index]);
            var b = new SolidBrush(c);
            e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            //e.Graphics.DrawString(c.Name, lstKnownColors.Font, b, 0, 0);
        }
        private void Close(object sender, FormClosedEventArgs e)
        {
            Application.Quit();
        }

        private void DrawCoordinates(object sender, IMouseEventArgs e)
        {
            txtPts.Text += "," + e.X + "," + e.Y;
            var x = e.X;
            var y = e.Y;
            var boundary = Factory.newBoundary();
            Window.WritePixels(new float[] { x, y, x, y + 1 },
                (Command.Screen| Command.SkipDisplayUpdate).Add(), 2.ToThickness(), Rgba.Red, boundary);
            Window.Update(boundary, UpdateCommand.UpdateScreenOnly);
        }
        private void PlotPoints(object sender, System.EventArgs e)
        {
            if (txtPts.Text.Length == 0)
                return;

            if (txtPts.Text.Substring(0, 1) == ",")
                txtPts.Text = txtPts.Text.Substring(1);
            var points = txtPts.Text.Split(',').Select(p => Convert.ToInt32(p)).ToArray();
            var boundary = (IExBoundary) Factory.newBoundary();
            var temp = (IExBoundary)Factory.newBoundary();
            var parameters = new IParameter[]
            { (Command.Screen| Command.SkipDisplayUpdate).Add(), 2.ToThickness(), Rgba.Red, temp };
            Window.WriteLines(points, parameters);
            boundary.Update(temp);
            //var pts = Vectors.ToPointsF(points);
            //pts.Add(pts[0]);

            int j = 0;
            for (int i = 1; i < points.Length; i += 2)
            {
                Window.DrawText(Factory.SystemFont, "" + (j++),  points[i - 1] + 6, points[i], parameters);
                boundary.Update(temp);
            }
            Window.Update(boundary, UpdateCommand.UpdateScreenOnly);
        }
        private void ClearGraphics(object sender, System.EventArgs e)
        {
            GwsMethod = null;
            txtPts.Clear();
            Window.Clear();
        }
        private void SaveGraphics(object sender, System.EventArgs e)
        {
            var result = saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK && saveFileDialog1.FileName != null)
            {
                var canvas = Window.TakeScreenShot(new IParameter[] {(Command.Screen| Command.SwapRedBlueChannel).Replace() });
                canvas.SaveAs(saveFileDialog1.FileName, ImageFormat.BMP.ToParameter());
            }
        }
        private void CurveOptionCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (e.Index == -1)
                return;

            if (e.NewValue == CheckState.Unchecked)
            {
                curveType &= ~(CurveType)chkLstCurveOption.Items[e.Index];
                Draw(this, e);
                return;
            }
            if (e.NewValue == CheckState.Checked)
            {
                curveType |= (CurveType)chkLstCurveOption.Items[e.Index];
            }
            else
            {
                curveType &= ~(CurveType)chkLstCurveOption.Items[e.Index];
            }
            Draw(this, e);
        }
        private void LineOptionCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (e.Index == -1)
                return;
            Draw(this, e);
        }

        private void SelectTextureBrush(object sender, System.EventArgs e)
        {
            if (textureBrush != null)
                goto mks;

            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textureBrush = Factory.newBrush(openFileDialog1.FileName);
                btnTextureBrush.Text = "Remove Texture Brush";
                return;
            }

        mks:
            btnTextureBrush.Text = "Select Texture Brush";
            textureBrush?.Dispose();
            textureBrush = null;
            Draw(sender, e);
        }
        private void ShowCoordinates(object sender, IMouseEventArgs e)
        {
            Text = e.X + "," + e.Y + ",";
        }
        private void ShowCompareForm(object sender, System.EventArgs e)
        {
            if (chkCompare.Checked)
                MsDisplay.Screen.Show();
            else
                MsDisplay.Screen.Dispose();
        }
        #endregion

        #region SETTING DRAWING PARAMS AND APPROPRIATE METHOD
        unsafe void SetGwsMethod()
        {
            var Canvas = Window;
            //Boundary.Clear();

            IEnumerable<IParameter> parameters = new IParameter[] 
            { Command.Screen.Add(), SetAngle(), Boundary };
            if (chkImageOperation.Checked)
            {
                if ((numRotate.Value != 0 && numRotate.Value != 360 && numRotate.Value != 360) || numScale.Value != 0)
                {
                    var sz = Canvas.RotateAndScale(out IntPtr data,
                        new Rotation((float)numRotate.Value), new Scale((float)numScale.Value));
                    GwsMethod = () => Canvas.DrawImage(Factory.newCanvas(data, sz.Width, sz.Height),
                        0, 0, 0, UpdateCommand.UpdateScreenOnly);
                    Original = true;
                    return;
                }
            }

            if (textureBrush == null)
            {
                sbyte grad;
                if (cmbGradient.SelectedIndex == -1)
                    grad = BrushType.Horizontal;
                else
                    grad = ((NamedValue<sbyte>)cmbGradient.SelectedItem).Value;
                BrushStyle fStyle;
                if (lstColors.Items.Count > 0)
                {
                    object[] all = new object[lstColors.Items.Count];
                    lstColors.Items.CopyTo(all, 0);
                    fStyle = new BrushStyle(grad, all.Select(x => (int)x).ToArray());
                }
                else
                {
                    fStyle = new BrushStyle(grad, Rgba.Silver);
                }
                parameters = parameters.AppendItem(fStyle);
            }
            else
                parameters = parameters.AppendItem(textureBrush);

            var polyFill = Command.Default;

            var command = Command.Default;
            bresenham = 0;
            if (chkInvertBrush.Checked)
                command |= Command.BrushInvertRotation;
            if (chkFloodFill.Checked)
                polyFill |= Command.FloodFill;

            for (int i = 0; i < chkLstLinePattern.Items.Count; i++)
            {
                if (chkLstLinePattern.GetItemChecked(i))
                {
                    bresenham |= ((Command)chkLstLinePattern.Items[i]);
                }
                else
                {
                    bresenham &= ~((Command)chkLstLinePattern.Items[i]);
                }
            }
            parameters = parameters.AppendItems
            (
                (command |pse|(Command)cmbStrokeMode.SelectedItem).Add(),
                new Scale( ((float)numScale.Value / 10f)), 
                ((float)numStroke.Value).ToStroke()
            );

            RoundBoxOption rbo = 0;
            if (cmbShape.Text == "RoundedArea")
            {
                foreach (var item in chkRoundBoxOption.CheckedItems)
                {
                    rbo |= (RoundBoxOption)item;
                }
            }

            GwsMethod = null;

            var font = gwsFont ?? Factory.SystemFont;
            switch (cmbShape.Text)
            {
                case "Line":
                    if (drawPoints == null || drawPoints.Length < 4)
                    {
                        System.Windows.Forms.MessageBox.Show("Please provide 2 coordinates to create a line by clicking 2 points on GWS picture box!");
                        return;
                    }
                    GwsMethod = () => Canvas.DrawLines(parameters, true, drawPoints);
                    break;
                case "Trapezium":
                    if (drawPoints == null || drawPoints.Length < 4)
                    {
                        System.Windows.Forms.MessageBox.Show("Please provide 2 coordinates to create a line by clicking 2 points on GWS picture box!");
                        return;
                    }
                    GwsMethod = () => Canvas.DrawTrapezium(
                        new float[] {drawPoints[0], drawPoints[1], drawPoints[2], drawPoints[3],
                                (float) numSize.Value, (float)numSizeDiff.Value}, parameters);
                    break;

                case "Circle":
                    if (drawPoints?.Length >= 4)
                    {
                        GwsMethod = () => Canvas.DrawCircle(new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]), parameters);
                    }
                    else
                        GwsMethod = () => Canvas.DrawEllipse(parameters, x, y, w, w);

                    break;
                case "Ellipse":

                    if (drawPoints?.Length >= 10)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            new VectorF(drawPoints[8], drawPoints[9]), curveType);
                    }
                    else if (drawPoints?.Length >= 8)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            curveType);
                    }
                    else if (drawPoints?.Length >= 6)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            curveType);
                    }

                    else
                        GwsMethod = () => Canvas.DrawEllipse(parameters, x, y, w, h);
                    break;
                case "Arc":
                    if (drawPoints?.Length >= 10)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            new VectorF(drawPoints[8], drawPoints[9]),
                            curveType);
                    }
                    else if (drawPoints?.Length >= 8)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            curveType);
                    }

                    else if (drawPoints?.Length >= 6)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            curveType);
                    }
                    else
                        GwsMethod = () => Canvas.DrawArc(parameters, x, y, w, h, startA, endA);

                    break;
                case "Pie":
                    if (drawPoints?.Length >= 10)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            new VectorF(drawPoints[8], drawPoints[9]),
                            curveType);
                    }
                    else if (drawPoints?.Length >= 8)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            curveType);
                    }

                    else if (drawPoints?.Length >= 6)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            curveType);
                    }
                    else
                        GwsMethod = () => Canvas.DrawPie(parameters, x, y, w, h, startA, endA);

                    break;
                case "Curve":
                    if (drawPoints?.Length >= 10)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            new VectorF(drawPoints[8], drawPoints[9]),
                            curveType);
                    }
                    else if (drawPoints?.Length >= 8)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            new VectorF(drawPoints[6], drawPoints[7]),
                            curveType);
                    }

                    else if (drawPoints?.Length >= 6)
                    {
                        GwsMethod = () => Canvas.DrawCurve(parameters,
                            new VectorF(drawPoints[0], drawPoints[1]),
                            new VectorF(drawPoints[2], drawPoints[3]),
                            new VectorF(drawPoints[4], drawPoints[5]),
                            curveType);
                    }
                    else
                        GwsMethod = () => Canvas.DrawCurve(parameters, x, y, w, h, startA, endA, curveType);
                    break;
                case "Square":
                    GwsMethod = () => Canvas.DrawRectangle(x, y, w, w, parameters);
                    break;
                case "Rectangle":
                    GwsMethod = () => Canvas.DrawRectangle(x, y, w, h, parameters);
                    break;

                case "RoundedArea":
                    GwsMethod = () => Canvas.DrawRoundedBox(x, y, w, h,
                        (float)numCornerRadius.Value, rbo, parameters);
                    break;
                case "Capsule":
                    GwsMethod = () => Canvas.DrawCapsule(x, y, w, h,
                        (float)numCornerRadius.Value, parameters);
                    break;
                case "Rhombus":
                    if (drawPoints != null && drawPoints.Length >= 6)
                    {
                        GwsMethod = () => Canvas.DrawRhombus(
                            drawPoints[0], drawPoints[1],
                            drawPoints[2], drawPoints[3],
                            drawPoints[4], drawPoints[5],
                            parameters.ToArray());
                        break;
                    }
                    GwsMethod = () => Canvas.DrawRectangle(x, y, w, h, parameters);

                    break;
                case "Triangle":
                    if (drawPoints == null || drawPoints.Length < 6)
                    {
                        System.Windows.Forms.MessageBox.Show("Please provide 3 coordinates to create a triangle by clicking 3 points on GWS picture box!");
                        return;
                    }
                    GwsMethod = () => Canvas.DrawTriangle(drawPoints[0], drawPoints[1],
                        drawPoints[2], drawPoints[3], drawPoints[4], drawPoints[5], parameters);
                    break;

                case "Polygon":
                    if (drawPoints == null)
                        return;
                    GwsMethod = () => Canvas.DrawPolygon(drawPoints.Select(p => p + 0f), parameters);
                    break;
                case "Bezier":
                    var type = cmbBezier.SelectedIndex != -1 ?
                        ((KeyValuePair<string, BezierType>)cmbBezier.SelectedItem).Value : BezierType.Cubic;
                    if ((type & BezierType.Multiple) == BezierType.Multiple)
                    {
                        if (drawPoints == null || drawPoints.Length < 6)
                            return;
                        GwsMethod = () => Canvas.DrawBezier(type, drawPoints.Select(p => (float)p), parameters);
                    }
                    else
                    {
                        if (drawPoints == null || drawPoints.Length < 3)
                        {
                            System.Windows.Forms.MessageBox.Show("Please provide at least 3 coordinates to create a bezier by clicking 3 points on GWS picture box!");
                            return;
                        }
                        GwsMethod = () => Canvas.DrawBezier(type, drawPoints.Select(p => (float)p), parameters);
                    }
                    break;
                case "Text":
                    int _x = 0, _y = 0;
                    if (!string.IsNullOrEmpty(txtGlyphXY.Text))
                    {

                        var items = txtGlyphXY.Text.Split(',');
                        if (items.Length > 0 && int.TryParse(items[0], out int x1))
                            _x = x1;
                        if (items.Length > 1 && int.TryParse(items[1], out int y1))
                            _y = y1;
                    }
                    GwsMethod = () => Canvas.DrawText(parameters, font, txtPts.Text, _x, _y);
                    break;
            }
        }
        void SetDrawingParams()
        {
            drawPoints = null;
            //mnmCanvas.AntiAlias = chkAA.Checked;
            pse = cmbStroke.SelectedIndex != -1 ?
                ((KeyValuePair<string, Command>)cmbStroke.SelectedItem).Value : Command.Default;

            //get the user input values for width, height, x & y coordinates.
            x = (int)numX.Value;
            y = (int)numY.Value;
            w = (int)numW.Value;
            h = (int)numH.Value;
            var shape = cmbShape.SelectedItem + "";

            if (shape == "Square" || shape == "Circle")
                h = w;

            //get the start and sweep angle for arc/pie drawing
            startA = (float)numArcS.Value;
            endA = (float)numArcE.Value;

            var Rotation = SetAngle();

            //get the GWS gradient fill mode 

            //for Triangle, Bezier, Polygon etc. - get the points selected by the user
            RectangleF rc;
            if (shape == "Text")
            {
                return;
            }
            if (txtPts.Text.Length > 1)
            {
                if (txtPts.Text.Substring(0, 1) == ",")
                    txtPts.Text = txtPts.Text.Substring(1);
                drawPoints = txtPts.Text.Split(',').Select(p => Convert.ToInt32(p)).ToArray();
                switch (shape)
                {
                    case "Arc":
                    case "Pie":
                    case "BezierArc":
                    case "BezierPie":
                    case "Ellipse":
                        if (drawPoints.Length < 6)
                            goto exit;
                        goto mks;
                    case "Line":
                    case "Trapezium":
                        if (drawPoints.Length < 4)
                            goto exit;
                        rc = Vectors.ToVectorF(drawPoints).ToArea();
                        x = rc.X;
                        y = rc.Y;
                        w = rc.Width;
                        h = rc.Height;
                        goto mks;
                    case "Sqaure":
                    case "Rectangle":
                    case "Rhombus":

                        if (drawPoints.Length < 2)
                            goto exit;
                        var wd = Math.Abs(drawPoints[0] - drawPoints[2]);
                        var hd = Math.Abs(drawPoints[1] - drawPoints[2]);
                        if (drawPoints[0] < drawPoints[2])
                        {
                            x = drawPoints[0];
                            y = drawPoints[1];
                        }
                        else
                        {
                            x = drawPoints[2];
                            y = drawPoints[3];
                        }
                        if (wd < 3 || hd < 3)
                        {
                            var dist = Math.Sqrt(Numbers.Sqr(drawPoints[0] - drawPoints[2]) + Numbers.Sqr(drawPoints[1] - drawPoints[3]));
                            w = dist.Round();
                            h = dist.Round();
                        }
                        else
                        {
                            w = Math.Abs(drawPoints[2] - drawPoints[0]);
                            h = Math.Abs(drawPoints[3] - drawPoints[1]);
                        }
                        goto mks;
                    case "Triangle":
                    case "Polygon":
                    case "Bezier":
                        if (drawPoints.Length < 3)
                            goto exit;
                        rc = Vectors.ToVectorF(drawPoints).ToArea();
                        w = rc.Width;
                        h = rc.Height;

                        goto mks;
                }
            exit:
                txtPts.Clear();
            mks:
                return;
            }
        }

        IRotation SetAngle()
        {
            var value = (float)numRotate.Value;
            var center = txtShapeCenter.Text;
            var hasCenter = !chkCenter.Checked;
            var skewDegree = (float)numSkewRotate.Value;

            if ((value == 0 || value == 360 || value == -360) &&
               (skewDegree == 0 || skewDegree == 360 || skewDegree == -360))
                return null;
            float x = 0;
            float y = 0;
            if (!string.IsNullOrEmpty(center) && hasCenter)
            {
                try
                {
                    var arr = center.Split(',');
                    if (arr.Length > 0)
                    {
                        if (float.TryParse(arr[0], out float p))
                            x = p;
                    }
                    if (arr.Length > 1)
                    {
                        if (float.TryParse(arr[1], out float p))
                            y = p;
                    }
                }
                catch { }
            }
            if (hasCenter)
                return new Rotation(value, x, y,
                (SkewType)cmbZRotateDirection.SelectedItem, skewDegree);
            else
                return new Rotation(value, skewType: 
                    (SkewType)cmbZRotateDirection.SelectedItem, skewDegree: skewDegree);
        }
        internal void SetMsMethod()
        {
            MsMethod = null;
            var stroke = (float)numStroke.Value;
            Command lineCommand = 0;

            for (int i = 0; i < chkLstLinePattern.Items.Count; i++)
            {
                if (chkLstLinePattern.GetItemChecked(i))
                {
                    lineCommand |= ((Command)chkLstLinePattern.Items[i]);
                }
                else
                {
                    lineCommand &= ~((Command)chkLstLinePattern.Items[i]);
                }
            }

            MsDisplay.Screen.Canvas.SmoothingMode =
                ((bresenham & Command.Bresenham)==0
                ?
                System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.Default);

            if (textureBrush == null)
            {
                Enum.TryParse(cmbGradient.SelectedItem + "", true, out System.Drawing.Drawing2D.LinearGradientMode msgrad);
                //create an instance of the Microst gradient fill brush 

                object[] all = null;
                float[] positions = new float[lstColors.Items.Count];
                if (lstColors.Items.Count > 0)
                {
                    all = new object[lstColors.Items.Count];
                    lstColors.Items.CopyTo(all, 0);
                }

                if (all == null)
                {
                    all = new object[] { System.Drawing.Color.Red, System.Drawing.Color.Black };
                }
                var Colors = all.Select(x => System.Drawing.Color.FromArgb((int)x)).ToArray();
                if (Colors.Length == 1)
                {
                    Colors = new Color[] { Colors[0], Colors[0] };
                    positions = new float[2];
                }
                for (int i = 0; i < Colors.Length; i++)
                {
                    positions[i] = i / (float)Colors.Length;
                }
                positions[positions.Length - 1] = 1f;

                var Brush = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.RectangleF(x, y, w + 1, h + 1),
                    Colors[0], Colors[Colors.Length - 1], msgrad);
                System.Drawing.Drawing2D.ColorBlend ColorBlend = new System.Drawing.Drawing2D.ColorBlend();
                ColorBlend.Colors = Colors;
                ColorBlend.Positions = positions;
                Brush.InterpolationColors = ColorBlend;
                Brush.InterpolationColors.Positions = positions;
                MsBrush = Brush;
            }
            else
            {
                MsBrush = new TextureBrush(new Bitmap(textureBrush.Width,
                    textureBrush.Height, textureBrush.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, textureBrush.Source));
            }

            switch (cmbShape.Text)
            {
                case "Line":
                    if (drawPoints == null)
                        return;
                    MsMethod = () => MsDisplay.Screen.Canvas.DrawLine(new System.Drawing.Pen(MsBrush, stroke), drawPoints[0], drawPoints[1], drawPoints[2], drawPoints[3]);
                    break;
                case "Circle":
                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillEllipse(MsBrush, x, y, w, w);
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawEllipse(new System.Drawing.Pen(MsBrush, stroke), x, y, w, w);

                    break;
                case "Ellipse":

                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillEllipse(MsBrush, x, y, w, h);
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawEllipse(new System.Drawing.Pen(MsBrush, stroke), x, y, w, h);
                    break;
                case "Arc":
                case "BezierArc":
                    MsMethod = () => MsDisplay.Screen.Canvas.DrawArc(new System.Drawing.Pen(MsBrush, stroke), x, y, w, h, startA, endA);
                    break;
                case "Pie":
                case "BezierPie":
                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillPie(MsBrush, x, y, w, h, startA, endA);
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawPie(new System.Drawing.Pen(MsBrush, stroke), x, y, w, h, startA, endA); break;
                case "Square":
                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillRectangle(MsBrush, x, y, w, w);
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawRectangle(new System.Drawing.Pen(MsBrush, stroke), x, y, w, w);
                    break;
                case "Rectangle":
                    var loc = new System.Drawing.PointF(x, y);

                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillRectangle(MsBrush, new System.Drawing.RectangleF(loc, new System.Drawing.SizeF(w, h)));
                    else
                    {
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawRectangle(new System.Drawing.Pen(MsBrush), x, y, w, h);
                    }
                    break;
                case "Triangle":
                    if (drawPoints == null || drawPoints.Length < 6)
                        return;
                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillPolygon(MsBrush, drawPoints.ToPointsF());
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawPolygon(new System.Drawing.Pen(MsBrush, stroke), drawPoints.ToPointsF());
                    break;

                case "Polygon":
                    if (drawPoints == null)
                        return;
                    if (pse == Command.StrokeOuter || pse == Command.OriginalFill)
                        MsMethod = () => MsDisplay.Screen.Canvas.FillPolygon(MsBrush, drawPoints.ToPointsF(), System.Drawing.Drawing2D.FillMode.Winding);
                    else
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawPolygon(new System.Drawing.Pen(MsBrush, stroke), drawPoints.ToPointsF());
                    break;
                case "Bezier":
                    var type = cmbBezier.SelectedIndex != -1 ?
                        ((KeyValuePair<string, BezierType>)cmbBezier.SelectedItem).Value : BezierType.Cubic;
                    if ((type & BezierType.Multiple) == BezierType.Multiple)
                    {
                        if (drawPoints == null || drawPoints.Length < 6)
                            return;
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawBeziers(new System.Drawing.Pen(MsBrush, stroke), drawPoints.ToPointsF(true));

                    }
                    else
                    {
                        if (drawPoints == null || drawPoints.Length < 8)
                            return;
                        MsMethod = () => MsDisplay.Screen.Canvas.DrawBezier(new System.Drawing.Pen(MsBrush, stroke),
                            drawPoints[0], drawPoints[1], drawPoints[2], drawPoints[3], drawPoints[4], drawPoints[5], drawPoints[6], drawPoints[7]);
                    }
                    break;
                case "Text":
                    int mx = 0, my = 0;
                    if (!string.IsNullOrEmpty(txtGlyphXY.Text))
                    {

                        var items = txtGlyphXY.Text.Split(',');
                        if (items.Length > 0 && int.TryParse(items[0], out int x1))
                            mx = x1;
                        if (items.Length > 1 && int.TryParse(items[0], out int y1))
                            my = y1;
                    }
                    MsMethod = () => MsDisplay.Screen.Canvas.DrawString(txtPts.Text, MsFont, MsBrush, 
                        new System.Drawing.PointF(mx, my));

                    break;
            }
        }
        #endregion

        #region SHOWN
        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            Window.Show();
        }
        #endregion

        #region HIDE
        public void Hide(bool forcefully = false)
        {
            base.Hide();
        }
        #endregion
    }

    partial class Demo
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chkImageOperation = new System.Windows.Forms.CheckBox();
            this.lstColors = new System.Windows.Forms.ListBox();
            this.lstKnownColors = new System.Windows.Forms.ListBox();
            this.txtShapeCenter = new System.Windows.Forms.TextBox();
            this.chkFloodFill = new System.Windows.Forms.CheckBox();
            this.chkCenter = new System.Windows.Forms.CheckBox();
            this.numRotate = new System.Windows.Forms.NumericUpDown();
            this.grpXY = new System.Windows.Forms.GroupBox();
            this.pnlXy = new System.Windows.Forms.Panel();
            this.Xlabel = new System.Windows.Forms.Label();
            this.Ylabel = new System.Windows.Forms.Label();
            this.widthLabel = new System.Windows.Forms.Label();
            this.lblH = new System.Windows.Forms.Label();
            this.numW = new System.Windows.Forms.NumericUpDown();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numH = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.pnlArcE = new System.Windows.Forms.Panel();
            this.lblArce = new System.Windows.Forms.Label();
            this.numArcE = new System.Windows.Forms.NumericUpDown();
            this.pnlArcS = new System.Windows.Forms.Panel();
            this.numArcS = new System.Windows.Forms.NumericUpDown();
            this.numSkewRotate = new System.Windows.Forms.NumericUpDown();

            this.lblArcs = new System.Windows.Forms.Label();
            this.pnlRoundRC = new System.Windows.Forms.Panel();
            this.numCornerRadius = new System.Windows.Forms.NumericUpDown();
            this.lblRoundedBoxOption = new System.Windows.Forms.Label();
            this.lblCornerRadius = new System.Windows.Forms.Label();
            this.pnlTrapezium = new System.Windows.Forms.Panel();
            this.numSizeDiff = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numSize = new System.Windows.Forms.NumericUpDown();
            this.lblSize = new System.Windows.Forms.Label();
            this.cmbBezier = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.numFontSize = new System.Windows.Forms.NumericUpDown();
            this.btnOpenFont = new System.Windows.Forms.Button();
            this.grpArcPie = new System.Windows.Forms.GroupBox();
            this.chkLstCurveOption = new System.Windows.Forms.CheckedListBox();
            this.cmbShape = new System.Windows.Forms.ComboBox();
            this.numStroke = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbStrokeMode = new System.Windows.Forms.ComboBox();
            this.chkRoundBoxOption = new System.Windows.Forms.CheckedListBox();

            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbStroke = new System.Windows.Forms.ComboBox();
            this.cmbGradient = new System.Windows.Forms.ComboBox();
            this.lblLinePattern = new System.Windows.Forms.Label();
            this.btnTextureBrush = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpPts = new System.Windows.Forms.GroupBox();
            this.btnPlot = new System.Windows.Forms.Button();
            this.txtPts = new System.Windows.Forms.TextBox();
            this.btnDraw = new System.Windows.Forms.Button();
            this.chkCompare = new System.Windows.Forms.CheckBox();
            this.chkInvertBrush = new System.Windows.Forms.CheckBox();
            this.pnlBezier = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpFonts = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtGlyphXY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpZRotation = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numScale = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbZRotateDirection = new System.Windows.Forms.ComboBox();
            this.chkLstLinePattern = new System.Windows.Forms.CheckedListBox();

            ((System.ComponentModel.ISupportInitialize)(this.numRotate)).BeginInit();
            this.grpXY.SuspendLayout();
            this.pnlXy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            this.pnlArcE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numArcE)).BeginInit();
            this.pnlArcS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numArcS)).BeginInit();
            this.pnlRoundRC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCornerRadius)).BeginInit();
            this.pnlTrapezium.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSizeDiff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).BeginInit();
            this.grpArcPie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStroke)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grpPts.SuspendLayout();
            this.pnlBezier.SuspendLayout();
            this.grpFonts.SuspendLayout();
            this.grpZRotation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).BeginInit();
            this.SuspendLayout();
            // 
            // chkImageOperation
            // 
            this.chkImageOperation.AutoSize = true;
            this.chkImageOperation.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkImageOperation.ForeColor = System.Drawing.SystemColors.Control;
            this.chkImageOperation.Location = new System.Drawing.Point(85, 115);
            this.chkImageOperation.Name = "chkImageOperation";
            this.chkImageOperation.Size = new System.Drawing.Size(76, 19);
            this.chkImageOperation.TabIndex = 111;
            this.chkImageOperation.Text = "All Image";
            this.chkImageOperation.UseVisualStyleBackColor = true;
            // 
            // lstColors
            // 
            this.lstColors.AllowDrop = true;
            this.lstColors.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lstColors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstColors.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstColors.FormattingEnabled = true;
            this.lstColors.Location = new System.Drawing.Point(233, 18);
            this.lstColors.Name = "lstColors";
            this.lstColors.Size = new System.Drawing.Size(86, 158);
            this.lstColors.TabIndex = 115;
            // 
            // lstKnownColors
            // 
            this.lstKnownColors.AllowDrop = true;
            this.lstKnownColors.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lstKnownColors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstKnownColors.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstKnownColors.FormattingEnabled = true;
            this.lstKnownColors.Location = new System.Drawing.Point(326, 17);
            this.lstKnownColors.Name = "lstKnownColors";
            this.lstKnownColors.Size = new System.Drawing.Size(90, 158);
            this.lstKnownColors.TabIndex = 116;
            // 
            // txtShapeCenter
            // 
            this.txtShapeCenter.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtShapeCenter.Location = new System.Drawing.Point(85, 90);
            this.txtShapeCenter.Name = "txtShapeCenter";
            this.txtShapeCenter.Size = new System.Drawing.Size(116, 23);
            this.txtShapeCenter.TabIndex = 113;
            this.txtShapeCenter.Visible = false;
            // 
            // chkOddEvenStroking
            // 
            this.chkFloodFill.AutoSize = true;
            this.chkFloodFill.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFloodFill.ForeColor = System.Drawing.SystemColors.Control;
            this.chkFloodFill.Location = new System.Drawing.Point(9, 136);
            this.chkFloodFill.Name = "chkOddEvenStroking";
            this.chkFloodFill.Size = new System.Drawing.Size(104, 19);
            this.chkFloodFill.TabIndex = 133;
            this.chkFloodFill.Text = "Flood fill rule";
            this.chkFloodFill.UseVisualStyleBackColor = true;
            // 
            // chkCenter
            // 
            this.chkCenter.AutoSize = true;
            this.chkCenter.Checked = true;
            this.chkCenter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCenter.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCenter.ForeColor = System.Drawing.SystemColors.Control;
            this.chkCenter.Location = new System.Drawing.Point(6, 70);
            this.chkCenter.Name = "chkCenter";
            this.chkCenter.Size = new System.Drawing.Size(64, 19);
            this.chkCenter.TabIndex = 112;
            this.chkCenter.Text = "Center";
            this.chkCenter.UseVisualStyleBackColor = true;
            // 
            // numRotate
            // 
            this.numRotate.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numRotate.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numRotate.Location = new System.Drawing.Point(6, 38);
            this.numRotate.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numRotate.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numRotate.Name = "numRotate";
            this.numRotate.Size = new System.Drawing.Size(57, 22);
            this.numRotate.TabIndex = 109;
            this.numRotate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpXY
            // 
            this.grpXY.Controls.Add(this.pnlXy);
            this.grpXY.Controls.Add(this.pnlArcE);
            this.grpXY.Controls.Add(this.pnlArcS);
            this.grpXY.Controls.Add(this.pnlRoundRC);
            this.grpXY.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpXY.ForeColor = System.Drawing.SystemColors.Control;
            this.grpXY.Location = new System.Drawing.Point(5, 72);
            this.grpXY.Name = "grpXY";
            this.grpXY.Size = new System.Drawing.Size(197, 129);
            this.grpXY.TabIndex = 122;
            this.grpXY.TabStop = false;
            this.grpXY.Text = "Enter Circle parameters";
            // 
            // pnlXy
            // 
            this.pnlXy.Controls.Add(this.Xlabel);
            this.pnlXy.Controls.Add(this.Ylabel);
            this.pnlXy.Controls.Add(this.widthLabel);
            this.pnlXy.Controls.Add(this.lblH);
            this.pnlXy.Controls.Add(this.numW);
            this.pnlXy.Controls.Add(this.numX);
            this.pnlXy.Controls.Add(this.numH);
            this.pnlXy.Controls.Add(this.numY);
            this.pnlXy.Location = new System.Drawing.Point(3, 20);
            this.pnlXy.Name = "pnlXy";
            this.pnlXy.Size = new System.Drawing.Size(105, 100);
            this.pnlXy.TabIndex = 76;
            // 
            // Xlabel
            // 
            this.Xlabel.AutoSize = true;
            this.Xlabel.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Xlabel.Location = new System.Drawing.Point(3, 7);
            this.Xlabel.Name = "Xlabel";
            this.Xlabel.Size = new System.Drawing.Size(14, 15);
            this.Xlabel.TabIndex = 15;
            this.Xlabel.Text = "X";
            // 
            // Ylabel
            // 
            this.Ylabel.AutoSize = true;
            this.Ylabel.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ylabel.Location = new System.Drawing.Point(3, 29);
            this.Ylabel.Name = "Ylabel";
            this.Ylabel.Size = new System.Drawing.Size(14, 15);
            this.Ylabel.TabIndex = 19;
            this.Ylabel.Text = "Y";
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.widthLabel.Location = new System.Drawing.Point(3, 57);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(41, 15);
            this.widthLabel.TabIndex = 16;
            this.widthLabel.Text = "Width";
            // 
            // lblH
            // 
            this.lblH.AutoSize = true;
            this.lblH.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblH.Location = new System.Drawing.Point(3, 79);
            this.lblH.Name = "lblH";
            this.lblH.Size = new System.Drawing.Size(43, 15);
            this.lblH.TabIndex = 20;
            this.lblH.Text = "Height";
            this.lblH.Visible = false;
            // 
            // numW
            // 
            this.numW.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numW.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numW.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numW.Location = new System.Drawing.Point(46, 51);
            this.numW.Maximum = new decimal(new int[] {
            1100,
            0,
            0,
            0});
            this.numW.Name = "numW";
            this.numW.Size = new System.Drawing.Size(55, 23);
            this.numW.TabIndex = 33;
            this.numW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numW.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numX
            // 
            this.numX.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numX.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numX.Location = new System.Drawing.Point(46, 1);
            this.numX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(55, 23);
            this.numX.TabIndex = 31;
            this.numX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numH
            // 
            this.numH.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numH.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numH.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numH.Location = new System.Drawing.Point(46, 76);
            this.numH.Maximum = new decimal(new int[] {
            1100,
            0,
            0,
            0});
            this.numH.Name = "numH";
            this.numH.Size = new System.Drawing.Size(55, 23);
            this.numH.TabIndex = 34;
            this.numH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numH.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numH.Visible = false;
            // 
            // numY
            // 
            this.numY.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numY.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numY.Location = new System.Drawing.Point(46, 26);
            this.numY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(55, 23);
            this.numY.TabIndex = 32;
            this.numY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numY.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // pnlArcE
            // 
            this.pnlArcE.Controls.Add(this.lblArce);
            this.pnlArcE.Controls.Add(this.numArcE);
            this.pnlArcE.Location = new System.Drawing.Point(106, 75);
            this.pnlArcE.Name = "pnlArcE";
            this.pnlArcE.Size = new System.Drawing.Size(84, 48);
            this.pnlArcE.TabIndex = 52;
            this.pnlArcE.Visible = false;
            // 
            // lblArce
            // 
            this.lblArce.AutoSize = true;
            this.lblArce.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArce.Location = new System.Drawing.Point(2, 1);
            this.lblArce.Name = "lblArce";
            this.lblArce.Size = new System.Drawing.Size(61, 15);
            this.lblArce.TabIndex = 49;
            this.lblArce.Text = "End Angle";
            // 
            // numArcE
            // 
            this.numArcE.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numArcE.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numArcE.Location = new System.Drawing.Point(4, 20);
            this.numArcE.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numArcE.Name = "numArcE";
            this.numArcE.Size = new System.Drawing.Size(76, 22);
            this.numArcE.TabIndex = 48;
            this.numArcE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numArcE.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // pnlArcS
            // 
            this.pnlArcS.Controls.Add(this.numArcS);
            this.pnlArcS.Controls.Add(this.lblArcs);
            this.pnlArcS.Location = new System.Drawing.Point(106, 22);
            this.pnlArcS.Name = "pnlArcS";
            this.pnlArcS.Size = new System.Drawing.Size(84, 46);
            this.pnlArcS.TabIndex = 52;
            this.pnlArcS.Visible = false;
            // 
            // numArcS
            // 
            this.numArcS.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numArcS.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numArcS.Location = new System.Drawing.Point(4, 19);
            this.numArcS.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numArcS.Name = "numArcS";
            this.numArcS.Size = new System.Drawing.Size(75, 22);
            this.numArcS.TabIndex = 27;
            this.numArcS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblArcs
            // 
            this.lblArcs.AutoSize = true;
            this.lblArcs.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArcs.Location = new System.Drawing.Point(2, 2);
            this.lblArcs.Name = "lblArcs";
            this.lblArcs.Size = new System.Drawing.Size(68, 15);
            this.lblArcs.TabIndex = 29;
            this.lblArcs.Text = "Start Angle";
            // 
            // pnlRoundRC
            // 
            this.pnlRoundRC.Controls.Add(this.numCornerRadius);
            this.pnlRoundRC.Controls.Add(this.chkRoundBoxOption);
            this.pnlRoundRC.Controls.Add(this.lblCornerRadius);
            this.pnlRoundRC.Controls.Add(this.lblRoundedBoxOption);
            this.pnlRoundRC.Location = new System.Drawing.Point(106, 15);
            this.pnlRoundRC.Name = "pnlRoundRC";
            this.pnlRoundRC.Size = new System.Drawing.Size(88, 110);
            this.pnlRoundRC.TabIndex = 94;
            this.pnlRoundRC.Visible = false;
            // 
            // numCornerRadius
            // 
            this.numCornerRadius.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCornerRadius.Location = new System.Drawing.Point(6, 15);
            this.numCornerRadius.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numCornerRadius.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numCornerRadius.Name = "numCornerRadius";
            this.numCornerRadius.Size = new System.Drawing.Size(67, 23);
            this.numCornerRadius.TabIndex = 106;
            this.numCornerRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numCornerRadius.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});

            //
            // cmbRoundBoxOption
            // 
            this.chkRoundBoxOption.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.chkRoundBoxOption.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRoundBoxOption.Location = new System.Drawing.Point(6, 52);
            this.chkRoundBoxOption.Size = new System.Drawing.Size(78, 65);
            this.chkRoundBoxOption.CheckOnClick = true;
            // 
            // lblCornerRadius
            // 
            this.lblRoundedBoxOption.AutoSize = true;
            this.lblRoundedBoxOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoundedBoxOption.Location = new System.Drawing.Point(5, 37);
            this.lblRoundedBoxOption.Name = "lblRoundedBoxOption";
            this.lblRoundedBoxOption.Size = new System.Drawing.Size(87, 13);
            this.lblRoundedBoxOption.TabIndex = 109;
            this.lblRoundedBoxOption.Text = "Box Options";

            // 
            // lblCornerRadius
            // 
            this.lblCornerRadius.AutoSize = true;
            this.lblCornerRadius.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCornerRadius.Location = new System.Drawing.Point(5, 0);
            this.lblCornerRadius.Name = "lblCornerRadius";
            this.lblCornerRadius.Size = new System.Drawing.Size(87, 13);
            this.lblCornerRadius.TabIndex = 109;
            this.lblCornerRadius.Text = "Corner Radius";
            // 
            // pnlTrapezium
            // 
            this.pnlTrapezium.Controls.Add(this.numSizeDiff);
            this.pnlTrapezium.Controls.Add(this.label11);
            this.pnlTrapezium.Controls.Add(this.numSize);
            this.pnlTrapezium.Controls.Add(this.lblSize);
            this.pnlTrapezium.Location = new System.Drawing.Point(112, 86);
            this.pnlTrapezium.Name = "pnlTrapezium";
            this.pnlTrapezium.Size = new System.Drawing.Size(84, 54);
            this.pnlTrapezium.TabIndex = 98;
            this.pnlTrapezium.Visible = false;
            // 
            // numSizeDiff
            // 
            this.numSizeDiff.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numSizeDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSizeDiff.Location = new System.Drawing.Point(34, 31);
            this.numSizeDiff.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.numSizeDiff.Name = "numSizeDiff";
            this.numSizeDiff.Size = new System.Drawing.Size(55, 20);
            this.numSizeDiff.TabIndex = 72;
            this.numSizeDiff.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(13, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 20);
            this.label11.TabIndex = 73;
            this.label11.Text = "-";
            // 
            // numSize
            // 
            this.numSize.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSize.Location = new System.Drawing.Point(34, 5);
            this.numSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSize.Name = "numSize";
            this.numSize.Size = new System.Drawing.Size(51, 20);
            this.numSize.TabIndex = 70;
            this.numSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSize.ForeColor = System.Drawing.Color.White;
            this.lblSize.Location = new System.Drawing.Point(1, 7);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(31, 13);
            this.lblSize.TabIndex = 71;
            this.lblSize.Text = "Size";
            // 
            // cmbBezier
            // 
            this.cmbBezier.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbBezier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBezier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBezier.FormattingEnabled = true;
            this.cmbBezier.Location = new System.Drawing.Point(6, 19);
            this.cmbBezier.Name = "cmbBezier";
            this.cmbBezier.Size = new System.Drawing.Size(94, 23);
            this.cmbBezier.TabIndex = 36;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Black;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(225, 36);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 28);
            this.btnSave.TabIndex = 132;
            this.btnSave.Text = "Save ";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // numFontSize
            // 
            this.numFontSize.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numFontSize.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numFontSize.Location = new System.Drawing.Point(40, 53);
            this.numFontSize.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numFontSize.Name = "numFontSize";
            this.numFontSize.Size = new System.Drawing.Size(58, 23);
            this.numFontSize.TabIndex = 58;
            this.numFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // btnOpenFont
            // 
            this.btnOpenFont.BackColor = System.Drawing.Color.Black;
            this.btnOpenFont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFont.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenFont.ForeColor = System.Drawing.Color.White;
            this.btnOpenFont.Location = new System.Drawing.Point(6, 22);
            this.btnOpenFont.Name = "btnOpenFont";
            this.btnOpenFont.Size = new System.Drawing.Size(91, 25);
            this.btnOpenFont.TabIndex = 65;
            this.btnOpenFont.Text = "Open file";
            this.btnOpenFont.UseVisualStyleBackColor = false;
            this.btnOpenFont.Visible = false;
            // 
            // grpArcPie
            // 
            this.grpArcPie.Controls.Add(this.chkLstCurveOption);
            this.grpArcPie.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpArcPie.ForeColor = System.Drawing.SystemColors.Control;
            this.grpArcPie.Location = new System.Drawing.Point(4, 201);
            this.grpArcPie.Name = "grpArcPie";
            this.grpArcPie.Size = new System.Drawing.Size(197, 82);
            this.grpArcPie.TabIndex = 127;
            this.grpArcPie.TabStop = false;
            this.grpArcPie.Text = "Curve Option";
            this.grpArcPie.Visible = false;
            // 
            // lstCurveOption
            // 
            this.chkLstCurveOption.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.chkLstCurveOption.CheckOnClick = true;
            this.chkLstCurveOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLstCurveOption.FormattingEnabled = true;
            this.chkLstCurveOption.Location = new System.Drawing.Point(6, 22);
            this.chkLstCurveOption.Name = "lstCurveOption";
            this.chkLstCurveOption.Size = new System.Drawing.Size(178, 49);
            this.chkLstCurveOption.TabIndex = 72;
            // 
            // cmbShape
            // 
            this.cmbShape.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbShape.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbShape.FormattingEnabled = true;
            this.cmbShape.Location = new System.Drawing.Point(9, 36);
            this.cmbShape.Name = "cmbShape";
            this.cmbShape.Size = new System.Drawing.Size(184, 23);
            this.cmbShape.TabIndex = 35;

            // numStroke
            // 
            this.numStroke.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numStroke.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numStroke.Location = new System.Drawing.Point(57, 76);
            this.numStroke.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numStroke.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.numStroke.Name = "numStroke";
            this.numStroke.Size = new System.Drawing.Size(81, 22);
            this.numStroke.TabIndex = 56;
            this.numStroke.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkLstLinePattern);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.chkFloodFill);
            this.groupBox1.Controls.Add(this.numStroke);
            this.groupBox1.Controls.Add(this.cmbStrokeMode);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lstKnownColors);
            this.groupBox1.Controls.Add(this.lstColors);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbStroke);
            this.groupBox1.Controls.Add(this.cmbGradient);
            this.groupBox1.Controls.Add(this.lblLinePattern);
            this.groupBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Location = new System.Drawing.Point(5, 286);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 185);
            this.groupBox1.TabIndex = 123;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fill";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(7, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 15);
            this.label12.TabIndex = 74;
            this.label12.Text = "Stroke";
            // 
            // cmbStrokeMode
            // 
            this.cmbStrokeMode.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbStrokeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStrokeMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStrokeMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbStrokeMode.FormattingEnabled = true;
            this.cmbStrokeMode.Location = new System.Drawing.Point(57, 102);
            this.cmbStrokeMode.Name = "cmbStrokeMode";
            this.cmbStrokeMode.Size = new System.Drawing.Size(81, 23);
            this.cmbStrokeMode.TabIndex = 119;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(8, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 15);
            this.label7.TabIndex = 118;
            this.label7.Text = "Mode";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 15);
            this.label9.TabIndex = 39;
            this.label9.Text = "Option";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 15);
            this.label3.TabIndex = 37;
            this.label3.Text = "Mode";
            // 
            // cmbStroke
            // 
            this.cmbStroke.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbStroke.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStroke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStroke.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbStroke.FormattingEnabled = true;
            this.cmbStroke.Location = new System.Drawing.Point(57, 47);
            this.cmbStroke.Name = "cmbStroke";
            this.cmbStroke.Size = new System.Drawing.Size(170, 23);
            this.cmbStroke.TabIndex = 59;
            // 
            // cmbGradient
            // 
            this.cmbGradient.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbGradient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGradient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGradient.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGradient.FormattingEnabled = true;
            this.cmbGradient.Location = new System.Drawing.Point(57, 17);
            this.cmbGradient.Name = "cmbGradient";
            this.cmbGradient.Size = new System.Drawing.Size(170, 23);
            this.cmbGradient.TabIndex = 38;
            // 
            // lblLinePattern
            // 
            this.lblLinePattern.AutoSize = true;
            this.lblLinePattern.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLinePattern.Location = new System.Drawing.Point(144, 84);
            this.lblLinePattern.Name = "lblLinePattern";
            this.lblLinePattern.Size = new System.Drawing.Size(74, 15);
            this.lblLinePattern.TabIndex = 60;
            this.lblLinePattern.Text = "Line Pattern";
            // 
            // btnTextureBrush
            // 
            this.btnTextureBrush.BackColor = System.Drawing.Color.Black;
            this.btnTextureBrush.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTextureBrush.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTextureBrush.ForeColor = System.Drawing.Color.White;
            this.btnTextureBrush.Location = new System.Drawing.Point(213, 248);
            this.btnTextureBrush.Name = "btnTextureBrush";
            this.btnTextureBrush.Size = new System.Drawing.Size(210, 33);
            this.btnTextureBrush.TabIndex = 116;
            this.btnTextureBrush.Text = "Select Texture Brush";
            this.btnTextureBrush.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.Black;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(292, 36);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(63, 28);
            this.btnClear.TabIndex = 120;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // grpPts
            // 
            this.grpPts.Controls.Add(this.btnPlot);
            this.grpPts.Controls.Add(this.txtPts);
            this.grpPts.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPts.ForeColor = System.Drawing.SystemColors.Control;
            this.grpPts.Location = new System.Drawing.Point(5, 479);
            this.grpPts.Name = "grpPts";
            this.grpPts.Size = new System.Drawing.Size(424, 163);
            this.grpPts.TabIndex = 118;
            this.grpPts.TabStop = false;
            this.grpPts.Text = "Click Gws Drawing Area to select point";
            // 
            // btnPlot
            // 
            this.btnPlot.BackColor = System.Drawing.Color.Black;
            this.btnPlot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlot.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlot.ForeColor = System.Drawing.Color.White;
            this.btnPlot.Location = new System.Drawing.Point(6, 124);
            this.btnPlot.Name = "btnPlot";
            this.btnPlot.Size = new System.Drawing.Size(409, 32);
            this.btnPlot.TabIndex = 14;
            this.btnPlot.Text = "Click here to plot path of points";
            this.btnPlot.UseVisualStyleBackColor = false;
            // 
            // txtPts
            // 
            this.txtPts.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtPts.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPts.Location = new System.Drawing.Point(7, 18);
            this.txtPts.Multiline = true;
            this.txtPts.Name = "txtPts";
            this.txtPts.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPts.Size = new System.Drawing.Size(408, 103);
            this.txtPts.TabIndex = 12;

            // 
            // btnDraw
            // 
            this.btnDraw.BackColor = System.Drawing.Color.Black;
            this.btnDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDraw.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDraw.ForeColor = System.Drawing.Color.White;
            this.btnDraw.Location = new System.Drawing.Point(359, 36);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(63, 28);
            this.btnDraw.TabIndex = 119;
            this.btnDraw.Text = "Draw";
            this.btnDraw.UseVisualStyleBackColor = false;
            // 
            // chkCompare
            // 
            this.chkCompare.AutoSize = true;
            this.chkCompare.Font = new System.Drawing.Font("Book Antiqua", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCompare.ForeColor = System.Drawing.Color.Lime;
            this.chkCompare.Location = new System.Drawing.Point(68, 8);
            this.chkCompare.Name = "chkCompare";
            this.chkCompare.Size = new System.Drawing.Size(220, 22);
            this.chkCompare.TabIndex = 135;
            this.chkCompare.Text = "Compare Windows Graphics";
            this.chkCompare.UseVisualStyleBackColor = true;
            // 
            // chkInvertBrush
            // 
            this.chkInvertBrush.AutoSize = true;
            this.chkInvertBrush.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkInvertBrush.ForeColor = System.Drawing.SystemColors.Control;
            this.chkInvertBrush.Location = new System.Drawing.Point(57, 144);
            this.chkInvertBrush.Name = "chkInvertBrush";
            this.chkInvertBrush.Size = new System.Drawing.Size(143, 19);
            this.chkInvertBrush.TabIndex = 147;
            this.chkInvertBrush.Text = "Invert Brush Rotation";
            this.chkInvertBrush.UseVisualStyleBackColor = true;
            // 
            // pnlBezier
            // 
            this.pnlBezier.Controls.Add(this.cmbBezier);
            this.pnlBezier.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlBezier.ForeColor = System.Drawing.Color.White;
            this.pnlBezier.Location = new System.Drawing.Point(4, 68);
            this.pnlBezier.Name = "pnlBezier";
            this.pnlBezier.Size = new System.Drawing.Size(105, 48);
            this.pnlBezier.TabIndex = 140;
            this.pnlBezier.TabStop = false;
            this.pnlBezier.Text = "Bezier Type";
            this.pnlBezier.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 15);
            this.label2.TabIndex = 66;
            this.label2.Text = "Size";
            // 
            // grpFonts
            // 
            this.grpFonts.Controls.Add(this.label10);
            this.grpFonts.Controls.Add(this.txtGlyphXY);
            this.grpFonts.Controls.Add(this.numFontSize);
            this.grpFonts.Controls.Add(this.label2);
            this.grpFonts.Controls.Add(this.btnOpenFont);
            this.grpFonts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpFonts.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpFonts.ForeColor = System.Drawing.Color.White;
            this.grpFonts.Location = new System.Drawing.Point(4, 68);
            this.grpFonts.Name = "grpFonts";
            this.grpFonts.Size = new System.Drawing.Size(104, 130);
            this.grpFonts.TabIndex = 141;
            this.grpFonts.TabStop = false;
            this.grpFonts.Text = "Select Font";
            this.grpFonts.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(9, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 15);
            this.label10.TabIndex = 148;
            this.label10.Text = "Location";
            // 
            // txtGlyphXY
            // 
            this.txtGlyphXY.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtGlyphXY.Location = new System.Drawing.Point(9, 98);
            this.txtGlyphXY.Name = "txtGlyphXY";
            this.txtGlyphXY.Size = new System.Drawing.Size(88, 23);
            this.txtGlyphXY.TabIndex = 114;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.LemonChiffon;
            this.label4.Location = new System.Drawing.Point(7, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 19);
            this.label4.TabIndex = 144;
            this.label4.Text = "Shape:";
            // 
            // grpZRotation
            // 
            this.grpZRotation.Controls.Add(this.label15);
            this.grpZRotation.Controls.Add(this.numScale);
            this.grpZRotation.Controls.Add(this.txtShapeCenter);
            this.grpZRotation.Controls.Add(this.chkInvertBrush);
            this.grpZRotation.Controls.Add(this.label14);
            this.grpZRotation.Controls.Add(this.chkCenter);
            this.grpZRotation.Controls.Add(this.label6);
            this.grpZRotation.Controls.Add(this.chkImageOperation);
            this.grpZRotation.Controls.Add(this.cmbZRotateDirection);
            this.grpZRotation.Controls.Add(this.numRotate);
            this.grpZRotation.Controls.Add(this.numSkewRotate);
            this.grpZRotation.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpZRotation.ForeColor = System.Drawing.Color.White;
            this.grpZRotation.Location = new System.Drawing.Point(213, 72);
            this.grpZRotation.Name = "grpZRotation";
            this.grpZRotation.Size = new System.Drawing.Size(210, 169);
            this.grpZRotation.TabIndex = 145;
            this.grpZRotation.TabStop = false;
            this.grpZRotation.Text = "Rotation && Scalling";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(7, 94);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 15);
            this.label15.TabIndex = 147;
            this.label15.Text = "Scale x 0.25";
            // 
            // numScale
            // 
            this.numScale.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numScale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numScale.DecimalPlaces = 3;
            this.numScale.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numScale.Hexadecimal = true;
            this.numScale.Location = new System.Drawing.Point(6, 112);
            this.numScale.Name = "numScale";
            this.numScale.Size = new System.Drawing.Size(57, 22);
            this.numScale.TabIndex = 150;
            this.numScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(6, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(38, 15);
            this.label14.TabIndex = 149;
            this.label14.Text = "Angle";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(130, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 15);
            this.label6.TabIndex = 147;
            this.label6.Text = "Skew Effect";
            // 
            // cmbZRotateDirection
            // 
            this.cmbZRotateDirection.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbZRotateDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZRotateDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbZRotateDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbZRotateDirection.FormattingEnabled = true;
            this.cmbZRotateDirection.Location = new System.Drawing.Point(85, 38);
            this.cmbZRotateDirection.Name = "cmbZRotateDirection";
            this.cmbZRotateDirection.Size = new System.Drawing.Size(116, 23);
            this.cmbZRotateDirection.TabIndex = 146;
            //
            //numSkewRotate
            //
            this.numSkewRotate.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.numSkewRotate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numSkewRotate.DecimalPlaces = 0;
            this.numSkewRotate.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSkewRotate.Location = new System.Drawing.Point(85, 65);
            this.numSkewRotate.Name = "numSkewRotate";
            this.numSkewRotate.Size = new System.Drawing.Size(57, 22);
            this.numSkewRotate.TabIndex = 150;
            this.numSkewRotate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numSkewRotate.Minimum = -360;
            this.numSkewRotate.Maximum = 360;
            // 
            // cmbLinePattern
            // 
            this.chkLstLinePattern.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.chkLstLinePattern.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLstLinePattern.FormattingEnabled = true;
            this.chkLstLinePattern.Location = new System.Drawing.Point(140, 102);
            this.chkLstLinePattern.Name = "cmbLinePattern";
            this.chkLstLinePattern.Size = new System.Drawing.Size(90, 80);
            this.chkLstLinePattern.TabIndex = 135;
            this.chkLstLinePattern.CheckOnClick = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MidnightBlue;
            this.ClientSize = new System.Drawing.Size(429, 647);
            this.Controls.Add(this.grpZRotation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbShape);
            this.Controls.Add(this.grpFonts);
            this.Controls.Add(this.pnlBezier);
            this.Controls.Add(this.pnlTrapezium);
            this.Controls.Add(this.chkCompare);
            this.Controls.Add(this.grpXY);
            this.Controls.Add(this.btnTextureBrush);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpArcPie);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.grpPts);
            this.Controls.Add(this.btnDraw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(100, 200);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Demo";
            ((System.ComponentModel.ISupportInitialize)(this.numRotate)).EndInit();
            this.grpXY.ResumeLayout(false);
            this.pnlXy.ResumeLayout(false);
            this.pnlXy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            this.pnlArcE.ResumeLayout(false);
            this.pnlArcE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numArcE)).EndInit();
            this.pnlArcS.ResumeLayout(false);
            this.pnlArcS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numArcS)).EndInit();
            this.pnlRoundRC.ResumeLayout(false);
            this.pnlRoundRC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCornerRadius)).EndInit();
            this.pnlTrapezium.ResumeLayout(false);
            this.pnlTrapezium.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSizeDiff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).EndInit();
            this.grpArcPie.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numStroke)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPts.ResumeLayout(false);
            this.grpPts.PerformLayout();
            this.pnlBezier.ResumeLayout(false);
            this.grpFonts.ResumeLayout(false);
            this.grpFonts.PerformLayout();
            this.grpZRotation.ResumeLayout(false);
            this.grpZRotation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region CONTROL VARIABLES
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnOpenFont;
        private System.Windows.Forms.Button btnPlot;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTextureBrush;

        private System.Windows.Forms.CheckBox chkCenter;
        private System.Windows.Forms.CheckBox chkCompare;
        private System.Windows.Forms.CheckBox chkImageOperation;
        private System.Windows.Forms.CheckBox chkInvertBrush;
        private System.Windows.Forms.CheckBox chkFloodFill;
        private System.Windows.Forms.CheckedListBox chkLstCurveOption;

        private System.Windows.Forms.ComboBox cmbBezier;
        private System.Windows.Forms.ComboBox cmbGradient;
        private System.Windows.Forms.ComboBox cmbShape;
        private System.Windows.Forms.ComboBox cmbStroke;
        private System.Windows.Forms.ComboBox cmbStrokeMode;
        private System.Windows.Forms.ComboBox cmbZRotateDirection;
        private System.Windows.Forms.CheckedListBox chkLstLinePattern;
        private System.Windows.Forms.CheckedListBox chkRoundBoxOption;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpArcPie;
        private System.Windows.Forms.GroupBox grpFonts;
        private System.Windows.Forms.GroupBox grpPts;
        private System.Windows.Forms.GroupBox grpXY;
        private System.Windows.Forms.GroupBox grpZRotation;
        private System.Windows.Forms.GroupBox pnlBezier;

        private System.Windows.Forms.Label lblCornerRadius;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblArce;
        private System.Windows.Forms.Label lblArcs;
        private System.Windows.Forms.Label lblRoundedBoxOption;
        private System.Windows.Forms.Label lblH;
        private System.Windows.Forms.Label lblLinePattern;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label Xlabel;
        private System.Windows.Forms.Label Ylabel;

        private System.Windows.Forms.ListBox lstKnownColors;
        private System.Windows.Forms.ListBox lstColors;

        private System.Windows.Forms.NumericUpDown numArcE;
        private System.Windows.Forms.NumericUpDown numArcS;
        private System.Windows.Forms.NumericUpDown numCornerRadius;
        private System.Windows.Forms.NumericUpDown numFontSize;
        private System.Windows.Forms.NumericUpDown numH;
        private System.Windows.Forms.NumericUpDown numRotate;
        private System.Windows.Forms.NumericUpDown numScale;
        private System.Windows.Forms.NumericUpDown numSize;
        private System.Windows.Forms.NumericUpDown numSizeDiff;
        private System.Windows.Forms.NumericUpDown numStroke;
        private System.Windows.Forms.NumericUpDown numW;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numSkewRotate;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;

        private System.Windows.Forms.Panel pnlArcE;
        private System.Windows.Forms.Panel pnlArcS;
        private System.Windows.Forms.Panel pnlRoundRC;
        private System.Windows.Forms.Panel pnlTrapezium;
        private System.Windows.Forms.Panel pnlXy;

        private System.Windows.Forms.TextBox txtGlyphXY;
        private System.Windows.Forms.TextBox txtPts;
        private System.Windows.Forms.TextBox txtShapeCenter;

        #endregion
    }
}
#endif
