using System;
using System.Collections.Generic;
using System.Linq;

// The rules of the requirements PDF, kept free of forms and the database so they can be tested on their own.
// The forms call these; Tests\QC1.Tests checks them.
public static class QcRules
{
    // p.3-4: an inspection date may not be before the contract date (the same day is fine)
    public static bool IsBeforeContract(DateTime date, DateTime contract)
    {
        return date.Date < contract.Date;
    }

    // p.1: a part with Qty copies is numbered NAME/1 ... NAME/Qty (2C4 x5 gives 2C4/1 ... 2C4/5)
    public static IEnumerable<string> InstanceNames(string partName, int qty)
    {
        var name = (partName ?? "").Trim();
        for (int i = 1; i <= qty; i++) yield return name + "/" + i;
    }

    // p.1: C = column, B = beam, R / RA = rafter. A leading number is the phase (2C4 = phase 2).
    public static string PartType(string partName)
    {
        var s = (partName ?? "").Trim().ToUpperInvariant().TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        if (s.StartsWith("R")) return "Rafter";
        if (s.StartsWith("B")) return "Beam";
        if (s.StartsWith("C")) return "Column";
        return "";
    }

    // an empty masked date box reads "____/__/__"; prompt marks, slashes and spaces all count as blank
    public static bool IsBlankDateText(string text)
    {
        return (text ?? "").Replace("/", "").Replace("_", "").Trim().Length == 0;
    }

    // p.6: the "after" date may not be earlier than the "before" date (the same day is fine)
    public static bool EndBeforeStart(DateTime start, DateTime end)
    {
        return end.Date < start.Date;
    }

    // p.1-2 note 2: no duplicate entries (case and surrounding spaces are ignored)
    public static bool IsDuplicate(IEnumerable<string> existing, string name)
    {
        var n = (name ?? "").Trim();
        return existing.Any(x => string.Equals((x ?? "").Trim(), n, StringComparison.OrdinalIgnoreCase));
    }

    // p.1: checks for one part before it joins the list. Returns "" when it is fine, else the message to show.
    // The database also refuses Qty <= 0 and duplicate names; catching them here gives a clear message.
    public static string PartError(IEnumerable<string> existingNames, string name, int qty, decimal? length, decimal? weight)
    {
        if (string.IsNullOrWhiteSpace(name)) return "نام قطعه خالی است.";
        if (IsDuplicate(existingNames, name)) return "قطعه " + name.Trim() + " قبلاً ثبت شده است.";
        if (qty <= 0) return "تعداد قطعه باید بیشتر از صفر باشد.";
        if (length.HasValue && length.Value < 0) return "طول قطعه نمی‌تواند منفی باشد.";
        if (weight.HasValue && weight.Value < 0) return "وزن قطعه نمی‌تواند منفی باشد.";
        return "";
    }

    // p.2: checks for one connection before it joins the list. Returns "" when it is fine, else the message to show.
    // Code, plate thickness, quantity and weight are all required; no duplicate codes.
    public static string ConnectionError(IEnumerable<string> existingCodes, string code, decimal? thickness, int? qty, decimal? weight)
    {
        if (string.IsNullOrWhiteSpace(code)) return "فیلد «کد اتصال» الزامی است.";
        if (IsDuplicate(existingCodes, code)) return "اتصال «" + code.Trim() + "» قبلاً ثبت شده است.";
        if (!thickness.HasValue || thickness.Value <= 0) return "فیلد «ضخامت ورق» الزامی است.";
        if (!qty.HasValue) return "فیلد «تعداد» الزامی است.";
        if (qty.Value <= 0) return "تعداد اتصال باید بیشتر از صفر باشد.";
        if (!weight.HasValue || weight.Value < 0) return "فیلد «وزن» الزامی است.";
        return "";
    }

    // p.8: pages needed for a table of `rows` rows when a page holds `rowsPerPage` (at least one page)
    public static int TablePages(int rows, int rowsPerPage)
    {
        if (rowsPerPage <= 0) rowsPerPage = 1;
        return Math.Max(1, (rows + rowsPerPage - 1) / rowsPerPage);
    }

    // p.5: plate thickness above 15 mm is a hot cut, 15 mm or less is cold; no thickness gives no cut type
    public static string CutType(decimal? plateThickness)
    {
        if (!plateThickness.HasValue) return "";
        return plateThickness.Value > 15 ? "گرم" : "سرد";
    }

    // p.4: how many of a part's connections go into the report for a given percent (at least one)
    public static int ConnectionsToInspect(int linkedConnections, decimal percent)
    {
        if (linkedConnections <= 0) return 0;
        int take = (int)Math.Ceiling(linkedConnections * percent / 100m);
        return Math.Max(1, Math.Min(linkedConnections, take));
    }

    // p.3, p.8, p.11: how many parts the random pick takes for a given percent
    public static int RandomCount(int totalParts, decimal percent)
    {
        if (totalParts <= 0 || percent <= 0) return 0;
        int n = (int)Math.Ceiling(totalParts * percent / 100m);
        return Math.Min(totalParts, n);
    }

    // p.4: the factory makes `capacity` parts a day and a report is written every `gapBefore` days,
    // so one report date covers capacity * gapBefore parts. `index` counts parts from 0.
    public static DateTime WeldReportDate(DateTime start, int index, int capacity, int gapBefore)
    {
        int perReport = Math.Max(1, capacity * gapBefore);
        return start.AddDays((Math.Max(0, index) / perReport) * gapBefore);
    }
}
