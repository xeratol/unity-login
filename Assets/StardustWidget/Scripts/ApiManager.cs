using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{

    // "Include the identity token in the Authorization header... "
    private const string Api = StardustData.Api;
    private AuthenticationManager _authenticationManager;

   void Awake()
   {
      _authenticationManager = FindObjectOfType<AuthenticationManager>();
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

}

