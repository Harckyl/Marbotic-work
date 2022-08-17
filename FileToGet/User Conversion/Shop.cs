using System;

using UnityEngine;

namespace Marbotic.Game.OneClickBuy {

  [Serializable]
  public struct Shop {

    [SerializeField] string _domain;
    public string domain => _domain;

    [SerializeField] string _accessToken;
    public string accessToken => _accessToken;

    [SerializeField] string _name;
    public string name => _name;
  }
}
