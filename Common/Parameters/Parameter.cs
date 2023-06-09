/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif


namespace MnM.GWS
{
    public static partial class Parameters
    {
        #region CONSTS
        const byte ZERO = 0;
        const sbyte SZERO = 0;
        #endregion

        #region READONLY VARIABLES
        public static readonly IParameter[] StandardImageSavers = new IParameter[]
        {
            ImageFormat.PNG.ToParameter(),
            Command.SwapRedBlueChannel.Add()
        };
        public static readonly IScale EmptyScale = Scale.Empty;
        public static readonly ISkew EmptySkew = Skew.Empty;
        const gint OBJEMPTY = default(gint);
        #endregion

        #region EXTRACT SESSION FROM PARAMETERS
        public static void Extract(this IEnumerable<IParameter> Parameters, out
#if DevSupport
    IDevSession
#else
    ISession
#endif
            Result
        )
        {
            if (Parameters is IExSession)
            {
                Result = (IExSession)Parameters;
                return;
            }
            Result = new Session(Parameters);
        }
        internal static void Extract(this IEnumerable<IParameter> Parameters, out IExSession Result)
        {
            if (Parameters is IExSession)
            {
                Result = (IExSession)Parameters;
                return;
            }
            Result = new Session(Parameters);
        }
        #endregion

        #region EXTRACT ROTATION PARAMETRS
        /// <summary>
        /// Extracts parameters to assist and influence rendering process.
        /// </summary>
        /// <param name="Parameters">Instance representing IInLineParameters interface which contains parameters.
        /// Can be null.</param>
        /// <param name="rotation">Rotation object to render a shape in rotated state.</param>
        /// <param name="scale">Scale object to render a shape as scaled.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DevSupport
        public
#else
        internal
#endif
        static void ExtractRotationScaleParameters
        (
            this IEnumerable<IParameter> Parameters,
            out IRotation rotation,
            out IScale scale
        )
        {
            #region INITIALIZE ALL OUT VARIABLES
            rotation = null;
            scale = null;
            #endregion

            if (Parameters == null)
                return;

            #region EXTRACT PARAMETRS
            var items = Parameters;
            var rotparams = new PrimitiveList<IRotationParameter>();

            foreach (var parameter in items)
            {
                if (parameter == null)
                    continue;

                #region ROTATION
                if (parameter is IRotation)
                    rotation = ((IRotation)parameter);

                else if (parameter is IRotationParameter)
                    rotparams.Add((IRotationParameter)parameter);
                #endregion

                #region SCALE
                if (parameter is IScale)
                    scale = ((IScale)parameter);
                #endregion
            }
            #endregion

            rotparams.ModifyRotation(ref rotation);
        }
        #endregion

