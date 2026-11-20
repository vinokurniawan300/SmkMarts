using SmkMart.Models;
using SmkMart.Services;

namespace SmkMart.Tampilan;

public class UserDialog : DialogDasar
{
    private readonly DataStore _store;
    private readonly Pengguna? _ubah;

    private readonly TextBox _username = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly CheckBox _tampil = new() { Text = "Tampilkan password", AutoSize = true };
    private readonly TextBox _nama = new();
    private readonly ComboBox _role = new() { DropDownStyle = ComboBoxStyle.DropDownList };

    public UserDialog(DataStore store, Pengguna? ubah = null)
        : base(ubah == null ? "Tambah Pengguna" : "Ubah Pengguna", new Size(440, 360))
    {
        _store = store;
        _ubah = ubah;

        _role.Items.AddRange(Peran.Semua);
        _role.SelectedItem = Peran.Kasir;
        _tampil.CheckedChanged += (s, e) => _password.UseSystemPasswordChar = !_tampil.Checked;

        if (ubah != null)
        {
            _username.Text = ubah.Username;
            _username.ReadOnly = true;
            _nama.Text = ubah.Nama;
            _role.SelectedItem = ubah.Role;
            _role.Enabled = false;
            _password.PlaceholderText = "(kosongkan jika tidak diubah)";
        }

        Isi(Ui.Formulir(
            ("Username", _username),
            ("Password", _password),
            ("", _tampil),
            ("Nama lengkap", _nama),
            ("Role", _role)));
    }

    protected override bool Simpan() => Ui.Coba(() =>
    {
        if (_ubah == null)
            _store.TambahPengguna(_username.Text, _password.Text, _nama.Text, _role.SelectedItem as string ?? "");
        else
            _store.UbahPengguna(_ubah.Username, _nama.Text, _password.Text);
    });
}
