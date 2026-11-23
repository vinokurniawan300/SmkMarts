namespace SmkMart.Tampilan;

/// <summary>
/// Dasar semua jendela: ikut skala DPI layar (per-monitor) dan otomatis diperkecil
/// bila ukurannya melebihi area kerja layar (mis. laptop 1366x768 dengan skala 150%).
/// </summary>
public abstract class FormDasar : Form
{
    protected FormDasar()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        MuatkanKeLayar();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        MuatkanKeLayar();
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        MuatkanKeLayar();
    }

    private void MuatkanKeLayar()
    {
        var area = Screen.FromControl(this).WorkingArea;

        if (MinimumSize.Width > area.Width || MinimumSize.Height > area.Height)
            MinimumSize = new Size(Math.Min(MinimumSize.Width, area.Width), Math.Min(MinimumSize.Height, area.Height));

        if (Width > area.Width || Height > area.Height)
        {
            // Jendela tetap (dialog) yang lebih besar dari layar: perkecil dan sediakan scroll.
            if (FormBorderStyle == FormBorderStyle.FixedDialog) AutoScroll = true;
            Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
            Location = new Point(area.Left + (area.Width - Width) / 2, area.Top + (area.Height - Height) / 2);
        }
    }
}
