namespace SmkMart.Tampilan;

public abstract class PanelDasar : UserControl
{
    public event Action? DataBerubah;

    protected PanelDasar()
    {
        Dock = DockStyle.Fill;
        BackColor = Tema.Latar;
    }

    protected void Beritahu() => DataBerubah?.Invoke();

    public abstract void Segarkan();
}
