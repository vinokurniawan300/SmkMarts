using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class PenjualanPanel : PanelDasar
{
    private readonly DataStore _store;
    private readonly Pengguna _user;
    private readonly List<ItemKeranjang> _keranjang = new();

    private readonly DataGridView _gridBarang = Ui.BuatGrid();
    private readonly DataGridView _gridKeranjang = Ui.BuatGrid();
    private readonly TextBox _cari = new() { PlaceholderText = "Cari barang...", Width = 230 };
    private readonly NumericUpDown _qty = new() { Minimum = 1, Maximum = 100000, Value = 1, Width = 80 };
    private readonly DateTimePicker _tanggal = Ui.Tanggal();
    private readonly Label _total = Ui.Lbl("Rp0", tebal: true, ukuran: 18);
    private readonly NumericUpDown _bayar = Ui.Angka(max: 2_000_000_000);
    private readonly Label _kembali = Ui.Lbl("Rp0", tebal: true);

    public PenjualanPanel(DataStore store, Pengguna user)
    {
        _store = store;
        _user = user;
        Padding = new Padding(12);

        Ui.Kolom(_gridBarang, nameof(Barang.Kode), "Kode", bobot: 55);
        Ui.Kolom(_gridBarang, nameof(Barang.Nama), "Nama", bobot: 150);
        Ui.Kolom(_gridBarang, nameof(Barang.Kategori), "Kategori", bobot: 85);
        Ui.Kolom(_gridBarang, nameof(Barang.HargaJual), "Harga", JenisKolom.Rupiah, 80);
        Ui.Kolom(_gridBarang, nameof(Barang.Stok), "Stok", JenisKolom.Angka, 45);
        Ui.TandaiStokMenipis(_gridBarang);
        _gridBarang.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) TambahKeKeranjang(); };

        Ui.Kolom(_gridKeranjang, nameof(ItemKeranjang.Kode), "Kode", bobot: 50);
        Ui.Kolom(_gridKeranjang, nameof(ItemKeranjang.Nama), "Nama", bobot: 130);
        Ui.Kolom(_gridKeranjang, nameof(ItemKeranjang.Qty), "Jumlah", JenisKolom.Angka, 55);
        Ui.Kolom(_gridKeranjang, nameof(ItemKeranjang.Harga), "Harga", JenisKolom.Rupiah, 70);
        Ui.Kolom(_gridKeranjang, nameof(ItemKeranjang.Subtotal), "Subtotal", JenisKolom.Rupiah, 80);

        _total.Font = new Font(Tema.HurufJudul, 18f, FontStyle.Bold);
        _total.ForeColor = Tema.UtamaGelap;

        _cari.Margin = new Padding(0, 5, 8, 0);
        _qty.Margin = new Padding(0, 5, 12, 0);

        var bilahKiri = Ui.Bilah();
        bilahKiri.Controls.Add(_cari);
        bilahKiri.Controls.Add(new Label { Text = "Jumlah:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        bilahKiri.Controls.Add(_qty);
        bilahKiri.Controls.Add(Ui.Tombol("Tambah ke Keranjang", (s, e) => TambahKeKeranjang(), utama: true, lebar: 180));

        var kiri = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };
        kiri.Controls.Add(_gridBarang);
        kiri.Controls.Add(bilahKiri);

        var judulKeranjang = Ui.Lbl("Keranjang Belanja", tebal: true);
        judulKeranjang.Margin = new Padding(0, 9, 16, 0);

        var bilahKanan = Ui.Bilah();
        bilahKanan.Controls.Add(judulKeranjang);
        bilahKanan.Controls.Add(Ui.Tombol("Hapus Item", (s, e) => HapusItem(), lebar: 110));
        bilahKanan.Controls.Add(Ui.Tombol("Kosongkan", (s, e) => Kosongkan(), lebar: 110));

        var pembayaran = Ui.Formulir(
            ("Tanggal", _tanggal),
            ("Total", _total),
            ("Uang dibayar", _bayar),
            ("Kembalian", _kembali));
        pembayaran.Dock = DockStyle.Bottom;
        pembayaran.Padding = new Padding(0, 8, 0, 8);

        var proses = Ui.TombolEmas("Proses Transaksi", (s, e) => Proses());
        proses.Dock = DockStyle.Bottom;
        proses.Height = 48;
        proses.Margin = Padding.Empty;

        var kanan = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 0, 0, 0) };
        kanan.Controls.Add(_gridKeranjang);
        kanan.Controls.Add(bilahKanan);
        kanan.Controls.Add(pembayaran);
        kanan.Controls.Add(proses);

        var tabel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        tabel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        tabel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        tabel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tabel.Controls.Add(kiri, 0, 0);
        tabel.Controls.Add(kanan, 1, 0);
        Controls.Add(tabel);

        _cari.TextChanged += (s, e) => MuatBarang();
        _bayar.ValueChanged += (s, e) => HitungKembalian();
        SegarkanKeranjang();
    }

    public override void Segarkan() => MuatBarang();

    private void MuatBarang()
    {
        IEnumerable<Barang> q = _store.SemuaBarang();
        var kata = _cari.Text.Trim();
        if (kata != "")
        {
            q = q.Where(b => b.Kode.Contains(kata, StringComparison.OrdinalIgnoreCase)
                          || b.Nama.Contains(kata, StringComparison.OrdinalIgnoreCase)
                          || b.Kategori.Contains(kata, StringComparison.OrdinalIgnoreCase));
        }
        _gridBarang.DataSource = q.OrderBy(b => b.Nama, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private void TambahKeKeranjang()
    {
        var pilih = Ui.Terpilih<Barang>(_gridBarang);
        if (pilih == null) { Ui.Peringatan("Pilih barang terlebih dahulu."); return; }

        var b = _store.CariBarang(pilih.Kode);
        if (b == null) { Ui.Peringatan("Barang tidak ditemukan."); MuatBarang(); return; }

        var ada = _keranjang.FirstOrDefault(i => i.Kode == b.Kode);
        int tersedia = b.Stok - (ada?.Qty ?? 0);
        if (tersedia <= 0) { Ui.Peringatan($"Stok '{b.Nama}' tidak tersedia!"); return; }

        int qty = (int)_qty.Value;
        if (qty > tersedia)
        {
            Ui.Peringatan($"Stok tidak mencukupi! Stok tersedia hanya {tersedia}.");
            return;
        }

        if (ada == null)
            _keranjang.Add(new ItemKeranjang { Kode = b.Kode, Nama = b.Nama, Qty = qty, Harga = b.HargaJual });
        else
            ada.Qty += qty;

        _qty.Value = 1;
        SegarkanKeranjang();
    }

    private void HapusItem()
    {
        var item = Ui.Terpilih<ItemKeranjang>(_gridKeranjang);
        if (item == null) { Ui.Peringatan("Pilih item di keranjang yang ingin dihapus."); return; }
        _keranjang.Remove(item);
        SegarkanKeranjang();
    }

    private void Kosongkan()
    {
        if (_keranjang.Count == 0) return;
        if (!Ui.Konfirmasi("Kosongkan seluruh keranjang?")) return;
        _keranjang.Clear();
        SegarkanKeranjang();
    }

    private void SegarkanKeranjang()
    {
        _gridKeranjang.DataSource = _keranjang.ToList();
        _total.Text = Fmt.Rupiah(_keranjang.Sum(i => i.Subtotal));
        HitungKembalian();
    }

    private void HitungKembalian()
    {
        int total = _keranjang.Sum(i => i.Subtotal);
        int bayar = (int)_bayar.Value;

        if (total == 0)
        {
            _kembali.Text = "Rp0";
            _kembali.ForeColor = Tema.Teks;
        }
        else if (bayar >= total)
        {
            _kembali.Text = Fmt.Rupiah(bayar - total);
            _kembali.ForeColor = Tema.Utama;
        }
        else
        {
            _kembali.Text = "Kurang " + Fmt.Rupiah(total - bayar);
            _kembali.ForeColor = Tema.Bahaya;
        }
    }

    private void Proses()
    {
        if (_keranjang.Count == 0) { Ui.Peringatan("Keranjang masih kosong."); return; }

        var items = _keranjang.ToList();
        Transaksi? trx = null;
        bool ok = Ui.Coba(() =>
            trx = _store.ProsesPenjualan(Fmt.Tanggal(_tanggal.Value), _user.Nama, items, (int)_bayar.Value));
        if (!ok || trx == null) return;

        using (var struk = new StrukDialog(
                   StrukBuilder.Buat(trx.No, trx.Tanggal, trx.Kasir, items, trx.Total, trx.Bayar, trx.Kembali),
                   "Struk " + trx.No))
        {
            struk.ShowDialog(FindForm());
        }

        _keranjang.Clear();
        _bayar.Value = 0;
        SegarkanKeranjang();
        Beritahu();
    }
}
