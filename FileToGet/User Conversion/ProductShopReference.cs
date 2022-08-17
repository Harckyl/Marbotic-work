using System;

using UnityEngine;

namespace Marbotic.Game.OneClickBuy {

  using Core.Serialization.Dictionaries.Models;
  using Framework.Models;

  [Serializable]
  public struct ProductInformation {

    [SerializeField] Product _product;
    public Product product => _product;

    [SerializeField] ShopProductPreview _productPreview;
    public ShopProductPreview productPreview => _productPreview;

    [SerializeField] StringStringDictionary _ids;

    public string GetId(Shop shop) => _ids[shop.name];
  }
}
