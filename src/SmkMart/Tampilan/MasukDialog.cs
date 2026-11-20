using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class MasukDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly string _petugas;

    private readonly ComboBox _barang = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _jumlah = new() { Minimum = 1, Maximum = 1_000_000_000, Value = 1, ThousandsSeparator = true };
    private readonly NumericUpDown _harga = Ui.Angka();
    private readonly DateTimePicker _tanggal = Ui.Tanggal();
    private readonly TextBox _supplier = new();
    private readonly TextBox _keterangan = new();

    private string? _kodeBaru;

    public bool AdaPerubahan { get; private set; }

    public MasukDialog(DataStore store, string petugas)
        : base("Catat Barang Masuk", new Size(480, 470))
    {
        _store = store;
        _petugas = petugas;

        var tombolBaru = Ui.Tombol("+ Daftarkan barang baru...", (s, e) => BarangBaru(), lebar: 200);

        Isi(Ui.Formulir(
            ("Barang", _barang),
            ("", tombolBaru),
            ("Jumlah masuk", _jumlah),
            ("Harga dari supplier (Rp)", _harga),
            ("Tanggal masuk", _tanggal),
            ("Supplier", _supplier),
            ("Keterangan", _keterangan)));

        _barang.SelectedIndexChanged += (s, e) => PilihanBerubah();
        Muat(null);
    }

    private void Muat(string? pilihKode)
    {
        _barang.Items.Clear();
        foreach (var b in _store.SemuaBarang().OrderBy(x => x.Kode, StringComparer.OrdinalIgnoreCase))
            _barang.Items.Add(b);

        int idx = 0;
        if (pilihKode != null)
        {
            for (int i = 0; i < _barang.Items.Count; i++)
            {
                if (_barang.Items[i] is Barang b && b.Kode == pilihKode) { idx = i; break; }
            }
        }
        if (_barang.Items.Count > 0) _barang.SelectedIndex = idx;
        PilihanBerubah();
    }

    private void PilihanBerubah()
    {
        if (_barang.SelectedItem is Barang pilih)
            _harga.Value = Math.Clamp(pilih.HargaBeli, 0, 1_000_000_000);

        if (_barang.SelectedItem is Barang b && _kodeBaru != null && b.Kode == _kodeBaru)
        {
            _jumlah.Value = Math.Clamp(b.Stok, 1, 1_000_000_000);
            _jumlah.Enabled = false;
        }
        else
        {
            _jumlah.Enabled = true;
        }
    }

    private void BarangBaru()
    {
        using var d = new BarangDialog(_store);
        if (d.ShowDialog(this) != DialogResult.OK || d.Hasil == null) return;

        AdaPerubahan = true;
        var b = d.Hasil;
        if (b.Stok <= 0)
        {
            _kodeBaru = null;
            Muat(b.Kode);
            Ui.Info($"Barang '{b.Nama}' tersimpan dengan stok awal 0, tidak ada barang masuk yang perlu dicatat.");
            return;
        }
        _kodeBaru = b.Kode;
        Muat(b.Kode);
    }

    protected override bool Simpan()
    {
        if (_barang.SelectedItem is not Barang b)
        {
            Ui.Peringatan("Pilih barang terlebih dahulu.");
            return false;
        }

        bool tambahStok = b.Kode != _kodeBaru;
        bool ok = Ui.Coba(() => _store.CatatMasuk(b.Kode, (int)_jumlah.Value, Fmt.Tanggal(_tanggal.Value),
            _supplier.Text, _petugas, _keterangan.Text, tambahStok, (int)_harga.Value));
        if (ok) AdaPerubahan = true;
        return ok;
    }
}
