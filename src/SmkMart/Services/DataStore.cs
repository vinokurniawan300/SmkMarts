using System.Globalization;
using System.Text;
using SmkMart.Models;

namespace SmkMart.Services;

public class AturanException : Exception
{
    public AturanException(string pesan) : base(pesan) { }
}

public record LaporanPenjualan(
    List<Transaksi> Daftar,
    Dictionary<string, int> ItemPerTransaksi,
    int TotalPendapatan,
    List<KeyValuePair<string, int>> Terlaris);

public record BarisLaba(string No, string Tanggal, string Nama, int Jumlah, int Pendapatan, int Modal, int Laba);

public record LaporanLaba(List<BarisLaba> Baris, int Pendapatan, int Modal, int Laba, int Pembelian, int LabaBersih);

public record RingkasanStok(int TotalMasuk, int TotalKeluar, int TotalTerjual, int JumlahMenipis);

public class DataStore
{
    private const string FileUser = "pengguna.txt";
    private const string FileBarang = "barang.txt";
    private const string FileTransaksi = "transaksi.txt";
    private const string FileDetail = "detail_transaksi.txt";
    private const string FileMasuk = "barang_masuk.txt";
    private const string FileKeluar = "barang_keluar.txt";
    private const string FileRiwayat = "riwayat_harga.txt";

    private static readonly Encoding Utf8 = new UTF8Encoding(false);

    public string Folder { get; }

    public DataStore(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(folder);

        foreach (var f in new[] { FileUser, FileBarang, FileTransaksi, FileDetail, FileMasuk, FileKeluar, FileRiwayat })
        {
            if (!File.Exists(Jalur(f))) File.WriteAllText(Jalur(f), "", Utf8);
        }

        if (Baca(FileUser, 4).Count == 0)
        {
            Tambah(FileUser, "admin", "admin123", "Admin", "Administrator Toko");
            Tambah(FileUser, "kasir1", "kasir123", "Kasir", "Siti Kasir");
            Tambah(FileUser, "gudang1", "gudang123", "Gudang", "Budi Gudang");

            if (Baca(FileBarang, 7).Count == 0)
            {
                Tambah(FileBarang, "B001", "Indomie Goreng", "Makanan", "2500", "3000", "100", "20");
                Tambah(FileBarang, "B002", "Aqua 600ml", "Minuman", "2000", "3000", "80", "15");
                Tambah(FileBarang, "B003", "Roti Tawar", "Makanan", "8000", "11000", "15", "10");
                Tambah(FileBarang, "B004", "Pulpen Standard", "ATK", "1500", "2500", "5", "10");
            }
        }
    }

    private string Jalur(string file) => Path.Combine(Folder, file);

    private static int Ke(string s) =>
        int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;

    private static string S(int n) => n.ToString(CultureInfo.InvariantCulture);

    private List<string[]> Baca(string file, int minKolom)
    {
        var path = Jalur(file);
        if (!File.Exists(path)) return new List<string[]>();
        return File.ReadAllLines(path, Utf8)
            .Where(l => l.Trim() != "")
            .Select(l => l.Split('|'))
            .Where(k => k.Length >= minKolom)
            .ToList();
    }

    private void Tulis(string file, IEnumerable<string[]> baris) =>
        File.WriteAllLines(Jalur(file), baris.Select(b => string.Join("|", b)), Utf8);

    private void Tambah(string file, params string[] kolom) =>
        File.AppendAllText(Jalur(file), string.Join("|", kolom) + "\n", Utf8);

