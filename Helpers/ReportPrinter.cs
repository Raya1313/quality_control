using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

// One printable form (title band, info grid, optional sketch, table, signature boxes).
public class ReportDoc
{
    public string Title = "";
    public string Code = "";
    public string Revision = "";
    public List<string[]> Info = new List<string[]>();     // {label, value}
    public int InfoColumns = 2;
    public Image? Sketch;
    public string SketchCaption = "Sketch:";
    public string[] Headers = new string[0];
    public float[] Widths = new float[0];                  // relative widths, one per header
    public List<string[]> Rows = new List<string[]>();
    public int MinRows = 15;                               // blank rows added up to this count
    public string[] Signers = new string[0];               // "Label\nName: ..."
    public bool Landscape;
    public bool Rtl;                                       // Persian forms: columns run right to left
}

// Draws ReportDoc objects with the project logo in the header and as a faint watermark.
public class ReportPrinter
{
    readonly IList<ReportDoc> _docs;
    readonly Image? _logo = Session.LogoImage();
    readonly Font _fText = new Font("Tahoma", 8f);
    readonly Font _fBold = new Font("Tahoma", 8f, FontStyle.Bold);
    readonly Font _fTitle = new Font("Tahoma", 14f, FontStyle.Bold);
    int _di, _ri;

    ReportPrinter(IList<ReportDoc> docs) { _docs = docs; }

    public static void Preview(IList<ReportDoc> docs)
    {
        if (docs.Count == 0) return;
        var rp = new ReportPrinter(docs);
        using (var pd = new PrintDocument())
        {
            pd.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            pd.BeginPrint += (s, e) => { rp._di = 0; rp._ri = 0; };
            pd.QueryPageSettings += (s, e) =>
            {
                e.PageSettings.Landscape = rp._docs[Math.Min(rp._di, rp._docs.Count - 1)].Landscape;
            };
            pd.PrintPage += rp.OnPage;
            using (var dlg = new PrintPreviewDialog { Document = pd, Width = 1100, Height = 800 })
                dlg.ShowDialog();
        }
    }

    void OnPage(object sender, PrintPageEventArgs e)
    {
        var g = e.Graphics!;
        var d = _docs[_di];
        var b = e.MarginBounds;

        DrawWatermark(g, e.PageBounds);
        float y = b.Top;
        y = DrawHeader(g, d, b, y);
        y = DrawInfo(g, d, b, y);
        if (d.Sketch != null && _ri == 0) y = DrawSketch(g, d, b, y);

        bool done = true;
        if (d.Headers.Length > 0) done = DrawTable(g, d, b, ref y);
        if (done && d.Signers.Length > 0) DrawSigners(g, d, b);

        if (!done) { e.HasMorePages = true; return; }
        _di++;
        _ri = 0;
        e.HasMorePages = _di < _docs.Count;
    }

    // ---- pieces -----------------------------------------------------------

    void DrawWatermark(Graphics g, Rectangle page)
    {
        if (_logo == null) return;
        float w = page.Width * 0.5f;
        float h = w * _logo.Height / _logo.Width;
        if (h > page.Height * 0.5f) { h = page.Height * 0.5f; w = h * _logo.Width / _logo.Height; }

        var cm = new ColorMatrix { Matrix33 = 0.12f };
        using (var ia = new ImageAttributes())
        {
            ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var dest = new Rectangle((int)((page.Width - w) / 2), (int)((page.Height - h) / 2), (int)w, (int)h);
            g.DrawImage(_logo, dest, 0, 0, _logo.Width, _logo.Height, GraphicsUnit.Pixel, ia);
        }
    }

    float DrawHeader(Graphics g, ReportDoc d, Rectangle b, float y)
    {
        float h = 64, side = b.Width * 0.22f;
        g.DrawRectangle(Pens.Black, b.Left, y, b.Width, h);
        g.DrawLine(Pens.Black, b.Left + side, y, b.Left + side, y + h);
        g.DrawLine(Pens.Black, b.Right - side, y, b.Right - side, y + h);

        var left = new RectangleF(b.Left + 2, y, side - 4, h);
        var code = d.Code + (d.Revision.Length > 0 ? "\n" + d.Revision : "");
        using (var f = Fmt(code, StringAlignment.Center))
            g.DrawString(code, _fText, Brushes.Black, left, f);

        var mid = new RectangleF(b.Left + side, y, b.Width - 2 * side, h);
        using (var f = Fmt(d.Title, StringAlignment.Center))
            g.DrawString(d.Title, _fTitle, Brushes.Black, mid, f);

        if (_logo != null)
        {
            float pw = side - 8, ph = h - 8;
            float s = Math.Min(pw / _logo.Width, ph / _logo.Height);
            float w = _logo.Width * s, hh = _logo.Height * s;
            g.DrawImage(_logo, b.Right - side + (side - w) / 2, y + (h - hh) / 2, w, hh);
        }
        return y + h;
    }

    float CellHeight(Graphics g, string[] cell, float cw, float labelRatio)
    {
        var v = cell.Length > 1 ? cell[1] : "";
        float vw = cw * (1 - labelRatio) - 6;
        var sz = g.MeasureString(v, _fText, (int)Math.Max(10, vw));
        return Math.Max(20f, sz.Height + 6);
    }

