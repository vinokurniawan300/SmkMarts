namespace SmkMart.Tampilan;

public class StrukDialog : Form
{
    public StrukDialog(string teks, string judul = "Struk")
    {
        Text = judul;
        BackColor = Tema.Latar;
        ClientSize = new Size(440, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var kotak = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10f),
            Text = teks,
            ForeColor = Tema.Teks,
            BackColor = Color.White,
            BorderStyle = BorderStyle.None,
            ScrollBars = ScrollBars.Vertical
        };

        var tutup = Ui.Tombol("Tutup", (s, e) => Close(), utama: true, lebar: 100);
        var salin = Ui.Tombol("Salin", (s, e) => { Clipboard.SetText(teks); }, lebar: 100);

        var bawah = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12, 10, 12, 0)
        };
        bawah.Controls.Add(tutup);
        bawah.Controls.Add(salin);

        Controls.Add(kotak);
        Controls.Add(bawah);
        AcceptButton = tutup;
        CancelButton = tutup;

        Shown += (s, e) => { kotak.SelectionStart = 0; kotak.SelectionLength = 0; };
    }
}
