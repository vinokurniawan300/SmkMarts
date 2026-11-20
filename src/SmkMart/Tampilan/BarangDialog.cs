using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class BarangDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly Barang? _ubah;

    private readonly TextBox _kode = new() { ReadOnly = true };
    private readonly TextBox _nama = new();
    private readonly TextBox _kategori = new();
    private readonly NumericUpDown _beli = Ui.Angka();
    private readonly NumericUpDown _jual = Ui.Angka();
    private readonly NumericUpDown _stok = Ui.Angka();
    private readonly NumericUpDown _stokMin = Ui.Angka();

    public Barang? Hasil { get; private set; }

    public BarangDialog(DataStore store, Barang? ubah = null)
        : base(ubah == null ? "Tambah Barang" : "Ubah Barang", new Size(440, 400))
    {
        _store = store;
        _ubah = ubah;

        if (ubah == null)
        {
            _kode.Text = store.BuatKode();
        }
        else
        {
            _kode.Text = ubah.Kode;
            _nama.Text = ubah.Nama;
            _kategori.Text = ubah.Kategori;
            _beli.Value = Math.Clamp(ubah.HargaBeli, 0, 1_000_000_000);
            _jual.Value = Math.Clamp(ubah.HargaJual, 0, 1_000_000_000);
            _stok.Value = Math.Clamp(ubah.Stok, 0, 1_000_000_000);
            _stokMin.Value = Math.Clamp(ubah.StokMin, 0, 1_000_000_000);
            _stok.Enabled = false;
        }

        Isi(Ui.Formulir(
            ("Kode", _kode),
            ("Nama barang", _nama),
            ("Kategori", _kategori),
            ("Harga beli (Rp)", _beli),
            ("Harga jual (Rp)", _jual),
            (ubah == null ? "Stok awal" : "Stok saat ini", _stok),
            ("Stok minimum", _stokMin)));
    }

    protected override bool Simpan()
    {
        int beli = (int)_beli.Value, jual = (int)_jual.Value, stok = (int)_stok.Value, min = (int)_stokMin.Value;

        return Ui.Coba(() =>
        {
            if (_ubah == null)
                Hasil = _store.TambahBarang(_nama.Text, _kategori.Text, beli, jual, stok, min);
            else
                _store.UbahBarang(_ubah.Kode, _nama.Text, _kategori.Text, beli, jual, min);
        });
    }
}
