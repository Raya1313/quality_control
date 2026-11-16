using System;
using System.Globalization;

public static class Calendar
{
    static readonly PersianCalendar Pc = new();

    public static string ToText(DateTime d) =>
        $"{Pc.GetYear(d):0000}/{Pc.GetMonth(d):00}/{Pc.GetDayOfMonth(d):00}";

    public static bool TryParse(string s, out DateTime d)
    {
        d = default;
        var p = s.Trim().Split('/');
        if (p.Length != 3) return false;
        if (!int.TryParse(p[0], out var y) || !int.TryParse(p[1], out var m) || !int.TryParse(p[2], out var day))
            return false;
        try { d = new DateTime(y, m, day, Pc); return true; }
        catch { return false; }
    }
}