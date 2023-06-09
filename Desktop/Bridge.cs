/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if MS && (GWS || Window)
using System.Collections.Generic;
using System.Windows.Forms;

namespace MnM.GWS.MS 
{
    public static class MSBridge
{
	public const TimeUnit BUnit = TimeUnit.MilliSecond;

		public static System.Drawing.PointF[] ToPointsF(this IEnumerable<int> xyPairs, bool formultipleBezier = false)
		{
			var len = xyPairs.Count();
			if (formultipleBezier)
			{
				var i = len % 3;
				if (i != 1)
				{
					len -= i;
					len += 1;
				}
			}
			var previous = -1;

			var points = new PrimitiveList<System.Drawing.PointF>(len);
			foreach (var item in xyPairs)
			{
				if (previous == -1)
					previous = item;
				else
				{
					points.Add(new System.Drawing.PointF(previous, item));
					previous = -1;
				}
			}
			return points.ToArray();
		}
	public static System.Drawing.PointF[] ToPointsF(this IEnumerable<float> xyPairs, bool formultipleBezier = false)
	{
		var len = xyPairs.Count();
		var previous = -1f;

		var points = new PrimitiveList<System.Drawing.PointF>(len);
		foreach (var item in xyPairs)
		{
			if (previous == -1)
				previous = item;
			else
			{
				points.Add(new System.Drawing.PointF(previous, item));
				previous = -1;
			}
		}
		return points.ToArray();
	}

	public static System.Drawing.PointF[] ToPointsF(params int[] xyPairs) =>
		ToPointsF(xyPairs as IEnumerable<int>);
	public static System.Drawing.PointF[] ToPointsF(params float[] xyPairs) =>
		ToPointsF(xyPairs as IEnumerable<float>);

	public static System.Drawing.PointF[] ToPointsF(bool formultipleBezier, params int[] xyPairs) =>
		ToPointsF(xyPairs as IEnumerable<int>, formultipleBezier);
	public static System.Drawing.PointF[] ToPointsF(bool formultipleBezier, params float[] xyPairs) =>
		ToPointsF(xyPairs as IEnumerable<float>, formultipleBezier);

	public static MouseButton ToGwsButton(this MouseButtons button)
	{
		switch (button)
		{
			case MouseButtons.Left:
				return MouseButton.Left;
			case MouseButtons.None:
				return MouseButton.None;
			case MouseButtons.Right:
				return MouseButton.Right;
			case MouseButtons.Middle:
				return MouseButton.Middle;
			case MouseButtons.XButton1:
				return MouseButton.Button1;
			case MouseButtons.XButton2:
				return MouseButton.Button2;
			default:
				return MouseButton.None;
		}
	}
} 
}
#endif
