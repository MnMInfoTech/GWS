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
        }
        #endregion

        #region TO PEN
        public static IPen ToPen(this IReadContext context, int? w = null, int? h = null)
        {
            return Instance.ToPen(context, w, h);
        }
        #endregion

        #region COUNT
        public static int CountOf<T>() where T : IPen
        {
            return Instance.CountOf<T>();
        }

        public static int CountOf<T>(Predicate<T> condition) where T : IPen
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
        public static void Replace(IPen obj)
        {
            Instance.Replace(obj);
        }
        #endregion

        #region ADD
        public static U Add<U>(U obj) where U : IPen
        {
            return Instance.Add(obj);
        }
        #endregion

        #region REMOVE
        public static bool Remove(IPen obj)
        {
            return Instance.Remove(obj);
        }

        public static bool Remove(string id)
        {
            return Instance.Remove(id);
        }
        #endregion

        #region GET
        public static U Get<U>(string key) where U : IPen
        {
            return Instance.Get<U>(key);
        }

        public static IPen Get(string key)
        {
            return Instance.Get(key);
        }

        public static bool Get<U>(string key, out U obj) where U : IPen
        {
            return Instance.Get(key, out obj);
        }

        public static bool Get(string key, out IPen obj)
        {
            return Instance.Get(key, out obj);
        }

        public static U Get<U>(Predicate<U> condition) where U : IPen
        {
            return Instance.Get(condition);
        }

        public static IPen Get(Predicate<IPen> condition)
        {
            return Instance.Get(condition);
        }
        #endregion

        #region GET ALL
        public static IEnumerable<U> GetAll<U>(Predicate<U> condition) where U : IPen
        {
            return Instance.GetAll(condition);
        }

        public static IEnumerable<IPen> GetAll(Predicate<IPen> condition)
        {
            return Instance.GetAll(condition);
        }

        public static IEnumerable<IPen> GetAll()
        {
            foreach (var item in Instance)
                yield return item;
        }
        #endregion

        public static void Dispose()
        {
            Instance.Dispose();
            Instance = null;
        }
    }

    static partial class Pens
    {
        #region PREDEFINED PENS
        public static IPen Empty { get; private set; }
        public static IPen ActiveBorder { get; private set; }
        public static IPen ActiveCaption { get; private set; }
        public static IPen ActiveCaptionText { get; private set; }
        public static IPen AppWorkspace { get; private set; }
        public static IPen Control { get; private set; }
        public static IPen ControlDark { get; private set; }
        public static IPen ControlDarkDark { get; private set; }
        public static IPen ControlLight { get; private set; }
        public static IPen ControlLightLight { get; private set; }
        public static IPen ControlText { get; private set; }
        public static IPen Desktop { get; private set; }
        public static IPen GrayText { get; private set; }
        public static IPen Highlight { get; private set; }
        public static IPen HighlightText { get; private set; }
        public static IPen HotTrack { get; private set; }
        public static IPen InactiveBorder { get; private set; }
        public static IPen InactiveCaption { get; private set; }
        public static IPen InactiveCaptionText { get; private set; }
        public static IPen Info { get; private set; }
        public static IPen InfoText { get; private set; }
        public static IPen Menu { get; private set; }
        public static IPen MenuText { get; private set; }
        public static IPen ScrollBar { get; private set; }
        public static IPen Window { get; private set; }
        public static IPen WindowFrame { get; private set; }
        public static IPen WindowText { get; private set; }
        public static IPen Transparent { get; private set; }
        public static IPen AliceBlue { get; private set; }
        public static IPen AntiqueWhite { get; private set; }
        public static IPen Aqua { get; private set; }
        public static IPen Aquamarine { get; private set; }
        public static IPen Azure { get; private set; }
        public static IPen Beige { get; private set; }
        public static IPen Bisque { get; private set; }
        public static IPen Black { get; private set; }
        public static IPen BlanchedAlmond { get; private set; }
        public static IPen Blue { get; private set; }
        public static IPen BlueViolet { get; private set; }
        public static IPen Brown { get; private set; }
        public static IPen BurlyWood { get; private set; }
        public static IPen CadetBlue { get; private set; }
        public static IPen Chartreuse { get; private set; }
        public static IPen Chocolate { get; private set; }
        public static IPen Coral { get; private set; }
        public static IPen CornflowerBlue { get; private set; }
        public static IPen Cornsilk { get; private set; }
        public static IPen Crimson { get; private set; }
        public static IPen Cyan { get; private set; }
        public static IPen DarkBlue { get; private set; }
        public static IPen DarkCyan { get; private set; }
        public static IPen DarkGoldenrod { get; private set; }
        public static IPen DarkGray { get; private set; }
        public static IPen DarkGreen { get; private set; }
        public static IPen DarkKhaki { get; private set; }
        public static IPen DarkMagenta { get; private set; }
        public static IPen DarkOliveGreen { get; private set; }
        public static IPen DarkOrange { get; private set; }
        public static IPen DarkOrchid { get; private set; }
        public static IPen DarkRed { get; private set; }
        public static IPen DarkSalmon { get; private set; }
        public static IPen DarkSeaGreen { get; private set; }
        public static IPen DarkSlateBlue { get; private set; }
        public static IPen DarkSlateGray { get; private set; }
        public static IPen DarkTurquoise { get; private set; }
        public static IPen DarkViolet { get; private set; }
        public static IPen DeepPink { get; private set; }
        public static IPen DeepSkyBlue { get; private set; }
        public static IPen DimGray { get; private set; }
        public static IPen DodgerBlue { get; private set; }
        public static IPen Firebrick { get; private set; }
        public static IPen FloralWhite { get; private set; }
        public static IPen ForestGreen { get; private set; }
        public static IPen Fuchsia { get; private set; }
        public static IPen Gainsboro { get; private set; }
        public static IPen GhostWhite { get; private set; }
        public static IPen Gold { get; private set; }
        public static IPen Goldenrod { get; private set; }
        public static IPen Gray { get; private set; }
        public static IPen Green { get; private set; }
        public static IPen GreenYellow { get; private set; }
        public static IPen Honeydew { get; private set; }
        public static IPen HotPink { get; private set; }
        public static IPen IndianRed { get; private set; }
        public static IPen Indigo { get; private set; }
        public static IPen Ivory { get; private set; }
        public static IPen Khaki { get; private set; }
        public static IPen Lavender { get; private set; }
        public static IPen LavenderBlush { get; private set; }
        public static IPen LawnGreen { get; private set; }
        public static IPen LemonChiffon { get; private set; }
        public static IPen LightBlue { get; private set; }
        public static IPen LightCoral { get; private set; }
        public static IPen LightCyan { get; private set; }
        public static IPen LightGoldenrodYellow { get; private set; }
        public static IPen LightGray { get; private set; }
        public static IPen LightGreen { get; private set; }
        public static IPen LightPink { get; private set; }
        public static IPen LightSalmon { get; private set; }
        public static IPen LightSeaGreen { get; private set; }
        public static IPen LightSkyBlue { get; private set; }
        public static IPen LightSlateGray { get; private set; }
        public static IPen LightSteelBlue { get; private set; }
        public static IPen LightYellow { get; private set; }
        public static IPen Lime { get; private set; }
        public static IPen LimeGreen { get; private set; }
        public static IPen Linen { get; private set; }
        public static IPen Magenta { get; private set; }
        public static IPen Maroon { get; private set; }
        public static IPen MediumAquamarine { get; private set; }
        public static IPen MediumBlue { get; private set; }
        public static IPen MediumOrchid { get; private set; }
        public static IPen MediumPurple { get; private set; }
        public static IPen MediumSeaGreen { get; private set; }
        public static IPen MediumSlateBlue { get; private set; }
        public static IPen MediumSpringGreen { get; private set; }
        public static IPen MediumTurquoise { get; private set; }
        public static IPen MediumVioletRed { get; private set; }
        public static IPen MidnightBlue { get; private set; }
        public static IPen MintCream { get; private set; }
        public static IPen MistyRose { get; private set; }
        public static IPen Moccasin { get; private set; }
        public static IPen NavajoWhite { get; private set; }
        public static IPen Navy { get; private set; }
        public static IPen OldLace { get; private set; }
        public static IPen Olive { get; private set; }
        public static IPen OliveDrab { get; private set; }
        public static IPen Orange { get; private set; }
        public static IPen OrangeRed { get; private set; }
        public static IPen Orchid { get; private set; }
        public static IPen PaleGoldenrod { get; private set; }
        public static IPen PaleGreen { get; private set; }
        public static IPen PaleTurquoise { get; private set; }
        public static IPen PaleVioletRed { get; private set; }
        public static IPen PapayaWhip { get; private set; }
        public static IPen PeachPuff { get; private set; }
        public static IPen Peru { get; private set; }
        public static IPen Pink { get; private set; }
        public static IPen Plum { get; private set; }
        public static IPen PowderBlue { get; private set; }
        public static IPen Purple { get; private set; }
        public static IPen Red { get; private set; }
        public static IPen RosyBrown { get; private set; }
        public static IPen RoyalBlue { get; private set; }
        public static IPen SaddleBrown { get; private set; }
        public static IPen Salmon { get; private set; }
        public static IPen SandyBrown { get; private set; }
        public static IPen SeaGreen { get; private set; }
        public static IPen SeaShell { get; private set; }
        public static IPen Sienna { get; private set; }
        public static IPen Silver { get; private set; }
        public static IPen SkyBlue { get; private set; }
        public static IPen SlateBlue { get; private set; }
        public static IPen SlateGray { get; private set; }
        public static IPen Snow { get; private set; }
        public static IPen SpringGreen { get; private set; }
        public static IPen SteelBlue { get; private set; }
        public static IPen Tan { get; private set; }
        public static IPen Teal { get; private set; }
        public static IPen Thistle { get; private set; }
        public static IPen Tomato { get; private set; }
        public static IPen Turquoise { get; private set; }
        public static IPen Violet { get; private set; }
        public static IPen Wheat { get; private set; }
        public static IPen White { get; private set; }
        public static IPen WhiteSmoke { get; private set; }
        public static IPen Yellow { get; private set; }
        public static IPen YellowGreen { get; private set; }
        public static IPen ButtonFace { get; private set; }
        public static IPen ButtonHighlight { get; private set; }
        public static IPen ButtonShadow { get; private set; }
        public static IPen GradientActiveCaption { get; private set; }
        public static IPen GradientInactiveCaption { get; private set; }
        public static IPen MenuBar { get; private set; }
        public static IPen MenuHighlight { get; private set; }

        /// <summary>
        /// Retrieves a default disabled pen available GWS.
        /// </summary>
        public static IPen DisabledPen { get; private set; }
        #endregion
    }
#endif
}
