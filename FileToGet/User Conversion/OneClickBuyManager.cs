using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Shopify.Unity;
using Shopify.Unity.GraphQL;

using UniTween.Core;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using Image = Shopify.Unity.Image;

namespace Marbotic.Game.OneClickBuy {

  using Analytics.Models;

  using Core.Serialization.Dictionaries.Models;

  using Framework.Components;
  using Framework.Holders.Models;
  using Framework.Models;

  public class OneClickBuyManager : MonoBehaviour {

    static readonly Dictionary<string, int> _imagesResolutions = new Dictionary<string, int> { { "small", 256 }, { "large", 1024 } };

    [Serializable] class ProductDictionary : SerializedDictionary<string, Product> {}

    [SerializeField] ShopProxy _shopProxy;
    [SerializeField] UserHolder _userHolder;

    [Space, SerializeField] GameObject _parentalLock;
    [Space, SerializeField] GameObject _confirmationPopup;
    [SerializeField] UniTweenSequence _sequenceClose;

    [Space, SerializeField] StringConfigurationProperty _popupEventProperty;

    [SerializeField] List<ProductInformation> _productsReference;
    [SerializeField] ProductDictionary _products;

    Shop _shop;
    ProductInformation _productReference;
    ShopProductPreview _productPreview;

    Action _parentalLockCallback;

    void Awake() {
      _shop = _shopProxy.GetShop(_userHolder.Value.Country);
      _productReference = ChooseProductToDisplay(_userHolder?.Value.Pieces.ToArray());
      ShopifyBuy.Init(_shop.accessToken, _shop.domain);
      var test = _productReference.GetId(_shop);

      var popupLock = _parentalLock.GetComponentInChildren<PopupLock>();
      popupLock.onClosed += HandleParentalLockClosed;
      SetupPopup(_productReference);
      _productPreview.buyNowButton?.onClick.AddListener(StartWebCheckoutProcess);
      _popupEventProperty.SetCurrentValue(_productPreview.eventName);

      // TODO : Create a custom query to reduce response size
      ShopifyBuy.Client().products(
        (products, error) => {
          if (error != null) { HandleQueryError(error); }
          else {  HandleProducts(products); }
        },
        _productReference.GetId(_shop)
      );
    }

    void OnDestroy() {
      var popupLock = _parentalLock.GetComponentInChildren<PopupLock>();
      popupLock.onClosed -= HandleParentalLockClosed;
      _productPreview.buyNowButton?.onClick.RemoveListener(StartWebCheckoutProcess);
    }

    void StartWebCheckoutProcess() => StartCheckoutProcess(CheckoutWithWebView);

    void StartCheckoutProcess(Action callback) {
      _parentalLockCallback = callback;
      _parentalLock.SetActive(true);
    }

    void HandleParentalLockClosed(bool success) {
      if (success) { _parentalLockCallback?.Invoke();}
      _parentalLockCallback = null;
    }

    void HandleQueryError(ShopifyError error) {
      Debug.Log("an error happend whild building the query");
      Debug.Log(error.Description);
    }

    void HandleProducts(List<Shopify.Unity.Product> products) {
      foreach (var product in products) { HandleProduct(product); }
    }

    void HandleProduct(Shopify.Unity.Product product) {
      var currencyCode = ((List<ProductVariant>)product.variants())[0].priceV2().currencyCode();
      var price = ((List<ProductVariant>)product.variants())[0].priceV2().amount();
      _productPreview.price = GetLocalizedPrice(price, currencyCode);
      StartCoroutine(GetImages(product));
    }

    // FIXME: Not scalable
    string GetLocalizedPrice(decimal price, CurrencyCode currencyCode) {
      switch (currencyCode.ToString()) {
        case "EUR": return string.Concat(price, " €");
        case "USD": return string.Concat("$ ", price);
        default: return string.Concat(currencyCode, " ", price);
      }
    }

    IEnumerator GetImages(Shopify.Unity.Product product) {
      var images = (List<Image>) product.images();
      var productImages = new List<Texture>();
      foreach(var image in images) {
        var request = UnityWebRequestTexture.GetTexture(image.transformedSrc("large"), false);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
          Debug.Log(request.error);
        } else {
          productImages.Add(DownloadHandlerTexture.GetContent(request));
        }
      }
      _productPreview.SetProductImages(productImages);
      _confirmationPopup.GetComponentInChildren<RawImage>().texture = productImages[0];
    }

    ProductInformation ChooseProductToDisplay(UserPiece[] allocatedPieces) {
      var hasLetters = false;
      var hasNumbers = false;
      foreach(var piece in allocatedPieces) {
        if (piece.Code == "Uppercase Letters" || piece.Code == "Lowercase Letters") {
          hasLetters = true;
        } else if (piece.Code == "Numbers") {
          hasNumbers = true;
        }
      }

      if (hasLetters && !hasNumbers) {
        return _productsReference.FirstOrDefault(s => s.product == _products["Smart Numbers"]);
      } else if (hasNumbers && !hasLetters) {
        return _productsReference.FirstOrDefault(s => s.product == _products["Lowercase Letters"]);
      }

      return _productsReference.FirstOrDefault(s => s.product == _products["Sensory Kit"]);
    }

    void SetupPopup(ProductInformation productReference) => _productPreview = Instantiate(productReference.productPreview, transform);

    void CheckoutWithWebView() {
      ShopifyBuy.Client().products(
        (products, error) => {
          if (error != null) { HandleQueryError(error); }
          else {
            var cart = ShopifyBuy.Client().Cart();
            var productVariants = (List<ProductVariant>) products[0].variants();
            ProductVariant productVariantToCheckout = productVariants[0];
            cart.LineItems.AddOrUpdate(productVariantToCheckout, 1);
            cart.CheckoutWithWebView(
              success: () => {
                Debug.Log("User finished purchase/checkout!");
                _confirmationPopup.SetActive(true);
                _sequenceClose.Play();
              },
              cancelled: () => {
                Debug.Log("User cancelled out of the web checkout.");
                cart.LineItems.Delete(productVariantToCheckout);
                _sequenceClose.Play();

              },
              failure: (e) => {
                Debug.Log("Something bad happened - Error: " + e);
                cart.LineItems.Delete(productVariantToCheckout);
                _sequenceClose.Play();
              }
            );
          }
        },
        _productReference.GetId(_shop)
      );
    }
  }
}
