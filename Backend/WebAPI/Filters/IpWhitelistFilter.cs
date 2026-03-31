using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using WebAPI.Configurations;

namespace WebAPI.Filters;

public class IpWhitelistFilter : ActionFilterAttribute
{
    private readonly List<Regex> _patterns;
    private readonly bool _allowAll;

    public IpWhitelistFilter(IOptions<IPWhitelistConfiguration> configuration)
    {
        var rawPatterns = configuration.Value.IPs ?? Array.Empty<string>();

        _allowAll = rawPatterns.Any(p => p.Trim() == "*");
        _patterns = rawPatterns
            .Where(p => p.Trim() != "*")
            .Select(ConvertToRegex)
            .ToList();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (_allowAll) return;

        var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(remoteIp) || !_patterns.Any(p => p.IsMatch(remoteIp)))
        {
            context.Result = new ForbidResult();
        }
    }

    /// <summary>
    /// Chuyển đổi pattern dạng glob (wildcard) sang Regex.
    /// Hỗ trợ:
    ///   - "*" → cho phép tất cả (xử lý riêng)
    ///   - "172.26.*" → match mọi IP bắt đầu bằng 172.26.
    ///   - "192.168.0.*" → match subnet
    ///   - "::ffff:172.*" → match IPv6-mapped IPv4
    ///   - "::1" → match exact
    ///   - "10.0.0.1" → match exact
    /// </summary>
    private static Regex ConvertToRegex(string pattern)
    {
        // Escape special regex characters, then replace escaped wildcard (\*) back to regex (.*)
        var escaped = Regex.Escape(pattern.Trim());
        var regexPattern = "^" + escaped.Replace("\\*", ".*") + "$";
        return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}