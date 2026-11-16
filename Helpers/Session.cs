using System.Drawing;
using System.IO;

public static class Session
{
    public static int ProjectId;
    public static Project Project = new();

    public static Image? LogoImage() =>
        Project.Logo == null ? null : Image.FromStream(new MemoryStream(Project.Logo));
}