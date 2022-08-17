using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Marbotic.Game.Literacy {

  using Marbotic.Localization.Models;
  using Marbotic.Game.Literacy.Holders.Models;

  public class PieceInfo : MonoBehaviour {

    [SerializeField] LevelButton[] _levels;
    [SerializeField] TextMeshProUGUI _title, _content;
    [SerializeField] LocalizedString _titleKey;
    [SerializeField] AlphabetHolder _alphabet;
    [SerializeField] ModalityLocalizationDictionary _dictionary;

    List<GameObject> _contentList = new List<GameObject>();
    void OnEnable() {
      _title.text = _titleKey.Asset;
      foreach (var level in _levels) {
        var scaffoldingLevel = _alphabet.Value.Scaffolding.levels[level.level];

        if (scaffoldingLevel.type == Models.LetterScaffoldingLevelType.Defined) {
          var content = Instantiate(_content, transform);
          content.text = $"{String.Join("-", scaffoldingLevel.letters)} {_dictionary.dictionary[scaffoldingLevel.modality].Asset}";
          _contentList.Add(content.gameObject);
        }
      }
    }


    void OnDisable() {
      foreach (var content in _contentList) {
        Destroy(content);
      }
    }
  }
}
