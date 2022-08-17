using System;

using TMPro;

using UnityEngine;

namespace  Marbotic.Framework.UI {

  public class Popup : MonoBehaviour {

    [SerializeField] Transform _content;
    public Transform content => _content;

    [SerializeField] ButtonAction _validateButton;
    public event Action<Popup> validated;

    [SerializeField] ButtonAction _dismissButton;
    public event Action<Popup> dismissed;

    public string validateTitle {
      get => GetButtonTitle(_validateButton);
      set => SetButtonTitle(_validateButton, value);
    }

    public string dismissTitle {
      get => GetButtonTitle(_dismissButton);
      set => SetButtonTitle(_dismissButton, value);
    }

    void OnEnable() {
      _validateButton.triggered += OnValidated;
      _dismissButton.triggered += OnDismissed;
    }

    void OnDisable() {
      _validateButton.triggered -= OnValidated;
      _dismissButton.triggered -= OnDismissed;
    }

    public void OnValidated(ButtonAction button) => NotifyValidated();

    public void OnDismissed(ButtonAction button) => NotifyDismissed();

    public void NotifyValidated() => validated?.Invoke(this);

    public void NotifyDismissed() => dismissed?.Invoke(this);

    string GetButtonTitle(ButtonAction button) => GetButtonText(button)?.text;

    void SetButtonTitle(ButtonAction button, string title) {
      button.gameObject.SetActive(title != null);
      var titleText = GetButtonText(button);
      if (titleText != null) {
        titleText.text = title;
      }
    }

    TextMeshProUGUI GetButtonText(ButtonAction button) => button.GetComponentInChildren<TextMeshProUGUI>();
  }
}
