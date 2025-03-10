using Microsoft.AspNetCore.Http;
using System.Linq;
using JUMS.Domain.Infrastructure;

namespace JUMS.Infrastructure.MultiTenancy;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetTenant()
    {
        // Retrieve the tenant from the request header "X-Tenant-Id"; default to "public" if not provided.
        return _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault() 
               ?? "public";
    }
}