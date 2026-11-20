using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public enum JenisKolom { Teks, Angka, Rupiah }

public static class Tema
{
    public static readonly Color Utama = Color.FromArgb(34, 84, 63);
    public static readonly Color UtamaHover = Color.FromArgb(46, 105, 79);
    public static readonly Color UtamaGelap = Color.FromArgb(23, 56, 43);
    public static readonly Color Emas = Color.FromArgb(190, 154, 70);
    public static readonly Color EmasGelap = Color.FromArgb(166, 132, 54);
    public static readonly Color EmasMuda = Color.FromArgb(228, 208, 150);
    public static readonly Color EmasPudar = Color.FromArgb(236, 227, 196);
    public static readonly Color Latar = Color.FromArgb(244, 246, 240);
    public static readonly Color Garis = Color.FromArgb(214, 218, 204);
    public static readonly Color Teks = Color.FromArgb(33, 42, 36);
    public static readonly Color TeksRedup = Color.FromArgb(104, 113, 101);
    public static readonly Color TeksPadaGelap = Color.FromArgb(206, 220, 208);
    public static readonly Color Bahaya = Color.FromArgb(139, 46, 46);
    public static readonly Color BahayaHover = Color.FromArgb(160, 58, 58);
    public static readonly Color PeringatanLatar = Color.FromArgb(247, 232, 226);
    public static readonly Color AmanLatar = Color.FromArgb(228, 236, 225);

    public const string HurufJudul = "Georgia";
}

public class TabElegan : TabControl
{
    public TabElegan()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SizeMode = TabSizeMode.Normal;
        ItemSize = new Size(0, 38);
        Padding = new Point(22, 6);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        if (e.Control is TabPage halaman)
        {
            halaman.UseVisualStyleBackColor = false;
            halaman.BackColor = Tema.Latar;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Tema.Latar);
        if (TabCount == 0) return;

        int dasar = GetTabRect(0).Bottom;
        using (var pena = new Pen(Tema.Garis))
            g.DrawLine(pena, 0, dasar, Width, dasar);

        for (int i = 0; i < TabCount; i++)
        {
            var r = GetTabRect(i);
            bool aktif = i == SelectedIndex;

            TextRenderer.DrawText(g, TabPages[i].Text, Font, r,
                aktif ? Tema.UtamaGelap : Tema.TeksRedup,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);

            if (aktif)
            {
                using var kuas = new SolidBrush(Tema.Emas);
                g.FillRectangle(kuas, r.Left + 10, dasar - 3, r.Width - 20, 3);
            }
        }
    }
}

/// <summary>
/// Bilah alat horizontal: semua isinya (kotak cari, label, pilihan, tombol) otomatis
/// dipusatkan secara vertikal dengan jarak seragam, dan tinggi bilah selalu sama.
/// </summary>
public sealed class BilahPanel : FlowLayoutPanel
{
    public BilahPanel()
    {
        Dock = DockStyle.Top;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        WrapContents = false;
        Padding = new Padding(0, 6, 0, 6);
        MinimumSize = new Size(0, 52);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        if (e.Control is not { } kontrol) return;
        kontrol.Anchor = AnchorStyles.Left;
        kontrol.Margin = new Padding(0, 0, 10, 0);
    }
}

