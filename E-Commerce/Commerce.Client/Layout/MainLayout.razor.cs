using MudBlazor;

namespace Commerce.Client.Layout;

public sealed partial class MainLayout
{
    private readonly bool isDarkMode = true;

    private readonly MudTheme currentTheme = new()
    {
        Palette = new PaletteLight(),
        PaletteDark = new PaletteDark
        {
            Black = "#ffffff",
            White = "#ffffff",
            Primary = "#7E57C2",
            PrimaryContrastText = "#ffffff",
            Secondary = "#453d59",
            SecondaryContrastText = "#ffffff",
            Tertiary = "#9E9E9E",
            TertiaryContrastText = "#FFFFFF",
            Info = "#1E1E2D",
            InfoContrastText = "#ffffff",
            Success = "#0BBA83",
            SuccessContrastText = "#ffffff",
            Warning = "#1E1E2D",
            WarningContrastText = "#ffffff",
            Error = "#F64E62",
            ErrorContrastText = "#ffffff",
            Dark = "#27272F",
            DarkContrastText = "#ffffff",
            TextPrimary = "#ffffff",
            TextSecondary = "#878787",
            TextDisabled = "#4B4B57",
            ActionDefault = "#ADADB1",
            ActionDisabled = "#4B4B57",
            Background = "#121212",
            BackgroundGrey = "#1E1E2D",
            Surface = "#242424",
        },
        Typography = new()
        {
            Default = new()
            {
                FontFamily = new[] { "Poppins" }
            }
        }
    };
}