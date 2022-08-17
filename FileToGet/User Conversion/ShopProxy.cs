using System;
using System.Linq;

using UnityEngine;

namespace Marbotic.Game.OneClickBuy {

  public class ShopProxy : ScriptableObject {

    [Serializable]
    class ShopIndirection {

      public string[] countryCodes;
      public Shop shop;

      public bool AcceptsCountryCode(string countryCode) => countryCodes.Contains(countryCode);
    }

    [SerializeField] ShopIndirection[] _shopsIndirections;
    [SerializeField] string _fallbackCountryCode;

    public Shop GetShop(string countryCode) {
      return (
        _shopsIndirections.FirstOrDefault(s => s.AcceptsCountryCode(countryCode)) ??
        _shopsIndirections.First(s => s.AcceptsCountryCode(_fallbackCountryCode))
      ).shop;
    }
  }
}
