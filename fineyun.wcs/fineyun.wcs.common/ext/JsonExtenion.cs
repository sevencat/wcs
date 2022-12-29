using System.Text.Json;
using System.Text.Json.Nodes;

namespace fineyun.wcs.common.ext;

public static class JsonExtenion
{
	public static T FromJson<T>(this string src)
	{
		return JsonSerializer.Deserialize<T>(src);
	}

	public static T FromJson<T>(this byte[] src)
	{
		return JsonSerializer.Deserialize<T>(src);
	}

	public static T FromJson<T>(this string src, JsonSerializerOptions settings)
	{
		return JsonSerializer.Deserialize<T>(src, settings);
	}

	public static string ToJson(this object src)
	{
		return JsonSerializer.Serialize(src);
	}

	public static string ToJson(this object src, JsonSerializerOptions settings)
	{
		return JsonSerializer.Serialize(src, settings);
	}

	public static JsonNode ToJsonNode(this object src)
	{
		return JsonSerializer.SerializeToNode(src);
	}

	public static JsonNode ToJsonNode(this object src, JsonSerializerOptions settings)
	{
		return JsonSerializer.SerializeToNode(src, settings);
	}


	public static T FromJsonNode<T>(this JsonNode src)
	{
		return src.Deserialize<T>();
	}

	public static T FromJsonNode<T>(this JsonNode src, JsonSerializerOptions settings)
	{
		return src.Deserialize<T>(settings);
	}


	public static string GetValue(this JsonObject obj, string key, string defvalue)
	{
		var subnode = obj[key];
		return subnode == null ? defvalue : subnode.GetValue<string>();
	}

	public static long GetValue(this JsonObject obj, string key, long defvalue)
	{
		var subnode = obj[key];
		return subnode?.GetValue<long>() ?? defvalue;
	}

	public static int GetValue(this JsonObject obj, string key, int defvalue)
	{
		var subnode = obj[key];
		return subnode?.GetValue<int>() ?? defvalue;
	}

	public static decimal GetValue(this JsonObject obj, string key, decimal defvalue)
	{
		var subnode = obj[key];
		return subnode?.GetValue<decimal>() ?? defvalue;
	}
}