using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Marbotic.Framework.UI {

  public class PopupManager {

    readonly Canvas _canvas;

    readonly Popup _defaultPopupPrefab;

    readonly Transform _errorContentPrefab;

    readonly Dictionary<Popup, (Action<Popup> validated, Action<Popup> dismissed)> _popupCallbacks = new();

    public PopupManager(Canvas canvas, Popup defaultPopupPrefab) {
      _canvas = canvas;
      _defaultPopupPrefab = defaultPopupPrefab;
    }

    public Popup ShowWithContent(Transform content, PopupAction validateAction = default, PopupAction dismissAction = default) => ShowWithContent(null, content, validateAction, dismissAction);

    public Popup ShowWithContent(Popup popup, Transform content, PopupAction validateAction = default, PopupAction dismissAction = default) {
      popup = SetupPopupInstance(validateAction, dismissAction, popup);
      content.SetParent(popup.content, false);
      SetPopupVisible(popup, true);
      return popup;
    }

    public Popup Show(PopupAction validateAction = default, PopupAction dismissAction = default) => Show(null, validateAction, dismissAction);

    public Popup Show(Popup popup, PopupAction validateAction = default, PopupAction dismissAction = default) {
      popup = SetupPopupInstance(validateAction, dismissAction, popup);
      SetPopupVisible(popup, true);
      return popup;
    }

    public Popup ShowError(string errorDescription) {
      var popup = Show("Too bad !");
      var errorContent = UnityEngine.Object.Instantiate(_errorContentPrefab, popup.content);
      errorContent.GetComponentInChildren<TextMeshProUGUI>().text = errorDescription;
      return popup;
    }

    Popup SetupPopupInstance(PopupAction validateAction, PopupAction dismissAction, Popup popup) {
      popup ??= UnityEngine.Object.Instantiate(_defaultPopupPrefab, _canvas.transform);
      popup.validateTitle = validateAction.title;
      popup.dismissTitle = dismissAction.title;
      _popupCallbacks[popup] = (
        validated: popup => { OnPopupFinished(popup); validateAction.action?.Invoke(popup); },
        dismissed: popup => { OnPopupFinished(popup); dismissAction.action?.Invoke(popup); }
      );
      popup.validated += _popupCallbacks[popup].validated;
      popup.dismissed += _popupCallbacks[popup].dismissed;
      return popup;
    }

    void OnPopupFinished(Popup popup) {
      popup.validated -= _popupCallbacks[popup].validated;
      popup.dismissed -= _popupCallbacks[popup].dismissed;
      _popupCallbacks.Remove(popup);
      SetPopupVisible(popup, false);
    }

    void SetPopupVisible(Popup popup, bool visible) {
      popup.gameObject.SetActive(visible);
      popup.transform.SetAsLastSibling();
    }
  }
}
