/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;

using MnM.GWS;

using static MnM.GWS.Application;

namespace Test
{
    public static class Program
    {
        static IWindow window;
        static bool? frame = null;
        static int j = 1000;
        static BrushStyle rg, rg1, fs, fs1, moonfs;
        static ITextureBrush textureBrush;
        static int X, Y;
        static IFont tahoma;
        static ISettings Settings;
        static void Main()
        {
            //frame = true;
            Attach(NativeFactory.Instance);
            rg = new BrushStyle(BrushType.Conical, Rgba.Red, Rgba.SkyBlue, Rgba.LightPink, Rgba.Green);
            rg1 = new BrushStyle(BrushType.Vertical, Rgba.Yellow, Rgba.Green, Rgba.Orange, Rgba.Olive);
            fs = new BrushStyle(BrushType.Conical, Rgba.Gold, Rgba.Orange, Rgba.Green, Rgba.Teal);
            fs1 = new BrushStyle(BrushType.Vertical, Rgba.Black, Rgba.Maroon);
            moonfs = new BrushStyle(BrushType.BackwardDiagonal, Rgba.Red, Rgba.Green);
            textureBrush = Factory.newBrush(AppDomain.CurrentDomain.BaseDirectory + "\\GWS.png");
            tahoma = Factory.newFont("c:\\windows\\fonts\\tahoma.ttf", 40);
            Settings = Factory.newSettings();
            /*If you want direct rendering for popups, add flags:GwsWindowFlags.EnableDirectRender;
             */
            window = Factory.newWindow(width: 1000, height: 900, flags: GwsWindowFlags.Resizable);

            //graphics.Window = window;
            //window.Transparency = .5f;
            window.Paint += Window_PaintBackground;
            window.FirstShown += Window_Load;
            window.KeyPress += Window_KeyPress;
            window.MouseDown += Window_MouseDown;
            window.MouseUp += Window_MouseUp;
            window.MouseMove += Window_MouseMove;
            window.Background = moonfs;

            IPenContext brush = null;
            var Path = window.Objects;
            //Renderer.Settings.InvertBrushColor = true;
            //window.InvertColor = true;
            Path.Add(Factory.newPolygon(90, 215, 163, 29, 63, 202,
                 188, 46, 41, 182, 206, 70, 26, 156, 217, 97, 20,
                 127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182, 51,
                 46, 176, 202, 76, 29, 149, 215, 105,
                 21, 120, 220, 134, 21, 90, 215), Settings);
            Path.Add(Factory.newCurve(600, 10, 300, 400));
            //Renderer.Settings.InvertBrushColor = false;

            Settings.Stroke = 23;
            Settings.FillMode = FillMode.FillOutLine;

            Path.Add(Factory.newBezier(158, 181, 174, 348, 350, 363, 541, 145));

            Path.Add(Factory.newTetragon(425, 480, 650, 690, 190), Settings);


            Path.Add(Factory.newText(tahoma, "GWS - Drawing", 576, 507));

            Path.Add(Factory.newRoundBox(300, 300, 200, 200, 25));
            Path.Add(Factory.newTriangle(20, 300, 200, 350, 200, 467));

            Path.Add(Factory.newCurve(100, 500, 300, 200, 55, 300, CurveType.Pie));
            window.Show();
            
            //w1.Show();
            Application.Run();
        }

        private static void Window_MouseMove(object sender, IMouseEventArgs e)
        {
        }

        private static void Window_MouseUp(object sender, IMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                //window.Background = Rgba.Aqua;
                //window.Update();
            }
        }

        private static void Window_Load(object sender, IEventArgs e)
        {
            var timer = new Timer(50);
            timer.Tick += (s, eV) =>
             {
                 window.Background = new Rgba(1000 + j);
                 Factory.SystemFont.Size = 20;
                 Settings.PenContext = textureBrush;
                 Program.window.Text = Benchmarks.Execute((() =>
                 {
                     float sin, cos;
                     for (int i = 0; i <= 360; i++)
                     {
                         Angles.SinCos(i, out sin, out cos);
                         X = 200 + (int)(200 * sin);
                         Y = 200 + (int)(200 * cos);
                         window.DrawEllipse(X, Y, 500, 400, Settings);
                         //break;
                     };
                 }), "Shape frame drawing");
                 j += 1000;
             };
            timer.Start();
        }

