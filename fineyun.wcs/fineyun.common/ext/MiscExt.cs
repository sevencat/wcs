using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace fineyun.common.ext;

public static class MiscExt
{
	public static string EncodeJson(this string value)
	{
		return string.Concat
		("\"",
			value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n"),
			"\""
		);
	}

	public static string FromUtf8Bytes(this byte[] bytes)
	{
		return bytes == null
			? null
			: bytes.Length > 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF
				? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3)
				: Encoding.UTF8.GetString(bytes, 0, bytes.Length);
	}

	public static byte[] ToUtf8Bytes(this string value)
	{
		return Encoding.UTF8.GetBytes(value);
	}

	public static string LeftPart(this string strVal, char needle)
	{
		if (strVal == null) return null;
		var pos = strVal.IndexOf(needle);
		return pos == -1
			? strVal
			: strVal.Substring(0, pos);
	}

	public static string LeftPart(this string strVal, string needle)
	{
		if (strVal == null) return null;
		var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? strVal
			: strVal.Substring(0, pos);
	}

	public static string RightPart(this string strVal, char needle)
	{
		if (strVal == null) return null;
		var pos = strVal.IndexOf(needle);
		return pos == -1
			? strVal
			: strVal.Substring(pos + 1);
	}

	public static string RightPart(this string strVal, string needle)
	{
		if (strVal == null) return null;
		var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? strVal
			: strVal.Substring(pos + needle.Length);
	}

	public static string LastLeftPart(this string strVal, char needle)
	{
		if (strVal == null) return null;
		var pos = strVal.LastIndexOf(needle);
		return pos == -1
			? strVal
			: strVal.Substring(0, pos);
	}

	public static string LastLeftPart(this string strVal, string needle)
	{
		if (strVal == null) return null;
		var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? strVal
			: strVal.Substring(0, pos);
	}

	public static string LastRightPart(this string strVal, char needle)
	{
		if (strVal == null) return null;
		var pos = strVal.LastIndexOf(needle);
		return pos == -1
			? strVal
			: strVal.Substring(pos + 1);
	}

	public static string LastRightPart(this string strVal, string needle)
	{
		if (strVal == null) return null;
		var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? strVal
			: strVal.Substring(pos + needle.Length);
	}

	public static readonly string[] EmptyStringArray = Array.Empty<string>();

	public static string[] SplitOnFirst(this string strVal, char needle)
	{
		if (strVal == null) return EmptyStringArray;
		var pos = strVal.IndexOf(needle);
		return pos == -1
			? new[] { strVal }
			: new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
	}

	public static string[] SplitOnFirst(this string strVal, string needle)
	{
		if (strVal == null) return EmptyStringArray;
		var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? new[] { strVal }
			: new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
	}

	public static string[] SplitOnLast(this string strVal, char needle)
	{
		if (strVal == null) return EmptyStringArray;
		var pos = strVal.LastIndexOf(needle);
		return pos == -1
			? new[] { strVal }
			: new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
	}

	public static string[] SplitOnLast(this string strVal, string needle)
	{
		if (strVal == null) return EmptyStringArray;
		var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
		return pos == -1
			? new[] { strVal }
			: new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
	}

	public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> f)
	{
		if (dict.TryGetValue(key, out var val))
			return val;
		val = f(key);
		dict[key] = val;
		return val;
	}

	public static bool IsNullOrEmpty<E>(this List<E> lst)
	{
		if (lst == null)
			return true;
		return lst.Count == 0;
	}

	public static string Format(this string fmt, params object[] args)
	{
		return string.Format(fmt, args);
	}

	public static string Fmt(this string text, params object[] args)
	{
		return string.Format(text, args);
	}

	public static string Fmt(this string text, IFormatProvider provider, params object[] args)
	{
		return string.Format(provider, text, args);
	}

	public static string Fmt(this string text, object arg1)
	{
		return string.Format(text, arg1);
	}

	public static string Fmt(this string text, object arg1, object arg2)
	{
		return string.Format(text, arg1, arg2);
	}

	public static string Fmt(this string text, object arg1, object arg2, object arg3)
	{
		return string.Format(text, arg1, arg2, arg3);
	}

	public static string ToLowerSafe(this string value)
	{
		return value?.ToLower();
	}

	public static string ToUpperSafe(this string value)
	{
		return value?.ToUpper();
	}

	public static string SafeSubstring(this string value, int startIndex)
	{
		if (String.IsNullOrEmpty(value)) return string.Empty;
		return SafeSubstring(value, startIndex, value.Length);
	}

	public static string SafeSubstring(this string value, int startIndex, int length)
	{
		if (String.IsNullOrEmpty(value) || length <= 0) return string.Empty;
		if (startIndex < 0) startIndex = 0;
		if (value.Length >= (startIndex + length))
			return value.Substring(startIndex, length);

		return value.Length > startIndex ? value.Substring(startIndex) : string.Empty;
	}


	public static int ToInt32(this string str)
	{
		return int.Parse(str);
	}

	public static int ToInt32OrDefault(this string str, int defaultValue)
	{
		return int.TryParse(str, out var ret) ? ret : defaultValue;
	}

	public static long ToInt64(this string str)
	{
		return int.Parse(str);
	}

	public static long ToInt64OrDefault(this string str, long defaultValue)
	{
		return int.TryParse(str, out var ret) ? ret : defaultValue;
	}

	public static bool IsNotNullOrWhiteSpace(this string str)
	{
		return !String.IsNullOrWhiteSpace(str);
	}

	public static bool IsNullOrWhiteSpace(this string str)
	{
		return string.IsNullOrWhiteSpace(str);
	}

	public static bool EqualsIgnoreCase(this string lhs, string rhs)
	{
		return string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase);
	}

	public static string Reverse(this string @this)
	{
		if (@this.Length <= 1)
		{
			return @this;
		}

		char[] chars = @this.ToCharArray();
		Array.Reverse(chars);
		return new string(chars);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Clamp<T>(this T value, T min, T max)
		where T : notnull, IComparable<T>
	{
		if (value.CompareTo(min) < 0)
			return min;
		if (value.CompareTo(max) > 0)
			return max;
		return value;
	}

	public static bool IsBetween<T>(this T value, T left, T right, BoundType boundType = BoundType.Open)
		where T : notnull, IComparable<T>
	{
		int l = value.CompareTo(left), r = value.CompareTo(right);
		return (l > 0 || (l is 0 && (boundType & BoundType.LeftClosed) is not 0))
		       && (r < 0 || (r is 0 && (boundType & BoundType.RightClosed) is not 0));
	}

	public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition,
		Expression<Func<T, bool>> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}

	public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition,
		Expression<Func<T, int, bool>> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}

	public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}

	public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}
}

[Flags]
public enum BoundType : byte
{
	/// <summary>
	/// Both endpoints are not considered part of the set: (X, Y)
	/// </summary>
	Open = 0,

	/// <summary>
	/// The left endpoint value is considered part of the set: [X, Y)
	/// </summary>
	LeftClosed = 0x01,

	/// <summary>
	/// The right endpoint value is considered part of the set: (X, Y]
	/// </summary>
	RightClosed = 0x02,

	/// <summary>
	/// Both endpoints are considered part of the set.
	/// </summary>
	Closed = LeftClosed | RightClosed,
}