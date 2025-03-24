using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovGraphExplorerMaui;
internal class QueryModel {
    public string Nickname { get; set; } = "";
    public string Query { get; set; } = "";
    public string HttpVerb { get; set; } = "Get";
    public Dictionary<string, string> HeaderParameters { get; set; } = new();
    public string? RequestBody { get; set; } = null;

    public string GetAbbreviatedQuery(int length) {
        return Query.Substring(0, Math.Min(length, Query.Length));
    }

    public string GetAbbreviatedRequestBody(int length) {
        if (string.IsNullOrEmpty(RequestBody)) return "";
        return RequestBody.Substring(0, Math.Min(length, RequestBody.Length));
    }
}
