using System.Globalization;

namespace SmkMart.Services;

public static class Fmt
{
    public static string Rupiah(int angka)
    {
        string isi = Math.Abs((long)angka).ToString("#,##0", CultureInfo.InvariantCulture).Replace(',', '.');
        return (angka < 0 ? "-" : "") + "Rp" + isi;
    }

    public static string Tanggal(DateTime d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string Bersihkan(string? teks) =>
        (teks ?? "").Replace("|", " ").Replace("\r", " ").Replace("\n", " ").Trim();
}