        #region EXTRACT IMAGE SAVE PARAMETRS
        /// <summary>
        /// Extracts parameters to assist and influence rendering process.
        /// </summary>
        /// <param name="Parameters">Instance representing IInLineParameters interface which contains parameters.
        /// Can be null.</param>
        /// <param name="rotation">Rotation object to render a shape in rotated state.</param>
        /// <param name="scale">Scale object to render a shape as scaled.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ExtractImageSaveParameters
        (
            this IEnumerable<IParameter> Parameters,
            out ImageFormat imageFormat,
            out GreyScale greyScale,
            out byte imageQuality,
            out byte imagePitch,
            out byte transparency
        )
        {
            #region INITIALIZE ALL OUT VARIABLES
            imageFormat = 0;
            greyScale = 0;
            imageQuality = 100;
            imagePitch = 4;
            transparency = 0;
            #endregion

            if (Parameters == null)
                return;

            #region EXTRACT PARAMETRS
            var items = Parameters;

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                if (item is IImageFormat)
                    imageFormat = ((IImageFormat)item).Format;

                if (item is IGreyScale)
                    greyScale = ((IGreyScale)item).GreyScale;

                if (item is ITransparency)
                    transparency = ((ITransparency)item).Transparency;

                if (item is IImagePitch)
                    imagePitch = ((IImagePitch)item).Pitch;

                if (item is IImageQuality)
                    imageQuality = ((IImageQuality)item).Quality;
            }
            #endregion
        }
        #endregion

#if (GWS || Window)
        #region EXTRACT TEXT DRAW PARAMETRS
        /// <summary>
        /// Extracts parameters to assist and influence text rendering process.
        /// </summary>
        /// <param name="Parameters">Instance representing IInLineParameters interface which contains parameters.
        /// Can be null.</param>
        /// <param name="renderBounds"></param>
        /// <param name="font"></param>
        /// <param name="rotation">Rotation object to render a shape in rotated state.</param>
        /// <param name="scale">Scale object to render a shape as scaled.</param>
        /// <param name="textCommand">Tet command to influence the rendering process.</param>
        /// <param name="maxCharDisply">a number indicating how many characters can be allowed to display.</param> 
        /// <param name="textContainer">Sorrounding container to be considered if text commnad has any alignment option/s.</param>
        /// <param name="fixedWidth">Maximum width allowed to display the text. Any character display going beyond the width will be discarded.</param>
        /// <param name="location">User supplied location of the beginning of text draw operation.</param>
        internal static void ExtractTextDrawParameters
        (
            this IEnumerable<IParameter> Parameters,
            out IRenderBounds renderBounds,
            out IRotation rotation,
            out IScale scale,
            out TextCommand textCommand,
            out int maxCharDisply,
            out IBounds textContainer,
            out int fixedWidth
        )
        {
            #region INITIALIZE ALL OUT VARIABLES
            rotation = null;
            scale = null;
            renderBounds = null;
            textCommand = 0;
            Command command = 0;
            maxCharDisply = 0;
            textCommand = 0;
            textContainer = null;
            fixedWidth = 0;
            #endregion

            if (Parameters == null)
                return;

            #region PARSE PARAMETERS
            var rotparams = new PrimitiveList<IRotationParameter>();

            foreach (var parameter in Parameters)
            {
                if (parameter == null)
                    continue;

                #region COMMAND
                if (parameter is IModifier<Command>)
                    command = ((IModifier<Command>)parameter).Modify(command);
                #endregion

                #region ROTATION
                if (parameter is IRotation)
                    rotation = ((IRotation)parameter);

                else if (parameter is IRotationParameter)
                    rotparams.Add((IRotationParameter)parameter);
                #endregion

                #region SCALE
                if (parameter is IScale)
                    scale = ((IScale)parameter);
                #endregion

                #region RENDER BOUNDS
                if (parameter is IRenderBounds)
                    renderBounds = (IRenderBounds)parameter;
                #endregion

                #region TEXT DRAW FILTER
                if (parameter is ITextCommand)
                    textCommand = ((ITextCommand)parameter).Command;
                #endregion

                #region CHAR LIMIT
                if (parameter is ICharLimit)
                    maxCharDisply = ((ICharLimit)parameter).MaxCharDisplay;
                #endregion

                #region TEXT CONTAINER
                if (parameter is IArea && ((IArea)parameter).Kind == Purpose.TextContainer)
                    textContainer = ((IArea)parameter);
                #endregion

                #region FIXED WIDTH
                if (parameter is IWrapWidth)
                    fixedWidth = ((IWrapWidth)parameter).WrapWidth;
                #endregion
            }

            rotparams.ModifyRotation(ref rotation);
            #endregion
        }

