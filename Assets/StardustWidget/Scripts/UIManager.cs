using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

// Manages all the text and button inputs
// Also acts like the main manager script for the game.
public class UIManager : MonoBehaviour
{
    #region Properties
    [Header("Logo Refs")]
    [SerializeField] private RawImage stardustLogo;
    [SerializeField] private RawImage stardustButtonLogo;


    [Header("UI Holders")]
    [SerializeField] private GameObject _unauthInterface;
    [SerializeField] private GameObject _authInterface;

    [Header("Login buttons")]
    [SerializeField] Button LoginButton;
    [SerializeField] Button LogoutButton;

    [Header("Terms and Privacy")]
    [SerializeField] Button TermsAndServicesButton;
    [SerializeField] Button PrivacyButton;


    [Header("Social Buttons")]
    [SerializeField] Button googleSignupButton;
    [SerializeField] Button facebookSignupButton;
    [SerializeField] Button appleSignupButton;
    [SerializeField] Button discordSignupButton;

    [Header("Snackbar")]
    [SerializeField] private SnackBar snackBarRefs;

    [Header("Editor fields")]
    // WARNING: these fields and the below #defines are for development and testing the code exchange used in the social login.
    [SerializeField] Button urlWithCodeButton;
    [SerializeField] InputField urlWithCodeField;

    private AuthenticationManager _authenticationManager;
    public GameObject snackBar;
    private bool resizeDone = false;

    #endregion

    #region Unity Functions
    void Awake()
    {
        _authInterface.SetActive(false);
        LogoutButton.gameObject.SetActive(false);
        LogoutButton.gameObject.SetActive(true);

        _authenticationManager = FindObjectOfType<AuthenticationManager>();

#if !UNITY_EDITOR
      // WARNING: For development and testing of the code exchange. Hide these when NOT in editor mode.
      urlWithCodeButton.gameObject.SetActive(false);
      urlWithCodeField.gameObject.SetActive(false);
#endif
    }

    void Start()
    {
        Init();
        StartCoroutine(Initialization());
    }
    #endregion

    private void Init()
    {
        if (StardustData.LogoUrl != "" || StardustData.LogoUrl != " ")
        {
            StartCoroutine(DownloadImage(stardustLogo, StardustData.LogoUrl));
            StartCoroutine(DownloadImage(stardustButtonLogo, StardustData.LogoUrl));
        }

        snackBar.SetActive(false);
    }

    IEnumerator ShowStatus(bool successfulRefresh)
    {
        yield return new WaitForEndOfFrame();

        if (successfulRefresh)
            SuccessfullLogin();
        else
            ErrorLogin(AuthenticationManager._status);


        yield return new WaitForSeconds(5f);
            snackBar.SetActive(false);
    }

        #region Functions
        private void displayComponentsFromAuthStatus(bool authStatus)
    {
        if (authStatus)
        {
            // Debug.Log("User authenticated, show welcome screen with options");
            _unauthInterface.gameObject.SetActive(false);
            _authInterface.SetActive(true);
            LogoutButton.gameObject.SetActive(true);
        }
        else
        {
            // Debug.Log("User not authenticated, activate/stay on login scene");
            _unauthInterface.gameObject.SetActive(true);
            _authInterface.SetActive(false);
            LogoutButton.gameObject.SetActive(false);
        }
    }

    public async void ProcessDeepLink(string deepLinkUrl)
    {
        Debug.Log("UIInputManager.ProcessDeepLink: " + deepLinkUrl);
        bool exchangeSuccess = await _authenticationManager.ExchangeAuthCodeForAccessToken(deepLinkUrl);

        if (exchangeSuccess)
        {
            _unauthInterface.SetActive(false);
            _authInterface.SetActive(true);
            LogoutButton.gameObject.SetActive(true);
        StartCoroutine(ShowStatus(exchangeSuccess));
        }
        else
        {
        StartCoroutine(ShowStatus(exchangeSuccess));
        }
    }

    private void onLoginClicked(string Provider)
    {
        Debug.Log("onLoginClicked ");
        string loginUrl = _authenticationManager.GetLoginUrl(Provider);
#if UNITY_WSA
        UnityEngine.WSA.Launcher.LaunchUri(loginUrl, false);
#else
        Application.OpenURL(loginUrl);
#endif
    }

