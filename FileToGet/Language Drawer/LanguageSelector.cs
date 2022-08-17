using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Marbotic.Framework.Languages {

  using Core.Collections;
  using Core.Holders.Models;
  using Core.Registers;
  using Localization.Holders.Models;
  using Localization.Models;

  using ButtonsListeners = Dictionary<LanguageButton, UnityAction<bool>>;

  public class LanguageSelector : MonoBehaviour {

    [SerializeField] LanguageHolder _languageHolder;
    [SerializeField] ToggleGroup _languageButtonsRoot;
    [SerializeField] LanguageButton _languageButtonPrefab;

    [Space, SerializeField] LocalizedAudioClip _currentLanguageAudioClip;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSourceHolder _sfxAudioSource;

    [Space, SerializeField] GameObject _loadingPanel;

    public event Action<LanguageButton, bool> languageSelectionChanged;

    Language _language;
    public Language language => _language = _language ?? Registry.Instance.Get<Language>();

    readonly ButtonsListeners _languageButtonsListeners = new ButtonsListeners();
    ButtonsListeners languageButtonsListeners { get {
      if (_languageButtonsListeners.Count == 0) {
        foreach (var language in _languageHolder.Value.AvailableCodes) {
          var languageButton = Instantiate(_languageButtonPrefab, _languageButtonsRoot.transform);
          languageButton.language = language;
          languageButton.button.group = _languageButtonsRoot;
          var listener = new UnityAction<bool>(selected => OnLanguageSelectionChange(languageButton, selected));
          _languageButtonsListeners[languageButton] = listener;
        }
      }
      return _languageButtonsListeners;
    }}

    public IEnumerable<LanguageButton> languageButtons => languageButtonsListeners.Keys;

    void OnEnable() {
      if (language.isLoading) {
        language.OnChanged += InitializeButtons;
      } else {
        InitializeButtons(language.Current);
      }
    }

    void InitializeButtons(LanguageCode languageCode) {
      language.OnChanged -= InitializeButtons;
      foreach (var (languageButton, listener) in languageButtonsListeners) {
        languageButton.button.isOn = languageButton.language == language.Current;
        languageButton.button.interactable = !languageButton.button.isOn;
        languageButton.button.onValueChanged.AddListener(listener);
      }
    }

    void OnDisable() {
      foreach (var (languageButton, listener) in languageButtonsListeners) {
        languageButton.button.onValueChanged.RemoveListener(listener);
      }
    }

    void OnLanguageSelectionChange(LanguageButton languageButton, bool selected) {
      languageButton.button.interactable = !selected;
      if (selected) {
        _sfxAudioSource.Play();
        ChangeLanguage(languageButton.language);
      }
      languageSelectionChanged?.Invoke(languageButton, selected);
    }

    void ChangeLanguage(LanguageCode languageCode) {
      _loadingPanel.SetActive(true);

      language.Change(languageCode, () => {
        _loadingPanel.SetActive(false);
        _audioSource.clip = _currentLanguageAudioClip.Asset;
        _audioSource.Play();
      });
    }
  }
}
