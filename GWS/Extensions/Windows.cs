/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Windows
    {
        #region FIND OBJECT
        /// <summary>
        /// Finds an element from this collection if it exists on a given x and y coordinates.
        /// the test is applied on a last drawn area rather than an actual area of each element so if an element is not drawn yet, 
        /// it can not be found!
        /// </summary>
        /// <param name="x">X coordinate to search for</param>
        /// <param name="y">Y coordinate to search for</param>
        /// <returns></returns>
        //public static IRenderable FindObject(this IObjectFinder finder, int x, int y) =>
        //     finder.FindObject(x, y, out _, out _);
        #endregion

        #region UIOBJECT
        //public static string GetText(this IElement item, ContentDisplay? externalDisplay = null)
        //{
        //    var ext = externalDisplay ?? item.ContentDisplay;

        //    if (item.ContentDisplay == ContentDisplay.TextAlways)
        //        return item.Text ?? item.Name;

        //    if ((!item.ContentDisplay.HasFlag(ContentDisplay.Text) ||
        //        !ext.HasFlag(ContentDisplay.Text)) && GetImage(item, externalDisplay) != null)
        //        return "";

        //    return item.Text ?? item.Name;
        //}
        //public static string GetTip(this IElement item, ContentDisplay? externalDisplay = null)
        //{
        //    if (!string.IsNullOrEmpty(item.ToolTipText))
        //        return item.ToolTipText;

        //    var ext = externalDisplay ?? item.ContentDisplay;

        //    if (!item.ContentDisplay.HasFlag(ContentDisplay.Text) ||
        //        !ext.HasFlag(ContentDisplay.Text))
        //        return item.Name;

        //    return null;
        //}
        //public static ISurface GetImage(this IElement item, ContentDisplay? externalDisplay = null)
        //{
        //    var ext = externalDisplay ?? item.ContentDisplay;

        //    ISurface image = item.Image;

        //    if (item.ContentDisplay == ContentDisplay.ImageAlways)
        //        return image;

        //    if (!item.ContentDisplay.HasFlag(ContentDisplay.Image) ||
        //           !ext.HasFlag(ContentDisplay.Image))
        //    {
        //        if (!item.ContentDisplay.HasFlag(ContentDisplay.Text) ||
        //            !ext.HasFlag(ContentDisplay.Text))
        //            return image;

        //        return null;
        //    }
        //    return image;
        //}
        //public static void ShowTip(this IElement e, IUIControl control)
        //{
        //    var tip = (!e.IsButton) ? e.ToolTipText :
        //        GetTip(e, control?.ContentDisplay);

        //    if (string.IsNullOrEmpty(tip))
        //        return;

        //    //RectMovement move = (control.Orientation == Direction.Vertical) ?
        //    //       RectMovement.Right : RectMovement.Down;

        //    var right = (e as IDDControl)?.IsDDCStyle ?? false;

        //    IArea tiprc = e.Bounds;
        //    IPoint p;

        //    if ((e is IUIObject && (e as IUIObject).Children != null) || right)
        //    {
        //        //move = RectMovement.Right;
        //        tiprc = e.DropDownButton;
        //        p = e.DropDownButton.RTPoint();
        //    }
        //    else
        //    {
        //        p = e.DropDownButton.RBPoint();
        //    }
        //    //tiprc = tiprc.Neighbour(1, move);
        //    //p = tiprc.Location;//.RightBottom();
        //    p.Offset(1, 1);
        //    control?.ShowToolTip(tip, p);
        //}
        //public static IElement Find(this IElement e, string value, ref int index)
        //{
        //    if (e is IUIObject)
        //    {
        //        var children = (e as IUIObject).Children;
        //        if (children.FirstMatchIndex((x => string.Equals(value, x.Text, NoCase)), out index, 0, false))
        //            return children[index];
        //    }
        //    else
        //    {
        //        if (string.Equals(e.Text, value, NoCase))
        //            return e;
        //    }
        //    index = -1;
        //    return null;
        //}
        #endregion

        #region KEYEVENTARGS
        public static bool AltOrCtrlOrShift(this IKeyEventArgs e) => 
            e[ModifierKeys.Alt] || e[ModifierKeys.Shift] || e[ModifierKeys.Ctrl];
        public static bool AltOrCtrl(this IKeyEventArgs e) => 
            e[ModifierKeys.Alt] || e[ModifierKeys.Ctrl];
        public static bool ShiftOrCtrl(this IKeyEventArgs e) =>
             e[ModifierKeys.Shift] || e[ModifierKeys.Ctrl];
        public static bool AltOrShift(this IKeyEventArgs e) =>
             e[ModifierKeys.Alt] || e[ModifierKeys.Shift];
        public static bool AltCtrlShift(this IKeyEventArgs e) =>
             e[ModifierKeys.Alt | ModifierKeys.Shift | ModifierKeys.Ctrl];
        public static bool AltCtrl(this IKeyEventArgs e) =>
            e[ModifierKeys.Alt | ModifierKeys.Ctrl];
        public static bool ShiftCtrl(this IKeyEventArgs e) =>
            e[ModifierKeys.Shift | ModifierKeys.Ctrl];
        public static bool AltShift(this IKeyEventArgs e) =>
            e[ModifierKeys.Alt | ModifierKeys.Shift];

        public static bool IsHotKey(this IKeyEventArgs e) =>
             e[ModifierKeys.Alt] && !ShiftOrCtrl(e) && e.State == KeyState.Up && (
            IsWithinRange(e.KeyCode, Key.A, Key.Z) ||
            IsWithinRange(e.KeyCode, Key.NumPad0, Key.NumPad9) ||
            IsWithinRange(e.KeyCode, Key.D0, Key.D9));
        public static bool IsKeyNumeric(this IKeyEventArgs e) =>
            !AltOrCtrlOrShift(e) && IsNumKey(e.KeyCode);
        public static bool IsKeyLetter(this IKeyEventArgs e) => !AltOrCtrl(e) &&
           IsWithinRange(e.KeyCode, Key.A, Key.Z);

        public static bool OpenSubMenu(this IKeyEventArgs e) =>
           !IsHotKey(e) && !AltOrCtrl(e) && e.KeyCode == Key.Up && 
            ((e[ModifierKeys.Shift] && e.KeyCode == Key.F10) ||
            (!e[ModifierKeys.Shift] && e.KeyCode == Key.Menu) || (e.KeyCode == Key.Right));

        public static bool IsNumKey(this Key key) =>
                IsWithinRange(key, Key.NumPad0, Key.NumPad9) ||
               IsWithinRange(key, Key.D0, Key.D9);
        public static char NumKeyToChar(this Key key)
        {
            switch (key)
            {
                case Key.NumPad0:
                case Key.D0:
                    return '0';
                case Key.NumPad1:
                case Key.D1:
                    return '1';
                case Key.NumPad2:
                case Key.D2:
                    return '2';
                case Key.NumPad3:
                case Key.D3:
                    return '3';
                case Key.NumPad4:
                case Key.D4:
                    return '4';
                case Key.NumPad5:
                case Key.D5:
                    return '5';
                case Key.NumPad6:
                case Key.D6:
                    return '6';
                case Key.NumPad7:
                case Key.D7:
                    return '7';
                case Key.NumPad8:
                case Key.D8:
                    return '8';
                case Key.NumPad9:
                case Key.D9:
                    return '9';
                default:
                    return (char)0;
            }
        }
        public static bool IsModifierKey(this Key KeyCode)
        {
            return Exists(KeyCode, Key.Delete, Key.Back, Key.Enter,
                Key.Tab, Key.ShiftKey, Key.Alt, Key.ControlKey);
        }
        public static bool IsNavigationKey(this Key KeyCode)
        {
            return IsWithinRange(KeyCode, Key.End, Key.Down);
        }
        public static bool IsNavigationOrModifierKey(this Key KeyCode)
        {
            return IsWithinRange(KeyCode, Key.End, Key.Down) ||
                Exists(KeyCode,
                Key.Delete,

                Key.Back,
                Key.Enter,
                Key.Return,
                Key.Separator,

                Key.Escape,
                Key.Tab,
                Key.ShiftKey,
                Key.Shift,
                Key.LShiftKey,
                Key.RShiftKey,
                Key.Alt,

                Key.Control,
                Key.LControlKey,
                Key.RControlKey,
                Key.ControlKey,
                Key.PageDown,
                Key.PageUp);
        }
        public static bool IsWithinRange(this Key CompareNumber, Key Range1, Key Range2)
        {
            if (CompareNumber >= Range1 & CompareNumber <= Range2)
                return true;
            return false;
        }
        public static bool IsWithinRange(int CompareNumber, Key Range1, Key Range2)
        {
            try
            {
                Key key = (Key)CompareNumber;
                return IsWithinRange(key, Range1, Range2);
            }
            catch
            {
                return false;
            }
        }
        public static bool Exists(this Key check, params Key[] allkeys)
        {
            foreach (Key obj in allkeys)
            {
                if (obj.Equals(check))
                    return true;
            }
            return false;
        }
        public static bool Exists(int check, params Key[] allkeys)
        {
            var key = (Key)check;
            foreach (Key obj in allkeys)
            {
                if (obj.Equals(key))
                    return true;
            }
            return false;
        }

        public static bool Up(this IKeyEventArgs e) =>
            e.State == KeyState.Up;
        public static bool Down(this IKeyEventArgs e) =>
            e.State == KeyState.Down;
        public static Vector Point(this IKeyEventArgs e) =>
            new Vector();
        public static bool IsShift(this IKeyEventArgs e) =>
             e[ModifierKeys.Shift];
        public static bool Enter(this IKeyEventArgs e) =>
            e.KeyCode == Key.Return || e.KeyCode == Key.Enter;

        public static bool AppClick(this IKeyEventArgs e) =>
            !e.AltOrCtrl() && (e.KeyCode == Key.Tab || e.KeyCode == Key.Escape) || e.OpenContextMenu();
        public static bool MoveFocus(this IKeyEventArgs e) =>
            !e.AltOrCtrl() && e.Up() && e.KeyCode == Key.Tab;
        public static bool IsClicked(this IKeyEventArgs e) =>
             !e.AltOrCtrlOrShift() && e.Up() && (e.KeyCode == Key.Return || e.KeyCode == Key.Enter);
        public static bool IsPressed(this IKeyEventArgs e) =>
             !e.AltOrCtrlOrShift() && e.Down() && (e.KeyCode == Key.Return || e.KeyCode == Key.Enter);
        public static bool IsFocused(this IKeyEventArgs e) =>
            e.MoveFocus() || e.IsHotKey() || e.IsClicked();

        public static bool OpenContextMenu(this IKeyEventArgs e) =>
            !e.AltOrCtrl() && e.Up() && ((e.IsShift() && e.KeyCode == Key.F10) ||
            (!e.IsShift() && e.KeyCode == Key.Menu));

        internal
        static bool Leave(this IKeyEventArgs e, IExEventPusher control) =>
            !e.AltOrCtrlOrShift() && e.KeyCode == Key.Escape;
        #endregion

        #region MOUSEEVENTARGS
        public static bool Move(this IMouseEventArgs e) =>
            e.Status == MouseState.Move;
        public static bool Up(this IMouseEventArgs e) =>
            e.Status == MouseState.Up;
        public static bool Down(this IMouseEventArgs e) =>
            e.Status == MouseState.Down;
        public static bool Click(this IMouseEventArgs e) =>
            e.Status == MouseState.Click;
        public static bool DoubleClick(this IMouseEventArgs e) =>
            e.Status == MouseState.DoubleClick;
        public static bool Wheel(this IMouseEventArgs e) =>
            e.Status == MouseState.Wheel;
        public static bool None(this IMouseEventArgs e) =>
            e.Button == MouseButton.None;
        public static bool Left(this IMouseEventArgs e) =>
            e.Button == MouseButton.Left;
        public static bool Right(this IMouseEventArgs e) =>
            e.Button == MouseButton.Right;
        public static bool Middle(this IMouseEventArgs e) =>
            e.Button == MouseButton.Middle;
        public static Vector Point(this IMouseEventArgs e) =>
            new Vector(e.X, e.Y);
        public static bool IsClicked(this IMouseEventArgs e) =>
            e.Up() && e.Left();
        public static bool IsPressed(this IMouseEventArgs e) =>
            e.Down() && e.Left();
        #endregion

        #region RESET-KEYS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetModifierKeys(this KeyState KeyState, Key KeyCode, ref ModifierKeys ModifierKeys)
        {
            if ((KeyState & KeyState.Down) == KeyState.Down)
            {
                switch (KeyCode)
                {
                    case Key.ShiftKey:
                    case Key.LShiftKey:
                    case Key.Shift:
                    case Key.RShiftKey:
                        ModifierKeys |= ModifierKeys.Shift;
                        break;
                    case Key.ControlKey:
                    case Key.LControlKey:
                    case Key.RControlKey:
                    case Key.Control:
                        ModifierKeys |= ModifierKeys.Ctrl;
                        break;
                    case Key.Alt:
                    case Key.LAlt:
                    case Key.RALT:
                        ModifierKeys |= ModifierKeys.Alt;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (KeyCode)
                {
                    case Key.ShiftKey:
                    case Key.LShiftKey:
                    case Key.Shift:
                    case Key.RShiftKey:
                        ModifierKeys &= ~ModifierKeys.Shift;
                        break;
                    case Key.ControlKey:
                    case Key.LControlKey:
                    case Key.RControlKey:
                    case Key.Control:
                        ModifierKeys &= ~ModifierKeys.Ctrl;
                        break;
                    case Key.Alt:
                    case Key.LAlt:
                    case Key.RALT:
                        ModifierKeys &= ~ModifierKeys.Alt;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
#endif
