namespace SentinelaDocumentos.Application.DTOs.Auth
{
    public sealed class AuthResponseDto
    {
        public bool IsSuccess { get; init; }
        public required string Message { get; init; }
        public string? Token { get; init; } // Tornado anulável
        public object? UserInfo { get; init; } // Tornado anulável
    }
}