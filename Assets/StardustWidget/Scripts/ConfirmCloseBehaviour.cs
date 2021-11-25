using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Windows.UI.Core.Preview;
#endif

class ConfirmCloseBehaviour : MonoBehaviour
{
    void Awake()
    {
#if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
        }, false);
#endif
    }

#if ENABLE_WINMD_SUPPORT
   void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
   {
       Debug.Log("Got here!");
   }
#endif
}