using System.Windows.Forms;

public static class Msg
{
    public static void Show(string text) =>
        MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
}