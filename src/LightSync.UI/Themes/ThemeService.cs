using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace LightSync.UI.Themes;

public enum Density { Normal, Compact }

public class ThemeService
{
    private const string Root = "avares://LightSync.UI/Themes/";
    private IResourceProvider? _density;

    public static void SetVariant(ThemeVariant variant) => Application.Current!.RequestedThemeVariant = variant;

    public void SetDensity(Density density)
    {
        var merged = Application.Current!.Resources.MergedDictionaries;
        if (_density is not null) merged.Remove(_density);
        _density = null;
        if (density == Density.Normal) return;

        _density = new ResourceInclude(new Uri(Root))
        {
            Source = new Uri($"{Root}Sizes/Spacing.{density}.axaml")
        };
        merged.Add(_density);
    }
}
