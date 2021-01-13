/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if GWS || Window
namespace MnM.GWS
{
    internal sealed class Shape : IShape
    {
        #region VARIABLES
        public IRenderable Renderable;
        public Settings Settings;
        #endregion

        #region COSTRUCTORS
        internal Shape(IRenderable shape) :
            this(shape, Factory.newSettings(shape.ID))
        { }
        internal Shape(IRenderable shape, ISettings settings)
        {
            Renderable = shape;
            if (settings is Settings)
                Settings = (Settings)settings;
            else
            {
                Settings = Factory.newSettings(shape.ID);
                Settings.Receive(settings);
            }
        }
        #endregion

        #region PROPERTIES
        string IID<string>.ID => Renderable.ID;
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
            var result = Renderable.ID;
#if Advanced
            result += "," + Settings.ZOrder;
#endif
            return result;
        }
    }
}
#endif