        /// <summary>
        /// Extracts parameters to assist and influence text rendering process.
        /// </summary>
        /// <param name="Parameters">Instance representing IInLineParameters interface which contains parameters.
        /// Can be null.</param>
        /// <param name="renderBounds"></param>
        /// <param name="font"></param>
        /// <param name="rotation">Rotation object to render a shape in rotated state.</param>
        /// <param name="scale">Scale object to render a shape as scaled.</param>
        /// <param name="textCommand">Tet command to influence the rendering process.</param>
        /// <param name="maxCharDisplay">a number indicating how many characters can be allowed to display.</param> 
        /// <param name="textContainer">Surrounding container to be considered if text command has any alignment option/s.</param>
        /// <param name="fixedWidth">Maximum width allowed to display the text. Any character display going beyond the width will be discarded.</param>
        /// <param name="location">User supplied location of the beginning of text draw operation.</param>
        internal static void ExtractTextDrawParameters
        (
            this IEnumerable<IParameter> Parameters,
            out TextCommand textCommand,
            out int maxCharDisplay,
            out IBounds textContainer,
            out int fixedWidth
        )
        {
            #region INITIALIZE ALL OUT VARIABLES
            textCommand = 0;
            Command command = 0;
            maxCharDisplay = 0;
            textCommand = 0;
            textContainer = null;
            fixedWidth = 0;
            #endregion

            if (Parameters == null)
                return;

            #region PARSE PARAMETERS
            var rotParams = new PrimitiveList<IRotationParameter>();

            foreach (var parameter in Parameters)
            {
                if (parameter == null)
                    continue;

                #region COMMAND
                if (parameter is IModifier<Command>)
                    command = ((IModifier<Command>)parameter).Modify(command);
                #endregion

                #region TEXT DRAW FILTER
                if (parameter is ITextCommand)
                    textCommand = ((ITextCommand)parameter).Command;
                #endregion

                #region CHAR LIMIT
                if (parameter is ICharLimit)
                    maxCharDisplay = ((ICharLimit)parameter).MaxCharDisplay;
                #endregion

                #region TEXT CONTAINER
                if (parameter is IArea && ((IArea)parameter).Kind == Purpose.TextContainer)
                    textContainer = ((IArea)parameter);
                #endregion

                #region FIXED WIDTH
                if (parameter is IWrapWidth)
                    fixedWidth = ((IWrapWidth)parameter).WrapWidth;
                #endregion
            }
            #endregion
        }
        #endregion
#endif
        #region MODIFY ROTATION
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ModifyRotation(this IReadOnlyCollection<IRotationParameter> rotparams, ref IRotation rotation)
        {
            if (rotparams.Count == 0)
                return;
            if (rotation == null)
                rotation = new Rotation();
            rotation.Modify(rotparams);
        }
        #endregion

        #region TO SKEW
        public static ISkew[] GetStandardVariations(this SkewType skewType, float step = 1f)
        {
            if (step == 0)
                step = 1;
            ISkew[] skews;
            int len;
            float j = 0;
            switch (skewType)
            {
                case SkewType.Horizontal:
                case SkewType.Vertical:
                    skews = new ISkew[(int)(90 / step)];
                    len = skews.Length;
                    for (int i = 0; i < len; i++, j += step)
                    {
                        skews[i] = new Skew(j, skewType);
                    }
                    break;
                case SkewType.Diagonal:
                    skews = new ISkew[(int)(90 / step) + 1];
                    len = skews.Length;
                    for (int i = 0; i < len; i++, j += step)
                    {
                        skews[i] = new Skew(j, skewType);
                    }
                    break;
                case SkewType.Downsize:
                    skews = new ISkew[(int)(90 / step)];
                    len = skews.Length;
                    for (int i = 0; i < len; i++, j += step)
                    {
                        skews[i] = new Skew(j, skewType);
                    }
                    break;
                default:
                    skews = new ISkew[0];
                    break;
            }
            return skews;
        }
        #endregion

        #region IS-TYPEMATCHED
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypeMatched(this object value, string TypeID)
        {
            if (string.IsNullOrEmpty(TypeID))
                return false;
            if (value == null)
                return true;
            if (value is ITypeID && ((ITypeID)value).TypeID == TypeID)
                return true;
            var t1 = Type.GetType(TypeID).GetTypeInfo();
            if (t1 == null)
                return false;
            Type t2;
            object val = value;
            if (value is IValue)
                val = ((IValue)value).Value;
            t2 = val.GetType();
            if (!t1.IsAssignableFrom(t2))
                return false;
            return true;
        }
        #endregion

