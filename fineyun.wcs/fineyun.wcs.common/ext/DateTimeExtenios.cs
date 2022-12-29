namespace fineyun.wcs.common.ext;

public static class DateTimeExtenios
{
	public static long GetLong(this DateTime dt)
	{
		long ret = dt.Year * 10000 + dt.Month * 100 + dt.Day;
		ret = ret * 1000000 + dt.Hour * 10000 + dt.Minute * 100 + dt.Second;
		return ret;
	}

	public static int GetDay(this DateTime dt)
	{
		return dt.Year * 10000 + dt.Month * 100 + dt.Day;
	}

	public static long ToUnixTs(this DateTime value)
	{
		return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
	}

	public static long UnixTs()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	}

	public static DateTime FromDay(int dt)
	{
		int year = dt / 10000;
		dt = dt % 10000;
		int month = dt / 100;
		int day = dt % 100;
		var date1 = new DateTime(year, month, day, 8, 0, 0);
		return date1;
	}

	public static DateTime FromLdt(long ldt)
	{
		if (ldt <= 900000000)
			return FromDay(19700101);
		int dt = (int)Math.DivRem(ldt, 1000000, out var lhms);

		int year = dt / 10000;
		dt %= 10000;
		int month = dt / 100;
		int day = dt % 100;

		int hms = (int)lhms;
		int h = hms / 10000;
		hms %= 10000;
		int m = hms / 100;
		int s = hms % 100;
		var date1 = new DateTime(year, month, day, h, m, s);
		return date1;
	}
}