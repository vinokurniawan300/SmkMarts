using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class PenggunaPanel : PanelDasar
{
    private readonly DataStore _store;
    private readonly Pengguna _user;
    private readonly DataGridView _grid = Ui.BuatGrid();

    public PenggunaPanel(DataStore store, Pengguna user)
    {
        _store = store;
        _user = user;
        Padding = new Padding(12);

        Ui.Kolom(_grid, nameof(Pengguna.Username), "Username");
        Ui.Kolom(_grid, nameof(Pengguna.Nama), "Nama", bobot: 160);
        Ui.Kolom(_grid, nameof(Pengguna.Role), "Role", bobot: 70);
        _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Ubah(); };

        var bilah = Ui.Bilah();
        bilah.Controls.Add(Ui.Tombol("Tambah", (s, e) => Tambah(), utama: true, lebar: 100));
        bilah.Controls.Add(Ui.Tombol("Ubah", (s, e) => Ubah(), lebar: 100));
        bilah.Controls.Add(Ui.Tombol("Hapus", (s, e) => Hapus(), bahaya: true, lebar: 100));

        Controls.Add(_grid);
        Controls.Add(bilah);
    }

    public override void Segarkan() => _grid.DataSource = _store.SemuaPengguna();

    private void Tambah()
    {
        using var d = new UserDialog(_store);
        if (d.ShowDialog(FindForm()) == DialogResult.OK) Beritahu();
    }

    private void Ubah()
    {
        var u = Ui.Terpilih<Pengguna>(_grid);
        if (u == null) { Ui.Peringatan("Pilih pengguna yang ingin diubah."); return; }

        using var d = new UserDialog(_store, u);
        if (d.ShowDialog(FindForm()) == DialogResult.OK) Beritahu();
    }

    private void Hapus()
    {
        var u = Ui.Terpilih<Pengguna>(_grid);
        if (u == null) { Ui.Peringatan("Pilih pengguna yang ingin dihapus."); return; }
        if (!Ui.Konfirmasi($"Yakin menghapus pengguna '{u.Nama}'?")) return;

        if (Ui.Coba(() => _store.HapusPengguna(u.Username, _user.Username))) Beritahu();
    }
}
