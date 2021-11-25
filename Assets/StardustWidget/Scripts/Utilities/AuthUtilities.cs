using UnityEngine;

public class AuthUtilities
{
    public static string GetUserSubFromIdToken(string idToken)
    {
        string[] parts = idToken.Split('.');
        string payload = parts[1];

        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

        string payloadJson = payload.DecodeBase64();

        TokenPayload payloadData = JsonUtility.FromJson<TokenPayload>(payloadJson);

        return payloadData.getSub();
    }
}