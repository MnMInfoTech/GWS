/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    public sealed partial class Shape : IShape
    {
        #region VARIABLES
        public IRenderable Renderable;
        internal Settings Settings;
        #endregion

        #region COSTRUCTORS
        public Shape(IRenderable shape) 
        {
            Renderable = shape;
            Settings = (shape is ISettingsHolder) ? 
                (Settings)((ISettingsHolder)shape).Settings : new Settings(shape.ID);
        }
        #endregion

        #region PROPERTIES
        uint IID<uint>.ID => Renderable.ID;
        IRenderable IShape.Renderable => Renderable;
        ISettings IShape.Settings => Settings;
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            Renderable = null;
            Settings = null;
        }
        #endregion

        public override string ToString()
        {
            var result = Renderable.Name + "";
#if Advanced
            result += "," + Settings.ZOrder;
#endif
            return result;
        }
    }
}
#endif
