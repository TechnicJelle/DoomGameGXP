// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
//  Except for the functions in this file that are either inspired by, or taken from Processing: https://github.com/processing/processing4/blob/master/core/src/processing/core/PVector.java
// You're allowed to learn from this, but please do not simply copy.

using System;
using System.Diagnostics.CodeAnalysis;

namespace GXPEngine.GXPEngine.Core;

//TODO: Polish up XML documentation
public struct Vector2
{
	// ReSharper disable InconsistentNaming
	public float x;
	public float y;
	// ReSharper restore InconsistentNaming

	/// <summary>
	/// When comparing values, the values can be off by this much in either direction before it gets flagged as actually two different numbers
	/// </summary>
	private const float TOLERANCE = 0.0000001f;

	/// <summary>
	/// Constructs a new vector (defaults to (0, 0) )
	/// </summary>
	/// <param name="pX">new x position</param>
	/// <param name="pY">new y position</param>
	public Vector2(float pX = 0.0f, float pY = 0.0f)
	{
		x = pX;
		y = pY;
	}

	public Vector2(double pX = 0.0, double pY = 0.0) : this((float) pX, (float) pY)
	{
	}

	public Vector2(int pX = 0, int pY = 0) : this((float) pX, (float) pY)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vector2(float f) : this(f, f)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vector2(double d) : this(d, d)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vector2(int i) : this(i, i)
	{
	}

	/// <summary>
	/// Returns a new vector pointing in the given direction
	/// </summary>
	public static Vector2 FromAngle(Angle angle)
	{
		return new Vector2(Math.Cos(angle), Math.Sin(angle));
	}

	/// <summary>
	/// Returns a new unit vector pointing in a random direction
	/// </summary>
	public static Vector2 Random()
	{
		return FromAngle(Angle.Random());
	}

	/// <summary>
	/// Set the vector to a different point
	/// </summary>
	/// <param name="x">new x position</param>
	/// <param name="y">new y position</param>
	[SuppressMessage("ReSharper", "ParameterHidesMember")]
	// ReSharper disable once InconsistentNaming
	public Vector2 SetXY(float x = 0.0f, float y = 0.0f)
	{
		this.x = x;
		this.y = y;
		return this;
	}

	/// <summary>
	/// Calculates the magnitude of the vector (using sqrt)
	/// </summary>
	/// <returns>The magnitude of the vector</returns>
	public static float Mag(Vector2 v)
	{
		return (float) Math.Sqrt(v.x * v.x + v.y * v.y);
	}

	/// <summary>
	/// Calculates the magnitude of the vector (using sqrt)
	/// </summary>
	/// <returns>The magnitude of the vector</returns>
	public float Mag()
	{
		return Mag(this);
	}

	/// <summary>
	/// Calculates the square magnitude of the vector (so there is no slow sqrt() being called)
	/// </summary>
	/// <returns>The square magnitude of the vector</returns>
	public static float MagSq(Vector2 v)
	{
		return v.x * v.x + v.y * v.y;
	}

	/// <summary>
	/// Calculates the square magnitude of the vector (so there is no slow sqrt() being called)
	/// </summary>
	/// <returns>The square magnitude of the vector</returns>
	public float MagSq()
	{
		return MagSq(this);
	}

	/// <summary>
	/// Calculates a normalized version of the vector
	/// </summary>
	/// <returns>A normalized copy of the vector</returns>
	public Vector2 Normalized()
	{
		return Normalized(this);
	}

	/// <summary>
	/// Calculates a normalized version of the vector
	/// </summary>
	/// <returns>A normalized copy of the vector</returns>
	public static Vector2 Normalized(Vector2 v)
	{
		float mag = v.Mag();
		return mag == 0 ? new Vector2() : new Vector2(v.x / mag, v.y / mag);
	}

	/// <summary>
	/// Modifies the vector to be normalized
	/// </summary>
	/// <returns>The normalized vector</returns>
	public Vector2 Normalize()
	{
		return this = Normalized();
	}

	/// <summary>
	/// Sets the magnitude of this vector
	/// </summary>
	/// <param name="mag">The desired magnitude for this vector</param>
	/// <returns>The modified vector</returns>
	public Vector2 SetMag(float mag)
	{
		Normalize();
		return this *= mag;
	}

	/// <summary>
	/// Limit the magnitude of this vector
	/// </summary>
	/// <param name="max">The maximum magnitude the vector may be</param>
	/// <returns>The modified vector</returns>
	public Vector2 Limit(float max)
	{
		return MagSq() < max * max ? this : SetMag(max);
	}

	/// <summary>
	/// Set vector heading angle (magnitude doesn't change)
	/// </summary>
	/// <returns>The modified vector</returns>
	public Vector2 SetHeading(Angle angle)
	{
		float m = Mag();
		x = (float) (m * Math.Cos(angle));
		y = (float) (m * Math.Sin(angle));
		return this;
	}

	/// <summary>
	/// Gets the vector's heading angle
	/// </summary>
	public Angle Heading()
	{
		return Angle.FromRadians((float) Math.Atan2(y, x));
	}

	/// <summary>
	/// Rotate the vector over the given angle
	/// </summary>
	/// <returns>The modified vector</returns>
	public Vector2 Rotate(Angle angle)
	{
		return this = Rotate(this, angle);
	}

