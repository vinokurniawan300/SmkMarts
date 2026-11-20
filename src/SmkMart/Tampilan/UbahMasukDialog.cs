using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class UbahMasukDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly BarangMasuk _data;

    private readonly NumericUpDown _jumlah = new() { Minimum = 1, Maximum = 1_000_000_000, ThousandsSeparator = true };
    private readonly NumericUpDown _harga = Ui.Angka();
    private readonly DateTimePicker _tanggal = Ui.Tanggal();
    private readonly TextBox _supplier = new();
    private readonly TextBox _keterangan = new();

    public bool AdaPerubahan { get; private set; }

    public UbahMasukDialog(DataStore store, BarangMasuk data)
        : base("Ubah Barang Masuk", new Size(480, 440))
    {
        _store = store;
        _data = data;

        var barang = new Label { Text = $"{data.Kode} - {data.Nama}", AutoSize = true, UseMnemonic = false };
        var no = new Label { Text = data.No, AutoSize = true, UseMnemonic = false };

        _jumlah.Value = Math.Clamp(data.Jumlah, 1, 1_000_000_000);
        _harga.Value = Math.Clamp(data.HargaSupplier, 0, 1_000_000_000);
        if (DateTime.TryParse(data.Tanggal, out var tgl)) _tanggal.Value = tgl;
        _supplier.Text = data.Supplier;
        _keterangan.Text = data.Keterangan;

        Isi(Ui.Formulir(
            ("No", no),
            ("Barang", barang),
            ("Jumlah masuk", _jumlah),
            ("Harga dari supplier (Rp)", _harga),
            ("Tanggal masuk", _tanggal),
            ("Supplier", _supplier),
            ("Keterangan", _keterangan)));
    }

    protected override bool Simpan()
    {
        bool ok = Ui.Coba(() => _store.UbahMasuk(_data.No, (int)_jumlah.Value, Fmt.Tanggal(_tanggal.Value),
            _supplier.Text, _keterangan.Text, (int)_harga.Value));
        if (ok) AdaPerubahan = true;
        return ok;
    }
}
