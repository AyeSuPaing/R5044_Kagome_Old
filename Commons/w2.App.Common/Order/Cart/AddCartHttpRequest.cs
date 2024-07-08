/*
=========================================================================================================
  Module      : カート投入URL情報クラス(AddCartHttpRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using w2.Common.Util;
using System.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カート投入URL情報クラス
	/// </summary>
	public class AddCartHttpRequest
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AddCartHttpRequest()
		{
			// 何もしない
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="httpRequest">リクエスト情報</param>
		/// <param name="isPc">PCサイトか</param>
		public AddCartHttpRequest(HttpRequest httpRequest, bool isPc) :this()
		{
			// HACK：単体テストコード用に対応するためHttpRequestを引数としない処理を実装する

			this.IsPc = isPc;
			this.Request = httpRequest;
			this.CartAddUrlType = GetRequestParameter(this.Key_CartAddUrlType);

			// 必須キー{商品ID・バリエーションID}の枝番がペアで全て存在するか判定
			this.ErrorMessages = (CheckNecessaryKeyPairAll() == false) ? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ADD_CART_HTTPREQUEST) : "";

			// リクエスト情報をリストで取得
			CreateAddCartProductList();
		}

		/// <summary>
		/// リクエスト情報をリストで取得
		/// </summary>
		private void CreateAddCartProductList()
		{
			// 各パラメータを一旦1商品単位づつ取得
			// 商品単位でパラメタ指定が誤っていてもとりあえず作成
			Dictionary<string, AddCartProduct> addCartProducts = new Dictionary<string, AddCartProduct>();
			foreach (string suffix in GetRequestKeySuffixList(this.Key_ProductId))
			{
				// 1商品単位のリクエスト情報作成
				AddCartProduct addCartProduct = new AddCartProduct(
					CheckNecessaryKeyPair(suffix),
					Constants.CONST_DEFAULT_SHOP_ID,//((GetRequestParameter(this.Key_ShopId) != "") ? GetRequestParameter(this.Key_ShopId) : Constants.CONST_DEFAULT_SHOP_ID),
					(Constants.PRODUCT_SET_OPTION_ENABLED ? GetRequestParameter(this.Key_ProductSetId) : ""),
					GetRequestParameter(this.Key_ProductId + suffix),
					GetRequestParameter(this.Key_VariationId + suffix),
					GetProductCount(suffix),
					(Constants.FIXEDPURCHASE_OPTION_ENABLED ? GetRequestParameter(this.Key_FixedPurchase + suffix) : ""),
					(Constants.GIFTORDER_OPTION_ENABLED ? GetRequestParameter(this.Key_Gift + suffix) : ""),
					GetRequestAddCartType(suffix),
					GetProductOptionSelectedValueList(suffix),
					GetRequestParameter(this.Key_FixedPurchaseKbn + suffix),
					GetRequestParameter(this.Key_FixedPurchaseSetting + suffix),
                    Constants.SUBSCRIPTION_BOX_OPTION_ENABLED ? GetRequestParameter(this.Key_CartSubscriptionBoxCourseId + suffix) : string.Empty);

				addCartProducts.Add(suffix, addCartProduct);
			}

			this.AddCartProducts = addCartProducts;
		}

		/// <summary>
		/// 必須キー{商品ID・バリエーションID}の枝番がペアで全て存在するか判定
		/// </summary>
		/// <returns>TRUE:ペア FALSE:ペアでない</returns>
		/// <remarks>
		/// ※現状店舗IDはチェックしない
		/// ※投入URLタイプキーの存在有無はカートリスト側で行う
		/// ※キー重複の検知は不可
		/// </remarks>
		private bool CheckNecessaryKeyPairAll()
		{
			int pairCount = (GetRequestKeySuffixList(this.Key_ProductId).Intersect(GetRequestKeySuffixList(this.Key_VariationId)).Count());
			return ((GetRequestKeySuffixList(this.Key_ProductId).Count() == pairCount) && (GetRequestKeySuffixList(this.Key_VariationId).Count() == pairCount));
		}

		/// <summary>
		/// 商品単位で必須Keyを指定しているか判定
		/// </summary>
		/// <param name="suffix">キーの接尾辞</param>
		/// <returns>True：ある False：なし</returns>
		private bool CheckNecessaryKeyPair(string suffix)
		{
			return (GetAllKeys().Contains(this.Key_ProductId + suffix) && (GetAllKeys().Contains(this.Key_VariationId + suffix)));
		}

		/// <summary>
		/// 商品単位で必須Valueを指定しているか判定
		/// </summary>
		/// <param name="suffix">キーの接尾辞</param>
		/// <returns>True：ある False：なし</returns>
		private bool CheckAccurateProductUnit(string suffix)
		{
			return ((GetRequestParameter(this.Key_ProductId + suffix) != "") && (GetRequestParameter(this.Key_VariationId + suffix) != ""));
		}

		/// <summary>
		/// 特定のリクエストキーの接尾辞をリスト取得
		/// </summary>
		/// <param name="prefix">キーの接頭辞</param>
		/// <returns>商品単位の数</returns>
		/// <remarks>
		/// 商品ID/バリエーションIDキーの末尾数字列をリストで保持し、後処理で利用
		/// </remarks>
		private List<string> GetRequestKeySuffixList(string prefix)
		{
			List<string> suffixList = new List<string>();
			foreach (string key in GetAllKeys())
			{
				Match match = Regex.Match(StringUtility.ToEmpty(key), "^" + prefix + "[0-9]*$", RegexOptions.IgnoreCase);
				string suffix = match.Value.Replace(prefix, "");
				if (match.Success && (suffixList.Contains(suffix) == false)) suffixList.Add(suffix);
			}
			return suffixList;
		}

		/// <summary>
		/// POST/GETのリクエストパラメータの全キー取得
		/// </summary>
		/// <returns>全リクエストキー</returns>
		/// <remarks>重複分は除かれる</remarks>
		private string[] GetAllKeys()
		{
			return (this.Request.RequestType.ToUpper() == "GET") ? this.Request.QueryString.AllKeys : this.Request.Form.AllKeys;
		}

		/// <summary>
		/// POST/GETのリクエストパラメータのキー値取得
		/// </summary>
		/// <param name="key">リクエストキー</param>
		/// <returns>キー値</returns>
		private string GetRequestParameter(string key)
		{
			return (this.Request.RequestType.ToUpper() == "GET") ? StringUtility.ToEmpty(this.Request.QueryString[key]) : StringUtility.ToEmpty(this.Request.Form[key]);
		}

		/// <summary>
		/// カートに投入・加算する商品数量を取得
		/// </summary>
		/// <param name="suffix">キーの接尾辞</param>
		/// <returns>商品数量</returns>
		private int GetProductCount(string suffix)
		{
			int productCount;
			if (int.TryParse(GetRequestParameter(this.Key_ProductCount + suffix), out productCount) == false) productCount = 1;
			return productCount;
		}

		/// <summary>
		/// カート追加区分の取得
		/// </summary>
		/// <param name="suffix">キーの接尾辞</param>
		/// <returns>カート追加区分</returns>
		private Constants.AddCartKbn GetRequestAddCartType(string suffix)
		{
			Constants.AddCartKbn addCartKbn = Constants.AddCartKbn.Normal;
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED && (GetRequestParameter(this.Key_FixedPurchase + suffix) == "1"))
			{
				addCartKbn = Constants.AddCartKbn.FixedPurchase;
			}
			// モバイルはギフト未対応
			else if (this.IsPc && Constants.GIFTORDER_OPTION_ENABLED && (GetRequestParameter(this.Key_Gift + suffix) == "1"))
			{
				addCartKbn = Constants.AddCartKbn.GiftOrder;
			}
			else if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (GetRequestParameter(this.Key_SubscriptionBox + suffix) == "1"))
			{
				addCartKbn = Constants.AddCartKbn.SubscriptionBox;
			}

			// カート投入URLタイプが商品セットなら「通常」に上書き
			addCartKbn = (this.CartAddUrlType == Constants.KBN_REQUEST_CART_ACTION_ADD_SET) ? Constants.AddCartKbn.Normal : addCartKbn;

			return addCartKbn;
		}

		/// <summary>
		/// 商品付帯情報の取得
		/// </summary>
		/// <param name="suffix">キーの接尾辞</param>
		/// <returns>商品付帯情報</returns>
		/// <remarks>
		/// 付帯情報には日本語も投入される可能性もある為、送り側がUTF8エンコードした文字列で付与する必要があります。
		/// 連番なし:1商品目のpov_1 pov_2 pov_3 pov_4 pov_5
		/// 連番あり:1商品目のpov1_1 pov1_2・・・　2商品目のpov2_1 pov2_2 pov2_3・・・
		/// </remarks>
		private List<string> GetProductOptionSelectedValueList(string suffix)
		{
			suffix = (suffix != "") ? (suffix + "_") : "_";
			List<string> productOptionSelectedValueList = new List<string>();
			for (int index = 1; index <= Constants.PRODUCTOPTIONVALUES_MAX_COUNT; index++) productOptionSelectedValueList.Add(GetRequestParameter(Constants.REQUEST_KEY_PRODUCT_OPTION_VALUE + suffix + index).Trim());

			return productOptionSelectedValueList;
		}

		/// <summary>
		/// 単体テストコード用
		/// </summary>
		/// <returns></returns>
		private string UnitTest()
		{
			return "";
		}

		#region プロパティ
		/// <summary>PCサイトか？</summary>
		public bool IsPc { get; private set; }
		/// <summary>カート投入URLタイプ</summary>
		public string CartAddUrlType { get; private set; }
		/// <summary>リクエスト情報</summary>
		public HttpRequest Request { get; private set; }
		/// <summary>エラーメッセージ ※商品情報が正しい有無ではなく、リクエスト情報として正しいかどうかのメッセージ</summary>
		public string ErrorMessages { get; private set; }
		/// <summary>カート投入URLリクエストの商品情報リスト</summary>
		public Dictionary<string, AddCartProduct> AddCartProducts { get; private set; }
		/// <summary>Is Add Set Item</summary>
		public bool IsAddSetItem { get { return (this.CartAddUrlType == Constants.KBN_REQUEST_CART_ACTION_ADD_SET); } }

		/// <summary>店舗ID</summary>
		private string Key_ShopId { get { return Constants.REQUEST_KEY_SHOP_ID; } }
		/// <summary>カート投入アクション区分</summary>
		private string Key_CartAddUrlType { get { return this.IsPc ? Constants.REQUEST_KEY_CART_ACTION : Constants.REQUEST_KEY_PRODUCT_ADD; } }
		/// <summary>商品セットID</summary>
		private string Key_ProductSetId { get { return this.IsPc ? Constants.REQUEST_KEY_PRODUCTSET_ID : Constants.REQUEST_KEY_MFRONT_PRODUCTSET_ID; } }
		/// <summary>商品ID</summary>
		private string Key_ProductId { get { return this.IsPc ? Constants.REQUEST_KEY_PRODUCT_ID : Constants.REQUEST_KEY_MFRONT_PRODUCT_ID; } }
		/// <summary>商品バリエーションID</summary>
		private string Key_VariationId { get { return this.IsPc ? Constants.REQUEST_KEY_VARIATION_ID : Constants.REQUEST_KEY_MFRONT_VARIATION_ID; } }
		/// <summary>数量</summary>
		private string Key_ProductCount { get { return this.IsPc ? Constants.REQUEST_KEY_PRODUCT_COUNT : Constants.REQUEST_KEY_MFRONT_PRODUCT_COUNT; } }
		/// <summary>定期購入フラグ</summary>
		private string Key_FixedPurchase { get { return this.IsPc ? Constants.REQUEST_KEY_FIXED_PURCHASE : Constants.REQUEST_KEY_MFRONT_FIXED_PURCHASE; } }
		/// <summary>ギフト購入フラグ ※モバイルは非対応</summary>
		private string Key_Gift { get { return this.IsPc ? Constants.REQUEST_KEY_GIFT_ORDER : ""; } }
		/// <summary>定期購入区分</summary>
		private string Key_FixedPurchaseKbn { get { return this.IsPc ? Constants.REQUEST_KEY_FIXED_PURCHASE_KBN : string.Empty; } }
		/// <summary>定期購入設定</summary>
		private string Key_FixedPurchaseSetting { get { return this.IsPc ? Constants.REQUEST_KEY_FIXED_PURCHASE_SETTING : string.Empty; } }
		/// <summary>頒布会購入フラグ</summary>
		private string Key_SubscriptionBox { get { return Constants.REQUEST_KEY_SUBSCRIPTION_BOX; } }
		/// <summary>頒布会コースID</summary>
		private string Key_CartSubscriptionBoxCourseId { get { return Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID; } }
		#endregion

		#region 内部クラス
		/// <summary>
		/// カート投入URLリクエストの商品情報
		/// </summary>
		public class AddCartProduct
		{
			/// <summary>
			/// デフォルトコンストラクタ
			/// </summary>
			private AddCartProduct()
			{
				// 何もしない
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="accurate"></param>
			/// <param name="shopId">店舗ID</param>
			/// <param name="productSetId">商品セットID</param>
			/// <param name="productId">商品ID</param>
			/// <param name="variationId">商品バリエーションID</param>
			/// <param name="productCount">数量</param>
			/// <param name="fixedPurchase">定期購入フラグ</param>
			/// <param name="gift">ギフト購入フラグ</param>
			/// <param name="addCartType">カート追加区分</param>
			/// <param name="productOptionValue">商品付帯情報</param>
			/// <param name="fixedPurchaseKbn">定期購入区分</param>
			/// <param name="fixedPurchaseSetting">定期購入設定</param>
			/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
			public AddCartProduct(
				bool accurate,
				string shopId,
				string productSetId,
				string productId,
				string variationId,
				int productCount,
				string fixedPurchase,
				string gift,
				Constants.AddCartKbn addCartType,
				List<string> productOptionValue,
				string fixedPurchaseKbn,
				string fixedPurchaseSetting,
				string subscriptionBoxCourseId
			)
				: this()
			{
				this.Accurate = accurate;
				this.ShopId = shopId;
				this.ProductSetId = productSetId;
				this.ProductId = productId;
				this.VariationId = variationId;
				this.ProductCount = productCount;
				this.FixedPurchase = fixedPurchase;
				this.Gift = gift;
				this.ProductOptionValue = productOptionValue;
				this.AddCartType = addCartType;
				this.FixedPurchaseKbn = fixedPurchaseKbn;
				this.FixedPurchaseSetting = fixedPurchaseSetting;
				this.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			}

			#region プロパティ
			public bool Accurate { get; set; }
			/// <summary>店舗ID</summary>
			public string ShopId { get; private set; }
			/// <summary>商品セットID</summary>
			public string ProductSetId { get; private set; }
			/// <summary>商品ID</summary>
			public string ProductId { get; private set; }
			/// <summary>商品バリエーションID</summary>
			public string VariationId { get; private set; }
			/// <summary>数量</summary>
			public int ProductCount { get; private set; }
			/// <summary>定期購入フラグ</summary>
			public string FixedPurchase { get; private set; }
			/// <summary>ギフト購入フラグ</summary>
			public string Gift { get; private set; }
			/// <summary>商品付帯情報</summary>
			public List<string> ProductOptionValue { get; private set; }
			/// <summary>カート追加区分</summary>
			public Constants.AddCartKbn AddCartType { get; private set; }
			/// <summary>Is Fixed Purchase</summary>
			public bool IsFixedPurchase
			{
				get { return (this.AddCartType == Constants.AddCartKbn.FixedPurchase); }
			}
			/// <summary>定期購入区分</summary>
			public string FixedPurchaseKbn { get; private set; }
			/// <summary>定期購入設定</summary>
			public string FixedPurchaseSetting { get; private set; }
			/// <summary>頒布会コースID</summary>
			public string SubscriptionBoxCourseId { get; private set; }
			#endregion
		}

		#endregion
	}
}
