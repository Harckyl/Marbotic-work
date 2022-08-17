using UnityEngine.Events;
using UnityEngine;

namespace Marbotic.Game.Literacy{

  using Core.Holders.Models;
  using Core.SceneManagement.Events;
  using Holders.Models;

  public class ActivityLoader : MonoBehaviour {

    [SerializeField] GameSceneEvent _startLevel;
    [SerializeReference] ILevelManager _levelManager;

    public void LaunchActivity() {
      if (!_levelManager.selectedLevel.HasValue) { return; }

      var selectedLevel = _levelManager.selectedLevel.Value;
      _levelManager.LoadDataToPlay(selectedLevel);
      _startLevel.Invoke(_levelManager.GetActivityScene(selectedLevel));
    }
  }
}
