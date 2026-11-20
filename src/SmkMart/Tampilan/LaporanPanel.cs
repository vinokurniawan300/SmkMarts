using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public record BarisPenjualan(string No, string Tanggal, string Kasir, int JumlahItem, int Total);

public record BarisRiwayatHarga(string Tanggal, string Barang, string HargaBeli, string HargaJual, string Margin, string Keterangan, int Selisih);

public class LaporanPanel : PanelDasar
{
    private readonly DataStore _store;
    private readonly bool _penjualan;
    private readonly bool _stok;

    private readonly DateTimePicker _awal = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", ShowCheckBox = true, Checked = false, Width = 150 };
    private readonly DateTimePicker _akhir = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", ShowCheckBox = true, Checked = false, Width = 150 };
    private readonly ComboBox _urutJual = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130, Margin = new Padding(0, 5, 14, 0) };
    private readonly DataGridView _gridJual = Ui.BuatGrid();
    private readonly Label _ringkasJual = new() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Tema.UtamaGelap, Padding = new Padding(8), UseMnemonic = false };
    private readonly ListBox _terlaris = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };

    private readonly DateTimePicker _awalLaba = Pemilih();
    private readonly DateTimePicker _akhirLaba = Pemilih();
    private readonly DataGridView _gridLaba = Ui.BuatGrid();
    private readonly Label _ringkasLaba = new() { Dock = DockStyle.Bottom, Height = 130, Font = new Font("Segoe UI", 11f, FontStyle.Bold), Padding = new Padding(8), UseMnemonic = false };
    private readonly DataGridView _gridHarga = Ui.BuatGrid();

    private readonly Label _ringkasStok = new() { Dock = DockStyle.Top, Height = 90, Font = new Font("Consolas", 10f), Padding = new Padding(4, 8, 4, 0), UseMnemonic = false };
    private readonly DataGridView _gridStok = Ui.BuatGrid();

    private static DateTimePicker Pemilih() => new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "yyyy-MM-dd",
        ShowCheckBox = true,
        Checked = false,
        Width = 150,
        Margin = new Padding(0, 5, 14, 0)
    };

    public LaporanPanel(DataStore store, bool penjualan, bool stok)
    {
        _store = store;
        _penjualan = penjualan;
        _stok = stok;
        Padding = new Padding(12);

        var tab = new TabElegan { Dock = DockStyle.Fill };
        if (penjualan) tab.TabPages.Add(BuatHalamanPenjualan());
        if (penjualan) tab.TabPages.Add(BuatHalamanLaba());
        if (penjualan) tab.TabPages.Add(BuatHalamanRiwayatHarga());
        if (stok) tab.TabPages.Add(BuatHalamanStok());
        Controls.Add(tab);
    }

    private TabPage BuatHalamanPenjualan()
    {
        Ui.Kolom(_gridJual, nameof(BarisPenjualan.No), "No. Transaksi");
        Ui.Kolom(_gridJual, nameof(BarisPenjualan.Tanggal), "Tanggal");
        Ui.Kolom(_gridJual, nameof(BarisPenjualan.Kasir), "Kasir", bobot: 130);
        Ui.Kolom(_gridJual, nameof(BarisPenjualan.JumlahItem), "Jumlah Item", JenisKolom.Angka, 70);
        Ui.Kolom(_gridJual, nameof(BarisPenjualan.Total), "Total", JenisKolom.Rupiah, 90);
        _gridJual.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) LihatStruk(); };

        _awal.Margin = new Padding(0, 5, 14, 0);
        _akhir.Margin = new Padding(0, 5, 14, 0);

        var bilah = Ui.Bilah();
        bilah.Controls.Add(new Label { Text = "Dari:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        bilah.Controls.Add(_awal);
        bilah.Controls.Add(new Label { Text = "Sampai:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        bilah.Controls.Add(_akhir);
        bilah.Controls.Add(new Label { Text = "Urutan:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        _urutJual.Items.AddRange(new object[] { "Terbaru dulu", "Terlama dulu" });
        _urutJual.SelectedIndex = 0;
        _urutJual.SelectedIndexChanged += (s, e) => Tampilkan(false);
        bilah.Controls.Add(_urutJual);
        bilah.Controls.Add(Ui.Tombol("Tampilkan", (s, e) => Tampilkan(true), utama: true, lebar: 110));
        bilah.Controls.Add(new Label
        {
            Text = "Kosong = semua tanggal. Klik 2x baris = struk.",
            AutoSize = true,
            ForeColor = Tema.TeksRedup,
            Margin = new Padding(6, 9, 0, 0)
        });

        var grup = new GroupBox { Text = "Barang Terlaris", Dock = DockStyle.Fill, Padding = new Padding(8) };
        grup.Controls.Add(_terlaris);

        var ringkas = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 170, ColumnCount = 2, RowCount = 1 };
        ringkas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        ringkas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        ringkas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        ringkas.Controls.Add(_ringkasJual, 0, 0);
        ringkas.Controls.Add(grup, 1, 0);

        var hal = new TabPage("Laporan Penjualan") { Padding = new Padding(8) };
        hal.Controls.Add(_gridJual);
        hal.Controls.Add(ringkas);
        hal.Controls.Add(bilah);
        return hal;
    }

    private TabPage BuatHalamanLaba()
    {
        Ui.Kolom(_gridLaba, nameof(BarisLaba.No), "No. Transaksi", bobot: 80);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Tanggal), "Tanggal", bobot: 70);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Nama), "Barang", bobot: 140);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Jumlah), "Jumlah", JenisKolom.Angka, 50);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Pendapatan), "Pendapatan", JenisKolom.Rupiah, 85);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Modal), "Modal", JenisKolom.Rupiah, 85);
        Ui.Kolom(_gridLaba, nameof(BarisLaba.Laba), "Laba / Rugi", JenisKolom.Rupiah, 85);

        _gridLaba.DataBindingComplete += (s, e) =>
        {
            foreach (DataGridViewRow r in _gridLaba.Rows)
            {
                if (r.DataBoundItem is not BarisLaba b) continue;
                if (b.Laba < 0)
                {
                    r.DefaultCellStyle.BackColor = Tema.PeringatanLatar;
                    r.DefaultCellStyle.ForeColor = Tema.Bahaya;
                }
                else if (b.Laba > 0)
                {
                    r.Cells[nameof(BarisLaba.Laba)].Style.ForeColor = Tema.UtamaGelap;
                }
            }
        };

        var bilah = Ui.Bilah();
        bilah.Controls.Add(new Label { Text = "Dari:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        bilah.Controls.Add(_awalLaba);
        bilah.Controls.Add(new Label { Text = "Sampai:", AutoSize = true, Margin = new Padding(0, 9, 6, 0) });
        bilah.Controls.Add(_akhirLaba);
        bilah.Controls.Add(Ui.Tombol("Tampilkan", (s, e) => TampilkanLaba(true), utama: true, lebar: 110));
        bilah.Controls.Add(new Label
        {
            Text = "Laba / rugi bersih = pendapatan penjualan - pembelian dari supplier (barang masuk).",
            AutoSize = true,
            ForeColor = Tema.TeksRedup,
            Margin = new Padding(6, 9, 0, 0)
        });

        var hal = new TabPage("Laba / Rugi") { Padding = new Padding(8) };
        hal.Controls.Add(_gridLaba);
        hal.Controls.Add(_ringkasLaba);
        hal.Controls.Add(bilah);
        return hal;
    }

    private TabPage BuatHalamanRiwayatHarga()
    {
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.Tanggal), "Tanggal", bobot: 70);
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.Barang), "Barang", bobot: 130);
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.HargaBeli), "Harga Beli", bobot: 130);
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.HargaJual), "Harga Jual", bobot: 130);
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.Margin), "Untung per Unit", bobot: 130);
        Ui.Kolom(_gridHarga, nameof(BarisRiwayatHarga.Keterangan), "Keterangan", bobot: 220);

        _gridHarga.DataBindingComplete += (s, e) =>
        {
            foreach (DataGridViewRow r in _gridHarga.Rows)
            {
                if (r.DataBoundItem is not BarisRiwayatHarga b) continue;
                if (b.Selisih < 0)
                {
                    r.DefaultCellStyle.BackColor = Tema.PeringatanLatar;
                    r.DefaultCellStyle.ForeColor = Tema.Bahaya;
                }
                else if (b.Selisih > 0)
                {
                    r.DefaultCellStyle.ForeColor = Tema.UtamaGelap;
                }
            }
        };

        var hal = new TabPage("Riwayat Harga") { Padding = new Padding(8) };
        hal.Controls.Add(_gridHarga);
        return hal;
    }

    private TabPage BuatHalamanStok()
    {
        Ui.Kolom(_gridStok, nameof(Barang.Kode), "Kode", bobot: 55);
        Ui.Kolom(_gridStok, nameof(Barang.Nama), "Nama", bobot: 150);
        Ui.Kolom(_gridStok, nameof(Barang.Kategori), "Kategori", bobot: 90);
        Ui.Kolom(_gridStok, nameof(Barang.HargaBeli), "Harga Beli", JenisKolom.Rupiah, 80);
        Ui.Kolom(_gridStok, nameof(Barang.HargaJual), "Harga Jual", JenisKolom.Rupiah, 80);
        Ui.Kolom(_gridStok, nameof(Barang.Stok), "Stok", JenisKolom.Angka, 50);
        Ui.Kolom(_gridStok, nameof(Barang.StokMin), "Stok Min", JenisKolom.Angka, 60);
        Ui.TandaiStokMenipis(_gridStok);

        var hal = new TabPage("Laporan Stok & Gudang") { Padding = new Padding(8) };
        hal.Controls.Add(_gridStok);
        hal.Controls.Add(_ringkasStok);
        return hal;
    }

    public override void Segarkan()
    {
        if (_penjualan)
        {
            Tampilkan(false);
            TampilkanLaba(false);
            TampilkanRiwayatHarga();
        }
        if (_stok) TampilkanStok();
    }

    private void Tampilkan(bool daritombol)
    {
        string? awal = _awal.Checked ? Fmt.Tanggal(_awal.Value) : null;
        string? akhir = _akhir.Checked ? Fmt.Tanggal(_akhir.Value) : null;

        if (daritombol && awal != null && akhir != null && string.CompareOrdinal(awal, akhir) > 0)
        {
            Ui.Peringatan("Tanggal awal tidak boleh setelah tanggal akhir.");
            return;
        }

        var lap = _store.BuatLaporanPenjualan(awal, akhir);

        bool terbaru = _urutJual.SelectedIndex <= 0;
        var urut = terbaru
            ? lap.Daftar.OrderByDescending(t => t.Tanggal, StringComparer.Ordinal).ThenByDescending(t => t.No, StringComparer.Ordinal)
            : lap.Daftar.OrderBy(t => t.Tanggal, StringComparer.Ordinal).ThenBy(t => t.No, StringComparer.Ordinal);

        _gridJual.DataSource = urut
            .Select(t => new BarisPenjualan(t.No, t.Tanggal, t.Kasir, lap.ItemPerTransaksi.GetValueOrDefault(t.No), t.Total))
            .ToList();

        _ringkasJual.Text = lap.Daftar.Count == 0
            ? "Tidak ada transaksi pada rentang tanggal tersebut."
            : $"Total transaksi  : {lap.Daftar.Count}\nTotal pendapatan : {Fmt.Rupiah(lap.TotalPendapatan)}";

        _terlaris.Items.Clear();
        int no = 1;
        foreach (var p in lap.Terlaris)
            _terlaris.Items.Add($"{no++}. {p.Key}: {p.Value} unit terjual");
    }

    private void TampilkanLaba(bool daritombol)
    {
        string? awal = _awalLaba.Checked ? Fmt.Tanggal(_awalLaba.Value) : null;
        string? akhir = _akhirLaba.Checked ? Fmt.Tanggal(_akhirLaba.Value) : null;

        if (daritombol && awal != null && akhir != null && string.CompareOrdinal(awal, akhir) > 0)
        {
            Ui.Peringatan("Tanggal awal tidak boleh setelah tanggal akhir.");
            return;
        }

        var lap = _store.BuatLaporanLaba(awal, akhir);
        _gridLaba.DataSource = lap.Baris;

        if (lap.Baris.Count == 0 && lap.Pembelian == 0)
        {
            _ringkasLaba.ForeColor = Tema.Teks;
            _ringkasLaba.Text = "Tidak ada penjualan pada rentang tanggal tersebut.";
            return;
        }

        string status = lap.LabaBersih > 0 ? "UNTUNG" : lap.LabaBersih < 0 ? "RUGI" : "IMPAS";
        _ringkasLaba.ForeColor = lap.LabaBersih > 0 ? Tema.UtamaGelap : lap.LabaBersih < 0 ? Tema.Bahaya : Tema.Teks;
        _ringkasLaba.Text =
            $"Pendapatan penjualan : {Fmt.Rupiah(lap.Pendapatan)}\n" +
            $"Laba kotor (setelah modal barang terjual) : {Fmt.Rupiah(lap.Laba)}\n" +
            $"Pembelian dari supplier (barang masuk) : {Fmt.Rupiah(lap.Pembelian)}\n" +
            $"Laba / Rugi bersih : {Fmt.Rupiah(lap.LabaBersih)}  ({status})";
    }

    private void TampilkanRiwayatHarga()
    {
        _gridHarga.DataSource = _store.SemuaRiwayatHarga()
            .OrderByDescending(r => r.No)
            .Select(r =>
            {
                int marginLama = r.JualLama - r.BeliLama;
                int marginBaru = r.JualBaru - r.BeliBaru;
                int selisih = marginBaru - marginLama;

                var perubahan = new List<string>();
                if (r.BeliBaru > r.BeliLama) perubahan.Add("Harga beli naik");
                else if (r.BeliBaru < r.BeliLama) perubahan.Add("Harga beli turun");
                if (r.JualBaru > r.JualLama) perubahan.Add("Harga jual naik");
                else if (r.JualBaru < r.JualLama) perubahan.Add("Harga jual turun");

                string dampak =
                    marginBaru < 0 ? $"RUGI {Fmt.Rupiah(-marginBaru)} per unit"
                    : selisih > 0 ? $"untung naik {Fmt.Rupiah(selisih)} per unit"
                    : selisih < 0 ? $"untung turun {Fmt.Rupiah(-selisih)} per unit"
                    : "untung per unit tetap";

                return new BarisRiwayatHarga(
                    r.Tanggal,
                    $"{r.Kode} - {r.Nama}",
                    $"{Fmt.Rupiah(r.BeliLama)} → {Fmt.Rupiah(r.BeliBaru)}",
                    $"{Fmt.Rupiah(r.JualLama)} → {Fmt.Rupiah(r.JualBaru)}",
                    $"{Fmt.Rupiah(marginLama)} → {Fmt.Rupiah(marginBaru)}",
                    string.Join(", ", perubahan) + "; " + dampak,
                    selisih);
            })
            .ToList();
    }

    private void TampilkanStok()
    {
        var r = _store.Ringkasan();
        _ringkasStok.Text =
            $"Total barang masuk (gudang)       : {r.TotalMasuk} unit\n" +
            $"Total barang keluar non-penjualan : {r.TotalKeluar} unit\n" +
            $"Total barang terjual (penjualan)  : {r.TotalTerjual} unit\n" +
            (r.JumlahMenipis > 0
                ? $"Jenis barang di bawah stok minimum : {r.JumlahMenipis}"
                : "Semua stok barang dalam kondisi aman.");

        _gridStok.DataSource = _store.SemuaBarang().OrderBy(b => b.Nama, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private void LihatStruk()
    {
        var baris = Ui.Terpilih<BarisPenjualan>(_gridJual);
        if (baris == null) return;

        var t = _store.CariTransaksi(baris.No);
        if (t == null) return;

        var items = _store.DetailDari(t.No)
            .Select(d => new ItemKeranjang { Kode = d.Kode, Nama = d.Nama, Qty = d.Qty, Harga = d.Harga })
            .ToList();

        using var dlg = new StrukDialog(
            StrukBuilder.Buat(t.No, t.Tanggal, t.Kasir, items, t.Total, t.Bayar, t.Kembali), "Struk " + t.No);
        dlg.ShowDialog(FindForm());
    }
}
