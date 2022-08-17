using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;

namespace Marbotic.Discovery.FiveF.FingerCam {

  using Framework.UI;

  public class LevelSelector : MonoBehaviour {

    [SerializeField] AnimatedButton _button;

    [SerializeField] int _levelDifficulty;

    [SerializeField] string _sceneName;
    ZenjectSceneLoader _sceneLoader;

    [Inject]
    public void Initialize(ZenjectSceneLoader sceneLoader) => _sceneLoader = sceneLoader;

    void OnEnable() => _button.clicked += LaunchScene;

    void OnDisable() => _button.clicked -= LaunchScene;

    void LaunchScene(AnimatedButton animatedButton) {
      _sceneLoader.LoadScene(_sceneName, LoadSceneMode.Single, container => {
        container.Bind<FingerCamDifficulty>().WithArguments(_levelDifficulty).WhenInjectedInto<FingerCamInstaller>();
      });
    }
  }
}