        #region GET PROPERTY VALUE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V Get<V>(this IProperty Property)
        {
            if (Property == null)
                return default(V);
            if (Property is V)
                return (V)Property;
            if (Property.Value is V)
                return (V)Property.Value;
            return default(V);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Get<V>(this IProperty Property, out V value)
        {
            value = default(V);
            if (Property == null)
                return;
            if (Property is V)
            {
                value = (V)Property;
                return;
            }
            if (Property.Value is V)
                value = (V)Property.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V Get<P, V>(this IPropertyBag Bag) where P : IProperty
        {
            if (Bag == null)
                return default(V);
            var Property = Bag.Get<P>();
            if (Property is V)
            {
                return (V)(object)Property;
            }
            if (Property.Value is V)
                return (V)Property.Value;
            return default(V);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Get<P, V>(this IPropertyBag Bag, out V value) where P : IProperty
        {
            value = default(V);
            if (Bag == null)
                return;
            var Property = Bag.Get<P>();
            if (Property is V)
            {
                value = (V)(object)Property;
                return;
            }
            if (Property.Value is V)
                value = (V)Property.Value;
        }
        #endregion

#if !Advance
        #region COMMAND <-> ENUM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TEnum ToEnum<TEnum>(this Command command) where TEnum : Enum
        {
            TEnum result = default(TEnum);

            #region FILL COMMAND
            if (typeof(TEnum) == typeof(FillCommand))
            {
                FillCommand fillMode = 0;

                if ((command & Command.OriginalFill) == Command.OriginalFill)
                    fillMode |= FillCommand.OriginalFill;

                if ((command & Command.StrokeInner) == Command.StrokeInner)
                    fillMode |= FillCommand.StrokeInner;

                if ((command & Command.StrokeOuter) == Command.StrokeOuter)
                    fillMode |= FillCommand.StrokeOuter;

                if ((command & Command.DrawOutLines) == Command.DrawOutLines)
                    fillMode |= FillCommand.DrawOutLines;

                if ((command & Command.FillOddLines) == Command.FillOddLines)
                    fillMode |= FillCommand.FillOddLines;


                if ((command & Command.FloodFill) == Command.FloodFill)
                    fillMode |= FillCommand.FloodFill;

                if ((command & Command.SkipFill) == Command.SkipFill)
                    fillMode |= FillCommand.SkipFill;

                if ((command & Command.SkipDraw) == Command.SkipDraw)
                    fillMode |= FillCommand.SkipDraw;

                result = (TEnum)(object)fillMode;
                goto RETURN_RESULT;
            }
            #endregion

            #region UPDATE COMMAND
            if (typeof(TEnum) == typeof(UpdateCommand))
            {
                UpdateCommand updateCommand = 0;

                if ((command & Command.SkipDisplayUpdate) == Command.SkipDisplayUpdate)
                    updateCommand |= UpdateCommand.SkipDisplayUpdate;

                if ((command & Command.RestoreView) == Command.RestoreView)
                    updateCommand |= UpdateCommand.RestoreView;

                if ((command & Command.UpdateView) == Command.UpdateView)
                    updateCommand |= UpdateCommand.UpdateView;

                if ((command & Command.UpdateScreenOnly) == Command.UpdateScreenOnly)
                    updateCommand |= UpdateCommand.UpdateScreenOnly;

                if ((command & Command.CopyScreen) == Command.CopyScreen)
                    updateCommand |= UpdateCommand.CopyScreen;

                if ((command & Command.RestoreScreen) == Command.RestoreScreen)
                    updateCommand |= UpdateCommand.RestoreScreen;

                result = (TEnum)(object)updateCommand;
                goto RETURN_RESULT;
            }
            #endregion

            #region LINE COMMAND
            if (typeof(TEnum) == typeof(LineCommand))
            {
                LineCommand lineCommand = 0;

                if ((command & Command.Bresenham) == Command.Bresenham)
                    lineCommand |= LineCommand.Bresenham;

                if ((command & Command.DottedLine) == Command.DottedLine)
                    lineCommand |= LineCommand.DottedLine;

                if ((command & Command.DashedLine) == Command.DashedLine)
                    lineCommand |= LineCommand.DashedLine;

                if ((command & Command.DashDotDashLine) == Command.DashDotDashLine)
                    lineCommand |= LineCommand.DashDotDashLine;

                if ((command & Command.LineCap) == Command.LineCap)
                    lineCommand |= LineCommand.LineCap;

                if ((command & Command.DrawEndPixel) == Command.DrawEndPixel)
                    lineCommand |= LineCommand.DrawEndPixel;

                result = (TEnum)(object)lineCommand;
                goto RETURN_RESULT;
            }
            #endregion

            #region COPY COMMAND
            if (typeof(TEnum) == typeof(CopyCommand))
            {
                CopyCommand copyCommand = 0;

                if ((command & Command.Backdrop) == Command.Backdrop)
                    copyCommand |= CopyCommand.Backdrop;

                if ((command & Command.SizeToFit) == Command.SizeToFit)
                    copyCommand |= CopyCommand.SizeToFit;

                if ((command & Command.CopyRGBOnly) == Command.CopyRGBOnly)
                    copyCommand |= CopyCommand.CopyRGBOnly;

                if ((command & Command.SwapRedBlueChannel) == Command.SwapRedBlueChannel)
                    copyCommand |= CopyCommand.SwapRedBlueChannel;

                if ((command & Command.CopyContentOnly) == Command.CopyContentOnly)
                    copyCommand |= CopyCommand.CopyContentOnly;

                if ((command & Command.CopyOpaque) == Command.CopyOpaque)
                    copyCommand |= CopyCommand.CopyOpaque;

                result = (TEnum)(object)copyCommand;
                goto RETURN_RESULT;
            }
            #endregion

            #region CLEAR COMMAND
            if (typeof(TEnum) == typeof(ClearCommand))
            {
                ClearCommand clearCommand = 0;

                if ((command & Command.SkipDesktop) == Command.SkipDesktop)
                    clearCommand |= ClearCommand.SkipDesktop;

                if ((command & Command.Screen) == Command.Screen)
                    clearCommand |= ClearCommand.Screen;

                if ((command & Command.SkipDisplayUpdate) == Command.SkipDisplayUpdate)
                    clearCommand |= ClearCommand.NoScreenUpdate;

                result = (TEnum)(object)clearCommand;
                goto RETURN_RESULT;
            }
            #endregion

            #region BRUSH COMMAND
            if (typeof(TEnum) == typeof(BrushCommand))
            {
                BrushCommand brushCommand = 0;

                if ((command & Command.BrushFollowCanvas) == Command.BrushFollowCanvas)
                    brushCommand |= BrushCommand.BrushFollowCanvas;

                if ((command & Command.BrushInvertRotation) == Command.BrushInvertRotation)
                    brushCommand |= BrushCommand.BrushInvertRotation;

                if ((command & Command.BrushNoAutoPosition) == Command.BrushNoAutoPosition)
                    brushCommand |= BrushCommand.BrushNoAutoPosition;

                if ((command & Command.BrushNoSizeToFit) == Command.BrushNoSizeToFit)
                    brushCommand |= BrushCommand.BrushNoSizeToFit;

                result = (TEnum)(object)brushCommand;
                goto RETURN_RESULT;
            }
            #endregion

            #region COMMAND
            if (typeof(TEnum) == typeof(Command))
            {
                result = (TEnum)(object)command;
                goto RETURN_RESULT;
            }
            #endregion

            throw new Exception("Given enum type is not compatible to update GWS internal command!");

            RETURN_RESULT:
            return result;
        }
        #endregion

        #region UPDATE WITH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Update<TEnum>(this TEnum relevantCommand, ref Command Command, ModifyCommand modifyCommand)
            where TEnum : Enum
        {
            Command command = 0;
            #region UPDATE COMMAND
            if (relevantCommand is UpdateCommand)
            {
                UpdateCommand updateCommand = ((UpdateCommand)(object)relevantCommand);

                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.SkipDisplayUpdate
                        | GWS.Command.RestoreView
                        | GWS.Command.UpdateView
                        | GWS.Command.UpdateScreenOnly
                        | GWS.Command.CopyScreen
                        | Command.RestoreScreen
                    );
                }
                if ((updateCommand & UpdateCommand.SkipDisplayUpdate) == UpdateCommand.SkipDisplayUpdate)
                    command |= Command.SkipDisplayUpdate;

                if ((updateCommand & UpdateCommand.RestoreView) == UpdateCommand.RestoreView)
                    command |= Command.RestoreView;

                if ((updateCommand & UpdateCommand.UpdateView) == UpdateCommand.UpdateView)
                    command |= Command.UpdateView;

                if ((updateCommand & UpdateCommand.UpdateScreenOnly) == UpdateCommand.UpdateScreenOnly)
                    command |= Command.UpdateScreenOnly;

                if ((updateCommand & UpdateCommand.CopyScreen) == UpdateCommand.CopyScreen)
                    command |= Command.CopyScreen;

                if ((updateCommand & UpdateCommand.RestoreScreen) == UpdateCommand.RestoreScreen)
                    command |= Command.RestoreScreen;

                goto RETURN_RESULT;
            }
            #endregion

            #region FILL COMMAND
            if (relevantCommand is FillCommand)
            {
                FillCommand fillCommand = ((FillCommand)(object)relevantCommand);
                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.OriginalFill
                            | GWS.Command.StrokeInner
                            | GWS.Command.StrokeOuter
                            | GWS.Command.DrawOutLines
                            | GWS.Command.FillOddLines
                            | GWS.Command.FloodFill
                            | GWS.Command.SkipFill
                            | GWS.Command.SkipFill
                    );
                }
                if ((fillCommand & FillCommand.OriginalFill) == FillCommand.OriginalFill)
                    command |= Command.OriginalFill;

                if ((fillCommand & FillCommand.StrokeInner) == FillCommand.StrokeInner)
                    command |= Command.StrokeInner;

                if ((fillCommand & FillCommand.StrokeOuter) == FillCommand.StrokeOuter)
                    command |= Command.StrokeOuter;

                if ((fillCommand & FillCommand.DrawOutLines) == FillCommand.DrawOutLines)
                    command |= Command.DrawOutLines;

                if ((fillCommand & FillCommand.FillOddLines) == FillCommand.FillOddLines)
                    command |= Command.FillOddLines;


                if ((fillCommand & FillCommand.FloodFill) == FillCommand.FloodFill)
                    command |= Command.FloodFill;

                if ((fillCommand & FillCommand.SkipFill) == FillCommand.SkipFill)
                    command |= Command.SkipFill;

                if ((fillCommand & FillCommand.SkipDraw) == FillCommand.SkipDraw)
                    command |= Command.SkipDraw;

                goto RETURN_RESULT;
            }
            #endregion

