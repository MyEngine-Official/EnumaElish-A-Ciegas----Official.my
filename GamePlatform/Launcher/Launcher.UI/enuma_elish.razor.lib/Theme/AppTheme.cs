using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace enuma_elish.razor.lib.Theme
{
    public static class AppTheme
    {
        public static MudTheme Current { get; set; } = new MudTheme()
        {
          PaletteDark = new PaletteDark()
        };

        public static void SetLightTheme()
        {
            Current = new MudTheme() { PaletteLight = new PaletteLight() };
        }

        public static void SetDarkTheme()
        {
            Current = new MudTheme() { PaletteDark = new PaletteDark() };
        }

    }
}
