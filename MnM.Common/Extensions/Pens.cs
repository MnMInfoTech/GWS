/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    public static partial class Pens
    {
        #region RESET
        internal static void Reset()
        {
            Rgba.Reset();
            BrushStyle.Reset();
            Empty = Factory.ToPen(Rgba.Empty);
            ActiveBorder = Factory.ToPen(Rgba.ActiveBorder);
            ActiveCaption = Factory.ToPen(Rgba.ActiveCaption);
            ActiveCaptionText = Factory.ToPen(Rgba.ActiveCaptionText);
            AppWorkspace = Factory.ToPen(Rgba.AppWorkspace);
            Control = Factory.ToPen(Rgba.Control);
            ControlDark = Factory.ToPen(Rgba.ControlDark);
            ControlDarkDark = Factory.ToPen(Rgba.ControlDarkDark);
            ControlLight = Factory.ToPen(Rgba.ControlLight);
            ControlLightLight = Factory.ToPen(Rgba.ControlLightLight);
            ControlText = Factory.ToPen(Rgba.ControlText);
            Desktop = Factory.ToPen(Rgba.Desktop);
            GrayText = Factory.ToPen(Rgba.GrayText);
            Highlight = Factory.ToPen(Rgba.Highlight);
            HighlightText = Factory.ToPen(Rgba.HighlightText);
            HotTrack = Factory.ToPen(Rgba.HotTrack);
            InactiveBorder = Factory.ToPen(Rgba.InactiveBorder);
            InactiveCaption = Factory.ToPen(Rgba.InactiveCaption);
            InactiveCaptionText = Factory.ToPen(Rgba.InactiveCaptionText);
            Info = Factory.ToPen(Rgba.Info);
            InfoText = Factory.ToPen(Rgba.InfoText);
            Menu = Factory.ToPen(Rgba.Menu);
            MenuText = Factory.ToPen(Rgba.MenuText);
            ScrollBar = Factory.ToPen(Rgba.ScrollBar);
            Window = Factory.ToPen(Rgba.Window);
            WindowFrame = Factory.ToPen(Rgba.WindowFrame);
            WindowText = Factory.ToPen(Rgba.WindowText);
            Transparent = Factory.ToPen(Rgba.Transparent);
            AliceBlue = Factory.ToPen(Rgba.AliceBlue);
            AntiqueWhite = Factory.ToPen(Rgba.AntiqueWhite);
            Aqua = Factory.ToPen(Rgba.Aqua);
            Aquamarine = Factory.ToPen(Rgba.Aquamarine);
            Azure = Factory.ToPen(Rgba.Azure);
            Beige = Factory.ToPen(Rgba.Beige);
            Bisque = Factory.ToPen(Rgba.Bisque);
            Black = Factory.ToPen(Rgba.Black);
            BlanchedAlmond = Factory.ToPen(Rgba.BlanchedAlmond);
            Blue = Factory.ToPen(Rgba.Blue);
            BlueViolet = Factory.ToPen(Rgba.BlueViolet);
            Brown = Factory.ToPen(Rgba.Brown);
            BurlyWood = Factory.ToPen(Rgba.BurlyWood);
            CadetBlue = Factory.ToPen(Rgba.CadetBlue);
            Chartreuse = Factory.ToPen(Rgba.Chartreuse);
            Chocolate = Factory.ToPen(Rgba.Chocolate);
            Coral = Factory.ToPen(Rgba.Coral);
            CornflowerBlue = Factory.ToPen(Rgba.CornflowerBlue);
            Cornsilk = Factory.ToPen(Rgba.Cornsilk);
            Crimson = Factory.ToPen(Rgba.Crimson);
            Cyan = Factory.ToPen(Rgba.Cyan);
            DarkBlue = Factory.ToPen(Rgba.DarkBlue);
            DarkCyan = Factory.ToPen(Rgba.DarkCyan);
            DarkGoldenrod = Factory.ToPen(Rgba.DarkGoldenrod);
            DarkGray = Factory.ToPen(Rgba.DarkGray);
            DarkGreen = Factory.ToPen(Rgba.DarkGreen);
            DarkKhaki = Factory.ToPen(Rgba.DarkKhaki);
            DarkMagenta = Factory.ToPen(Rgba.DarkMagenta);
            DarkOliveGreen = Factory.ToPen(Rgba.DarkOliveGreen);
            DarkOrange = Factory.ToPen(Rgba.DarkOrange);
            DarkOrchid = Factory.ToPen(Rgba.DarkOrchid);
            DarkRed = Factory.ToPen(Rgba.DarkRed);
            DarkSalmon = Factory.ToPen(Rgba.DarkSalmon);
            DarkSeaGreen = Factory.ToPen(Rgba.DarkSeaGreen);
            DarkSlateBlue = Factory.ToPen(Rgba.DarkSlateBlue);
            DarkSlateGray = Factory.ToPen(Rgba.DarkSlateGray);
            DarkTurquoise = Factory.ToPen(Rgba.DarkTurquoise);
            DarkViolet = Factory.ToPen(Rgba.DarkViolet);
            DeepPink = Factory.ToPen(Rgba.DeepPink);
            DeepSkyBlue = Factory.ToPen(Rgba.DeepSkyBlue);
            DimGray = Factory.ToPen(Rgba.DimGray);
            DodgerBlue = Factory.ToPen(Rgba.DodgerBlue);
            Firebrick = Factory.ToPen(Rgba.Firebrick);
            FloralWhite = Factory.ToPen(Rgba.FloralWhite);
            ForestGreen = Factory.ToPen(Rgba.ForestGreen);
            Fuchsia = Factory.ToPen(Rgba.Fuchsia);
            Gainsboro = Factory.ToPen(Rgba.Gainsboro);
            GhostWhite = Factory.ToPen(Rgba.GhostWhite);
            Gold = Factory.ToPen(Rgba.Gold);
            Goldenrod = Factory.ToPen(Rgba.Goldenrod);
            Gray = Factory.ToPen(Rgba.Gray);
            Green = Factory.ToPen(Rgba.Green);
            GreenYellow = Factory.ToPen(Rgba.GreenYellow);
            Honeydew = Factory.ToPen(Rgba.Honeydew);
            HotPink = Factory.ToPen(Rgba.HotPink);
            IndianRed = Factory.ToPen(Rgba.IndianRed);
            Indigo = Factory.ToPen(Rgba.Indigo);
            Ivory = Factory.ToPen(Rgba.Ivory);
            Khaki = Factory.ToPen(Rgba.Khaki);
            Lavender = Factory.ToPen(Rgba.Lavender);
            LavenderBlush = Factory.ToPen(Rgba.LavenderBlush);
            LawnGreen = Factory.ToPen(Rgba.LawnGreen);
            LemonChiffon = Factory.ToPen(Rgba.LemonChiffon);
            LightBlue = Factory.ToPen(Rgba.LightBlue);
            LightCoral = Factory.ToPen(Rgba.LightCoral);
            LightCyan = Factory.ToPen(Rgba.LightCyan);
            LightGoldenrodYellow = Factory.ToPen(Rgba.LightGoldenrodYellow);
            LightGray = Factory.ToPen(Rgba.LightGray);
            LightGreen = Factory.ToPen(Rgba.LightGreen);
            LightPink = Factory.ToPen(Rgba.LightPink);
            LightSalmon = Factory.ToPen(Rgba.LightSalmon);
            LightSeaGreen = Factory.ToPen(Rgba.LightSeaGreen);
            LightSkyBlue = Factory.ToPen(Rgba.LightSkyBlue);
            LightSlateGray = Factory.ToPen(Rgba.LightSlateGray);
            LightSteelBlue = Factory.ToPen(Rgba.LightSteelBlue);
            LightYellow = Factory.ToPen(Rgba.LightYellow);
            Lime = Factory.ToPen(Rgba.Lime);
            LimeGreen = Factory.ToPen(Rgba.LimeGreen);
            Linen = Factory.ToPen(Rgba.Linen);
            Magenta = Factory.ToPen(Rgba.Magenta);
            Maroon = Factory.ToPen(Rgba.Maroon);
            MediumAquamarine = Factory.ToPen(Rgba.MediumAquamarine);
            MediumBlue = Factory.ToPen(Rgba.MediumBlue);
            MediumOrchid = Factory.ToPen(Rgba.MediumOrchid);
            MediumPurple = Factory.ToPen(Rgba.MediumPurple);
            MediumSeaGreen = Factory.ToPen(Rgba.MediumSeaGreen);
            MediumSlateBlue = Factory.ToPen(Rgba.MediumSlateBlue);
            MediumSpringGreen = Factory.ToPen(Rgba.MediumSpringGreen);
            MediumTurquoise = Factory.ToPen(Rgba.MediumTurquoise);
            MediumVioletRed = Factory.ToPen(Rgba.MediumVioletRed);
            MidnightBlue = Factory.ToPen(Rgba.MidnightBlue);
            MintCream = Factory.ToPen(Rgba.MintCream);
            MistyRose = Factory.ToPen(Rgba.MistyRose);
            Moccasin = Factory.ToPen(Rgba.Moccasin);
            NavajoWhite = Factory.ToPen(Rgba.NavajoWhite);
            Navy = Factory.ToPen(Rgba.Navy);
            OldLace = Factory.ToPen(Rgba.OldLace);
            Olive = Factory.ToPen(Rgba.Olive);
            OliveDrab = Factory.ToPen(Rgba.OliveDrab);
            Orange = Factory.ToPen(Rgba.Orange);
            OrangeRed = Factory.ToPen(Rgba.OrangeRed);
            Orchid = Factory.ToPen(Rgba.Orchid);
            PaleGoldenrod = Factory.ToPen(Rgba.PaleGoldenrod);
            PaleGreen = Factory.ToPen(Rgba.PaleGreen);
            PaleTurquoise = Factory.ToPen(Rgba.PaleTurquoise);
            PaleVioletRed = Factory.ToPen(Rgba.PaleVioletRed);
            PapayaWhip = Factory.ToPen(Rgba.PapayaWhip);
            PeachPuff = Factory.ToPen(Rgba.PeachPuff);
            Peru = Factory.ToPen(Rgba.Peru);
            Pink = Factory.ToPen(Rgba.Pink);
            Plum = Factory.ToPen(Rgba.Plum);
            PowderBlue = Factory.ToPen(Rgba.PowderBlue);
            Purple = Factory.ToPen(Rgba.Purple);
            Red = Factory.ToPen(Rgba.Red);
            RosyBrown = Factory.ToPen(Rgba.RosyBrown);
            RoyalBlue = Factory.ToPen(Rgba.RoyalBlue);
            SaddleBrown = Factory.ToPen(Rgba.SaddleBrown);
            Salmon = Factory.ToPen(Rgba.Salmon);
            SandyBrown = Factory.ToPen(Rgba.SandyBrown);
            SeaGreen = Factory.ToPen(Rgba.SeaGreen);
            SeaShell = Factory.ToPen(Rgba.SeaShell);
            Sienna = Factory.ToPen(Rgba.Sienna);
            Silver = Factory.ToPen(Rgba.Silver);
            SkyBlue = Factory.ToPen(Rgba.SkyBlue);
            SlateBlue = Factory.ToPen(Rgba.SlateBlue);
            SlateGray = Factory.ToPen(Rgba.SlateGray);
            Snow = Factory.ToPen(Rgba.Snow);
            SpringGreen = Factory.ToPen(Rgba.SpringGreen);
            SteelBlue = Factory.ToPen(Rgba.SteelBlue);
            Tan = Factory.ToPen(Rgba.Tan);
            Teal = Factory.ToPen(Rgba.Teal);
            Thistle = Factory.ToPen(Rgba.Thistle);
            Tomato = Factory.ToPen(Rgba.Tomato);
            Turquoise = Factory.ToPen(Rgba.Turquoise);
            Violet = Factory.ToPen(Rgba.Violet);
            Wheat = Factory.ToPen(Rgba.Wheat);
            White = Factory.ToPen(Rgba.White);
            WhiteSmoke = Factory.ToPen(Rgba.WhiteSmoke);
            Yellow = Factory.ToPen(Rgba.Yellow);
            YellowGreen = Factory.ToPen(Rgba.YellowGreen);
            ButtonFace = Factory.ToPen(Rgba.ButtonFace);
            ButtonHighlight = Factory.ToPen(Rgba.ButtonHighlight);
            ButtonShadow = Factory.ToPen(Rgba.ButtonShadow);
            GradientActiveCaption = Factory.ToPen(Rgba.GradientActiveCaption);
            GradientInactiveCaption = Factory.ToPen(Rgba.GradientInactiveCaption);
            MenuBar = Factory.ToPen(Rgba.MenuBar);
            MenuHighlight = Factory.ToPen(Rgba.MenuHighlight);

            DisabledPen = Silver;
            BackgroundPen = Black;
            ForegroundPen = White;
            HoverBackgroundPen = ActiveCaption;
            HoverForegroundPen = MidnightBlue;
        }
        #endregion

        #region PREDEFINED PENS
        public static IReadable Empty { get; private set; }
        public static IReadable ActiveBorder { get; private set; }
        public static IReadable ActiveCaption { get; private set; }
        public static IReadable ActiveCaptionText { get; private set; }
        public static IReadable AppWorkspace { get; private set; }
        public static IReadable Control { get; private set; }
        public static IReadable ControlDark { get; private set; }
        public static IReadable ControlDarkDark { get; private set; }
        public static IReadable ControlLight { get; private set; }
        public static IReadable ControlLightLight { get; private set; }
        public static IReadable ControlText { get; private set; }
        public static IReadable Desktop { get; private set; }
        public static IReadable GrayText { get; private set; }
        public static IReadable Highlight { get; private set; }
        public static IReadable HighlightText { get; private set; }
        public static IReadable HotTrack { get; private set; }
        public static IReadable InactiveBorder { get; private set; }
        public static IReadable InactiveCaption { get; private set; }
        public static IReadable InactiveCaptionText { get; private set; }
        public static IReadable Info { get; private set; }
        public static IReadable InfoText { get; private set; }
        public static IReadable Menu { get; private set; }
        public static IReadable MenuText { get; private set; }
        public static IReadable ScrollBar { get; private set; }
        public static IReadable Window { get; private set; }
        public static IReadable WindowFrame { get; private set; }
        public static IReadable WindowText { get; private set; }
        public static IReadable Transparent { get; private set; }
        public static IReadable AliceBlue { get; private set; }
        public static IReadable AntiqueWhite { get; private set; }
        public static IReadable Aqua { get; private set; }
        public static IReadable Aquamarine { get; private set; }
        public static IReadable Azure { get; private set; }
        public static IReadable Beige { get; private set; }
        public static IReadable Bisque { get; private set; }
        public static IReadable Black { get; private set; }
        public static IReadable BlanchedAlmond { get; private set; }
        public static IReadable Blue { get; private set; }
        public static IReadable BlueViolet { get; private set; }
        public static IReadable Brown { get; private set; }
        public static IReadable BurlyWood { get; private set; }
        public static IReadable CadetBlue { get; private set; }
        public static IReadable Chartreuse { get; private set; }
        public static IReadable Chocolate { get; private set; }
        public static IReadable Coral { get; private set; }
        public static IReadable CornflowerBlue { get; private set; }
        public static IReadable Cornsilk { get; private set; }
        public static IReadable Crimson { get; private set; }
        public static IReadable Cyan { get; private set; }
        public static IReadable DarkBlue { get; private set; }
        public static IReadable DarkCyan { get; private set; }
        public static IReadable DarkGoldenrod { get; private set; }
        public static IReadable DarkGray { get; private set; }
        public static IReadable DarkGreen { get; private set; }
        public static IReadable DarkKhaki { get; private set; }
        public static IReadable DarkMagenta { get; private set; }
        public static IReadable DarkOliveGreen { get; private set; }
        public static IReadable DarkOrange { get; private set; }
        public static IReadable DarkOrchid { get; private set; }
        public static IReadable DarkRed { get; private set; }
        public static IReadable DarkSalmon { get; private set; }
        public static IReadable DarkSeaGreen { get; private set; }
        public static IReadable DarkSlateBlue { get; private set; }
        public static IReadable DarkSlateGray { get; private set; }
        public static IReadable DarkTurquoise { get; private set; }
        public static IReadable DarkViolet { get; private set; }
        public static IReadable DeepPink { get; private set; }
        public static IReadable DeepSkyBlue { get; private set; }
        public static IReadable DimGray { get; private set; }
        public static IReadable DodgerBlue { get; private set; }
        public static IReadable Firebrick { get; private set; }
        public static IReadable FloralWhite { get; private set; }
        public static IReadable ForestGreen { get; private set; }
        public static IReadable Fuchsia { get; private set; }
        public static IReadable Gainsboro { get; private set; }
        public static IReadable GhostWhite { get; private set; }
        public static IReadable Gold { get; private set; }
        public static IReadable Goldenrod { get; private set; }
        public static IReadable Gray { get; private set; }
        public static IReadable Green { get; private set; }
        public static IReadable GreenYellow { get; private set; }
        public static IReadable Honeydew { get; private set; }
        public static IReadable HotPink { get; private set; }
        public static IReadable IndianRed { get; private set; }
        public static IReadable Indigo { get; private set; }
        public static IReadable Ivory { get; private set; }
        public static IReadable Khaki { get; private set; }
        public static IReadable Lavender { get; private set; }
        public static IReadable LavenderBlush { get; private set; }
        public static IReadable LawnGreen { get; private set; }
        public static IReadable LemonChiffon { get; private set; }
        public static IReadable LightBlue { get; private set; }
        public static IReadable LightCoral { get; private set; }
        public static IReadable LightCyan { get; private set; }
        public static IReadable LightGoldenrodYellow { get; private set; }
        public static IReadable LightGray { get; private set; }
        public static IReadable LightGreen { get; private set; }
        public static IReadable LightPink { get; private set; }
        public static IReadable LightSalmon { get; private set; }
        public static IReadable LightSeaGreen { get; private set; }
        public static IReadable LightSkyBlue { get; private set; }
        public static IReadable LightSlateGray { get; private set; }
        public static IReadable LightSteelBlue { get; private set; }
        public static IReadable LightYellow { get; private set; }
        public static IReadable Lime { get; private set; }
        public static IReadable LimeGreen { get; private set; }
        public static IReadable Linen { get; private set; }
        public static IReadable Magenta { get; private set; }
        public static IReadable Maroon { get; private set; }
        public static IReadable MediumAquamarine { get; private set; }
        public static IReadable MediumBlue { get; private set; }
        public static IReadable MediumOrchid { get; private set; }
        public static IReadable MediumPurple { get; private set; }
        public static IReadable MediumSeaGreen { get; private set; }
        public static IReadable MediumSlateBlue { get; private set; }
        public static IReadable MediumSpringGreen { get; private set; }
        public static IReadable MediumTurquoise { get; private set; }
        public static IReadable MediumVioletRed { get; private set; }
        public static IReadable MidnightBlue { get; private set; }
        public static IReadable MintCream { get; private set; }
        public static IReadable MistyRose { get; private set; }
        public static IReadable Moccasin { get; private set; }
        public static IReadable NavajoWhite { get; private set; }
        public static IReadable Navy { get; private set; }
        public static IReadable OldLace { get; private set; }
        public static IReadable Olive { get; private set; }
        public static IReadable OliveDrab { get; private set; }
        public static IReadable Orange { get; private set; }
        public static IReadable OrangeRed { get; private set; }
        public static IReadable Orchid { get; private set; }
        public static IReadable PaleGoldenrod { get; private set; }
        public static IReadable PaleGreen { get; private set; }
        public static IReadable PaleTurquoise { get; private set; }
        public static IReadable PaleVioletRed { get; private set; }
        public static IReadable PapayaWhip { get; private set; }
        public static IReadable PeachPuff { get; private set; }
        public static IReadable Peru { get; private set; }
        public static IReadable Pink { get; private set; }
        public static IReadable Plum { get; private set; }
        public static IReadable PowderBlue { get; private set; }
        public static IReadable Purple { get; private set; }
        public static IReadable Red { get; private set; }
        public static IReadable RosyBrown { get; private set; }
        public static IReadable RoyalBlue { get; private set; }
        public static IReadable SaddleBrown { get; private set; }
        public static IReadable Salmon { get; private set; }
        public static IReadable SandyBrown { get; private set; }
        public static IReadable SeaGreen { get; private set; }
        public static IReadable SeaShell { get; private set; }
        public static IReadable Sienna { get; private set; }
        public static IReadable Silver { get; private set; }
        public static IReadable SkyBlue { get; private set; }
        public static IReadable SlateBlue { get; private set; }
        public static IReadable SlateGray { get; private set; }
        public static IReadable Snow { get; private set; }
        public static IReadable SpringGreen { get; private set; }
        public static IReadable SteelBlue { get; private set; }
        public static IReadable Tan { get; private set; }
        public static IReadable Teal { get; private set; }
        public static IReadable Thistle { get; private set; }
        public static IReadable Tomato { get; private set; }
        public static IReadable Turquoise { get; private set; }
        public static IReadable Violet { get; private set; }
        public static IReadable Wheat { get; private set; }
        public static IReadable White { get; private set; }
        public static IReadable WhiteSmoke { get; private set; }
        public static IReadable Yellow { get; private set; }
        public static IReadable YellowGreen { get; private set; }
        public static IReadable ButtonFace { get; private set; }
        public static IReadable ButtonHighlight { get; private set; }
        public static IReadable ButtonShadow { get; private set; }
        public static IReadable GradientActiveCaption { get; private set; }
        public static IReadable GradientInactiveCaption { get; private set; }
        public static IReadable MenuBar { get; private set; }
        public static IReadable MenuHighlight { get; private set; }

        /// <summary>
        /// Retrieves a default disabled pen available in GWS.
        /// </summary>
        public static IReadable DisabledPen { get; private set; }

        /// <summary>
        /// Retrieves a default background pen available in GWS.
        /// </summary>
        public static IReadable BackgroundPen { get; private set; }

        /// <summary>
        /// Retrieves a default foreground pen available in GWS.
        /// </summary>
        public static IReadable ForegroundPen { get; private set; }

        /// <summary>
        /// Retrieves a default hover background pen available in GWS.
        /// </summary>
        public static IReadable HoverBackgroundPen { get; private set; }

        /// <summary>
        /// Retrieves a default hover foreground pen available in GWS.
        /// </summary>
        public static IReadable HoverForegroundPen { get; private set; }
        #endregion
    }
}
#endif
