using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

using Object = UnityEngine.Object;

namespace Marbotic.Framework.UI {

  using Core.Attributes;
  using Core.Holders.Models;
  using Core.Locks;
  using Core.Serialization.Dictionaries.Models;
  using Localization.Models;

  public class ContentPreviewPopupManager : MonoBehaviour {

    [Serializable] struct PreviewDetails {

      [SerializeField] LocalizedString _title;
      public LocalizedString title => _title;

      [SerializeField] Color _color;
      public Color color => _color;

      [SerializeField] GameObject _visual;
      public GameObject visual => _visual;

      [SerializeField] StringConfigurationHolder _eventName;
      public StringConfigurationHolder eventName => _eventName;
    }

    [Serializable] class StringConfigurationHolderEvent : UnityEvent<StringConfigurationHolder> {}

    [Serializable] class StringLocalizedStringDictionary : SerializedDictionary<StringConfigurationHolder, PreviewDetails> {}

    [SerializeField] StringLocalizedStringDictionary _previewDetails;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] Image _background;
    [SerializeField] Transform _previewRoot;
    [SerializeField] StringConfigurationHolderEvent _event;

    GameObject _currentContent;

    [Header("IAP")]
    [SerializeField, TypeConstraint(typeof(Lock), null)] Object _isIAPAvailableLock;
    Lock isIAPAvailableLock => _isIAPAvailableLock as Lock;

    [SerializeField] GameObject _iapButton;

    public void SetCurrentContent(StringConfigurationHolder sectionCode) {
      if (!_previewDetails.TryGetValue(sectionCode, out var previewDetails)) { return; }

      _title.text = previewDetails.title.Asset;
      _background.color = previewDetails.color;
      _iapButton.SetActive(!isIAPAvailableLock.isLocked);
      _currentContent = Instantiate(previewDetails.visual, _previewRoot);
      _event?.Invoke(previewDetails.eventName);
    }

    void OnDisable() {
      if (_currentContent != null) {
        Destroy(_currentContent);
      }
    }
  }
}
