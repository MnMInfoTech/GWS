/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
namespace MnM.GWS
{
    public sealed partial class ReadSession: IReadSession
    {
        /// <summary>
        /// Choice option for readin data from pen.
        /// </summary>
        public ulong Choice;

        public readonly static IReadSession Empty = new ReadSession();

        #region PROPERTIES
        ulong IReadSession.Choice { get => Choice; set => Choice = value; }

        public object Clone()
        {
            var session = new ReadSession();
            session.Choice = Choice;
            Clone2(session);
            return session;
        }
        partial void Clone2(ReadSession session);
        #endregion
    }
}
#endif