	/// <summary>
	/// Rotate the vector over the given angle
	/// </summary>
	/// <returns>The modified vector</returns>
	public static Vector2 Rotate(Vector2 vec, Angle angle)
	{
		float temp = vec.x;
		vec.x = (float) (vec.x * Math.Cos(angle) - vec.y * Math.Sin(angle));
		vec.y = (float) (temp * Math.Sin(angle) + vec.y * Math.Cos(angle));
		return vec;
	}

	/// <summary>
	/// Rotate the vector around the given point over the given angle
	/// </summary>
	/// <returns></returns>
	public Vector2 RotateAround(Vector2 rotateAround, Angle angle)
	{
		this -= rotateAround;
		Rotate(angle);
		return this += rotateAround;
	}

	/// <summary>
	/// Calculates the absolute components of the vector
	/// </summary>
	/// <returns>A new vector with both components positive</returns>
	public Vector2 GetAbs()
	{
		return new Vector2(Math.Abs(x), Math.Abs(y));
	}

	/// <summary>
	/// Calculates the normal vector on this vector
	/// </summary>
	/// <returns>A new vector that points in the perpendicular direction</returns>
	public Vector2 GetNormal()
	{
		return new Vector2(-y, x);
	}

	/// <summary>
	/// Calculates the distance between this vector and another vector
	/// </summary>
	public float Dist(Vector2 other)
	{
		return Dist(this, other);
	}

	/// <summary>
	/// Calculates the distance between two vectors
	/// </summary>
	public static float Dist(Vector2 v1, Vector2 v2)
	{
		Vector2 d = v1 - v2;
		return d.Mag();
	}

	/// <summary>
	/// Calculates the square distance between this vector and another vector (so there is no slow sqrt() being called)
	/// </summary>
	public float DistSq(Vector2 other)
	{
		return DistSq(this, other);
	}

	/// <summary>
	/// Calculates the square distance between two vectors (so there is no slow sqrt() being called)
	/// </summary>
	public static float DistSq(Vector2 v1, Vector2 v2)
	{
		Vector2 d = v1 - v2;
		return d.MagSq();
	}

	/// <summary>
	/// Calculates the dot product between this vector and another vector
	/// </summary>
	public float Dot(Vector2 v)
	{
		return Dot(this, v);
	}

	/// <summary>
	/// Calculates the dot product between two vectors
	/// </summary>
	public static float Dot(Vector2 v1, Vector2 v2)
	{
		return v1.x * v2.x + v1.y * v2.y;
	}

	/// <summary>
	/// Calculates the cross product between this vector and another vector
	/// </summary>
	public float Cross(Vector2 other)
	{
		return Cross(this, other);
	}

	/// <summary>
	/// Calculates the cross product between two vectors
	/// </summary>
	public static float Cross(Vector2 v1, Vector2 v2)
	{
		return v1.x * v2.y - v1.y * v2.x;
	}

	public static Vector2 operator +(Vector2 left, Vector2 right)
	{
		return new Vector2(left.x + right.x, left.y + right.y);
	}

	public static Vector2 operator -(Vector2 vec)
	{
		return new Vector2(-vec.x, -vec.y);
	}

	public static Vector2 operator -(Vector2 left, Vector2 right)
	{
		return new Vector2(left.x - right.x, left.y - right.y);
	}

	public static Vector2 operator *(Vector2 vec, float f)
	{
		return new Vector2(vec.x * f, vec.y * f);
	}

	public static Vector2 operator *(float f, Vector2 vec)
	{
		return new Vector2(f * vec.x, f * vec.y);
	}

	/// <summary>
	/// Element-wise multiplication
	/// </summary>
	public static Vector2 operator *(Vector2 left, Vector2 right)
	{
		return new Vector2(left.x * right.x, left.y * right.y);
	}

	/// <summary>
	/// Divide a vector by a number (scalar division)
	/// </summary>
	public static Vector2 operator /(Vector2 vec, float f)
	{
		return vec / new Vector2(f);
	}

	/// <summary>
	/// Divide a number by a vector
	/// </summary>
	public static Vector2 operator /(float f, Vector2 vec)
	{
		return new Vector2(f) / vec;
	}

	/// <summary>
	/// Element-wise division
	/// </summary>
	public static Vector2 operator /(Vector2 left, Vector2 right)
	{
		return new Vector2(left.x / right.x, left.y / right.y);
	}

	public static bool operator ==(Vector2 left, Vector2 right)
	{
		return Math.Abs(left.x - right.x) < TOLERANCE && Math.Abs(left.y - right.y) < TOLERANCE;
	}

	public static bool operator !=(Vector2 left, Vector2 right)
	{
		return Math.Abs(left.x - right.x) > TOLERANCE || Math.Abs(left.y - right.y) > TOLERANCE;
	}

	public override bool Equals(object obj)
	{
		if (obj is not Vector2 vec2)
			return false;
		return Math.Abs(x - vec2.x) < TOLERANCE && Math.Abs(y - vec2.y) < TOLERANCE;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + x.GetHashCode();
		hash = hash * 31 + y.GetHashCode();
		return hash;
	}

	public override string ToString()
	{
		return $"({x},{y})";
	}
}
