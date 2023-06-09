/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
using MnM.GWS;
#endif
#if NoFrameLimit
using fint = System.Int32;
#else
using fint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region ISESSION
    public partial interface ISession : ICloneable, INotToBeImplementedOutsideGWS, ICompositeParameter
    {
        /// <summary>
        /// Gets or sets Offset parameter for destination offset.
        /// </summary>
        IOffset Offset { get; set; }

        /// <summary>
        /// Gets or sets Pixel thickness value which pixel drawing operation should take account of.
        /// </summary>
        sbyte Thickness { get; set; }

        /// <summary>
        /// Gets or sets Scale object to render a shape as scaled.
        /// </summary>
        IScale Scale { get; set; }

        /// <summary>
        /// Gets or sets Interpolation mode to determine how pixel blending is handled during image drawing.
        /// </summary>
        Interpolation Interpolation { get; set; }

        /// <summary>
        ///Gets or sets Instance representing IPenContext interface.
        /// which is either IPen or IPen can be created from.
        /// </summary>
        IPenContext PenContext { get; set; }

        /// <summary>
        /// Gets or sets Transparency value which rendering operation should take account of.
        /// </summary>
        byte Transparency { get; set; }

        /// <summary>
        /// Gets or sets Area of the source to be copied for image drawing or source copy.
        /// </summary>
        IBounds CopyArea { get; set; }

        /// <summary>
        /// Gets or sets size of image to which image needs to be scaled to.
        /// </summary>
        IImageSize ImageSize { get; set; }

#if (GWS || Window)
        /// <summary>
        /// Gets or sets Stroke to be applied while rendering.
        /// </summary>
        float Stroke { get; set; }

        /// <summary>
        /// Gets a GWS type of screen object.
        /// </summary>
        ObjType Type { get; }
#endif

        /// <summary>
        /// Gets rotation object to render a shape in rotated state.
        /// </summary>
        IRotation Rotation { get; }

        /// <summary>
        /// Gets User provided location which is calculated taking account of scaling, skewing and rotation intended.
        /// Calculated X co-ordinate of destination.
        /// Calculated Y co-ordinate of destination.
        /// </summary>
        IPoint UserPoint { get; }

        /// <summary>
        /// Area of rendering operation to take place.
        /// </summary>
        IRenderBounds RenderBounds { get; }

        /// <summary>
        /// Collection of objects implementing IBoundary interface to notify affected area from rendering.
        /// </summary>
        IPrimitiveList<IUpdateArea> Boundaries { get; }

        /// <summary> 
        /// Any remaining parameters which can not be parsed for extraction will
        /// be accumulated in this collection and left to be handled by the user accordingly.
        /// </summary>
        IPrimitiveList<IParameter> UnUsedParameters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum">Type of enum. Expected enum types are:
        /// CopyCommand, UpdateCommand, ClearCommand, LineCommand, FillCommand, BrushCommand
        /// Any enum type other than these will raise an exception./</typeparam>
        /// <param name="relevantCommand">Value of enum of type TEnum.</param>
        /// <param name="modifyCommand">Instruction i.e Replace, Add or remove to modify internal command using the given value.</param>
        void UpdateWith<TEnum>(TEnum relevantCommand, ModifyCommand modifyCommand) where TEnum : Enum;
    }
    #endregion

    #region IDEV SESSION
#if DevSupport
    public
#else
    internal
