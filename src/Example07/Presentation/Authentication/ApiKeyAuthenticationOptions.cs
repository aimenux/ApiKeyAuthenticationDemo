﻿using Microsoft.AspNetCore.Authentication;

namespace Example07.Presentation.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string AuthenticationScheme { get; set; } = ApiKeyConstants.ApiKeyScheme;
    public string AuthenticationType { get; set; } = ApiKeyConstants.ApiKeyScheme;
}