using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common;

public class TokenGenerator
{
    public string CreateJwt(string userId, string issKey, string iss, string aud)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Hash, Random.Shared.Next().ToString()),
            ];

        var issuedAt = DateTime.UtcNow;
        var expires = issuedAt.AddHours(12);

        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(
            issuer: iss,
            audience: aud,
            claims: claims,
            notBefore: null,
            expires: expires,
            issuedAt: issuedAt);

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