#endif
    partial interface IDevSession : ISession, IExCompositeParameter, IDisposable
    {
        /// <summary>
        /// Gets or sets Rotation object to render a shape in rotated state.
        /// </summary>
        new IRotation Rotation { get; set; }

        /// <summary>
        /// Gets User provided location which is calculated taking account of scaling, skewing and rotation intended.
        /// Calculated X co-ordinate of destination.
        /// Calculated Y co-ordinate of destination.
        /// </summary>
        new IPoint UserPoint { get; set; }

#if (GWS || Window)
        /// <summary>
        /// Gets or sets a GWS type of screen object.
        /// </summary>
        new ObjType Type { get; set; }
#endif
        /// <summary>
        /// Clears all data in this object and sets it in default mode.
        /// </summary>
        void Clear();
    }
    #endregion

    #region IEx SESSION
    internal partial interface IExSession : IDevSession
    {
        /// <summary>
        /// Gets or sets a command to influence entire rendering process which includes
        /// drawing and filling of shapes as well as image draw and in the end updating display.
        /// </summary>
        Command Command { get; set; }
    }
    #endregion

    #region SESSION
    partial class Session : IExSession
    {
        #region VARIABLES
        IPenContext PenContext;
        IRotation Rotation;
        IScale Scale;
        Interpolation Interpolation;

        Command Command;
        IOffset Offset;
        IPoint UserPoint;
        IImageSize imageSize;
        sbyte Thickness;
        IRenderBounds RenderBounds = new RenderBounds();
        IPrimitiveList<IParameter> UnUsedParameters = new PrimitiveList<IParameter>();
        IPrimitiveList<IUpdateArea> Boundaries = new PrimitiveList<IUpdateArea>();

        byte Transparency;
        IBounds CopyArea;
#if (GWS || Window)
        float Stroke;
        ObjType Type;
#endif
        #endregion

        #region CONSTUCTORS
        public Session() { }
        public Session(IEnumerable<IParameter> parameters)
        {
            ((IExCompositeParameter)this).CopyFrom(parameters);
        }
        #endregion

        #region PROPERTIES
        #region ISESSION IMPLEMENTATION
        IOffset ISession.Offset
        {
            get => Offset;
            set => Offset = value;
        }
        sbyte ISession.Thickness
        {
            get => Thickness;
            set => Thickness = value;
        }
        IPenContext ISession.PenContext
        {
            get => PenContext;
            set => PenContext = value;
        }
        IScale ISession.Scale
        {
            get => Scale;
            set => Scale = value;
        }
        Interpolation ISession.Interpolation
        {
            get => Interpolation;
            set => Interpolation = value;
        }
        IImageSize ISession.ImageSize
        {
            get => imageSize;
            set => imageSize = value;
        }
        byte ISession.Transparency
        {
            get => Transparency;
            set => Transparency = value;
        }
#if (GWS || Window)
        float ISession.Stroke
        {
            get => Stroke;
            set => Stroke = value;
        }

        ObjType ISession.Type => Type;
        ObjType IDevSession.Type { get => Type; set => Type = value; }
#endif
        IBounds ISession.CopyArea
        {
            get => CopyArea;
            set => CopyArea = value;
        }

        IPrimitiveList<IUpdateArea> ISession.Boundaries => Boundaries;
        IPoint ISession.UserPoint => UserPoint;
        IRotation ISession.Rotation => Rotation;
        IPrimitiveList<IParameter> ISession.UnUsedParameters => UnUsedParameters;
        IRenderBounds ISession.RenderBounds => RenderBounds;
        #endregion

        #region IDEV-SESSION IMPLEMENTATION
        IPoint IDevSession.UserPoint
        {
            get => UserPoint;
            set
            {
                if (value != null)
                    UserPoint = new Point(value);
                else
                    UserPoint = null;
            }
        }
        IRotation IDevSession.Rotation { get => Rotation; set => Rotation = value; }
        #endregion

        #region IEX-SESSION IMPLEMENTATION
        Command IExSession.Command { get => Command; set => Command = value; }
        #endregion
        #endregion

        #region CLONE
        public object Clone()
        {
            IExSession info = new Session();
            info.CopyFrom(this);
            return info;
        }
        #endregion

        #region COPY FROM
        void IExCompositeParameter.CopyFrom(IEnumerable<IParameter> Parameters, bool incrementalOffset)
        {
            if (Parameters == null)
                return;

            #region DIRECT COPY FROM ANOTHER SESSION
            if (Parameters is IExSession)
            {
                var session = ((IExSession)Parameters);
                Command = session.Command;
                if (incrementalOffset && session.Offset != null)
                    Offset = new Offset(Offset, session.Offset.X, session.Offset.Y);
                else
                    Offset = session.Offset;
                Thickness = session.Thickness;
                if (session.UserPoint != null)
                    UserPoint = new Point(session.UserPoint);
                RenderBounds.Set(session.RenderBounds);

                PenContext = session.PenContext;
                Interpolation = session.Interpolation;
                Rotation = session.Rotation;
                Scale = session.Scale;
                Boundaries = session.Boundaries;
                imageSize = session.ImageSize;

#if (GWS || Window)
                Stroke = session.Stroke;
                Type = session.Type;
#endif
                if (session.UnUsedParameters != null && session.UnUsedParameters.Count > 0)
                    UnUsedParameters = new PrimitiveList<IParameter>(session.UnUsedParameters);

                CopyFrom(session);
                return;
            }
            #endregion

            #region INITIALIZE VARIABLES
            ObjType SuppliedType = 0;
            int offX = Offset?.X ?? 0;
            int offY = Offset?.Y ?? 0;
            bool HasOffset = false;
            FillCommand FillCommand = Command.ToEnum<FillCommand>();
            CopyCommand CopyCommand = Command.ToEnum<CopyCommand>();
            UpdateCommand UpdateCommand = Command.ToEnum<UpdateCommand>();
            ClearCommand ClearCommand = Command.ToEnum<ClearCommand>();
            LineCommand LineCommand = Command.ToEnum<LineCommand>();
            BrushCommand BrushCommand = Command.ToEnum<BrushCommand>();
            bool handled;
            var unused = new PrimitiveList<IParameter>();
            var rotParams = new PrimitiveList<IRotationParameter>();
            #endregion

            #region EXTRACT PARAMETERS
            foreach (var parameter in Parameters)
            {
                if (parameter == null)
                    continue;

                handled = false;

                #region COMPOSITE PARAMETER
                if (parameter is IEnumerable<IParameter>)
                {
                    ((IExCompositeParameter)this).CopyFrom((IEnumerable<IParameter>)parameter);
                    continue;
                }
                #endregion

                #region TRANSPARENCY
                if (parameter is ITransparency)
                {
                    Transparency = ((ITransparency)parameter).Transparency;
                    handled = true;
                }
                #endregion

                #region AREA
                if (parameter is IArea)
                {
                    var area = (IArea)parameter;
                    if (area.Valid)
                    {
                        switch (area.Kind)
                        {
                            case Purpose.Copy:
                                CopyArea = area;
                                handled = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                #endregion

                #region OFFSET
                if (parameter is IOffset)
                {
                    var off = (IOffset)parameter;
                    if (incrementalOffset)
                    {
                        offX += off.X;
                        offY += off.Y;
                    }
                    else
                    {
                        offX = off.X;
                        offY = off.Y;
                    }
                    handled = true;
                    HasOffset = true;
                }
                #endregion

                #region USER POINT
                if (parameter is IUserPoint)
                {
                    UserPoint = (IUserPoint)parameter;
                    handled = true;
                }
                #endregion

                #region COMMAND
                if (parameter is IModifier<Command>)
                {
                    Command = ((IModifier<Command>)parameter).Modify(Command);
                    handled = true;
                }
                #endregion

                #region FILL COMMAND
                if (parameter is IModifier<FillCommand>)
                {
                    FillCommand = ((IModifier<FillCommand>)parameter).Modify(FillCommand);
                    handled = true;
                }
                #endregion

                #region LINE COMMAND
                if (parameter is IModifier<LineCommand>)
                {
                    LineCommand = ((IModifier<LineCommand>)parameter).Modify(LineCommand);
                    handled = true;
                }
                #endregion

                #region COPY COMMAND
                if (parameter is IModifier<CopyCommand>)
                {
                    CopyCommand = ((IModifier<CopyCommand>)parameter).Modify(CopyCommand);
                    handled = true;
                }
                #endregion

                #region CLEAR COMMAND
                if (parameter is IModifier<ClearCommand>)
                {
                    ClearCommand = ((IModifier<ClearCommand>)parameter).Modify(ClearCommand);
                    handled = true;
                }
                #endregion

                #region UPDATE COMMAND
                if (parameter is IModifier<UpdateCommand>)
                {
                    UpdateCommand = ((IModifier<UpdateCommand>)parameter).Modify(UpdateCommand);
                    handled = true;
                }
                #endregion

                #region BRUSH COMMAND
                if (parameter is IModifier<BrushCommand>)
                {
                    BrushCommand = ((IModifier<BrushCommand>)parameter).Modify(BrushCommand);
                    handled = true;
                }
                #endregion

                #region THICKNESS
                if (parameter is IThickness)
                {
                    Thickness = ((IThickness)parameter).Thickness;
                    handled = true;
                }
                #endregion

                #region RENDER BOUNDS
                if (parameter is IRenderBounds)
                {
                    RenderBounds.Set((IRenderBounds)parameter);
                    handled = true;
                }
                #endregion

                #region PENCONTEXT
                if (parameter is IPenContext)
                {
                    PenContext = ((IPenContext)parameter);
                    handled = true;
                }
                #endregion

                #region ROTATION
                if (parameter is IRotation)
                {
                    Rotation = ((IRotation)parameter);
                    handled = true;
                }
                else if (parameter is IRotationParameter)
                {
                    rotParams.Add((IRotationParameter)parameter);
                    handled = true;
                }
                #endregion

                #region SCALE
                if (parameter is IScale)
                {
                    Scale = ((IScale)parameter);
                    handled = true;
                }
                #endregion

                #region INTERPOLATION
                if (parameter is IValue<Interpolation>)
                {
                    Interpolation = ((IValue<Interpolation>)parameter).Value;
                    handled = true;
                }
                #endregion

                #region IMAGE SIZE
                if (parameter is IImageSize)
                {
                    imageSize = ((ImageSize)parameter);
                    handled = true;
                }
                #endregion

                #region BOUNDARY
                if (parameter is IExBoundary)
                {
                    var boundary = (IExBoundary)parameter;
                    Boundaries.Add(boundary);
                    handled = true;
                }
                #endregion

#if (GWS || Window)
                #region STROKE
                if (parameter is IStroke)
                    Stroke = ((IStroke)parameter).Stroke;
                #endregion

                #region TYPE
                if (parameter is IExType)
                {
                    SuppliedType = ((IType)parameter).Type;
                    handled = true;
                }
                else if (parameter is IType)
                {
                    Type = ((IType)parameter).Type;
                    handled = true;
                }
                #endregion
#endif
                if (!handled)
                    unused.Add(parameter);
            }
            #endregion

#if (GWS || Window)
            #region CONSOLIDATE TYPE
            if (SuppliedType != 0)
                Type = SuppliedType;
            #endregion
#endif
            #region CONSOLDATE COMMAND
            if (CopyCommand != 0)
                CopyCommand.Update(ref Command, ModifyCommand.Replace);

            if (ClearCommand != 0)
                ClearCommand.Update(ref Command, ModifyCommand.Replace);

            if (LineCommand != 0)
                LineCommand.Update(ref Command, ModifyCommand.Replace);

            if (FillCommand != 0)
                FillCommand.Update(ref Command, ModifyCommand.Replace);

            if (BrushCommand != 0)
                BrushCommand.Update(ref Command, ModifyCommand.Replace);

            if (UpdateCommand != 0)
                UpdateCommand.Update(ref Command, ModifyCommand.Replace);
            #endregion

            #region CONSOLIDATE OFFSET
            if (HasOffset)
                Offset = new Offset(offX, offY);
            #endregion

            #region CONSOLIDATE ROTATION
            if (rotParams.Count > 0)
            {
                if (Rotation == null)
                    Rotation = new Rotation();
                Rotation.Modify(rotParams);
            }
            #endregion

            #region UPDATE UNUSED PARAMETERS COLLECTION
            if (unused.Count > 0)
            {
                UnUsedParameters.AddRange(unused);
                ParseRemainingParameters();
            }
            #endregion
        }
        partial void CopyFrom(IExSession session);
        partial void ParseRemainingParameters();
        #endregion

        #region ENUMERATION
        IEnumerator<IParameter> IEnumerable<IParameter>.GetEnumerator()
        {
            yield return Command.Replace();

            yield return Interpolation.ToParameter();
            yield return Thickness.ToThickness();

            if (PenContext != null)
                yield return PenContext;
            if (Rotation != null)
                yield return Rotation;
            if (Scale != null)
                yield return Scale;
            if (UserPoint != null)
                yield return new Point(UserPoint);

            if (Offset != null)
                yield return Offset;
            if (RenderBounds != null)
                yield return RenderBounds;

            if (Boundaries.Count > 0)
            {
                foreach (var item in Boundaries)
                    yield return item;
            }

#if (GWS || Window)
            if (Stroke != 0)
                yield return new Stroke(Stroke);
            yield return (ScrObj)Type;
#endif
            if (UnUsedParameters.Count > 0)
            {
                foreach (var item in UnUsedParameters)
                    yield return item;
            }

            yield return (Transparency).ToTransparency();

            if (CopyArea != null)
                yield return new Area(CopyArea, Purpose.Copy);

            if (imageSize != null && imageSize.Valid)
                yield return imageSize;
            IEnumerator<IParameter> enumerator = null;
            GetEnumerator(ref enumerator);
            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<IParameter>)this).GetEnumerator();
        partial void GetEnumerator(ref IEnumerator<IParameter> enumerator);
        #endregion

        #region CLEAR
        void IDevSession.Clear()
        {
            PenContext = null;
            Rotation = null;
            Scale = null;
            Interpolation = 0;
            imageSize = null;
            Command = 0;

            Offset = null;
            UserPoint = null;
            Thickness = 0;
            RenderBounds = null;
            UnUsedParameters.Clear();
            Boundaries.Clear();

            Transparency = 0;
            CopyArea = null;
#if (GWS || Window)
            Stroke = 0;
            Type = 0;
#endif
            Clear();
        }
        partial void Clear();
        #endregion

        #region UPDATE WITH
        void ISession.UpdateWith<TEnum>(TEnum relevantCommand, ModifyCommand modifyCommand)
        {
            relevantCommand.Update(ref Command, modifyCommand);
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            ((IExSession)this).Clear();
        }

        #endregion
    }
#endregion
}
