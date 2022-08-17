using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Marbotic.Framework.Languages {

  using Core.Holders.Serialization.Dictionaries.Models;
  using Localization.Models;

  public class LanguageButton : MonoBehaviour {

    [SerializeField] Toggle _button;
    public Toggle button => _button;

    [Space, SerializeField] Image _flag;
    [SerializeField] StringConfigurationHolderSpriteScriptableDictionary _flags;

    [Space, SerializeField] TextMeshProUGUI _label;
    [SerializeField] StringConfigurationHolderStringScriptableDictionary _names;

    LanguageCode _language;
    public LanguageCode language {
      get => _language;
      set {
        if (_flags.TryGetValue(value.Code, out var flag)) {
          _language = value;
          if (_flag != null) { _flag.sprite = flag; }
        }
        if ((_label != null) && _names.TryGetValue(value.Code, out var label)) {
          _label.text = label;
        }
      }
    }
  }
}
