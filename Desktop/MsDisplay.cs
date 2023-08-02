/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if MS && (GWS || Window)
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MnM.GWS.MS
{
    public partial class MsDisplay : System.Windows.Forms.Form
    {
        static MsDisplay screen;
        bool StopDraw;
        string shape;
        static IRotation rotate;

        public MsDisplay()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.Paint += Draw;
        }

        public static MsDisplay Screen
        {
            get
            {
                if (screen == null || screen.IsDisposed)
                {
                    screen = new MsDisplay();
                    screen.Show();
                }
                return screen;
            }
        }
        public static bool Destroyed => screen == null || screen.IsDisposed;
        internal System.Drawing.Graphics Canvas { get; private set; }

        void Draw(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (StopDraw)
                return;
            Canvas = e.Graphics;
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            Demo.Instance.SetMsMethod();
            StopDraw = true;
            if (rotate != null && rotate.Valid)
            {
                var matrix = new Matrix();

                matrix.RotateAt(rotate.Angle, new System.Drawing.PointF(rotate.Centre?.Cx??0, rotate.Centre?.Cy??0));
                e.Graphics.Transform = matrix;
            }
            if (Demo.Instance.MsMethod != null)
            {
                base.Text = Benchmarks.Execute(Demo.Instance.MsMethod, out long i, shape + "", MSBridge.BUnit);
            }
            if (rotate != null && rotate.Valid)
            {
                //e.Graphics.TranslateTransform(-rotate.CX, -rotate.CY);

                e.Graphics.ResetTransform();
            }
            StopDraw = false;
            Canvas = null;
        }
        internal void Refresh(string shapeName, IRotation rotation)
        {
            shape = shapeName;
            rotate = rotation;
            Refresh();
        }
        internal static void Clear()
        {
            //using (var g = Screen.CreateGraphics())
            //{
            //    g.Clear(Screen.BackColour);
            //}
        }

#region Windows Form Designer generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.SuspendLayout();
            // 
            // MsDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.BackColour = Colour.FromArgb(Rgba.Black);
            this.ClientSize = new System.Drawing.Size(404, 506);
            this.Location = new System.Drawing.Point(1073, 200);
            this.Name = "MsDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MsDisplay";
            this.ResumeLayout(false);
            Button b;
        }

#endregion
    }
}
#endif
