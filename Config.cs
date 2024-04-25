namespace OpenIdClient
{
    public static class Config
    {
        public static readonly string tokenEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/token";
        public static readonly string clientSecret = "SdvTM6AhIdTWDk6goGyOoShgPaRAZ5z1";
        public static readonly string clientId = "Hans-Peter";
        public static readonly string keyCloakUrl = "http://localhost:8080/realms/master/protocol/openid-connect/auth";
        public static readonly string userInfoEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/userinfo";

        public static string codeVerifier;
        public static string state;
    }
}
