﻿/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IItemIndex : IParameter
    { 
        int ItemIndex { get; }
    }

    public struct ItemIndex: IItemIndex
    {
        public readonly int Index;

        public ItemIndex(int index)
        {
            Index = index;
        }

        int IItemIndex.ItemIndex => Index;
    }
}
