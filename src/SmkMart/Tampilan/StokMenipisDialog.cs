using SmkMart.Models;

namespace SmkMart.Tampilan;

public class StokMenipisDialog : Form
{
    public StokMenipisDialog(List<Barang> daftar)
    {
        Text = "Peringatan Stok Barang Hampir Habis";
        BackColor = Tema.Latar;
        ClientSize = new Size(600, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var grid = Ui.BuatGrid();
        Ui.Kolom(grid, nameof(Barang.Kode), "Kode", bobot: 60);
        Ui.Kolom(grid, nameof(Barang.Nama), "Nama", bobot: 160);
        Ui.Kolom(grid, nameof(Barang.Stok), "Stok", JenisKolom.Angka, 50);
        Ui.Kolom(grid, nameof(Barang.StokMin), "Stok Min", JenisKolom.Angka, 60);
        grid.DataSource = daftar;

        var tutup = Ui.Tombol("Tutup", (s, e) => Close(), utama: true, lebar: 100);
        var bawah = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12, 10, 12, 0)
        };
        bawah.Controls.Add(tutup);

        Controls.Add(grid);
        Controls.Add(bawah);
        AcceptButton = tutup;
        CancelButton = tutup;
    }
}
