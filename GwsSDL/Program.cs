/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MnM.GWS;

namespace GwsSDL
{
    public static class Program
    {
        static IDesktop Desktop;
        static BrushStyle rg, rg1, fs, fs1, MoonFs;
        static ITextureBrush textureBrush;
        static int i;
        static ITimer timer;
        static IFont Tahoma;
        static IWindow window, window1;
        static void Main()
        {
            //frame = true;            
            Factory.Attach<Instance>();
            rg = new BrushStyle(BrushType.Conical, Rgba.Red, Rgba.SkyBlue, Rgba.LightPink, Rgba.Green);
            rg1 = new BrushStyle(BrushType.Vertical, Rgba.Yellow, Rgba.Green, Rgba.Orange, Rgba.Olive);
            fs = new BrushStyle(BrushType.Conical, Rgba.Gold, Rgba.Orange, Rgba.Green, Rgba.Teal);
            fs1 = new BrushStyle(BrushType.Vertical, Rgba.Black, Rgba.Maroon);
            MoonFs = new BrushStyle(BrushType.Horizontal, Rgba.Red, Rgba.Green);
            textureBrush = Factory.newBrush(AppContext.BaseDirectory + "\\GWS.png");
            Tahoma = Factory.newFont("c:\\windows\\fonts\\Tahoma.ttf", 30);
            Tahoma.EnableKerning = true;
            Desktop = Factory.newDesktop(x: 10, y: 30, width: 1000, height: 1000, flags:
                GwsWindowFlags.ShowImmediate | GwsWindowFlags.MultiWindow | GwsWindowFlags.FullScreen);

            Desktop.PaintImages += Window_PaintBackground;
            Desktop.Load += Window_Load;
            Desktop.KeyPress += Window_KeyPress;
            Desktop.MouseDown += Window_MouseDown;
            Desktop.MouseUp += Window_MouseUp;
            Desktop.MouseMove += Window_MouseMove;
            Desktop.Resized += Window_Resized;

#if Advance
            Desktop.LoadAnimations += Desktop_LoadAnimations;
            Desktop.LoadVirtualWindows += Desktop_LoadWindows;
            Desktop.BackgroundContext = Rgba.LightYellow;
#endif
            //Factory.DefaultPens.Add(DrawState.Background, MoonFs);
            //Desktop.IsMultiWindow = true;
            //Desktop.Show();

            //var Desktop1 = Factory.newDesktop(x: 300, y: 300, width: 500, height: 500, flags:
            //GwsWindowFlags.Resizable | GwsWindowFlags.MultiWindow | GwsWindowFlags.ShowImmediate);

            //var timer = new Timer(10);
            //int i = 0, j = 0;
            //timer.Tick += (s, e) =>
            //{
            //    ++i;
            //    Desktop1.Text = "Count is" + i;
            //    if (++j > 1000)
            //        timer.Dispose();
            //};
            //timer.Start();
            //var Desktop2 = Factory.newDesktop(x: 20, y: 300, width: 500, height: 500, flags:
            //    GwsWindowFlags.Resizable | GwsWindowFlags.MultiWindow | GwsWindowFlags.ShowImmediate);
            //timer.Start();
            Application.Run();
        }
        private static void Window_Load(object sender, IEventArgs e)
        {
            var Path = Desktop.Controls;


            var star = Path.Add(new Polygon(167 + 200, 97, 110 + 200, 216, 248 + 200, 149, 107 + 200, 151, 218 + 200, 216),
                new BrushStyle(BrushType.Conical, Rgba.Green, Rgba.Orange, Rgba.Black),
                8.ToStroke(), FillCommand.FloodFill.Add());

#if Advance

            //star.State |= ObjAbility.HoverInvert;

            star.AssignSpecialHover(new Scale(1.25f),
                16.ToStroke(), (FillCommand.FloodFill | FillCommand.StrokeOuter).Add(), new Rotation(123), MoonFs);

#endif
            Path.Add(new Polygon(90, 215, 163, 29, 63, 202, 188, 46, 41, 182, 206, 70,
                26, 156, 217, 97, 20, 127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182,
                51, 46, 176, 202, 76, 29, 149, 215, 105, 21, 120, 220, 134, 21, 90, 215), rg1);

            Path.Add(new Triangle(148, 95, 141, 244, 254, 167), MoonFs);

            Path.Add(new Polygon(0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
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
                    232, 443, 232, 600, 0, 600, 0, 500), MoonFs);

            var parameters = new IParameter[] { rg, 28.ToStroke(), FillCommand.FillOddLines.Add() };
            //Path.Add(new Curve(600, 10, 300, 400), @params);
            //Path.Add(new Pie(500, 127, 300, 200, 0, 270, false), @params);

            //Path.Add(new Tetragon(425, 480, 650, 690, 190), @params);

            Path.Add(new Bezier(158, 181, 174, 348, 350, 363, 541, 145), parameters);

            Path.Add(new RoundBox(300, 300, 200, 200, 25), parameters);
            Path.Add(new Curve(300, 550, 300, 200, 55, 300), parameters);

            //Path.Add(new TextElement(Tahoma, "GWS - Drawing", 400, 570), MoonFs);
            var txto = new Text("WELCOME TO GWS DEMO." + System.Environment.NewLine +
                "DO YOU LIKE IT?" + System.Environment.NewLine + "LET US KNOW", 400, 580);
            var txt = Path.Add(txto, MoonFs, Tahoma);

#if Advance

            var frame = Factory.newFrame(100, 500, 500, 250, "Junk Food");
            //frame.Properties.Set(new Rotation(45));
            frame.Controls.Add(Factory.newSimpleButton("MUKESH"), Tahoma);
            frame.Controls.Add(Factory.newSimpleButton(0, 47, "JAYDEVI"), "MukFrame".ToName(), Tahoma);
            frame.Controls.Add(Factory.newSimpleButton(0, 83, "MANAN"), Tahoma);
            frame.Controls.Add(new Text("Mukesh Adhvaryu"), new Location(0, 125));
            frame.Controls.Add(new Curve(100, 400, 200, 100), new Location(40, 145));


            var frame1 = Factory.newFrame(50, 30, 200, 150, "Great Food");
            frame1.Properties.Set<IRotation>(new Rotation(30));

            frame1.Controls.Add(Factory.newSimpleButton("BANANA"), Tahoma);
            frame1.Controls.Add(Factory.newSimpleButton(0, 47, "ORANGE"), Tahoma);
            frame1.Controls.Add(Factory.newSimpleButton(0, 83, "APPLE"), Tahoma);
            frame1.Controls.Add(new Text("Nice menu!"), new Location(0, 125));


            Path.Add(frame);
            frame.Controls.Add(frame1);
            var frame2 = Factory.newFrame(1050, 130, 300, 150, "Vandan Adhvaryu");
            Path.Add(frame2);

            var button = Factory.newSimpleButton("Mukesh my friend", Tahoma);
            button.Location = new Location(400, 580);
            button.MultiPen = true;

            //button.Pens.Add(DrawState.Click, new BrushStyle(BrushType.Horizontal,
            //    Rgba.Black, Rgba.Maroon, Rgba.Yellow, Rgba.GradientActiveCaption));
            Path.Add(button);
            button.MouseClick += (o, e) =>
            {
                Desktop.Text = System.DateTime.Now.ToString();
            };
            //button.LostFocus += (o, e) =>
            //{
            //    //e.Cancel = true;
            //    //e.MessageTitle = "Invalid Operation";
            //};
            button.MouseMove += (o, e) =>
            {
                button.ShowToolTip(e.X + "," + e.Y, e.X, e.Y);
            };

            //ctrl.MultiPen = true;
            //ctrl.Pens.Add(DrawState.Default, new BrushStyle(BrushType.Horizontal,
            //    Rgba.Black, Rgba.Maroon, Rgba.Yellow, Rgba.GradientActiveCaption));
            //frame.Controls.Add(new Box(10,10,50,65), Rgba.Red);


#endif
            //var im = Factory.newCanvas("C://users//mukesh adhvaryu//desktop//star.png", true);
            //Path.Add(im);

            //Path["Trapezium1"].LostFocus += LostFocus;
            //Path.Add(new RoundBox(400, 580, 300, 200, 25), @params);

            //var polygonList = new PolygonCollection();
            //polygonList.Add(new Box(20, 20, 300, 300));
            //polygonList.Add(new Triangle(50, 30, 10, 100, 255, 240));
            //polygonList.Parameters = new IParameter[] {
            //    MoonFs, FillMode.Outer.Add(), PolyState.FloodFill.Add(), 10.ToStroke()};
            //Path.Add(polygonList);

            //Path["Pie1"]?.AssignSpecialHover( new Scale(1.50f) );
            //Path["Polygon3"]?.AssignSpecialHover(new Box(100, 100, 100, 200));

            //timer = Factory.newTimer(8);
            //timer.Tick += (s, e) =>
            //{
            //    Desktop.Set(new Text( i++));
            //};
            //timer.Start();
            //Path["SimpleButton4"]?.AssignSpecialHover(new Scale(2.25f) );
            //Desktop.SetProperty(Factory.SystemFont);
        }
#if Advance
        private static void Desktop_LoadWindows(object sender, IEventArgs e)
        {
            window1 = Desktop.Windows.Add("Manan", 30, 30, 400, 400, null, Rgba.Aqua);

            window1.Controls.Add(new Triangle(100, 10, 160, 140, 56, 300), MoonFs);
            window1.Controls.Add(new Polygon(90, 215, 163, 29, 63, 202,
               188, 46, 41, 182, 206, 70, 26, 156, 217, 97, 20,
               127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182, 51,
               46, 176, 202, 76, 29, 149, 215, 105,
               21, 120, 220, 134, 21, 90, 215), rg1, Command.OriginalFill.Add());
            window1.Controls.Add(Factory.newSimpleButton(150, 100, null, Tahoma));

            window = Desktop.Windows.Add("Mukesh", 242, 60, 400, 400, WindowAbility.Transparent);
            //window.BackgroundContext = rg1;
            //window.Controls.Add(new Curve(new VectorF(280, 0), new VectorF(100, 140), new VectorF(300, 45)), rg1);
            //window.Controls.Add(Factory.newSimpleButton(200, 70, font: Tahoma));

            window.Controls.Add(new Triangle(40 + 210, 10, 100 + 210, 140, 56 + 210, 300),
                Rgba.Green);
            window.Controls.Add(new Curve(224, 100, 200, 400), MoonFs);
            //window.Controls["Triangle5"].HoverSettings = new IDrawParameter[] { new Scale(1.25f), new Rotation(75) };

            //window1.MouseDragEnd += Window1_MouseDragEnd;
            //window1.DragEnded += Window1_MouseDragEnd;
            Desktop.DragEnded += Desktop_MouseDragEnd;

            var window2 = Desktop.Windows.Add("Jaydevi", 500, 90, 400, 400);
            window2.Closing += (s, e) =>
            {
                e.MessageText = "Cant Close";
                e.MessageTitle = "Insanity!";
                e.Cancel = true;
            };
            window2.Controls.Add(new Triangle(100, 10, 160, 140, 56, 300), MoonFs);
            window2.Controls.Add(new Polygon(90, 215, 163, 29, 63, 202, 188, 46, 41, 182, 206, 70, 26, 156, 217, 97, 20,
               127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182, 51, 46, 176, 202, 76, 29, 149, 215, 105,
               21, 120, 220, 134, 21, 90, 215), rg1, FillCommand.OriginalFill.Add());
            //var SB = Factory.newSimpleButton(150, 100, null, Tahoma);
            //window2.Controls.Add(SB);
            //window2.Controls.Add(Factory.newCanvas("C://users//mukesh adhvaryu//desktop//star.png", true));

            window.Show();
            window1.Show();
            window2.Show();
            //window1.GotFocus += Window1_GotFocus;
            //window.LostFocus += LostFocus;
            //window.Controls["Triangle4"].LostFocus += LostFocus;
        }

#endif
        private static void GotFocus(object sender, ICancelEventArgs e)
        {
            //e.Cancel = true; 
        }
        private static void LostFocus(object sender, ICancelEventArgs e)
        {
            //e.Cancel = true;
        }
#if Advance
        private static void Desktop_LoadAnimations(object sender, IEventArgs e)
        {
            //var image = Factory.newCanvas(100, 100);
            //var parameters = new IParameter[]{ new BrushStyle(BrushType.Conical, Rgba.Green, Rgba.Orange, Rgba.Black),
            //     new Degree(-45), Command.FullView.Add() };
            //image.Draw(Desktop.Controls["Polygon3"], parameters);
            //image.Draw(new Polygon(167 + 200, 97, 110 + 200, 216, 248 + 200, 149, 107 + 200, 151, 218 + 200, 216), parameters.AppendItem(8.ToStroke()));

            //image.Draw(new Curve(50, 50, 300, 200, 0, 133, CurveType.Full), parameters);

            //var s1 = Factory.newSimpleLabel("Mukesh my friend", Tahoma);
            ////s1.Location = new Point(400, 580);
            //image.Draw(s1, Command.FullView.Add(), DrawState.Redraw.Add(), new Skew(120,SkewType.Diagonal),
            //    new Scale(1.2f, 1.4f));
            //image.SaveAs("C://users//maadh//desktop//fullView", ImageFormat.PNG.ToParameter(),
            //    Command.SwapRedBlueChannel.Add());
            ICircular circular;

            circular = Factory.newCircular(new Curve(1, 1, 500, 400, type: CurveType.Full), 250, 250, MoonFs);
            circular.CycleComplete += Animations_AnimationCycleComplete;

            //circular.Interval = 1000;
            //circular = Factory.newCircular(Factory.newCanvas("C:\\users\\maadh\\desktop\\sprite.png"), 250, 300);

            //circular = Factory.newCircular(new Curve(0, 0, 500, 400), 250, 250, MoonFs);
            //circular = Factory.newCircular(Factory.newSimpleLabel("Mukesh my friend", Tahoma));
            //var spriteSheet = Factory.newSkewer(new Ellipse(0, 0, 500, 400), 100, 100, SkewType.Diagonal, 1f, rg1);
            //((ICircular)circular).Step = 3f;
            //spriteSheet.Location = new Vector(200, 200);
            Desktop.Animations.Add(circular);//, spriteSheet);
                                             //Desktop.Animations.Add(lbl);
                                             //spriteSheet.SaveAs("C://users//maadh//desktop//spritesheet", ImageFormat.BMP);

            //Desktop.Animations.Add(Factory.newCircular(new RoundBox(0, 0, 300, 400, 30), 50, 50, rg));
            //Desktop.Animations.Add(Factory.newCircular(new Curve(0, 0, 300, 400), 50, 50, rg));
            //Desktop.Animations.Add(Factory.newCircular(lbl));
            //Desktop.Animations.Add(Factory.newCaret(330, 150, 100, 125, 500));
            //Desktop.Animations.Add(Factory.newCircular(new TextElement(Tahoma, "MUKESH ADHVARYU"), 250, 250, rg1));

            Desktop.Animations[0].AtBottom = true;

            //Desktop.Animations[0].AtBackdrop = true;
            //Desktop.Animations[1].AtBottom = true;
            //Desktop.Animations[1].Stop();

            //spriteSheet.DisplayLocation = new Vector(400, 200);
        }
#endif
        private static void Window_PaintBackground(object sender, IDrawEventArgs e)
        {
            if (e.Renderer == null)
                return;
            IImage img;
            //var result = Desktop.ShowMessageBox("MessageBox Demo", 200, 200, "Click any button below!", MsgBoxButtons.YesNo);

            //img = Factory.newCanvas(300, 300);
            //img.DrawEllipse(10, 10, 200, 200, MoonFs);

            //img.DrawPolygon(new IParameter[] { rg }, 10, 10, 200, 200, 300, 30, 67, 78, 100, 56);
            //var area = new Rectangle(10, 10, 400, 400);

            //var updatearea = e.Renderer.DrawImage(img, new Rectangle(20, 20, 300, 300), new Point(20, 20),
            //    (CopyCommand.CopyOpaque));
            //e.Session.Boundaries.Add(updatearea);
            //var button = Desktop.Controls.OfType<ISimpleButton>().First();
            //Desktop.Clear(new IParameter[] { new ID(button.ID), button.DrawnBounds });

            //e.ModifyParameters(new IParameter[] { Command.UpdateScreenOnly.Add(), new UpdateRect(10, 10, 400, 400) });
            //e.Modify(new IParameter[] { Command.UpdateScreenOnly.Add(), new TypeRect(20, 20, 400, 400) });

            //surface.DrawImage(img, 600, 500, area, Commands.Backdrop | Commands.BackBuffer);
            //surface.DrawImage(img, 500, 500, area, Commands.Backdrop | Commands.BackBuffer);

            //Desktop.Controls.Hide("Trapezium1");
            //window.Controls.Remove("Trapezium1");
            //window.Controls["Polygon1"] = new Curve(290,90, 400, 300);
            //window.Controls.Swap("SimpleButton1", "Trapezium1");
            //window.Controls.Relocate(0, "Trapezium1");

            //window.DrawPolygon(new IDrawParameter[] { rg1 }, 34, 22, 56, 95, 118, 40, 58, 40, 182, 87, 65, 177, 65, 177,
            //    96, 28, 138, 84, 138, 84, 66, 66, 255, 46, 186, 42, 168, 21, 92, 110, 141, 143, 285, 89, 194, 187, 72, 153);

            ////7 Mukesh
            //Desktop.DrawPolygon(new IDrawParameter[] { rg1 }, 0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
            //    497, 83, 494, 84, 490, 85, 484, 86, 477, 87, 469, 88, 460, 89, 450, 90, 439, 91, 427,
            //    92, 414, 93, 400, 93, 475, 94, 480, 95, 485, 97, 490, 99, 495, 101, 500, 103, 495, 105,
            //    490, 107, 485, 108, 480, 110, 475, 110, 400, 112, 400, 112, 490, 113, 495, 115, 500, 117,
            //    497, 118, 493, 119, 488, 120, 482, 121, 475, 122, 467, 123, 458, 124, 448, 125, 437,
            //    126, 425, 127, 412, 129, 350, 129, 500, 129, 450, 140, 400, 130, 455, 140, 500, 141,
            //    494, 142, 487, 143, 479, 144, 470, 145, 460, 146, 450, 167, 450, 167, 443, 165, 431,
            //    163, 417, 161, 405, 159, 402, 157, 400, 155, 402, 153, 405, 151, 417, 149, 431, 147,
            //    443, 145, 449, 147, 458, 149, 470, 151, 484, 153, 495, 155, 500, 157, 500, 159, 500,
            //    161, 494, 163, 487, 165, 479, 167, 470, 169, 460, 171, 449, 173, 442, 175, 431, 177,
            //    418, 179, 407, 181, 404, 183, 402, 185, 404, 187, 407, 189, 418, 191, 431, 189, 424,
            //    187, 414, 185, 411, 183, 409, 181, 411, 179, 414, 177, 423, 175, 434, 177, 442, 179,
            //    446, 181, 449, 183, 452, 185, 456, 187, 464, 189, 475, 187, 488, 185, 499, 183, 502,
            //    181, 504, 179, 501, 177, 490, 175, 477, 177, 474, 179, 474, 181, 473, 183, 471, 185,
            //    468, 187, 464, 189, 459, 191, 453, 193, 446, 195, 438, 197, 429, 199, 419, 201, 408,
            //    203, 396, 205, 383, 207, 369, 209, 354, 209, 350, 209, 500, 210, 450, 212, 438, 214,
            //    425, 216, 412, 218, 405, 220, 402, 222, 400, 224, 403, 226, 410, 228, 422, 230, 433,
            //    232, 443, 232, 600, 0, 600, 0, 500);

            //Desktop.Controls.Hide("TextElement1");
            //Desktop.Controls.Show("TextElement2");

            //Desktop.Controls.Replace("Pie1", Factory.newShape(new Box(400, 100, 300, 300), 
            //    null, rg1, FillType.FillOutLine, Stroke.Twelve));

            //Desktop.Controls.Relocate("Pie1", 2);


            //Desktop.Update(null);
            //Desktop.Clear(Desktop.Controls["Pie1"].DrawnBounds, ClearCommand.Hide);
            //Desktop.Update(Desktop.Controls["Pie1"].DrawnBounds, Command.UpdateScreenOnly);
            //e.Cancel = true;
            //Desktop.Clear(Desktop.Controls["TextElement2"].DrawnBounds, ClearCommand.KillPixels);
            //Desktop.Controls.Erase("Trapezium1", null, EraseCommand.Hide);
            //Desktop.Controls.Move("SimpleButton1", 300, 100);
            //Desktop.Controls.Skin("Pie1", MoonFs, SkinCommand.Display);
            //var result = Desktop.ShowMessageBox("Hello", 330, 330, "Graphics & Windowing System");

        }
        private static void Window_KeyPress(object sender, IKeyPressEventArgs e)
        {
            Desktop.Properties.Set(new TextProperty(e.KeyChar.ToString(), ModifyCommand.Remove));
        }
        private static void Desktop_MouseDragEnd(object sender, IDragEventArgs e)
        {
            int ii = 0;
        }

        private static void Window1_MouseDragEnd(object sender, IDragEventArgs e)
        {
            int ii = 0;
        }

        private static void Animations_AnimationCycleComplete(object sender, IEventArgs<long> e)
        {
            //timer?.Stop();
            Desktop.Text = "Animation loop completed in " + e.Args + " milliseconds";
        }

        private static void Window_Resized(object sender, ISizeEventArgs e)
        {
        }

        private static void Window_MouseMove(object sender, IMouseEventArgs e)
        {
            //Desktop.ShowToolTip(Desktop.Hover + ", " + e, e.X, e.Y, 3000);
        }

        private static void Window_MouseUp(object sender, IMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                //timer.Run(!timer.IsRunning);
            }
        }


        private static void Window_MouseDown(object sender, IMouseEventArgs e)
        {
            (sender as IDesktop).Properties.Set(new TextProperty(e));
        }
    }
}
