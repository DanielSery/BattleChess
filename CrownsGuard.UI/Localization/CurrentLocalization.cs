using System.Resources;
using System.Windows.Data;
using CrownsGuard.Core.Localization;
using CrownsGuard.UI.Resources;

namespace CrownsGuard.UI.Localization;

internal class CurrentLocalization : LocalizationSourceBase
{
    private CurrentLocalization()
    {
        Sources.Add(this);
    }

    public static LocalizationSourceBase Instance { get; } = new CurrentLocalization();

    protected override ResourceManager ResManager()
    {
        return Strings.ResourceManager;
    }
}

internal class LocExtension : Binding
{
    public LocExtension(string name) : base("[" + name + "]")
    {
        Mode = BindingMode.OneWay;
        Source = CurrentLocalization.Instance;
    }
}