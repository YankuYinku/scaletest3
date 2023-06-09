﻿global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Hosting;
global using System.Collections.Generic;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.OpenApi.Models;
global using Serilog;
global using Microsoft.AspNetCore.Mvc;
global using apetito.meinapetito.Portal.Api.GraphQl;
global using apetito.meinapetito.Portal.Api.GraphQl.Options;
global using HotChocolate.AspNetCore;
global using Open.Serialization.Json.Newtonsoft;
global using apetito.meinapetito.Portal.Api;
global using apetito.Cors;
global using PlaygroundOptions = HotChocolate.AspNetCore.Playground.PlaygroundOptions;
global using Azure.Core;
global using Azure.Identity;
global using Microsoft.Extensions.Configuration.AzureAppConfiguration;
global using apetito.meinapetito.Portal.Application.Dashboard.Options;