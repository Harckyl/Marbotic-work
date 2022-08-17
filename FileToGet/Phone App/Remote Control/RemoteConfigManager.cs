using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Marbotic.Discovery {
  public class RemoteConfigManager : MonoBehaviour {

    public struct userAttributes {
        // Optionally declare variables for any custom user attributes:
    }

    public struct appAttributes {
        // Optionally declare variables for any custom app attributes:
    }

    async Task InitializeRemoteConfigAsync() {
      await UnityServices.InitializeAsync();

      // remote config requires authentication for managing environment information
      if (!AuthenticationService.Instance.IsSignedIn) {
          await AuthenticationService.Instance.SignInAnonymouslyAsync();
      }
    }

    // Retrieve and apply the current key-value pairs from the service on Awake:
    async Task Awake () {

        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection()) { await InitializeRemoteConfigAsync(); }

        // Add a listener to apply settings when successfully retrieved:
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.SetCustomUserID("some-user-id");
        RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings (ConfigResponse configResponse) {
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin) {
            case ConfigOrigin.Default:
                Debug.Log ("No settings loaded this session; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log ("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                Debug.Log ("New settings loaded this session; update values accordingly.");
                Debug.Log(RemoteConfigService.Instance.appConfig.GetJson("mister finger pose"));
                break;
        }
    }


  }
}