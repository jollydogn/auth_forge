namespace AuthForge.Exceptions;

/// <summary>
/// AuthForge modülünde fırlatılabilecek tüm hata kodları.
/// Prefix: AuthForge:NNNNN
/// </summary>
public static class AuthForgeErrorCodes
{
    // Auth (01000 serisi)
    public const string InvalidCredentials = $"{FeatureConstants.AuthForgeFeatureName}:01001";
    public const string TokenGenerationFailed = $"{FeatureConstants.AuthForgeFeatureName}:01002";
    public const string TokenRefreshFailed = $"{FeatureConstants.AuthForgeFeatureName}:01003";
    public const string LoginFailed = $"{FeatureConstants.AuthForgeFeatureName}:01004";
    public const string TokenRevocationFailed = $"{FeatureConstants.AuthForgeFeatureName}:01005";
    public const string InvalidToken = $"{FeatureConstants.AuthForgeFeatureName}:01006";

    // Users (02000 serisi)
    public const string UserNotFound = $"{FeatureConstants.AuthForgeFeatureName}:02001";
    public const string UserAlreadyExists = $"{FeatureConstants.AuthForgeFeatureName}:02002";
    public const string UserCreationFailed = $"{FeatureConstants.AuthForgeFeatureName}:02003";
    public const string UserUpdateFailed = $"{FeatureConstants.AuthForgeFeatureName}:02004";
    public const string UserDeletionFailed = $"{FeatureConstants.AuthForgeFeatureName}:02005";
    public const string ResetPasswordFailed = $"{FeatureConstants.AuthForgeFeatureName}:02006";
    public const string ForgotPasswordFailed = $"{FeatureConstants.AuthForgeFeatureName}:02007";

    // Roles (03000 serisi)
    public const string RoleNotFound = $"{FeatureConstants.AuthForgeFeatureName}:03001";
    public const string RoleAlreadyExists = $"{FeatureConstants.AuthForgeFeatureName}:03002";
    public const string RoleCreationFailed = $"{FeatureConstants.AuthForgeFeatureName}:03003";
    public const string RoleDeletionFailed = $"{FeatureConstants.AuthForgeFeatureName}:03004";

    // Groups (04000 serisi)
    public const string GroupNotFound = $"{FeatureConstants.AuthForgeFeatureName}:04001";
    public const string GroupAlreadyExists = $"{FeatureConstants.AuthForgeFeatureName}:04002";
    public const string GroupCreationFailed = $"{FeatureConstants.AuthForgeFeatureName}:04003";
    public const string GroupDeletionFailed = $"{FeatureConstants.AuthForgeFeatureName}:04004";

    // Configuration / Client (05000 serisi)
    public const string ConfigurationError = $"{FeatureConstants.AuthForgeFeatureName}:05001";
    public const string ExternalApiError = $"{FeatureConstants.AuthForgeFeatureName}:05002";
}
