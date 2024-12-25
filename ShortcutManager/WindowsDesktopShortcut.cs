namespace UpdateManager.DotNetUpgradeLibrary.ShortcutManager;
public static class WindowsDesktopShortcut
{
    public static void UpdateShortcutTarget(string name, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string shortcutPath = Helpers.GetShortcutPath(name);

        // Load the existing shortcut from the specified file
        using InternalWindowsShortcut shortcut = InternalWindowsShortcut.Load(shortcutPath);

        // Update the target path of the shortcut
        shortcut.Path = path;

        shortcut.Save(shortcutPath);

    }
}