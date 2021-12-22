using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Amazon.Amplify.Internal;

public class ApiManager : MonoBehaviour
{

    [SerializeField] private Button GetGameIdButton;
    [SerializeField] private Button SetGameIdButton;

    // "Include the identity token in the Authorization header... "
    private const string Api = StardustData.Api;
    private const string GameIdUrl = "";
    private AuthenticationManager _authenticationManager;

    #region Unity Functions
    void Awake()
   {
      _authenticationManager = FindObjectOfType<AuthenticationManager>();
   }

    private void Start()
    {
        Init();
    }
    #endregion

    private void Init()
    {
        GetGameIdButton.onClick.AddListener(GetGameId);
        SetGameIdButton.onClick.AddListener(SetGameId);
    }

    public async void CallApi()
   {
      UnityWebRequest webRequest = UnityWebRequest.Get(Api);

      // https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-invoke-api-integrated-with-cognito-user-pool.html
      webRequest.SetRequestHeader("Authorization", _authenticationManager.GetIdToken());

      await webRequest.SendWebRequest();

      if (webRequest.result != UnityWebRequest.Result.Success)
      {
         Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
      }
      else
      {
         Debug.Log("Success, API call complete!");
         Debug.Log(webRequest.downloadHandler.text);
      }
      webRequest.Dispose();
   }

    public async void GetGameId()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(GameIdUrl);

        // https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-invoke-api-integrated-with-cognito-user-pool.html
        webRequest.SetRequestHeader("Authorization", _authenticationManager.GetIdToken());

        await webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
        }
        else
        {
            Debug.Log("Success, API call complete!");
            Debug.Log(webRequest.downloadHandler.text);
        }
        webRequest.Dispose();
    }

    public async void SetGameId()
    {

    }

    public async Task<bool> CallGetGameIdEndpoint()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);

        string preservedRefreshToken = "";

        if (userSessionCache != null && userSessionCache._refreshToken != null && userSessionCache._refreshToken != "")
        {
            // DOCS: https://docs.aws.amazon.com/cognito/latest/developerguide/token-endpoint.html
            
            string getGameIdUrl = "";

            // Debug.Log(getGameIdUrl);

            preservedRefreshToken = userSessionCache._refreshToken;

            WWWForm form = new WWWForm();
            //form.AddField("grant_type", RefreshTokenGrantType);
            //form.AddField("client_id", AppClientID);
            //form.AddField("refresh_token", userSessionCache._refreshToken);

            UnityWebRequest webRequest = UnityWebRequest.Post(getGameIdUrl, form);
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            await webRequest.SendWebRequest();


            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Refresh token call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
                // clear out invalid user session data to force re-authentication
                //ClearUserSessionData();
                webRequest.Dispose();
            }
            else
            {
                Debug.Log("Success, Refresh token call complete!");
                // Debug.Log(webRequest.downloadHandler.text);

                AuthenticationResultType authenticationResultType = JsonUtility.FromJson<AuthenticationResultType>(webRequest.downloadHandler.text);

                // token endpoint to get refreshed access token does NOT return the refresh token, so manually save it from before.
                authenticationResultType.refresh_token = preservedRefreshToken;

                //_userid = AuthUtilities.GetUserSubFromIdToken(authenticationResultType.id_token);

                // update session cache
                //SaveDataManager.SaveJsonData(new UserSessionCache(authenticationResultType, _userid));
                webRequest.Dispose();
                return true;
            }
        }
        return false;
    }
}