        private static void Window_MouseDown(object sender, IMouseEventArgs e)
        {
            (sender as IWindow).Text = e.ToString();
        }
        private static void Window_PaintBackground(object sender, IDrawEventArgs e)
        {
            var surface = e.Graphics;
            if (surface == null)
                return;

            if (frame == true)
            {
                int size = 400;
                int offset = 2;
                int offset2 = offset * 4;
                var ellipse = Factory.newCurve(0, 0, 500, 400);
                Rectangle rc;
                int x = 0, y = 0;
                var img = Factory.newImage(300, 300);

                Settings.Command = Command.Backdrop;
                img.DrawEllipse(10, 10, 200, 200);
                surface.DrawImage(img, 20, 20, 10, 10, 400, 400);
                surface.DrawImage(img, 600, 500, 10, 10, 400, 400);
                surface.DrawImage(img, 500, 500, 10, 10, 400, 400);
                Settings.Command = 0;
            }
            else if (frame == false)
            {
                Program.window.Text = Benchmarks.Execute((() =>
                {
                    //////triple breasted bra
                    surface.DrawPolygon(0, 500, 50, 600, 100, 550, 150, 600, 200, 550, 250, 600, 300, 500, 0, 500);

                    ////pickachoo1
                    surface.DrawPolygon(400, 10 + 300, 450, 200 + 300, 250, 125 + 300, 300, 225 + 300, 67, 176 + 300, 350, 80 + 300);

                    ////angry pickachoo1
                    surface.DrawPolygon(400, 10, 450, 200, 250, 125, 228, 150, 300, 225, 67, 176, 350, 80);

                    ////hot bra
                    surface.DrawPolygon(0, 500, 50, 600, 100, 550, 150, 600, 200, 600, 250, 550, 300, 600, 350, 500, 0, 500);


                    ////bra
                    surface.DrawPolygon(0, 60, 10, 80, 20, 70, 30, 80, 40, 60, 0, 60);

                    ////regular polygon
                    surface.DrawPolygon(50, 20, 50, 30, 60, 40, 70, 50, 80, 50, 90, 40, 100, 30, 100, 20, 90, 10, 80, 0, 70, 0, 60, 10, 50, 20);

                    //////star
                    surface.DrawPolygon(10, 0, 20, 50, 30, 0, 0, 30, 40, 30, 10, 0, 10, 0);

                    ////phaser1
                    surface.DrawPolygon(60, 60, 60, 100, 100, 100, 61, 90, 60, 80, 61, 70, 100, 60, 60, 60);

                    ////phaser2 - buggy
                    surface.DrawPolygon(60, 60, 60, 120, 100, 100, 61, 110, 60, 90, 61, 70, 100, 80, 60, 60);

                    //////bra
                    surface.DrawPolygon(0, 60, 10, 80, 20, 70, 30, 80, 40, 60, 0, 60);

                    ////big bra
                    surface.DrawPolygon(0, 500, 100, 700, 200, 600, 300, 700, 400, 500, 0, 500);

                    ////small squarish regular polygon
                    surface.DrawPolygon(50, 20, 50, 30, 60, 40, 70, 50, 80, 50, 90, 40, 100, 30, 100, 20, 90, 10, 80, 0, 70, 0, 60, 10, 50, 20);

                    //////small buggy phaser
                    surface.DrawPolygon(60, 60, 60, 120, 100, 100, 61, 110, 60, 90, 61, 70, 100, 80, 60, 60);

                    ///////big buggy big sucking phaser3
                    surface.DrawPolygon(500, 500, 500, 940, 665, 720, 501, 830, 500, 720, 501, 610, 720, 720, 500, 500);

                    ////big regular poygon
                    surface.DrawPolygon(400, 160, 400, 240, 440, 320, 560, 400, 640, 400, 760, 320, 800, 240, 800, 160, 760, 80, 640, 0, 560, 0, 440, 80, 400, 160);

                    /////small star
                    surface.DrawPolygon(10, 0, 20, 50, 30, 0, 0, 30, 40, 30, 10, 0, 10, 0);

                    ////big star
                    surface.DrawPolygon(80, 10, 160, 410, 240, 10, 0, 250, 320, 250);

                    surface.DrawPolygon(90, 215, 163, 29, 63, 202, 188, 46, 41, 182, 206, 70, 26, 156, 217, 97, 20,
                        127, 219, 127, 22, 97, 213, 156, 33, 70, 198, 182, 51, 46, 176, 202, 76, 29, 149, 215, 105,
                        21, 120, 220, 134, 21, 90, 215);

                    surface.DrawPolygon(0, 0, 10, 5, 15, 10, 10, 15, 15, 20, 10, 25, 15, 30, 10, 35, 15, 40,
                        10, 45, 15, 50, 10, 55, 15, 60, 10, 65, 15, 70, 10, 75, 15, 80, 10, 85, 15, 90,
                        10, 95, 15, 100, 10, 105, 15, 110, 10, 115, 15, 120, 10, 125, 15, 130, 10, 135, 15,
                        140, 10, 145, 15, 150, 10, 155, 15, 160, 10, 165, 15, 170, 10, 175, 15, 180, 10, 185,
                        15, 190, 10, 195, 15, 200, 10, 205, 15, 210, 10, 215, 15, 220, 10, 225, 0, 0);

                    surface.DrawPolygon(206, 124, 26, 37, 217, 96, 20, 66, 219, 66, 22, 96, 213, 37, 33, 124, 15, 119, 10, 114, 15, 109, 10, 104, 15, 99, 10, 94, 15, 89, 10, 84, 15, 79, 10, 74, 15, 69, 10, 64, 15, 59, 10, 54, 15, 49, 10, 44, 15, 39, 10, 34, 15, 29, 10, 24, 0, 19, 0, 190, 220, 190, 206, 124);

                    ////Test 6 Interesctions big
                    surface.DrawPolygon(Settings, 161, 411, 306, 39, 107, 385, 356, 73, 63, 344, 393,
                         120, 33, 293, 414, 175, 20, 234, 419, 234, 25, 175, 406, 293, 46, 120,
                         376, 344, 83, 73, 332, 385, 133, 39, 278, 411, 190, 22, 220, 420, 249,
                         22, 161, 411, 306, 39, 107, 385, 356, 73, 63, 344, 393, 120, 33, 293,
                         414, 175, 20, 234, 419, 234, 25, 175, 406, 293, 46, 120, 376, 344, 83,
                         73, 332, 385, 133, 39, 278, 411, 190, 22, 220, 420, 249, 22);


                    //7 Mukesh
                    surface.DrawPolygon(Settings, 0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
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
                }));
            }
            else
            {
                var img = Factory.newImage(300, 300);
                img.DrawEllipse(10, 10, 200, 200);
                //surface.DrawImage(img, 20, 20, 10, 10, 400, 400);
                //surface.DrawImage(img, 600, 500, 10, 10, 400, 400);
                //surface.DrawImage(img, 500, 500, 10, 10, 300, 300);

#if Advanced
                //window.Controls.Hide(window.Controls["Text1"]);
                //window.Controls.Hide(window.Controls["Tetragon1"]);
                //window.DrawMode = DrawMode.NoBkgDraw;
                //window.DrawMode = DrawMode.NoBkgDraw | DrawMode.NoControlDraw;
                //window.Controls.Disable(shape);
                //window.DrawFocusRect(window.Controls["Text1"]);

#endif
                ////7 Mukesh
                window.DrawPolygon(Factory.newSettings( rg), 0, 500, 0, 300, 20, 300, 40, 500, 60, 300, 80, 300, 80, 500, 81, 499, 82,
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
        }
        private static void Window_KeyPress(object sender, IKeyPressEventArgs e)
        {
            window.Text = e.KeyChar.ToString();
        }
    }
}
