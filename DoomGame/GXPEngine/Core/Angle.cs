// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using System;
using System.Diagnostics.CodeAnalysis;
using GXPEngine.GXPEngine.Utils;

namespace GXPEngine.GXPEngine.Core;
//TODO: Polish up XML documentation

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public readonly struct Angle
{
	/// <summary>
	/// When comparing values, the values can be off by this much in either direction before it gets flagged as actually two different numbers
	/// </summary>
	private const float TOLERANCE = 0.0000001f;

	// ReSharper disable InconsistentNaming
	public static readonly Angle ZERO = new(0);
	public static readonly Angle PI = new(Mathf.PI);
	public static readonly Angle HALF_PI = new(Mathf.HALF_PI);
	public static readonly Angle THIRD_PI = new(Mathf.THIRD_PI);
	public static readonly Angle QUARTER_PI = new(Mathf.QUARTER_PI);
	public static readonly Angle TWO_PI = new(Mathf.TWO_PI);
	// ReSharper restore InconsistentNaming

	private readonly float _totalRadians;

	private Angle(float totalRadians)
	{
		_totalRadians = totalRadians;
	}

	public static Angle FromRadians(float radians)
	{
		return new Angle(radians);
	}

	public static Angle FromDegrees(float degrees)
	{
		return new Angle(Deg2Rad(degrees));
	}

	public static Angle Random()
	{
		return new Angle(Utils.Utils.Random(0, TWO_PI));
	}

	/// <returns>An angle in radians between 0 and <see cref="Mathf.TWO_PI"/></returns>
	public float GetRadians()
	{
		return Mathf.Wrap(GetTotalRadians(), TWO_PI);
	}

	/// <returns>An angle in radians between -PI and PI</returns>
	public float GetRadiansWithNegative()
	{
		return Mathf.Wrap(GetTotalRadians() + PI, TWO_PI) - PI;
	}

	/// <returns>The actual angle in radians.<br/>
	/// <b>Can be less than 0 and more than <see cref="Mathf.TWO_PI"/>!</b></returns>
	public float GetTotalRadians()
	{
		return _totalRadians;
	}

	/// <returns>float degrees (0..360)</returns>
	public float GetDegrees()
	{
		return Rad2Deg(GetRadians());
	}

	/// <returns>float degrees (-180..180)</returns>
	public float GetDegreesWithNegative()
	{
		return Rad2Deg(GetRadiansWithNegative());
	}

	/// <returns>The actual angle in degrees.<br/>
	/// <b>Can be less than 0 and more than 360!</b></returns>
	public float GetTotalDegrees()
	{
		return Rad2Deg(GetTotalRadians());
	}

	private const float RAD_TO_DEG = 180.0f / Mathf.PI;
	private const float DEG_TO_RAD = Mathf.PI / 180.0f;

	/// <summary>
	/// Converts the given radians to degrees
	/// </summary>
	public static float Rad2Deg(float radians)
	{
		return radians * RAD_TO_DEG;
	}

	/// <summary>
	/// Converts the given degrees to radians
	/// </summary>
	public static float Deg2Rad(float degrees)
	{
		return degrees * DEG_TO_RAD;
	}

	/// <summary>
	/// Calculates the smallest difference between two angles<br/>
	/// (So it's always between -PI and PI)
	/// </summary>
	public static Angle Difference(Angle angle1, Angle angle2)
	{
		//TODO: Improve this
		float diff = (angle2.GetDegrees() - angle1.GetDegrees() + 180f) % 360f - 180f;
		return FromDegrees(diff < -180f ? diff + 360f : diff);
	}

	public static Angle operator +(Angle left, Angle right)
	{
		return new Angle(left.GetTotalRadians() + right.GetTotalRadians());
	}

	/// <summary>
	/// Inverts the angle (rotates by 180 degrees)
	/// </summary>
	public static Angle operator -(Angle angle)
	{
		return new Angle(-angle.GetTotalRadians());
	}

	public static Angle operator -(Angle left, Angle right)
	{
		return new Angle(left.GetTotalRadians() - right.GetTotalRadians());
	}

	public static Angle operator *(Angle angle, float f)
	{
		return new Angle(angle.GetTotalRadians() * f);
	}

	public static Angle operator *(float f, Angle angle)
	{
		return new Angle(f * angle.GetTotalRadians());
	}

	//Doesn't really make sense.
	// public static Angle operator *(Angle left, Angle right)
	// {
	// 	return new Angle(left.GetTotalRadians() * right.GetTotalRadians());
	// }

	public static Angle operator /(Angle angle, float f)
	{
		return new Angle(angle.GetTotalRadians() / f);
	}

	//Doesn't really make sense.
	// public static Angle operator /(float f, Angle angle)
	// {
	// 	return new Angle(f / angle.GetTotalRadians());
	// }

	//Doesn't really make sense.
	// public static Angle operator /(Angle left, Angle right)
	// {
	// 	return new Angle(left.GetTotalRadians() / right.GetTotalRadians());
	// }

	public static bool operator ==(Angle left, Angle right)
	{
		return Math.Abs(left.GetRadians() - right.GetRadians()) < TOLERANCE;
	}

	public static bool operator !=(Angle left, Angle right)
	{
		return Math.Abs(left.GetRadians() - right.GetRadians()) > TOLERANCE;
	}

	//TODO: make this check if the right angle is counter-clockwise in regards to the left angle
	public static bool operator <(Angle left, Angle right)
	{
		return left.GetRadians() < right.GetRadians();
	}

	//TODO: make this check if the right angle is clockwise in regards to the left angle
	public static bool operator >(Angle left, Angle right)
	{
		return left.GetRadians() > right.GetRadians();
	}

	//TODO: Same
	public static bool operator <=(Angle left, Angle right)
	{
		return left.GetRadians() <= right.GetRadians();
	}

	//TODO: Same
	public static bool operator >= (Angle left, Angle right)
	{
		return left.GetRadians() >= right.GetRadians();
	}

	public override string ToString() => $"{GetRadians()} rad";

	public static implicit operator float(Angle a) => a.GetTotalRadians();

	public override bool Equals(object obj)
	{
		if (obj is not Angle angle)
			return false;
		return Math.Abs(GetRadians() - angle.GetRadians()) < TOLERANCE;
	}

	public override int GetHashCode()
	{
		return GetRadians().GetHashCode();
	}
}
