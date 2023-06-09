/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region ISERIALIZABLE
    public interface ISerializable 
    {
        byte[] GetData();
        void SetData(byte[] data, ref int readIndex);
    }
    #endregion

    public interface ISerializer
    {
        /// <summary>
        /// Gets Path of a file located on disk or network drive.</param>
        /// </summary>
        string SerializationPath { get; set; }

        /// <summary>
        /// Saves metadata for a given shape at specified path.
        /// </summary>
        /// <param name="serializableList">Collections object which can be serialized.</param>
        /// <returns></returns>
        Task<bool> Serialize(IEnumerable<ISerializable> serializableList);

        /// <summary>
        /// Reads a file located on a given path on disk or network drive and creates shape using it. 
        /// </summary>
        /// <param name="deserializableList">>Collections object which can be de-serialized.</param>
        Task<bool> DeSerialize(IEnumerable<ISerializable> deserializableList);
    }

    partial class Factory
    {
        sealed class Serializer : ISerializer
        {
            const byte Pipe = (byte)'|';

            /// <summary>
            /// Gets Path of a file located on disk or network drive.</param>
            /// </summary>
            public string SerializationPath { get; set; }

            #region SERIALIZE
            public async Task<bool> Serialize(IEnumerable<ISerializable> serializables)
            {
                if (serializables == null || SerializationPath == null)
                {
                    return false;
                }
                using (var stream = File.Open(SerializationPath, FileMode.OpenOrCreate))
                {
                    PrimitiveList<byte> list = new PrimitiveList<byte>(100);
                    
                    foreach (var serializable in serializables)
                    {
                        if (serializable == null)
                            continue;
                        var items = serializable.GetData();
                        if (items.Length > 0)
                        {
                            list.Add(Pipe);
                            list.AddRange(items);
                        }
                    }

                    var data = list.ToArray();
                    var ww = (float)Math.Sqrt(data.Length);
                    var width = (int)ww;
                    var height = (int)ww;
                    if (ww - height != 0)
                        ++height;

                    var Data = data.ToPtr(out GCHandle handle);
                    var r = await Factory.ImageProcessor.Write(Data, width, height, stream);
                    handle.Free();
                    return r;
                }
            }
            #endregion

            #region DESERIALIZE
            public async Task<bool> DeSerialize(IEnumerable<ISerializable> serializables)
            {
                if (serializables == null || SerializationPath == null 
                    || !File.Exists(SerializationPath))
                {
                    return false;
                }
                using (var stream = File.OpenRead(SerializationPath))
                {
                    var tuple = await Factory.ImageProcessor.Read(stream);
                    var Data = tuple.Item1;
                    int startIndex = 0;
                    foreach (var serializable in serializables)
                    {
                        if (serializable == null)
                            continue;
                        serializable.SetData(Data, ref startIndex);
                    }
                    return true;
                }
            }
            #endregion
        }
    }
}
