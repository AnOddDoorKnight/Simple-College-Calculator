namespace HDAdvance.Mathematics;

using System;
using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// Shows a range between two values.
/// </summary>
[Serializable]
public struct Range<TNumber> where TNumber : INumber<TNumber>
{
	public static Range<TNumber> operator -(Range<TNumber> number, TNumber with)
	{
		return new Range<TNumber>(number.Min - with, number.Max - with);
	}
	public static Range<TNumber> operator +(Range<TNumber> number, TNumber with)
	{
		return new Range<TNumber>(number.Min + with, number.Max + with);
	}
	public static Range<TNumber> operator *(Range<TNumber> number, TNumber with)
	{
		return new Range<TNumber>(number.Min * with, number.Max * with);
	}
	public static Range<TNumber> operator /(Range<TNumber> number, TNumber with)
	{
		return new Range<TNumber>(number.Min / with, number.Max / with);
	}


	/// <summary>
	/// The Minimum Value of the range
	/// </summary>
	public TNumber Min
	{ 
		readonly get => _min;
		set => _min = value;
	}
	private TNumber _min;
	/// <summary>
	/// The Maximum Value of the range
	/// </summary>
	public TNumber Max
	{
		readonly get => _max;
		set => _max = value;
	}
	private TNumber _max;
	/// <summary>
	/// The Difference between <see cref="Max"/> and <see cref="Min"/>
	/// </summary>
	public readonly TNumber Difference => Max - Min;

	public Range()
	{
		_min = TNumber.Zero;
		_max = TNumber.Zero;
	}
	public Range(TNumber min, TNumber max) : this()
	{
		this.Min = min;
		this.Max = max;
	}

	/// <summary>
	/// If the value specified is within range of <see cref="Min"/> and
	/// <see cref="Max"/>.
	/// </summary>
	/// <param name="value"> The value to check if it is in range. </param>
	public readonly bool InRangeExclusive(TNumber value)
	{
		return value > Min && value < Max;
	}
	/// <summary>
	/// If the value specified is within range of <see cref="Min"/> and
	/// <see cref="Max"/>.
	/// </summary>
	/// <param name="value"> The value to check if it is in range. </param>
	public readonly bool InRangeInclusive(TNumber value)
	{
		return value >= Min && value <= Max;
	}
	/// <summary>
	/// Converts a raw value into a normalized value. If in range, returns 0.
	/// However if out of the range, it will return the normalized value. Negative
	/// if its less than, greater than 0 if greater than.
	/// </summary>
	/// <param name="input"> A raw value. </param>
	public readonly TNumber OverNormalize(TNumber input)
	{
		if (InRangeExclusive(input))
			return TNumber.Zero;
		TNumber normalized = ToNormalize(input);
		if (normalized > TNumber.Zero)
			normalized -= TNumber.One;
		return normalized;
	}
	/// <summary>
	/// Converts a raw value into a normalized value. If in range, it will be
	/// in between 0 and 1.
	/// </summary>
	/// <param name="input"> A raw value. </param>
	public readonly TNumber ToNormalize(TNumber input)
	{
		TNumber difference = input - Min;
		difference /= Difference;
		return difference;
	}
	/// <summary>
	/// Converts a normalized value into a value that is determined by the range.
	/// 0 typically only returns <see cref="Min"/>, and 1 returns <see cref="Max"/>
	/// </summary>
	/// <param name="percentage"> A normalized Value. </param>
	public readonly TNumber FromNormalize(TNumber percentage)
	{
		return Min + (Difference * percentage);
	}
	public readonly TNumber Clamp(TNumber input)
	{
		return TNumber.Clamp(input, Min, Max);
	}

	/// <summary>
	/// Converts a raw value into a normalized value. it will always be
	/// in between 0 and 1.
	/// </summary>
	/// <param name="input"> A raw value. </param>
	public readonly TNumber ToNormalize01(TNumber input)
	{
		if (input < Min)
			return TNumber.Zero;
		if (input > Max)
			return TNumber.One;
		return ToNormalize(input);
	}
	/// <summary>
	/// Converts a normalized value into a value that is determined by the range.
	/// 0 and below only returns <see cref="Min"/>, and 1 and above returns <see cref="Max"/>
	/// </summary>
	/// <param name="percentage"> A normalized Value. </param>
	public readonly TNumber FromNormalize01(TNumber percentage)
	{
		if (percentage <= TNumber.Zero)
			return Min;
		if (percentage >= TNumber.One)
			return Max;
		return FromNormalize(percentage);
	}
}

public static class RangeUtility
{
	public static Range<int> GetRangeFromCollection<T>(this IReadOnlyList<T> collection)
	{
		return new Range<int>(0, collection.Count - 1);
	}
}