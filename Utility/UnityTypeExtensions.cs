
using UnityEngine;
using System;
using System.Globalization;

public static class Vector2Extension
{
	public static Vector2 PUParse(this Vector2 v, string value)
	{
		var elements = value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		v.x = float.Parse (elements [0], System.Globalization.CultureInfo.InvariantCulture);
		v.y = float.Parse (elements [1], System.Globalization.CultureInfo.InvariantCulture);
		return v;
	}

	public static string PUToString(this Vector2 v)
	{
		return string.Format ("{0},{1}", v.x, v.y);
	}
}

public static class Vector3Extension
{
	public static Vector3 PUParse(this Vector3 v, string value)
	{
		var elements = value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		v.x = float.Parse (elements [0], System.Globalization.CultureInfo.InvariantCulture);
		v.y = float.Parse (elements [1], System.Globalization.CultureInfo.InvariantCulture);
		v.z = float.Parse (elements [2], System.Globalization.CultureInfo.InvariantCulture);
		return v;
	}

	public static string PUToString(this Vector3 v)
	{
		return string.Format ("{0},{1},{2}", v.x, v.y, v.z);
	}
}

public static class Vector4Extension
{
	public static Vector4 PUParse(this Vector4 v, string value)
	{
		var elements = value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		v.x = float.Parse (elements [0], System.Globalization.CultureInfo.InvariantCulture);
		v.y = float.Parse (elements [1], System.Globalization.CultureInfo.InvariantCulture);
		v.z = float.Parse (elements [2], System.Globalization.CultureInfo.InvariantCulture);
		v.w = float.Parse (elements [2], System.Globalization.CultureInfo.InvariantCulture);
		return v;
	}

	public static string PUToString(this Vector4 v)
	{
		return string.Format ("{0},{1},{2},{3}", v.x, v.y, v.z, v.w);
	}
}

public static class ColorExtension
{
	public static Color PUParse(this Color c, string value)
	{
		if (value.StartsWith ("#")) {
			int argb = Int32.Parse(value.Substring(1), NumberStyles.HexNumber);
			c.r = (float)((argb & 0xFF000000) >> 24) / 255.0f;
			c.g = (float)((argb & 0x00FF0000) >> 16) / 255.0f;
			c.b = (float)((argb & 0x0000FF00) >> 8) / 255.0f;
			c.a = (float)((argb & 0x000000FF) >> 0) / 255.0f;
			return c;
		}

		var elements = value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		c.r = float.Parse (elements [0], System.Globalization.CultureInfo.InvariantCulture);
		c.g = float.Parse (elements [1], System.Globalization.CultureInfo.InvariantCulture);
		c.b = float.Parse (elements [2], System.Globalization.CultureInfo.InvariantCulture);
		c.a = float.Parse (elements [3], System.Globalization.CultureInfo.InvariantCulture);
		return c;
	}

	public static string PUToString(this Color c)
	{
		return string.Format ("{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
	}
}
