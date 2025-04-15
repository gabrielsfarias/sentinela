using System.IdentityModel.Tokens.Jwt;

namespace SentinelaDocumentos.Api;

public class JwtSettings
    {
    public string ChaveSecreta { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Publico { get; set; } = string.Empty;
    public string PublicoAlvo { get; internal set; } = string.Empty;
    public double TempoExpiracao { get; internal set; }
    }