    private static bool Sama(string a, string b) => a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);

    public List<Pengguna> SemuaPengguna() =>
        Baca(FileUser, 4)
            .Select(k => new Pengguna { Username = k[0], Password = k[1], Role = k[2], Nama = k[3] })
            .ToList();

    private void SimpanPengguna(IEnumerable<Pengguna> data) =>
        Tulis(FileUser, data.Select(p => new[] { p.Username, p.Password, p.Role, p.Nama }));

    public Pengguna? Login(string username, string password) =>
        SemuaPengguna().FirstOrDefault(u => u.Username == username.Trim() && u.Password == password.Trim());

    public void TambahPengguna(string username, string password, string nama, string role)
    {
        username = Fmt.Bersihkan(username);
        password = Fmt.Bersihkan(password);
        nama = Fmt.Bersihkan(nama);

        if (username == "") throw new AturanException("Username tidak boleh kosong!");
        if (password == "") throw new AturanException("Password tidak boleh kosong!");
        if (nama == "") throw new AturanException("Nama lengkap tidak boleh kosong!");
        if (!Peran.Semua.Contains(role)) throw new AturanException("Role tidak valid!");
        if (SemuaPengguna().Any(u => u.Username == username)) throw new AturanException("Username sudah digunakan!");

        Tambah(FileUser, username, password, role, nama);
    }

    public void UbahPengguna(string username, string namaBaru, string passwordBaru)
    {
        var data = SemuaPengguna();
        var u = data.FirstOrDefault(x => x.Username == username)
                ?? throw new AturanException("Username tidak ditemukan.");

        namaBaru = Fmt.Bersihkan(namaBaru);
        passwordBaru = Fmt.Bersihkan(passwordBaru);
        if (namaBaru != "") u.Nama = namaBaru;
        if (passwordBaru != "") u.Password = passwordBaru;
        SimpanPengguna(data);
    }

    public void HapusPengguna(string username, string usernameLogin)
    {
        var data = SemuaPengguna();
        var u = data.FirstOrDefault(x => x.Username == username)
                ?? throw new AturanException("Username tidak ditemukan.");

        if (u.Username == usernameLogin)
            throw new AturanException("Tidak bisa menghapus akun yang sedang login!");
        if (u.Role == Peran.Admin && data.Count(x => x.Role == Peran.Admin) <= 1)
            throw new AturanException("Tidak bisa menghapus admin terakhir!");

        data.Remove(u);
        SimpanPengguna(data);
    }

    public List<Barang> SemuaBarang() =>
        Baca(FileBarang, 7)
            .Select(k => new Barang
            {
                Kode = k[0], Nama = k[1], Kategori = k[2],
                HargaBeli = Ke(k[3]), HargaJual = Ke(k[4]), Stok = Ke(k[5]), StokMin = Ke(k[6])
            })
            .ToList();

    private void SimpanBarang(IEnumerable<Barang> data) =>
        Tulis(FileBarang, data.Select(b => new[]
        {
            b.Kode, b.Nama, b.Kategori, S(b.HargaBeli), S(b.HargaJual), S(b.Stok), S(b.StokMin)
        }));

    public Barang? CariBarang(string kode) => SemuaBarang().FirstOrDefault(b => Sama(b.Kode, kode));

    public string BuatKode()
    {
        int terbesar = 0;
        foreach (var b in SemuaBarang())
        {
            string angka = b.Kode.Length > 1 ? b.Kode[1..] : "";
            if (int.TryParse(angka, NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n > terbesar)
                terbesar = n;
        }
        return "B" + (terbesar + 1).ToString("D3", CultureInfo.InvariantCulture);
    }

    public Barang TambahBarang(string nama, string kategori, int hargaBeli, int hargaJual, int stokAwal, int stokMin)
    {
        nama = Fmt.Bersihkan(nama);
        kategori = Fmt.Bersihkan(kategori);
        if (nama == "") throw new AturanException("Nama barang tidak boleh kosong!");
        if (kategori == "") throw new AturanException("Kategori tidak boleh kosong!");

        var b = new Barang
        {
            Kode = BuatKode(), Nama = nama, Kategori = kategori,
            HargaBeli = hargaBeli, HargaJual = hargaJual, Stok = stokAwal, StokMin = stokMin
        };
        Tambah(FileBarang, b.Kode, b.Nama, b.Kategori, S(b.HargaBeli), S(b.HargaJual), S(b.Stok), S(b.StokMin));
        return b;
    }

    public void UbahBarang(string kode, string nama, string kategori, int hargaBeli, int hargaJual, int stokMin)
    {
        nama = Fmt.Bersihkan(nama);
        kategori = Fmt.Bersihkan(kategori);
        if (nama == "") throw new AturanException("Nama barang tidak boleh kosong!");
        if (kategori == "") throw new AturanException("Kategori tidak boleh kosong!");

        var data = SemuaBarang();
        var b = data.FirstOrDefault(x => Sama(x.Kode, kode))
                ?? throw new AturanException("Kode barang tidak ditemukan.");

        int beliLama = b.HargaBeli, jualLama = b.HargaJual;
        b.Nama = nama;
        b.Kategori = kategori;
        b.HargaBeli = hargaBeli;
        b.HargaJual = hargaJual;
        b.StokMin = stokMin;
        SimpanBarang(data);

        if (beliLama != hargaBeli || jualLama != hargaJual)
        {
            string no = "HRG" + (Baca(FileRiwayat, 8).Count + 1).ToString("D4", CultureInfo.InvariantCulture);
            Tambah(FileRiwayat, no, Fmt.Tanggal(DateTime.Now), b.Kode, b.Nama,
                   S(beliLama), S(hargaBeli), S(jualLama), S(hargaJual));
        }
    }

    public List<RiwayatHarga> SemuaRiwayatHarga() =>
        Baca(FileRiwayat, 8)
            .Select(k => new RiwayatHarga
            {
                No = k[0], Tanggal = k[1], Kode = k[2], Nama = k[3],
                BeliLama = Ke(k[4]), BeliBaru = Ke(k[5]), JualLama = Ke(k[6]), JualBaru = Ke(k[7])
            })
            .ToList();

    public void HapusBarang(string kode)
    {
        var data = SemuaBarang();
        var b = data.FirstOrDefault(x => Sama(x.Kode, kode))
                ?? throw new AturanException("Kode barang tidak ditemukan.");
        data.Remove(b);
        SimpanBarang(data);
    }

    public List<Barang> StokMenipis() =>
        SemuaBarang().Where(b => b.Stok <= b.StokMin).OrderBy(b => b.Stok).ToList();

    public List<Transaksi> SemuaTransaksi() =>
        Baca(FileTransaksi, 6)
            .Select(k => new Transaksi
            {
                No = k[0], Tanggal = k[1], Kasir = k[2], Total = Ke(k[3]), Bayar = Ke(k[4]), Kembali = Ke(k[5])
            })
            .ToList();

    public List<DetailTransaksi> SemuaDetail()
    {
        var modalSaatIni = new Dictionary<string, int>();
        foreach (var b in SemuaBarang())
            modalSaatIni.TryAdd(b.Kode.Trim().ToUpperInvariant(), b.HargaBeli);

        return Baca(FileDetail, 6)
            .Select(k => new DetailTransaksi
            {
                No = k[0], Kode = k[1], Nama = k[2], Qty = Ke(k[3]), Harga = Ke(k[4]), Subtotal = Ke(k[5]),
                Modal = k.Length > 6 ? Ke(k[6]) : modalSaatIni.GetValueOrDefault(k[1].Trim().ToUpperInvariant())
            })
            .ToList();
    }

    public Transaksi? CariTransaksi(string no) => SemuaTransaksi().FirstOrDefault(t => t.No == no);

    public List<DetailTransaksi> DetailDari(string no) => SemuaDetail().Where(d => d.No == no).ToList();

    public Transaksi ProsesPenjualan(string tanggal, string kasir, IReadOnlyList<ItemKeranjang> keranjang, int bayar)
    {
        if (keranjang.Count == 0) throw new AturanException("Keranjang masih kosong.");

        var barang = SemuaBarang();
        foreach (var item in keranjang)
        {
            var b = barang.FirstOrDefault(x => Sama(x.Kode, item.Kode))
                    ?? throw new AturanException($"Barang '{item.Nama}' tidak ditemukan.");
            if (item.Qty <= 0) throw new AturanException("Jumlah beli minimal 1.");
            if (item.Qty > b.Stok)
                throw new AturanException($"Stok '{b.Nama}' tidak mencukupi! Stok tersedia hanya {b.Stok}.");
        }

        int total = keranjang.Sum(i => i.Subtotal);
        if (bayar < total) throw new AturanException("Uang yang dibayarkan kurang!");
        int kembali = bayar - total;

        var modal = new Dictionary<string, int>();
        foreach (var item in keranjang)
            modal.TryAdd(item.Kode, barang.First(x => Sama(x.Kode, item.Kode)).HargaBeli);

        foreach (var item in keranjang)
            barang.First(x => Sama(x.Kode, item.Kode)).Stok -= item.Qty;
        SimpanBarang(barang);

        string no = "TRX" + (Baca(FileTransaksi, 6).Count + 1).ToString("D4", CultureInfo.InvariantCulture);
        Tambah(FileTransaksi, no, tanggal, kasir, S(total), S(bayar), S(kembali));
        foreach (var item in keranjang)
            Tambah(FileDetail, no, item.Kode, item.Nama, S(item.Qty), S(item.Harga), S(item.Subtotal), S(modal[item.Kode]));

        return new Transaksi { No = no, Tanggal = tanggal, Kasir = kasir, Total = total, Bayar = bayar, Kembali = kembali };
    }

    public List<BarangMasuk> SemuaMasuk() =>
        Baca(FileMasuk, 8)
            .Select(k => new BarangMasuk
            {
                No = k[0], Tanggal = k[1], Kode = k[2], Nama = k[3], Jumlah = Ke(k[4]),
                Supplier = k[5], Petugas = k[6], Keterangan = k[7],
                HargaSupplier = k.Length > 8 ? Ke(k[8]) : 0
            })
            .ToList();

    public List<BarangKeluar> SemuaKeluar() =>
        Baca(FileKeluar, 8)
            .Select(k => new BarangKeluar
            {
                No = k[0], Tanggal = k[1], Kode = k[2], Nama = k[3], Jumlah = Ke(k[4]),
                Jenis = k[5], Petugas = k[6], Keterangan = k[7]
            })
            .ToList();

    public void CatatMasuk(string kode, int jumlah, string tanggal, string supplier, string petugas,
                           string keterangan, bool tambahStok, int hargaSupplier)
    {
        supplier = Fmt.Bersihkan(supplier);
        keterangan = Fmt.Bersihkan(keterangan);
        if (jumlah < 1) throw new AturanException("Jumlah barang masuk minimal 1.");
        if (supplier == "") throw new AturanException("Nama supplier tidak boleh kosong!");
        if (hargaSupplier < 1) throw new AturanException("Harga dari supplier harus diisi!");

        var data = SemuaBarang();
        var b = data.FirstOrDefault(x => Sama(x.Kode, kode))
                ?? throw new AturanException("Kode barang tidak ditemukan!");

        if (tambahStok)
        {
            b.Stok += jumlah;
            SimpanBarang(data);
        }

        string no = "MSK" + (Baca(FileMasuk, 8).Count + 1).ToString("D4", CultureInfo.InvariantCulture);
        Tambah(FileMasuk, no, tanggal, b.Kode, b.Nama, S(jumlah), supplier, petugas, keterangan, S(hargaSupplier));
    }

    public void CatatKeluar(string kode, int jumlah, string jenis, string tanggal, string petugas, string keterangan)
    {
        keterangan = Fmt.Bersihkan(keterangan);
        if (jumlah < 1) throw new AturanException("Jumlah barang keluar minimal 1.");

        var data = SemuaBarang();
        var b = data.FirstOrDefault(x => Sama(x.Kode, kode))
                ?? throw new AturanException("Kode barang tidak ditemukan!");
        if (jumlah > b.Stok) throw new AturanException("Jumlah melebihi stok yang tersedia!");

        b.Stok -= jumlah;
        SimpanBarang(data);

        string no = "KLR" + (Baca(FileKeluar, 8).Count + 1).ToString("D4", CultureInfo.InvariantCulture);
        Tambah(FileKeluar, no, tanggal, b.Kode, b.Nama, S(jumlah), jenis, petugas, keterangan);
    }

    public void UbahMasuk(string no, int jumlah, string tanggal, string supplier, string keterangan, int hargaSupplier)
    {
        supplier = Fmt.Bersihkan(supplier);
        keterangan = Fmt.Bersihkan(keterangan);
        if (jumlah < 1) throw new AturanException("Jumlah barang masuk minimal 1.");
        if (supplier == "") throw new AturanException("Nama supplier tidak boleh kosong!");
        if (hargaSupplier < 1) throw new AturanException("Harga dari supplier harus diisi!");

        var baris = Baca(FileMasuk, 8);
        var k = baris.FirstOrDefault(x => x[0] == no)
                ?? throw new AturanException("Data barang masuk tidak ditemukan.");

        int selisih = jumlah - Ke(k[4]);
        if (selisih != 0)
        {
            var data = SemuaBarang();
            var b = data.FirstOrDefault(x => Sama(x.Kode, k[2]))
                    ?? throw new AturanException("Barang sudah dihapus, data barang masuk ini tidak bisa diubah.");
            if (b.Stok + selisih < 0)
                throw new AturanException($"Jumlah tidak bisa dikurangi sebanyak itu, stok saat ini hanya {b.Stok}.");
            b.Stok += selisih;
            SimpanBarang(data);
        }

        var baru = new string[9];
        for (int i = 0; i < 8; i++) baru[i] = k[i];
        baru[1] = tanggal;
        baru[4] = S(jumlah);
        baru[5] = supplier;
        baru[7] = keterangan;
        baru[8] = S(hargaSupplier);

        Tulis(FileMasuk, baris.Select(x => x[0] == no ? baru : x));
    }

    public void UbahKeluar(string no, int jumlah, string jenis, string tanggal, string keterangan)
    {
        keterangan = Fmt.Bersihkan(keterangan);
        if (jumlah < 1) throw new AturanException("Jumlah barang keluar minimal 1.");

        var baris = Baca(FileKeluar, 8);
        var k = baris.FirstOrDefault(x => x[0] == no)
                ?? throw new AturanException("Data barang keluar tidak ditemukan.");

        int selisih = jumlah - Ke(k[4]);
        if (selisih != 0)
        {
            var data = SemuaBarang();
            var b = data.FirstOrDefault(x => Sama(x.Kode, k[2]))
                    ?? throw new AturanException("Barang sudah dihapus, data barang keluar ini tidak bisa diubah.");
            if (selisih > b.Stok)
                throw new AturanException($"Jumlah melebihi stok yang tersedia! Maksimal {Ke(k[4]) + b.Stok}.");
            b.Stok -= selisih;
            SimpanBarang(data);
        }

        var baru = (string[])k.Clone();
        baru[1] = tanggal;
        baru[4] = S(jumlah);
        baru[5] = jenis;
        baru[7] = keterangan;

        Tulis(FileKeluar, baris.Select(x => x[0] == no ? baru : x));
    }

    public LaporanPenjualan BuatLaporanPenjualan(string? awal, string? akhir)
    {
        var hasil = SemuaTransaksi()
            .Where(t => string.IsNullOrEmpty(awal) || string.CompareOrdinal(t.Tanggal, awal) >= 0)
            .Where(t => string.IsNullOrEmpty(akhir) || string.CompareOrdinal(t.Tanggal, akhir) <= 0)
            .ToList();

        var nomor = hasil.Select(t => t.No).ToHashSet();
        var detail = SemuaDetail().Where(d => nomor.Contains(d.No)).ToList();

        var perTrx = detail.GroupBy(d => d.No).ToDictionary(g => g.Key, g => g.Sum(d => d.Qty));
        var terlaris = detail
            .GroupBy(d => d.Nama)
            .Select(g => new KeyValuePair<string, int>(g.Key, g.Sum(d => d.Qty)))
            .OrderByDescending(p => p.Value)
            .Take(5)
            .ToList();

        return new LaporanPenjualan(hasil, perTrx, hasil.Sum(t => t.Total), terlaris);
    }

    public LaporanLaba BuatLaporanLaba(string? awal, string? akhir)
    {
        var tanggal = new Dictionary<string, string>();
        foreach (var t in SemuaTransaksi())
        {
            if (!string.IsNullOrEmpty(awal) && string.CompareOrdinal(t.Tanggal, awal) < 0) continue;
            if (!string.IsNullOrEmpty(akhir) && string.CompareOrdinal(t.Tanggal, akhir) > 0) continue;
            tanggal.TryAdd(t.No, t.Tanggal);
        }

        var baris = SemuaDetail()
            .Where(d => tanggal.ContainsKey(d.No))
            .Select(d =>
            {
                int modal = d.Modal * d.Qty;
                return new BarisLaba(d.No, tanggal[d.No], d.Nama, d.Qty, d.Subtotal, modal, d.Subtotal - modal);
            })
            .ToList();

        long pembelian = SemuaMasuk()
            .Where(m => string.IsNullOrEmpty(awal) || string.CompareOrdinal(m.Tanggal, awal) >= 0)
            .Where(m => string.IsNullOrEmpty(akhir) || string.CompareOrdinal(m.Tanggal, akhir) <= 0)
            .Sum(m => (long)m.Jumlah * m.HargaSupplier);
        int totalBeli = (int)Math.Min(pembelian, int.MaxValue);

        int pendapatan = baris.Sum(b => b.Pendapatan);
        return new LaporanLaba(baris, pendapatan, baris.Sum(b => b.Modal), baris.Sum(b => b.Laba),
                               totalBeli, pendapatan - totalBeli);
    }

    public RingkasanStok Ringkasan() =>
        new(SemuaMasuk().Sum(m => m.Jumlah),
            SemuaKeluar().Sum(k => k.Jumlah),
            SemuaDetail().Sum(d => d.Qty),
            StokMenipis().Count);
}
