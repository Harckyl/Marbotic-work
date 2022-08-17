using System;
using System.Linq;
using System.Collections.Generic;

using DanielLochner.Assets.SimpleScrollSnap;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Marbotic.Game.OneClickBuy {

  using Core.Holders.Models;

  public class ShopProductPreview : MonoBehaviour {

    [Space, SerializeField] SimpleScrollSnap _scrollSnap;
    [SerializeField] GameObject _productImagePrefab;
    [SerializeField] GameObject _pagerPrefab;
    [SerializeField] GameObject _loading;

    [Space, SerializeField] TextMeshProUGUI _price;
    [SerializeField] Button _buyNowButton;
    public Button buyNowButton => _buyNowButton;

    [SerializeField] StringConfigurationHolder _eventName;
    public StringConfigurationHolder eventName => _eventName;

    public string price { get => _price.text; set => _price.text = value; }

    public void SetProductImages(IEnumerable<Texture> images) {
      foreach (var image in images) {
        var instantiatedObject = Instantiate(_productImagePrefab, _scrollSnap.Content);
        instantiatedObject.GetComponentInChildren<RawImage>().texture = image;
        Instantiate(_pagerPrefab, _scrollSnap.pagination.transform);
      }
      _scrollSnap.gameObject.SetActive(true);
      _loading.SetActive(false);
    }
  }
}
