using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class UbahKeluarDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly BarangKeluar _data;

    private readonly NumericUpDown _jumlah = new() { Minimum = 1, Maximum = 1, Value = 1, ThousandsSeparator = true };
    private readonly ComboBox _jenis = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _tanggal = Ui.Tanggal();
    private readonly TextBox _keterangan = new();

    public bool AdaPerubahan { get; private set; }

    public UbahKeluarDialog(DataStore store, BarangKeluar data)
        : base("Ubah Barang Keluar", new Size(480, 400))
    {
        _store = store;
        _data = data;

        var barang = new Label { Text = $"{data.Kode} - {data.Nama}", AutoSize = true, UseMnemonic = false };
        var no = new Label { Text = data.No, AutoSize = true, UseMnemonic = false };
        var info = new Label { AutoSize = true, UseMnemonic = false };

        _jenis.Items.AddRange(new object[] { "Barang Rusak", "Retur ke Supplier", "Lainnya" });
        int idx = _jenis.Items.IndexOf(data.Jenis);
        _jenis.SelectedIndex = idx >= 0 ? idx : _jenis.Items.Count - 1;

        int stok = store.CariBarang(data.Kode)?.Stok ?? 0;
        int maks = Math.Clamp(data.Jumlah + stok, 1, 1_000_000_000);
        _jumlah.Maximum = maks;
        _jumlah.Value = Math.Clamp(data.Jumlah, 1, maks);
        info.Text = "Stok saat ini: " + stok;

        if (DateTime.TryParse(data.Tanggal, out var tgl)) _tanggal.Value = tgl;
        _keterangan.Text = data.Keterangan;

        Isi(Ui.Formulir(
            ("No", no),
            ("Barang", barang),
            ("", info),
            ("Jumlah keluar", _jumlah),
            ("Jenis", _jenis),
            ("Tanggal keluar", _tanggal),
            ("Keterangan", _keterangan)));
    }

    protected override bool Simpan()
    {
        bool ok = Ui.Coba(() => _store.UbahKeluar(_data.No, (int)_jumlah.Value,
            _jenis.SelectedItem as string ?? "Lainnya", Fmt.Tanggal(_tanggal.Value), _keterangan.Text));
        if (ok) AdaPerubahan = true;
        return ok;
    }
}
