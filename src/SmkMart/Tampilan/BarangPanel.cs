using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class BarangPanel : PanelDasar
{
    private readonly DataStore _store;
    private readonly bool _bisaUbah;
    private readonly DataGridView _grid = Ui.BuatGrid();
    private readonly TextBox _cari = new() { PlaceholderText = "Cari kode / nama / kategori...", Width = 280 };
    private readonly ComboBox _urut = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 210 };

    public BarangPanel(DataStore store, bool bisaUbah)
    {
        _store = store;
        _bisaUbah = bisaUbah;
        Padding = new Padding(12);

        Ui.Kolom(_grid, nameof(Barang.Kode), "Kode", bobot: 55);
        Ui.Kolom(_grid, nameof(Barang.Nama), "Nama", bobot: 150);
        Ui.Kolom(_grid, nameof(Barang.Kategori), "Kategori", bobot: 90);
        Ui.Kolom(_grid, nameof(Barang.HargaBeli), "Harga Beli", JenisKolom.Rupiah, 80);
        Ui.Kolom(_grid, nameof(Barang.HargaJual), "Harga Jual", JenisKolom.Rupiah, 80);
        Ui.Kolom(_grid, nameof(Barang.Stok), "Stok", JenisKolom.Angka, 50);
        Ui.Kolom(_grid, nameof(Barang.StokMin), "Stok Min", JenisKolom.Angka, 60);
        Ui.TandaiStokMenipis(_grid);

        _urut.Items.AddRange(new object[]
        {
            "Nama (A-Z)", "Harga jual (termurah)", "Harga jual (termahal)",
            "Stok (paling sedikit)", "Stok (paling banyak)"
        });
        _urut.SelectedIndex = 0;
        _cari.Margin = new Padding(0, 5, 8, 0);
        _urut.Margin = new Padding(0, 5, 12, 0);

        var bilah = Ui.Bilah();
        bilah.Controls.Add(_cari);
        bilah.Controls.Add(_urut);
        if (bisaUbah)
        {
            bilah.Controls.Add(Ui.Tombol("Tambah", (s, e) => Tambah(), utama: true, lebar: 100));
            bilah.Controls.Add(Ui.Tombol("Ubah", (s, e) => Ubah(), lebar: 100));
            bilah.Controls.Add(Ui.Tombol("Hapus", (s, e) => Hapus(), bahaya: true, lebar: 100));
            _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Ubah(); };
        }

        _cari.TextChanged += (s, e) => Segarkan();
        _urut.SelectedIndexChanged += (s, e) => Segarkan();

        Controls.Add(_grid);
        Controls.Add(bilah);
    }

    public override void Segarkan()
    {
        IEnumerable<Barang> q = _store.SemuaBarang();

        var kata = _cari.Text.Trim();
        if (kata != "")
        {
            q = q.Where(b => b.Kode.Contains(kata, StringComparison.OrdinalIgnoreCase)
                          || b.Nama.Contains(kata, StringComparison.OrdinalIgnoreCase)
                          || b.Kategori.Contains(kata, StringComparison.OrdinalIgnoreCase));
        }

        q = _urut.SelectedIndex switch
        {
            1 => q.OrderBy(b => b.HargaJual),
            2 => q.OrderByDescending(b => b.HargaJual),
            3 => q.OrderBy(b => b.Stok),
            4 => q.OrderByDescending(b => b.Stok),
            _ => q.OrderBy(b => b.Nama, StringComparer.OrdinalIgnoreCase)
        };

        _grid.DataSource = q.ToList();
    }

    private void Tambah()
    {
        using var d = new BarangDialog(_store);
        if (d.ShowDialog(FindForm()) == DialogResult.OK) Beritahu();
    }

    private void Ubah()
    {
        var b = Ui.Terpilih<Barang>(_grid);
        if (b == null) { Ui.Peringatan("Pilih barang yang ingin diubah."); return; }

        using var d = new BarangDialog(_store, b);
        if (d.ShowDialog(FindForm()) == DialogResult.OK) Beritahu();
    }

    private void Hapus()
    {
        var b = Ui.Terpilih<Barang>(_grid);
        if (b == null) { Ui.Peringatan("Pilih barang yang ingin dihapus."); return; }
        if (!Ui.Konfirmasi($"Yakin menghapus '{b.Nama}'?")) return;

        if (Ui.Coba(() => _store.HapusBarang(b.Kode))) Beritahu();
    }
}
