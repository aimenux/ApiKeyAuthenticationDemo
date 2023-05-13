using Microsoft.AspNetCore.Mvc;

namespace Example05.Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeySecurityAttribute : ServiceFilterAttribute
{
    public ApiKeySecurityAttribute() : base(typeof(ApiKeySecurityFilter))
    {
    }
}