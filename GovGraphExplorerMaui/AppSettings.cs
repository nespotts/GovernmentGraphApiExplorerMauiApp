using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovGraphExplorerMaui;

internal static class AppSettings {
    public static string TokenBaseUrl { get; set; } = "https://login.microsoftonline.us";
    public static string TenantId { get; set; } = "";
    public static string ClientId { get; set; } = "";
    public static string ClientSecret { get; set; } = "";
    public static string GraphApi { get; set; } = "https://graph.microsoft.us/v1.0";
    public static string Scope { get; set; } = "https://graph.microsoft.us/.default";
}
