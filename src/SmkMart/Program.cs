using SmkMart.Models;
using SmkMart.Services;
using SmkMart.Tampilan;

namespace SmkMart;

internal static class Program
{
    private static string LokasiData()
    {
        var env = Environment.GetEnvironmentVariable("SMKMART_DATA");
        if (!string.IsNullOrWhiteSpace(env)) return env;
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmkMart");
    }

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        DataStore store;
        try
        {
            store = new DataStore(LokasiData());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Tidak dapat menyiapkan folder data:\n" + ex.Message, "SMK MART",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        while (true)
        {
            Pengguna? pengguna;
            using (var login = new LoginForm(store))
            {
                if (login.ShowDialog() != DialogResult.OK) break;
                pengguna = login.PenggunaMasuk;
            }
            if (pengguna == null) break;

            bool keluarAkun;
            using (var utama = new MainForm(store, pengguna))
            {
                utama.ShowDialog();
                keluarAkun = utama.Logout;
            }
            if (!keluarAkun) break;
        }
    }
}
