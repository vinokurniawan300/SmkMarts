using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class GudangPanel : PanelDasar
{
    private readonly DataStore _store;
    private readonly Pengguna _user;
    private readonly DataGridView _gridMasuk = Ui.BuatGrid();
    private readonly DataGridView _gridKeluar = Ui.BuatGrid();
    private readonly TabControl _tab;

    public GudangPanel(DataStore store, Pengguna user)
    {
        _store = store;
        _user = user;
        Padding = new Padding(12);

        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.No), "No", bobot: 60);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Tanggal), "Tanggal", bobot: 75);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Kode), "Kode", bobot: 50);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Nama), "Nama", bobot: 130);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Jumlah), "Jumlah", JenisKolom.Angka, 55);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Supplier), "Supplier", bobot: 100);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.HargaSupplier), "Harga Supplier", JenisKolom.Rupiah, 85);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Petugas), "Petugas", bobot: 90);
        Ui.Kolom(_gridMasuk, nameof(BarangMasuk.Keterangan), "Keterangan", bobot: 120);

        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.No), "No", bobot: 60);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Tanggal), "Tanggal", bobot: 75);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Kode), "Kode", bobot: 50);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Nama), "Nama", bobot: 130);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Jumlah), "Jumlah", JenisKolom.Angka, 55);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Jenis), "Jenis", bobot: 100);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Petugas), "Petugas", bobot: 90);
        Ui.Kolom(_gridKeluar, nameof(BarangKeluar.Keterangan), "Keterangan", bobot: 120);

        _tab = new TabElegan { Dock = DockStyle.Fill };
        var halMasuk = new TabPage("Riwayat Barang Masuk") { Padding = new Padding(8) };
        halMasuk.Controls.Add(_gridMasuk);
        var halKeluar = new TabPage("Riwayat Barang Keluar") { Padding = new Padding(8) };
        halKeluar.Controls.Add(_gridKeluar);
        _tab.TabPages.Add(halMasuk);
        _tab.TabPages.Add(halKeluar);

        var bilah = Ui.Bilah();
        bilah.Controls.Add(Ui.Tombol("Catat Barang Masuk", (s, e) => Masuk(), utama: true, lebar: 190));
        bilah.Controls.Add(Ui.Tombol("Catat Barang Keluar", (s, e) => Keluar(), lebar: 190));
        bilah.Controls.Add(Ui.Tombol("Ubah Barang Masuk / Keluar", (s, e) => Ubah(), lebar: 230));

        Controls.Add(_tab);
        Controls.Add(bilah);
    }

    public override void Segarkan()
    {
        _gridMasuk.DataSource = _store.SemuaMasuk().OrderByDescending(x => x.No).ToList();
        _gridKeluar.DataSource = _store.SemuaKeluar().OrderByDescending(x => x.No).ToList();
    }

    private void Masuk()
    {
        using var d = new MasukDialog(_store, _user.Nama);
        d.ShowDialog(FindForm());
        if (d.AdaPerubahan) Beritahu();
    }

    private void Keluar()
    {
        using var d = new KeluarDialog(_store, _user.Nama);
        d.ShowDialog(FindForm());
        if (d.AdaPerubahan) Beritahu();
    }

    private void Ubah()
    {
        if (_tab.SelectedIndex == 0)
        {
            var data = Ui.Terpilih<BarangMasuk>(_gridMasuk);
            if (data == null)
            {
                Ui.Peringatan("Pilih salah satu data pada Riwayat Barang Masuk terlebih dahulu.");
                return;
            }
            using var d = new UbahMasukDialog(_store, data);
            d.ShowDialog(FindForm());
            if (d.AdaPerubahan) Beritahu();
        }
        else
        {
            var data = Ui.Terpilih<BarangKeluar>(_gridKeluar);
            if (data == null)
            {
                Ui.Peringatan("Pilih salah satu data pada Riwayat Barang Keluar terlebih dahulu.");
                return;
            }
            using var d = new UbahKeluarDialog(_store, data);
            d.ShowDialog(FindForm());
            if (d.AdaPerubahan) Beritahu();
        }
    }
}
