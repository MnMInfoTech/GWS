/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS.MS
{
    static class Program
    {
        static IDesktop Desktop;
        static BrushStyle rg, rg1, fs, fs1, moonfs;
        static ITextureBrush textureBrush;
        static IFont tahoma;
        static int X, Y;
        static int j = 1000;

        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Factory.Attach<Instance>();
#if Demo
            goto Run;
#endif
            rg = new BrushStyle(BrushType.Conical, Rgba.Red, Rgba.SkyBlue, Rgba.LightPink, Rgba.Green);
            rg1 = new BrushStyle(BrushType.Vertical, Rgba.Yellow, Rgba.Green, Rgba.Orange, Rgba.Olive);
            fs = new BrushStyle(BrushType.Conical, Rgba.Gold, Rgba.Orange, Rgba.Green, Rgba.Teal);
            fs1 = new BrushStyle(BrushType.Vertical, Rgba.Black, Rgba.Maroon);
            moonfs = new BrushStyle(BrushType.BackwardDiagonal, Rgba.Red, Rgba.Green);
            textureBrush = Factory.newBrush(AppDomain.CurrentDomain.BaseDirectory + "\\GWS.png");
            tahoma = Factory.newFont("c:\\windows\\fonts\\tahoma.ttf", 40);

            Desktop = Factory.newDesktop(null, 120, 120, 1000, 900);
           

            Desktop.PaintImages += Window_PaintBackground;
            Desktop.KeyPress += Window_KeyPress;
            Desktop.MouseDown += Window_MouseDown;
            Desktop.MouseUp += Window_MouseUp;
            Desktop.Load += Desktop_Load;
#if Advance
            Desktop.LoadAnimations += Desktop_LoadAnimations;
#endif
        Run:
#if Demo
            MnM.GWS.Application.Run(Demo.Instance);
#else
            MnM.GWS.Application.Run(Desktop);
#endif
        }

        private static void Desktop_Load(object sender, IEventArgs e)
        {
            var Path = Desktop.Controls;

            Path.Add(new Polygon(167 + 200, 97, 110 + 200, 216, 248 + 200, 149, 107 + 200, 151, 218 + 200, 216),
                new BrushStyle(BrushType.Conical, Rgba.Green, Rgba.Orange, Rgba.Black),
                8.ToStroke(), FillCommand.FloodFill.Add());

            Path.Add(new Polygon(90, 215, 163, 29, 63, 202, 188, 46, 41, 182, 206, 70,
                26, 156, 217, 97, 20, 127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182,
                51, 46, 176, 202, 76, 29, 149, 215, 105, 21, 120, 220, 134, 21, 90, 215), rg1);

            Path.Add(new Triangle(40, 10, 100, 140, 56, 300), moonfs);

            //Path.Add(new Polygon(0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
            //        497, 83, 494, 84, 490, 85, 484, 86, 477, 87, 469, 88, 460, 89, 450, 90, 439, 91, 427,
            //        92, 414, 93, 400, 93, 475, 94, 480, 95, 485, 97, 490, 99, 495, 101, 500, 103, 495, 105,
            //        490, 107, 485, 108, 480, 110, 475, 110, 400, 112, 400, 112, 490, 113, 495, 115, 500, 117,
            //        497, 118, 493, 119, 488, 120, 482, 121, 475, 122, 467, 123, 458, 124, 448, 125, 437,
            //        126, 425, 127, 412, 129, 350, 129, 500, 129, 450, 140, 400, 130, 455, 140, 500, 141,
            //        494, 142, 487, 143, 479, 144, 470, 145, 460, 146, 450, 167, 450, 167, 443, 165, 431,
            //        163, 417, 161, 405, 159, 402, 157, 400, 155, 402, 153, 405, 151, 417, 149, 431, 147,
            //        443, 145, 449, 147, 458, 149, 470, 151, 484, 153, 495, 155, 500, 157, 500, 159, 500,
            //        161, 494, 163, 487, 165, 479, 167, 470, 169, 460, 171, 449, 173, 442, 175, 431, 177,
            //        418, 179, 407, 181, 404, 183, 402, 185, 404, 187, 407, 189, 418, 191, 431, 189, 424,
            //        187, 414, 185, 411, 183, 409, 181, 411, 179, 414, 177, 423, 175, 434, 177, 442, 179,
            //        446, 181, 449, 183, 452, 185, 456, 187, 464, 189, 475, 187, 488, 185, 499, 183, 502,
            //        181, 504, 179, 501, 177, 490, 175, 477, 177, 474, 179, 474, 181, 473, 183, 471, 185,
            //        468, 187, 464, 189, 459, 191, 453, 193, 446, 195, 438, 197, 429, 199, 419, 201, 408,
            //        203, 396, 205, 383, 207, 369, 209, 354, 209, 350, 209, 500, 210, 450, 212, 438, 214,
            //        425, 216, 412, 218, 405, 220, 402, 222, 400, 224, 403, 226, 410, 228, 422, 230, 433,
            //        232, 443, 232, 600, 0, 600, 0, 500), moonfs);

            var @params = new IParameter[] { rg, 23.ToStroke() };
            Path.Add(new Curve(600, 10, 300, 400), @params);
            //Path.Add(new Ellipse(500, 127, 300, 200, 35.ToRotation()), @params);
            Path.Add(new Tetragon(425, 480, 650, 690, 190), @params);

            Path.Add(new Bezier(158, 181, 174, 348, 350, 363, 541, 145), @params);

            Path.Add(new RoundBox(300, 300, 200, 200, 25), @params);
            Path.Add(new Curve(300, 527, 300, 200, 55, 300, CurveType.Pie), @params);

            Path.Add(new Text("GWS - Drawing", 400, 570), moonfs, tahoma);
            Path.Add(new Text("GWS - Drawing", 400, 570), rg1, tahoma);

            #if Advance
            var button = Factory.newSimpleButton(null, tahoma);
            button.Location = new Location(400, 580);
            button.MultiPen = true;
            button.Pens.Add(DrawState.Click, new BrushStyle(BrushType.Horizontal,
                Rgba.Black, Rgba.Maroon, Rgba.Yellow, Rgba.GradientActiveCaption));
            Path.Add(button);
            button.MouseMove += (o, e) =>
            {
                Desktop.ShowToolTip(e.X + "," + e.Y, e.X, e.Y);
            };
#endif
            Path.Add(new RoundBox(400, 580, 300, 200, 25), @params);

#if Advance

            Path["RoundBox2"]?.AssignSpecialHover(moonfs, new Scale(1.25f), new Rotation(75));
#endif

            //var polygonList = new PolygonCollection();
            //polygonList.Add(new Box(20, 20, 300, 300));
            //polygonList.Add(new Triangle(50, 30, 10, 100, 255, 240));
            //polygonList.Parameters = new IDrawParameter[] {
            //    moonfs, new FillType(FillMode.Outer), new Stroke(10)};
            //Path.Add(polygonList);

            //Path["Pie1"].HoverSettings = new IDrawParameter[] { new Scale(1.50f) };
            //Path["SimpleButton1"].HoverSettings = new IDrawParameter[] { new Scale(2.25f) };
        }

        #if Advance
        private static void Desktop_LoadAnimations(object sender, IEventArgs e)
        {
            var circular = Factory.newCircular(new Curve(0, 0, 500, 400), 250, 250, moonfs);
            //var lbl = Factory.newCircular(Factory.newSimpleLabel("Mukesh my friend", tahoma));
            var spriteSheet = Factory.newSpriteSheet(501, 401);

            spriteSheet.AddVariations(new Curve(0, 0, 500, 400), 
               ((IParameter[]) SkewType.Horizontal.GetStandardVariations()).AppendItem(rg1));
          
            Desktop.Animations.Add(circular);

            //Desktop.Animations.Add(lbl);
            //spriteSheet.SaveAs("C://users//maadh//desktop//spritesheet", ImageFormat.BMP);

            //Desktop.Animations.Add(Factory.newCircular(new RoundBox(0, 0, 300, 400, 30), 50, 50, rg));
            //Desktop.Animations.Add(Factory.newCircular(new Curve(0, 0, 300, 400), 50, 50, rg));
            //Desktop.Animations.Add(Factory.newCircular(lbl));
            //Desktop.Animations.Add(Factory.newCaret(330, 150, 100, 125, 500));
            //Desktop.Animations.Add(Factory.newCircular(new TextElement(tahoma, "MUKESH ADHVARYU"), 250, 250, rg1));
            //Desktop.Animations[0].Interval = 200;
            Desktop.Animations[1].AtBottom = true;
            //Desktop.Animations[1].Stop();

            //spriteSheet.DisplayLocation = new Vector(400, 200);
            circular.CycleComplete += Animations_AnimationCycleComplete;

        }
#endif
        private static void Animations_AnimationCycleComplete(object sender, IEventArgs<long> e)
        {
            Desktop.Properties.Set( new TextProperty ( "Animation loop completed in " + e.Args + " milliseconds"));
        }

        private static void Window_MouseUp(object sender, IMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                //window.Background = Rgba.Aqua;
                //window.Update();
            }
        }

        private static void Window_MouseDown(object sender, IMouseEventArgs e)
        {
            (sender as ITextDisplayer).Text = e.ToString();
        }
        private static void Window_PaintBackground(object sender, IDrawEventArgs e)
        {
            var surface = e.Renderer;
            if (surface == null)
                return;
            IImage img;
            img = Factory.newCanvas(300, 300);

            img.DrawEllipse(10, 10, 200, 200, rg);
            var area = new Rectangle(10, 10, 400, 400);
            img.DrawPolygon(new IParameter[] { rg }, 10, 10, 200, 200, 300, 30, 67, 78, 100, 56);
            //surface.DrawImage(img, 20, 20, area, Commands.BackBuffer);
            //surface.DrawImage(img, 600, 500, area, Commands.BackBuffer);
            surface.DrawImage(img, 500, 500, area, CopyCommand.Backdrop);

            //window.Objects.Hide(window.Objects["SimpleButton1"]);
            //window.Objects.Hide(window.Objects["Tetragon1"]);

            //settings.Command = Command.BackgroundBuffer;

            //Desktop.DrawPolygon(new IParameter[] { Factory.newSettings().Modify(rg, new GwsCommand(Commands.BackBuffer)) },
            //    34, 22, 56, 95, 118, 40, 58, 40, 182, 87, 65, 177, 65, 177,
            //    96, 28, 138, 84, 138, 84, 66, 66, 255, 46, 186, 42, 168, 21, 92, 110, 141, 143, 285, 89, 194, 187, 72, 153);
            ////7 Mukesh
            Desktop.DrawPolygon(new IParameter[] { rg }, 0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
                497, 83, 494, 84, 490, 85, 484, 86, 477, 87, 469, 88, 460, 89, 450, 90, 439, 91, 427,
                92, 414, 93, 400, 93, 475, 94, 480, 95, 485, 97, 490, 99, 495, 101, 500, 103, 495, 105,
                490, 107, 485, 108, 480, 110, 475, 110, 400, 112, 400, 112, 490, 113, 495, 115, 500, 117,
                497, 118, 493, 119, 488, 120, 482, 121, 475, 122, 467, 123, 458, 124, 448, 125, 437,
                126, 425, 127, 412, 129, 350, 129, 500, 129, 450, 140, 400, 130, 455, 140, 500, 141,
                494, 142, 487, 143, 479, 144, 470, 145, 460, 146, 450, 167, 450, 167, 443, 165, 431,
                163, 417, 161, 405, 159, 402, 157, 400, 155, 402, 153, 405, 151, 417, 149, 431, 147,
                443, 145, 449, 147, 458, 149, 470, 151, 484, 153, 495, 155, 500, 157, 500, 159, 500,
                161, 494, 163, 487, 165, 479, 167, 470, 169, 460, 171, 449, 173, 442, 175, 431, 177,
                418, 179, 407, 181, 404, 183, 402, 185, 404, 187, 407, 189, 418, 191, 431, 189, 424,
                187, 414, 185, 411, 183, 409, 181, 411, 179, 414, 177, 423, 175, 434, 177, 442, 179,
                446, 181, 449, 183, 452, 185, 456, 187, 464, 189, 475, 187, 488, 185, 499, 183, 502,
                181, 504, 179, 501, 177, 490, 175, 477, 177, 474, 179, 474, 181, 473, 183, 471, 185,
                468, 187, 464, 189, 459, 191, 453, 193, 446, 195, 438, 197, 429, 199, 419, 201, 408,
                203, 396, 205, 383, 207, 369, 209, 354, 209, 350, 209, 500, 210, 450, 212, 438, 214,
                425, 216, 412, 218, 405, 220, 402, 222, 400, 224, 403, 226, 410, 228, 422, 230, 433,
                232, 443, 232, 600, 0, 600, 0, 500);
        }
        private static void Window_KeyPress(object sender, IKeyPressEventArgs e)
        {
            Desktop.Properties.Set( new TextProperty ( e.KeyChar));
        }
    }
}
