using System.Diagnostics;
using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class MainForm : Form
{
    private readonly DataStore _store;
    private readonly Pengguna _user;
    private readonly TabElegan _tab = new() { Dock = DockStyle.Fill };
    private readonly List<PanelDasar> _panel = new();
    private readonly Label _banner = new()
    {
        Dock = DockStyle.Bottom,
        Height = 34,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(14, 0, 0, 0),
        Cursor = Cursors.Hand,
        UseMnemonic = false
    };

    public bool Logout { get; private set; }

    public MainForm(DataStore store, Pengguna user)
    {
        _store = store;
        _user = user;

        Text = "SMK MART - Aplikasi Kasir & Gudang";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        MinimumSize = new Size(1000, 640);
        Size = new Size(1180, 720);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Tema.Latar;
        ShowInTaskbar = true;
        MinimizeBox = true;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;

        _banner.Click += (s, e) => LihatStokMenipis();

        BuatTab();

        Controls.Add(_tab);
        Controls.Add(_banner);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 3, BackColor = Tema.Emas });
        Controls.Add(BuatHeader());

        SegarkanSemua();
    }

    private Panel BuatHeader()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Tema.UtamaGelap };

        var judul = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16, 4, 0, 12),
            BackColor = Color.Transparent
        };

        judul.Controls.Add(new Label
        {
            Text = "SMK MART",
            ForeColor = Tema.EmasMuda,
            Font = new Font(Tema.HurufJudul, 20f, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0),
            UseMnemonic = false
        });
        judul.Controls.Add(new Label
        {
            Text = "Aplikasi Kasir & Gudang",
            ForeColor = Tema.TeksPadaGelap,
            AutoSize = true,
            Margin = new Padding(2, 0, 0, 0),
            UseMnemonic = false
        });

        header.Controls.Add(judul);
        header.Height = Math.Max(84, judul.PreferredSize.Height + 8);

        var kanan = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Padding = new Padding(0, 20, 12, 0)
        };

        var info = Ui.Lbl($"{_user.Nama}  |  {_user.Role}");
        info.ForeColor = Tema.TeksPadaGelap;
        info.Margin = new Padding(0, 9, 18, 0);

        kanan.Controls.Add(info);
        kanan.Controls.Add(Ui.TombolGelap("Stok Menipis", (s, e) => LihatStokMenipis(), lebar: 120));
        kanan.Controls.Add(Ui.TombolGelap("Folder Data", (s, e) => BukaFolderData(), lebar: 110));
        kanan.Controls.Add(Ui.TombolGelap("Keluar Akun", (s, e) => { Logout = true; DialogResult = DialogResult.OK; }, lebar: 120));

        header.Controls.Add(kanan);
        return header;
    }

    private void BuatTab()
    {
        switch (_user.Role)
        {
            case Peran.Admin:
                TambahTab("Penjualan", new PenjualanPanel(_store, _user));
                TambahTab("Barang", new BarangPanel(_store, bisaUbah: true));
                TambahTab("Gudang", new GudangPanel(_store, _user));
                TambahTab("Laporan", new LaporanPanel(_store, penjualan: true, stok: true));
                TambahTab("Pengguna", new PenggunaPanel(_store, _user));
                break;

            case Peran.Kasir:
                TambahTab("Penjualan", new PenjualanPanel(_store, _user));
                TambahTab("Daftar Barang", new BarangPanel(_store, bisaUbah: false));
                break;

            case Peran.Gudang:
                TambahTab("Gudang", new GudangPanel(_store, _user));
                TambahTab("Daftar Barang", new BarangPanel(_store, bisaUbah: false));
                TambahTab("Laporan Stok", new LaporanPanel(_store, penjualan: false, stok: true));
                break;
        }
    }

    private void TambahTab(string judul, PanelDasar panel)
    {
        var halaman = new TabPage(judul);
        halaman.Controls.Add(panel);
        _tab.TabPages.Add(halaman);
        _panel.Add(panel);
        panel.DataBerubah += SegarkanSemua;
    }

    private void SegarkanSemua()
    {
        foreach (var p in _panel) p.Segarkan();
        PerbaruiBanner();
    }

    private void PerbaruiBanner()
    {
        int n = _store.StokMenipis().Count;
        if (n == 0)
        {
            _banner.BackColor = Tema.AmanLatar;
            _banner.ForeColor = Tema.UtamaGelap;
            _banner.Text = "Semua stok barang dalam kondisi aman.";
        }
        else
        {
            _banner.BackColor = Tema.PeringatanLatar;
            _banner.ForeColor = Tema.Bahaya;
            _banner.Text = $"Peringatan: {n} jenis barang stoknya hampir habis (klik untuk melihat).";
        }
    }

    private void LihatStokMenipis()
    {
        var daftar = _store.StokMenipis();
        if (daftar.Count == 0)
        {
            Ui.Info("Semua stok barang dalam kondisi aman.");
            return;
        }
        using var d = new StokMenipisDialog(daftar);
        d.ShowDialog(this);
    }

    private void BukaFolderData()
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = _store.Folder, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Ui.Peringatan("Tidak dapat membuka folder data:\n" + ex.Message);
        }
    }
}
