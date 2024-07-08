/*
=========================================================================================================
  Module      : Criteoタグ出力コントローラ処理(Criteo.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using w2.App.Common.Order;

/// <summary>Criteoタグ出力コントローラ の概要の説明です</summary>
public partial class Form_Common_Criteo : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// Criteo無効なら実行しない
		if (Constants.CRITEO_OPTION_ENABLED == false) return;

		// タグ共通情報を生成
		CreateCommonInfo();

		//------------------------------------------------------
		// トップ（ブランドトップは対象外）
		//------------------------------------------------------
		if ((Request.Url.AbsolutePath.ToLower() == (Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT).ToLower())
			|| (Request.Url.AbsolutePath.ToLower() == (Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_DEFAULT).ToLower()))
		{
			MainTag = CreateTopPageTrackerInfo();
		}
		//------------------------------------------------------
		// 商品詳細（商品IDを連携）
		//------------------------------------------------------
		else if (Request.Url.AbsolutePath.ToLower().Contains((Constants.PAGE_FRONT_PRODUCT_DETAIL).ToLower()))
		{
			MainTag = CreateProductDetailTrackerInfo(string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]) ? "" : Request[Constants.REQUEST_KEY_PRODUCT_ID]);
		}
		//------------------------------------------------------
		// 商品一覧（一覧の上位3商品を連携）
		//------------------------------------------------------
		else if ((Request.Url.AbsolutePath.ToLower().Contains((Constants.PAGE_FRONT_PRODUCT_LIST).ToLower()))
			&& (Datas != null))
		{
			MainTag = CreateProductListTrackerInfo((DataView)Datas);
		}
		//------------------------------------------------------
		// カート一覧（カート内商品の｛商品ID、商品単価、個数｝を連携）
		//------------------------------------------------------
		else if ((Request.Url.AbsolutePath.ToLower().Contains((Constants.PAGE_FRONT_CART_LIST).ToLower()))
			&& (Datas != null))
		{
			MainTag = CreateCartListTrackerInfo((CartObjectList)Datas);
		}
		//------------------------------------------------------
		// 注文完了（注文の注文ID、注文商品の｛商品ID、商品単価、個数｝を連携）
		//------------------------------------------------------
		else if ((Request.Url.AbsolutePath.ToLower().Contains((Constants.PAGE_FRONT_ORDER_COMPLETE).ToLower()))
			&& (Datas != null))
		{
			MainTag = CreateConversionTrackerInfo((List<DataView>)Datas);
		}
	}

	/// <summary>
	/// Criteoタグ用共通情報生成
	/// </summary>
	/// <remarks>加盟店アカウント、サイトタイプ、ハッシュ化済みメールアドレス　を設定</remarks>
	protected void CreateCommonInfo()
	{
		this.Account = Constants.CRITEO_ACCOUNT_ID;
		this.SiteType = this.IsSmartPhone ? "m" : "d";
		this.HashedEmail = SetHashedEmail();
		this.HashedEmailSha256 = SetHashedEmailSha256();
	}

	/// <summary>
	/// Criteoタグ用ハッシュ化済みメールアドレスの設定"MD5"
	/// </summary>
	/// <returns>ハッシュ化済みメールアドレス</returns>
	/// <remarks>クロスデバイスが有効でなければ、ブランクを設定</remarks>
	protected string SetHashedEmail()
	{
#pragma warning disable 618
		string userMail = "";
		if ((this.IsLoggedIn) && (Constants.CRITEO_CROSS_DEVICE_ENABLED) && (string.IsNullOrEmpty(this.LoginUserMail) == false))
		{
			userMail = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.LoginUserMail, "MD5");
		}
		else if ((this.IsLoggedIn) && (Constants.CRITEO_CROSS_DEVICE_ENABLED) && (string.IsNullOrEmpty(this.LoginUserMail2) == false))
		{
			userMail = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.LoginUserMail2, "MD5");
		}
#pragma warning restore 618

		return userMail;
	}

	/// <summary>
	/// Criteoタグ用ハッシュ化済みメールアドレスの設定SHA256
	/// </summary>
	/// <returns>ハッシュ化済みメールアドレス</returns>
	/// <remarks>クロスデバイスが有効でなければ、ブランクを設定</remarks>
	protected string SetHashedEmailSha256()
	{
		var userMail = new byte[32];
		if ((this.IsLoggedIn) && (Constants.CRITEO_CROSS_DEVICE_ENABLED) && (string.IsNullOrEmpty(this.LoginUserMail) == false))
		{
			userMail = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(this.LoginUserMail));
		}
		else if ((this.IsLoggedIn) && (Constants.CRITEO_CROSS_DEVICE_ENABLED) && (string.IsNullOrEmpty(this.LoginUserMail2) == false))
		{
			userMail = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(this.LoginUserMail2));
		}

		var result = BitConverter.ToString(userMail).Replace("-", string.Empty).ToLower();
		return result;
	}

	/// <summary>
	/// トップページトラッカー タグ出力
	/// </summary>
	/// <returns>トップページトラッカー タグ</returns>
	protected string CreateTopPageTrackerInfo()
	{
		/* トップページトラッカーイメージ */
		/*
		<script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
		<script type="text/javascript">
		window.criteo_q = window.criteo_q || [];
		window.criteo_q.push(
			{ event: "setAccount", account: 加盟店アカウント },
			{ event: "setSiteType", type: "m"(モバイル) "t"(タブレット) "d"(PC) },
			{ event: "setHashedEmail", email: "ハッシュ化されたemailアドレス(MD5)" },
			{ event: "viewHome" }
		);
		</script>
		*/
		var mainString = new StringBuilder();
		mainString.Append("{ event: \"viewHome\" }");

		return mainString.ToString();
	}

	/// <summary>
	/// 商品詳細結果トラッカー タグ出力
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品詳細結果トラッカー タグ</returns>
	protected string CreateProductDetailTrackerInfo(string productId)
	{
		/* 商品詳細結果トラッカーイメージ */
		/*
		<script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
		<script type="text/javascript">
		window.criteo_q = window.criteo_q || [];
		window.criteo_q.push(
			{ event: "setAccount", account: 加盟店アカウント },
			{ event: "setSiteType", type: "m"(モバイル) "t"(タブレット) "d"(PC) },
			{ event: "setHashedEmail", email: "ハッシュ化されたemailアドレス(MD5)" },
			{ event: "viewItem", item: "商品ID" }
		);
		</script>
		*/
		var mainString = new StringBuilder();
		mainString.Append("{ event: \"viewItem\", item: \"").Append(WebSanitizer.HtmlEncode(productId)).Append("\" }");

		return mainString.ToString();
	}

	/// <summary>
	/// 一覧ページトラッカー タグ出力
	/// </summary>
	/// <param name="productList">商品情報リスト</param>
	/// <returns>一覧ページトラッカー タグ</returns>
	protected string CreateProductListTrackerInfo(DataView productList)
	{
		/* 一覧ページトラッカーイメージ */
		/*
		<script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
		<script type="text/javascript">
		window.criteo_q = window.criteo_q || [];
		window.criteo_q.push(
			{ event: "setAccount", account: 加盟店アカウント },
			{ event: "setSiteType", type: "m"(モバイル) "t"(タブレット) "d"(PC) },
			{ event: "setHashedEmail", email: "ハッシュ化されたemailアドレス(MD5)" },
			{ event: "viewList", item: ["商品ID1", "商品ID2", "商品ID3"] }
		);
		</script>
		*/
		var mainString = new StringBuilder();
		var itemString = new StringBuilder();
		int top = 0;

		mainString.Append("{ event: \"viewList\", item: [");
		foreach (DataRowView item in productList)
		{
			top++;
			if (itemString.Length != 0) itemString.Append(", ");
			itemString.Append("\"").Append(WebSanitizer.HtmlEncode(item[Constants.FIELD_PRODUCT_PRODUCT_ID])).Append("\"");

			// 上位3商品のみ
			if (top == 3) break;
		}
		mainString.Append(itemString.ToString()).Append("] }");

		return mainString.ToString();
	}

	/// <summary>
	/// 買い物カゴ・申し込み開始トラッカー タグ出力
	/// </summary>
	/// <param name="cartList">カート情報リスト</param>
	/// <returns>買い物カゴ・申し込み開始トラッカー タグ</returns>
	protected string CreateCartListTrackerInfo(CartObjectList cartList)
	{
		/* 買い物カゴ・申し込み開始トラッカーイメージ */
		/*
		<script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
		<script type="text/javascript">
		window.criteo_q = window.criteo_q || [];
		window.criteo_q.push(
			{ event: "setAccount", account: 加盟店アカウント },
			{ event: "setSiteType", type: "m"(モバイル) "t"(タブレット) "d"(PC) },
			{ event: "setHashedEmail", email: "ハッシュ化されたemailアドレス(MD5)" },
			{ event: "viewBasket", item: [
				{ id: "商品ID1", price: 商品ID1の商品単価, quantity: 商品ID1の個数 },
				{ id: "商品ID2", price: 商品ID2の商品単価, quantity: 商品ID2の個数 } ]} ※ カートに追加されたすべての商品情報を取得
		);
		</script>
		*/
		var mainString = new StringBuilder();
		var itemString = new StringBuilder();

		mainString.Append("{ event: \"viewBasket\", item: [").Append("\r\n");

		// カート内の品番単位で商品情報をまとめる
		List<Hashtable> items = new List<Hashtable>();
		foreach (CartObject cart in cartList)
		{
			foreach (CartProduct product in cart.Items)
			{
				// 同一商品が存在する？
				Hashtable tempItem = items.Find(ti => ((product.ShopId == (string)ti[Constants.FIELD_PRODUCTVARIATION_SHOP_ID])
					&& (product.ProductId == (string)ti[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])));
				if (tempItem != null)
				{
					// 数量を加算
					tempItem["count_single"] = (int)tempItem["count_single"] + product.CountSingle;
					continue;
				}

				// 追加
				Hashtable item = new Hashtable();
				item.Add(Constants.FIELD_PRODUCTVARIATION_SHOP_ID, product.ShopId);
				item.Add(Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, product.ProductId);
				item.Add(Constants.FIELD_PRODUCTVARIATION_PRICE, product.PriceOrg);	// 値引き前金額を設定
				item.Add("count_single", product.CountSingle);
				items.Add(item);
			}
		}

		// 品番単位でまとめた商品情報を出力
		foreach (Hashtable item in items)
		{
			if (itemString.Length != 0) itemString.Append(",").Append("\r\n");
			itemString.Append(
				TagCriteoItemList(WebSanitizer.HtmlEncode(item[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]),
								  WebSanitizer.HtmlEncode(item[Constants.FIELD_PRODUCTVARIATION_PRICE]),
								  WebSanitizer.HtmlEncode(item["count_single"])));
		}
		mainString.Append(itemString.ToString()).Append(" ]}");

		return mainString.ToString();
	}

	/// <summary>
	/// コンバージョントラッカー タグ出力
	/// </summary>
	/// <param name="orderList">注文情報リスト</param>
	/// <returns>コンバージョントラッカー タグ</returns>
	protected string CreateConversionTrackerInfo(List<DataView> orderList)
	{
		/* コンバージョントラッカーイメージ */
		/*
		<script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
		<script type="text/javascript">
		window.criteo_q = window.criteo_q || [];
		window.criteo_q.push(
			{ event: "setAccount", account: 加盟店アカウント },
			{ event: "setSiteType", type: "m"(モバイル) "t"(タブレット) "d"(PC) },
			{ event: "setHashedEmail", email: "ハッシュ化されたemailアドレス(MD5)" },
			{ event: "trackTransaction", id: "トランザクションID", item: [
				{ id: "商品ID1", price: 商品ID1の商品単価, quantity: 商品ID1の個数 },
				{ id: "商品ID2", price: 商品ID2の商品単価, quantity: 商品ID2の個数 } ]} ※ 購入されたすべての商品情報を取得
		);
		</script>
		*/
		var mainString = new StringBuilder();
		var itemString = new StringBuilder();

		mainString.Append("{ event: \"trackTransaction\", id: \"").Append(WebSanitizer.HtmlEncode(orderList[0][0][Constants.FIELD_ORDER_ORDER_ID])).Append("\", item: [").Append("\r\n");

		// 注文内の品番単位で商品情報をまとめる
		List<Hashtable> items = new List<Hashtable>();
		foreach (DataView order in orderList)
		{
			foreach (DataRowView orderItem in order)
			{
				// 同一商品が存在する？
				Hashtable tempItem = items.Find(ti => (((string)orderItem[Constants.FIELD_ORDERITEM_SHOP_ID] == (string)ti[Constants.FIELD_ORDERITEM_SHOP_ID])
					&& ((string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID] == (string)ti[Constants.FIELD_ORDERITEM_PRODUCT_ID])));
				if (tempItem != null)
				{
					// 数量を加算
					tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = (int)tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] + (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
					continue;
				}

				// 追加
				Hashtable item = new Hashtable();
				item.Add(Constants.FIELD_ORDERITEM_SHOP_ID, (string)orderItem[Constants.FIELD_ORDERITEM_SHOP_ID]);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, (string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, (decimal)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG]);	// 値引き前金額を設定
				item.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]);
				items.Add(item);
			}
		}

		// 品番単位でまとめた商品情報を出力
		foreach (Hashtable item in items)
		{
			if (itemString.Length != 0) itemString.Append(",").Append("\r\n");
			itemString.Append(
				TagCriteoItemList(WebSanitizer.HtmlEncode(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
								  WebSanitizer.HtmlEncode(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString()),
								  WebSanitizer.HtmlEncode(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY])));
		}
		mainString.Append(itemString.ToString()).Append(" ]}");

		return mainString.ToString();
	}

	/// <summary>
	/// 商品リスト情報出力
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="price">商品単価</param>
	/// <param name="quantity">個数</param>
	/// <returns>商品リスト情報</returns>
	public string TagCriteoItemList(string productId, string price, string quantity)
	{
		var itemString = new StringBuilder();
		itemString.Append("		{ id: \"").Append(productId).Append("\", ");
		itemString.Append("price: ").Append(price).Append(", ");
		itemString.Append("quantity: ").Append(quantity).Append(" }");

		return itemString.ToString();
	}

	/// <summary>
	/// ■商品一覧（ProductList.aspx）⇒ 商品一覧情報（DataView）
	/// ■カート一覧（CartList.aspx））⇒ カート情報（CartObjectList）
	/// ■注文完了（OrderComplete.aspx））⇒ 注文情報（List{DataView}）
	/// </summary>
	/// <remarks>外部から設定可能</remarks>
	public object Datas { get; set; }
	/// <summary>加盟店アカウント</summary>
	public string Account { get; set; }
	/// <summary>サイトタイプ（"m"(モバイル) "t"(タブレット) "d"(PC) ）</summary>
	[Obsolete("criteoが提供するサイトタイプ取得メソッドを使用することが推奨。\n詳細については、次を参照 https://help.criteo.com/kb/guide/en/switching-to-the-dynamic-loader-for-criteo-onetag-2khAnEokH0/Steps/775824,853906,853907,853909,853908")]
	public string SiteType { get; set; }
	/// <summary>ハッシュ化されたemailアドレス(MD5)</summary>
	public string HashedEmail { get; set; }
	/// <summary>ハッシュ化されたemailアドレス(SHA256)</summary>
	public string HashedEmailSha256 { get; set; }
	/// <summary>タグメイン部分</summary>
	public string MainTag { get; set; }
}