    float DrawInfo(Graphics g, ReportDoc d, Rectangle b, float y)
    {
        if (d.Info.Count == 0) return y;
        int cols = Math.Max(1, d.InfoColumns);
        float cw = b.Width / (float)cols;
        float ratio = cols == 1 ? 0.25f : 0.42f;

        for (int i = 0; i < d.Info.Count; i += cols)
        {
            var cells = d.Info.Skip(i).Take(cols).ToList();
            float rowH = 20;
            foreach (var c in cells) rowH = Math.Max(rowH, CellHeight(g, c, cw, ratio));

            for (int k = 0; k < cells.Count; k++)
            {
                float x = d.Rtl ? b.Right - (k + 1) * cw : b.Left + k * cw;
                g.DrawRectangle(Pens.Black, x, y, cw, rowH);

                var label = cells[k][0];
                var value = cells[k].Length > 1 ? cells[k][1] : "";
                float lw = cw * ratio;
                RectangleF lr, vr;
                if (d.Rtl)
                {
                    lr = new RectangleF(x + cw - lw, y, lw, rowH);
                    vr = new RectangleF(x, y, cw - lw, rowH);
                }
                else
                {
                    lr = new RectangleF(x, y, lw, rowH);
                    vr = new RectangleF(x + lw, y, cw - lw, rowH);
                }
                using (var f = Fmt(label, d.Rtl ? StringAlignment.Far : StringAlignment.Near))
                    g.DrawString(label, _fBold, Brushes.Black, Inflate(lr), f);
                using (var f = Fmt(value, d.Rtl ? StringAlignment.Far : StringAlignment.Near))
                    g.DrawString(value, _fText, Brushes.Black, Inflate(vr), f);
            }
            y += rowH;
        }
        return y;
    }

    static RectangleF Inflate(RectangleF r)
    {
        return new RectangleF(r.X + 3, r.Y, r.Width - 6, r.Height);
    }

    float DrawSketch(Graphics g, ReportDoc d, Rectangle b, float y)
    {
        float h = Math.Min(300f, (b.Bottom - y) * 0.45f);
        g.DrawRectangle(Pens.Black, b.Left, y, b.Width, h);
        g.DrawString(d.SketchCaption, _fBold, Brushes.Black, b.Left + 4, y + 2);

        var img = d.Sketch!;
        float aw = b.Width - 10, ah = h - 22;
        float s = Math.Min(aw / img.Width, ah / img.Height);
        float w = img.Width * s, hh = img.Height * s;
        g.DrawImage(img, b.Left + (b.Width - w) / 2, y + 18 + (ah - hh) / 2, w, hh);
        return y + h;
    }

    static float[] Norm(float[] rel, int n, float total)
    {
        var w = new float[n];
        float sum = 0;
        for (int i = 0; i < n; i++) { w[i] = i < rel.Length && rel[i] > 0 ? rel[i] : 1f; sum += w[i]; }
        for (int i = 0; i < n; i++) w[i] = w[i] / sum * total;
        return w;
    }

    bool DrawTable(Graphics g, ReportDoc d, Rectangle b, ref float y)
    {
        float sigH = d.Signers.Length > 0 ? 80 : 0;
        float limit = b.Bottom - sigH - 4;
        float hh = 36, rh = 20;
        var widths = Norm(d.Widths, d.Headers.Length, b.Width);

        DrawRow(g, d, widths, b, y, hh, d.Headers, true);
        y += hh;

        int drawn = 0;
        while (_ri < d.Rows.Count && (y + rh <= limit || drawn == 0))
        {
            DrawRow(g, d, widths, b, y, rh, d.Rows[_ri], false);
            y += rh;
            _ri++;
            drawn++;
        }
        if (_ri < d.Rows.Count) return false;

        int blanks = Math.Max(0, d.MinRows - d.Rows.Count);
        var empty = new string[d.Headers.Length];
        for (int i = 0; i < d.Headers.Length; i++) empty[i] = "";
        while (blanks > 0 && y + rh <= limit)
        {
            DrawRow(g, d, widths, b, y, rh, empty, false);
            y += rh;
            blanks--;
        }
        return true;
    }

    void DrawRow(Graphics g, ReportDoc d, float[] widths, Rectangle b, float y, float h, string[] cells, bool header)
    {
        float acc = 0;
        for (int i = 0; i < widths.Length; i++)
        {
            float x = d.Rtl ? b.Right - acc - widths[i] : b.Left + acc;
            var r = new RectangleF(x, y, widths[i], h);
            if (header) g.FillRectangle(Brushes.LightGray, r);
            g.DrawRectangle(Pens.Black, r.X, r.Y, r.Width, r.Height);

            var text = i < cells.Length ? cells[i] : "";
            if (text.Length > 0)
                using (var f = Fmt(text, StringAlignment.Center))
                    g.DrawString(text, header ? _fBold : _fText, Brushes.Black, r, f);
            acc += widths[i];
        }
    }

    void DrawSigners(Graphics g, ReportDoc d, Rectangle b)
    {
        float h = 74, y = b.Bottom - h;
        float w = b.Width / (float)d.Signers.Length;
        for (int i = 0; i < d.Signers.Length; i++)
        {
            float x = d.Rtl ? b.Right - (i + 1) * w : b.Left + i * w;
            g.DrawRectangle(Pens.Black, x, y, w, h);
            var r = new RectangleF(x + 4, y + 2, w - 8, h - 4);
            using (var f = new StringFormat { Alignment = d.Rtl ? StringAlignment.Far : StringAlignment.Near })
            {
                if (d.Rtl) f.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                g.DrawString(d.Signers[i], _fBold, Brushes.Black, r, f);
            }
        }
    }

    // ---- text helpers -----------------------------------------------------

    static bool HasArabic(string s)
    {
        foreach (var c in s)
            if (c >= 0x0600 && c <= 0x06FF) return true;
        return false;
    }

    static StringFormat Fmt(string s, StringAlignment al)
    {
        var f = new StringFormat
        {
            Alignment = al,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        if (HasArabic(s)) f.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        return f;
    }
}
