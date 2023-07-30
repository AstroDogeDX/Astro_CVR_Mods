using System.Reflection;
using Astro.ScrollZoom;
using Astro.ScrollZoom.Properties;
using MelonLoader;


[assembly: AssemblyVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyFileVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyInformationalVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyTitle(nameof(Astro.ScrollZoom))]
[assembly: AssemblyCompany(AssemblyInfoParams.Author)]
[assembly: AssemblyProduct(nameof(Astro.ScrollZoom))]

[assembly: MelonInfo(
    typeof(ScrollZoom),
    nameof(Astro.ScrollZoom),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/AstroDogeDX/Astro_CVR_Mods"
)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 249, 190, 124)]
[assembly: MelonAuthorColor(255, 255, 127, 127)]

namespace Astro.ScrollZoom.Properties;
internal static class AssemblyInfoParams
{
    public const string Version = "1.0.1";
    public const string Author = "AstroDoge";
}