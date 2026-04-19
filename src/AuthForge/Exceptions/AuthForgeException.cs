using System.Net;

namespace AuthForge.Exceptions;

/// <summary>
/// AuthForge modülüne ait spesifik exception sınıfı.
/// HTTP Status Code ve modül özel error code içerir.
/// </summary>
public class AuthForgeException : Exception
{
    /// <summary>Modüle özel hata kodu (Örn: AuthForge:00001)</summary>
    public string ErrorCode { get; }

    /// <summary>HTTP Durum Kodu</summary>
    public HttpStatusCode StatusCode { get; }

    public AuthForgeException(string errorCode, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public AuthForgeException(string errorCode, string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
