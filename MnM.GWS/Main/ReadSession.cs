/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
namespace MnM.GWS
{
    public partial class ReadSession: IReadSession
    {
        /// <summary>
        /// Choice option for readin data from pen.
        /// </summary>
        public ReadChoice Choice;

        #region PROPERTIES
        ReadChoice IReadSession.Choice { get => Choice; set => Choice = value; }

        protected virtual ReadSession newInstance() =>
            new ReadSession();
        public object Clone()
        {
            var session = newInstance();
            session.Choice = Choice;
            Clone2(session);
            CopyTo(session);
            return session;
        }
        protected virtual void CopyTo(ReadSession session) { }
        partial void Clone2(ReadSession session);
        #endregion
    }
}
#endif
