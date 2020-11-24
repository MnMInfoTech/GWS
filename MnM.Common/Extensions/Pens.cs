/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
#if (GWS || Window)
    public static partial class Pens
    {
        internal static IPens Instance;

        #region ATTACH
        internal static void Attach(IFactory factory)
        {
            if (factory == null)
                return;
            Instance = factory.newPenStore();
        }
        #endregion

        #region RESET
        internal static void Reset()
        {
            Rgba.Reset();
            BrushStyle.Reset();
            if (Instance == null)
                return;
            Empty = Instance.ToPen(Rgba.Empty);
            ActiveBorder = Instance.ToPen(Rgba.ActiveBorder);
            ActiveCaption = Instance.ToPen(Rgba.ActiveCaption);
            ActiveCaptionText = Instance.ToPen(Rgba.ActiveCaptionText);
            AppWorkspace = Instance.ToPen(Rgba.AppWorkspace);
            Control = Instance.ToPen(Rgba.Control);
            ControlDark = Instance.ToPen(Rgba.ControlDark);
            ControlDarkDark = Instance.ToPen(Rgba.ControlDarkDark);
            ControlLight = Instance.ToPen(Rgba.ControlLight);
            ControlLightLight = Instance.ToPen(Rgba.ControlLightLight);
            ControlText = Instance.ToPen(Rgba.ControlText);
            Desktop = Instance.ToPen(Rgba.Desktop);
            GrayText = Instance.ToPen(Rgba.GrayText);
            Highlight = Instance.ToPen(Rgba.Highlight);
            HighlightText = Instance.ToPen(Rgba.HighlightText);
            HotTrack = Instance.ToPen(Rgba.HotTrack);
            InactiveBorder = Instance.ToPen(Rgba.InactiveBorder);
            InactiveCaption = Instance.ToPen(Rgba.InactiveCaption);
            InactiveCaptionText = Instance.ToPen(Rgba.InactiveCaptionText);
            Info = Instance.ToPen(Rgba.Info);
            InfoText = Instance.ToPen(Rgba.InfoText);
            Menu = Instance.ToPen(Rgba.Menu);
            MenuText = Instance.ToPen(Rgba.MenuText);
            ScrollBar = Instance.ToPen(Rgba.ScrollBar);
            Window = Instance.ToPen(Rgba.Window);
            WindowFrame = Instance.ToPen(Rgba.WindowFrame);
            WindowText = Instance.ToPen(Rgba.WindowText);
            Transparent = Instance.ToPen(Rgba.Transparent);
            AliceBlue = Instance.ToPen(Rgba.AliceBlue);
            AntiqueWhite = Instance.ToPen(Rgba.AntiqueWhite);
            Aqua = Instance.ToPen(Rgba.Aqua);
            Aquamarine = Instance.ToPen(Rgba.Aquamarine);
            Azure = Instance.ToPen(Rgba.Azure);
            Beige = Instance.ToPen(Rgba.Beige);
            Bisque = Instance.ToPen(Rgba.Bisque);
            Black = Instance.ToPen(Rgba.Black);
            BlanchedAlmond = Instance.ToPen(Rgba.BlanchedAlmond);
            Blue = Instance.ToPen(Rgba.Blue);
            BlueViolet = Instance.ToPen(Rgba.BlueViolet);
            Brown = Instance.ToPen(Rgba.Brown);
            BurlyWood = Instance.ToPen(Rgba.BurlyWood);
            CadetBlue = Instance.ToPen(Rgba.CadetBlue);
            Chartreuse = Instance.ToPen(Rgba.Chartreuse);
            Chocolate = Instance.ToPen(Rgba.Chocolate);
            Coral = Instance.ToPen(Rgba.Coral);
            CornflowerBlue = Instance.ToPen(Rgba.CornflowerBlue);
            Cornsilk = Instance.ToPen(Rgba.Cornsilk);
            Crimson = Instance.ToPen(Rgba.Crimson);
            Cyan = Instance.ToPen(Rgba.Cyan);
            DarkBlue = Instance.ToPen(Rgba.DarkBlue);
            DarkCyan = Instance.ToPen(Rgba.DarkCyan);
            DarkGoldenrod = Instance.ToPen(Rgba.DarkGoldenrod);
            DarkGray = Instance.ToPen(Rgba.DarkGray);
            DarkGreen = Instance.ToPen(Rgba.DarkGreen);
            DarkKhaki = Instance.ToPen(Rgba.DarkKhaki);
            DarkMagenta = Instance.ToPen(Rgba.DarkMagenta);
            DarkOliveGreen = Instance.ToPen(Rgba.DarkOliveGreen);
            DarkOrange = Instance.ToPen(Rgba.DarkOrange);
            DarkOrchid = Instance.ToPen(Rgba.DarkOrchid);
            DarkRed = Instance.ToPen(Rgba.DarkRed);
            DarkSalmon = Instance.ToPen(Rgba.DarkSalmon);
            DarkSeaGreen = Instance.ToPen(Rgba.DarkSeaGreen);
            DarkSlateBlue = Instance.ToPen(Rgba.DarkSlateBlue);
            DarkSlateGray = Instance.ToPen(Rgba.DarkSlateGray);
            DarkTurquoise = Instance.ToPen(Rgba.DarkTurquoise);
            DarkViolet = Instance.ToPen(Rgba.DarkViolet);
            DeepPink = Instance.ToPen(Rgba.DeepPink);
            DeepSkyBlue = Instance.ToPen(Rgba.DeepSkyBlue);
            DimGray = Instance.ToPen(Rgba.DimGray);
            DodgerBlue = Instance.ToPen(Rgba.DodgerBlue);
            Firebrick = Instance.ToPen(Rgba.Firebrick);
            FloralWhite = Instance.ToPen(Rgba.FloralWhite);
            ForestGreen = Instance.ToPen(Rgba.ForestGreen);
            Fuchsia = Instance.ToPen(Rgba.Fuchsia);
            Gainsboro = Instance.ToPen(Rgba.Gainsboro);
            GhostWhite = Instance.ToPen(Rgba.GhostWhite);
            Gold = Instance.ToPen(Rgba.Gold);
            Goldenrod = Instance.ToPen(Rgba.Goldenrod);
            Gray = Instance.ToPen(Rgba.Gray);
            Green = Instance.ToPen(Rgba.Green);
            GreenYellow = Instance.ToPen(Rgba.GreenYellow);
            Honeydew = Instance.ToPen(Rgba.Honeydew);
            HotPink = Instance.ToPen(Rgba.HotPink);
            IndianRed = Instance.ToPen(Rgba.IndianRed);
            Indigo = Instance.ToPen(Rgba.Indigo);
            Ivory = Instance.ToPen(Rgba.Ivory);
            Khaki = Instance.ToPen(Rgba.Khaki);
            Lavender = Instance.ToPen(Rgba.Lavender);
            LavenderBlush = Instance.ToPen(Rgba.LavenderBlush);
            LawnGreen = Instance.ToPen(Rgba.LawnGreen);
            LemonChiffon = Instance.ToPen(Rgba.LemonChiffon);
            LightBlue = Instance.ToPen(Rgba.LightBlue);
            LightCoral = Instance.ToPen(Rgba.LightCoral);
            LightCyan = Instance.ToPen(Rgba.LightCyan);
            LightGoldenrodYellow = Instance.ToPen(Rgba.LightGoldenrodYellow);
            LightGray = Instance.ToPen(Rgba.LightGray);
            LightGreen = Instance.ToPen(Rgba.LightGreen);
            LightPink = Instance.ToPen(Rgba.LightPink);
            LightSalmon = Instance.ToPen(Rgba.LightSalmon);
            LightSeaGreen = Instance.ToPen(Rgba.LightSeaGreen);
            LightSkyBlue = Instance.ToPen(Rgba.LightSkyBlue);
            LightSlateGray = Instance.ToPen(Rgba.LightSlateGray);
            LightSteelBlue = Instance.ToPen(Rgba.LightSteelBlue);
            LightYellow = Instance.ToPen(Rgba.LightYellow);
            Lime = Instance.ToPen(Rgba.Lime);
            LimeGreen = Instance.ToPen(Rgba.LimeGreen);
            Linen = Instance.ToPen(Rgba.Linen);
            Magenta = Instance.ToPen(Rgba.Magenta);
            Maroon = Instance.ToPen(Rgba.Maroon);
            MediumAquamarine = Instance.ToPen(Rgba.MediumAquamarine);
            MediumBlue = Instance.ToPen(Rgba.MediumBlue);
            MediumOrchid = Instance.ToPen(Rgba.MediumOrchid);
            MediumPurple = Instance.ToPen(Rgba.MediumPurple);
            MediumSeaGreen = Instance.ToPen(Rgba.MediumSeaGreen);
            MediumSlateBlue = Instance.ToPen(Rgba.MediumSlateBlue);
            MediumSpringGreen = Instance.ToPen(Rgba.MediumSpringGreen);
            MediumTurquoise = Instance.ToPen(Rgba.MediumTurquoise);
            MediumVioletRed = Instance.ToPen(Rgba.MediumVioletRed);
            MidnightBlue = Instance.ToPen(Rgba.MidnightBlue);
            MintCream = Instance.ToPen(Rgba.MintCream);
            MistyRose = Instance.ToPen(Rgba.MistyRose);
            Moccasin = Instance.ToPen(Rgba.Moccasin);
            NavajoWhite = Instance.ToPen(Rgba.NavajoWhite);
            Navy = Instance.ToPen(Rgba.Navy);
            OldLace = Instance.ToPen(Rgba.OldLace);
            Olive = Instance.ToPen(Rgba.Olive);
            OliveDrab = Instance.ToPen(Rgba.OliveDrab);
            Orange = Instance.ToPen(Rgba.Orange);
            OrangeRed = Instance.ToPen(Rgba.OrangeRed);
            Orchid = Instance.ToPen(Rgba.Orchid);
            PaleGoldenrod = Instance.ToPen(Rgba.PaleGoldenrod);
            PaleGreen = Instance.ToPen(Rgba.PaleGreen);
            PaleTurquoise = Instance.ToPen(Rgba.PaleTurquoise);
            PaleVioletRed = Instance.ToPen(Rgba.PaleVioletRed);
            PapayaWhip = Instance.ToPen(Rgba.PapayaWhip);
            PeachPuff = Instance.ToPen(Rgba.PeachPuff);
            Peru = Instance.ToPen(Rgba.Peru);
            Pink = Instance.ToPen(Rgba.Pink);
            Plum = Instance.ToPen(Rgba.Plum);
            PowderBlue = Instance.ToPen(Rgba.PowderBlue);
            Purple = Instance.ToPen(Rgba.Purple);
            Red = Instance.ToPen(Rgba.Red);
            RosyBrown = Instance.ToPen(Rgba.RosyBrown);
            RoyalBlue = Instance.ToPen(Rgba.RoyalBlue);
            SaddleBrown = Instance.ToPen(Rgba.SaddleBrown);
            Salmon = Instance.ToPen(Rgba.Salmon);
            SandyBrown = Instance.ToPen(Rgba.SandyBrown);
            SeaGreen = Instance.ToPen(Rgba.SeaGreen);
            SeaShell = Instance.ToPen(Rgba.SeaShell);
            Sienna = Instance.ToPen(Rgba.Sienna);
            Silver = Instance.ToPen(Rgba.Silver);
            SkyBlue = Instance.ToPen(Rgba.SkyBlue);
            SlateBlue = Instance.ToPen(Rgba.SlateBlue);
            SlateGray = Instance.ToPen(Rgba.SlateGray);
            Snow = Instance.ToPen(Rgba.Snow);
            SpringGreen = Instance.ToPen(Rgba.SpringGreen);
            SteelBlue = Instance.ToPen(Rgba.SteelBlue);
            Tan = Instance.ToPen(Rgba.Tan);
            Teal = Instance.ToPen(Rgba.Teal);
            Thistle = Instance.ToPen(Rgba.Thistle);
            Tomato = Instance.ToPen(Rgba.Tomato);
            Turquoise = Instance.ToPen(Rgba.Turquoise);
            Violet = Instance.ToPen(Rgba.Violet);
            Wheat = Instance.ToPen(Rgba.Wheat);
            White = Instance.ToPen(Rgba.White);
            WhiteSmoke = Instance.ToPen(Rgba.WhiteSmoke);
            Yellow = Instance.ToPen(Rgba.Yellow);
            YellowGreen = Instance.ToPen(Rgba.YellowGreen);
            ButtonFace = Instance.ToPen(Rgba.ButtonFace);
            ButtonHighlight = Instance.ToPen(Rgba.ButtonHighlight);
            ButtonShadow = Instance.ToPen(Rgba.ButtonShadow);
            GradientActiveCaption = Instance.ToPen(Rgba.GradientActiveCaption);
            GradientInactiveCaption = Instance.ToPen(Rgba.GradientInactiveCaption);
            MenuBar = Instance.ToPen(Rgba.MenuBar);
            MenuHighlight = Instance.ToPen(Rgba.MenuHighlight);

            DisabledPen = Silver;
            BackgroundPen = Black;
            ForegroundPen = White;
            HoverBackgroundPen = ActiveCaption;
            HoverForegroundPen = MidnightBlue;
        }
        #endregion

        #region TO PEN
        public static IReadable ToPen(this IPenContext context, int? w = null, int? h = null)
        {
            return Instance.ToPen(context, w, h);
        }
        #endregion

        #region COUNT
        public static int CountOf<T>() where T : IReadable
        {
            return Instance.CountOf<T>();
        }

        public static int CountOf<T>(Predicate<T> condition) where T : IReadable
        {
            return Instance.CountOf(condition);
        }
        #endregion

        #region CONTAINS
        public static bool Contains(string key)
        {
            return Instance.Contains(key);
        }
        #endregion

        #region REPLACE
        public static void Replace(IReadable obj)
        {
            Instance.Replace(obj);
        }
        #endregion

        #region ADD
        public static U Add<U>(U obj) where U : IReadable
        {
            return Instance.Add(obj);
        }
        #endregion

        #region REMOVE
        public static bool Remove(IReadable obj)
        {
            return Instance.Remove(obj);
        }

        public static bool Remove(string id)
        {
            return Instance.Remove(id);
        }
        #endregion

        #region GET
        public static U Get<U>(string key) where U : IReadable
        {
            return Instance.Get<U>(key);
        }

        public static IReadable Get(string key)
        {
            return Instance.Get(key);
        }

        public static bool Get<U>(string key, out U obj) where U : IReadable
        {
            return Instance.Get(key, out obj);
        }

        public static bool Get(string key, out IReadable obj)
        {
            return Instance.Get(key, out obj);
        }

        public static U Get<U>(Predicate<U> condition) where U : IReadable
        {
            return Instance.Get(condition);
        }

        public static IReadable Get(Predicate<IReadable> condition)
        {
            return Instance.Get(condition);
        }
        #endregion

        #region GET ALL
        public static IEnumerable<U> GetAll<U>(Predicate<U> condition) where U : IReadable
        {
            return Instance.GetAll(condition);
        }

        public static IEnumerable<IReadable> GetAll(Predicate<IReadable> condition)
        {
            return Instance.GetAll(condition);
        }

        public static IEnumerable<IReadable> GetAll()
        {
            foreach (var item in Instance)
                yield return item;
        }
        #endregion

        public static void Dispose()
        {
            Instance?.Dispose();
            Instance = null;
        }
    }

    static partial class Pens
    {
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
#endif
}
