# SMK MART - Aplikasi Kasir & Gudang

Aplikasi desktop (C# / .NET 11 / Windows Forms) untuk kasir dan administrasi gudang toko.
Ini adalah versi C# dengan tampilan grafis dari program konsol Python `mbuh.py`.
Seluruh fitur dan aturan bisnis aslinya dipertahankan.

## Fitur

| Peran  | Yang bisa dilakukan |
|--------|---------------------|
| **Admin**  | Penjualan, kelola barang (tambah/ubah/hapus/urutkan/cari), administrasi gudang, laporan penjualan & stok, kelola pengguna |
| **Kasir**  | Transaksi penjualan (keranjang, kembalian otomatis, struk), lihat daftar barang & harga |
| **Gudang** | Catat barang masuk (termasuk mendaftarkan barang baru), catat barang keluar (rusak / retur / lainnya), riwayat, laporan stok |

Tambahan di versi GUI:

- Pencarian barang langsung saat mengetik, urutkan lewat pilihan di atas tabel
- Baris barang dengan stok di bawah/sama dengan stok minimum diberi warna merah
- Banner stok menipis di bagian bawah jendela (klik untuk melihat daftarnya)
- Pemilih tanggal (tidak perlu mengetik `YYYY-MM-DD`)
- Laporan penjualan dengan filter tanggal; klik dua kali sebuah transaksi untuk melihat struknya
- Struk bisa disalin ke clipboard
- Ukuran jendela & DPI fleksibel (per-monitor DPI, jendela otomatis menyesuaikan layar kecil)

## Riwayat versi

- **1.0.2** - Tampilan mengikuti DPI/skala layar (100%-300%, ganti monitor), jendela tidak melebihi area layar, menu login diperbesar.

## Akun bawaan (saat pertama kali dijalankan)

| Username | Password   | Peran  |
|----------|------------|--------|
| admin    | admin123   | Admin  |
| kasir1   | kasir123   | Kasir  |
| gudang1  | gudang123  | Gudang |

Segera ganti password ini lewat menu **Pengguna** (login sebagai Admin).

## Menjalankan

Syarat: Windows 10/11 dan [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0).

```bash
git clone <url-repo-anda>
cd SmkMarts
dotnet run --project src/SmkMart
```

Atau buka `SmkMart.sln` di Visual Studio 2022 lalu tekan F5.

Cara cepat di Windows: klik dua kali `BuatInstaller.bat` (otomatis publish + membuat Setup.exe di folder `installer-output`; Inno Setup dipasang lewat winget bila belum ada).

Di GitHub: push tag, mis. `git tag v1.0.2 && git push origin v1.0.2`. GitHub Actions membuat `SmkMart-Setup-1.0.2.exe` di halaman Releases.

Membuat versi rilis manual:

```bash
dotnet publish src/SmkMart/SmkMart.csproj -c Release -r win-x64 --self-contained false -o publish
```

## Lokasi data

Data disimpan sebagai file teks berpemisah `|` (format sama persis dengan versi Python) di:

```
%AppData%\SmkMart
```

Folder lain bisa dipilih dengan mengisi variabel lingkungan `SMKMART_DATA`.
Tombol **Folder Data** di jendela utama membuka folder tersebut.

**Migrasi dari versi Python:** salin `pengguna.txt`, `barang.txt`, `transaksi.txt`,
`detail_transaksi.txt`, `barang_masuk.txt`, dan `barang_keluar.txt` ke folder data di atas.

File data masuk `.gitignore` agar password dan data transaksi tidak ikut ter-commit.

## Struktur proyek

```
SmkMart.sln
src/SmkMart/
├── Program.cs              # titik masuk: login -> jendela utama -> logout
├── Models/                 # kelas data (Barang, Pengguna, Transaksi, ...)
├── Services/
│   ├── DataStore.cs        # baca/tulis file + seluruh aturan bisnis
│   ├── StrukBuilder.cs     # teks struk
│   └── Fmt.cs              # format Rupiah, tanggal, pembersih input
└── Tampilan/               # form, dialog, dan panel Windows Forms
```

Logika data (`Services`) dipisah dari tampilan (`Tampilan`) sehingga mudah diuji atau dipindah ke UI lain.

## Catatan keamanan

Seperti versi Python aslinya, password disimpan sebagai **teks biasa** di `pengguna.txt`.
Cukup untuk tugas/latihan, tetapi jangan dipakai untuk toko sungguhan tanpa menambahkan hashing password
(misalnya `PasswordHasher` / BCrypt).

## Lisensi

Belum ditentukan. Tambahkan berkas `LICENSE` sebelum repositori dibuka untuk umum.
