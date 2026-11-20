using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class LoginForm : DialogDasar
{
    private readonly DataStore _store;
    private readonly TextBox _username = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly Label _galat = new() { ForeColor = Tema.Bahaya, AutoSize = true, UseMnemonic = false };

    public Pengguna? PenggunaMasuk { get; private set; }

    public LoginForm(DataStore store) : base("Login - SMK MART", new Size(400, 340), "Masuk", "Keluar")
    {
        _store = store;
        StartPosition = FormStartPosition.CenterScreen;
        ShowInTaskbar = true;
        Shown += (s, e) =>
        {
            TopMost = true;
            TopMost = false;
            Activate();
            _username.Focus();
        };

        var judul = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Tema.UtamaGelap };
        judul.Controls.Add(new Label
        {
            Text = "Aplikasi Kasir dan Gudang",
            Dock = DockStyle.Fill,
            ForeColor = Tema.TeksPadaGelap,
            TextAlign = ContentAlignment.TopCenter,
            UseMnemonic = false
        });
        judul.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = Tema.Emas });
        judul.Controls.Add(new Label
        {
            Text = "SMK MART",
            Dock = DockStyle.Top,
            Height = 58,
            ForeColor = Tema.EmasMuda,
            TextAlign = ContentAlignment.BottomCenter,
            Font = new Font(Tema.HurufJudul, 22f, FontStyle.Bold),
            UseMnemonic = false
        });

        var form = Ui.Formulir(("Username", _username), ("Password", _password), ("", _galat));

        var isi = new Panel { Dock = DockStyle.Fill };
        isi.Controls.Add(form);
        isi.Controls.Add(judul);
        Isi(isi);
    }

    protected override bool Simpan()
    {
        var u = _store.Login(_username.Text, _password.Text);
        if (u == null)
        {
            _galat.Text = "Username atau password salah!";
            _password.Clear();
            _password.Focus();
            return false;
        }
        PenggunaMasuk = u;
        return true;
    }
}
