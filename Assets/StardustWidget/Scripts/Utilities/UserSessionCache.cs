using UnityEngine;

// Store user session data to use later to refresh tokens
[System.Serializable]
public class UserSessionCache : ISaveable
{
    public string _idToken;
    public string _refreshToken;
    public string _userId;

    public UserSessionCache() { }

    public UserSessionCache(AuthenticationResultType authenticationResultType, string userId)
    {
        _idToken = authenticationResultType.id_token;
        _refreshToken = authenticationResultType.refresh_token;
        _userId = userId;
    }

    public string getIdToken()
    {
        return _idToken;
    }

    public string getRefreshToken()
    {
        return _refreshToken;
    }

    public string getUserId()
    {
        return _userId;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonToLoadFrom)
    {
        JsonUtility.FromJsonOverwrite(jsonToLoadFrom, this);
    }

    public string FileNameToUseForData()
    {
        return "data_01.dat";
    }
}