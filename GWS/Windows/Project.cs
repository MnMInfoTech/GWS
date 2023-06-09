#if(GWS || Window) && Advance

using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region IPROJECT WINDOW
    /// <summary>
    /// Addional properties and methods of Windows that buffers do not have.
    /// </summary>
    public partial interface IProjectWindow : IOSMinimalWindow, IBaseWindow, IRenderWindow, IHandle, IID<string>, IFocus
    {
        void Open();

        /// <summary>
        /// Close the window and manage memory.
        /// </summary>
        void Close();
    }
    #endregion

    #region IEx-DESKTOP
    internal partial interface IExProjectWindow : IProjectWindow, IExBaseWindow, IExRenderWindow, IEventProcessor
    { }
    #endregion

    partial class ProjectWindow
    {
    }
}
#endif