public static class Ui
{
    public static Button Tombol(string teks, EventHandler? klik, bool utama = false, bool bahaya = false, int lebar = 130)
    {
        var b = new Button
        {
            Text = teks,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(lebar, 36),
            Padding = new Padding(10, 0, 10, 0),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0),
            UseVisualStyleBackColor = false
        };
        if (utama || bahaya)
        {
            b.BackColor = utama ? Tema.Utama : Tema.Bahaya;
            b.ForeColor = Color.White;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = utama ? Tema.UtamaHover : Tema.BahayaHover;
        }
        else
        {
            b.BackColor = Color.White;
            b.ForeColor = Tema.UtamaGelap;
            b.FlatAppearance.BorderColor = Tema.Emas;
            b.FlatAppearance.MouseOverBackColor = Tema.EmasPudar;
        }
        if (klik != null) b.Click += klik;
        return b;
    }

    public static Button TombolGelap(string teks, EventHandler? klik, int lebar = 120)
    {
        var b = Tombol(teks, klik, lebar: lebar);
        b.BackColor = Tema.UtamaGelap;
        b.ForeColor = Tema.EmasMuda;
        b.FlatAppearance.BorderColor = Tema.Emas;
        b.FlatAppearance.MouseOverBackColor = Tema.Utama;
        return b;
    }

    public static Button TombolEmas(string teks, EventHandler? klik, int lebar = 130)
    {
        var b = Tombol(teks, klik, lebar: lebar);
        b.BackColor = Tema.Emas;
        b.ForeColor = Tema.UtamaGelap;
        b.Font = new Font(b.Font, FontStyle.Bold);
        b.FlatAppearance.BorderSize = 0;
        b.FlatAppearance.MouseOverBackColor = Tema.EmasGelap;
        return b;
    }

    public static Label Lbl(string teks, bool tebal = false, float ukuran = 0)
    {
        var l = new Label { Text = teks, AutoSize = true, Anchor = AnchorStyles.Left, UseMnemonic = false };
        if (tebal || ukuran > 0)
            l.Font = new Font(l.Font.FontFamily, ukuran > 0 ? ukuran : l.Font.Size, tebal ? FontStyle.Bold : FontStyle.Regular);
        return l;
    }

    public static NumericUpDown Angka(int nilai = 0, int max = 1_000_000_000) => new()
    {
        Minimum = 0,
        Maximum = max,
        ThousandsSeparator = true,
        Value = nilai
    };

    public static DateTimePicker Tanggal() => new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "yyyy-MM-dd",
        Width = 160
    };

    public static BilahPanel Bilah() => new();

    public static TableLayoutPanel Formulir(params (string Label, Control Kontrol)[] baris)
    {
        var t = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            Padding = new Padding(16, 16, 16, 0)
        };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        foreach (var (label, kontrol) in baris)
        {
            var l = new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 14, 8),
                UseMnemonic = false
            };
            if (kontrol is Label)
            {
                // Label (mis. total & kembalian) dibiarkan seukuran teksnya dan dipusatkan vertikal,
                // bukan ditarik penuh, supaya sejajar dengan label kirinya.
                kontrol.Dock = DockStyle.None;
                kontrol.Anchor = AnchorStyles.Left;
            }
            else
            {
                kontrol.Dock = DockStyle.Fill;
            }
            kontrol.Margin = new Padding(0, 4, 0, 4);
            t.Controls.Add(l);
            t.Controls.Add(kontrol);
        }
        return t;
    }

    public static DataGridView BuatGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            GridColor = Tema.Garis,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 36
        };
        g.RowTemplate.Height = 30;
        g.ColumnHeadersDefaultCellStyle.BackColor = Tema.UtamaGelap;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Tema.EmasMuda;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tema.UtamaGelap;
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Tema.EmasMuda;
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        g.DefaultCellStyle.ForeColor = Tema.Teks;
        g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 246);
        g.DefaultCellStyle.SelectionBackColor = Tema.EmasPudar;
        g.DefaultCellStyle.SelectionForeColor = Tema.Teks;

        g.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex < 0 || s is not DataGridView dg) return;
            if (dg.Columns[e.ColumnIndex].Tag as string == "rp" && e.Value is int v)
            {
                e.Value = Fmt.Rupiah(v);
                e.FormattingApplied = true;
            }
        };
        return g;
    }

    public static void Kolom(DataGridView g, string properti, string judul, JenisKolom jenis = JenisKolom.Teks, float bobot = 100)
    {
        var c = new DataGridViewTextBoxColumn
        {
            DataPropertyName = properti,
            HeaderText = judul,
            Name = properti,
            FillWeight = bobot,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };
        // Judul kolom diratakan sama dengan isinya supaya selaras (angka & Rupiah rata kanan, teks rata kiri).
        var rataKanan = jenis != JenisKolom.Teks;
        var rata = rataKanan ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
        c.DefaultCellStyle.Alignment = rata;
        c.HeaderCell.Style.Alignment = rata;
        if (jenis == JenisKolom.Rupiah) c.Tag = "rp";
        g.Columns.Add(c);
    }

    public static T? Terpilih<T>(DataGridView g) where T : class => g.CurrentRow?.DataBoundItem as T;

    public static void TandaiStokMenipis(DataGridView g)
    {
        g.DataBindingComplete += (s, e) =>
        {
            foreach (DataGridViewRow r in g.Rows)
            {
                if (r.DataBoundItem is Barang b && b.Stok <= b.StokMin)
                {
                    r.DefaultCellStyle.BackColor = Tema.PeringatanLatar;
                    r.DefaultCellStyle.ForeColor = Tema.Bahaya;
                }
            }
        };
    }

    public static void Info(string pesan) =>
        MessageBox.Show(pesan, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void Peringatan(string pesan) =>
        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static bool Konfirmasi(string pesan) =>
        MessageBox.Show(pesan, "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    public static bool Coba(Action aksi)
    {
        try
        {
            aksi();
            return true;
        }
        catch (AturanException ex)
        {
            Peringatan(ex.Message);
            return false;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Gagal mengakses file data:\n" + ex.Message, "Kesalahan",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
