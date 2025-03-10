namespace JUMS.Domain.Infrastructure;

public interface ITenantProvider
{
    string GetTenant();
}