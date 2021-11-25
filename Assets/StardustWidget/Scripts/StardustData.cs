public static class StardustData
{
    #region Properties
    /// Url logo
    /// https://upload.wikimedia.org/wikipedia/commons/thumb/c/ce/Coca-Cola_logo.svg/640px-Coca-Cola_logo.svg.png
    public const string LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/ce/Coca-Cola_logo.svg/640px-Coca-Cola_logo.svg.png";
    public const string TermsOfServiceUrl = "https://www.stardust.gg/";
    public const string PrivacyPolicy = "https://www.stardust.gg/";
    public const string Api = "https://bddtm60cbd.execute-api.us-east-1.amazonaws.com/v1/oauth2/token";

    public const string AppClientID = "4rlsk57pb34chl7819oh7tm7f7"; // App client ID, found under App Client Settings
    public const string AuthCognitoDomain = "stardustplayers-dev"; // Found under App Integration and them Domain Name.

#if UNITY_EDITOR
    public const string RedirectUrl = "http://localhost:3000"; 
#else
    public const string RedirectUrl = "myapp://dev.d3twdqo7cb5otm.amplifyapp.com/";
#endif
    public const string Region = "us-east-1"; // Update with your region, the AWS region that contains your services
#endregion
}
