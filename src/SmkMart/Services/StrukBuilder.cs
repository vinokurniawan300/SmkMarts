using System.Text;
using SmkMart.Models;

namespace SmkMart.Services;

public static class StrukBuilder
{
    private const int Lebar = 40;

    private static string Tengah(string teks) =>
        teks.Length >= Lebar ? teks : teks.PadLeft((Lebar + teks.Length) / 2).PadRight(Lebar);

    public static string Buat(string no, string tanggal, string kasir, IEnumerable<ItemKeranjang> items,
                              int total, int bayar, int kembali)
    {
        var garisGanda = new string('=', Lebar);
        var garis = new string('-', Lebar);
        var sb = new StringBuilder();

        sb.AppendLine(garisGanda);
        sb.AppendLine(Tengah("STRUK PEMBELIAN - SMK MART"));
        sb.AppendLine(garisGanda);
        sb.AppendLine("No. Transaksi : " + no);
        sb.AppendLine("Tanggal       : " + tanggal);
        sb.AppendLine("Kasir         : " + kasir);
        sb.AppendLine(garis);
        foreach (var i in items)
        {
            sb.AppendLine(i.Nama);
            sb.AppendLine($"  {i.Qty} x {Fmt.Rupiah(i.Harga)} = {Fmt.Rupiah(i.Subtotal)}");
        }
        sb.AppendLine(garis);
        sb.AppendLine("TOTAL     : " + Fmt.Rupiah(total));
        sb.AppendLine("BAYAR     : " + Fmt.Rupiah(bayar));
        sb.AppendLine("KEMBALIAN : " + Fmt.Rupiah(kembali));
        sb.AppendLine(garisGanda);
        sb.AppendLine(Tengah("Terima kasih telah berbelanja!"));
        sb.AppendLine(garisGanda);
        return sb.ToString();
    }
}
