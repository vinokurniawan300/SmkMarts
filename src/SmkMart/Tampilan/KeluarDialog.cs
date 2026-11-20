using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class KeluarDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly string _petugas;

    private readonly ComboBox _barang = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _info = new() { AutoSize = true, UseMnemonic = false };
    private readonly NumericUpDown _jumlah = new() { Minimum = 1, Maximum = 1, Value = 1, ThousandsSeparator = true };
    private readonly ComboBox _jenis = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _tanggal = Ui.Tanggal();
    private readonly TextBox _keterangan = new();

    public bool AdaPerubahan { get; private set; }

    public KeluarDialog(DataStore store, string petugas)
        : base("Catat Barang Keluar", new Size(480, 420))
    {
        _store = store;
        _petugas = petugas;

        _jenis.Items.AddRange(new object[] { "Barang Rusak", "Retur ke Supplier", "Lainnya" });
        _jenis.SelectedIndex = 0;

        foreach (var b in store.SemuaBarang().OrderBy(x => x.Kode, StringComparer.OrdinalIgnoreCase))
            _barang.Items.Add(b);
        _barang.SelectedIndexChanged += (s, e) => PilihanBerubah();
        if (_barang.Items.Count > 0) _barang.SelectedIndex = 0;

        Isi(Ui.Formulir(
            ("Barang", _barang),
            ("", _info),
            ("Jumlah keluar", _jumlah),
            ("Jenis", _jenis),
            ("Tanggal keluar", _tanggal),
            ("Keterangan", _keterangan)));
    }

    private void PilihanBerubah()
    {
        if (_barang.SelectedItem is not Barang b) return;
        _jumlah.Maximum = Math.Clamp(b.Stok, 1, 1_000_000_000);
        _info.Text = "Stok saat ini: " + b.Stok;
    }

    protected override bool Simpan()
    {
        if (_barang.SelectedItem is not Barang b)
        {
            Ui.Peringatan("Pilih barang terlebih dahulu.");
            return false;
        }

        bool ok = Ui.Coba(() => _store.CatatKeluar(b.Kode, (int)_jumlah.Value, _jenis.SelectedItem as string ?? "Lainnya",
            Fmt.Tanggal(_tanggal.Value), _petugas, _keterangan.Text));
        if (ok) AdaPerubahan = true;
        return ok;
    }
}
