using UnityEngine;

using System;

namespace Marbotic.Game.Literacy {
  
  using Core.Serialization.Dictionaries.Models;
  using Models;
  using Localization.Models;

  [Serializable]
  public class SerializedModalityLocalizationDictionary : SerializedDictionary<Modality, LocalizedString> { }

  public class ModalityLocalizationDictionary : ScriptableObject {

    [SerializeField] SerializedModalityLocalizationDictionary _dictionary;
    public SerializedModalityLocalizationDictionary dictionary => _dictionary;
  }
}