            #region LINE COMMAND
            if (relevantCommand is LineCommand)
            {
                LineCommand lineCommand = ((LineCommand)(object)relevantCommand);
                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.Bresenham
                        | GWS.Command.DottedLine
                        | GWS.Command.DashedLine
                        | GWS.Command.DashDotDashLine
                        | GWS.Command.LineCap
                        | GWS.Command.DrawEndPixel
                    );
                }
                if ((lineCommand & LineCommand.Bresenham) == LineCommand.Bresenham)
                    command |= Command.Bresenham;

                if ((lineCommand & LineCommand.DottedLine) == LineCommand.DottedLine)
                    command |= Command.DottedLine;

                if ((lineCommand & LineCommand.DashedLine) == LineCommand.DashedLine)
                    command |= Command.DashedLine;

                if ((lineCommand & LineCommand.DashDotDashLine) == LineCommand.DashDotDashLine)
                    command |= Command.DashDotDashLine;

                if ((lineCommand & LineCommand.LineCap) == LineCommand.LineCap)
                    command |= Command.LineCap;

                if ((lineCommand & LineCommand.DrawEndPixel) == LineCommand.DrawEndPixel)
                    command |= Command.DrawEndPixel;

                goto RETURN_RESULT;
            }
            #endregion

            #region COPY COMMAND
            if (relevantCommand is CopyCommand)
            {
                CopyCommand copyCommand = ((CopyCommand)(object)relevantCommand);

                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.Backdrop
                        | GWS.Command.SizeToFit
                        | GWS.Command.CopyRGBOnly
                        | GWS.Command.SwapRedBlueChannel
                        | GWS.Command.CopyContentOnly
                        | Command.CopyOpaque
                    );
                }

                if ((copyCommand & CopyCommand.Backdrop) == CopyCommand.Backdrop)
                    command |= Command.Backdrop;

                if ((copyCommand & CopyCommand.SizeToFit) == CopyCommand.SizeToFit)
                    command |= Command.SizeToFit;

                if ((copyCommand & CopyCommand.CopyRGBOnly) == CopyCommand.CopyRGBOnly)
                    command |= Command.CopyRGBOnly;

                if ((copyCommand & CopyCommand.SwapRedBlueChannel) == CopyCommand.SwapRedBlueChannel)
                    command |= Command.SwapRedBlueChannel;

                if ((copyCommand & CopyCommand.CopyContentOnly) == CopyCommand.CopyContentOnly)
                    command |= Command.CopyContentOnly;

                if ((copyCommand & CopyCommand.CopyOpaque) == CopyCommand.CopyOpaque)
                    command |= Command.CopyOpaque;

                goto RETURN_RESULT;
            }
            #endregion

            #region CLEAR COMMAND
            if (relevantCommand is ClearCommand)
            {
                ClearCommand clearCommand = ((ClearCommand)(object)relevantCommand);
                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.SkipDesktop
                        | GWS.Command.Screen
                        | GWS.Command.SkipDisplayUpdate
                    );
                }
                if ((clearCommand & ClearCommand.SkipDesktop) == ClearCommand.SkipDesktop)
                    command |= Command.SkipDesktop;

                if ((clearCommand & ClearCommand.Screen) == ClearCommand.Screen)
                    command |= Command.Screen;

                if ((clearCommand & ClearCommand.NoScreenUpdate) == ClearCommand.NoScreenUpdate)
                    command |= Command.SkipDisplayUpdate;

                goto RETURN_RESULT;
            }
            #endregion

            #region BRUSH COMMAND
            if (relevantCommand is BrushCommand)
            {
                BrushCommand brushCommand = ((BrushCommand)(object)relevantCommand);
                if (modifyCommand == ModifyCommand.Replace)
                {
                    Command &= ~
                    (
                        GWS.Command.BrushFollowCanvas
                        | GWS.Command.BrushInvertRotation
                        | GWS.Command.BrushNoAutoPosition
                        | GWS.Command.BrushNoSizeToFit
                    );
                }
                if ((brushCommand & BrushCommand.BrushFollowCanvas) == BrushCommand.BrushFollowCanvas)
                    command |= Command.BrushFollowCanvas;

                if ((brushCommand & BrushCommand.BrushInvertRotation) == BrushCommand.BrushInvertRotation)
                    command |= Command.BrushInvertRotation;

                if ((brushCommand & BrushCommand.BrushNoAutoPosition) == BrushCommand.BrushNoAutoPosition)
                    command |= Command.BrushNoAutoPosition;

                if ((brushCommand & BrushCommand.BrushNoSizeToFit) == BrushCommand.BrushNoSizeToFit)
                    command |= Command.BrushNoSizeToFit;

                goto RETURN_RESULT;
            }
            #endregion

            #region COMMAND
            if (relevantCommand is Command)
            {
                command = ((Command)(object)relevantCommand);
                goto RETURN_RESULT;
            }
            #endregion

            throw new Exception("Given enum type is not compatible to update GWS internal command!");

            RETURN_RESULT:
            switch (modifyCommand)
            {
                case ModifyCommand.Add:
                case ModifyCommand.Replace:
                default:
                    Command = Command | command;
                    break;
                case ModifyCommand.Remove:
                    Command = Command & ~command;
                    break;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Command UpdateWith<TEnum>(this Command Command, TEnum relevantCommand, ModifyCommand modifyCommand) where TEnum : Enum
        {
            Command command = Command;
            relevantCommand.Update(ref command, modifyCommand);
            return command;

        }
        #endregion
#endif
    }
}
