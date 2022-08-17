using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Marbotic.Game.Literacy {

  using Core.Attributes;
  using Core.Holders.Models;
  using Holders.Models;

  public class LevelButton : MonoBehaviour {

    [SerializeField] int _level;
    public int level => _level;

    [SerializeField] StringHolder _activityName;
    [SerializeField] UnityEvent _sendAnalyticsNewActivity, _sendAnalyticsReplay;
    [SerializeField] Image _icon;

    GameObject _reticuleClosingPoint;
    GameObject _reticule;

    public ILevelManager levelManager { get; set; }

    public void SetupReticule(GameObject reticuleClosingPoint, GameObject reticule) {
      _reticuleClosingPoint = reticuleClosingPoint;
      _reticule = reticule;
    }

    public void SetupIcon() => _icon.sprite = levelManager.GetActivityIcon(level);

    public void OnClick() {
      levelManager.selectedLevel = level;
      _reticuleClosingPoint.transform.position = transform.position;
      _reticule.GetComponent<Animator>().Play("scene-transition-close");
      SendAnalytics();
    }

    public void SendAnalytics() {
      var progression = levelManager.currentScaffoldingLevelIndex;
      _activityName.Value = levelManager.GetActivityName(level);

      if (progression == level) {
        _sendAnalyticsNewActivity.Invoke();
      } else {
        _sendAnalyticsReplay.Invoke();
      }
    }
  }
}
