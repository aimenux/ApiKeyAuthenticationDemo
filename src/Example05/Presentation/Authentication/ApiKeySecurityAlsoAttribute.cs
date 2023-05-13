using Microsoft.AspNetCore.Mvc;

namespace Example05.Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeySecurityAlsoAttribute : TypeFilterAttribute
{
    public ApiKeySecurityAlsoAttribute() : base(typeof(ApiKeySecurityFilter))
    {
    }
}