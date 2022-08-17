using UnityEngine;
using UnityEngine.UI;

namespace Marbotic.Framework.Languages {

  public class LanguageButtonLock : MonoBehaviour {

    [SerializeField] GameObject _lock;

    public void SetLocked(bool isLocked) => _lock.SetActive(isLocked);
  }
}
