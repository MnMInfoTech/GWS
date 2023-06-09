/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)

using System.Collections.Generic;

namespace MnM.GWS
{
    public interface IImageList : IPrimitiveList<IImageSource>,
        IAdd<IImageSource, IObject>, IAddRange<IImageSource, IObject>
    {
        /// <summary>
        /// Adds an image located at a specified path.
        /// </summary>
        /// <param name="path">Path where the image is located.</param>
        void Add(string path);

        /// <summary>
        /// Adds multiple images located at specified paths.
        /// </summary>
        /// <param name="paths">Paths where the images are located.</param>
        void AddRange(params string[] paths);
    }

    internal sealed class ImageList: PrimitiveList<IImageSource>, IImageList
    {
        #region CONSTRUCTOR
        internal ImageList() { }
        internal ImageList(int capacity) :
            base(capacity)
        { }
        #endregion

        #region ADD
        public void Add(IObject shape)
        {
            if (shape is IImageSource)
            {
                Add((IImageSource)shape);
                return;
            }
            var surface = Factory.newCanvas(shape);
            Add(surface);
        }
        public void Add(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var surface = Factory.newCanvas(path, true);
            Add(surface);
        }
        #endregion

        #region INSERT
        public void Insert(int position, IObject shape)
        {
            if (shape is IImageSource)
            {
                Insert(position, (IImageSource)shape);
                return;
            }
            var surface = Factory.newCanvas(shape);
            Insert(position, surface);
        }
        #endregion

        #region ADD RANGE
        public void AddRange(params IObject[] shapes)
        {
            if (shapes == null || shapes.Length == 0)
                return;
            AddRange(shapes);       
        }
        public void AddRange(IEnumerable<IObject> items)
        {
            if(items == null)
                return;
            foreach (var shape in items)
            {
                if (shape is ICanvas)
                {
                    Add((ICanvas)shape);
                    continue;
                }
                var surface = Factory.newCanvas(shape);
                Add(surface);
            }
        }
        public void AddRange(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                var surface = Factory.newCanvas(path, true);
                Add(surface);
            }
        }
        #endregion
    }
}
#endif
