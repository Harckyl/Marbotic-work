using System;
using System.Collections.Generic;
using UnityEngine;
namespace Marbotic.Game.Literacy{
  using Core.Serialization.Dictionaries.Models;
  [Serializable]
  public class PiecesDictionary : SerializedDictionary<int, GameObject> { }
  public class ListPiece : ScriptableObject { 
    [SerializeField] PiecesDictionary _dictionary;

    public GameObject DictionaryPiece(int ID) => _dictionary[ID];
  }
}