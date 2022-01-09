using System;

namespace GXPEngine
{
	/// <summary>
	/// Contains several functions for doing floating point Math
	/// </summary>
	public static class Mathf
	{
		/// <summary>
		/// Constant PI
		/// </summary>
		public const float PI = (float)Math.PI;

		/// <summary>
		/// Returns the absolute value of specified number
		/// </summary>
		public static int Abs (int value) {
			return (value<0)?-value:value;
		}

		/// <summary>
		/// Returns the absolute value of specified number
		/// </summary>
		public static float Abs(float value) {
			return (value<0)?-value:value;
		}

		/// <summary>
		/// Returns the acosine of the specified number
		/// </summary>
		public static float Acos(float f) {
			return (float)Math.Acos(f);
		}

		/// <summary>
		/// Returns the arctangent of the specified number
		/// </summary>
		public static float Atan(float f) {
			return (float)Math.Atan (f);
		}

		/// <summary>
		/// Returns the angle whose tangent is the quotent of the specified values
		/// </summary>
		public static float Atan2(float y, float x) {
			return (float)Math.Atan2 (y, x);
		}

		/// <summary>
		/// Returns the smallest integer bigger greater than or equal to the specified number
		/// </summary>
		public static int Ceiling(float a) {
			return (int)Math.Ceiling (a);
		}

		/// <summary>
		/// Returns the cosine of the specified number
		/// </summary>
		public static float Cos(float f) {
			return (float)Math.Cos (f);
		}

		/// <summary>
		/// Returns the hyperbolic cosine of the specified number
		/// </summary>
		public static float Cosh(float value) {
			return (float)Math.Cosh (value);
		}

		/// <summary>
		/// Returns e raised to the given number
		/// </summary>
		public static float Exp(float f) {
			return (float)Math.Exp (f);
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified value
		/// </summary>
		public static int Floor(float f) {
			return (int)Math.Floor (f);
		}

		/// <summary>
		/// Returns the natural logarithm of the specified number
		/// </summary>
		public static float Log(float f) {
			return (float)Math.Log (f);
		}

		/// <summary>
		/// Returns the log10 of the specified number
		/// </summary>
		public static float Log10(float f) {
			return (float)Math.Log10(f);
		}

		/// <summary>
		/// Returns the biggest of the two specified values
		/// </summary>
		public static float Max(float value1, float value2) {
			return (value2 > value1)?value2:value1;
		}

		/// <summary>
		/// Returns the biggest of the two specified values
		/// </summary>
		public static int Max(int value1, int value2) {
			return (value2 > value1)?value2:value1;
		}

		/// <summary>
		/// Returns the smallest of the two specified values
		/// </summary>
		public static float Min(float value1, float value2) {
			return (value2<value1)?value2:value1;
		}

		/// <summary>
		/// Returns the smallest of the two specified values
		/// </summary>
		public static int Min(int value1, int value2) {
			return (value2<value1)?value2:value1;
		}
		
		/// <summary>
		/// Returns x raised to the power of y
		/// </summary>
		public static float Pow(float x, float y) {
			return (float)Math.Pow (x, y);
		}

		/// <summary>
		/// Returns the nearest integer to the specified value
		/// </summary>
		public static int Round(float f) {
			return (int)Math.Round (f);
		}

		/// <summary>
		/// Returns a value indicating the sign of the specified number (-1=negative, 0=zero, 1=positive)
		/// </summary>
		public static int Sign(float f) {
			if (f < 0) return -1;
			if (f > 0) return 1;
			return 0;
		}

		/// <summary>
		/// Returns a value indicating the sign of the specified number (-1=negative, 0=zero, 1=positive)
		/// </summary>
		public static int Sign(int f) {
			if (f < 0) return -1;
			if (f > 0) return 1;
			return 0;
		}

		/// <summary>
		/// Returns the sine of the specified number
		/// </summary>
		public static float Sin(float f) {
			return (float)Math.Sin (f);
		}
		
		/// <summary>
		/// Returns the hyperbolic sine of the specified number
		/// </summary>
		public static float Sinh(float value) {
			return (float)Math.Sinh (value);
		}

		/// <summary>
		/// Returns the square root of the specified number
		/// </summary>
		public static float Sqrt(float f) {
			return (float)Math.Sqrt (f);
		}

		/// <summary>
		/// Returns the tangent of the specified number
		/// </summary>
		public static float Tan(float f) {
			return (float)Math.Tan (f);
		}
		
		/// <summary>
		/// Returns the hyperbolic tangent of the specified number
		/// </summary>
		public static float Tanh(float value) {
			return (float)Math.Tanh (value);
		}

		/// <summary>
		/// Calculates the integral part of the specified number
		/// </summary>
		public static float Truncate(float f) {
			return (float)Math.Truncate (f);
		}

		/// <summary>
		/// Clamps f in the range [min,max]:
		/// Returns min if f<min, max if f>max, and f otherwise.
		/// </summary>
		public static float Clamp(float f, float min, float max) {
			return f < min ? min : (f > max ? max : f);
		}

	}
}

