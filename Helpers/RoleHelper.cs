using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClientesApp.Helpers;

public static class RoleHelper
{
    public static bool IsAdmin(ClaimsPrincipal user)
        => user?.Identity?.IsAuthenticated == true &&
           user.IsInRole("Administrador");

    public static string NomeUsuario(ClaimsPrincipal user)
        => user?.Identity?.IsAuthenticated == true
            ? user.Identity.Name ?? "Usuário"
            : string.Empty;
}
