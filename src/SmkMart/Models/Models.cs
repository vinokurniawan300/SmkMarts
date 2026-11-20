namespace SmkMart.Models;

public static class Peran
{
    public const string Admin = "Admin";
    public const string Kasir = "Kasir";
    public const string Gudang = "Gudang";

    public static readonly string[] Semua = { Admin, Kasir, Gudang };
}

public class Pengguna
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
    public string Nama { get; set; } = "";
}

public class Barang
{
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public string Kategori { get; set; } = "";
    public int HargaBeli { get; set; }
    public int HargaJual { get; set; }
    public int Stok { get; set; }
    public int StokMin { get; set; }

    public override string ToString() => $"{Kode} - {Nama} (stok: {Stok})";
}

public class Transaksi
{
    public string No { get; set; } = "";
    public string Tanggal { get; set; } = "";
    public string Kasir { get; set; } = "";
    public int Total { get; set; }
    public int Bayar { get; set; }
    public int Kembali { get; set; }
}

public class DetailTransaksi
{
    public string No { get; set; } = "";
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public int Qty { get; set; }
    public int Harga { get; set; }
    public int Subtotal { get; set; }
    public int Modal { get; set; }
}

public class RiwayatHarga
{
    public string No { get; set; } = "";
    public string Tanggal { get; set; } = "";
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public int BeliLama { get; set; }
    public int BeliBaru { get; set; }
    public int JualLama { get; set; }
    public int JualBaru { get; set; }
}

public class BarangMasuk
{
    public string No { get; set; } = "";
    public string Tanggal { get; set; } = "";
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public int Jumlah { get; set; }
    public string Supplier { get; set; } = "";
    public int HargaSupplier { get; set; }
    public string Petugas { get; set; } = "";
    public string Keterangan { get; set; } = "";
}

public class BarangKeluar
{
    public string No { get; set; } = "";
    public string Tanggal { get; set; } = "";
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public int Jumlah { get; set; }
    public string Jenis { get; set; } = "";
    public string Petugas { get; set; } = "";
    public string Keterangan { get; set; } = "";
}

public class ItemKeranjang
{
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public int Qty { get; set; }
    public int Harga { get; set; }
    public int Subtotal => Qty * Harga;
}
