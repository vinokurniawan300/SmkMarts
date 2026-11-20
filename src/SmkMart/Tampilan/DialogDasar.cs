namespace SmkMart.Tampilan;

public abstract class DialogDasar : Form
{
    protected readonly Button BtnOk;
    protected readonly Button BtnBatal;

    protected DialogDasar(string judul, Size ukuranKlien, string okTeks = "Simpan", string batalTeks = "Batal")
    {
        Text = judul;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        BackColor = Tema.Latar;
        ClientSize = ukuranKlien;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        BtnOk = Ui.Tombol(okTeks, (s, e) => Konfirmasi(), utama: true, lebar: 110);
        BtnBatal = Ui.Tombol(batalTeks, (s, e) => DialogResult = DialogResult.Cancel, lebar: 100);

        var bawah = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12, 10, 12, 0)
        };
        bawah.Controls.Add(BtnBatal);
        bawah.Controls.Add(BtnOk);
        Controls.Add(bawah);

        AcceptButton = BtnOk;
        CancelButton = BtnBatal;
    }

    protected void Isi(Control isi)
    {
        Controls.Add(isi);
        isi.BringToFront();
    }

    protected abstract bool Simpan();

    private void Konfirmasi()
    {
        if (Simpan()) DialogResult = DialogResult.OK;
    }
}