    private void onGoogleLoginClicked()
    {
        onLoginClicked("Google");
    }

    private void onFacebookLoginClicked()
    {
        onLoginClicked("Facebook");
    }

    private void onAppleLoginClicked()
    {
        onLoginClicked("Apple");
    }

    private void onDiscordLoginClicked()
    {
        onLoginClicked("Discord");
    }

    private void onLogoutClick()
    {
        _authenticationManager.Logout();
        displayComponentsFromAuthStatus(false);
    }
    
    private void onLoginClick()
    {
        if (!_unauthInterface.activeInHierarchy)
        {
            _unauthInterface.SetActive(true);
        }
        else
            _unauthInterface.SetActive(false);
    }

    // Pass in URL from link a player clicked on from our game forums
    void OpenBrowser(string url)
    {
#if UNITY_WSA
        //System.Diagnostics.Process.Start(url);
        UnityEngine.WSA.Launcher.LaunchUri(url,false);
#else
        Application.OpenURL(url);
#endif
    }

    public void CloseWidget()
    {
        _unauthInterface.SetActive(false);
    }

    public void ShowSnackbar()
    {
        snackBar.SetActive(true);
    }

    public void ShowPrivacyPolicy()
    {
        OpenBrowser(StardustData.PrivacyPolicy);

    }

    public void ShowTermsOfService()
    {
        OpenBrowser(StardustData.TermsOfServiceUrl);
    }

    public void SuccessfullLogin()
    {
        snackBar.SetActive(true);
        snackBarRefs.image.sprite = snackBarRefs.successfulSprite;
        snackBarRefs.mainMessage.text = "Succesfully Logged In";
        snackBarRefs.secondaryMessage.text = "";
    }

    public void ErrorLogin(string message)
    {
        snackBar.SetActive(true);
        snackBarRefs.image.sprite = snackBarRefs.errorSprite;
        snackBarRefs.mainMessage.text = "Error Loggin in. Please try again.";
        snackBarRefs.secondaryMessage.text = message;
    }

    private async void RefreshToken()
    {
        bool successfulRefresh = await _authenticationManager.CallRefreshTokenEndpoint();
        displayComponentsFromAuthStatus(successfulRefresh);
    }

#if UNITY_EDITOR
    private void onCodeClick()
    {
        if (urlWithCodeField && urlWithCodeField.text != "")
        {
            ProcessDeepLink(urlWithCodeField.text);
        }
    }
#endif

    /// Handle the brand logo size and keeps a correct ratio
    public static Vector2 SizeToParent(RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }

        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        return imageTransform.sizeDelta;
    }
    #endregion

    #region Coroutines
    IEnumerator Initialization()
    {
        while (resizeDone == false)
        {
            yield return null;
        }

        Debug.Log("UIInputManager: Start");

        // We perform the refresh here to keep our user's session alive so they don't have to keep logging in.
        RefreshToken();

        googleSignupButton.onClick.AddListener(onGoogleLoginClicked);
        facebookSignupButton.onClick.AddListener(onFacebookLoginClicked);
        appleSignupButton.onClick.AddListener(onAppleLoginClicked);
        discordSignupButton.onClick.AddListener(onDiscordLoginClicked);

        LogoutButton.onClick.AddListener(onLogoutClick);
        LoginButton.onClick.AddListener(onLoginClick);

        TermsAndServicesButton.onClick.AddListener(ShowTermsOfService);
        PrivacyButton.onClick.AddListener(ShowPrivacyPolicy);

#if UNITY_EDITOR
        urlWithCodeButton.onClick.AddListener(onCodeClick);
#endif
    }

    IEnumerator DownloadImage(RawImage image, string logoUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(logoUrl);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log(request.error);
            yield return new WaitForEndOfFrame();
            resizeDone = true;
        }
        else
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            SizeToParent(image, 0.1f);
            yield return new WaitForEndOfFrame();
            resizeDone = true;           
        }

    }
    #endregion
}

[System.Serializable]
public class SnackBar
{
    public Image image;
    public Sprite successfulSprite;
    public Sprite errorSprite;
    public TMPro.TextMeshProUGUI mainMessage;
    public TMPro.TextMeshProUGUI secondaryMessage;
}