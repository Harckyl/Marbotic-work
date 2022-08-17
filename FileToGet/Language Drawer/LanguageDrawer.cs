using UnityEngine;
using UnityEngine.UI;

namespace Marbotic.Framework.Languages {

  using Core.Attributes;
  using Core.Holders.Models;
  using Core.Locks;
  using Core.Registers;

  using Framework.UI;
  using Localization.Models;

  public class LanguageDrawer : MonoBehaviour {

    static readonly int _openAnimationTrigger = Animator.StringToHash("Open");

    [SerializeField] LanguageSelector _languageSelector;
    [SerializeField] Button _drawerButton;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSourceHolder _sfxAudioSource;

    [Space, SerializeField, TypeConstraint(typeof(Lock))] UnityEngine.Object _languageLock;
    Lock languageLock => _languageLock as Lock;

    [SerializeField] StringConfigurationHolder _contentPreviewCode;

    [Space, SerializeField] ContentPreviewPopupManager _contentPreviewManager;

    Language _language;
    public Language language => _language = _language ?? Registry.Instance.Get<Language>();

    bool _opened;

    void OnEnable() {
      _opened = false;
      _drawerButton.onClick.AddListener(OnDrawerButtonClicked);
      _languageSelector.languageSelectionChanged += OnLanguageSelectionChange;
      if (language.isLoading) {
        language.OnChanged += InitializeLocks;
      } else {
        InitializeLocks(language.Current);
      }
    }

    void OnDisable() {
      _drawerButton.onClick.RemoveListener(OnDrawerButtonClicked);
      _languageSelector.languageSelectionChanged -= OnLanguageSelectionChange;
    }

    void InitializeLocks(LanguageCode languageCode) {
      language.OnChanged -= InitializeLocks;
      var otherLanguagesLocked = languageLock.isLocked;
      foreach (var languageButton in _languageSelector.languageButtons) {
        var selected = languageButton.language == languageCode;
        OnLanguageSelectionChange(languageButton, selected);
        languageButton.GetComponent<LanguageButtonLock>().SetLocked(!selected && otherLanguagesLocked);
      }
    }

    void OnLanguageSelectionChange(LanguageButton languageButton, bool selected) {
      if (selected) { languageButton.transform.SetAsLastSibling(); }
    }

    void OnDrawerButtonClicked() {
      _drawerButton.gameObject.SetActive(languageLock.isLocked);

      _sfxAudioSource.Play();
      if (!_opened) { OpenDrawer(); }
      else if (languageLock.isLocked) { OpenContentPreview(); }
    }

    void OpenDrawer() {
      _animator.SetTrigger(_openAnimationTrigger);
      _opened = true;
    }

    void OpenContentPreview() {
      _contentPreviewManager.SetCurrentContent(_contentPreviewCode);
      _contentPreviewManager.gameObject.SetActive(true);
    }
  }
}
