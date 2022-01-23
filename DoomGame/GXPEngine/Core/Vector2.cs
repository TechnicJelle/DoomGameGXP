using System;
using System.Diagnostics.CodeAnalysis;

namespace GXPEngine.Core
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "ParameterHidesMember")]
	//File basically copied from Processing: https://github.com/processing/processing/blob/master/core/src/processing/core/PVector.java
	public class Vector2
	{
		public float x;
		public float y;

		public Vector2()
		{
			x = 0;
			y = 0;
		}

		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2 Set(float x, float y) {
			this.x = x;
			this.y = y;
			return this;
		}

		public Vector2 Set(float[] source) {
			if (source.Length < 2) return this;
			x = source[0];
			y = source[1];
			return this;
		}

		//TODO: Random()

		public static Vector2 FromAngle(float angle, Vector2 target = null) {
			if (target == null) {
				target = new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
			} else {
				target.Set(Mathf.Cos(angle),Mathf.Sin(angle));
			}
			return target;
		}

		public Vector2 Copy() {
			return new Vector2(x, y);
		}

		public float[] Get(float[] target) {
			if (target == null) {
				return new[] { x, y };
			}
			if (target.Length < 2) return target;
			target[0] = x;
			target[1] = y;
			return target;
		}

		public float Mag() {
			return Mathf.Sqrt(x*x + y*y);
		}

		public float MagSq() {
			return x*x + y*y;
		}

		public Vector2 Add(Vector2 v) {
			x += v.x;
			y += v.y;
			return this;
		}

		public Vector2 Add(float x, float y) {
			this.x += x;
			this.y += y;
			return this;
		}

		public static Vector2 Add(Vector2 v1, Vector2 v2, Vector2 target = null) {
			if (target == null) {
				target = new Vector2(v1.x + v2.x,v1.y + v2.y);
			} else {
				target.Set(v1.x + v2.x, v1.y + v2.y);
			}
			return target;
		}

		public Vector2 Sub(Vector2 v) {
			x -= v.x;
			y -= v.y;
			return this;
		}

		public Vector2 Sub(float x, float y) {
			this.x -= x;
			this.y -= y;
			return this;
		}

		public static Vector2 Sub(Vector2 v1, Vector2 v2, Vector2 target = null) {
			if (target == null) {
				target = new Vector2(v1.x - v2.x, v1.y - v2.y);
			} else {
				target.Set(v1.x - v2.x, v1.y - v2.y);
			}
			return target;
		}

		public Vector2 Mult(float n) {
			x *= n;
			y *= n;
			return this;
		}

		public static Vector2 Mult(Vector2 v, float n, Vector2 target = null) {
			if (target == null) {
				target = new Vector2(v.x*n, v.y*n);
			} else {
				target.Set(v.x*n, v.y*n);
			}
			return target;
		}

		public Vector2 Div(float n) {
			x /= n;
			y /= n;
			return this;
		}

		public static Vector2 Div(Vector2 v, float n, Vector2 target = null) {
			if (target == null) {
				target = new Vector2(v.x/n, v.y/n);
			} else {
				target.Set(v.x/n, v.y/n);
			}
			return target;
		}

		public float Dist(Vector2 v) {
			float dx = x - v.x;
			float dy = y - v.y;
			return Mathf.Sqrt(dx*dx + dy*dy);
		}

		public static float Dist(Vector2 v1, Vector2 v2) {
			float dx = v1.x - v2.x;
			float dy = v1.y - v2.y;
			return Mathf.Sqrt(dx*dx + dy*dy);
		}

		public float Dot(Vector2 v) {
			return x*v.x + y*v.y;
		}

		public float Dot(float x, float y) {
			return this.x*x + this.y*y;
		}

		public static float Dot(Vector2 v1, Vector2 v2) {
			return v1.x*v2.x + v1.y*v2.y;
		}

		//TODO: Cross()

		public Vector2 Normalize() {
			float m = Mag();
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (m != 0.0 && m != 1.0) {
				Div(m);
			}
			return this;
		}

		public Vector2 Normalize(Vector2 target) {
			if (target == null) {
				target = new Vector2();
			}
			float m = Mag();
			if (m > 0) {
				target.Set(x/m, y/m);
			} else {
				target.Set(x, y);
			}
			return target;
		}

		public Vector2 Limit(float max) {
			if (!(MagSq() > max * max)) return this;
			Normalize();
			Mult(max);
			return this;
		}

		public Vector2 SetMag(float len) {
			Normalize();
			Mult(len);
			return this;
		}

		public Vector2 SetMag(Vector2 target, float len) {
			target = Normalize(target);
			target.Mult(len);
			return target;
		}

		public float Heading() {
			float angle = Mathf.Atan2(y, x);
			return angle;
		}

		public Vector2 Rotate(float theta) {
			float temp = x;
			// Might need to check for rounding errors like with angleBetween function?
			x = x*Mathf.Cos(theta) - y*Mathf.Sin(theta);
			y = temp*Mathf.Sin(theta) + y*Mathf.Cos(theta);
			return this;
		}

		//TODO: Lerp()

		public static float AngleBetween(Vector2 v1, Vector2 v2) {

			// We get NaN if we pass in a zero vector which can cause problems
			// Zero seems like a reasonable angle between a (0,0,0) vector and something else
			if (v1.x == 0 && v1.y == 0) return 0.0f;
			if (v2.x == 0 && v2.y == 0) return 0.0f;

			double dot = v1.x * v2.x + v1.y * v2.y;
			double v1Mag = Mathf.Sqrt(v1.x * v1.x + v1.y * v1.y);
			double v2Mag = Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y);
			// This should be a number between -1 and 1, since it's "normalized"
			double amt = dot / (v1Mag * v2Mag);
			// But if it's not due to rounding error, then we need to fix it
			// http://code.google.com/p/processing/issues/detail?id=340
			// Otherwise if outside the range, acos() will return NaN
			// http://www.cppreference.com/wiki/c/math/acos
			if (amt <= -1) {
				return Mathf.PI;
			}

			if (amt >= 1) {
				// http://code.google.com/p/processing/issues/detail?id=435
				return 0;
			}
			return (float) Math.Acos(amt);
		}

		public static float AngleBetween2(Vector2 v1, Vector2 v2) {
			//Thanks to https://github.com/EV4gamer for making this one!
			float a = Mathf.Atan2(v2.y, v2.x) - Mathf.Atan2(v1.y, v1.x);
			return (a + Mathf.TWO_PI) % Mathf.TWO_PI;
		}

		public override string ToString() {
			return "[ " + x + ", " + y + " ]";
		}
	}
}

