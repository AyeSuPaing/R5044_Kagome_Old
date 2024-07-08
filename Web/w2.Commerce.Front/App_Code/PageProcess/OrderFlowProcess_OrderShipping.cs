/*
=========================================================================================================
  Module      : 注文フロー（注文配送先）プロセス(OrderFlowProcess_OrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using jp.veritrans.tercerog.mdk.dto;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Auth.RakutenIDConnect.Helper;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Line.Util;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Cart;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.OrderExtend;
using w2.App.Common.RealShop;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.CountryLocation;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Holiday.Helper;
using w2.Domain.RealShop;
using w2.Domain.ShopShipping;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserBusinessOwner;
using w2.Domain.UserShipping;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
[System.Runtime.InteropServices.GuidAttribute("63A9EFAE-F63B-4983-83D6-E1BE56B0BBB3")]
public partial class OrderFlowProcess
{
	#region 配送情報入力画面系処理

	/// <summary> Form内のCustomValidatorのリスト </summary>
	private List<CustomValidator> m_customValidatorList;

	/// <summary>
	/// 注文者情報作成
	/// </summary>
	public void CreateOrderOwner()
	{
		//------------------------------------------------------
		// 注文者情報作成
		//------------------------------------------------------
		if (this.CartList.Owner == null)
		{
			if (this.IsLoggedIn)
			{
				// ユーザー情報取得
				var user = new UserService().Get(this.LoginUserId);

				var addrCountryIsoCode = string.Empty;
				var countryName = string.Empty;
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					if ((string.IsNullOrEmpty(user.AddrCountryIsoCode) == false) && (string.IsNullOrEmpty(user.AddrCountryName) == false))
					{
						addrCountryIsoCode = user.AddrCountryIsoCode;
						countryName = user.AddrCountryName;
					}
					else
					{
						// グローバルOP：OFF時に登録したユーザには住所ISOコード未設定のため、IPから判定した国名を表示する
						addrCountryIsoCode = RegionManager.GetInstance().Region.CountryIsoCode;
						countryName = GlobalAddressUtil.GetCountryName(addrCountryIsoCode);
					}
				}

				// 注文者情報セット
				this.CartList.SetOwner(new CartOwner(
					user.UserKbn,
					user.Name,
					user.Name1,
					user.Name2,
					user.NameKana,
					user.NameKana1,
					user.NameKana2,
					user.MailAddr,
					user.MailAddr2,
					user.Zip,
					user.Zip1,
					user.Zip2,
					user.Addr1,
					user.Addr2,
					user.Addr3,
					user.Addr4,
					user.Addr5,
					addrCountryIsoCode,
					countryName,
					user.CompanyName,
					user.CompanyPostName,
					user.Tel1,
					user.Tel1_1,
					user.Tel1_2,
					user.Tel1_3,
					user.Tel2,
					user.Tel2_1,
					user.Tel2_2,
					user.Tel2_3,
					user.MailFlg == Constants.FLG_USER_MAILFLG_OK,
					user.Sex,
					user.Birth,
					RegionManager.GetInstance().Region.CountryIsoCode,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId,
					RegionManager.GetInstance().Region.CurrencyCode,
					RegionManager.GetInstance().Region.CurrencyLocaleId));
			}
			else
			{
				var ownerCountryIsoCode = string.Empty;
				var countryName = string.Empty;
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					ownerCountryIsoCode = RegionManager.GetInstance().Region.CountryIsoCode;
					countryName = GlobalAddressUtil.GetCountryName(ownerCountryIsoCode);
				}
				var ownerTmp = new CartOwner
				{
					AddrCountryIsoCode = ownerCountryIsoCode,
					AddrCountryName = countryName,
					MailFlg = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_flg.default@@") == Constants.FLG_USER_MAILFLG_OK),
					Sex = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.sex.default@@"),
					Birth = DateTimeUtility.GetDefaultSettingBirthday(),
					AccessCountryIsoCode = RegionManager.GetInstance().Region.CountryIsoCode,
					DispLanguageCode = RegionManager.GetInstance().Region.LanguageCode,
					DispLanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
					DispCurrencyCode = RegionManager.GetInstance().Region.CurrencyCode,
					DispCurrencyLocaleId = RegionManager.GetInstance().Region.CurrencyLocaleId,
				};

				// Amazonログインの場合の処理
				if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED)
				{
					var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
					ownerTmp.Name = amazonModel.Name;
					ownerTmp.Name1 = (string.IsNullOrEmpty(amazonModel.GetName1()))
						? amazonModel.Name
						: amazonModel.GetName1();
					ownerTmp.Name2 = amazonModel.GetName2();
					ownerTmp.MailAddr = amazonModel.Email;
					ownerTmp.MailAddr2 = amazonModel.Email;
					ownerTmp.AccessCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
					ownerTmp.AddrCountryName = "Japan";
				}
				// PayPalログインの場合の処理
				else if (SessionManager.PayPalLoginResult != null)
				{
					ownerTmp.Name = SessionManager.PayPalLoginResult.AddressInfo.Name;
					ownerTmp.Name1 = SessionManager.PayPalLoginResult.AddressInfo.Name1;
					ownerTmp.Name2 = SessionManager.PayPalLoginResult.AddressInfo.Name2;
					ownerTmp.MailAddr = SessionManager.PayPalLoginResult.AddressInfo.MailAddr;
					ownerTmp.AddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
						? SessionManager.PayPalLoginResult.AddressInfo.ContryIsoCode
						: "";
					ownerTmp.AddrCountryName = GlobalAddressUtil.GetCountryName(SessionManager.PayPalLoginResult.AddressInfo.ContryIsoCode);
					ownerTmp.Zip1 = SessionManager.PayPalLoginResult.AddressInfo.Zip1;
					ownerTmp.Zip2 = SessionManager.PayPalLoginResult.AddressInfo.Zip2;
					ownerTmp.Addr1 = SessionManager.PayPalLoginResult.AddressInfo.Addr1;
					ownerTmp.Addr2 = SessionManager.PayPalLoginResult.AddressInfo.Addr2;
					ownerTmp.Addr3 = SessionManager.PayPalLoginResult.AddressInfo.Addr3;
					ownerTmp.Addr4 = SessionManager.PayPalLoginResult.AddressInfo.Addr4;
					ownerTmp.Addr5 = SessionManager.PayPalLoginResult.AddressInfo.Addr5;
					ownerTmp.Tel1_1 = SessionManager.PayPalLoginResult.AddressInfo.Tel_1;
					ownerTmp.Tel1_2 = SessionManager.PayPalLoginResult.AddressInfo.Tel_2;
					ownerTmp.Tel1_3 = SessionManager.PayPalLoginResult.AddressInfo.Tel_3;
				}
				// 楽天ID Connectログインの場合の処理
				else if (SessionManager.RakutenIdConnectActionInfo != null)
				{
					var user = new RakutenIDConnectUser(SessionManager.RakutenIdConnectActionInfo);
					ownerTmp.Name1 = user.FamilyName;
					ownerTmp.Name2 = user.GivenName;
					ownerTmp.NameKana1 = user.FamilyNameKana;
					ownerTmp.NameKana2 = user.GivenNameKana;
					ownerTmp.Birth = user.Birth;
					ownerTmp.Sex = user.Gender;
					ownerTmp.MailAddr = user.Email;
					ownerTmp.AddrCountryName = user.Country;
					ownerTmp.Zip1 = user.PostalCode1;
					ownerTmp.Zip2 = user.PostalCode2;
					ownerTmp.Addr1 = user.Address1;
					ownerTmp.Addr2 = user.Address2;
					ownerTmp.Addr3 = user.Address3;
					ownerTmp.Tel1_1 = user.PhoneNumber1;
					ownerTmp.Tel1_2 = user.PhoneNumber2;
					ownerTmp.Tel1_3 = user.PhoneNumber3;
				}

				//------------------------------------------------------
				// ディフォルト設定
				//------------------------------------------------------
				this.CartList.SetOwner(ownerTmp);
			}
		}
	}

	/// <summary>
	/// 配送先情報作成
	/// </summary>
	public void CreateOrderShipping()
	{
		foreach (CartObject cartObject in this.CartList)
		{
			if ((cartObject.Shippings.Count == 0)
				|| (cartObject.Shippings[0].ShippingAddrKbn == null))
			{
				CartShipping cartShipping = new CartShipping(cartObject);
				if (cartObject.IsGift)
				{
					cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
				}
				else
				{
					if (this.CartList.Owner != null) cartShipping.UpdateShippingAddr(this.CartList.Owner, false);
				}

				if (cartObject.Shippings.Count == 0)
				{
					cartObject.Shippings.Add(cartShipping);
				}
				else
				{
					cartObject.Shippings[0] = cartShipping;
				}
			}
		}
	}

	/// <summary>
	/// 注文メモ作成
	/// </summary>
	public void CreateOrderMemo()
	{
		foreach (CartObject coCart in this.CartList)
		{
			coCart.CreateOrderMemo(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
		}
	}

	/// <summary>
	/// 注文拡張項目の作成
	/// </summary>
	public void CreateOrderExtend()
	{
		foreach (CartObject coCart in this.CartList)
		{
			coCart.CreateOrderExtend();
		}
	}

	/// <summary>
	/// 注文配送先データバインド準備
	/// </summary>
	/// <param name="urlForError">エラー向けURL（カートやエラーページ）</param>
	public void PrepareForDataBindOrderShipping(string urlForError)
	{
		// 配送種別情報情報作成
		CreateShopShippingListOnDataBind(urlForError);

		// 配送方法情報情報作成
		CreateShippingMethodListOnDataBind();

		// 配送会社情報作成
		CreateDeliveryCompanyListOnDataBind();

		// 配送サービス選択項目情報作成
		CreateSelectableDeliveryCompanyListOnDataBind();

		// Create real shop area list on data bind
		CreateRealShopAreaListOnDataBind();

		// Create real shop name list on data bind
		CreateRealShopNameListOnDataBind();
	}

	/// <summary>
	/// データバインド用店舗配送種別作成
	/// </summary>
	/// <param name="strRidirectPage">エラー時に遷移する画面</param>
	private void CreateShopShippingListOnDataBind(string strRidirectPage)
	{
		this.ShopShippingList = new List<ShopShippingModel>();
		foreach (CartObject coCart in this.CartList.Items)
		{
			var shippingDateTimeInput = OrderCommon.GetShopShipping(coCart.ShopId, coCart.ShippingType);
			if (shippingDateTimeInput == null)
			{
				// TODO:カート画面で配送種別がない場合にちゃんと動くかチェック
				// 配送種別が見つからなかった場合はカート画面へとばす
				Response.Redirect(strRidirectPage);
			}

			this.ShopShippingList.Add(shippingDateTimeInput);
		}
	}

	/// <summary>
	/// データバインド用配送会社作成
	/// </summary>
	private void CreateDeliveryCompanyListOnDataBind()
	{
		this.DeliveryCompanyList = new List<DeliveryCompanyModel>();
		foreach (CartObject cart in this.CartList.Items)
		{
			var shopShipping = GetShopShipping(cart.ShopId, cart.ShippingType);
			var deliveryCompanyIds = shopShipping.CompanyList
				.Select(model => model.DeliveryCompanyId)
				.Distinct()
				.ToArray();
			this.DeliveryCompanyList.AddRange(
				deliveryCompanyIds
					.Where(id => (this.DeliveryCompanyList.Any(c => (c.DeliveryCompanyId == id)) == false))
					.Select(id => new DeliveryCompanyService().Get(id))
					.ToArray());
		}
	}

	/// <summary>
	/// データバインド用配送方法作成
	/// </summary>
	public void CreateShippingMethodListOnDataBind()
	{
		this.ShippingMethodList = new List<ListItemCollection>();
		var selected = new List<string>();

		// 商品同梱で追加となる商品を追加した状態で配送方法作成
		using (var productBundler = new ProductBundler(
			this.CartList.Items,
			this.LoginUserId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			hitTargetListIds: this.LoginUserHitTargetListIds,
			isFront: true))
		{
			this.DeliveryCompanyListMail = new List<List<DeliveryCompanyModel>>();
			var cartNo = 0;
			foreach (var cart in productBundler.CartList)
			{
				var shippingMethod = new ListItemCollection();

				//メール便配送サービスエスカレーション機能がONの場合
				if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
				{
					var shopShipping = GetShopShipping(cart.ShopId, cart.ShippingType);
					var deliveryCompanyId = DeliveryCompanyMailEscalation(cart.Items, shopShipping.CompanyListMail);
					// 合計サイズ係数以上のメール便が存在しない
					// カート内に配送サイズ区分がメールではない商品がある
					if (string.IsNullOrEmpty(deliveryCompanyId)
						|| cart.Items.Any(item => item.ShippingSizeKbn != Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL))
					{
						var expressDeliveryText = ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
						shippingMethod.Add(new ListItem(expressDeliveryText, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
						cart.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
					}
					else
					{
						shippingMethod = ValueText.GetValueItemList(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD);
					}
					this.ShippingMethodList.Add(shippingMethod);
					selected.Add(cart.Shippings[0].ShippingMethod);

					// メール便の場合
					if (cart.Shippings[0].IsMail)
					{
						var isValue = this.DeliveryCompanyListMail[cartNo].Any(company => company.DeliveryCompanyId == cart.Shippings[0].DeliveryCompanyId);
						if (isValue == false) cart.Shippings[0].DeliveryCompanyId = deliveryCompanyId;
						cartNo++;
					}
					// 宅配便の場合
					else
					{
						cartNo++;
						var checkDeliveryCompanyId = shopShipping.CompanyListExpress
							.Any(c => c.DeliveryCompanyId == cart.Shippings[0].DeliveryCompanyId);
						if (checkDeliveryCompanyId) continue;
						var newDefaultCompanyId = shopShipping.CompanyListExpress
							.FirstOrDefault(company => company.IsDefault);
						if (newDefaultCompanyId != null) cart.Shippings[0].DeliveryCompanyId = newDefaultCompanyId.DeliveryCompanyId;
					}
				}
				else
				{
					if (OrderCommon.IsAvailableShippingKbnMail(cart.Items) == false)
					{
						var expressDeliveryText = ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
						shippingMethod.Add(new ListItem(expressDeliveryText, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
					}
					else
					{
						shippingMethod = ValueText.GetValueItemList(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD);
					}

					this.ShippingMethodList.Add(shippingMethod);

					if (cart.Shippings[0].IsMail)
					{
						selected.AddRange(cart.Shippings.Select(shipping => shipping.ShippingMethod = OrderCommon.GetShippingMethod(cart.Items)));

						//商品同梱で配送方法のみ変更されるので、配送サービスの整合性を確認
						if (cart.Shippings[0].IsExpress)
						{
							var shopShipping = GetShopShipping(cart.ShopId, cart.ShippingType);
							var checkDeliveryCompanyId = shopShipping.CompanyListExpress
								.Any(c => c.DeliveryCompanyId == cart.Shippings[0].DeliveryCompanyId);
							if (checkDeliveryCompanyId) continue;

							var newDefaultCompanyId = shopShipping.CompanyListExpress
								.FirstOrDefault(company => company.IsDefault);
							if (newDefaultCompanyId != null) cart.Shippings[0].DeliveryCompanyId = newDefaultCompanyId.DeliveryCompanyId;
						}
					}
					else
					{
						selected.Add(shippingMethod[0].Value);
					}
				}
			}
		}
		this.SelectedShippingMethod = selected.ToArray();
	}

	/// <summary>
	/// 配送方法選択
	/// </summary>
	/// <param name="riCart">リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	public void SelectShippingMethod(RepeaterItem riCart, CartObject cartObj)
	{
		var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(riCart);
		wddlShippingMethod.SelectedValue = cartObj.Shippings[0].ShippingMethod;
	}

	#region 住所系

	/// <summary>
	/// 配送先グローバル関連項目設定
	/// </summary>
	/// <param name="riCart">リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	public void SetOrderShippingGlobalColumn(RepeaterItem riCart, CartObject cartObj)
	{
		var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingCountry");
		var shippingCountryName = (string.IsNullOrEmpty(cartObj.GetShipping().ShippingCountryName) == false)
			? cartObj.GetShipping().ShippingCountryName
			: cartObj.Owner.AddrCountryName;
		wddlShippingCountry.SelectItemByText(shippingCountryName);

		var shippingCountryIsoCode = (string.IsNullOrEmpty(cartObj.GetShipping().ShippingCountryIsoCode) == false)
			? cartObj.GetShipping().ShippingCountryIsoCode
			: cartObj.Owner.AddrCountryIsoCode;

		if (IsCountryUs(shippingCountryIsoCode))
		{
			var shippingAddr5 = (string.IsNullOrEmpty(cartObj.GetShipping().Addr5) == false)
				? cartObj.GetShipping().Addr5
				: cartObj.Owner.Addr5;

			var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingAddr5");
			wddlShippingAddr5.SelectItemByText(shippingAddr5);
		}
		if (IsCountryTw(shippingCountryIsoCode))
		{
			var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingAddr2");
			var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingAddr3");

			if (wddlShippingAddr2.HasInnerControl && wddlShippingAddr3.HasInnerControl)
			{
				wddlShippingAddr2.SelectItemByText(
					(string.IsNullOrEmpty(cartObj.GetShipping().Addr2) == false)
						? cartObj.GetShipping().Addr2
						: cartObj.Owner.Addr2);

				if (wddlShippingAddr2.DataSource != null) BindingDdlShippingAddr3(wddlShippingCountry);
				wddlShippingAddr3.ForceSelectItemByText(
					(string.IsNullOrEmpty(cartObj.GetShipping().Addr3) == false)
						? cartObj.GetShipping().Addr3
						: cartObj.Owner.Addr3);
			}
		}
	}

	/// <summary>
	/// 配送情報入力画面初期処理
	/// </summary>
	public void InitComponentsOrderShipping()
	{
		//------------------------------------------------------
		// 都道府県ドロップダウン作成
		//------------------------------------------------------
		this.Addr1List = new ListItemCollection();
		this.Addr1List.Add(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.Addr1List.Add(new ListItem(strPrefecture));
		}

		//------------------------------------------------------
		// 生年月日ドロップダウン作成
		//------------------------------------------------------
		this.OrderOwnerBirthYear.Add("");
		this.OrderOwnerBirthYear.AddRange(DateTimeUtility.GetBirthYearListItem());
		this.OrderOwnerBirthMonth.Add("");
		this.OrderOwnerBirthMonth.AddRange(DateTimeUtility.GetMonthListItem());
		this.OrderOwnerBirthDay.Add("");
		this.OrderOwnerBirthDay.AddRange(DateTimeUtility.GetDayListItem());

		//------------------------------------------------------
		// 性別ラジオボタン設定
		//------------------------------------------------------
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_SEX))
		{
			this.OrderOwnerSex.Add(li);
		}

		//------------------------------------------------------
		// アドレス帳ドロップダウン作成
		//------------------------------------------------------
		this.UserShippingList = new ListItemCollection();
		this.UserShippingList.Add(
			new ListItem(
				ReplaceTag("@@DispText.shipping_addr_kbn_list.owner@@"),
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER));

		this.UserShippingList.Add(
			new ListItem(
				ReplaceTag("@@DispText.shipping_addr_kbn_list.new@@"),
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));

		if (Constants.STORE_PICKUP_OPTION_ENABLED
			&& (this.IsGiftPage == false))
		{
			this.UserShippingList.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.storepickup@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP));
		}

		if (this.IsGiftPage)
		{
			if (this.IsLoggedIn)
			{
				this.UserShippingAddr = new UserShippingService()
					.GetAllOrderByShippingNoDesc(this.LoginUserId)
					.Where(item => (item.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
						.ToArray();

				this.UserShippingList.AddRange(
					this.UserShippingAddr.Select(us => new ListItem(
						us.Name,
						us.ShippingNo.ToString()))
							.ToArray());
			}
		}
		else
		{
			if (this.IsLoggedIn)
			{
				this.UserShippingAddr = new UserShippingService().GetAllOrderByShippingNoDesc(this.LoginUserId);

				if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
				{
					this.UserShippingList.AddRange(
						this.UserShippingAddr.Select(us => new ListItem(
							us.Name,
							us.ShippingNo.ToString()))
								.ToArray());
				}
				else
				{
					var userShippingNormal = this.UserShippingAddr.Where(item => item.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
					this.UserShippingList.AddRange(
						userShippingNormal.Select(us => new ListItem(
							us.Name,
							us.ShippingNo.ToString()))
								.ToArray());
				}
			}

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.GIFTORDER_OPTION_ENABLED == false)
			{
				this.UserShippingList.Add(
					new ListItem(
						ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
			}
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 注文者国情報ドロップダウン作成
			this.UserCountryList = GlobalAddressUtil.GetCountriesAll();
			this.UserCountryDisplayList = new ListItemCollection();
			this.UserCountryDisplayList.Add(new ListItem("", ""));
			this.UserCountryDisplayList.AddRange(this.UserCountryList.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());

			// 州情報ドロップダウン作成
			this.UserStateList = new ListItemCollection();
			this.UserStateList.Add(new ListItem("", ""));
			this.UserStateList.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

			// 台湾都市情報ドロップダウン作成
			this.UserTwCityList = new ListItemCollection();
			this.UserTwCityList.AddRange(Constants.TW_CITIES_LIST.Select(state => new ListItem(state)).ToArray());

			// 配送可能国情報ドロップダウン作成
			this.ShippingAvailableCountryList = new CountryLocationService().GetShippingAvailableCountry();
			this.ShippingAvailableCountryDisplayList = new ListItemCollection();
			this.ShippingAvailableCountryDisplayList.Add(new ListItem("", ""));
			this.ShippingAvailableCountryDisplayList.AddRange(this.ShippingAvailableCountryList.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		}
		else
		{
			// グローバルOPがOFFの場合は、空のドロップダウンリスト
			this.UserCountryDisplayList = new ListItemCollection { new ListItem("") };
			this.UserStateList = new ListItemCollection { new ListItem("") };
			this.UserTwCityList = new ListItemCollection { new ListItem("") };
			this.ShippingAvailableCountryDisplayList = new ListItemCollection { new ListItem("") };
		}

		this.HasUserShippingError = false;
	}

	/// <summary>
	/// Get Shipping Kbn List
	/// </summary>
	/// <param name="index">Cart Index</param>
	/// <returns>List Item Shipping Kbn</returns>
	public ListItem[] GetShippingKbnList(int index)
	{
		var result = this.UserShippingList.Cast<ListItem>().ToArray();
		var cart = this.CartList.Items[index];
		var shopShipping = GetShopShipping(
			cart.ShopId,
			cart.ShippingType);
		var deliveryCompanyList = (
			cart.GetShipping().ShippingMethod
				== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? shopShipping.CompanyListExpress
					: shopShipping.CompanyListMail;

		var deliveryCompanyIds = deliveryCompanyList
			.Select(model => model.DeliveryCompanyId)
			.Distinct()
			.ToArray();

		cart.IsShowDisplayConvenienceStore = (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& deliveryCompanyIds.Any(item => item
				== Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID));

		if (cart.IsShowDisplayConvenienceStore == false)
		{
			result = this.UserShippingList.Cast<ListItem>()
				.Where(item => item.Value
					!= CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE).ToArray();

			if (this.UserShippingAddr != null)
			{
				var userShippingConvenience = this.UserShippingAddr
					.Where(item => item.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					.Select(item => item.ShippingNo.ToString())
					.ToArray();

				if (userShippingConvenience.Any())
				{
					result = result.Where(item => (
						userShippingConvenience.Contains(item.Value) == false)).ToArray();
				}
			}
		}

		return result;
	}

	/// <summary>
	/// 配送情報入力画面表示初期処理
	/// </summary>
	/// <param name="ri">対象リピータアイテム</param>
	/// <param name="e"></param>
	public void InitComponentsDispOrderShipping(EventArgs e)
	{
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// 注文メモ欄デザイン設定
			SetOrderMemoDesign(riCart);

			// 配送情報入力画面表示初期処理
			InitComponentsDispOrderShipping(
				riCart,
				e);

			// 配送変更表示初期処理
			var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(riCart);
			if (wddlShippingMethod.InnerControl != null)
			{
				ddlShippingMethodList_OnSelectedIndexChanged(wddlShippingMethod, e);
			}
		}

		SetOrderExtendInputForm();
	}
	/// <summary>
	/// 配送情報入力画面表示初期処理
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム(cart/shipping)</param>
	/// <param name="e"></param>
	public void InitComponentsDispOrderShipping(RepeaterItem riTarget, EventArgs e)
	{
		// ラップ済みコントロール宣言
		var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(riTarget, "cbShipToCart1Address", false);
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingKbnList");
		var wrblSaveToUserShipping = GetWrappedControl<WrappedRadioButtonList>(riTarget, "rblSaveToUserShipping");

		if (wcbShipToCart1Address.InnerControl != null
			&& riTarget.ItemIndex != 0)
		{
			cbShipToCart1Address_OnCheckedChanged(wcbShipToCart1Address, e);
		}

		if (wddlShippingKbnList.InnerControl != null)
		{
			ddlShippingKbnList_OnSelectedIndexChanged(wddlShippingKbnList, e);
		}

		if (wrblSaveToUserShipping.InnerControl != null)
		{
			rblSaveToUserShipping_OnSelectedIndexChanged(wrblSaveToUserShipping, e);
		}

		var wddlUniformInvoiceType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceType");
		var wddlUniformInvoiceTypeOption = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceTypeOption");
		var wcbSaveToUserInvoice = GetWrappedControl<WrappedCheckBox>(riTarget, "cbSaveToUserInvoice");

		var repeaterItem = GetParentRepeaterItem(riTarget, "rCartList");
		if (repeaterItem == null) return;

		var cart = this.CartList.Items[repeaterItem.ItemIndex];

		if (wddlUniformInvoiceType.InnerControl != null)
		{
			wddlUniformInvoiceType.SelectedValue = StringUtility.ToEmpty(cart.Shippings[0].UniformInvoiceType);
			ddlUniformInvoiceType_SelectedIndexChanged(wddlUniformInvoiceType, e);
		}

		if (wddlUniformInvoiceTypeOption.InnerControl != null)
		{
			wddlUniformInvoiceTypeOption.SelectedValue = StringUtility.ToEmpty(cart.Shippings[0].UniformInvoiceTypeOption);
			ddlUniformInvoiceTypeOption_SelectedIndexChanged(wddlUniformInvoiceTypeOption, e);
		}

		if (wcbSaveToUserInvoice.InnerControl != null)
		{
			wcbSaveToUserInvoice.Checked = cart.Shippings[0].UserInvoiceRegistFlg;
			cbSaveToUserInvoice_CheckedChanged(wcbSaveToUserInvoice, e);
		}

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			var wddlInvoiceCarryType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlInvoiceCarryType");
			var wddlInvoiceCarryTypeOption = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlInvoiceCarryTypeOption");
			var wcbCarryTypeOptionRegist = GetWrappedControl<WrappedCheckBox>(riTarget, "cbCarryTypeOptionRegist");
			if (wddlInvoiceCarryType.InnerControl != null)
			{
				wddlInvoiceCarryType.SelectedValue = StringUtility.ToEmpty(cart.Shippings[0].CarryType);
				ddlInvoiceCarryType_SelectedIndexChanged(wddlInvoiceCarryType, e);
			}

			if (wddlInvoiceCarryTypeOption.InnerControl != null)
			{
				wddlInvoiceCarryTypeOption.SelectedValue = StringUtility.ToEmpty(cart.Shippings[0].CarryTypeOption);
				ddlInvoiceCarryTypeOption_SelectedIndexChanged(wddlInvoiceCarryTypeOption, e);
			}

			if (wcbCarryTypeOptionRegist.InnerControl != null)
			{
				wcbCarryTypeOptionRegist.Checked = cart.Shippings[0].UserInvoiceRegistFlg;
				cbCarryTypeOptionRegist_CheckedChanged(wcbCarryTypeOptionRegist, e);
			}
		}
	}

	/// <summary>
	/// 配送先不可エリアエラーメッセージを表示
	/// </summary>
	/// <param name="rCart">カート</param>
	public void ShowShippingAreaErrorMessage(Repeater rCart)
	{
		// 配送先不可エラーで戻ってきた場合、該当箇所にエラー表示
		if (string.IsNullOrEmpty(SessionManager.UnavailableShippingErrorMessage)) return;

		var unavailableShippingErrorMessage = SessionManager.UnavailableShippingErrorMessage;
		SessionManager.UnavailableShippingErrorMessage = null;

		// 注文者情報が配送先か配送先情報に入力された値が配送先かを判断する
		foreach (RepeaterItem riCart in rCart.Items)
		{
			var addrKbn = this.CartList.Items[riCart.ItemIndex].Shippings[0].ShippingAddrKbn;

			// カート分割時に配送不可エリアをチェックする
			if (rCart.Items.Count > 1)
			{
				// カートごとの配送不可エリアを取得
				var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
					this.CartList.Items[riCart.ItemIndex].ShippingType,
					this.CartList.Items[riCart.ItemIndex].Shippings[0].DeliveryCompanyId);

				var unavailableShippingArea = OrderCommon.CheckUnavailableShippingArea(
					unavailableShippingZip,
					this.CartList.Items[riCart.ItemIndex].Shippings[0].HyphenlessZip);
				if (unavailableShippingArea == false) continue;
			}

			// 注文者情報が配送先か配送先情報に入力された値が配送先かを判断する
			switch (addrKbn)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					// 注文者情報の郵便番号入力フォーム
					var wsOwnerZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sOwnerZipError");
					wsOwnerZipError.InnerText = unavailableShippingErrorMessage;
					wsOwnerZipError.Focus();
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
					// 配送先情報の郵便番号入力フォーム
					var wsShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "sShippingZipError");
					wsShippingZipError.InnerText = unavailableShippingErrorMessage;
					wsShippingZipError.Focus();
					break;
			}
		}
	}

	/// <summary>
	/// ギフト配送先不可エリアエラーメッセージを表示
	/// </summary>
	/// <param name="rCart">カート</param>
	public void ShowShippingAreaErrorMessageForGiftOrder(Repeater rCart)
	{
		// 配送先不可エラーで戻ってきた場合、該当箇所にエラー表示
		if (string.IsNullOrEmpty(SessionManager.UnavailableShippingErrorMessage)) return;

		var unavailableShippingErrorMessage = SessionManager.UnavailableShippingErrorMessage;
		SessionManager.UnavailableShippingErrorMessage = null;

		foreach (RepeaterItem riCart in rCart.Items)
		{
			var rCartShipping = ((Repeater)riCart.FindControl("rCartShippings"));
			if (rCartShipping == null) continue;

			foreach (RepeaterItem riCartShippingItem in rCartShipping.Items)
			{
				var addrKbn = this.CartList.Items[riCart.ItemIndex].Shippings[riCartShippingItem.ItemIndex].ShippingAddrKbn;

				// カート分割時に配送不可エリアをチェックする
				if (rCart.Items.Count > 1)
				{
					// カートごとの配送不可エリアを取得
					var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
						this.CartList.Items[riCart.ItemIndex].ShippingType,
						this.CartList.Items[riCart.ItemIndex].Shippings[riCartShippingItem.ItemIndex].DeliveryCompanyId);

					var unavailableShippingArea = OrderCommon.CheckUnavailableShippingArea(
						unavailableShippingZip,
						this.CartList.Items[riCart.ItemIndex].Shippings[riCartShippingItem.ItemIndex].HyphenlessZip);
					if (unavailableShippingArea == false) continue;
				}

				// 注文者情報が配送先か配送先情報に入力された値が配送先かを判断する
				switch (addrKbn)
				{
					case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
						// 注文者情報の郵便番号入力フォーム
						var wsOwnerZipError = GetWrappedControl<WrappedHtmlGenericControl>(riCartShippingItem, "sOwnerZipError");
						wsOwnerZipError.InnerText = unavailableShippingErrorMessage;
						wsOwnerZipError.Focus();
						break;

					case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
						// 配送先情報の郵便番号入力フォーム
						var wsShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(riCartShippingItem, "sShippingZipError");
						wsShippingZipError.InnerText = unavailableShippingErrorMessage;
						wsShippingZipError.Focus();
						break;
				}
			}
		}
	}

	/// <summary>
	/// Get cart payment ID
	/// </summary>
	/// <param name="riCart">A cart repeater item</param>
	/// <returns>A cart payment ID</returns>
	public string GetCartPaymentId(RepeaterItem riCart)
	{
		foreach (RepeaterItem riPaymentItem in GetWrappedControl<WrappedRepeater>(riCart, "rPayment").Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId");

			bool isSelected = false;
			if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB))
			{
				isSelected = GetWrappedControl<WrappedRadioButtonGroup>(riPaymentItem, "rbgPayment").Checked;
			}
			else if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL))
			{
				var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPaymentItem.Parent.Parent, "ddlPayment");
				isSelected = (wddlPayment.SelectedValue == whfPaymentId.Value);
			}
			if (isSelected) return whfPaymentId.Value;
		}

		return string.Empty;
	}

	/// <summary>
	/// 注文メモデザインセット
	/// </summary>
	/// <param name="riCart"></param>
	public void SetOrderMemoDesign(RepeaterItem riCart)
	{
		var wrRepeaterMemo = GetWrappedControl<WrappedRepeater>(riCart, "rMemos");
		foreach (RepeaterItem riMemoItem in wrRepeaterMemo.Items)
		{
			var wrtbMemo = GetWrappedControl<WrappedTextBox>(riMemoItem, "tbMemo");

			if ((this.CartList.Items.Count > 0) && (this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Width != null))
			{
				wrtbMemo.InnerControl.Width = (int)this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Width;
			}
			if ((this.CartList.Items.Count > 0) && (this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Height != null))
			{
				if (this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Width != null)
				{
					wrtbMemo.InnerControl.Width = (int)this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Width;
				}
				if (this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Height != null)
				{
					wrtbMemo.InnerControl.Height = (int)this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Height;
				}
				wrtbMemo.CssClass = this.CartList.Items[riCart.ItemIndex].OrderMemos[riMemoItem.ItemIndex].Class;
			}
		}
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（注文者情報）
	/// </summary>
	/// <param name="e"></param>
	public bool lbNext_Click_OrderShipping_Owner(object sender, System.EventArgs e)
	{
		#region ラップ済みコントロール宣言
		var wtbOwnerName1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName1", "");
		var wtbOwnerName2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName2", "");
		var wtbOwnerNameKana1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana1", "");
		var wtbOwnerNameKana2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana2", "");
		var wddlOwnerBirthYear = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthYear", "");
		var wddlOwnerBirthMonth = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthMonth", "");
		var wddlOwnerBirthDay = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthDay", "");
		var wrblOwnerSex = GetWrappedControl<WrappedRadioButtonList>(this.FirstRpeaterItem, "rblOwnerSex");
		var wtbOwnerMailAddr = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr", "");
		var wtbOwnerMailAddrConf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf", "");
		var wtbOwnerMailAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr2", "");
		var wtbOwnerMailAddr2Conf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr2Conf", "");
		var wtbOwnerZip1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip1", "");
		var wtbOwnerZip2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip2", "");
		var wddlOwnerAddr1 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr1");
		var wddlOwnerAddr2 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr2");
		var wddlOwnerAddr3 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr3");
		var wtbOwnerAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr2", "");
		var wtbOwnerAddr3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr3", "");
		var wtbOwnerAddr4 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr4", "");
		var wtbOwnerCompanyName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyName", "");
		var wtbOwnerCompanyPostName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyPostName", "");
		var wtbOwnerTel1_1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_1", "");
		var wtbOwnerTel1_2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_2", "");
		var wtbOwnerTel1_3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_3", "");
		var wtbOwnerTel2_1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2_1", "");
		var wtbOwnerTel2_2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2_2", "");
		var wtbOwnerTel2_3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2_3", "");
		var wcbOwnerMailFlg = GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg", false);
		var wcvOwnerBirth = GetWrappedControl<WrappedCustomValidator>(this.FirstRpeaterItem, "cvOwnerBirth");
		var wcvOwnerSex = GetWrappedControl<WrappedCustomValidator>(this.FirstRpeaterItem, "cvOwnerSex");
		var wcvOwnerMailAddrForCheck = GetWrappedControl<WrappedCustomValidator>(this.FirstRpeaterItem, "cvOwnerMailAddrForCheck");
		var wcvOwnerMailAddr = GetWrappedControl<WrappedCustomValidator>(this.FirstRpeaterItem, "cvOwnerMailAddr");
		var wcvOwnerMailAddrConf = GetWrappedControl<WrappedCustomValidator>(this.FirstRpeaterItem, "cvOwnerMailAddrConf");
		var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry");
		var wtbOwnerZipGlobal = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZipGlobal", "");
		var wddlOwnerAddr5 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr5");
		var wtbOwnerAddr5 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr5", "");
		var wtbOwnerTel1Global = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1Global", "");
		var wtbOwnerTel2Global = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2Global", "");

		var wtbLoginId = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginId");
		var wtbPassword = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbPassword");
		var wtbLoginIdInMailAddr = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginIdInMailAddr");
		var wdLoginErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dLoginErrorMessage", "");
		var wcbAutoCompleteLoginIdFlg = GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbAutoCompleteLoginIdFlg", false);
		var wtbUserLoginId = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserLoginId");
		var wtbUserPassword = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPassword");
		var wtbUserPasswordConf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPasswordConf");
		var wcbUserRegister = GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbUserRegister");
		var wtbOwnerZip = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip", string.Empty);
		var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1", string.Empty);
		var wtbOwnerTel2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2", string.Empty);
		var wdvUserPassword = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dvUserPassword");
		var wucUserExtend = wdvUserPassword.FindControl("ucBodyUserExtendLandingPageRegist");
		#endregion
		var userService = new UserService();
		var user = userService.Get(this.LoginUserId);
		var ownerKbn = this.IsLoggedIn
			? user.UserKbn
			: this.IsSmartPhone
				? Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST
				: Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST;
		var isOwnerAddrJp = IsCountryJp(wddlOwnerCountry.SelectedValue);
		//------------------------------------------------------
		// 注文者情報取得･入力チェック
		//------------------------------------------------------
		// データ格納
		Hashtable htOrderOwner = new Hashtable();
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerName1.Text + wtbOwnerName2.Text).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME1, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerName1.Text).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME2, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerName2.Text).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, StringUtility.ToZenkaku(wtbOwnerNameKana1.Text + wtbOwnerNameKana2.Text).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, StringUtility.ToZenkaku(wtbOwnerNameKana1.Text).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, StringUtility.ToZenkaku(wtbOwnerNameKana2.Text).Trim());
		if ((wddlOwnerBirthYear.SelectedValue + wddlOwnerBirthMonth.SelectedValue + wddlOwnerBirthDay.SelectedValue).Length != 0)
		{
			htOrderOwner.Add(
				Constants.FIELD_ORDEROWNER_OWNER_BIRTH,
				wddlOwnerBirthYear.SelectedValue + "/" + wddlOwnerBirthMonth.SelectedValue + "/" + wddlOwnerBirthDay.SelectedValue);
		}
		else
		{
			htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, (wddlOwnerBirthYear.InnerControl == null) ? DateTimeUtility.GetDefaultSettingBirthday().ToString() : null);
		}

		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX, (wrblOwnerSex.InnerControl != null)
			? wrblOwnerSex.SelectedValue
			: Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.sex.default@@"));
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)
		{
			// どちらか一方が入力されていること
			htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_for_check", StringUtility.ToHankaku(wtbOwnerMailAddr.Text.Trim() + wtbOwnerMailAddr2.Text.Trim()));
		}
		else
		{
			// PCアドレス必須
			htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_for_check", StringUtility.ToHankaku(wtbOwnerMailAddr.Text.Trim()));
		}
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH + "_for_check", (htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] == null) ? "" : htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, StringUtility.ToHankaku(wtbOwnerMailAddr.Text));
		var wtbOwnerMailAddrConfText = (wtbOwnerMailAddrConf.InnerControl != null) ? wtbOwnerMailAddrConf.Text : wtbOwnerMailAddr.Text;
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_conf", StringUtility.ToHankaku(wtbOwnerMailAddrConfText));
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2, StringUtility.ToHankaku(wtbOwnerMailAddr2.Text));
		var wtbOwnerMailAddr2ConfText = (wtbOwnerMailAddr2Conf.InnerControl != null) ? wtbOwnerMailAddr2Conf.Text : wtbOwnerMailAddr2.Text;
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2 + "_conf", StringUtility.ToHankaku(wtbOwnerMailAddr2ConfText));

		// Set value for zip code
		var inputZipCode = (wtbOwnerZip1.HasInnerControl)
			? StringUtility.ToHankaku(wtbOwnerZip1.Text.Trim())
			: StringUtility.ToHankaku(wtbOwnerZip.Text.Trim());
		if (wtbOwnerZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(wtbOwnerZip2.Text.Trim()));
		var zipCode = new ZipCode(inputZipCode);
		htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = (string.IsNullOrEmpty(zipCode.Zip) == false)
			? zipCode.Zip
			: inputZipCode;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1] = zipCode.Zip1;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2] = zipCode.Zip2;
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, wddlOwnerAddr1.SelectedValue);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr2.Text, isOwnerAddrJp).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr3.Text, isOwnerAddrJp).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR4, DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr4.Text, isOwnerAddrJp).Trim());
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR5, string.Empty);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE, string.Empty);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME, string.Empty);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME, wtbOwnerCompanyName.Text);
		htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME, wtbOwnerCompanyPostName.Text);

		// Set value for telephone 1
		var inputTel1 = (wtbOwnerTel1.HasInnerControl)
			? StringUtility.ToHankaku(wtbOwnerTel1.Text.Trim())
			: StringUtility.ToHankaku(wtbOwnerTel1_1.Text.Trim());
		if (wtbOwnerTel1_1.HasInnerControl)
		{
			inputTel1 = UserService.CreatePhoneNo(
				inputTel1,
				StringUtility.ToHankaku(wtbOwnerTel1_2.Text.Trim()),
				StringUtility.ToHankaku(wtbOwnerTel1_3.Text.Trim()));
		}
		var tel1 = new Tel(inputTel1);
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1] = tel1.Tel1;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2] = tel1.Tel2;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3] = tel1.Tel3;
		htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1] =
			(string.IsNullOrEmpty(tel1.TelNo) == false)
				? tel1.TelNo
				: inputTel1;

		// Set value for telephone 2
		var inputTel2 = (wtbOwnerTel2_1.HasInnerControl)
			? StringUtility.ToHankaku(wtbOwnerTel2_1.Text.Trim())
			: StringUtility.ToHankaku(wtbOwnerTel2.Text.Trim());
		if (wtbOwnerTel2_1.HasInnerControl)
		{
			inputTel2 = UserService.CreatePhoneNo(
				inputTel2,
				StringUtility.ToHankaku(wtbOwnerTel2_2.Text.Trim()),
				StringUtility.ToHankaku(wtbOwnerTel2_3.Text.Trim()));
		}
		var tel2 = new Tel(inputTel2);
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1] = tel2.Tel1;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2] = tel2.Tel2;
		htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3] = tel2.Tel3;
		htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2] =
			(string.IsNullOrEmpty(tel2.TelNo) == false)
				? tel2.TelNo
				: inputTel2;

		if ((string.IsNullOrEmpty((string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1]) == false)
			|| (string.IsNullOrEmpty((string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2]) == false)
			|| (string.IsNullOrEmpty((string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3]) == false))
		{
			htOrderOwner.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1 + "_for_check", htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1]);
			htOrderOwner.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2 + "_for_check", htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2]);
			htOrderOwner.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3 + "_for_check", htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3]);
		}
		htOrderOwner.Add(
			Constants.FIELD_USER_MAIL_FLG,
			(wcbOwnerMailFlg.InnerControl != null)
				? wcbOwnerMailFlg.Checked
					? Constants.FLG_USER_MAILFLG_OK
					: Constants.FLG_USER_MAILFLG_NG
				: this.IsLoggedIn
					? user.MailFlg
					: Constants.FLG_USER_MAILFLG_NG);


		// メールアドレスのログインID利用時 かつ PCサイトユーザーの場合は、メールアドレス（ログインID）の重複チェック
		if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) && UserService.IsPcSiteOrOfflineUser(ownerKbn))
		{
			htOrderOwner.Add(Constants.FIELD_USER_LOGIN_ID, StringUtility.ToHankaku(wtbOwnerMailAddr.Text));
			htOrderOwner.Add(Constants.FIELD_USER_LOGIN_ID + "_use_mailaddress_flg", StringUtility.ToHankaku(wtbOwnerMailAddr.Text));
			htOrderOwner.Add(Constants.FIELD_USER_USER_ID, this.LoginUserId);
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var ownerAddrCountryName = wddlOwnerCountry.SelectedText;
			var ownerAddrCountryIsoCode = wddlOwnerCountry.SelectedValue;

			htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME] = ownerAddrCountryName;
			htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE] = ownerAddrCountryIsoCode;
			htOrderOwner.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME + "_for_check", ownerAddrCountryName);
			if (isOwnerAddrJp == false)
			{
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = StringUtility.ToHankaku(wtbOwnerZipGlobal.Text);
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1] = string.Empty;
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2] = string.Empty;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1] = StringUtility.ToHankaku(wtbOwnerTel1Global.Text);
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1] = string.Empty;
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2] = string.Empty;
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3] = string.Empty;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2] = StringUtility.ToHankaku(wtbOwnerTel2Global.Text);
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1] = string.Empty;
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2] = string.Empty;
				htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3] = string.Empty;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] = string.Empty;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] = string.Empty;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] = string.Empty;

				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR5] =
					IsCountryUs(ownerAddrCountryIsoCode)
						? wddlOwnerAddr5.SelectedText
						: DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr5.Text, isOwnerAddrJp).Trim();
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2] =
					(IsCountryTw(ownerAddrCountryIsoCode) && (string.IsNullOrEmpty(wddlOwnerAddr2.SelectedText) == false))
						? wddlOwnerAddr2.SelectedText
						: DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr2.Text, isOwnerAddrJp).Trim();
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3] =
					(IsCountryTw(ownerAddrCountryIsoCode) && (string.IsNullOrEmpty(wddlOwnerAddr3.SelectedText) == false))
						? wddlOwnerAddr3.SelectedText
						: DataInputUtility.ConvertToFullWidthBySetting(wtbOwnerAddr3.Text, isOwnerAddrJp).Trim();
			}
		}

		// 会員登録の場合、ログイン・パスワードセット
		var isUserRegister = (wcbUserRegister.HasInnerControl && wcbUserRegister.Checked);
		if (isUserRegister)
		{
			htOrderOwner[Constants.FIELD_USER_LOGIN_ID] = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? wtbOwnerMailAddr.Text : wtbUserLoginId.Text;

			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false)
			{
				htOrderOwner.Add(Constants.FIELD_USER_LOGIN_ID + "_input_check", StringUtility.ToHankaku(wtbUserLoginId.Text));
			}

			// AmazonPayの請求先住所取得オプションの対象ではない場合はパスワード情報を設定
			if (IsTargetToExtendedAmazonAddressManagerOption() == false)
			{
				var userSessionInPassword = (string)Session[Constants.SESSION_KEY_LP_PASSWORD];
				var userSessionInPasswordCnf = (string)Session[Constants.SESSION_KEY_LP_PASSWORDCONF];

				if ((string.IsNullOrEmpty(userSessionInPassword)) == false)
				{
					htOrderOwner[Constants.FIELD_USER_PASSWORD] = this.IsVisible_UserPassword ? userSessionInPassword : null;
					htOrderOwner[Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF] = this.IsVisible_UserPassword ? userSessionInPasswordCnf : null;
				}
				else
				{
					htOrderOwner[Constants.FIELD_USER_PASSWORD] = this.IsVisible_UserPassword ? wtbUserPassword.Text : null;
					htOrderOwner[Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF] = this.IsVisible_UserPassword ? wtbUserPasswordConf.Text : null;
				}
			}
		}

		// エラーチェック＆カスタムバリデータへセット
		var validationName = IsTargetToExtendedAmazonAddressManagerOption()
			? "OrderOwnerForAmazonPayGuest"
			: (this.IsLoggedIn || isUserRegister)
				? isOwnerAddrJp ? "OrderShipping" : "OrderShippingGlobal"
				: isOwnerAddrJp ? "OrderShippingGuest" : "OrderShippingGuestGlobal";
		var orderOwnerForValidate = (Hashtable)htOrderOwner.Clone();
		if (wtbOwnerTel1.HasInnerControl == false)
		{
			orderOwnerForValidate.Remove(Constants.FIELD_ORDEROWNER_OWNER_TEL1);
		}

		if (wtbOwnerTel2.HasInnerControl == false)
		{
			orderOwnerForValidate.Remove(Constants.FIELD_ORDEROWNER_OWNER_TEL2);
		}
		var dicOwnerErrorMessages = Validator.ValidateAndGetErrorContainer(
			validationName,
			orderOwnerForValidate,
			Constants.GLOBAL_OPTION_ENABLE ? wddlOwnerCountry.SelectedValue : "");

		// Remove validate for 1 text box owner tel 1
		if (wtbOwnerTel1_1.HasInnerControl
			&& dicOwnerErrorMessages.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_TEL1))
		{
			dicOwnerErrorMessages.Remove(Constants.FIELD_ORDEROWNER_OWNER_TEL1);
		}

		// Remove validate for 1 text box owner tel 2
		if (wtbOwnerTel2_1.HasInnerControl
			&& dicOwnerErrorMessages.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_TEL2))
		{
			dicOwnerErrorMessages.Remove(Constants.FIELD_ORDEROWNER_OWNER_TEL2);
		}

		// Check convenience store EcPay
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& HasConvenienceStoreInCartList())
		{
			// Check tel no taiwan
			var ownerTelNo1 = StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1]);
			var ownerTelNo2 = StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2]);

			ownerTelNo1 = isOwnerAddrJp
				? ownerTelNo1.Replace("-", string.Empty)
				: ownerTelNo1;
			ownerTelNo2 = isOwnerAddrJp
				? ownerTelNo2.Replace("-", string.Empty)
				: ownerTelNo2;

			var isTelNo1Valid = OrderCommon.CheckValidTelNoTaiwanForEcPay(ownerTelNo1);
			var isTelNo2Valid = OrderCommon.CheckValidTelNoTaiwanForEcPay(ownerTelNo2);
			var errorMessageFrontTelNoIsNotTaiwan = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TEL_NO_NOT_TAIWAN);

			if ((string.IsNullOrEmpty(ownerTelNo1) == false) && (isTelNo1Valid == false))
			{
				var currentOwnerTel1 = isOwnerAddrJp
					? Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_1"
					: Constants.FIELD_ORDEROWNER_OWNER_TEL1;
				dicOwnerErrorMessages[currentOwnerTel1] = errorMessageFrontTelNoIsNotTaiwan;
			}

			if ((string.IsNullOrEmpty(ownerTelNo2) == false) && (isTelNo2Valid == false))
			{
				var currentOwnerTel2 = isOwnerAddrJp
					? Constants.FIELD_ORDEROWNER_OWNER_TEL2 + "_2"
					: Constants.FIELD_ORDEROWNER_OWNER_TEL2;
				dicOwnerErrorMessages[currentOwnerTel2] = errorMessageFrontTelNoIsNotTaiwan;
			}

			// Check owner name
			var ownerName = StringUtility.ToEmpty(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME]);
			if (OrderCommon.CheckOwnerNameForEcPay(ownerName) == false)
			{
				dicOwnerErrorMessages[Constants.FIELD_ORDEROWNER_OWNER_NAME1] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_OWNER_NAME_OF_CONVENIENCE_STORE_INVALID);
			}
		}

		// ログインID重複チェック（UserInputを暫定的に利用）
		if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) && UserService.IsPcSiteOrOfflineUser(ownerKbn))
		{
			var userInputForDuplicationCheck = new UserInput
			{
				LoginId = (string)htOrderOwner[Constants.FIELD_USER_LOGIN_ID],
				UserId = this.LoginUserId,
			};
			var duplicationMessage = userInputForDuplicationCheck.CheckDuplicationLoginId(
				isOwnerAddrJp
					? UserInput.EnumUserInputValidationKbn.UserModify
					: UserInput.EnumUserInputValidationKbn.UserModifyGlobal);
			if (string.IsNullOrEmpty(duplicationMessage) == false)
			{
				dicOwnerErrorMessages[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] =
					dicOwnerErrorMessages.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR)
						? string.Format("{0}\n{1}", dicOwnerErrorMessages[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR], duplicationMessage)
						: duplicationMessage;
			}
		}
		else if (isUserRegister)
		{
			var userInputRegisterCheck = new UserInput
			{
				LoginId = (string)htOrderOwner[Constants.FIELD_USER_LOGIN_ID],
				Password = wtbUserPassword.Text,
				PasswordConf = wtbUserPasswordConf.Text,
			};
			var duplicationMessage = userInputRegisterCheck.CheckDuplicationLoginId(
				isOwnerAddrJp
					? UserInput.EnumUserInputValidationKbn.UserRegist
					: UserInput.EnumUserInputValidationKbn.UserRegistGlobal);
			if (string.IsNullOrEmpty(duplicationMessage) == false)
			{
				dicOwnerErrorMessages[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] =
					dicOwnerErrorMessages.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR)
						? string.Format("{0}\n{1}", dicOwnerErrorMessages[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR], duplicationMessage)
						: duplicationMessage;
			}
		}

		// ユーザー拡張項目検証
		if (wdvUserPassword.Visible && wucUserExtend != null)
		{
			var userExtendInput = ((UserExtendUserControl)wucUserExtend).UserExtend;
			dicOwnerErrorMessages = dicOwnerErrorMessages.Concat(userExtendInput.Validate())
				.ToDictionary(i => i.Key, i => i.Value);
		}

		var isLandingCartInput = ((this.CartList.LandingCartInputAbsolutePath != null)
			&& this.CartList.LandingCartInputAbsolutePath.Contains(Constants.PAGE_FRONT_LANDING_LANDING_CART_INPUT));

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
			&& (isLandingCartInput == false))
		{
			var countryIsoCode = Constants.GLOBAL_OPTION_ENABLE
				? wddlOwnerCountry.SelectedValue
				: Constants.COUNTRY_ISO_CODE_JP;

			var telephone = GetValueForTelephone(
				wtbOwnerTel1_1,
				wtbOwnerTel1_2,
				wtbOwnerTel1_3,
				wtbOwnerTel1,
				wtbOwnerTel1Global,
				countryIsoCode);

			// Is a user login account and does not change the phone number
			var isLoginAndNotChangePhone = ((this.LoginUser != null)
				&& (this.LoginUser.Tel1 == telephone));

			// Is a shipping page and use guest account
			var isUserGuest = ((this.IsLandingPage == false)
				&& (this.IsLoggedIn == false));

			// Is a landing page not login and register
			var isNotRegisterUser = (this.IsLandingPage
				&& (this.IsLoggedIn == false)
				&& (isUserRegister == false));

			if (isLoginAndNotChangePhone
				|| isUserGuest
				|| isNotRegisterUser)
			{
				this.HasAuthenticationCode = true;
			}

			if (this.HasAuthenticationCode)
			{
				var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(
					IsCountryJp(wddlOwnerCountry.SelectedValue),
					this.FirstRpeaterItem);

				this.AuthenticationCode = wtbAuthenticationCode.Text;
				this.CartList.AuthenticationCode = wtbAuthenticationCode.Text;
				this.CartList.HasAuthenticationCode = true;
			}
			else
			{
				dicOwnerErrorMessages.Add(
					Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_VERIFICATION_CODE_IS_INCORRECT));
			}
		}

		if (dicOwnerErrorMessages.Count != 0)
		{
			if (dicOwnerErrorMessages.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_ZIP))
			{
				var wsOwnerZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sOwnerZipError");
				wsOwnerZipError.InnerText = string.Empty;
			}

			// カスタムバリデータ取得
			m_customValidatorList = new List<CustomValidator>();
			CreateCustomValidators(this.Page, m_customValidatorList);

			// エラーをカスタムバリデータへ
			if (this.BlockErrorDisplay == false)
			{
				SetControlViewsForError(validationName, dicOwnerErrorMessages, m_customValidatorList);
			}

			// メールアドレス必須チェック
			ChangeControlLooksForValidator(
				dicOwnerErrorMessages,
				Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_for_check",
				wcvOwnerMailAddrForCheck,
				wtbOwnerMailAddr);

			ChangeControlLooksForValidator(
				dicOwnerErrorMessages,
				Constants.FIELD_ORDEROWNER_OWNER_BIRTH + "_for_check",
				wcvOwnerBirth,
				wddlOwnerBirthDay);

			// ユーザー拡張項目エラーメッセージをセット
			if (wdvUserPassword.Visible && wucUserExtend != null)
			{
				((UserExtendUserControl)wucUserExtend).SetErrMessage();
			}

			return false;
		}

		//------------------------------------------------------
		// ユーザ情報更新
		//------------------------------------------------------
		if (IsLoggedIn)
		{
			// モバイルメールアドレス入力欄が非表示の場合 ※V5.2以前の下位互換用
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false) || (Constants.EITHER_ENTER_MAIL_ADDRESS_ENABLED == false))
			{
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] = user.MailAddr2;
			}

			// 企業名、部署名入力欄が非表示の場合(法人OPがOFFの場合)
			if (Constants.DISPLAY_CORPORATION_ENABLED == false)
			{
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] = user.CompanyName;
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] = user.CompanyPostName;
			}

			var ownerForUpdateUser = (Hashtable)htOrderOwner.Clone();
			ownerForUpdateUser[Constants.FIELD_USER_MAIL_FLG] = wcbOwnerMailFlg.Checked ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG;

			// 項目が消えていル場合の対応
			if (wtbOwnerName1.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME] = user.Name;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME1] = user.Name1;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME2] = user.Name2;
			}
			if (wtbOwnerNameKana1.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] = user.NameKana;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] = user.NameKana1;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] = user.NameKana2;
			}
			if (wddlOwnerBirthYear.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] = user.Birth.HasValue ? user.Birth.Value.ToString("yyyy/MM/dd") : null;
			}
			if (wrblOwnerSex.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_SEX] = user.Sex;
			}
			if (wtbOwnerMailAddr.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] = user.MailAddr;

				if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) && UserService.IsPcSiteOrOfflineUser(ownerKbn))
				{
					ownerForUpdateUser[Constants.FIELD_USER_LOGIN_ID] = user.MailAddr;
					ownerForUpdateUser[Constants.FIELD_USER_LOGIN_ID + "_use_mailaddress_flg"] = StringUtility.ToHankaku(user.MailAddr);
				}
			}
			if (wtbOwnerMailAddr2.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] = user.MailAddr2;
			}
			if ((wtbOwnerZip1.HasInnerControl == false)
				&& (wtbOwnerZip.HasInnerControl == false))
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = user.Zip;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1] = user.Zip1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2] = user.Zip2;
			}
			if (wddlOwnerAddr1.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR1] = user.Addr1;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR2] = user.Addr2;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR3] = user.Addr3;
			}
			if (wtbOwnerAddr4.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR4] = user.Addr4;
			}
			if (wtbOwnerCompanyName.InnerControl == null)
			{
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] = user.CompanyName;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] = user.CompanyName;
			}
			if (wtbOwnerCompanyPostName.InnerControl == null)
			{
				htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] = user.CompanyPostName;
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] = user.CompanyPostName;
			}
			if (isOwnerAddrJp
				&& (wtbOwnerTel1_1.HasInnerControl == false)
				&& (wtbOwnerTel1.HasInnerControl == false))
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL1] = user.Tel1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1] = user.Tel1_1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2] = user.Tel1_2;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3] = user.Tel1_3;
			}
			if ((isOwnerAddrJp == false) && (wtbOwnerTel1Global.InnerControl == null))
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL1] = user.Tel1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1] = user.Tel1_1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2] = user.Tel1_2;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3] = user.Tel1_3;
			}
			if ((wtbOwnerTel2_1.HasInnerControl == false)
				&& (wtbOwnerTel2.HasInnerControl == false))
			{
				ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL2] = user.Tel2;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1] = user.Tel2_1;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2] = user.Tel2_2;
				ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3] = user.Tel2_3;
			}
			if (wcbOwnerMailFlg.InnerControl == null)
			{
				ownerForUpdateUser[Constants.FIELD_USER_MAIL_FLG] = user.MailFlg;
			}

			// 生年月日有効？
			bool blIsValidBirth = (ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != null);
			DateTime? ownerBirth = (blIsValidBirth ? DateTime.Parse((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]) : (DateTime?)null);

			// どれか一つでも項目が変更されていた場合に、ユーザー情報を更新する。
			if (((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME] != user.Name)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME1] != user.Name1)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME2] != user.Name2)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] != user.NameKana)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] != user.NameKana1)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] != user.NameKana2)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != user.MailAddr)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != user.MailAddr2)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ZIP] != user.Zip)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1] != user.Zip1)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2] != user.Zip2)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR1] != user.Addr1)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR2] != user.Addr2)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR3] != user.Addr3)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR4] != user.Addr4)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR5] != user.Addr5)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME] != user.AddrCountryName)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE] != user.AddrCountryIsoCode)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] != user.CompanyName)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] != user.CompanyPostName)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL1] != user.Tel1)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1] != user.Tel1_1)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2] != user.Tel1_2)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3] != user.Tel1_3)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL2] != user.Tel2)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1] != user.Tel2_1)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2] != user.Tel2_2)
				|| ((string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3] != user.Tel2_3)
				|| ((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_SEX] != user.Sex)
				|| (ownerBirth != user.Birth)
				|| ((string)ownerForUpdateUser[Constants.FIELD_USER_MAIL_FLG] != user.MailFlg)
				|| (Request.ServerVariables["REMOTE_ADDR"] != user.RemoteAddr)
				|| this.IsEasyUser)
			{
				// ユーザ情報を注文者情報で更新する
				user.Name = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME];
				user.Name1 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME1];
				user.Name2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME2];
				user.NameKana = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA];
				user.NameKana1 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1];
				user.NameKana2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2];
				user.MailAddr = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
				user.MailAddr2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
				user.Zip = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ZIP];
				user.Zip1 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1];
				user.Zip2 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2];
				user.Addr1 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR1];
				user.Addr2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR2];
				user.Addr3 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR3];
				user.Addr4 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR4];
				user.Addr5 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR5];
				user.AddrCountryIsoCode = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE];
				user.AddrCountryName = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME];
				user.CompanyName = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME];
				user.CompanyPostName = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME];
				user.Tel1 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL1];
				user.Tel1_1 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1];
				user.Tel1_2 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2];
				user.Tel1_3 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3];
				user.Tel2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_TEL2];
				user.Tel2_1 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1];
				user.Tel2_2 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2];
				user.Tel2_3 = (string)ownerForUpdateUser[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3];
				user.Sex = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_SEX];
				user.Birth = (blIsValidBirth) ? DateTime.Parse((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]) : (DateTime?)null;
				user.BirthYear = (blIsValidBirth) ? DateTime.Parse((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]).Year.ToString() : "";
				user.BirthMonth = (blIsValidBirth) ? DateTime.Parse((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]).Month.ToString() : "";
				user.BirthDay = (blIsValidBirth) ? DateTime.Parse((string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]).Day.ToString() : "";
				// メールアドレスのログインID利用時 かつ PCサイトユーザーの場合は、入力メールアドレスでログインIDを更新
				if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED && UserService.IsPcSiteOrOfflineUser(ownerKbn))
				{
					user.LoginId = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
				}
				user.MailFlg = (string)ownerForUpdateUser[Constants.FIELD_USER_MAIL_FLG];
				user.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;
				user.RemoteAddr = Request.ServerVariables["REMOTE_ADDR"];
				user.LastChanged = Constants.FLG_LASTCHANGED_USER;

				user.Addr = ConcatenateAddress(user);

				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					var result = new CrossPointUserApiService().Update(user);

					if (result.IsSuccess == false)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
							w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}

				// ユーザー情報更新
				userService.UpdateWithUserExtend(user, UpdateHistoryAction.DoNotInsert);

				if (Constants.PAYMENT_GMO_POST_ENABLED)
				{
					// Update Gmo information
					UpdateUserBusinessOwner(user);
				}

				var updatedUser = userService.Get(this.LoginUserId);

				// ユーザー情報の登録イベント
				if (Constants.USER_COOPERATION_ENABLED)
				{
					var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
					userCooperationPlugin.Update(updatedUser);
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertForUser(user.UserId, user.LastChanged);

				//------------------------------------------------------
				// セッション情報を書き換え（再ログイン処理は行わない）
				//------------------------------------------------------
				this.LoginUser = updatedUser;
				this.LoginUserName = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME1]
					+ (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_NAME2];
				this.LoginUserMail = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
				this.LoginUserMail2 = (string)ownerForUpdateUser[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
				if (user.Birth.HasValue)
				{
					this.LoginUserBirth = user.Birth.Value.ToString("yyyy/MM/dd");
				}
				this.LoginUserEasyRegisterFlg = user.EasyRegisterFlg;

				// Update Address Country Iso Code After Update User
				this.LoginUser.AddrCountryIsoCode = user.AddrCountryIsoCode;
			}
		}

		//------------------------------------------------------
		// 注文者情報セット
		//------------------------------------------------------
		this.CartList.Owner.UpdateOrderOwner(
			ownerKbn,
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME1],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME2],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR1],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR4],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR5],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3],
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2],
			(string)htOrderOwner[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3],
			((string)htOrderOwner[Constants.FIELD_USER_MAIL_FLG] == Constants.FLG_USER_MAILFLG_OK),
			(string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_SEX],
			(htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != null) ?
				DateTime.Parse((string)htOrderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]) : (DateTime?)null,
			RegionManager.GetInstance().Region.CountryIsoCode,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId,
			RegionManager.GetInstance().Region.CurrencyCode,
			RegionManager.GetInstance().Region.CurrencyLocaleId);

		return true;
	}

	/// <summary>
	/// 配送情報入力画面 送り主情報セット（ギフトのみ）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	public bool lbNext_Click_OrderShipping_ShippingSender(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 配送先 送り主情報設定１（住所）
		//------------------------------------------------------
		bool blHasError = false;
		CartShipping cartShippingFirst = null;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			CartObject cartObjectCurrent = this.CartList.Items[riCart.ItemIndex];

			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			for (int iShippingIndex = 0; (iShippingIndex < wrCartShippings.Items.Count) || ((iShippingIndex == 0) && (wrCartShippings.Items.Count == 0)); iShippingIndex++)
			{
				RepeaterItem riTarget = (wrCartShippings.Items.Count != 0) ? wrCartShippings.Items[iShippingIndex] : riCart;	// 配送先ループのある場合と無い場合を共通化したい

				#region ラップ済みコントロール宣言
				var wcbSameSenderAsShipping1 = GetWrappedControl<WrappedCheckBox>(riTarget, "cbSameSenderAsShipping1");
				var wrblSenderSelector = GetWrappedControl<WrappedRadioButtonList>(riTarget, "rblSenderSelector");
				var wtbSenderName1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderName1");
				var wtbSenderName2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderName2");
				var wtbSenderNameKana1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderNameKana1");
				var wtbSenderNameKana2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderNameKana2");
				var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderZipGlobal");
				var wtbSenderZip1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderZip1");
				var wtbSenderZip2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderZip2");
				var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlSenderCountry");
				var wddlSenderAddr1 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlSenderAddr1");
				var wtbSenderAddr2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderAddr2");
				var wtbSenderAddr3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderAddr3");
				var wtbSenderAddr4 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderAddr4");
				var wddlSenderAddr2 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlSenderAddr2");
				var wddlSenderAddr3 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlSenderAddr3");
				var wddlSenderAddr5 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlSenderAddr5");
				var wtbSenderAddr5 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderAddr5");
				var wtbSenderCompanyName = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderCompanyName");
				var wtbSenderCompanyPostName = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderCompanyPostName");
				var wtbSenderTel1_1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderTel1_1");
				var wtbSenderTel1_2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderTel1_2");
				var wtbSenderTel1_3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderTel1_3");
				var wtbSenderTel1Global = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderTel1Global");
				var wtbSenderZip = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderZip");
				var wtbSenderTel1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbSenderTel1");
				#endregion

				CartShipping cartShippingTemp = new CartShipping(this.CartList.Items[riCart.ItemIndex]);

				// 配送先1と同じ配送先？
				if ((iShippingIndex != 0) && (wcbSameSenderAsShipping1.Checked))
				{
					cartShippingTemp.UpdateSenderAddr(
						cartShippingFirst.SenderName1,
						cartShippingFirst.SenderName2,
						cartShippingFirst.SenderNameKana1,
						cartShippingFirst.SenderNameKana2,
						cartShippingFirst.SenderZip,
						cartShippingFirst.SenderZip1,
						cartShippingFirst.SenderZip2,
						cartShippingFirst.SenderAddr1,
						cartShippingFirst.SenderAddr2,
						cartShippingFirst.SenderAddr3,
						cartShippingFirst.SenderAddr4,
						cartShippingFirst.SenderAddr5,
						cartShippingFirst.SenderCountryIsoCode,
						cartShippingFirst.SenderCountryName,
						cartShippingFirst.SenderCompanyName,
						cartShippingFirst.SenderCompanyPostName,
						cartShippingFirst.SenderTel1,
						cartShippingFirst.SenderTel1_1,
						cartShippingFirst.SenderTel1_2,
						cartShippingFirst.SenderTel1_3,
						true,	// カート1と同じ配送先か
						cartShippingFirst.SenderAddrKbn);
				}
				// 注文者情報？
				else if (wrblSenderSelector.SelectedValue == CartShipping.AddrKbn.Owner.ToString())
				{
					cartShippingTemp.UpdateSenderAddr(this.CartList.Owner, false);
				}
				// 新規入力？
				else if (wrblSenderSelector.SelectedValue == CartShipping.AddrKbn.New.ToString())
				{
					//------------------------------------------------------
					// 送り主･入力チェック
					//------------------------------------------------------
					// データ格納
					Hashtable htOrderSender = new Hashtable();
					var isSenderAddrJp = IsCountryJp(wddlSenderCountry.SelectedValue);
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderName1.Text + wtbSenderName2.Text, isSenderAddrJp));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME1, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderName1.Text, isSenderAddrJp));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME2, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderName2.Text, isSenderAddrJp));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA, StringUtility.ToZenkaku(wtbSenderNameKana1.Text + wtbSenderNameKana2.Text));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1, StringUtility.ToZenkaku(wtbSenderNameKana1.Text));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2, StringUtility.ToZenkaku(wtbSenderNameKana2.Text));
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1, wddlSenderAddr1.SelectedValue);
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr2.Text, isSenderAddrJp).Trim());
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr3.Text, isSenderAddrJp).Trim());
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4, DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr4.Text, isSenderAddrJp).Trim());
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME, wtbSenderCompanyName.Text);
					htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME, wtbSenderCompanyPostName.Text);

					var senderAddrCountryName = Constants.GLOBAL_OPTION_ENABLE ? wddlSenderCountry.SelectedText : "";
					var senderAddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE ? wddlSenderCountry.SelectedValue : "";

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME + "_for_check", senderAddrCountryName);
					}
					if (isSenderAddrJp)
					{
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = senderAddrCountryName;
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = senderAddrCountryIsoCode;

						// Set value for zip code
						var inputZipCode = (wtbSenderZip1.HasInnerControl)
							? StringUtility.ToHankaku(wtbSenderZip1.Text.Trim())
							: StringUtility.ToHankaku(wtbSenderZip.Text.Trim());
						if (wtbSenderZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(wtbSenderZip2.Text.Trim()));
						var zipCode = new ZipCode(inputZipCode);
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = zipCode.Zip;
						htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_1] = zipCode.Zip1;
						htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_2] = zipCode.Zip2;

						// Set value for telephone
						var inputTel = (wtbSenderTel1_1.HasInnerControl)
							? StringUtility.ToHankaku(wtbSenderTel1_1.Text.Trim())
							: StringUtility.ToHankaku(wtbSenderTel1.Text.Trim());
						if (wtbSenderTel1_1.HasInnerControl)
						{
							inputTel = UserService.CreatePhoneNo(
								inputTel,
								StringUtility.ToHankaku(wtbSenderTel1_2.Text.Trim()),
								StringUtility.ToHankaku(wtbSenderTel1_3.Text.Trim()));
						}
						var tel = new Tel(inputTel);
						htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_1] = tel.Tel1;
						htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_2] = tel.Tel2;
						htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_3] = tel.Tel3;
						htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5, string.Empty);
					}
					else
					{
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = senderAddrCountryName;
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = senderAddrCountryIsoCode;

						htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ZIP, StringUtility.ToHankaku(wtbSenderZipGlobal.Text));
						htOrderSender.Add(CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_1, string.Empty);
						htOrderSender.Add(CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_2, string.Empty);
						htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_TEL1, StringUtility.ToHankaku(wtbSenderTel1Global.Text));
						htOrderSender.Add(CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_1, string.Empty);
						htOrderSender.Add(CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_2, string.Empty);
						htOrderSender.Add(CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_3, string.Empty);

						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = string.Empty;
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1] = string.Empty;
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2] = string.Empty;

						var addr5 = IsCountryUs(senderAddrCountryIsoCode)
							? wddlSenderAddr5.SelectedText
							: DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr5.Text, isSenderAddrJp).Trim();
						htOrderSender.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5, addr5);

						var addr2 = IsCountryTw(senderAddrCountryIsoCode) && (string.IsNullOrEmpty(wddlSenderAddr2.SelectedText) == false)
							? wddlSenderAddr2.SelectedText
							: DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr2.Text, isSenderAddrJp).Trim();
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = addr2;

						var addr3 = IsCountryTw(senderAddrCountryIsoCode) && (string.IsNullOrEmpty(wddlSenderAddr3.SelectedText) == false)
							? wddlSenderAddr3.SelectedText
							: DataInputUtility.ConvertToFullWidthBySetting(wtbSenderAddr3.Text, isSenderAddrJp).Trim();
						htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = addr3;
					}

					// エラーチェック＆カスタムバリデータへセット
					var validateName = isSenderAddrJp ? "OrderShipping" : "OrderShippingGlobal";
					var dicShippingErrorMessages = Validator.ValidateAndGetErrorContainer(
						validateName,
						htOrderSender,
						Constants.GLOBAL_OPTION_ENABLE ? senderAddrCountryIsoCode : "");
					if (dicShippingErrorMessages.Count != 0)
					{
						// カスタムバリデータ取得
						List<CustomValidator> lCustomValidators = new List<CustomValidator>();
						CreateCustomValidators(this.Page.Form.FindControl(riTarget.UniqueID), lCustomValidators);

						// エラーをカスタムバリデータへ
						if (this.BlockErrorDisplay == false)
						{
							SetControlViewsForError(validateName, dicShippingErrorMessages, lCustomValidators);
						}

						blHasError = true;
					}
					else
					{
						cartShippingTemp.UpdateSenderAddr(
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP],
							(string)htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_1],
							(string)htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_2],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME],
							(string)htOrderSender[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1],
							(string)htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_1],
							(string)htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_2],
							(string)htOrderSender[CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_3],
							false,
							(CartShipping.AddrKbn)Enum.Parse(typeof(CartShipping.AddrKbn), wrblSenderSelector.SelectedValue));
					}
				}

				//------------------------------------------------------
				// カート配送先セット＆カート１の配送先保存
				//------------------------------------------------------
				cartObjectCurrent.Shippings[riTarget.ItemIndex].UpdateSenderAddr(cartShippingTemp, cartShippingTemp.IsSameSenderAsShipping1);

				// カート毎・配送先１の送り主情報を保持 ※配送先1と同じ送り主を指定する
				if (iShippingIndex == 0)
				{
					cartShippingFirst = cartShippingTemp;
				}
			}
		}

		return (blHasError == false);
	}

	/// <summary>
	///  配送情報入力画面次へ画面クリック（Amazon注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public bool lbNext_Click_OrderShipping_AmazonOwner(object sender, System.EventArgs e)
	{
		// ログイン済みの場合は通常の共通処理を呼び出す
		if (this.IsLoggedIn || IsTargetToExtendedAmazonAddressManagerOption())
		{
			// グローバルオプションONの場合の処理
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry");
				wddlOwnerCountry.SelectItemByText("Japan");
				wddlOwnerCountry.SelectedValue = Constants.COUNTRY_ISO_CODE_JP;
			}
			return lbNext_Click_OrderShipping_Owner(sender, e);
		}

		// セッションにエラーメッセージがあればエラー
		var amazonOwnerAddressErrorMesage = (string)Session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG];
		if (string.IsNullOrEmpty(amazonOwnerAddressErrorMesage) == false) return false;

		// セッションから注文者情報が取得できなければエラー
		var amazonOwner = (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS];
		if (amazonOwner == null) return false;

		#region ラップ済みコントロール宣言
		var wcbOwnerMailFlg = this.IsLoggedIn
			? GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg", false)
			: Constants.AMAZON_PAYMENT_CV2_ENABLED
				? GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbGuestOwnerMailFlg2", false)
			: GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbGuestOwnerMailFlg", false);
		var wtbOwnerName1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName1", string.Empty);
		var wtbOwnerName2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName2", string.Empty);
		var wtbOwnerNameKana1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana1", string.Empty);
		var wtbOwnerNameKana2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana2", string.Empty);
		var wddlOwnerBirthYear = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthYear", string.Empty);
		var wddlOwnerBirthMonth = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthMonth", string.Empty);
		var wddlOwnerBirthDay = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthDay", string.Empty);
		var wrblOwnerSex = GetWrappedControl<WrappedRadioButtonList>(this.FirstRpeaterItem, "rblOwnerSex", string.Empty);
		var wtbOwnerMailAddr = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr", string.Empty);
		var wtbOwnerMailAddrConf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf", string.Empty);
		var wtbOwnerMailAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr2", string.Empty);
		var wtbOwnerMailAddrConf2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf2", string.Empty);
		var wtbOwnerZip = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip", string.Empty);
		var wddlOwnerAddr1 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr1", string.Empty);
		var wtbOwnerAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr2", string.Empty);
		var wtbOwnerAddr3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr3", string.Empty);
		var wtbOwnerAddr4 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr4", string.Empty);
		var wtbOwnerCompanyName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyName", string.Empty);
		var wtbOwnerCompanyPostName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyPostName", string.Empty);
		var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1", string.Empty);
		var wtbOwnerTel2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2", string.Empty);
		#endregion

		// コントロールから値の取得
		var ownerKbn = this.IsSmartPhone
			? Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST
			: Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST;
		var mailFlg = wcbOwnerMailFlg.Checked;
		var companyName = wtbOwnerCompanyName.Text.Trim();
		var companyPostName = wtbOwnerCompanyPostName.Text.Trim();
		var mailAddr = amazonOwner.MailAddr;
		var name = string.IsNullOrEmpty(amazonOwner.Name1 + amazonOwner.Name2)
			? amazonOwner.Name
			: amazonOwner.Name1 + amazonOwner.Name2; ;
		var name1 = string.IsNullOrEmpty(amazonOwner.Name1 + amazonOwner.Name2)
			? amazonOwner.Name
			: amazonOwner.Name1;
		var name2 = StringUtility.ToEmpty(amazonOwner.Name2);
		var nameKana = StringUtility.ToEmpty(amazonOwner.NameKana);
		var nameKana1 = StringUtility.ToEmpty(amazonOwner.NameKana1);
		var nameKana2 = StringUtility.ToEmpty(amazonOwner.NameKana2);
		var nullrableBirth = (DateTime?)null;
		var sex = string.Empty;
		var joinedTel = StringUtility.ToEmpty(amazonOwner.Tel);
		var tel1 = StringUtility.ToEmpty(amazonOwner.Tel1);
		var tel2 = StringUtility.ToEmpty(amazonOwner.Tel2);
		var tel3 = StringUtility.ToEmpty(amazonOwner.Tel3);
		var joinedZip = StringUtility.ToEmpty(amazonOwner.Zip);
		var zip1 = StringUtility.ToEmpty(amazonOwner.Zip1);
		var zip2 = StringUtility.ToEmpty(amazonOwner.Zip2);
		var addr1 = StringUtility.ToEmpty(amazonOwner.Addr1);
		var addr2 = StringUtility.ToEmpty(amazonOwner.Addr2);
		var addr3 = StringUtility.ToEmpty(amazonOwner.Addr3);
		var addr4 = StringUtility.ToEmpty(amazonOwner.Addr4);

		// 注文者情報セット
		this.CartList.Owner.UpdateOrderOwner(
			ownerKbn,
			name,
			name1,
			name2,
			nameKana,
			nameKana1,
			nameKana2,
			mailAddr,
			string.Empty,
			joinedZip,
			zip1,
			zip2,
			addr1,
			addr2,
			addr3,
			addr4,
			string.Empty,
			Constants.GLOBAL_OPTION_ENABLE ? Constants.COUNTRY_ISO_CODE_JP : string.Empty,
			Constants.GLOBAL_OPTION_ENABLE ? "Japan" : string.Empty,
			companyName,
			companyPostName,
			joinedTel,
			tel1,
			tel2,
			tel3,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			mailFlg,
			sex,
			nullrableBirth,
			RegionManager.GetInstance().Region.CountryIsoCode,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId,
			RegionManager.GetInstance().Region.CurrencyCode,
			RegionManager.GetInstance().Region.CurrencyLocaleId);

		return true;
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	public bool lbNext_Click_OrderShipping_Shipping(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 配送先情報設定１（住所）
		//------------------------------------------------------
		bool blHasError = false;
		CartShipping cartShippingFirst = null;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			CartObject cartObjectCurrent = this.CartList.Items[riCart.ItemIndex];

			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			for (int iShippingIndex = 0; (iShippingIndex < wrCartShippings.Items.Count) || ((iShippingIndex == 0) && (wrCartShippings.Items.Count == 0)); iShippingIndex++)
			{
				RepeaterItem riTarget = (wrCartShippings.Items.Count != 0) ? wrCartShippings.Items[iShippingIndex] : riCart;	// 配送先ループのある場合と無い場合を共通化したい

				#region ラップ済みコントロール宣言
				var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(riTarget, "cbShipToCart1Address");
				var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingKbnList");
				var wtbUserShippingName = GetWrappedControl<WrappedTextBox>(riTarget, "tbUserShippingName");
				var wtbShippingName1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingName1");
				var wtbShippingName2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingName2");
				var wtbShippingNameKana1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingNameKana1");
				var wtbShippingNameKana2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingNameKana2");
				var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingZip1");
				var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingZip2");
				var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingAddr1");
				var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingAddr2");
				var wtbShippingAddr3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingAddr3");
				var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingAddr4");
				var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingCompanyName");
				var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingCompanyPostName");
				var wtbShippingTel1_1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingTel1_1");
				var wtbShippingTel1_2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingTel1_2");
				var wtbShippingTel1_3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingTel1_3");
				var wrblSaveToUserShipping = GetWrappedControl<WrappedRadioButtonList>(riTarget, "rblSaveToUserShipping");
				var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingCountry");
				var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingZipGlobal");
				var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingAddr2");
				var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingAddr3");
				var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingAddr5");
				var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingAddr5");
				var wtbShippingTel1Global = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingTel1Global");
				var wddlUniformInvoiceType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceType");
				var wddlUniformInvoiceTypeOption = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceTypeOption");
				var wtbUniformInvoiceOption1_8 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption1_8");
				var wtbUniformInvoiceOption1_3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption1_3");
				var wtbUniformInvoiceOption2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption2");
				var wtbUniformInvoiceTypeName = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceTypeName");
				var wcbSaveToUserInvoice = GetWrappedControl<WrappedCheckBox>(riTarget, "cbSaveToUserInvoice");
				var wddlInvoiceCarryType = GetWrappedControl<WrappedDropDownList>(riCart, "ddlInvoiceCarryType");
				var wtbCarryTypeOption_8 = GetWrappedControl<WrappedTextBox>(riCart, "tbCarryTypeOption_8");
				var wtbCarryTypeOption_16 = GetWrappedControl<WrappedTextBox>(riCart, "tbCarryTypeOption_16");
				var wtbCarryTypeOptionName = GetWrappedControl<WrappedTextBox>(riCart, "tbCarryTypeOptionName");
				var wddlInvoiceCarryTypeOption = GetWrappedControl<WrappedDropDownList>(riCart, "ddlInvoiceCarryTypeOption");
				var wcbCarryTypeOptionRegist = GetWrappedControl<WrappedCheckBox>(riTarget, "cbCarryTypeOptionRegist");
				var wlbCarryTypeOption = GetWrappedControl<WrappedLabel>(riCart, "lbCarryTypeOption");
				var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(riTarget, "hfCvsShopId");
				var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(riTarget, "hfCvsShopName");
				var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(riTarget, "hfCvsShopAddress");
				var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(riTarget, "hfCvsShopTel");
				var lCvsShopId = GetWrappedControl<WrappedLiteral>(riTarget, "lCvsShopId");
				var wlShopIdErrorMessage = GetWrappedControl<WrappedLiteral>(riTarget, "lShippingCountryErrorMessage");
				var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingReceivingStoreType");
				var wtbShippingZip = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingZip");
				var wtbShippingTel1 = GetWrappedControl<WrappedTextBox>(riTarget, "tbShippingTel1");
				var wddlRealShopArea = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlRealShopArea");
				var wddlRealShopName = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlRealShopName");
				var wlRealShopZip = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopZip");
				var wlRealShopAddr1 = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopAddr1");
				var wlRealShopAddr2 = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopAddr2");
				var wlRealShopAddr3 = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopAddr3");
				var wlRealShopAddr4 = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopAddr4");
				var wlRealShopAddr5 = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopAddr5");
				var wlRealShopOpenningHours = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopOpenningHours");
				var wlRealShopTel = GetWrappedControl<WrappedLiteral>(riTarget, "lRealShopTel");
				var wlStorePickupInvalidMessage = GetWrappedControl<WrappedLiteral>(riTarget, "lStorePickupInvalidMessage");
				var wlShipToCart1AddressInvalidMessage = GetWrappedControl<WrappedLiteral>(riTarget, "lShipToCart1AddressInvalidMessage");
				#endregion

				// 配送先にエラーがある場合、次のページへ遷移できない
				if (this.HasUserShippingError) return false;

				var cartShippingTemp = new CartShipping(this.CartList.Items[riCart.ItemIndex]);
				var shippingAddrCountryIsoCode = wddlShippingCountry.SelectedValue;
				var isShippingAddrJp = IsCountryJp(shippingAddrCountryIsoCode);

				// エラーチェック＆カスタムバリデータへセット
				var validationName = this.IsLoggedIn
					? isShippingAddrJp ? "OrderShipping" : "OrderShippingGlobal"
					: isShippingAddrJp ? "OrderShippingGuest" : "OrderShippingGuestGlobal";

				// カート1と同じ配送先？
				if ((riCart.ItemIndex != 0) && (wcbShipToCart1Address.Checked))
				{
					cartShippingTemp.UpdateShippingAddr(
						cartShippingFirst.Name1,
						cartShippingFirst.Name2,
						cartShippingFirst.NameKana1,
						cartShippingFirst.NameKana2,
						cartShippingFirst.Zip,
						cartShippingFirst.Zip1,
						cartShippingFirst.Zip2,
						cartShippingFirst.Addr1,
						cartShippingFirst.Addr2,
						cartShippingFirst.Addr3,
						cartShippingFirst.Addr4,
						cartShippingFirst.Addr5,
						cartShippingFirst.ShippingCountryIsoCode,
						cartShippingFirst.ShippingCountryName,
						cartShippingFirst.CompanyName,
						cartShippingFirst.CompanyPostName,
						cartShippingFirst.Tel1,
						cartShippingFirst.Tel1_1,
						cartShippingFirst.Tel1_2,
						cartShippingFirst.Tel1_3,
						true,	// カート1と同じ配送先か
						cartShippingFirst.ShippingAddrKbn,
						cartShippingFirst.RealShopId);

					// Set Convenience Store Addr to Cart Shipping Temp
					if (this.CartList.Items[0].GetShipping().ConvenienceStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					{
						cartShippingTemp.UpdateConvenienceStoreAddr(
							cartShippingFirst.ShippingAddrKbn,
							cartShippingFirst.ConvenienceStoreId,
							cartShippingFirst.Name1,
							cartShippingFirst.Addr4,
							cartShippingFirst.Tel1,
							cartShippingFirst.ShippingReceivingStoreType);
						cartShippingTemp.IsSameShippingAsCart1 = true;
					}

					if(this.CartList.Items[0].GetShipping().ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
					{
						var storePickupInvalidMessage = CheckProductBuyStorePickup(riCart.ItemIndex);

						if(string.IsNullOrEmpty(storePickupInvalidMessage) == false)
						{
							wlShipToCart1AddressInvalidMessage.Visible = true;
							wlShipToCart1AddressInvalidMessage.Text = storePickupInvalidMessage;
							blHasError = true;
						}
					}
				}
				// 注文者情報の住所へ配送する？
				else if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
				{
					cartShippingTemp.UpdateShippingAddr(this.CartList.Owner, false);
					cartShippingTemp.ConvenienceStoreFlg
						= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				}
				// 配送先を新規入力する？
				else if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
				{
					cartShippingTemp.ShippingAddrKbn = wddlShippingKbnList.SelectedValue;
					//------------------------------------------------------
					// 配送情報取得･入力チェック
					//------------------------------------------------------
					// データ格納
					Hashtable htOrderShipping = new Hashtable();
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingName1.Text + wtbShippingName2.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingName1.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingName2.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA, StringUtility.ToZenkaku(wtbShippingNameKana1.Text + wtbShippingNameKana2.Text).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, StringUtility.ToZenkaku(wtbShippingNameKana1.Text).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, StringUtility.ToZenkaku(wtbShippingNameKana2.Text).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, wddlShippingAddr1.SelectedValue);
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr2.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr3.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr4.Text, isShippingAddrJp).Trim());
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME, wtbShippingCompanyName.Text);
					htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME, wtbShippingCompanyPostName.Text);

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME + "_for_check", wddlShippingCountry.SelectedText);
					}

					// グローバルOPがOFFまたは配送先が日本
					if (isShippingAddrJp)
					{
						// Set value for zip
						var inputZipCode = (wtbShippingZip1.HasInnerControl)
							? StringUtility.ToHankaku(wtbShippingZip1.Text.Trim())
							: StringUtility.ToHankaku(wtbShippingZip.Text.Trim());
						if (wtbShippingZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(wtbShippingZip2.Text.Trim()));
						var zipCode = new ZipCode(inputZipCode);
						htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = (string.IsNullOrEmpty(zipCode.Zip) == false)
							? zipCode.Zip
							: inputZipCode;
						htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1] = StringUtility.ToHankaku(zipCode.Zip1);
						htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2] = StringUtility.ToHankaku(zipCode.Zip2);

						// Set value for telephone
						var inputTel = (wtbShippingTel1_1.HasInnerControl)
							? StringUtility.ToHankaku(wtbShippingTel1_1.Text.Trim())
							: StringUtility.ToHankaku(wtbShippingTel1.Text.Trim());
						if (wtbShippingTel1_1.HasInnerControl)
						{
							inputTel = UserService.CreatePhoneNo(
								inputTel,
								StringUtility.ToHankaku(wtbShippingTel1_2.Text.Trim()),
								StringUtility.ToHankaku(wtbShippingTel1_3.Text.Trim()));
						}
						var tel = new Tel(inputTel);
						htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1] = tel.Tel1;
						htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2] = tel.Tel2;
						htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3] = tel.Tel3;
						htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] =
							(string.IsNullOrEmpty(tel.TelNo) == false)
								? tel.TelNo
								: inputTel;

						if (Constants.GLOBAL_OPTION_ENABLE)
						{
							htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, shippingAddrCountryIsoCode);
							htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME, wddlShippingCountry.SelectedText);
						}
						else
						{
							htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, string.Empty);
							htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME, string.Empty);
						}
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5, string.Empty);
					}
					else
					{
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, StringUtility.ToHankaku(wtbShippingZipGlobal.Text));
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, StringUtility.ToHankaku(wtbShippingTel1Global.Text));

						if (IsCountryTw(shippingAddrCountryIsoCode)
							&& wddlShippingAddr2.HasInnerControl
							&& wddlShippingAddr3.HasInnerControl)
						{
							htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] = wddlShippingAddr2.SelectedText;
							htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] = wddlShippingAddr3.SelectedText;
						}

						var addr5 = IsCountryUs(shippingAddrCountryIsoCode)
							? wddlShippingAddr5.SelectedText
							: DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr5.Text, isShippingAddrJp).Trim();
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5, addr5);
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, shippingAddrCountryIsoCode);
						htOrderShipping.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME, wddlShippingCountry.SelectedText);
						htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = string.Empty;
						htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1] = string.Empty;
						htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2] = string.Empty;
					}

					// 配送先情報を保存する？
					if (wrblSaveToUserShipping.SelectedValue == "1")
					{
						htOrderShipping.Add(Constants.FIELD_USERSHIPPING_NAME, StringUtility.ToZenkaku(wtbUserShippingName.Text));
					}

					// エラーチェック＆カスタムバリデータへセット
					var dicShippingErrorMessages = Validator.ValidateAndGetErrorContainer(
						validationName,
						htOrderShipping,
						Constants.GLOBAL_OPTION_ENABLE ? shippingAddrCountryIsoCode : "");
					if (dicShippingErrorMessages.Count != 0)
					{
						if (dicShippingErrorMessages.ContainsKey(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP))
						{
							var wsShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sShippingZipError");
							wsShippingZipError.InnerText = string.Empty;
						}

						// カスタムバリデータ取得
						List<CustomValidator> lCustomValidators = new List<CustomValidator>();
						CreateCustomValidators(this.Page.Form.FindControl(riTarget.UniqueID), lCustomValidators);

						// エラーをカスタムバリデータへ
						if (this.BlockErrorDisplay == false)
						{
							SetControlViewsForError(validationName, dicShippingErrorMessages, lCustomValidators);
						}

						blHasError = true;

						// カート１と同じ配送先か
						cartShippingTemp.IsSameShippingAsCart1 = false;
					}
					else
					{
						cartShippingTemp.ConvenienceStoreFlg
							= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;

						// アドレス帳保存情報セット
						if (wrblSaveToUserShipping.SelectedValue == "1")
						{
							cartShippingTemp.UpdateUserShippingRegistSetting(
								true,
								wtbUserShippingName.Text);
						}
					}

					cartShippingTemp.UpdateShippingAddr(
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP],
						(string)htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1],
						(string)htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME],
						(string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1],
						(string)htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1],
						(string)htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2],
						(string)htOrderShipping[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3],
						false,	// カート1と同じ配送先か
						wddlShippingKbnList.SelectedValue);
				}
				// Convenience store
				else if (wddlShippingKbnList.SelectedValue
					== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				{
					// Set Convenience Store Addr to Cart Shipping Temp
					cartShippingTemp.UpdateConvenienceStoreAddr(
						wddlShippingKbnList.SelectedValue,
						StringUtility.ToEmpty(whfCvsShopId.Value),
						StringUtility.ToEmpty(whfCvsShopName.Value),
						StringUtility.ToEmpty(whfCvsShopAddress.Value),
						StringUtility.ToEmpty(whfCvsShopTel.Value),
						wddlShippingReceivingStoreType.SelectedValue);

					// アドレス帳保存情報セット
					if (wrblSaveToUserShipping.SelectedValue == "1")
					{
						cartShippingTemp.UpdateUserShippingRegistSetting(
							true,
							wtbUserShippingName.Text);
					}
				}
				// Store pickup
				else if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
				{
					cartShippingTemp.ShippingAddrKbn = wddlShippingKbnList.SelectedValue;

					var orderShippingInput = new Hashtable
					{
						{ Constants.FIELD_REALSHOP_REAL_SHOP_ID, StringUtility.ToEmpty(wddlRealShopName.SelectedValue) }
					};

					var dicShippingErrorMessages = Validator.ValidateAndGetErrorContainer(
						validationName,
						orderShippingInput,
						Constants.GLOBAL_OPTION_ENABLE ? shippingAddrCountryIsoCode : string.Empty);
					var storePickupInvalidMessage = CheckProductBuyStorePickup(iShippingIndex);
					if (dicShippingErrorMessages.Count != 0)
					{
						var lCustomValidators = new List<CustomValidator>();
						CreateCustomValidators(this.Page.Form.FindControl(riTarget.UniqueID), lCustomValidators);
						SetControlViewsForError(validationName, dicShippingErrorMessages, lCustomValidators);
						blHasError = true;
					}
					if (string.IsNullOrEmpty(storePickupInvalidMessage) == false)
					{
						if((riCart.ItemIndex != 0) && (wcbShipToCart1Address.Checked))
						{
							wddlShippingReceivingStoreType.Text = storePickupInvalidMessage;
						}
						else
						{
							wlStorePickupInvalidMessage.Text = storePickupInvalidMessage;
						}
						blHasError = true;
					}

					var realShopShipping = (string.IsNullOrEmpty(wddlRealShopName.SelectedValue))
						? new RealShopModel()
						: new RealShopService().Get(StringUtility.ToEmpty(wddlRealShopName.SelectedValue));
					
					// Save realshop selection to session
					var realShopSelection = new Hashtable
					{
						{ Constants.FIELD_REALSHOP_AREA_ID, StringUtility.ToEmpty(wddlRealShopArea.SelectedValue) },
						{ Constants.FIELD_REALSHOP_REAL_SHOP_ID, StringUtility.ToEmpty(wddlRealShopName.SelectedValue) }
					};

					Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] = realShopSelection;

					// Set store pickup addr to cart shipping temp
					cartShippingTemp.UpdateStorePickupShippingAddr(
						realShopShipping.AreaId,
						realShopShipping.RealShopId,
						realShopShipping.Name,
						realShopShipping.NameKana,
						realShopShipping.Zip,
						realShopShipping.Addr1,
						realShopShipping.Addr2,
						realShopShipping.Addr3,
						realShopShipping.Addr4,
						realShopShipping.Addr5,
						realShopShipping.OpeningHours,
						realShopShipping.Tel,
						realShopShipping.CountryIsoCode,
						realShopShipping.CountryName);
				}
				// 特定アドレス帳の住所へ配送する？
				else
				{
					var shippingNo = 0;
					if (int.TryParse(wddlShippingKbnList.SelectedValue, out shippingNo) == false) return false;

					var userShipping = new UserShippingService().Get(this.LoginUserId, shippingNo);
					if (userShipping == null) return false;

					cartShippingTemp.UpdateShippingAddr(
						userShipping.ShippingName1,
						userShipping.ShippingName2,
						userShipping.ShippingNameKana1,
						userShipping.ShippingNameKana2,
						userShipping.ShippingZip,
						userShipping.ShippingZip1,
						userShipping.ShippingZip2,
						userShipping.ShippingAddr1,
						userShipping.ShippingAddr2,
						userShipping.ShippingAddr3,
						userShipping.ShippingAddr4,
						userShipping.ShippingAddr5,
						userShipping.ShippingCountryIsoCode,
						userShipping.ShippingCountryName,
						userShipping.ShippingCompanyName,
						userShipping.ShippingCompanyPostName,
						userShipping.ShippingTel1,
						userShipping.ShippingTel1_1,
						userShipping.ShippingTel1_2,
						userShipping.ShippingTel1_3,
						false,	// カート1と同じ配送先か
						wddlShippingKbnList.SelectedValue);

					cartShippingTemp.ConvenienceStoreId = userShipping.ShippingReceivingStoreId;
					cartShippingTemp.ConvenienceStoreFlg = userShipping.ShippingReceivingStoreFlg;
					cartShippingTemp.ShippingReceivingStoreType = userShipping.ShippingReceivingStoreType;

					// Check Id Exists In Xml Store
					if ((userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& (OrderCommon.CheckIdExistsInXmlStore(userShipping.ShippingReceivingStoreId) == false)
						&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
					{
						wlShopIdErrorMessage.Text =
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE);
						blHasError = true;
					}

					cartShippingTemp.UpdateUserShippingRegistSetting(false, wddlShippingKbnList.SelectedItem.Text);
				}

				// Taiwan invoice enable
				if (OrderCommon.DisplayTwInvoiceInfo())
				{
					if (Constants.GIFTORDER_OPTION_ENABLED)
					{
						cartShippingTemp.UpdateInvoice(cartObjectCurrent.Shippings[iShippingIndex].UniformInvoiceType,
							cartObjectCurrent.Shippings[iShippingIndex].UniformInvoiceOption1,
							cartObjectCurrent.Shippings[iShippingIndex].UniformInvoiceOption2,
							cartObjectCurrent.Shippings[iShippingIndex].CarryType,
							cartObjectCurrent.Shippings[iShippingIndex].CarryTypeOptionValue);
					}
					else
					{
						cartShippingTemp.CarryType = wddlInvoiceCarryType.SelectedValue;
						cartShippingTemp.CarryTypeOption = wddlInvoiceCarryTypeOption.SelectedValue;
						cartShippingTemp.UniformInvoiceTypeOption = wddlUniformInvoiceTypeOption.SelectedValue;
						cartShippingTemp.UniformInvoiceType = wddlUniformInvoiceType.SelectedValue;
						if (string.IsNullOrEmpty(cartShippingTemp.UniformInvoiceTypeOption))
						{
							switch (cartShippingTemp.UniformInvoiceType)
							{
								case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
									cartShippingTemp.UniformInvoiceOption1 = wtbUniformInvoiceOption1_8.Text.Trim();
									cartShippingTemp.UniformInvoiceOption2 = wtbUniformInvoiceOption2.Text.Trim();
									break;

								case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
									cartShippingTemp.UniformInvoiceOption1 = wtbUniformInvoiceOption1_3.Text.Trim();
									cartShippingTemp.UniformInvoiceOption2 = string.Empty;
									break;

								default:
									cartShippingTemp.UniformInvoiceOption1 = string.Empty;
									cartShippingTemp.UniformInvoiceOption2 = string.Empty;
									if (string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption))
									{
										cartShippingTemp.CarryTypeOptionValue = ((cartShippingTemp.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
											? wtbCarryTypeOption_8.Text
											: wtbCarryTypeOption_16.Text);

										if (wcbCarryTypeOptionRegist.Checked)
										{
											cartShippingTemp.UpdateUserInvoiceRegistSetting(true, wtbCarryTypeOptionName.Text.Trim());
										}
									}
									else
									{
										cartShippingTemp.CarryTypeOptionValue = wlbCarryTypeOption.Text;
										cartShippingTemp.UpdateUserInvoiceRegistSetting(false, wddlInvoiceCarryTypeOption.SelectedText);
									}
									break;
							}

							if ((cartShippingTemp.UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
								&& wcbSaveToUserInvoice.Checked
								&& string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption))
							{
								cartShippingTemp.UpdateUserInvoiceRegistSetting(true, wtbUniformInvoiceTypeName.Text.Trim());
							}
						}
						else
						{
							var invoiceNo = int.Parse(wddlUniformInvoiceTypeOption.SelectedValue);
							var model = new TwUserInvoiceService().Get(this.LoginUserId, invoiceNo);

							if (model != null)
							{
								switch (model.TwUniformInvoice)
								{
									case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
										cartShippingTemp.UniformInvoiceOption1 = model.TwUniformInvoiceOption1;
										cartShippingTemp.UniformInvoiceOption2 = model.TwUniformInvoiceOption2;
										break;

									case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
										cartShippingTemp.UniformInvoiceOption1 = model.TwUniformInvoiceOption1;
										cartShippingTemp.UniformInvoiceOption2 = string.Empty;
										break;
								}

								cartShippingTemp.UpdateUserInvoiceRegistSetting(false, model.TwInvoiceName);
							}
						}

						// Validate uniform invoice
						var uniformInput = CreateUserInvoiceInput(cartShippingTemp);
						var uniformErrorMessages = Validator.ValidateAndGetErrorContainer(validationName, uniformInput);

						if ((cartShippingTemp.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
							&& string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption)
							&& (uniformErrorMessages.Count == 0))
						{
							// Validate carry type
							Regex regex = null;
							switch (cartShippingTemp.CarryType)
							{
								case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
									regex = new Regex(Constants.REGEX_MOBILE_CARRY_TYPE_OPTION_8);
									if (regex.IsMatch(wtbCarryTypeOption_8.Text.Trim()) == false)
									{
										uniformErrorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8"]
											= WebMessages.GetMessages(WebMessages.ERRMSG_MOBILE_CARRY_TYPE_OPTION_8);
									}
									break;

								case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
									regex = new Regex(Constants.REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16);
									if (regex.IsMatch(wtbCarryTypeOption_16.Text.Trim()) == false)
									{
										uniformErrorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16"]
											= WebMessages.GetMessages(WebMessages.ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16);
									}
									break;
							}
						}

						if (uniformErrorMessages.Count != 0)
						{
							// Custom Validator
							List<CustomValidator> lCustomValidators = new List<CustomValidator>();
							CreateCustomValidators(this.Page.Form.FindControl(riTarget.UniqueID), lCustomValidators);

							// Error Custom Validator
							SetControlViewsForError(validationName, uniformErrorMessages, lCustomValidators);

							blHasError = true;
						}
					}
				}

				//------------------------------------------------------
				// カート配送先セット＆カート１の配送先保存
				//------------------------------------------------------
				cartObjectCurrent.Shippings[iShippingIndex].UpdateShippingAddr(cartShippingTemp);
				cartObjectCurrent.Shippings[iShippingIndex].UpdateUserShippingRegistSetting(
					cartShippingTemp.UserShippingRegistFlg,
					cartShippingTemp.UserShippingName);

				// Set Convenience Store Addr to Cart Shipping Temp
				if (cartShippingTemp.ConvenienceStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				{
					cartObjectCurrent.Shippings[iShippingIndex].UpdateConvenienceStoreAddr(
						cartShippingTemp.ShippingAddrKbn,
						cartShippingTemp.ConvenienceStoreId,
						cartShippingTemp.Name1,
						cartShippingTemp.Addr4,
						cartShippingTemp.Tel1,
						cartShippingTemp.ShippingReceivingStoreType);
					cartObjectCurrent.Shippings[iShippingIndex]
						.IsSameShippingAsCart1 = cartShippingTemp.IsSameShippingAsCart1;
					cartObjectCurrent.Shippings[iShippingIndex].SpecifyShippingTimeFlg = false;
				}

				if (riCart.ItemIndex == 0)
				{
					cartShippingFirst = cartShippingTemp;
				}

				if (cartShippingTemp.ConvenienceStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				{
					var shippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;
					if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(
						cartObjectCurrent,
						shippingReceivingStoreType))
					{
						var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(shippingReceivingStoreType);
						var wlConvenienceStoreErrorMessage = GetWrappedControl<WrappedLiteral>(riTarget, "lConvenienceStoreErrorMessage");
						wlConvenienceStoreErrorMessage.Text =
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
								.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
								.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());

						var wlOrderErrorMessage = GetWrappedControl<WrappedLiteral>(((Control)sender), "lOrderErrorMessage");
						wlOrderErrorMessage.Text = (wcbShipToCart1Address.Checked)
							? wlConvenienceStoreErrorMessage.Text
							: wlOrderErrorMessage.Text.Replace(
								wlConvenienceStoreErrorMessage.Text,
								string.Empty);

						blHasError = true;
					}
				}
			}
			// 別出荷フラグを更新
			cartObjectCurrent.UpdateAnotherShippingFlag();
		}

		return (blHasError == false);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（Amazon配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	public bool lbNext_Click_OrderShipping_AmazonShipping(object sender, System.EventArgs e)
	{
		var riCart = this.WrCartList.Items[0];

		#region ラップ済みコントロール宣言
		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		var wcbShipToOwnerAddress = GetWrappedControl<WrappedCheckBox>(riCart, "cbShipToOwnerAddress", false);
		var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(riCart, "tbShippingCompanyName", "");
		var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(riCart, "tbShippingCompanyPostName", "");
		#endregion

		var cartObject = this.CartList.Items[0];
		var cartShipping = new CartShipping(cartObject);

		if ((wcbShipToOwnerAddress.InnerControl == null)
			|| ((wcbShipToOwnerAddress.Checked) && (this.IsLoggedIn == false)))
		{
			cartShipping.UpdateShippingAddr(this.CartList.Owner, true);
		}
		else
		{
			// セッションにエラーメッセージがあればエラー
			var amazonShippingAddressErrorMesage = (string)Session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG];
			if (string.IsNullOrEmpty(amazonShippingAddressErrorMesage) == false) return false;

			// セッションから配送先情報が取得できなければエラー
			var amazonShipping = (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS];
			if (amazonShipping == null) return false;

			var shippingCountryIsoCode = string.Empty;
			var shippingCountryName = string.Empty;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				shippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
				shippingCountryName = "Japan";
			}

			cartShipping.UpdateShippingAddr(
				amazonShipping.Name1,
				amazonShipping.Name2,
				amazonShipping.NameKana1,
				amazonShipping.NameKana2,
				amazonShipping.Zip,
				amazonShipping.Zip1,
				amazonShipping.Zip2,
				amazonShipping.Addr1,
				amazonShipping.Addr2,
				amazonShipping.Addr3,
				amazonShipping.Addr4,
				string.Empty,
				shippingCountryIsoCode,
				shippingCountryName,
				wtbShippingCompanyName.Text,
				wtbShippingCompanyPostName.Text,
				amazonShipping.Tel,
				amazonShipping.Tel1,
				amazonShipping.Tel2,
				amazonShipping.Tel3,
				false,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
		}

		// 配送先セット
		cartObject.Shippings[0].UpdateShippingAddr(cartShipping);

		// 別出荷フラグを更新
		cartObject.UpdateAnotherShippingFlag();

		return true;
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（その他情報）
	/// </summary>
	/// <param name="e"></param>
	public bool lbNext_Click_OrderShipping_Others(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 配送先情報設定１（住所）
		//------------------------------------------------------
		var hasError = false;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var hasCartError = false;
			var wrRepeaterMemo = GetWrappedControl<WrappedRepeater>(riCart, "rMemos");

			CartObject cartObjectCurrent = this.CartList.Items[riCart.ItemIndex];

			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			for (int iShippingIndex = 0; (iShippingIndex < wrCartShippings.Items.Count) || ((iShippingIndex == 0) && (wrCartShippings.Items.Count == 0)); iShippingIndex++)
			{
				RepeaterItem riTarget = (wrCartShippings.Items.Count != 0) ? wrCartShippings.Items[iShippingIndex] : riCart;	// 配送先ループのある場合と無い場合を共通化したい

				#region ラップ済みコントロール宣言
				var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingDate");
				var wdtShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingTime");
				var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
				var wddlShippingTime = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingTime");
				var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(riTarget);
				var wddlDeliveryCompany = CreateWrappedDropDownListDeliveryCompany(riTarget);
				var wddlUniformInvoiceType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceType");
				var wddlUniformInvoiceTypeOption = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlUniformInvoiceTypeOption");
				var wtbUniformInvoiceOption1_8 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption1_8");
				var wtbUniformInvoiceOption1_3 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption1_3");
				var wtbUniformInvoiceOption2 = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceOption2");
				var wtbUniformInvoiceTypeName = GetWrappedControl<WrappedTextBox>(riTarget, "tbUniformInvoiceTypeName");
				var wcbSaveToUserInvoice = GetWrappedControl<WrappedCheckBox>(riTarget, "cbSaveToUserInvoice");
				var wddlInvoiceCarryType = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlInvoiceCarryType");
				var wtbCarryTypeOption_8 = GetWrappedControl<WrappedTextBox>(riTarget, "tbCarryTypeOption_8");
				var wtbCarryTypeOption_16 = GetWrappedControl<WrappedTextBox>(riTarget, "tbCarryTypeOption_16");
				var wtbCarryTypeOptionName = GetWrappedControl<WrappedTextBox>(riTarget, "tbCarryTypeOptionName");
				var wddlInvoiceCarryTypeOption = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlInvoiceCarryTypeOption");
				var wcbCarryTypeOptionRegist = GetWrappedControl<WrappedCheckBox>(riTarget, "cbCarryTypeOptionRegist");
				var wlbCarryTypeOption = GetWrappedControl<WrappedLabel>(riTarget, "lbCarryTypeOption");
				#endregion

				CartShipping cartShippingTemp = new CartShipping(cartObjectCurrent);

				string strValidationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";

				//------------------------------------------------------
				// 配送先情報設定２-(1)（配送希望日など）
				//------------------------------------------------------
				var isUseFirstShippingDate = this.ShopShippingList[riCart.ItemIndex].FixedPurchaseFirstShippingNextMonthFlg
					== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_VALID;
				cartShippingTemp.UpdateShippingDateTime(
					wdtShippingDate.Visible,
					wdtShippingTime.Visible,
					((isUseFirstShippingDate || wdtShippingDate.Visible)
						&& (wddlShippingDate.SelectedValue != string.Empty))
							? (DateTime?)DateTime.Parse(wddlShippingDate.SelectedValue)
							: null,
					wdtShippingTime.Visible ? wddlShippingTime.SelectedItem.Value : null,
					wdtShippingTime.Visible ? wddlShippingTime.SelectedItem.Text : null);

				cartShippingTemp.ShippingMethod = wddlShippingMethod.SelectedValue;
				cartShippingTemp.UserSelectedShippingMethod = true;
				cartObjectCurrent.Shippings[iShippingIndex].ShippingMethod = wddlShippingMethod.SelectedValue;
				cartObjectCurrent.Shippings[iShippingIndex].UserSelectedShippingMethod = true;
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cartObjectCurrent.ShippingType);

				var isUserSelectedDeliveryCompany = (wddlDeliveryCompany.HasInnerControl
					&& (string.IsNullOrEmpty(wddlDeliveryCompany.SelectedValue) == false));

				// ギフト購入でない場合、通常の宅配便の判定を行い、ギフト購入時の場合、ギフト購入時用の宅配便の判定を行う。
				if (cartObjectCurrent.IsGift == false)
				{
					cartShippingTemp.DeliveryCompanyId = isUserSelectedDeliveryCompany
						? wddlDeliveryCompany.SelectedValue
						: ((cartObjectCurrent.IsExpressDelivery)
							? shopShipping.CompanyListExpress
							: shopShipping.CompanyListMail).First(i => i.IsDefault).DeliveryCompanyId;
				}
				else
				{
					cartShippingTemp.DeliveryCompanyId = isUserSelectedDeliveryCompany
						? wddlDeliveryCompany.SelectedValue
						: ((cartObjectCurrent.IsExpressDeliveryForGift(iShippingIndex))
							? shopShipping.CompanyListExpress
							: shopShipping.CompanyListMail).First(i => i.IsDefault).DeliveryCompanyId;
				}

				//------------------------------------------------------
				// 配送先情報設定２-(2)（定期購入の配送パターン）
				//------------------------------------------------------
				if (cartObjectCurrent.HasFixedPurchase)
				{
					Hashtable htFixedPurchase;
					string kbn;		// 配送パターン選択値
					string setting;	// 配送パターンプルダウン選択値
					CreateFixedPurchaseData(riTarget, out htFixedPurchase, out kbn, out setting);

					// エラーチェック＆カスタムバリデータへセット
					Hashtable htFixedPurchase_tmp = new Hashtable();	// まずは配送パターン選択（ラジオボタン）だけをチェックする
					htFixedPurchase_tmp.Add(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN, htFixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
					string strErrorMessage = Validator.Validate(strValidationName, htFixedPurchase_tmp);
					var wsErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "sErrorMessage");
					if (strErrorMessage != "")
					{
						// エラーに格納
						wsErrorMessage.InnerText = strErrorMessage;
						hasCartError = true;
					}
					else
					{
						// エラー消す
						wsErrorMessage.InnerText = "";
					}

					// エラーチェック＆カスタムバリデータへセット
					Dictionary<string, string> dicFixedPurchaseErrorMessages = Validator.ValidateAndGetErrorContainer(strValidationName, htFixedPurchase);
					if (dicFixedPurchaseErrorMessages.Count != 0)
					{
						// カスタムバリデータ取得
						List<CustomValidator> lCustomValidators = new List<CustomValidator>();
						CreateCustomValidators(this.Page.Form.FindControl(riCart.UniqueID), lCustomValidators);

						// エラーをカスタムバリデータへ
						SetControlViewsForError(strValidationName, dicFixedPurchaseErrorMessages, lCustomValidators);

						hasCartError = true;
					}

					// 定期購入の配送パターンをカートに格納
					var whfFixedPurchaseDaysRequired = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseDaysRequired");
					var whfFixedPurchaseMinSpan = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseMinSpan");
					int fixedPurchaseDaysRequired = int.Parse(whfFixedPurchaseDaysRequired.Value);
					int fixedPurchaseMinSpan = int.Parse(whfFixedPurchaseMinSpan.Value);
					cartShippingTemp.UpdateFixedPurchaseSetting(kbn, setting, fixedPurchaseDaysRequired, fixedPurchaseMinSpan);

					// 定期購入の次回配送予定日をカートに格納
					if (hasCartError == false)
					{
						var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlNextShippingDate");
						DateTime nextShippingDate;

						var service = new FixedPurchaseService();
						var calculateMode = service.GetCalculationMode(kbn, Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
						if (wddlNextShippingDate.InnerControl != null)
						{
							// aspx側にドロップダウンが存在するときは選択値を取得
							nextShippingDate = DateTime.Parse(wddlNextShippingDate.SelectedValue);
						}
						else
						{
							// aspx側にドロップダウンが存在しないときは、初期値を計算
							nextShippingDate = service.CalculateNextShippingDate(
								kbn,
								setting,
								cartShippingTemp.ShippingDate,
								fixedPurchaseDaysRequired,
								fixedPurchaseMinSpan,
								calculateMode);
						}
						var nextNextShippingDate = service.CalculateFollowingShippingDate(
							kbn,
							setting,
							nextShippingDate,
							fixedPurchaseMinSpan,
							calculateMode);
						cartShippingTemp.UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);

						if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
						{
							var cartFixedPurchaseNextShippingProduct = cartObjectCurrent.Items
								.FirstOrDefault(cartProduct => cartProduct.CanSwitchProductFixedPurchaseNextShippingSecondTime());
							if (cartFixedPurchaseNextShippingProduct != null)
							{
								cartShippingTemp.CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
								cartShippingTemp.UpdateNextShippingItemFixedPurchaseInfos(
									cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
									cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
								cartShippingTemp.CalculateNextShippingItemNextNextShippingDate();
							}
						}
					}
				}

				// Taiwan invoice enable
				if (OrderCommon.DisplayTwInvoiceInfo()
					&& Constants.GIFTORDER_OPTION_ENABLED)
				{
					cartShippingTemp.CarryType = wddlInvoiceCarryType.SelectedValue;
					cartShippingTemp.CarryTypeOption = wddlInvoiceCarryTypeOption.SelectedValue;
					cartShippingTemp.UniformInvoiceTypeOption = wddlUniformInvoiceTypeOption.SelectedValue;
					cartShippingTemp.UniformInvoiceType = wddlUniformInvoiceType.SelectedValue;
					if (string.IsNullOrEmpty(cartShippingTemp.UniformInvoiceTypeOption))
					{
						switch (cartShippingTemp.UniformInvoiceType)
						{
							case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
								cartShippingTemp.UniformInvoiceOption1 = wtbUniformInvoiceOption1_8.Text.Trim();
								cartShippingTemp.UniformInvoiceOption2 = wtbUniformInvoiceOption2.Text.Trim();
								break;

							case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
								cartShippingTemp.UniformInvoiceOption1 = wtbUniformInvoiceOption1_3.Text.Trim();
								cartShippingTemp.UniformInvoiceOption2 = string.Empty;
								break;

							default:
								cartShippingTemp.UniformInvoiceOption1 = string.Empty;
								cartShippingTemp.UniformInvoiceOption2 = string.Empty;
								if (string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption))
								{
									cartShippingTemp.CarryTypeOptionValue = ((cartShippingTemp.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
										? wtbCarryTypeOption_8.Text
										: wtbCarryTypeOption_16.Text);

									if (wcbCarryTypeOptionRegist.Checked)
									{
										cartShippingTemp.UpdateUserInvoiceRegistSetting(true, wtbCarryTypeOptionName.Text.Trim());
									}
								}
								else
								{
									cartShippingTemp.CarryTypeOptionValue = wlbCarryTypeOption.Text;
									cartShippingTemp.UpdateUserInvoiceRegistSetting(false, wddlInvoiceCarryTypeOption.SelectedText);
								}
								break;
						}

						if ((cartShippingTemp.UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
							&& wcbSaveToUserInvoice.Checked
							&& string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption))
						{
							cartShippingTemp.UpdateUserInvoiceRegistSetting(true, wtbUniformInvoiceTypeName.Text.Trim());
						}
					}
					else
					{
						var invoiceNo = int.Parse(wddlUniformInvoiceTypeOption.SelectedValue);
						var model = new TwUserInvoiceService().Get(this.LoginUserId, invoiceNo);

						if (model != null)
						{
							switch (model.TwUniformInvoice)
							{
								case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
									cartShippingTemp.UniformInvoiceOption1 = model.TwUniformInvoiceOption1;
									cartShippingTemp.UniformInvoiceOption2 = model.TwUniformInvoiceOption2;
									break;

								case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
									cartShippingTemp.UniformInvoiceOption1 = model.TwUniformInvoiceOption1;
									cartShippingTemp.UniformInvoiceOption2 = string.Empty;
									break;
							}

							cartShippingTemp.UpdateUserInvoiceRegistSetting(false, model.TwInvoiceName);
						}
					}

					// Validate uniform invoice
					var uniformInput = CreateUserInvoiceInput(cartShippingTemp);
					var uniformErrorMessages = Validator.ValidateAndGetErrorContainer("OrderShippingGlobal", uniformInput);

					if ((cartShippingTemp.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
						&& string.IsNullOrEmpty(cartShippingTemp.CarryTypeOption)
						&& (uniformErrorMessages.Count == 0))
					{
						// Validate carry type
						Regex regex = null;
						switch (cartShippingTemp.CarryType)
						{
							case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
								regex = new Regex(Constants.REGEX_MOBILE_CARRY_TYPE_OPTION_8);
								if (regex.IsMatch(wtbCarryTypeOption_8.Text.Trim()) == false)
								{
									uniformErrorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8"]
										= WebMessages.GetMessages(WebMessages.ERRMSG_MOBILE_CARRY_TYPE_OPTION_8);
								}
								break;

							case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
								regex = new Regex(Constants.REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16);
								if (regex.IsMatch(wtbCarryTypeOption_16.Text.Trim()) == false)
								{
									uniformErrorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16"]
										= WebMessages.GetMessages(WebMessages.ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16);
								}
								break;
						}
					}

					if (uniformErrorMessages.Count != 0)
					{
						// Custom Validator
						List<CustomValidator> lCustomValidators = new List<CustomValidator>();
						CreateCustomValidators(this.Page.Form.FindControl(riTarget.UniqueID), lCustomValidators);

						// Error Custom Validator
						SetControlViewsForError("OrderShippingGlobal", uniformErrorMessages, lCustomValidators);

						hasCartError = true;
					}
					else
					{
						cartObjectCurrent.Shippings[iShippingIndex].CarryTypeOption = cartShippingTemp.CarryTypeOption;
						cartObjectCurrent.Shippings[iShippingIndex].UniformInvoiceTypeOption = cartShippingTemp.UniformInvoiceTypeOption;
						cartObjectCurrent.Shippings[iShippingIndex].UpdateInvoice(cartShippingTemp.UniformInvoiceType,
							cartShippingTemp.UniformInvoiceOption1,
							cartShippingTemp.UniformInvoiceOption2,
							cartShippingTemp.CarryType,
							cartShippingTemp.CarryTypeOptionValue);
						cartObjectCurrent.Shippings[iShippingIndex].UpdateUserInvoiceRegistSetting(cartShippingTemp.UserInvoiceRegistFlg,
							cartShippingTemp.InvoiceName);
					}
				}

				//------------------------------------------------------
				// カート配送先に正式にセット
				//------------------------------------------------------
				// 配送日時更新
				cartObjectCurrent.Shippings[iShippingIndex].UpdateShippingDateTime(
					cartShippingTemp.SpecifyShippingDateFlg,
					cartShippingTemp.SpecifyShippingTimeFlg,
					cartShippingTemp.ShippingDate,
					cartShippingTemp.ShippingTime,
					cartShippingTemp.ShippingTimeMessage);

				// 配送方法、配送会社
				cartObjectCurrent.Shippings[iShippingIndex].ShippingMethod = cartShippingTemp.ShippingMethod;
				cartObjectCurrent.Shippings[iShippingIndex].DeliveryCompanyId = cartShippingTemp.DeliveryCompanyId;

				// 定期購入情報更新
				if (cartObjectCurrent.HasFixedPurchase)
				{
					cartObjectCurrent.Shippings[iShippingIndex].UpdateFixedPurchaseSetting(
						cartShippingTemp.FixedPurchaseKbn,
						cartShippingTemp.FixedPurchaseSetting,
						cartShippingTemp.FixedPurchaseDaysRequired,
						cartShippingTemp.FixedPurchaseMinSpan);
					cartObjectCurrent.Shippings[iShippingIndex].UpdateNextShippingDates(
						cartShippingTemp.NextShippingDate,
						cartShippingTemp.NextNextShippingDate);

					if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
					{
						cartObjectCurrent.Shippings[iShippingIndex].CanSwitchProductFixedPurchaseNextShippingSecondTime
							= cartShippingTemp.CanSwitchProductFixedPurchaseNextShippingSecondTime;
						cartObjectCurrent.Shippings[iShippingIndex].UpdateNextShippingItemFixedPurchaseInfos(
							cartShippingTemp.NextShippingItemFixedPurchaseKbn,
							cartShippingTemp.NextShippingItemFixedPurchaseSetting);
						cartObjectCurrent.Shippings[iShippingIndex].CalculateNextShippingItemNextNextShippingDate();
					}
				}

				// Update shipping date
				if ((string.IsNullOrEmpty(wddlShippingDate.SelectedValue) == false)
					&& (this.ShopShippingList.Count > iShippingIndex)
					&& (this.ShopShippingList[iShippingIndex].FixedPurchaseFirstShippingNextMonthFlg
						== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_VALID))
				{
					cartShippingTemp.ShippingDate = DateTime.Parse(wddlShippingDate.SelectedValue);
				}

				var wrcbOnlyReflectMemoToFirstOrder = GetWrappedControl<WrappedCheckBox>(riCart, "cbOnlyReflectMemoToFirstOrder");
				cartObjectCurrent.ReflectMemoToFixedPurchase =
					(((wrcbOnlyReflectMemoToFirstOrder.HasInnerControl) && (cartObjectCurrent.HasFixedPurchase))
						? wrcbOnlyReflectMemoToFirstOrder.Checked
						: false);
			}

			//------------------------------------------------------
			// 注文メモセット
			//------------------------------------------------------
			StringBuilder sbOrderMemo = new StringBuilder();
			foreach (RepeaterItem riMemoItem in wrRepeaterMemo.Items)
			{
				var wrtbMemo = GetWrappedControl<WrappedTextBox>(riMemoItem, "tbMemo");
				var wrsError = GetWrappedControl<WrappedHtmlGenericControl>(riMemoItem, "sErrorMessageMemo");
				var inputMemo = StringUtility.RemoveUnavailableControlCode(wrtbMemo.Text).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

				// メモ欄毎のエラーチェック
				string strErrorMessage = Validator.CheckLengthMaxError(
					cartObjectCurrent.OrderMemos[riMemoItem.ItemIndex].OrderMemoName,
					inputMemo,
					cartObjectCurrent.OrderMemos[riMemoItem.ItemIndex].MaxLength);

				// カートメモ更新
				if (strErrorMessage == "")
				{
					cartObjectCurrent.OrderMemos[riMemoItem.ItemIndex].InputText = inputMemo;
					wrsError.InnerText = "";
				}
				// エラーメッセージ追加
				else
				{
					hasCartError = true;
					wrsError.InnerHtml = strErrorMessage + "<br />";
					wrtbMemo.Focus();

					if (this.IsLandingPage && string.IsNullOrEmpty(SessionManager.LpValidateErrorElementClientId))
					{
						SessionManager.LpValidateErrorElementClientId = wrtbMemo.ClientID;
					}
				}
			}

			// 注文拡張項目はギフトOPでは未対応
			if (Constants.ORDER_EXTEND_OPTION_ENABLED
				&& (Constants.GIFTORDER_OPTION_ENABLED == false))
			{
				var inputDictionary = CreateOrderExtendFromInputData(riCart);
				var input = new OrderExtendInput(OrderExtendInput.UseType.Register, inputDictionary);
				var errorMessage = input.Validate();
				if (errorMessage.Count == 0)
				{
					cartObjectCurrent.OrderExtend = inputDictionary;
				}
				else
				{
					hasCartError = true;
					SetOrderExtendErrMessage(riCart, errorMessage);
				}
			}

			//-------------------------------------------------------------------------------------
			// その他のコントロール　案件毎で追加したコントロールでもチェックしたい
			//-------------------------------------------------------------------------------------
			if (OtherValidation() == false) hasCartError = true;

			if (hasCartError) hasError = hasCartError;
		}

		return (hasError == false);
	}

	/// <summary>
	/// 注文拡張項目の設定
	/// </summary>
	public void SetOrderExtendInputForm()
	{
		foreach (var riCart in this.WrCartList.Items.Cast<RepeaterItem>())
		{
			var wrOrderExtendInput = GetWrappedControl<WrappedRepeater>(riCart, "rOrderExtendInput");
			var orderExtendInput = new OrderExtendInput(OrderExtendInput.UseType.Register, this.CartList.Items[riCart.ItemIndex].OrderExtend);
			wrOrderExtendInput.DataSource = orderExtendInput.OrderExtendItems;
			wrOrderExtendInput.DataBind();
			var cartObjectCurrent = this.CartList.Items[riCart.ItemIndex];
			SetOrderExtendFromUserExtendObject(
				wrOrderExtendInput,
				orderExtendInput);
		}
	}

	/// <summary>
	/// 注文拡張項目の入力内容を設定
	/// </summary>
	/// <param name="rItem">注文拡張項目リピータ</param>
	/// <param name="input">入力内容</param>
	public void SetOrderExtendFromUserExtendObject(WrappedRepeater rItem, OrderExtendInput input)
	{
		foreach (RepeaterItem item in rItem.Items)
		{
			var settingId = GetOrderExtendSettingId(item);
			var itemInput = input.OrderExtendItems.FirstOrDefault(m => m.SettingModel.SettingId == settingId) ?? new OrderExtendItemInput();
			switch (GetOrderExtendInputType(item))
			{
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
					var wtbSelect = GetOrderExtendWrappedInputText(item);
					wtbSelect.Text = itemInput.InputValue;
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					var wddlSelect = GetOrderExtendWrappedDdlSelect(item);
					OrderExtendCommon.GetListItemForFront(itemInput.SettingModel.InputDefault).ForEach(listitem => wddlSelect.Items.Add(listitem));
					if (wddlSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)) != null)
					{
						wddlSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)).Selected = true;
					}
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					var wrblSelect = GetOrderExtendWrappedRadioSelect(item);
					OrderExtendCommon.GetListItemForFront(itemInput.SettingModel.InputDefault).ForEach(listitem => wrblSelect.Items.Add(listitem));
					if (wrblSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)) != null)
					{
						wrblSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)).Selected = true;
					}
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					var wcblSelect = GetOrderExtendWrappedCheckSelect(item);
					wcblSelect.ClearItems();
					OrderExtendCommon.GetListItemForFront(itemInput.SettingModel.InputDefault).ForEach(listitem => wcblSelect.Items.Add(listitem));
					var selectedItem = (StringUtility.ToEmpty(itemInput.InputValue)).Split(',');
					foreach (ListItem listitem in wcblSelect.Items)
					{
						listitem.Selected = selectedItem.Contains(listitem.Value);
					}
					break;

				default:
					// なにもしない
					break;
			}
		}
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎の拡張項目設定ID取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>拡張項目設定ID</returns>
	private string GetOrderExtendSettingId(RepeaterItem item)
	{
		var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId");
		return whfSettingId.Value;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎の入力方法取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>入力方法</returns>
	private string GetOrderExtendInputType(RepeaterItem item)
	{
		var whfInputType = GetWrappedControl<WrappedHiddenField>(item, "hfInputType");
		return whfInputType.Value;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みテキストボックスコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みテキストボックス</returns>
	private WrappedTextBox GetOrderExtendWrappedInputText(RepeaterItem item)
	{
		return GetWrappedControl<WrappedTextBox>(item, "tbSelect");
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みドロップダウンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みドロップダウンリスト</returns>
	private WrappedDropDownList GetOrderExtendWrappedDdlSelect(RepeaterItem item)
	{
		return GetWrappedControl<WrappedDropDownList>(item, "ddlSelect");
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みラジオボタンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みラジオボタンリスト</returns>
	private WrappedRadioButtonList GetOrderExtendWrappedRadioSelect(RepeaterItem item)
	{
		var wrblSelect = GetWrappedControl<WrappedRadioButtonList>(item, "rblSelect");
		return wrblSelect;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みチェックボックスリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みチェックボックスリスト</returns>
	private WrappedCheckBoxList GetOrderExtendWrappedCheckSelect(RepeaterItem item)
	{
		var wcblSelect = GetWrappedControl<WrappedCheckBoxList>(item, "cblSelect");
		return wcblSelect;
	}

	/// <summary>
	/// 注文拡張項目 エラーメッセージの設定
	/// </summary>
	/// <param name="riCart">カートリピーター</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	public void SetOrderExtendErrMessage(RepeaterItem riCart, Dictionary<string, string> errorMessage)
	{
		var wrOrderExtendInput = GetWrappedControl<WrappedRepeater>(riCart, "rOrderExtendInput");
		SetOrderExtendErrMessage(wrOrderExtendInput, errorMessage);
	}
	/// <summary>
	/// 注文拡張項目 エラーメッセージの設定
	/// </summary>
	/// <param name="rItem">注文項目リピーター</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	public void SetOrderExtendErrMessage(WrappedRepeater rItem, Dictionary<string, string> errorMessage)
	{
		foreach (RepeaterItem item in rItem.Items)
		{
			var settingId = GetOrderExtendSettingId(item);
			var wlbErrMessage = GetWrappedControl<WrappedLabel>(item, "lbErrMessage");
			wlbErrMessage.Text = string.Empty;

			if (errorMessage.ContainsKey(settingId) == false) continue;

			wlbErrMessage.Focus();
			wlbErrMessage.Text = errorMessage[settingId];

			if (this.IsLandingPage && string.IsNullOrEmpty(SessionManager.LpValidateErrorElementClientId))
			{
				SessionManager.LpValidateErrorElementClientId = wlbErrMessage.ClientID;
			}
		}
	}

	/// <summary>
	/// 注文拡張項目 入力内容の取得
	/// </summary>
	/// <param name="riCart">カートリピーター</param>
	/// <returns>入力内容</returns>
	public Dictionary<string, CartOrderExtendItem> CreateOrderExtendFromInputData(RepeaterItem riCart)
	{
		var wrOrderExtendInput = GetWrappedControl<WrappedRepeater>(riCart, "rOrderExtendInput");
		var cartObjectCurrent = this.CartList.Items[riCart.ItemIndex];
		return CreateOrderExtendFromInputData(wrOrderExtendInput);
	}
	/// <summary>
	/// 注文拡張項目 入力内容の取得
	/// </summary>
	/// <param name="rItem">注文拡張項目リピーター</param>
	/// <returns>入力内容</returns>
	public Dictionary<string, CartOrderExtendItem> CreateOrderExtendFromInputData(WrappedRepeater rItem)
	{
		var result = new Dictionary<string, CartOrderExtendItem>();
		foreach (RepeaterItem item in rItem.Items)
		{
			var value = "";
			switch (GetOrderExtendInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					var wtbSelect = GetOrderExtendWrappedInputText(item);
					value = StringUtility.ToEmpty(wtbSelect.Text);
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					var wddlSelect = GetOrderExtendWrappedDdlSelect(item);
					value = (wddlSelect.SelectedItem != null) ? wddlSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					var wrblSelect = GetOrderExtendWrappedRadioSelect(item);
					value = (wrblSelect.SelectedItem != null) ? wrblSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					var wcblSelect = GetOrderExtendWrappedCheckSelect(item);
					foreach (ListItem listitem in wcblSelect.Items)
					{
						if (listitem.Selected == false) continue;

						value += (value != "") ? "," : "";
						value += listitem.Value;
					}
					break;

				default:
					// なにもしない
					break;
			}

			var settingId = GetOrderExtendSettingId(item);
			var cartOrderExtendItem = new CartOrderExtendItem()
			{
				Value = value,
			};

			result.Add(settingId, cartOrderExtendItem);
		}
		return result;
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（Amazonその他情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns>True:エラーなし, False:エラーあり</returns>
	public bool lbNext_Click_OrderShipping_AmazonOthers(object sender, System.EventArgs e)
	{
		// 共通処理
		if (lbNext_Click_OrderShipping_Others(sender, e) == false) return false;

		// Amazonモデル取得
		var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
		if (amazonModel == null) return false;

		// 以下CV1処理のためCv2はここで抜ける
		if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return true;

		#region ラップ済みコントロール宣言
		var whfAmazonOrderRefID = GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonOrderRefID");
		var whfAmazonBillingAgreementId = GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonBillingAgreementId");
		var whgcConstraintErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "constraintErrorMessage");
		#endregion

		// 通常注文の場合
		if (this.CartList.Items[0].HasFixedPurchase == false)
		{
			if (string.IsNullOrEmpty(whfAmazonOrderRefID.Value)) return false;

			// 注文情報をセット(エラー処理のために一時的に空のOrderIdセット)
			var response = AmazonApiFacade.SetOrderReferenceDetails(whfAmazonOrderRefID.Value, this.CartList.Items[0].PriceTotal, "");

			if (response.GetSuccess() == false)
			{
				whgcConstraintErrorMessage.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION); ;
				return false;
			}

			if (response.GetConstraintIdList().Any())
			{
				var messages = response.GetConstraintIdList().Select(
					constraintId => AmazonApiMessageManager.GetOrderReferenceConstraintMessage(constraintId));
				whgcConstraintErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(string.Join("\r\n", messages));
				return false;
			}
		}
		// 定期注文の場合
		else
		{
			if (string.IsNullOrEmpty(whfAmazonBillingAgreementId.Value)) return false;

			// 注文情報をセット
			var response = AmazonApiFacade.SetBillingAgreementDetails(whfAmazonBillingAgreementId.Value);

			if (response.GetSuccess() == false)
			{
				whgcConstraintErrorMessage.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
				return false;
			}

			if (response.GetConstraintIdList().Any())
			{
				var messages = response.GetConstraintIdList().Select(
					constraintId => AmazonApiMessageManager.GetBillingAgreementConstraintMessage(constraintId));
				whgcConstraintErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(string.Join("\r\n", messages));
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// 注文時会員登録
	/// </summary>
	/// <param name="userInfo">ユーザーインプット</param>
	/// <returns>ログインユーザー情報</returns>
	public UserModel UserRegist(UserInput userInfo)
	{
		var user = GetRegisterUser(userInfo);
		var result = UserRegister(user);
		return result;
	}

	/// <summary>
	/// 注文時会員登録
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <returns>ログインユーザー情報</returns>
	public UserModel UserRegister(UserModel user)
	{
		var userService = new UserService();

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Insert member Cross Point
			var result = new CrossPointUserApiService().Insert(user);
			if (result.IsSuccess == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = result.ErrorCodeList.Contains(
						w2.App.Common.Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
					? result.ErrorMessage
					: MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (user.UserExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
				&& user.UserExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
				&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataText.CrossPointShopCardNo) == false)
				&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataText.CrossPointShopCardPin) == false))
			{
				// Merge member Cross Point
				result = new CrossPointUserApiService().Merge(
					user.UserId,
					user.UserExtend.UserExtendDataText.CrossPointShopCardNo,
					user.UserExtend.UserExtendDataText.CrossPointShopCardPin);
				if (result.IsSuccess == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages
						(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				// Get member Cross Point info
				var userApiResult = new CrossPointUserApiService().Get(user.UserId);
				if (userApiResult == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				user.UserExtend.UserExtendDataText.CrossPointShopCardNo = userApiResult.RealShopCardNo;
				user.UserExtend.UserExtendDataText.CrossPointShopCardPin = userApiResult.PinCode;
			}
			else
			{
				// Get member Cross Point info
				var userApiResult = new CrossPointUserApiService().Get(user.UserId);
				if (userApiResult == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_NO) == false)
				{
					user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_NO);
				}

				if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN) == false)
				{
					user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN);
				}

				if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME) == false)
				{
					user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME);
				}

				user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = userApiResult.RealShopCardNo;
				user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = userApiResult.PinCode;
				user.UserExtend.UserExtendDataValue.CrossPointAddShopName = userApiResult.AdmissionShopName;
			}
		}

		userService.InsertWithUserExtend(
			user,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		var registeredUser = userService.Get(user.UserId);
		var socialLogin = Constants.SOCIAL_LOGIN_ENABLED
			? (SocialLoginModel)this.Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL]
			: null;
		new UserRegister(socialLogin).ExecProcessOnUserRegisted(
			registeredUser,
			UpdateHistoryAction.DoNotInsert);

		Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);

		if (string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false)
		{
			LineUtil.UpdateUserExtendForLineUserId(registeredUser.UserId,
				SessionManager.LineProviderUserId,
				UpdateHistoryAction.DoNotInsert);
			Session.Remove(Constants.SESSION_KEY_LOGIN_USER_LINE_ID);
		}

		return registeredUser;
	}
	/// <summary>
	/// 注文時会員登録
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="mailData">会員登録メール内容</param>
	/// <returns>ログインユーザー情報</returns>
	public UserModel UserRegister(UserModel user, out Hashtable mailData)
	{
		var userService = new UserService();

		userService.InsertWithUserExtend(
			user,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		var registeredUser = userService.Get(user.UserId);
		var socialLogin = Constants.SOCIAL_LOGIN_ENABLED
			? (SocialLoginModel)this.Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL]
			: null;
		mailData = new UserRegister(socialLogin).ExecProcessOnUserRegistered(
			registeredUser,
			UpdateHistoryAction.DoNotInsert);

		Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);

		return registeredUser;
	}

	/// <summary>
	/// 注文時登録用のユーザー情報
	/// </summary>
	/// <param name="userInfo">ユーザー入力情報</param>
	/// <returns>登録用ユーザー情報</returns>
	protected UserModel GetRegisterUser(UserInput userInfo)
	{
		var user = new UserModel
		{
			UserId = userInfo.UserId,
			UserKbn = this.IsSmartPhone ? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER : Constants.FLG_USER_USER_KBN_PC_USER,
			Name = this.CartList.Owner.Name,
			Name1 = this.CartList.Owner.Name1,
			Name2 = this.CartList.Owner.Name2,
			NameKana = this.CartList.Owner.NameKana,
			NameKana1 = this.CartList.Owner.NameKana1,
			NameKana2 = this.CartList.Owner.NameKana2,
			NickName = "",
			Birth = this.CartList.Owner.Birth,
			BirthYear = this.CartList.Owner.BirthYear,
			BirthMonth = this.CartList.Owner.BirthMonth,
			BirthDay = this.CartList.Owner.BirthDay,
			Sex = this.CartList.Owner.Sex,
			MailAddr = this.CartList.Owner.MailAddr,
			MailAddr2 = this.CartList.Owner.MailAddr2,
			Zip = this.CartList.Owner.Zip,
			Zip1 = this.CartList.Owner.Zip1,
			Zip2 = this.CartList.Owner.Zip2,
			Addr = this.CartList.Owner.ConcatenateAddressWithoutCountryName(),
			Addr1 = this.CartList.Owner.Addr1,
			Addr2 = this.CartList.Owner.Addr2,
			Addr3 = this.CartList.Owner.Addr3,
			Addr4 = this.CartList.Owner.Addr4,
			CompanyName = this.CartList.Owner.CompanyName,
			CompanyPostName = this.CartList.Owner.CompanyPostName,
			Tel1 = this.CartList.Owner.Tel1,
			Tel1_1 = this.CartList.Owner.Tel1_1,
			Tel1_2 = this.CartList.Owner.Tel1_2,
			Tel1_3 = this.CartList.Owner.Tel1_3,
			Tel2 = this.CartList.Owner.Tel2,
			Tel2_1 = this.CartList.Owner.Tel2_1,
			Tel2_2 = this.CartList.Owner.Tel2_2,
			Tel2_3 = this.CartList.Owner.Tel2_3,
			MailFlg = this.CartList.Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG,
			EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL,
			RemoteAddr = this.Page.Request.ServerVariables["REMOTE_ADDR"],
			LoginId = userInfo.LoginId,
			PasswordDecrypted = userInfo.Password,
			MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(), // デフォルト会員ランクの設定
			RecommendUid = UserCookieManager.UniqueUserId,
			LastChanged = Constants.FLG_LASTCHANGED_USER,
			UserExtend = userInfo.UserExtendInput.CreateModel(),
			AddrCountryIsoCode = this.CartList.Owner.AddrCountryIsoCode,
			AddrCountryName = this.CartList.Owner.AddrCountryName,
			Addr5 = this.CartList.Owner.Addr5,
			AdvcodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]),
		};

		// 広告コードより補正
		UserUtility.CorrectUserByAdvCode(user);

		return user;
	}

	/// <summary>
	/// その他の検証実行
	/// </summary>
	/// <returns>True:エラーなし, False:エラーあり</returns>
	private bool OtherValidation()
	{
		if (m_customValidatorList == null)
		{
			//未取得の場合はカスタムバリデータを取得する
			m_customValidatorList = new List<CustomValidator>();
			CreateCustomValidators(this.Page, m_customValidatorList);
		}
		// IDが「OtherValidator」を含むものを抽出
		var targetValidators = m_customValidatorList.Where(validator =>
		{
			if (validator.ID != null) return validator.ID.Contains("OtherValidator");
			return false;
		});

		bool isError = false;
		foreach (var validator in targetValidators)
		{
			string validationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";
			// 別のテキストボックスなどに入力させた場合でもチェックしたいのでコントロール名でみる
			var control = validator.Parent.FindControl(validator.ControlToValidate);
			var controlValue = Request[control.UniqueID];

			var message = Validator.ValidateControl(validationName, validator.ControlToValidate, controlValue);
			WebControl webControl = null;
			HtmlControl htmlControl = null;
			if (control is WebControl) webControl = (WebControl)control;
			if (control is HtmlControl) htmlControl = (HtmlControl)control;

			if (string.IsNullOrEmpty(message))
			{
				validator.ErrorMessage = "";
				validator.IsValid = true;
				if (webControl != null) webControl.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
				if (htmlControl != null) htmlControl.Attributes.Add("class", "");
			}
			else
			{
				// エラーメッセージ追加
				validator.IsValid = false;
				validator.ErrorMessage = message;

				// ラジオボタンリストの場合、エラーの背景色をセットしない
				if (control.GetType() != typeof(RadioButtonList))
				{
					if (webControl != null)
					{
						if (webControl.CssClass.Contains(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING) == false)
						{
							webControl.CssClass += Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING;
						}
					}

					if (htmlControl != null)
					{
						htmlControl.Attributes.Add("class", Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);
					}
				}

				isError = true;
			}
		}

		return (isError == false);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <param name="isFocus">住所3テキストボックスにフォーカスするか</param>
	public void lbSearchOwnergAddr_Click(object sender, EventArgs e, bool isFocus = true)
	{
		SearchZipCodeForOwner(sender, isFocus: isFocus);
		SetNextShippingDateByShippingKbnSelectedValue(sender, e, CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
	}

	/// <summary>
	/// 注文者郵便番号検索
	/// </summary>
	/// <param name="sender">郵便番号検索の発生元コントロール（郵便番号テキストボックスor検索ボタン）</param>
	/// <param name="isFocus">住所3テキストボックスにフォーカスするか</param>
	private void SearchZipCodeForOwner(object sender, bool isFocus = true)
	{
		var control = (Control)sender;
		var wsOwnerZipError = GetWrappedControl<WrappedHtmlGenericControl>(control.Parent, "sOwnerZipError");

		var cart = this.CartList.Items[0];
		// 配送先入力状態かギフトオプションオンなら注文者情報の住所チェックはしない
		wsOwnerZipError.InnerText
			= ((cart.Shippings[0].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
				|| Constants.GIFTORDER_OPTION_ENABLED)
					? SearchZipCode(sender, isFocus : isFocus)
					: SearchZipCode(sender, this.UnavailableShippingZip, isFocus: isFocus);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（送り主情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchSenderAddr_Click(object sender, System.EventArgs e)
	{
		SearchZipCodeForSender(sender);
	}

	/// <summary>
	/// 送り主郵便番号検索
	/// </summary>
	/// <param name="sender">郵便番号検索の発生元コントロール（郵便番号テキストボックスor検索ボタン）</param>
	private void SearchZipCodeForSender(object sender)
	{
		var control = (Control)sender;
		var whcSenderZipError = GetWrappedControl<WrappedHtmlGenericControl>(control.Parent, "sSenderZipError");
		whcSenderZipError.InnerText = SearchZipCode(sender);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchShippingAddr_Click(object sender, System.EventArgs e)
	{
		SearchZipCodeForShipping(sender);
		SetNextShippingDateByShippingKbnSelectedValue(sender, e, CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
	}

	/// <summary>
	/// 配送先郵便番号検索
	/// </summary>
	/// <param name="sender">郵便番号検索の発生元コントロール（郵便番号テキストボックスor検索ボタン）</param>
	private void SearchZipCodeForShipping(object sender)
	{
		var control = (Control)sender;
		var whcShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(control.Parent, "sShippingZipError");
		whcShippingZipError.InnerText = SearchZipCode(sender, this.UnavailableShippingZip);
	}

	/// <summary>
	/// 画面上配送先区分選択値が指定配送先区分と一致する場合、次回配送予定日を設定
	/// </summary>
	/// <param name="sender">発生元コントロール</param>
	/// <param name="e">イベントデータ</param>
	/// <param name="shippingKbn">設定対象配送先区分</param>
	private void SetNextShippingDateByShippingKbnSelectedValue(object sender, EventArgs e, string shippingKbn)
	{
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return;

		var selectCartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(selectCartControl, "ddlShippingKbnList");
		if ((wddlShippingKbnList.HasInnerControl == false)
			|| (wddlShippingKbnList.SelectedValue != shippingKbn))
		{
			return;
		}

		// 配送先区分がコンビニ、指定不可な値の場合は設定しない
		switch (shippingKbn)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				SetNextShippingDateForOwnderAddr(sender, e);
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				SetNextShippingDateForShippingAddr(sender, e);
				break;
		}
	}

	/// <summary>
	/// カート１の配送先へ配送するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbShipToCart1Address_OnCheckedChanged(object sender, System.EventArgs e)
	{
		// ラップ済みコントロール宣言
		WrappedCheckBox wcbShipToCart1Address = null;
		if (sender is CheckBox)
		{
			wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is Control)
		{
			wcbShipToCart1Address = (WrappedCheckBox)sender;
		}
		else
		{
			return;
		}
		var whgcdivShippingInputForm = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "divShippingInputForm");
		var whgcdivShipToCart1Address = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "divShipToCart1Address");
		var whfShowShippingInputForm = GetWrappedControl<WrappedHiddenField>(wcbShipToCart1Address.Parent, "hcShowShippingInputForm");

		whgcdivShippingInputForm.Visible = (wcbShipToCart1Address.Checked == false) || (whgcdivShipToCart1Address.Visible == false);
		whgcdivShippingInputForm.Visible = (whfShowShippingInputForm.Value == "") ? // ギフトフラグON時は "" となるので元の値を代入
			whgcdivShippingInputForm.Visible : whgcdivShippingInputForm.Visible && bool.Parse(whfShowShippingInputForm.Value);

		// For case taiwan invoice enable and has multi cart
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry");
			var wsInvoicesInFirstCart = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sInvoices");
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(wcbShipToCart1Address.Parent, "ddlShippingKbnList");
			var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(wcbShipToCart1Address.Parent, "ddlShippingCountry");
			var wsInvoices = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "sInvoices");
			if (wcbShipToCart1Address.Checked)
			{
				wsInvoices.Visible = wsInvoicesInFirstCart.Visible;
			}
			else
			{
				var countryCode = string.Empty;
				switch (wddlShippingKbnList.SelectedValue)
				{
					case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
						countryCode = wddlOwnerCountry.SelectedValue;
						break;

					case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
						countryCode = wddlShippingCountry.SelectedValue;
						break;

					default:
						var shippingNo = 0;
						if (int.TryParse(wddlShippingKbnList.SelectedValue, out shippingNo))
						{
							var userShipping = new UserShippingService().Get(this.LoginUserId, shippingNo);
							if (userShipping != null)
							{
								countryCode = userShipping.ShippingCountryIsoCode;
							}
						}
						break;
				}
				wsInvoices.Visible = IsCountryTw(countryCode);
			}
		}
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& (sender is CheckBox))
		{
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(wcbShipToCart1Address, "ddlShippingKbnList");

			if (wddlShippingKbnList.InnerControl != null)
				ddlShippingKbnList_OnSelectedIndexChanged(wddlShippingKbnList, e);
		}

		if (Constants.REALSHOP_OPTION_ENABLED
			&& Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			var cartControl = GetParentRepeaterItem((Control)sender, "rCartList");
			var whgcdivStorePickup = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "dvStorePickup");
			var whgch4DeliveryOptions = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "h4DeliveryOptions");
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(wcbShipToCart1Address.Parent, "ddlShippingKbnList");
			var wlShipToCart1AddressInvalidMessage = GetWrappedControl<WrappedLiteral>(wcbShipToCart1Address.Parent, "lShipToCart1AddressInvalidMessage");
			var wdvDeliveryCompany = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "dvDeliveryCompany");
			var wdvShippingDateTime = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "dvShipppingDateTime");
			var whgcdivShippingMethod = GetWrappedControl<WrappedHtmlGenericControl>(wcbShipToCart1Address.Parent, "dvShippingMethod");

			whgch4DeliveryOptions.Visible = true;
			if (wcbShipToCart1Address.Checked)
			{
				whgcdivStorePickup.Visible = false;
				if (this.CartList.Items[0].Shippings[0].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
				{
					wlShipToCart1AddressInvalidMessage.Text = CheckProductBuyStorePickup(cartControl.ItemIndex);
					wlShipToCart1AddressInvalidMessage.Visible = string.IsNullOrEmpty(wlShipToCart1AddressInvalidMessage.Text) == false;
					whgch4DeliveryOptions.Visible = false;
					wdvDeliveryCompany.Visible = false;
					whgcdivShippingMethod.Visible = false;
					wdvShippingDateTime.Visible = false;
				}
				else
				{
					wlShipToCart1AddressInvalidMessage.Visible = false;
					whgch4DeliveryOptions.Visible = true;
					var count = GetDeliveryCompanyListItem(cartControl.ItemIndex).Count;
					wdvDeliveryCompany.Visible = (CanInputShippingTo(cartControl.ItemIndex)
						&& ((count > 1)
						|| (this.CartList.Items[cartControl.ItemIndex].IsShippingConvenienceStore
							&& count != 0)));
					whgcdivShippingMethod.Visible = CanInputShippingTo(cartControl.ItemIndex);
					wdvShippingDateTime.Visible = (CanInputDateSet(cartControl.ItemIndex)
						|| CanInputTimeSet(cartControl.ItemIndex));
				}
			}
			else
			{
				wlShipToCart1AddressInvalidMessage.Visible = false;
				if(wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
				{
					whgcdivStorePickup.Visible = true;
				}
			}
		}
	}

	/// <summary>
	/// 配送情報ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlShippingKbnList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		// ラップ済みコントロール宣言
		WrappedDropDownList wddlShippingKbnList = null;
		if (sender is WrappedDropDownList)
		{
			wddlShippingKbnList = (WrappedDropDownList)sender;
		}
		else if (sender is Control)
		{
			wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else
		{
			return;
		}

		// 表示制御のため、配送先に設定されている国を取得する
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlShippingCountry_SelectedIndexChanged(sender, e);
			if (Constants.GIFTORDER_OPTION_ENABLED) ddlSenderCountry_SelectedIndexChanged(sender, e);
		}

		#region ラップ済みコントロール宣言
		var cartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var shippingControl = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var whgcdivShippingDisp = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingDisp");
		var wddlDeliveryCompany = GetWrappedControl<WrappedDropDownList>(wddlShippingKbnList.Parent, "ddlDeliveryCompany");
		var whgcdivShippingInputFormInner = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingInputFormInner");
		var wlShippingName1 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingName1");
		var wlShippingName2 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingName2");
		var wlShippingNameKana1 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingNameKana1");
		var wlShippingNameKana2 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingNameKana2");
		var wlShippingZip = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingZip");
		var wlShippingZipGlobal = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingZipGlobal");
		var wlShippingAddr1 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingAddr1");
		var wlShippingAddr2 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingAddr2");
		var wlShippingAddr3 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingAddr3");
		var wlShippingAddr4 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingAddr4");
		var wlShippingAddr5 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingAddr5");
		var wlShippingCountryName = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingCountryName");
		var wlShippingCompanyName = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingCompanyName");
		var wlShippingCompanyPostName = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingCompanyPostName");
		var wlShippingTel1 = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lShippingTel1");
		var wlShippingCountryErrorMessage = GetWrappedControl<WrappedLiteral>(((Control)sender).Parent, "lShippingCountryErrorMessage");
		var whgcdivShippingInputFormConvenience = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingInputFormConvenience");
		var whgcdivShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingTime");
		var whgcdivShippingVisibleConvenienceStore = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingVisibleConvenienceStore");
		var whgcdivSaveToUserShipping = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divSaveToUserShipping");
		var whgcdivButtonOpenConvenienceStoreMapPopup = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divButtonOpenConvenienceStoreMapPopup");
		var wlCvsShopId = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lCvsShopId");
		var wlCvsShopName = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lCvsShopName");
		var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lCvsShopAddress");
		var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(whgcdivShippingDisp, "lCvsShopTel");
		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(whgcdivShippingDisp, "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(whgcdivShippingDisp, "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(whgcdivShippingDisp, "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(whgcdivShippingDisp, "hfCvsShopTel");
		var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(wddlShippingKbnList.Parent, "cbShipToCart1Address");
		var wdvDeliveryCompany = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "dvDeliveryCompany");
		var wdvShippingDateTime = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "dvShipppingDateTime");
		var wddShippingReceivingStoreType = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "ddShippingReceivingStoreType");
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(wddlShippingKbnList.Parent, "ddlShippingReceivingStoreType");
		var whgcdivStorePickup = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "dvStorePickup");
		var whgcdivShippingMethod = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "dvShippingMethod");
		var wlStorePickupInvalidMessage = GetWrappedControl<WrappedLiteral>(((Control)sender).Parent, "lStorePickupInvalidMessage");
		var wlShipToCart1AddressInvalidMessage = GetWrappedControl<WrappedLiteral>(((Control)sender).Parent, "lShipToCart1AddressInvalidMessage");
		var wddlRealShopArea = GetWrappedControl<WrappedDropDownList>(wddlShippingKbnList.Parent, "ddlRealShopArea");
		var wddlRealShopName = GetWrappedControl<WrappedDropDownList>(wddlShippingKbnList.Parent, "ddlRealShopName");
		var wdlRealShopAddress = GetWrappedControl<WrappedHtmlGenericControl>(wddlRealShopName, "dlRealShopAddress");
		var wdlRealShopOpenningHours = GetWrappedControl<WrappedHtmlGenericControl>(wddlRealShopName, "dlRealShopOpenningHours");
		var wdlRealShopTel = GetWrappedControl<WrappedHtmlGenericControl>(wddlRealShopName, "dlRealShopTel");
		#endregion

		wlShippingCountryErrorMessage.Text = string.Empty;
		var hasError = false;

		var cartIndex = cartControl.ItemIndex;
		var shippingNo = ((shippingControl != null) ? shippingControl.ItemIndex : 0);
		var isShippingConvenience = false;
		var shippingReceivingStoreTypeSelected = this.CartList.Items[cartIndex].Shippings[shippingNo].ShippingReceivingStoreType;

		// ページ初期表示時、配送先が指定可能(デジタルコンテンツ商品購入ではない)または注文者情報の「国」が配送不可の国の場合
		// 配送先選択のドロップダウンリストは「登録済みのアドレス帳」または「配送先入力」を初期選択する
		if ((this.CartList.Items.Any(c => c.IsDigitalContentsOnly) == false) && (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER))
		{
			var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry");
			var ownerAddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
				? (wddlOwnerCountry.HasInnerControl)
					? wddlOwnerCountry.SelectedValue
					: GetCartObjectList().Owner.AddrCountryIsoCode // 注文者の国ドロップダウンが無いとき
				: "";
			hasError = (ShippingCountryUtil.CheckShippingCountryAvailable(this.ShippingAvailableCountryList, ownerAddrCountryIsoCode) == false);

			if ((sender is WrappedDropDownList) && hasError)
			{
				var userShipping = this.IsLoggedIn
					? this.UserShippingAddr.Where(x => ShippingCountryUtil.CheckShippingCountryAvailable(this.ShippingAvailableCountryList, x.ShippingCountryIsoCode)).FirstOrDefault()
					: null;

				wddlShippingKbnList.SelectedValue = (userShipping == null)
					? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW
					: userShipping.ShippingNo.ToString();

				hasError = false;
			}
		}

		switch (wddlShippingKbnList.SelectedValue)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				whgcdivShippingDisp.Visible = false;
				whgcdivShippingInputFormInner.Visible = true;
				whgcdivShippingTime.Visible = true;
				//whgcdivShippingMethod.Visible = true;
				whgcdivShippingInputFormConvenience.Visible = false;
				whgcdivShippingVisibleConvenienceStore.Visible = true;
				whgcdivSaveToUserShipping.Visible = true;
				wdvShippingDateTime.Visible = true;
				wddShippingReceivingStoreType.Visible = false;
				whgcdivStorePickup.Visible = false;
				wlStorePickupInvalidMessage.Visible = false;
				wlShipToCart1AddressInvalidMessage.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				whgcdivShippingTime.Visible = true;
				whgcdivShippingDisp.Visible = Request.RawUrl.Contains(Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING); // 配送先選択画面の場合のみ注文者情報を表示
				//whgcdivShippingMethod.Visible = true;
				whgcdivShippingInputFormInner.Visible = false;
				whgcdivShippingInputFormConvenience.Visible = false;
				wlShippingName1.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Name1);
				wlShippingName2.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Name2);
				wlShippingNameKana1.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.NameKana1);
				wlShippingNameKana2.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.NameKana2);

				if (this.CartList.Owner.IsAddrJp)
				{
					wlShippingZip.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Zip1 + "-" + this.CartList.Owner.Zip2);
					wlShippingZipGlobal.Text = string.Empty;
				}
				else
				{
					wlShippingZip.Text = string.Empty;
					wlShippingZipGlobal.Text = wlShippingZipGlobal.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Zip);
				}

				wlShippingAddr1.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Addr1);
				wlShippingAddr2.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Addr2);
				wlShippingAddr3.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Addr3);
				wlShippingAddr4.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Addr4);
				wlShippingAddr5.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.Addr5);
				wlShippingCountryName.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.AddrCountryName);
				wlShippingCompanyName.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.CompanyName);
				wlShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(this.CartList.Owner.CompanyPostName);
				wlShippingTel1.Text = IsCountryJp(this.CartList.Owner.AddrCountryIsoCode)
					? WebSanitizer.HtmlEncode(this.CartList.Owner.Tel1_1 + "-" + this.CartList.Owner.Tel1_2 + "-" + this.CartList.Owner.Tel1_3)
					: WebSanitizer.HtmlEncode(this.CartList.Owner.Tel1);
				wdvShippingDateTime.Visible = true;
				wddShippingReceivingStoreType.Visible = false;
				whgcdivStorePickup.Visible = false;
				wlStorePickupInvalidMessage.Visible = false;
				wlShipToCart1AddressInvalidMessage.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				whgcdivShippingVisibleConvenienceStore.Visible = false;
				whgcdivShippingDisp.Visible = false;
				whgcdivShippingInputFormInner.Visible = true;
				whgcdivShippingTime.Visible = false;
				//whgcdivShippingMethod.Visible = true;
				whgcdivShippingInputFormConvenience.Visible = true;
				whgcdivSaveToUserShipping.Visible = true;
				whgcdivButtonOpenConvenienceStoreMapPopup.Visible = true;
				isShippingConvenience = true;
				wdvShippingDateTime.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
				wddlShippingReceivingStoreType.Items.Clear();
				wddlShippingReceivingStoreType.Items.AddRange(ShippingReceivingStoreType());
				wddlShippingReceivingStoreType.Items.Cast<ListItem>().ToList()
					.ForEach(li => li.Selected = (li.Value == shippingReceivingStoreTypeSelected));
				wddShippingReceivingStoreType.Visible = true;
				whgcdivStorePickup.Visible = false;
				wlStorePickupInvalidMessage.Visible = false;
				wlShipToCart1AddressInvalidMessage.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP:
				whgcdivStorePickup.Visible = (cartIndex == 0)
					? true
					: (wcbShipToCart1Address.Checked == false);
				whgcdivShippingDisp.Visible = false;
				whgcdivShippingInputFormInner.Visible = false;
				whgcdivShippingInputFormConvenience.Visible = false;
				whgcdivSaveToUserShipping.Visible = false;
				wdvShippingDateTime.Visible = false;
				wddShippingReceivingStoreType.Visible = false;
				//whgcdivShippingMethod.Visible = false;
				//wdvDeliveryCompany.Visible = false;
				whgcdivShippingTime.Visible = false;
				this.CanStorePickupVisible = true;
				wlStorePickupInvalidMessage.Visible = true;
				wlStorePickupInvalidMessage.Text = CheckProductBuyStorePickup(cartIndex);
				break;

			default:
				whgcdivShippingVisibleConvenienceStore.Visible = false;
				whgcdivShippingDisp.Visible = true;
				whgcdivSaveToUserShipping.Visible = false;
				//whgcdivShippingMethod.Visible = true;
				whgcdivButtonOpenConvenienceStoreMapPopup.Visible = false;
				whgcdivShippingInputFormInner.Visible = false;
				wddShippingReceivingStoreType.Visible = false;
				whgcdivStorePickup.Visible = false;
				wlStorePickupInvalidMessage.Visible = false;
				wlShipToCart1AddressInvalidMessage.Visible = false;

				// ユーザ配送先情報画面セット
				var userShipping = this.UserShippingAddr.FirstOrDefault(x => x.ShippingNo.ToString().Equals(wddlShippingKbnList.SelectedValue));
				hasError = (ShippingCountryUtil.CheckShippingCountryAvailable(this.ShippingAvailableCountryList, userShipping.ShippingCountryIsoCode) == false);
				if (userShipping.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					whgcdivShippingInputFormConvenience.Visible = false;
					whgcdivShippingTime.Visible = true;
					whgcdivShippingDisp.Visible = true;
				}
				else
				{
					whgcdivShippingInputFormConvenience.Visible = true;
					whgcdivShippingTime.Visible = false;
					whgcdivShippingDisp.Visible = false;
				}
				if (userShipping != null)
				{
					wlShippingName1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName1);
					wlShippingName2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName2);
					wlShippingNameKana1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingNameKana1);
					wlShippingNameKana2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingNameKana2);
					wlShippingAddr1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr1);
					wlShippingAddr2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr2);
					wlShippingAddr3.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr3);
					wlShippingAddr4.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr4);
					wlShippingAddr5.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr5);
					wlShippingCountryName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCountryName);
					wlShippingCompanyName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCompanyName);
					wlShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCompanyPostName);
					wlShippingTel1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingTel1);
					this.CountryIsoCode = userShipping.ShippingCountryIsoCode;
					if (IsCountryJp(userShipping.ShippingCountryIsoCode))
					{
						wlShippingZip.Text = WebSanitizer.HtmlEncode(userShipping.ShippingZip);
						wlShippingZipGlobal.Text = string.Empty;
					}
					else
					{
						wlShippingZip.Text = string.Empty;
						wlShippingZipGlobal.Text = WebSanitizer.HtmlEncode(userShipping.ShippingZip);
						wdvShippingDateTime.Visible = false;
					}

					wlCvsShopName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName1);
					whfCvsShopName.Value = userShipping.ShippingName1;
					wlCvsShopId.Text = WebSanitizer.HtmlEncode(userShipping.ShippingReceivingStoreId);
					whfCvsShopId.Value = userShipping.ShippingReceivingStoreId;
					wlCvsShopAddress.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr4);
					whfCvsShopAddress.Value = userShipping.ShippingAddr4;
					wlCvsShopTel.Text = WebSanitizer.HtmlEncode(userShipping.ShippingTel1);
					whfCvsShopTel.Value = userShipping.ShippingTel1;

					isShippingConvenience = (userShipping.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
					if (isShippingConvenience && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
					{
						wddlShippingReceivingStoreType.Items.Clear();
						wddlShippingReceivingStoreType.AddItem(new ListItem(
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE,
								userShipping.ShippingReceivingStoreType),
							userShipping.ShippingReceivingStoreType));
						wddShippingReceivingStoreType.Visible = true;
					}
				}
				break;
		}

		if (wcbShipToCart1Address.Checked
			&& (cartIndex != 0))
		{
			isShippingConvenience =
				(this.CartList.Items[0].GetShipping().ConvenienceStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
		}

		// 配送先区分をセットする
		this.CartList.Items[cartIndex].Shippings[shippingNo].ShippingAddrKbn = wddlShippingKbnList.SelectedValue;

		// Calculate shipping date range when shipping convenience
		CalculateShippingDate(cartIndex, isShippingConvenience, wddlShippingKbnList);
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& (cartIndex == 0)
			&& (this.IsGiftPage == false))
		{
			var shippingKbnCart1 = wddlShippingKbnList.SelectedValue;
			foreach (RepeaterItem item in this.WrCartList.Items)
			{
				var cbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(item, "cbShipToCart1Address");
				var wddlOtherCartShippingKbnList = GetWrappedControl<WrappedDropDownList>(item, "ddlShippingKbnList");
				var cart = this.CartList.Items[0];

				// Show Check Box Ship To Cart 1 Address
				if (cart.IsShowDisplayConvenienceStore)
				{
					var shippingKbn = wddlOtherCartShippingKbnList.Items.FindByValue(shippingKbnCart1);
					if (shippingKbn == null)
					{
						cbShipToCart1Address.Checked = false;
						cbShipToCart1Address_OnCheckedChanged(cbShipToCart1Address, e);
						cbShipToCart1Address.Visible = false;
					}
					else
					{
						cbShipToCart1Address.Visible = true;
					}
				}

				if (cbShipToCart1Address.Checked)
				{
					var cartShipping = cart.GetShipping();
					CalculateShippingDate(
						item.ItemIndex,
						cartShipping.ConvenienceStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON,
						wddlOtherCartShippingKbnList);
				}
			}
		}

		if (isShippingConvenience)
		{
			var shippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;
			var wlConvenienceStoreErrorMessage = GetWrappedControl<WrappedLiteral>(
				this.WrCartList.Items[cartIndex],
				"lConvenienceStoreErrorMessage");
			wlConvenienceStoreErrorMessage.Text = string.Empty;

			if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(
				this.CartList.Items[cartIndex],
				shippingReceivingStoreType))
			{
				var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(shippingReceivingStoreType);
				wlConvenienceStoreErrorMessage.Text = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
						.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());

				var wlOrderErrorMessage = GetWrappedControl<WrappedLiteral>(((Control)sender), "lOrderErrorMessage");
				wlOrderErrorMessage.Text = (wcbShipToCart1Address.Checked)
							? wlConvenienceStoreErrorMessage.Text
							: wlOrderErrorMessage.Text.Replace(wlConvenienceStoreErrorMessage.Text, string.Empty);
			}
		}

		if (this.IsGiftPage == false)
		{
			this.CartList.Items[cartIndex].IsShippingConvenienceStore = isShippingConvenience;
			CreateSelectableDeliveryCompanyListOnDataBind();
			wdvDeliveryCompany.Visible = (CanInputShippingTo(cartIndex)
				&& CanDisplayDeliveryCompany(cartIndex));
			whgcdivShippingMethod.Visible = IsShippingStorePickup(cartIndex) == false;
			CreateDeliveryCompanyDropDownListItem(wddlDeliveryCompany, cartIndex, shippingNo);
		}

		if (cartIndex == 0
			&& WrCartList.Items.Count != 1
			&& Constants.REALSHOP_OPTION_ENABLED
			&& Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			foreach (RepeaterItem item in this.WrCartList.Items)
			{
				if (item.ItemIndex == 0) continue;
				var cbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(item, "cbShipToCart1Address");

				if (cbShipToCart1Address.Checked)
				{
					cbShipToCart1Address_OnCheckedChanged(cbShipToCart1Address, null);
				}
			}
		}

		var deliveryCompanyId = this.CartList.Items[cartIndex].Shippings[shippingNo].DeliveryCompanyId;
		if (wddlDeliveryCompany.Items.FindByValue(deliveryCompanyId) != null)
		{
			wddlDeliveryCompany.SelectedValue = deliveryCompanyId;
		}

		wlShippingCountryErrorMessage.Text = hasError
			? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE)
			: string.Empty;
		this.HasUserShippingError = hasError;
	}

	/// <summary>
	/// Calculate Shipping Date
	/// </summary>
	/// <param name="cartIndex">Cart Index</param>
	/// <param name="isShippingConvenience">Is Shipping Convenience</param>
	/// <param name="wddlShippingKbnList">Drop down list Shipping kbn</param>
	public void CalculateShippingDate(int cartIndex, bool isShippingConvenience, WrappedDropDownList wddlShippingKbnList)
	{
		if (this.IsGiftPage == false)
		{
			this.CartList.Items[cartIndex].GetShipping().ConvenienceStoreFlg = isShippingConvenience
				? Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON
				: Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			var divShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(wddlShippingKbnList.Parent, "divShippingTime");
			divShippingTime.Visible = (isShippingConvenience == false);
			var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(wddlShippingKbnList.Parent, "ddlShippingDate");
			if (wddlShippingDate.HasInnerControl)
			{
				var selectedValue = this.CartList.Items[cartIndex].GetShipping().ShippingDate.HasValue
					? this.CartList.Items[cartIndex].GetShipping().ShippingDate.Value.ToString("yyyy/MM/dd")
					: CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
				wddlShippingDate.DataSource = GetShippingDateList(
					this.CartList.Items[cartIndex],
					this.ShopShippingList[cartIndex]);
				wddlShippingDate.DataBind();

				// Set selected value
				if (wddlShippingDate.Items.FindByValue(selectedValue) != null)
				{
					wddlShippingDate.SelectedValue = selectedValue;
				}
			}
		}
		else
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(this.WrCartList.Items[cartIndex], "rCartShippings");
			foreach (RepeaterItem riShipping in wrCartShippings.Items)
			{
				var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingDate");
				if (wddlShippingDate.HasInnerControl)
				{
					var selectedValue = this.CartList.Items[cartIndex].Shippings[riShipping.ItemIndex].ShippingDate.HasValue
						? this.CartList.Items[cartIndex].Shippings[riShipping.ItemIndex].ShippingDate.Value.ToString("yyyy/MM/dd")
						: CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
					if (wddlShippingDate.Items.FindByValue(selectedValue) != null)
					{
						wddlShippingDate.SelectedValue = selectedValue;
					}
				}
			}
		}
	}

	/// <summary>
	/// 配送先を保存するラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rblSaveToUserShipping_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		WrappedRadioButtonList wrblSaveToUserShipping = null;
		if (sender is RadioButtonList)
		{
			wrblSaveToUserShipping = GetWrappedControl<WrappedRadioButtonList>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is WrappedRadioButtonList)
		{
			wrblSaveToUserShipping = (WrappedRadioButtonList)sender;
		}
		else
		{
			return;
		}

		// ラップ済みコントロール宣言
		var whgcdlUserShippingName = GetWrappedControl<WrappedHtmlGenericControl>(wrblSaveToUserShipping.Parent, "dlUserShippingName");

		whgcdlUserShippingName.Visible = (wrblSaveToUserShipping.SelectedValue == "1");
	}

	/// <summary>
	/// 配送方法ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlShippingMethodList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		var cartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var shippingControl = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(shippingControl ?? cartControl);
		var wddlDeliveryCompany = CreateWrappedDropDownListDeliveryCompany(shippingControl ?? cartControl);
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(shippingControl ?? cartControl, "ddlShippingKbnList");
		var wdvShippingDateTime = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dvShipppingDateTime");

		var index = cartControl.ItemIndex;

		var isExpress = (wddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);

		var cart = this.CartList.Items[index];
		var shopShipping = GetShopShipping(cart.ShopId, cart.ShippingType);

		var shippingNo = (shippingControl != null) ? shippingControl.ItemIndex : 0;
		cart.Shippings[shippingNo].ShippingMethod = wddlShippingMethod.SelectedValue;
		cart.Shippings[shippingNo].UserSelectedShippingMethod = true;
		this.SelectedShippingMethod[index] = wddlShippingMethod.SelectedValue;
		wdvShippingDateTime.Visible = (CanInputDateOrTimeSet(index)
			&& ((Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
				|| (cart.Shippings[0].ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)));

		// カートに設定した配送サービス調整
		var newCompanyListItem = GetDeliveryCompanyListItem(index, shippingNo);
		var defaultCompanyId = (isExpress
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail)
			.First(company => company.IsDefault).DeliveryCompanyId;
		if ((wddlDeliveryCompany.HasInnerControl == false)
			|| (newCompanyListItem.Any(item => (item.Key == cart.Shippings[shippingNo].DeliveryCompanyId)) == false))
		{
			// 指定した配送方法に合わせて、カートの配送サービスをデフォルト配送サービスに変更
			cart.Shippings[shippingNo].DeliveryCompanyId = defaultCompanyId;
		}

		if (wddlDeliveryCompany.HasInnerControl)
		{
			CreateSelectableDeliveryCompanyListOnDataBind();
			// 配送サービ項目に該当のデータを反映
			CreateDeliveryCompanyDropDownListItem(wddlDeliveryCompany, index, shippingNo);

			if (wddlDeliveryCompany.Items.FindByValue(cart.Shippings[shippingNo].DeliveryCompanyId) != null)
			{
				wddlDeliveryCompany.SelectedValue = cart.Shippings[shippingNo].DeliveryCompanyId;
			}
			wdvShippingDateTime.Visible = CanDisplayDeliveryCompany(index, shippingNo);
		}

		// 代金引換が選択されて、メール便に変更し直した場合は決済画面へ遷移させる
		if ((isExpress == false)
			&& (cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}

		// 配送希望日時の表示有無更新（配送希望日時の表示有無はカート配送方法更新後にする必要がある。）
		wdvShippingDateTime.Visible = CanInputDateOrTimeSet(index, shippingNo);
		var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dtShippingDate");
		var wddShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "ddShippingDate");
		var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlShippingDate");
		if ((wdtShippingDate.InnerControl != null) && (wddShippingDate.InnerControl != null))
		{
			wdtShippingDate.Visible = wddShippingDate.Visible = CanInputDateSet(index) && wddlShippingDate.Visible;
		}

		// 配送希望時間帯の表示有無更新（配送希望時間帯の表示有無はカート配送サービス更新後にする必要がある。）
		ControlShippingTimeInput(shippingControl ?? cartControl, shippingNo);

		// Update First shipping date and next shipping date if not landing page
		if ((this.IsLandingPage == false)
			&& (cart.Items.Any(cp => cp.IsGift) == false))
		{
			CheckShippingPatternForFirstShippingDate(sender);
			UpdateNextShippingDateDropDownList(sender);
			UpdateFirstShippingDateDropDownList(sender, e);
		}
	}

	/// <summary>
	/// ddlDeliveryCompanyのItemを作成する
	/// </summary>
	/// <param name="wddlDeliveryCompany">ddlDeliveryCompanyのWrappedControl</param>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <remarks>元々DataBindで更新していたが、例外が発生するためこちらに切り替える</remarks>
	private void CreateDeliveryCompanyDropDownListItem(
		WrappedDropDownList wddlDeliveryCompany,
		int cartIndex,
		int shippingIndex)
	{
		wddlDeliveryCompany.Items.Clear();
		wddlDeliveryCompany.Items.AddRange(
			GetDeliveryCompanyListItem(cartIndex, shippingIndex).Select(kvp => new ListItem(kvp.Value, kvp.Key))
				.ToArray());
	}

	/// <summary>
	/// 注文者国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlOwnerCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return;

		var riCart = this.WrCartList.Items[0];
		var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(riCart, "ddlOwnerCountry");
		if ((Constants.GIFTORDER_OPTION_ENABLED == false) || (wddlOwnerCountry.InnerControl != null))
		{
			this.CartList.Owner.AddrCountryIsoCode = GetWrappedControl<WrappedDropDownList>(riCart, "ddlOwnerCountry").SelectedValue;
		}
		var ownerAddrCountryIsoCode = this.CartList.Owner.AddrCountryIsoCode;

		// 国変更向けグローバル国設定CSS付与（クライアント検証用）
		AddCountryInfoToControlForChangeCountryForOrderShipping(riCart, "Owner", ownerAddrCountryIsoCode);

		if (this.CartList.Items.Any(c => c.IsDigitalContentsOnly)) return;

		var isOwnerAddrCountryJapan = IsCountryJp(ownerAddrCountryIsoCode);
		var isOwnerAddrCountryTaiwan = IsCountryTw(ownerAddrCountryIsoCode);

		// 注文者の国が配送先に利用できるか判定
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingKbnList");
		var wlShippingCountryErrorMessage = GetWrappedControl<WrappedLiteral>(riCart, "lShippingCountryErrorMessage");
		if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
		{
			this.HasUserShippingError = (ShippingCountryUtil.CheckShippingCountryAvailable(this.ShippingAvailableCountryList, ownerAddrCountryIsoCode) == false);
			wlShippingCountryErrorMessage.Text = this.HasUserShippingError
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE)
				: string.Empty;
		}

		// 国変更向けValidationGroup変更処理
		ChangeValidationGroupForChangeCountry(
			riCart,
			"Owner",
			isOwnerAddrCountryJapan ? "OrderShipping" : "OrderShippingGlobal");

		// Display Zip Global
		if (IsCountryTw(ownerAddrCountryIsoCode))
		{
			BindingDdlOwnerAddr3((Control)sender);
			var wtbOwnerZipGlobal = GetWrappedControl<WrappedTextBox>(riCart, "tbOwnerZipGlobal");
			var wddlOwnerAddr3 = GetWrappedControl<WrappedDropDownList>(riCart, "ddlOwnerAddr3");
			if (string.IsNullOrEmpty(wddlOwnerAddr3.SelectedValue) == false) wtbOwnerZipGlobal.Text = wddlOwnerAddr3.SelectedValue;
		}

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			foreach (RepeaterItem riCartItem in this.WrCartList.Items)
			{
				var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(riCartItem, "cbShipToCart1Address");
				var wddlShippingKbnListItem = GetWrappedControl<WrappedDropDownList>(riCartItem, "ddlShippingKbnList");
				var wsInvoicesItem = GetWrappedControl<WrappedHtmlGenericControl>(riCartItem, "sInvoices");

				if (((riCartItem.ItemIndex != 0)
					&& wcbShipToCart1Address.Checked
					&& (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER))
					|| (wddlShippingKbnListItem.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER))
				{
					wsInvoicesItem.Visible = isOwnerAddrCountryTaiwan;
				}
			}
		}

		if (IsPostBack
			&& Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
			var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(isOwnerAddrCountryJapan, this.FirstRpeaterItem);
			var wlbGetAuthenticationCode = GetWrappedControlOfLinkButtonAuthenticationCode(isOwnerAddrCountryJapan, this.FirstRpeaterItem);
			var wtbOwnerTel1_1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_1");
			var wtbOwnerTel1_2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_2");
			var wtbOwnerTel1_3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_3");
			var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1");
			var wtbOwnerTel1Global = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1Global");
			var wtbAuthenticationStatus = GetWrappedControlOfLabelAuthenticationStatus(isOwnerAddrCountryJapan, this.FirstRpeaterItem);

			if (isOwnerAddrCountryJapan)
			{
				if (wtbOwnerTel1_1.HasInnerControl)
				{
					wtbOwnerTel1_1.Text
						= wtbOwnerTel1_2.Text
						= wtbOwnerTel1_3.Text
						= string.Empty;
				}
				else
				{
					wtbOwnerTel1.Text = string.Empty;
				}
			}
			else
			{
				wtbOwnerTel1Global.Text = string.Empty;
			}

			// Stop process validation when changing country
			StopTimeCount();
			this.HasAuthenticationCode = false;
			wtbAuthenticationStatus.Text = string.Empty;

			DisplayAuthenticationCode(
				wlbGetAuthenticationCode,
				wtbAuthenticationCode);
		}
	}

	/// <summary>
	/// 配送先国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		var cParent = ((Control)sender).Parent;
		ddlShippingCountry_SelectedIndexChangedInner(cParent);
	}

	/// <summary>
	/// 配送先国ドロップダウンリスト選択インナーメソッド
	/// </summary>
	/// <param name="oParent">親コントロール</param>
	public void ddlShippingCountry_SelectedIndexChangedInner(Control oParent)
	{
		if (this.CartList.Items.Any(c => c.IsDigitalContentsOnly)) return;

		var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(oParent, "ddlShippingCountry");
		var shippingAddrCountryIsoCode = wddlShippingCountry.SelectedValue;
		var isShippingAddrCountryJapan = IsCountryJp(shippingAddrCountryIsoCode);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 国変更向けグローバル国設定CSS付与（クライアント検証用）
			AddCountryInfoToControlForChangeCountryForOrderShipping(oParent, "Shipping", shippingAddrCountryIsoCode);
			// 国変更向けValidationGroup変更処理
			ChangeValidationGroupForChangeCountry(
				oParent,
				"Shipping",
				isShippingAddrCountryJapan ? "OrderShipping" : "OrderShippingGlobal");

			// Display Zip Global
			if (IsCountryTw(shippingAddrCountryIsoCode))
			{
				BindingDdlShippingAddr3((Control)oParent);
				var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(oParent, "tbShippingZipGlobal");
				var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(oParent, "ddlShippingAddr3");
				if (wddlShippingAddr3.HasInnerControl) wtbShippingZipGlobal.Text = wddlShippingAddr3.SelectedValue.Split('|')[0];
			}

			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(oParent, "ddlShippingKbnList");
			var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(oParent, "ddlOwnerCountry");
			var ownerAddrCountryIsoCode = wddlOwnerCountry.SelectedValue;

			// Get country code
			var countryCode = shippingAddrCountryIsoCode;
			if (OrderCommon.DisplayTwInvoiceInfo())
			{
				if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
				{
					countryCode = ownerAddrCountryIsoCode;
				}
				else if (wddlShippingKbnList.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
				{
					var shippingNo = 0;
					if (int.TryParse(wddlShippingKbnList.SelectedValue, out shippingNo))
					{
						var userShipping = new UserShippingService().Get(this.LoginUserId, shippingNo);
						if (userShipping != null)
						{
							countryCode = userShipping.ShippingCountryIsoCode;
						}
					}
				}
				var wsInvoices = GetWrappedControl<WrappedHtmlGenericControl>(oParent, "sInvoices");
				if (string.IsNullOrEmpty(countryCode)) countryCode = ownerAddrCountryIsoCode;
				wsInvoices.Visible = IsCountryTw(countryCode);

				// For case multi cart
				foreach (RepeaterItem riCartItem in this.WrCartList.Items)
				{
					var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(riCartItem, "cbShipToCart1Address");
					var wsInvoicesItem = GetWrappedControl<WrappedHtmlGenericControl>(riCartItem, "sInvoices");

					if ((riCartItem.ItemIndex != 0)
						&& wcbShipToCart1Address.Checked)
					{
						wsInvoicesItem.Visible = IsCountryTw(countryCode);
					}
				}
			}
		}
	}

	/// <summary>
	/// 送り主国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlSenderCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		var cParent = ((Control)sender).Parent;
		var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(cParent, "ddlSenderCountry");
		var senderAddrCountryIsoCode = wddlSenderCountry.SelectedValue;
		var isSenderAddrCountryJapan = IsCountryJp(senderAddrCountryIsoCode);

		if (this.CartList.Items.Any(c => c.IsDigitalContentsOnly)) return;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 国変更向けグローバル国設定CSS付与（クライアント検証用）
			AddCountryInfoToControlForChangeCountryForOrderShipping(cParent, "Sender", senderAddrCountryIsoCode);
			// 国変更向けValidationGroup変更処理
			ChangeValidationGroupForChangeCountry(
				cParent,
				"Sender",
				isSenderAddrCountryJapan ? "OrderShipping" : "OrderShippingGlobal");

			// Display Zip Global
			if (IsCountryTw(senderAddrCountryIsoCode))
			{
				BindingDdlSenderAddr3(((Control)sender).Parent);
				var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(cParent, "tbSenderZipGlobal");
				var wddlSenderAddr3 = GetWrappedControl<WrappedDropDownList>(cParent, "ddlSenderAddr3");
				wtbSenderZipGlobal.Text = wddlSenderAddr3.SelectedValue.Split('|')[0];
			}
		}
	}

	/// <summary>
	/// 国情報をコントロールのCSSクラス情報に付与（注文配送先向け）
	/// </summary>
	/// <param name="cParent">親コントロール</param>
	/// <param name="targetKbn">対象区分（Owner/Shipping/Sender）</param>
	/// <param name="countryIsoCode"></param>
	private void AddCountryInfoToControlForChangeCountryForOrderShipping(
		Control cParent,
		string targetKbn,
		string countryIsoCode)
	{
		AddCountryInfoToControlForChangeCountry(
			new WrappedWebControl[]
			{
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Name1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Name2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}NameKana1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}NameKana2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}MailAddr", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}MailAddr2", targetKbn)),
				GetWrappedControl<WrappedDropDownList>(cParent, string.Format("ddl{0}BirthYear", targetKbn)),
				GetWrappedControl<WrappedDropDownList>(cParent, string.Format("ddl{0}BirthMonth", targetKbn)),
				GetWrappedControl<WrappedDropDownList>(cParent, string.Format("ddl{0}BirthDay", targetKbn)),
				GetWrappedControl<WrappedRadioButtonList>(cParent, string.Format("rbl{0}Sex", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Zip1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Zip2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Addr2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Addr2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Addr2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Addr3", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Addr4", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel1_1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel1_2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel1_3", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel3", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel2_1", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel2_2", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel2_3", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}ZipGlobal", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel1Global", targetKbn)),
				GetWrappedControl<WrappedTextBox>(cParent, string.Format("tb{0}Tel2Global", targetKbn)),
			},
			countryIsoCode);
	}

	/// <summary>
	/// 国情報によりCustomValidatorのValidationGroupをセット（注文配送先向け）
	/// </summary>
	/// <param name="cParent">親コントロール</param>
	/// <param name="targetKbn">対象区分（Owner/Shipping/Sender）</param>
	/// <param name="validationGroup">設定するバリデーショングループ</param>
	private void ChangeValidationGroupForChangeCountry(Control cParent, string targetKbn, string validationGroup)
	{
		ChangeValidationGroupForChangeCountry(
			new WrappedControl[]
			{
				GetWrappedControl<WrappedCustomValidator>(cParent, string.Format("cv{0}Addr2", targetKbn)),
				GetWrappedControl<WrappedCustomValidator>(cParent, string.Format("cv{0}Addr3", targetKbn)),
				GetWrappedControl<WrappedCustomValidator>(cParent, string.Format("cv{0}Addr4", targetKbn)),
				GetWrappedControl<WrappedCustomValidator>(cParent, string.Format("cv{0}Tel1Global", targetKbn)),
				GetWrappedControl<WrappedCustomValidator>(cParent, string.Format("cv{0}Tel2Global", targetKbn)),
				GetWrappedControl<WrappedLinkButton>(cParent, "lbNext"),
			},
			validationGroup);
	}

	/// <summary>
	/// 住所項目結合
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <returns>結合した住所</returns>
	private string ConcatenateAddress(UserModel user)
	{
		string address = string.Empty;
		if (IsCountryJp(user.AddrCountryIsoCode))
		{
			address = user.Addr1 + user.Addr2 + user.Addr3 + " " + user.Addr4 + user.AddrCountryName;
		}
		else
		{
			address = user.Addr2
				+ ((string.IsNullOrEmpty(user.Addr3) == false) ? " " : "")
				+ user.Addr3
				+ " "
				+ user.Addr4
				+ ((string.IsNullOrEmpty(user.Addr5) == false) ? " " : "")
				+ user.Addr5
				+ " "
				+ user.AddrCountryName;
		}
		return address;
	}

	/// <summary>
	/// 送り主住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <returns>ISOコード</returns>
	public string GetSenderAddrCountryIsoCode(int cartIndex, int shippingIndex)
	{
		var countryIsoCode = GetAddrCountryIsoCodeInner(cartIndex, shippingIndex, "ddlSenderCountry");
		return countryIsoCode;
	}

	/// <summary>
	/// 配送先住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <returns>ISOコード</returns>
	public string GetShippingAddrCountryIsoCode(int cartIndex, int shippingIndex = 0)
	{
		var countryIsoCode = GetAddrCountryIsoCodeInner(cartIndex, shippingIndex, "ddlShippingCountry");
		return countryIsoCode;
	}

	/// <summary>
	/// 国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <param name="addrCountryControlId">住所国コントロールID</param>
	/// <returns>ISOコード</returns>
	public string GetAddrCountryIsoCodeInner(int cartIndex, int shippingIndex, string addrCountryControlId)
	{
		var wddlCountry = GetWrappedControl<WrappedDropDownList>(this.WrCartList.Items[cartIndex], addrCountryControlId);
		if (wddlCountry.HasInnerControl == false)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(this.WrCartList.Items[cartIndex], "rCartShippings");
			if (wrCartShippings.HasInnerControl && (wrCartShippings.Items.Count > shippingIndex))
			{
				wddlCountry = GetWrappedControl<WrappedDropDownList>(wrCartShippings.Items[shippingIndex], addrCountryControlId);
			}
		}
		return wddlCountry.SelectedValue;
	}

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択(注文者)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlOwnerAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		BindingDdlOwnerAddr3((Control)sender);
	}

	/// <summary>
	/// 台湾地域ドロップダウンリスト生成(注文者)
	/// </summary>
	/// <param name="control">Control</param>
	public void BindingDdlOwnerAddr3(Control control)
	{
		var wddlOwnerAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr2");
		var wddlOwnerAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr3");
		var wtbOwnerZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerZipGlobal");
		var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZipGlobal");
		wddlOwnerAddr3.DataSource = this.UserTwDistrictDict[wddlOwnerAddr2.SelectedItem.ToString()];
		wddlOwnerAddr3.DataBind();
		if (this.IsLandingPage)
		{
			this.CartList.Owner.Addr2 = wddlOwnerAddr2.SelectedValue;
			this.CartList.Owner.Addr3 = wddlOwnerAddr3.SelectedText;
		}

		var zipGlobalValue = (string.IsNullOrEmpty(wddlOwnerAddr3.SelectedText))
			? string.Empty
			: wddlOwnerAddr3.SelectedValue;
		wtbOwnerZipGlobal.Text = zipGlobalValue;
		wtbSenderZipGlobal.Text = zipGlobalValue;
	}

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択(配送先)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlShippingAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		BindingDdlShippingAddr3((Control)sender);
	}

	/// <summary>
	/// 台湾地域ドロップダウンリスト生成(配送先)
	/// </summary>
	/// <param name="control">Control</param>
	public void BindingDdlShippingAddr3(Control control)
	{
		var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr2");
		var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr3");
		var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingZipGlobal");

		wddlShippingAddr3.DataSource = this.UserTwDistrictDict[wddlShippingAddr2.SelectedItem.ToString()];
		wddlShippingAddr3.DataBind();

		wtbShippingZipGlobal.Text = (string.IsNullOrEmpty(wddlShippingAddr3.SelectedText))
			? string.Empty
			: wddlShippingAddr3.SelectedValue.Split('|')[0]; ;

	}
	/// <summary>
	/// 台湾都市ドロップダウンリスト選択(依頼先)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlSenderAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		BindingDdlSenderAddr3((Control)sender);
	}

	/// <summary>
	/// 台湾地域ドロップダウンリスト生成(依頼先)
	/// </summary>
	/// <param name="control">Control</param>
	public void BindingDdlSenderAddr3(Control control)
	{
		var wddlSenderAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr2");
		var wddlSenderAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr3");
		var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZipGlobal");

		wddlSenderAddr3.DataSource = this.UserTwDistrictDict[wddlSenderAddr2.SelectedItem.ToString()];
		wddlSenderAddr3.DataBind();

		wtbSenderZipGlobal.Text = (string.IsNullOrEmpty(wddlSenderAddr3.SelectedText))
			? string.Empty
			: wddlSenderAddr3.SelectedValue;
	}

	/// <summary>
	/// Display Shop Data
	/// </summary>
	public void DisplayShopData()
	{
		foreach (RepeaterItem item in this.WrCartList.Items)
		{
			var wlCvsShopId = GetWrappedControl<WrappedLiteral>(item, "lCvsShopId");
			var wlCvsShopName = GetWrappedControl<WrappedLiteral>(item, "lCvsShopName");
			var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(item, "lCvsShopAddress");
			var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(item, "lCvsShopTel");
			var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(item, "hfCvsShopId");
			var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(item, "hfCvsShopName");
			var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(item, "hfCvsShopAddress");
			var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(item, "hfCvsShopTel");
			wlCvsShopId.Text = whfCvsShopId.Value;
			wlCvsShopName.Text = whfCvsShopName.Value;
			wlCvsShopAddress.Text = whfCvsShopAddress.Value;
			wlCvsShopTel.Text = whfCvsShopTel.Value;
		}
	}

	/// <summary>
	/// 注文者住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートNO</param>
	/// <returns>ISOコード</returns>
	public string GetOwnerAddrCountryIsoCode(int cartIndex)
	{
		var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.WrCartList.Items[cartIndex], "ddlOwnerCountry");
		return wddlOwnerCountry.SelectedValue;
	}

	/// <summary>
	/// 初期値男性で性別を取得（データバインド用）
	/// </summary>
	/// <param name="sex">性別</param>
	/// <returns>性別</returns>
	public string GetCorrectSexForDataBindDefault(string sex = null)
	{
		var result = (sex != null) ? GetCorrectSexForDataBind(sex) : GetCorrectSexForDataBind(this.CartList.Owner.Sex);
		return result ?? ReplaceTag("@@User.sex.default@@");
	}

	/// <summary>
	/// 正しい性別情報取得（データバインド用）
	/// </summary>
	/// <param name="sex">性別</param>
	/// <returns>性別</returns>
	public string GetCorrectSexForDataBind(string sex)
	{
		var sexValues = ValueText.GetValueKvpArray(Constants.TABLE_USER, Constants.FIELD_USER_SEX);
		var isValidSex = (sexValues.Any(kvp => (kvp.Key == this.CartList.Owner.Sex)));
		var result = isValidSex ? this.CartList.Owner.Sex : null;
		return result;
	}

	/// <summary>
	/// Dropdownlist real shop area on databound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlRealShopArea_DataBound(object sender, EventArgs e)
	{
		if (sender is DropDownList) ((DropDownList)sender).Items.Insert(0, new ListItem(string.Empty, string.Empty));
	}

	#endregion

	#region 配送希望日系
	/// <summary>
	/// 配送希望日ドロップダウンリストの最短日取得
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>選択可能な最短配送希望日</returns>
	public DateTime? GetShortestShippingDate(CartObject coCart, ShopShippingModel shopShipping)
	{
		var shippingDateList = GetShippingDateList(coCart.GetShipping(), shopShipping);
		if (shippingDateList.Count == 0) return null;
		var shortestDate = shippingDateList[0];
		DateTime result;
		return DateTime.TryParse(shortestDate.Value, out result)
			? result
			: (DateTime?)null;
	}

	/// <summary>
	/// 配送希望日ドロップダウンリスト作成
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日ドロップダウンリスト</returns>
	public ListItemCollection GetShippingDateList(CartObject coCart, ShopShippingModel shopShipping)
	{
		return GetShippingDateList(coCart.GetShipping(), shopShipping);
	}
	/// <summary>
	/// 配送希望日ドロップダウンリスト作成
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日ドロップダウンリスト</returns>
	public ListItemCollection GetShippingDateList(CartShipping csShipping, ShopShippingModel shopShipping)
	{
		var licShippingDate = new ListItemCollection();

		if (shopShipping.IsValidShippingDateSetFlg)
		{
			if ((csShipping != null)
				&& (csShipping.ConvenienceStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			{
				// Get Shipping Date For ConvenienceStore
				licShippingDate.AddRange(OrderCommon.GetShippingDateForConvenienceStore(true));
			}
			else
			{
				// 配送希望日リスト作成
				licShippingDate = OrderCommon.GetListItemShippingDate(shopShipping, true);
			}

			if (csShipping.ShippingDate.HasValue
				&& ((csShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
					|| (csShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)))
			{
				var shippingDateList = licShippingDate.Cast<ListItem>().ToArray();
				var isSelected = shippingDateList
					.Count(listItem => listItem.Value == csShipping.ShippingDate.Value.ToString("yyyy/MM/dd")) > 0;
				if (isSelected == false)
				{
					licShippingDate.Add(new ListItem(
						DateTimeUtility.ToStringFromRegion(
							csShipping.ShippingDate.Value,
							DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
						csShipping.ShippingDate.Value.ToString("yyyy/MM/dd")));
				}
			}

			// カート内に配達希望日が設定されているがリストに無い場合、カートの設定をクリアする
			if ((csShipping != null)
				&& (csShipping.ShippingDate.HasValue)
				&& (licShippingDate.FindByValue(csShipping.ShippingDate.Value.ToString("yyyy/MM/dd")) == null))
			{
				csShipping.ShippingDate = null;
			}
		}

		return licShippingDate;
	}

	/// <summary>
	/// 配送希望時間帯ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送希望時間帯ドロップダウンリスト</returns>
	public ListItemCollection GetShippingTimeList(int index, int shippingNo = 0)
	{
		// 指定した配送情報の中の配送サービスIDを取得
		var deliveryCompanyId = this.CartList.Items[index].Shippings[shippingNo].DeliveryCompanyId;
		return GetShippingTimeList(deliveryCompanyId);
	}
	/// <summary>
	/// 配送希望時間帯ドロップダウンリスト作成
	/// </summary>
	/// <param name="deliveryCompanyId">配送サービスID</param>
	/// <returns>配送希望時間帯ドロップダウンリスト</returns>
	public ListItemCollection GetShippingTimeList(string deliveryCompanyId)
	{
		var listItem = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), string.Empty)
		};

		// ViewStateより配送サービス情報がなければ、DBから配送サービス情報を取得
		var deliveryCompany = ((this.DeliveryCompanyList == null)
			|| (this.DeliveryCompanyList.Any(c => (c.DeliveryCompanyId == deliveryCompanyId)) == false))
				? new DeliveryCompanyService().Get(deliveryCompanyId)
				: this.DeliveryCompanyList.First(c => (c.DeliveryCompanyId == deliveryCompanyId));
		if ((deliveryCompany != null)
			&& deliveryCompany.IsValidShippingTimeSetFlg)
		{
			listItem.AddRange(deliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
		}
		return listItem;
	}

	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日</returns>
	public string GetShippingDate(CartObject coCart, ShopShippingModel shopShipping)
	{
		return GetShippingDate(coCart.GetShipping(), shopShipping);
	}
	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日</returns>
	public string GetShippingDate(CartShipping csShipping, ShopShippingModel shopShipping)
	{
		//------------------------------------------------------
		// 配送希望日指定不可の場合、DropDownListのSelectedValueにnullが設定される
		//------------------------------------------------------
		if (shopShipping.IsValidShippingDateSetFlg == false) return null;

		// メール便の場合、null
		if (csShipping.IsMail) return null;

		//------------------------------------------------------
		// 配送希望日が指定されていれば、配送希望日を返す
		//------------------------------------------------------
		if (csShipping != null)
		{
			if (csShipping.SpecifyShippingDateFlg && csShipping.ShippingDate.HasValue)
			{
				return csShipping.ShippingDate.Value.ToString("yyyy/MM/dd");
			}
		}

		// セッティングできなかったらデフォルト値「指定なし」セット
		if (shopShipping.IsValidShippingDateSetFlg == false) return null;

		// デフォルト値表示可の場合はデフォルト値をセット
		return Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED
			? string.Empty
			: null;
	}

	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="cartNo">rCartListのインデックス</param>
	/// <param name="shippingNo">rCartShippingsのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	public string GetShippingTime(int cartNo, int shippingNo = 0)
	{
		var result = GetShippingTime(this.CartList.Items[cartNo].Shippings[shippingNo], cartNo);
		return result;
	}
	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	public string GetShippingTime(CartObject coCart, int index)
	{
		return GetShippingTime(coCart.GetShipping(), index);
	}
	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	public string GetShippingTime(CartShipping csShipping, int index)
	{
		// 配送希望日時間帯指定不可の場合、DropDownListのSelectedValueにnullが設定される
		var deliveryCompanyId = csShipping.DeliveryCompanyId;
		var deliveryCompany = this.DeliveryCompanyList
			.FirstOrDefault(company => company.DeliveryCompanyId == deliveryCompanyId);
		if ((deliveryCompany != null)
			&& deliveryCompany.IsValidShippingTimeSetFlg == false)
		{
			return null;
		}

		// メール便の場合、null
		if (csShipping.IsMail) return null;

		// 配送日時希望が希望 & 配送希望時間帯設定可能フラグが有効の場合、カートの希望時間帯を返す
		if (csShipping != null)
		{
			return csShipping.ShippingTime;
		}

		return null;
	}

	/// <summary>
	/// Display shipping date error message
	/// </summary>
	/// <returns>Return true if has error, otherwise return false</returns>
	public bool DisplayShippingDateErrorMessage()
	{
		var hasError = false;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var cart = this.CartList.Items[riCart.ItemIndex];
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");

			for (int index = 0; (index < wrCartShippings.Items.Count) || ((index == 0) && (wrCartShippings.Items.Count == 0)); index++)
			{
				var riTarget = (wrCartShippings.Items.Count != 0) ? wrCartShippings.Items[index] : riCart;

				var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
				var wlShippingDateErrorMessage = GetWrappedControl<WrappedLabel>(riTarget, "lShippingDateErrorMessage");
				var wlbScheduledShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lbScheduledShippingDate");

				// Reset
				wlShippingDateErrorMessage.Visible = false;
				wlShippingDateErrorMessage.Text = string.Empty;

				var shipping = cart.Shippings[index];
				var beforeShippingDate = shipping.ShippingDate;
				if (string.IsNullOrEmpty(wddlShippingDate.SelectedValue) == false) shipping.ShippingDate = DateTime.Parse(wddlShippingDate.SelectedValue);
				if (string.IsNullOrEmpty(wlbScheduledShippingDate.Text) == false)
				{
					shipping.ShippingDateForCalculation = shipping.ShippingDate;
					shipping.CalculateScheduledShippingDate(
						cart.ShopId,
						shipping.ShippingCountryIsoCode,
						shipping.IsTaiwanCountryShippingEnable ? shipping.Addr2 : shipping.Addr1,
						shipping.Zip);
				}

				// Display error message if existing
				if (shipping.ShippingDate.HasValue
					&& (OrderCommon.CanCalculateScheduledShippingDate(this.ShopId, shipping) == false))
				{
					hasError = true;
					wlShippingDateErrorMessage.Visible = true;
					wlShippingDateErrorMessage.Text = CreateShippingDateErrorMessage(shipping);
					this.CartList.Items[riCart.ItemIndex].Shippings[index].ShippingDate = beforeShippingDate;
				}
			}
		}

		return hasError;
	}

	/// <summary>
	/// 配送希望日についてのエラーメッセージ作成
	/// </summary>
	/// <param name="shipping">配送先情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CreateShippingDateErrorMessage(CartShipping shipping)
	{
		var message = WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID)
			.Replace("@@ 1 @@", DateTimeUtility.ToStringFromRegion(
				HolidayUtil.GetShortestDeliveryDate(
					this.ShopId,
					shipping.DeliveryCompanyId,
					shipping.IsTaiwanCountryShippingEnable
						? shipping.Addr2
						: shipping.Addr1,
					shipping.Zip.Replace("-", string.Empty)),
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter
				));
		return message;
	}

	/// <summary>
	/// 配送希望日が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートの配送希望日が指定可能であれば true</returns>
	public bool CanInputDateSet(int index, int shippingNo = 0)
	{
		var result = (this.ShopShippingList[index].IsValidShippingDateSetFlg
			&& (this.CartList.Items[index].IsDigitalContentsOnly == false)
			&& IsExpressDelivery(index, shippingNo));
		return result;
	}

	/// <summary>
	/// 配送希望時間帯が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートの配送希望時間帯が指定可能であれば true</returns>
	public bool CanInputTimeSet(int index, int shippingNo = 0)
	{
		if ((this.CartList.Items[index].IsDigitalContentsOnly)
			|| (IsExpressDelivery(index, shippingNo) == false)) return false;

		var deliveryId = this.CartList.Items[index].Shippings[shippingNo].DeliveryCompanyId;
		var result = this.DeliveryCompanyList.First(company => company.DeliveryCompanyId == deliveryId).IsValidShippingTimeSetFlg;
		return result;
	}

	/// <summary>
	/// 配送希望日or時間帯が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートで、配送希望日or時間帯のどちらか一方でも指定可能であれば true</returns>
	public bool CanInputDateOrTimeSet(int index, int shippingNo = 0)
	{
		var result = ((CanInputDateSet(index, shippingNo) 
			|| CanInputTimeSet(index, shippingNo))
			&& (this.CartList.Items[index].Shippings[0].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP) == false);
		return result;
	}
	#endregion

	/// <summary>
	/// ラップ済配送方法ドロップダウンリスト取得
	/// </summary>
	/// <param name="parent">親コントロール</param>
	/// <returns>配送方法ドロップダウンリスト</returns>
	/// <remarks>デフォルトで宅急便が選択されている。</remarks>
	public WrappedDropDownList CreateWrappedDropDownListShippingMethod(Control parent)
	{
		var wddlShippingMethod = GetWrappedControl<WrappedDropDownList>(parent, "ddlShippingMethod", Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
		return wddlShippingMethod;
	}

	/// <summary>
	/// 宅配便か
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>宅配便か？</returns>
	public bool IsExpressDelivery(int index, int shippingNo = 0)
	{
		var result = this.CartList.Items[index].Shippings[shippingNo].IsExpress;
		return result;
	}

	/// <summary>
	/// AmazonPayで配送希望日についてのチェックとエラーメッセージ表示
	/// </summary>
	/// <returns>エラーあり：TRUE；エラーなし：FALSE</returns>
	public bool CheckAndDisplayAmazonShippingDateErrorMessage()
	{
		var wlbShippingDateErrorMessage = GetWrappedControl<WrappedLabel>(this.WrCartList.Items[0], "lbShippingDateErrorMessage");
		var shipping = this.CartList.Items[0].Shippings[0];

		// エラーメッセージ表示をリセット
		wlbShippingDateErrorMessage.Visible = false;
		wlbShippingDateErrorMessage.Text = string.Empty;

		// 配送リードタイム考慮して、配送希望日についてのエラーがあれば、メッセージ表示
		if (shipping.ShippingDate.HasValue
			&& (OrderCommon.CanCalculateScheduledShippingDate(this.ShopId, shipping) == false))
		{
			wlbShippingDateErrorMessage.Visible = true;
			wlbShippingDateErrorMessage.Text = CreateShippingDateErrorMessage(shipping);
			return true;
		}

		return false;
	}

	/// <summary>
	/// 配送先が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>指定カートで、配送先が設定可能であれば true</returns>
	public bool CanInputShippingTo(int index)
	{
		return (this.CartList.Items[index].IsDigitalContentsOnly == false);
	}

	/// <summary>
	/// 配送先が店舗受取かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>指定カートで、配送先が店舗受取であれば true</returns>
	public bool IsShippingStorePickup(int index, int shippingNo = 0)
	{
		var result = this.CartList.Items[index].Shippings[shippingNo].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP;
		return result;
	}

	/// <summary>
	/// 全てのカートの中身がデジタルコンテンツのみかどうかを返します。
	/// </summary>
	/// <returns>全カートの中身すべてデジタルコンテンツのみの場合、TRUE</returns>
	public bool ContainsOnlyDigitalContentsInCarts()
	{
		var result = this.CartList.Items.All(cart => cart.IsDigitalContentsOnly);
		return result;
	}

	/// <summary>
	/// 配送種別情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送種別モデル</returns>
	public ShopShippingModel GetShopShipping(string shopId, string shippingId)
	{
		// ViewStateより配送種別がなければ、DBから情報取得
		var shopShipping = ((this.ShopShippingList == null)
			|| (this.ShopShippingList.Any(s => ((s.ShopId == shopId) && (s.ShippingId == shippingId))) == false))
				? DataCacheControllerFacade.GetShopShippingCacheController().Get(shippingId)
				: this.ShopShippingList
					.First(s => ((s.ShopId == shopId) && (s.ShippingId == shippingId)));
		return shopShipping;
	}

	/// <summary>
	/// データバインド用の選択可能配送サービス作成
	/// </summary>
	public void CreateSelectableDeliveryCompanyListOnDataBind()
	{
		this.SelectableDeliveryCompanyList = new List<Dictionary<string, Dictionary<string, string>>>();
		var cartNo = 0;
		foreach (var cart in this.CartList.Items)
		{
			var shopShipping = GetShopShipping(cart.ShopId, cart.ShippingType);
			var dic = new Dictionary<string, Dictionary<string, string>>
			{
				{
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
					CreateDeliveryCompanyListItem(shopShipping.CompanyListExpress, cart.IsShippingConvenienceStore)
				}
			};
			var deliveryCompanyListItem = (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED == false)
				? CreateDeliveryCompanyListItem(shopShipping.CompanyListMail, cart.IsShippingConvenienceStore)
				: (this.DeliveryCompanyListMail[cartNo].Count != 0)
					? CreateDeliveryCompanyListItem(this.DeliveryCompanyListMail[cartNo].ToArray(), cart.IsShippingConvenienceStore)
					: null;
			if(deliveryCompanyListItem != null) dic.Add(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL, deliveryCompanyListItem);

			this.SelectableDeliveryCompanyList.Add(dic);
			cartNo++;
		}
	}

	/// <summary>
	/// 配送サービスプルダウンの選択肢作成
	/// </summary>
	/// <param name="shopShippingCompany">配送種別に紐づく配送サービス</param>
	/// <param name="isShippingConvenience">Is Shipping Convenience</param>
	/// <returns>配送サービスプルダウンの選択肢</returns>
	public Dictionary<string, string> CreateDeliveryCompanyListItem(
		ShopShippingCompanyModel[] shopShippingCompany,
		bool isShippingConvenience)
	{
		var companyList = shopShippingCompany.Select(
			company =>
				((this.DeliveryCompanyList == null)
					|| (this.DeliveryCompanyList.Any(c => (c.DeliveryCompanyId == company.DeliveryCompanyId)) == false))
						? new DeliveryCompanyService().Get(company.DeliveryCompanyId)
						: this.DeliveryCompanyList.First(c => (c.DeliveryCompanyId == company.DeliveryCompanyId)))
			.OrderBy(company => company.DisplayOrder).ThenBy(company => company.DeliveryCompanyId)
			.ToArray();

		companyList = ((isShippingConvenience)
			? companyList.Where(company => (company.DeliveryCompanyId
				== Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: companyList.Where(company => (company.DeliveryCompanyId
				!= Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray());

		var dic = companyList.ToDictionary(
			company => company.DeliveryCompanyId,
			company => company.DeliveryCompanyName);
		return dic;
	}

	/// <summary>
	/// 配送サービスプルダウンの選択肢作成(メール便配送サービスエスカレーション機能用)
	/// </summary>
	/// <param name="deliveryCompanyListMail">配送種別に紐づく配送サービス</param>
	/// <param name="isShippingConvenience">Is Shipping Convenience</param>
	/// <returns>配送サービスプルダウンの選択肢</returns>
	public Dictionary<string, string> CreateDeliveryCompanyListItem(
		DeliveryCompanyModel[] deliveryCompanyListMail,
		bool isShippingConvenience)
	{
		deliveryCompanyListMail = ((isShippingConvenience)
			? deliveryCompanyListMail.Where(
				company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: deliveryCompanyListMail.Where(
				company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray());

		var dic = deliveryCompanyListMail.ToDictionary(
			company => company.DeliveryCompanyId,
			company => company.DeliveryCompanyName);
		return dic;
	}

	/// <summary>
	/// 配送サービスドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlDeliveryCompanyList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// コントロール宣言
		var cartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var shippingControl = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var wddlDeliveryCompany = CreateWrappedDropDownListDeliveryCompany(shippingControl ?? cartControl);

		// 自体の配送サービス項目がない場合、または配送サービスが未選択の場合、何もしない
		if ((wddlDeliveryCompany.HasInnerControl == false)
			|| string.IsNullOrEmpty(wddlDeliveryCompany.SelectedValue)) return;

		var cartNo = cartControl.ItemIndex;
		var cart = this.CartList.Items[cartNo];
		var shippingNo = (shippingControl != null) ? shippingControl.ItemIndex : 0;

		// カート配送情報の配送サービスIDをセット
		cart.Shippings[shippingNo].DeliveryCompanyId = wddlDeliveryCompany.SelectedValue;

		// 配送希望時間帯の表示有無更新（配送希望時間帯の表示有無はカート配送サービス更新後にする必要がある。）
		ControlShippingTimeInput(shippingControl ?? cartControl, shippingNo);
	}

	/// <summary>
	/// 配送希望時間帯入力項目の制御
	/// </summary>
	/// <param name="parent">親リピーターアイテム</param>
	/// <param name="shippingNo">カート配送のナンバー</param>
	public void ControlShippingTimeInput(RepeaterItem parent, int shippingNo)
	{
		// コントロール宣言
		var wdtShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(parent, "dtShippingTime");
		var wddShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(parent, "ddShippingTime");
		var wddlShippingTime = GetWrappedControl<WrappedDropDownList>(parent, "ddlShippingTime");
		var cartControl = (parent.ID == "rCartList")
			? parent
			: GetParentRepeaterItem(parent, "rCartList");

		// 自体の配送希望時間帯入力項目がなければ、なにもしない
		if ((wdtShippingTime.HasInnerControl == false)
			|| (wddShippingTime.HasInnerControl == false)
			|| (wddlShippingTime.HasInnerControl == false)) return;

		// 配送希望時間帯の表示有無更新（配送希望時間帯の表示有無はカート配送サービス更新後にする必要がある。）
		var cartNo = cartControl.ItemIndex;
		wdtShippingTime.Visible
			= wddShippingTime.Visible = CanInputTimeSet(cartNo, shippingNo);
		if (wdtShippingTime.Visible == false) return;

		// 配送希望時間帯を調整
		var cartShipping = this.CartList.Items[cartNo].Shippings[shippingNo];
		if ((string.IsNullOrEmpty(cartShipping.ShippingTime) == false)
			&& (GetShippingTimeList(cartShipping.DeliveryCompanyId).Cast<ListItem>()
				.Any(li => (li.Value == cartShipping.ShippingTime)) == false))
		{
			// 配送希望時間帯をnullで設定
			cartShipping.ShippingTime = null;
		}
		wddlShippingTime.DataBind();
	}

	/// <summary>
	/// 配送サービス選択
	/// </summary>
	/// <param name="parentItem">親リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	/// <param name="shippingNo">配送ナンバー</param>
	public void SelectDeliveryCompany(RepeaterItem parentItem, CartObject cartObj, int shippingNo = 0)
	{
		var wddlDeliveryCompany = CreateWrappedDropDownListDeliveryCompany(parentItem);
		wddlDeliveryCompany.SelectedValue = cartObj.Shippings[shippingNo].DeliveryCompanyId;
	}

	/// <summary>
	/// 配送サービス名取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送サービス名</returns>
	public string GetDeliveryCompanyName(string deliveryCompanyId)
	{
		var deliveryCompany = this.DeliveryCompanyList
			.First(company => (company.DeliveryCompanyId == deliveryCompanyId));
		var name = (deliveryCompany != null)
			? deliveryCompany.DeliveryCompanyName
			: string.Empty;
		return name;
	}

	/// <summary>
	/// ラップ済配送サービスドロップダウンリスト取得
	/// </summary>
	/// <param name="parent">親コントロール</param>
	/// <returns>配送サービスドロップダウンリスト</returns>
	public WrappedDropDownList CreateWrappedDropDownListDeliveryCompany(Control parent)
	{
		var wddlDeliveryCompany = GetWrappedControl<WrappedDropDownList>(parent, "ddlDeliveryCompany");
		return wddlDeliveryCompany;
	}

	/// <summary>
	/// 配送サービス選択肢取得
	/// </summary>
	/// <param name="cartNo">カートナンバー</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送サービス選択肢</returns>
	public Dictionary<string, string> GetDeliveryCompanyListItem(int cartNo, int shippingNo = 0)
	{
		var shippingMethod = this.CartList.Items[cartNo].Shippings[shippingNo].ShippingMethod
			?? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
		var result = this.SelectableDeliveryCompanyList[cartNo][shippingMethod];
		return result;
	}

	/// <summary>
	/// 配送サービス表示可能の判断
	/// </summary>
	/// <param name="cartNo">カートナンバー</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送サービスが表示かどうか</returns>
	/// <remarks>指定したカートの選択可能な配送サービス個数が１であれば非表示</remarks>
	public bool CanDisplayDeliveryCompany(int cartNo, int shippingNo = 0)
	{
		if (this.CartList.Items[cartNo].Shippings[0].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
			return false;
		
		var count = GetDeliveryCompanyListItem(cartNo, shippingNo).Count;
		return (count > 1)
			|| (this.CartList.Items[cartNo].IsShippingConvenienceStore
				&& count != 0);
	}

	/// <summary>
	/// コントロルが入っているカートのインデックス取得
	/// </summary>
	/// <param name="control">コントロル</param>
	/// <returns>コントロルが入っているカートのインデックス</returns>
	public int GetCartIndexFromControl(Control control)
	{
		var cartControl = GetParentRepeaterItem(control, "rCartList");
		var index = (cartControl != null) ? cartControl.ItemIndex : 0;
		return index;
	}

	/// <summary>
	/// 注文者情報グローバル関連項目設定
	/// </summary>
	public void SetOrderOwnerGlobalColumn()
	{
		// 注文者の国は日本であれば、何もしない
		if (IsCountryJp(this.CartList.Owner.AddrCountryIsoCode)) return;

		if (this.WrCartList.Items.Count == 0) return;

		var parentControl = this.WrCartList.Items.Cast<RepeaterItem>().ToArray()[0];

		// 注文者州のドロップダウン初期表示
		if (IsCountryUs(this.CartList.Owner.AddrCountryIsoCode)
			&& Constants.US_STATES_LIST.Contains(this.CartList.Owner.Addr5))
		{
			var wddlOwnerAddr5 = GetWrappedControl<WrappedDropDownList>(parentControl, "ddlOwnerAddr5");
			wddlOwnerAddr5.SelectItemByText(this.CartList.Owner.Addr5);
		}

		// 注文者情報が台湾国の住所であれば、住所２・３の値をセット
		if (IsCountryTw(this.CartList.Owner.AddrCountryIsoCode))
		{
			var wddlOwnerAddr2 = GetWrappedControl<WrappedDropDownList>(parentControl, "ddlOwnerAddr2");
			var wddlOwnerAddr3 = GetWrappedControl<WrappedDropDownList>(parentControl, "ddlOwnerAddr3");
			var wtbOwnerZipGlobal = GetWrappedControl<WrappedTextBox>(parentControl, "tbOwnerZipGlobal");
			var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(parentControl, "tbSenderZipGlobal");

			// プルダウン住所２＆３の自体があれば、情報をセット
			if (wddlOwnerAddr2.HasInnerControl && wddlOwnerAddr3.HasInnerControl)
			{
				wddlOwnerAddr2.SelectItemByText(this.CartList.Owner.Addr2);

				if (wddlOwnerAddr2.DataSource != null)
				{
					wddlOwnerAddr3.DataSource = this.UserTwDistrictDict[wddlOwnerAddr2.SelectedItem.ToString()];
					wddlOwnerAddr3.DataBind();
				}

				wddlOwnerAddr3.ForceSelectItemByText(this.CartList.Owner.Addr3);
				wtbOwnerZipGlobal.Text = this.CartList.Owner.Zip;
				wtbSenderZipGlobal.Text = this.CartList.Owner.Zip;
			}
		}
	}

	/// <summary>
	/// Get Invoice Carry Type Option
	/// </summary>
	/// <param name="invoiceCarryType">Invoice Carry Type</param>
	/// <returns>List Item</returns>
	protected ListItemCollection GetInvoiceCarryTypeOption(string invoiceCarryType)
	{
		var listItem = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.invoice_carry_type_option.new@@"), string.Empty),
		};
		var userInvoice = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.LoginUserId);
		if (userInvoice != null)
		{
			listItem.AddRange(userInvoice
				.Where(item => (item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) && (item.TwCarryType == invoiceCarryType))
				.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
		}

		return listItem;
	}

	/// <summary>
	/// Dropdown Invoice Carry Type Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlInvoiceCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlInvoiceCarryType = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlInvoiceCarryType");
		var wddlInvoiceCarryTypeOption = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlInvoiceCarryTypeOption");
		var wdivCarryTypeOption = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divCarryTypeOption");
		var wlbCarryTypeOption = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbCarryTypeOption");
		var wlbCarryTypeOptionText = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbCarryTypeOptionText");
		var wdlCarryTypeOptionRegist = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlCarryTypeOptionRegist");
		var wdivCarryTypeOption_8 = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divCarryTypeOption_8");
		var wdivCarryTypeOption_16 = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divCarryTypeOption_16");

		wdivCarryTypeOption_8.Visible = false;
		wdivCarryTypeOption_16.Visible = false;

		switch (wddlInvoiceCarryType.SelectedValue)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
				wddlInvoiceCarryTypeOption.Visible = true;
				wdivCarryTypeOption_8.Visible = true;
				wdlCarryTypeOptionRegist.Visible = true;
				break;

			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				wddlInvoiceCarryTypeOption.Visible = true;
				wdivCarryTypeOption_16.Visible = true;
				wdlCarryTypeOptionRegist.Visible = true;
				break;

			default:
				wddlInvoiceCarryTypeOption.Visible = false;
				wdlCarryTypeOptionRegist.Visible = false;
				break;
		}
		wddlInvoiceCarryTypeOption.DataSource = GetInvoiceCarryTypeOption(wddlInvoiceCarryType.SelectedValue);
		wddlInvoiceCarryTypeOption.DataBind();

		wddlInvoiceCarryTypeOption.SelectedValue = string.Empty;
		wdivCarryTypeOption.Visible = true;
		wlbCarryTypeOption.Visible = false;
		wlbCarryTypeOptionText.Visible = false;
	}

	/// <summary>
	/// Check Box Carry Type Option Regist Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbCarryTypeOptionRegist_CheckedChanged(object sender, EventArgs e)
	{
		var wdivCarryTypeOptionName = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divCarryTypeOptionName");
		var wcbCarryTypeOptionRegist = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, "cbCarryTypeOptionRegist");

		wdivCarryTypeOptionName.Visible = wcbCarryTypeOptionRegist.Checked;
	}

	/// <summary>
	/// Dropdown Invoice Carry Type Option Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlInvoiceCarryTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlInvoiceCarryType = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlInvoiceCarryType");
		var wdivCarryTypeOption = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divCarryTypeOption");
		var wddlInvoiceCarryTypeOption = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlInvoiceCarryTypeOption");
		var wlbCarryTypeOption = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbCarryTypeOption");
		var wlbCarryTypeOptionText = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbCarryTypeOptionText");
		var wdlCarryTypeOptionRegist = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlCarryTypeOptionRegist");
		var isNewCarryTypeOption = string.IsNullOrEmpty(wddlInvoiceCarryTypeOption.SelectedValue);

		wdivCarryTypeOption.Visible = isNewCarryTypeOption;
		var wcbCarryTypeOptionRegist = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, "cbCarryTypeOptionRegist");
		wdlCarryTypeOptionRegist.Visible = ((string.IsNullOrEmpty(wddlInvoiceCarryType.SelectedValue) == false) && isNewCarryTypeOption);

		if (isNewCarryTypeOption == false)
		{
			var userInvoice = new TwUserInvoiceService().Get(this.LoginUserId, int.Parse(wddlInvoiceCarryTypeOption.SelectedValue));
			if (userInvoice != null)
			{
				wlbCarryTypeOption.Visible = true;
				wlbCarryTypeOptionText.Visible = true;
				wlbCarryTypeOption.Text = userInvoice.TwCarryTypeOption;
			}
		}
		else
		{
			wlbCarryTypeOption.Visible = false;
			wlbCarryTypeOptionText.Visible = false;
			wcbCarryTypeOptionRegist.Checked = false;
		}
	}

	/// <summary>
	/// Dropdown List Uniform Invoice Type Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlUniformInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlUniformInvoiceType = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlUniformInvoiceType");
		var wddlUniformInvoiceTypeOption = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlUniformInvoiceTypeOption");
		var wdlUniformInvoiceOption1_8 = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlUniformInvoiceOption1_8");
		var wdlUniformInvoiceOption1_3 = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlUniformInvoiceOption1_3");
		var wdivInvoiceCarryType = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "divInvoiceCarryType");
		var wdlUniformInvoiceTypeRegist = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlUniformInvoiceTypeRegist");

		var userInvoice = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.LoginUserId);
		wddlUniformInvoiceTypeOption.Items.Clear();
		wddlUniformInvoiceTypeOption.Items.Add(new ListItem(ReplaceTag("@@DispText.uniform_invoice_option.new@@"), string.Empty));

		switch (wddlUniformInvoiceType.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				wdivInvoiceCarryType.Visible = false;
				wddlUniformInvoiceTypeOption.Visible = true;
				wdlUniformInvoiceTypeRegist.Visible = true;
				wdlUniformInvoiceOption1_8.Visible = true;
				wdlUniformInvoiceOption1_3.Visible = false;
				if (userInvoice != null)
				{
					wddlUniformInvoiceTypeOption.Items.AddRange(
						userInvoice.Where(item => item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
							.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
				}

				ddlUniformInvoiceTypeOption_SelectedIndexChanged(sender, e);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				wdivInvoiceCarryType.Visible = false;
				wddlUniformInvoiceTypeOption.Visible = true;
				wdlUniformInvoiceTypeRegist.Visible = true;
				wdlUniformInvoiceOption1_8.Visible = false;
				wdlUniformInvoiceOption1_3.Visible = true;
				if (userInvoice != null)
				{
					wddlUniformInvoiceTypeOption.Items.AddRange(
						userInvoice.Where(item => item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE)
							.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
				}
				ddlUniformInvoiceTypeOption_SelectedIndexChanged(sender, e);
				break;

			default:
				wdivInvoiceCarryType.Visible = true;
				wddlUniformInvoiceTypeOption.Visible = false;
				wdlUniformInvoiceTypeRegist.Visible = false;
				wdlUniformInvoiceOption1_8.Visible = false;
				wdlUniformInvoiceOption1_3.Visible = false;
				break;
		}
	}

	/// <summary>
	/// Dropdown List Uniform Invoice Type Option Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlUniformInvoiceTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlUniformInvoiceType = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlUniformInvoiceType");
		var wddlUniformInvoiceTypeOption = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlUniformInvoiceTypeOption");
		var wdlUniformInvoiceTypeRegist = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlUniformInvoiceTypeRegist");
		var wtbUniformInvoiceOption2 = GetWrappedControl<WrappedTextBox>(((Control)sender).Parent, "tbUniformInvoiceOption2");
		var wlbtbUniformInvoiceOption2 = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbtbUniformInvoiceOption2");

		var wtbUniformInvoiceOption1_8 = GetWrappedControl<WrappedTextBox>(((Control)sender).Parent, "tbUniformInvoiceOption1_8");
		var wlbUniformInvoiceOption1_8 = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbUniformInvoiceOption1_8");

		var wtbUniformInvoiceOption1_3 = GetWrappedControl<WrappedTextBox>(((Control)sender).Parent, "tbUniformInvoiceOption1_3");
		var wlbUniformInvoiceOption1_3 = GetWrappedControl<WrappedLabel>(((Control)sender).Parent, "lbUniformInvoiceOption1_3");

		if (string.IsNullOrEmpty(wddlUniformInvoiceTypeOption.SelectedValue) == false)
		{
			var userInvoice = new TwUserInvoiceService().Get(this.LoginUserId, int.Parse(wddlUniformInvoiceTypeOption.SelectedValue));
			if (userInvoice != null)
			{
				wlbtbUniformInvoiceOption2.Text
					= userInvoice.TwUniformInvoiceOption2;

				wlbUniformInvoiceOption1_8.Text
					= userInvoice.TwUniformInvoiceOption1;

				wlbUniformInvoiceOption1_3.Text
					= userInvoice.TwUniformInvoiceOption1;

				wtbUniformInvoiceOption2.Visible = false;
				wtbUniformInvoiceOption1_8.Visible = false;
				wtbUniformInvoiceOption1_3.Visible = false;

				wlbtbUniformInvoiceOption2.Visible = true;
				wlbUniformInvoiceOption1_8.Visible = true;
				wlbUniformInvoiceOption1_3.Visible = true;
			}
		}
		else
		{
			wtbUniformInvoiceOption2.Visible = true;
			wtbUniformInvoiceOption1_8.Visible = true;
			wtbUniformInvoiceOption1_3.Visible = true;

			wlbtbUniformInvoiceOption2.Visible = false;
			wlbUniformInvoiceOption1_8.Visible = false;
			wlbUniformInvoiceOption1_3.Visible = false;
		}
		wdlUniformInvoiceTypeRegist.Visible = ((wddlUniformInvoiceType.SelectedValue != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			&& string.IsNullOrEmpty(wddlUniformInvoiceTypeOption.SelectedValue));
	}

	/// <summary>
	/// Check Box Save To User Invoice Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbSaveToUserInvoice_CheckedChanged(object sender, EventArgs e)
	{
		var wdlUniformInvoiceTypeRegistInput = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dlUniformInvoiceTypeRegistInput");
		var wcbSaveToUserInvoice = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, "cbSaveToUserInvoice");

		wdlUniformInvoiceTypeRegistInput.Visible = wcbSaveToUserInvoice.Checked;
	}

	/// <summary>
	/// Create User Invoice Input
	/// </summary>
	/// <param name="CartShipping">Cart Shipping</param>
	/// <returns>Hashtable User Invoice Input</returns>
	protected Hashtable CreateUserInvoiceInput(CartShipping CartShipping)
	{
		var userInvoiceInput = new Hashtable();

		if (CartShipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
		{
			if (CartShipping.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
			{
				userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8", CartShipping.CarryTypeOptionValue);
			}
			else if (CartShipping.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
			{
				userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16", CartShipping.CarryTypeOptionValue);
			}
			userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME + "_carry", CartShipping.InvoiceName);
		}
		else
		{
			if (CartShipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
			{
				userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_8", CartShipping.UniformInvoiceOption1);
				userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2, CartShipping.UniformInvoiceOption2);
			}
			else if (CartShipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_DONATE)
			{
				userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_3", CartShipping.UniformInvoiceOption1);
			}
			userInvoiceInput.Add(Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME + "_uniform", CartShipping.InvoiceName);
		}

		return userInvoiceInput;
	}

	/// <summary>
	/// ECPay利用可能か
	/// </summary>
	/// <param name="cartList">カートオブジェクトリスト</param>
	/// <returns>ECPay利用可能か</returns>
	public bool CheckCountryIsoCodeCanOrderWithECPay(CartObjectList cartList)
	{
		if ((Constants.GLOBAL_OPTION_ENABLE == false)
			|| (Constants.ECPAY_PAYMENT_OPTION_ENABLED == false))
		{
			return true;
		}

		var hasECPayAddressLimit = cartList.Items.Any(cart => (cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			&& (cart.Shippings.Any(cartShipping => (cartShipping.IsShippingAddrTw == false))
				|| (cart.Owner.IsAddrTw == false)));

		if (hasECPayAddressLimit)
		{
			this.ErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY));
			return false;
		}

		return true;
	}

	/// <summary>
	/// Check Shipping Country Iso Code Can Order With Paidy Pay
	/// </summary>
	/// <param name="cartList">Cart List</param>
	/// <returns>True: can order with paidy pay, otherwise: false</returns>
	public bool CheckShippingCountryIsoCodeCanOrderWithPaidyPay(CartObjectList cartList)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return true;

		var hasPaidyPayShippingLimit = cartList.Items.Any(cart => (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& cart.Shippings.Any(cartShipping => (cartShipping.IsShippingAddrJp == false)));
		if (hasPaidyPayShippingLimit)
		{
			this.ErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR));
			return false;
		}

		return true;
	}

	/// <summary>
	/// Check Country Iso Code Can Order With NP After Pay
	/// </summary>
	/// <param name="cartList">Cart List</param>
	/// <returns>True: can order, otherwise: false</returns>
	public bool CheckCountryIsoCodeCanOrderWithNPAfterPay(CartObjectList cartList)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return true;

		var hasNPAfterPayShippingLimit = cartList.Items.Any(cart => (cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			&& (cart.Shippings.Any(cartShipping => (cartShipping.IsShippingAddrJp == false))
				|| (cart.Owner.IsAddrJp == false)));
		if (hasNPAfterPayShippingLimit)
		{
			this.ErrorMessages.Add(NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3));
			return false;
		}
		return true;
	}

	/// <summary>
	/// DropDownList Shipping Receiving Store Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, "ddlShippingReceivingStoreType");
		var cartIndex = (string.IsNullOrEmpty(((DropDownList)sender).DataMember) == false)
			? int.Parse(((DropDownList)sender).DataMember)
			: 0;

		// Set error when the product price and weight are over
		var wlConvenienceStoreErrorMessage = GetWrappedControl<WrappedLiteral>(this.WrCartList.Items[cartIndex], "lConvenienceStoreErrorMessage");
		var shippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;
		wlConvenienceStoreErrorMessage.Text = string.Empty;

		if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(
			this.CartList.Items[cartIndex],
			shippingReceivingStoreType))
		{
			var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(shippingReceivingStoreType);

			wlConvenienceStoreErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
				.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
				.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());
		}

		ClearConvenienceStoreEcPay(cartIndex);
	}

	/// <summary>
	/// Shipping Receiving Store Type
	/// </summary>
	/// <returns>List item collection shipping receiving store type</returns>
	public ListItem[] ShippingReceivingStoreType()
	{
		var listItem = ValueText.GetValueItemArray(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE);
		return listItem;
	}

	/// <summary>
	/// Has convenience store in cart list
	/// </summary>
	/// <returns>True: if convenience store exists or false: if convenience store does not exist</returns>
	public bool HasConvenienceStoreInCartList()
	{
		var hasConvenienceStore = false;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingKbnList");
			var userShipping = (this.UserShippingAddr != null)
				? this.UserShippingAddr.FirstOrDefault(shipping =>
					shipping.ShippingNo.ToString().Equals(wddlShippingKbnList.SelectedValue))
				: null;
			var isEcPayUserShipping = ((userShipping != null)
				&& (string.IsNullOrEmpty(userShipping.ShippingReceivingStoreType) == false));

			if ((wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				|| (isEcPayUserShipping))
			{
				hasConvenienceStore = true;
				break;
			}
		}
		return hasConvenienceStore;
	}

	/// <summary>
	/// Clear Convenience Store EcPay
	/// </summary>
	/// <param name="cartIndex">Cart index</param>
	public void ClearConvenienceStoreEcPay(int cartIndex)
	{
		var wlCvsShopId = GetWrappedControl<WrappedLiteral>(this.WrCartList.Items[cartIndex], "lCvsShopId");
		var wlCvsShopName = GetWrappedControl<WrappedLiteral>(this.WrCartList.Items[cartIndex], "lCvsShopName");
		var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(this.WrCartList.Items[cartIndex], "lCvsShopAddress");
		var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(this.WrCartList.Items[cartIndex], "lCvsShopTel");
		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(this.WrCartList.Items[cartIndex], "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(this.WrCartList.Items[cartIndex], "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(this.WrCartList.Items[cartIndex], "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(this.WrCartList.Items[cartIndex], "hfCvsShopTel");

		wlCvsShopId.Text = string.Empty;
		wlCvsShopName.Text = string.Empty;
		wlCvsShopAddress.Text = string.Empty;
		wlCvsShopTel.Text = string.Empty;
		whfCvsShopId.Value = string.Empty;
		whfCvsShopName.Value = string.Empty;
		whfCvsShopAddress.Value = string.Empty;
		whfCvsShopTel.Value = string.Empty;
	}

	/// <summary>
	/// Click Open Window EcPay
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbOpenEcPay_Click(object sender, EventArgs e)
	{
		var riCart = this.WrCartList.Items[0];
		var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(riCart, "ddlOwnerCountry");
		var ownerAddrCountryIsoCode = wddlOwnerCountry.SelectedValue;
		var wlShippingCountryErrorMessage = GetWrappedControl<WrappedLiteral>(((Control)sender).Parent, "lShippingCountryErrorMessage");
		wlShippingCountryErrorMessage.Text = string.Empty;
		if (IsCountryTw(ownerAddrCountryIsoCode) == false)
		{
			wlShippingCountryErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE);
			return;
		}

		if (lbNext_Click_OrderShipping_Owner(sender, e) == false) return;
		
		var cartIndex = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			this.WrCartList.Items[cartIndex],
			"ddlShippingReceivingStoreType");
		var shippingService = wddlShippingReceivingStoreType.SelectedValue;

		this.CartList.Items[cartIndex].Shippings[0].ShippingReceivingStoreType = shippingService;

		var baseUrl = string.Format(
			"{0}{1}{2}",
			Constants.PROTOCOL_HTTP,
			(string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
				? Constants.SITE_DOMAIN
				: Constants.WEBHOOK_DOMAIN),
			Request.Url.LocalPath);
		var url = new UrlCreator(baseUrl)
			.AddParam(Constants.REQUEST_KEY_CART_INDEX, cartIndex.ToString())
			.CreateUrl();
		var parameters = ECPayUtility.CreateParametersForOpenConvenienceStoreMap(
			shippingService,
			url,
			this.IsSmartPhone);
		var json = JsonConvert.SerializeObject(parameters);
		var script = "NextPageSelectReceivingStore(JSON.parse('" + json + "'));";

		ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ReceivingStore", script, true);
	}

	/// <summary>
	/// 配送予定日の表示セット 注文者 住所変更向け
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void SetNextShippingDateForOwnderAddr(object sender, EventArgs e)
	{
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return;

		var selectCartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var wddlOwnerAddr1 = GetWrappedControl<WrappedDropDownList>(selectCartControl, "ddlOwnerAddr1");
		var wtbOwnerZip = GetWrappedControl<WrappedTextBox>(selectCartControl, "tbOwnerZip");
		var wtbOwnerZip1 = GetWrappedControl<WrappedTextBox>(selectCartControl, "tbOwnerZip1");
		var wtbOwnerZip2 = GetWrappedControl<WrappedTextBox>(selectCartControl, "tbOwnerZip2");
		var zipCode = GetZipCode(wtbOwnerZip, wtbOwnerZip1, wtbOwnerZip2);

		var errorMessages = Validator.CheckZipCode(zipCode.Zip1, zipCode.Zip2);
		if (string.IsNullOrEmpty(errorMessages) == false) return;

		var cart = this.CartList.Items[selectCartControl.ItemIndex];
		cart.Owner.Addr1 = wddlOwnerAddr1.SelectedValue;
		cart.Owner.Zip1 = zipCode.Zip1;
		cart.Owner.Zip2 = zipCode.Zip2;

		lbNext_Click_OrderShipping_Shipping(sender, e);
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingKbnList");
			var wcbShipToCart1Address = GetWrappedControl<WrappedCheckBox>(riCart, "cbShipToCart1Address");
			if ((((riCart.ItemIndex != 0) && wcbShipToCart1Address.HasInnerControl && wcbShipToCart1Address.Checked)
				|| (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER))
				&& this.CartList.Items[riCart.ItemIndex].HasFixedPurchase)
			{
				UpdateNextShippingDateDropDownList(riCart);
			}
		}
	}

	/// <summary>
	/// 配送予定日の表示セット 配送先 住所変更向け
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void SetNextShippingDateForShippingAddr(object sender, EventArgs e)
	{
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return;

		var cartControl = GetParentRepeaterItem((Control)sender, "rCartList");
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(cartControl, "ddlShippingKbnList");
		var cart = this.CartList.Items[cartControl.ItemIndex];

		if ((wddlShippingKbnList.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
			|| (cart.HasFixedPurchase == false)) return;

		var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(cartControl, "ddlShippingAddr1");
		var wtbShippingZip = GetWrappedControl<WrappedTextBox>(cartControl, "tbShippingZip");
		var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(cartControl, "tbShippingZip1");
		var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(cartControl, "tbShippingZip2");
		var zipCode = GetZipCode(wtbShippingZip, wtbShippingZip1, wtbShippingZip2);

		var errorMessages = Validator.CheckZipCode(zipCode.Zip1, zipCode.Zip2);
		if (string.IsNullOrEmpty(errorMessages) == false) return;

		cart.Shippings[0].Addr1 = wddlShippingAddr1.SelectedValue;
		cart.Shippings[0].Zip1 = zipCode.Zip1;
		cart.Shippings[0].Zip2 = zipCode.Zip2;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(cartControl, "ddlShippingCountry");
			cart.Shippings[0].ShippingCountryIsoCode = wddlShippingCountry.SelectedValue;
		}
		UpdateNextShippingDateDropDownList(sender);
	}

	/// <summary>
	/// 画面入力情報からZipCode型郵便番号設定を取得
	/// </summary>
	/// <param name="wtbZip">郵便番号入力テキストボックス</param>
	/// <param name="wtbZip1">郵便番号１入力テキストボックス</param>
	/// <param name="wtbZip2">郵便番号２入力テキストボックス</param>
	/// <returns>ZipCode型郵便番号設定</returns>
	private ZipCode GetZipCode(WrappedTextBox wtbZip, WrappedTextBox wtbZip1, WrappedTextBox wtbZip2)
	{
		// NOTE：元ロジックを保つために、先にwtbZip1を判断
		var inputZipCode = wtbZip1.HasInnerControl
			? string.Format("{0}-{1}", StringUtility.ToHankaku(wtbZip1.Text.Trim()), StringUtility.ToHankaku(wtbZip2.Text.Trim()))
			: StringUtility.ToHankaku(wtbZip.Text.Trim());

		var zipCode = new ZipCode(inputZipCode);
		return zipCode;
	}

	/// <summary>
	/// Linkbutton search address owner from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchAddrOwnerFromZipGlobal_Click(object sender, EventArgs e)
	{
		var control = (Control)sender;
		var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerCountry");

		if (IsNotCountryJp(wddlOwnerCountry.SelectedValue) == false) return;

		var wtbOwnerZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerZipGlobal");
		var wtbOwnerAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerAddr2");
		var wtbOwnerAddr4 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerAddr4");
		var wtbOwnerAddr5 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerAddr5");
		var wddlOwnerAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr2");
		var wddlOwnerAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr3");
		var wddlOwnerAddr5 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr5");

		GlobalAddressUtil.BindingAddressByGlobalZipcode(
			wddlOwnerCountry.SelectedValue,
			StringUtility.ToHankaku(wtbOwnerZipGlobal.Text.Trim()),
			wtbOwnerAddr2,
			wtbOwnerAddr4,
			wtbOwnerAddr5,
			wddlOwnerAddr2,
			wddlOwnerAddr3,
			wddlOwnerAddr5);
	}

	/// <summary>
	/// Linkbutton search address shipping from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchAddrShippingFromZipGlobal_Click(object sender, EventArgs e)
	{
		var control = (Control)sender;
		var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingCountry");

		if (IsNotCountryJp(wddlShippingCountry.SelectedValue) == false) return;

		var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingZipGlobal");
		var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingAddr2");
		var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingAddr4");
		var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingAddr5");
		var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr2");
		var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr3");
		var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr5");

		GlobalAddressUtil.BindingAddressByGlobalZipcode(
			wddlShippingCountry.SelectedValue,
			StringUtility.ToHankaku(wtbShippingZipGlobal.Text.Trim()),
			wtbShippingAddr2,
			wtbShippingAddr4,
			wtbShippingAddr5,
			wddlShippingAddr2,
			wddlShippingAddr3,
			wddlShippingAddr5);
	}

	/// <summary>
	/// Linkbutton search address sender from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchAddrSenderFromZipGlobal_Click(object sender, EventArgs e)
	{
		var control = (Control)sender;
		var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderCountry");

		if (IsNotCountryJp(wddlSenderCountry.SelectedValue) == false) return;

		var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZipGlobal");
		var wtbSenderAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderAddr2");
		var wtbSenderAddr4 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderAddr4");
		var wtbSenderAddr5 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderAddr5");
		var wddlSenderAddr2 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr2");
		var wddlSenderAddr3 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr3");
		var wddlSenderAddr5 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr5");

		GlobalAddressUtil.BindingAddressByGlobalZipcode(
			wddlSenderCountry.SelectedValue,
			StringUtility.ToHankaku(wtbSenderZipGlobal.Text.Trim()),
			wtbSenderAddr2,
			wtbSenderAddr4,
			wtbSenderAddr5,
			wddlSenderAddr2,
			wddlSenderAddr3,
			wddlSenderAddr5);
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		var control = (Control)sender;
		var wtbOwnerTel1_1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerTel1_1");
		var wtbOwnerTel1_2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerTel1_2");
		var wtbOwnerTel1_3 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerTel1_3");
		var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerTel1");
		var wtbOwnerTel1Global = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerTel1Global");
		RemoveErrorInputClass(wtbOwnerTel1Global);

		var countryIsoCode = GetOwnerAddrCountryIsoCode(this.CartItemIndexTmp);
		if (string.IsNullOrEmpty(countryIsoCode)) countryIsoCode = Constants.COUNTRY_ISO_CODE_JP;

		var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(IsCountryJp(countryIsoCode), control.Parent);
		var wlbAuthenticationStatus = GetWrappedControlOfLabelAuthenticationStatus(IsCountryJp(countryIsoCode), control.Parent);
		RemoveErrorInputClass(wtbAuthenticationCode);

		var wcvOwnerTel1 = GetWrappedControl<WrappedCustomValidator>(control.Parent, "cvOwnerTel1");
		var wcvOwnerTel1Global = GetWrappedControl<WrappedCustomValidator>(control.Parent, "cvOwnerTel1Global");

		ChangeControlLooksForValidator(
			new Dictionary<string, string> { { string.Empty, string.Empty } },
			Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
			IsCountryJp(countryIsoCode) ? wcvOwnerTel1 : wcvOwnerTel1Global,
			wtbAuthenticationCode);

		this.HasAuthenticationCode = false;
		wtbAuthenticationCode.Text = string.Empty;
		wtbAuthenticationCode.Enabled = true;

		var telephone = GetValueForTelephone(
			wtbOwnerTel1_1,
			wtbOwnerTel1_2,
			wtbOwnerTel1_3,
			wtbOwnerTel1,
			wtbOwnerTel1Global,
			countryIsoCode);

		SendAuthenticationCode(
			wtbAuthenticationCode,
			wlbAuthenticationStatus,
			telephone,
			countryIsoCode);
	}

	/// <summary>
	/// Link button check authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCheckAuthenticationCode_Click(object sender, EventArgs e)
	{
		var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1");
		var wtbOwnerTel1_1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_1");
		var wtbOwnerTel1_2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_2");
		var wtbOwnerTel1_3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_3");
		var wtbOwnerTel1Global = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1Global");

		var countryIsoCode = GetOwnerAddrCountryIsoCode(this.CartItemIndexTmp);
		if (string.IsNullOrEmpty(countryIsoCode)) countryIsoCode = Constants.COUNTRY_ISO_CODE_JP;

		var isUserAddressJp = IsCountryJp(countryIsoCode);
		var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(isUserAddressJp, this.FirstRpeaterItem);
		var wlbGetAuthenticationCode = GetWrappedControlOfLinkButtonAuthenticationCode(isUserAddressJp, this.FirstRpeaterItem);
		var wlbAuthenticationMessage = GetWrappedControlOfLabelAuthenticationMessage(isUserAddressJp, this.FirstRpeaterItem);
		var wlbAuthenticationStatus = GetWrappedControlOfLabelAuthenticationStatus(isUserAddressJp, this.FirstRpeaterItem);
		var whfResetAuthenticationCode = GetWrappedControl<WrappedHiddenField>(((Control)sender).Parent, "hfResetAuthenticationCode");

		var telephone = GetValueForTelephone(
			wtbOwnerTel1_1,
			wtbOwnerTel1_2,
			wtbOwnerTel1_3,
			wtbOwnerTel1,
			wtbOwnerTel1Global,
			countryIsoCode);

		// Reset authentication code
		if (string.IsNullOrEmpty(whfResetAuthenticationCode.Value) == false)
		{
			wlbAuthenticationStatus.Text = string.Empty;

			// Set authenticated if phone number not change
			this.HasAuthenticationCode = (this.IsLoggedIn
				&& (this.LoginUser.Tel1 == telephone));

			var authenticationStatus = (this.IsLoggedIn
				&& (this.LoginUser.Tel1 != telephone))
					? wlbAuthenticationStatus
					: null;

			DisplayAuthenticationCode(
				wlbGetAuthenticationCode,
				wtbAuthenticationCode,
				authenticationStatus);
			return;
		}

		// Exec check authentication code
		var errorMessages = ExecCheckAuthenticationCode(
			wlbGetAuthenticationCode,
			wtbAuthenticationCode,
			wlbAuthenticationMessage,
			wlbAuthenticationStatus,
			telephone,
			countryIsoCode);

		if (errorMessages.Count > 0)
		{
			// Set error message to custom validator control
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this.FirstRpeaterItem, customValidators);

			SetControlViewsForError(
				isUserAddressJp ? "OrderShipping" : "OrderShippingGlobal",
				errorMessages,
				customValidators);

			this.HasAuthenticationCode = false;
			return;
		}

		this.HasAuthenticationCode = true;
		RemoveErrorInputClass(wtbAuthenticationCode);
	}

	/// <summary>
	/// メール便配送サービスエスカレーション
	/// </summary>
	/// <param name="cartProducts">カート内商品</param>
	/// <param name="companyListMail">メール便配送サービス</param>
	/// <returns>配送会社ID</returns>
	public string DeliveryCompanyMailEscalation(
		IEnumerable<CartProduct> cartProducts,
		ShopShippingCompanyModel[] companyListMail)
	{
		var totalProductSize = cartProducts.Sum(item => item.ProductSizeFactor * item.Count);
		var deliveryCompanyModel = OrderCommon.GetDeliveryCompanyList(companyListMail)
			.Where(company => company.DeliveryCompanyMailSizeLimit > (totalProductSize - 1))
			.OrderBy(company => company.DeliveryCompanyMailSizeLimit)
			.ToList();
		this.DeliveryCompanyListMail.Add(deliveryCompanyModel);
		var result = (deliveryCompanyModel.Count == 0)
			? string.Empty
			: deliveryCompanyModel.First().DeliveryCompanyId;
		return result;
	}

	/// <summary>
	/// ユーザーのGMO情報の更新
	/// </summary>
	/// <param name="user">ユーザ</param>
	private void UpdateUserBusinessOwner(UserModel user)
	{
		var userBusinessOwner = new UserBusinessOwnerService().GetByUserId(user.UserId);
		if (userBusinessOwner != null)
		{
			if (string.IsNullOrEmpty(userBusinessOwner.ShopCustomerId) == false)
			{
				// update purchase company: frame guarantee
				var facade = new GmoTransactionApi();
				var fgRequest = new GmoRequestFrameGuaranteeUpdate(user, userBusinessOwner);
				facade.FrameGuaranteeUpdate(fgRequest);
			}
		}
	}

	/// <summary>
	/// Check need redirect to page order payment process
	/// </summary>
	public void CheckNeedRedirectToPageOrderPaymentProcess()
	{
		// If Back From Confirm => Check valid Tel No And Country
		if (this.IsNextConfirmPage
			&& (Constants.PAYMENT_AFTEEOPTION_ENABLED
				|| Constants.PAYMENT_ATONEOPTION_ENABLED))
		{
			foreach (var cart in this.CartList.Items)
			{
				if (cart.Payment == null) continue;

				if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& ((cart.Owner.IsAddrJp == false)
						|| (cart.Shippings[0].IsShippingAddrJp == false)))
				{
					this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
					break;
				}

				if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& ((cart.Owner.IsAddrTw == false)
						|| (cart.Shippings[0].IsShippingAddrTw == false)))
				{
					this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
					break;
				}
			}
		}

		if (this.IsNextConfirmPage
			&& Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED)
		{
			var cart = this.CartList.Items[0];
			if ((cart.Payment != null)
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& ((cart.Owner.IsAddrTw == false)
					|| (cart.Shippings[0].IsShippingAddrTw == false)))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
			}
		}

		if (this.IsNextConfirmPage
			&& Constants.STORE_PICKUP_OPTION_ENABLED
			&& Constants.REALSHOP_OPTION_ENABLED)
		{
			foreach (var cart in this.CartList.Items)
			{
				if ((cart.Shippings[0].IsShippingStorePickup == false) || (cart.Payment == null)) continue;

				if (Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(cart.Payment.PaymentId) == false)
				{
					this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
					break;
				}
			}
		}
	}

	/// <summary>
	/// のし種類一覧取得
	/// </summary>
	/// <param name="cartIndex">rCartListのインデックス</param>
	/// <returns>のし種類一覧</returns>
	public ListItemCollection GetWrappingPaperTypes(int cartIndex)
	{
		var paperTypes = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.common_message.unused@@"), "")
		};
		paperTypes.AddRange(StringUtility.SplitCsvLine((string)this.ShopShippingList[cartIndex].WrappingPaperTypes)
			.Select(paper => new ListItem(paper.Replace('<', '＜').Replace('>', '＞')))
			.ToArray());
		return paperTypes;
	}

	/// <summary>
	/// 包装種類一覧取得
	/// </summary>
	/// <param name="cartIndex">rCartListのインデックス</param>
	/// <returns>包装種類</returns>
	public ListItemCollection GetWrappingBagTypes(int cartIndex)
	{
		var bagTypes = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.common_message.unused@@"), "")
		};
		bagTypes.AddRange(StringUtility.SplitCsvLine((string)this.ShopShippingList[cartIndex].WrappingBagTypes)
			.Select(bag => new ListItem(bag.Replace('<', '＜').Replace('>', '＞')))
			.ToArray());
		return bagTypes;
	}

	/// <summary>
	/// のし利用フラグ有効判定
	/// </summary>
	/// <param name="cartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送日設定可能フラグが有効かどうか</returns>
	public bool GetWrappingPaperFlgValid(int cartIndex)
	{
		return this.ShopShippingList[cartIndex].IsValidWrappingPaperFlg;
	}

	/// <summary>
	/// 包装利用フラグ有効判定
	/// </summary>
	/// <param name="cartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送希望時間帯設定可能フラグが有効かどうか</returns>
	public bool GetWrappingBagFlgValid(int cartIndex)
	{
		return this.ShopShippingList[cartIndex].IsValidWrappingBagFlg;
	}

	/// <summary>
	/// 配送先「１」と同じ送り主を指定するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbSameSenderAsShipping1_OnCheckedChanged(object sender, EventArgs e)
	{
		var cbSameSenderAsShipping1 = (CheckBox)sender;

		var whcShippingSender = GetWrappedControl<WrappedHtmlGenericControl>(
			cbSameSenderAsShipping1.Parent,
			"hcShippingSender");

		whcShippingSender.Visible = cbSameSenderAsShipping1.Checked == false;
	}

	/// <summary>
	/// 送り主選択ラジオボタンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rblSenderSelector_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// ラップ済みコントロール宣言
		WrappedRadioButtonList wrblSenderSelector;
		if (sender is WrappedRadioButtonList)
		{
			wrblSenderSelector = (WrappedRadioButtonList)sender;
		}
		else if (sender is Control)
		{
			wrblSenderSelector = GetWrappedControl<WrappedRadioButtonList>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else
		{
			return;
		}

		// ラップ済みコントロール宣言
		var whgcdivSenderDisp = GetWrappedControl<WrappedHtmlGenericControl>(
			wrblSenderSelector.Parent,
			"divSenderDisp");
		var whgcdivSenderInputFormInner = GetWrappedControl<WrappedHtmlGenericControl>(
			wrblSenderSelector.Parent,
			"divSenderInputFormInner");

		whgcdivSenderDisp.Visible = wrblSenderSelector.SelectedValue == CartShipping.AddrKbn.Owner.ToString();
		whgcdivSenderInputFormInner.Visible = wrblSenderSelector.SelectedValue == CartShipping.AddrKbn.New.ToString();
	}

	/// <summary>
	/// のし・包装情報コピーリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCopyWrappingInfoToOtherShippings_Click(object sender, EventArgs e)
	{
		var lbCopyWrappingInfoToOtherShippings = (LinkButton)sender;
		var riShipping = GetOuterControl(lbCopyWrappingInfoToOtherShippings, typeof(RepeaterItem));
		var rShippings = GetOuterControl(riShipping, typeof(Repeater));

		var wddlWrappingPaperType = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlWrappingPaperType");
		var wtbWrappingPaperName = GetWrappedControl<WrappedTextBox>(riShipping, "tbWrappingPaperName");
		var wddlWrappingBagType = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlWrappingBagType");

		foreach (RepeaterItem ri in rShippings.Items)
		{
			if (ri != riShipping)
			{
				var wddlWrappingPaperTypeTgt = GetWrappedControl<WrappedDropDownList>(ri, "ddlWrappingPaperType");
				var wtbWrappingPaperNameTgt = GetWrappedControl<WrappedTextBox>(ri, "tbWrappingPaperName");
				var wddlWrappingBagTypeTgt = GetWrappedControl<WrappedDropDownList>(ri, "ddlWrappingBagType");

				wddlWrappingPaperTypeTgt.SelectedValue = wddlWrappingPaperType.SelectedValue;
				wtbWrappingPaperNameTgt.Text = wtbWrappingPaperName.Text;
				wddlWrappingBagTypeTgt.SelectedValue = wddlWrappingBagType.SelectedValue;
			}
		}
	}

	/// <summary>
	/// Get gift wrapping paper and bag input
	/// </summary>
	public void GetGiftWrappingPaperAndBagInput()
	{
		// 熨斗・包装格納
		foreach (RepeaterItem riCart in WrCartList.Items)
		{
			var wrCartShipping = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			foreach (RepeaterItem riShipping in wrCartShipping.Items)
			{
				var cartShipping = this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex];

				cartShipping.WrappingPaperValidFlg = GetWrappingPaperFlgValid(riCart.ItemIndex);
				cartShipping.WrappingBagValidFlg = GetWrappingBagFlgValid(riCart.ItemIndex);

				if (cartShipping.WrappingPaperValidFlg)
				{
					var wddlWrappingPaperType = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlWrappingPaperType");
					var wtbWrappingPaperName = GetWrappedControl<WrappedTextBox>(riShipping, "tbWrappingPaperName");

					cartShipping.WrappingPaperType = wddlWrappingPaperType.SelectedValue;
					cartShipping.WrappingPaperName = string.IsNullOrEmpty(cartShipping.WrappingPaperType)
						? string.Empty
						: wtbWrappingPaperName.Text.Trim();
				}

				if (cartShipping.WrappingBagValidFlg)
				{
					var wddlWrappingBagType = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlWrappingBagType");
					cartShipping.WrappingBagType = wddlWrappingBagType.SelectedValue;
				}
			}
		}
	}

	#region 古い形式のメソッド（非推奨）
	/// <summary>
	/// 配送日設定可能状態取得
	/// </summary>
	/// <param name="iCartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送日設定可能フラグが有効かどうか</returns>
	[Obsolete("[V5.2] CanInputDateSet() を使用してください")]
	public bool GetShippingDateSetFlgValid(int iCartIndex)
	{
		return this.ShopShippingList[iCartIndex].IsValidShippingDateSetFlg;
	}
	/// <summary>
	/// 配送希望時間帯設定可能状態取得
	/// </summary>
	/// <param name="iCartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送希望時間帯設定可能フラグが有効かどうか</returns>
	[Obsolete("[V5.2] CanInputTimeSet() を使用してください")]
	public bool GetShippingTimeSetFlgValid(int iCartIndex)
	{
		var deliveryId = this.ShopShippingList[iCartIndex].GetDefaultDeliveryCompany(this.CartList.Items[iCartIndex].IsExpressDelivery).DeliveryCompanyId;
		return this.DeliveryCompanyList.First(company => company.DeliveryCompanyId == deliveryId).IsValidShippingTimeSetFlg;
	}
	#endregion

	/// <summary>
	/// 受取店舗絞り込みドロップダウンリスト選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlRealShopNarrowDown_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var wdlRealShopAddress = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopAddress");
		var wdlRealShopOpenningHours = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopOpenningHours");
		var wdlRealShopTel = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopTel");
		var wddlRealShopName = GetWrappedControl<WrappedDropDownList>((DropDownList)sender, "ddlRealShopName");
		var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>((DropDownList)sender, "ddlShippingKbnList");
		
		var wddlRealShopArea = GetWrappedControl<WrappedDropDownList>((DropDownList)sender, "ddlRealShopArea");
		var wddlddlRealShopAddr1List = GetWrappedControl<WrappedDropDownList>((DropDownList)sender, "ddlRealShopAddr1List");

		if (wddlShippingKbnList.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
		{
			var realShopList = new RealShopCacheController().GetRealShopModels();
			this.RealShopNameList = realShopList
				.Where(realShop => (string.IsNullOrEmpty(wddlRealShopArea.SelectedValue) || realShop.AreaId == wddlRealShopArea.SelectedValue)
					&& (string.IsNullOrEmpty(wddlddlRealShopAddr1List.SelectedValue) || realShop.Addr1 == wddlddlRealShopAddr1List.SelectedValue))
				.ToArray();
		}
		wdlRealShopAddress.Visible = false;
		wdlRealShopOpenningHours.Visible = false;
		wdlRealShopTel.Visible = false;
		wddlRealShopName.DataBind();
		ddlRealShopNameList_OnSelectedIndexChanged(((DropDownList)sender).FindControl("ddlRealShopName"), e);

		if (Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] != null)
		{
			var realShopSelection = (Hashtable)Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO];
			realShopSelection[Constants.FIELD_REALSHOP_AREA_ID] = StringUtility.ToEmpty(wddlRealShopArea.SelectedValue);
			realShopSelection[Constants.FIELD_REALSHOP_ADDR1] = StringUtility.ToEmpty(wddlddlRealShopAddr1List.SelectedValue);
			realShopSelection[Constants.FIELD_REALSHOP_REAL_SHOP_ID] = string.Empty;
		}
		else
		{
			// Save realshop selection to session
			var realShopSelection = new Hashtable
			{
				{ Constants.FIELD_REALSHOP_AREA_ID, StringUtility.ToEmpty(wddlRealShopArea.SelectedValue) },
				{ Constants.FIELD_REALSHOP_ADDR1, StringUtility.ToEmpty(wddlddlRealShopAddr1List.SelectedValue) },
				{ Constants.FIELD_REALSHOP_REAL_SHOP_ID, string.Empty }
			};

			Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] = realShopSelection;
		}
	}

	/// <summary>
	/// 受取店舗選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlRealShopNameList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var wdlRealShopAddress = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopAddress");
		var wdlRealShopOpenningHours = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopOpenningHours");
		var wdlRealShopTel = GetWrappedControl<WrappedHtmlGenericControl>((DropDownList)sender, "dlRealShopTel");
		var wlRealShopZip = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopZip");
		var wlRealShopAddr1 = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopAddr1");
		var wlRealShopAddr2 = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopAddr2");
		var wlRealShopAddr3 = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopAddr3");
		var wlRealShopAddr4 = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopAddr4");
		var wlRealShopAddr5 = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopAddr5");
		var wlRealShopOpenningHours = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopOpenningHours");
		var wlRealShopTel = GetWrappedControl<WrappedLiteral>((DropDownList)sender, "lRealShopTel");
		wdlRealShopAddress.Visible = true;
		wdlRealShopOpenningHours.Visible = true;
		wdlRealShopTel.Visible = true;

		if (string.IsNullOrEmpty(((DropDownList)sender).SelectedValue) == false)
		{
			this.RealShopInfo = new RealShopService().Get(((DropDownList)sender).SelectedValue);
			wlRealShopZip.Text = this.RealShopInfo.Zip;
			wlRealShopAddr1.Text = this.RealShopInfo.Addr1;
			wlRealShopAddr2.Text = this.RealShopInfo.Addr2;
			wlRealShopAddr3.Text = this.RealShopInfo.Addr3;
			wlRealShopAddr4.Text = this.RealShopInfo.Addr4;
			wlRealShopAddr5.Text = this.RealShopInfo.Addr5;
			wlRealShopOpenningHours.Text = this.RealShopInfo.OpeningHours;
			wlRealShopTel.Text = this.RealShopInfo.Tel;
		}
		else
		{
			wdlRealShopAddress.Visible = false;
			wdlRealShopOpenningHours.Visible = false;
			wdlRealShopTel.Visible = false;
		}

		if (Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] != null)
		{
			var realShopSelection = (Hashtable)Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO];
			realShopSelection[Constants.FIELD_REALSHOP_REAL_SHOP_ID] = StringUtility.ToEmpty(((DropDownList)sender).SelectedValue);
		}
		else
		{
			// Save realshop selection to session
			var realShopSelection = new Hashtable
			{
				{ Constants.FIELD_REALSHOP_AREA_ID, string.Empty },
				{ Constants.FIELD_REALSHOP_REAL_SHOP_ID, StringUtility.ToEmpty(((DropDownList)sender).SelectedValue) }
			};

			Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] = realShopSelection;
		}
	}

	/// <summary>
	/// Create real shop area list on data bind
	/// </summary>
	public void CreateRealShopAreaListOnDataBind()
	{
		this.RealShopAreaList = DataCacheControllerFacade
			.GetRealShopAreaCacheController()
			.GetRealShopAreaList();
	}

	/// <summary>
	/// Create real shop name list on data bind
	/// </summary>
	public void CreateRealShopNameListOnDataBind()
	{
		var realShopList = new RealShopService().GetRealShops(null, null);
		var realShopNameList = new List<RealShopModel>()
		{
			new RealShopModel(),
		};
		realShopNameList.AddRange(realShopList);
		this.RealShopNameList = realShopNameList.ToArray();
	}

	/// <summary>
	/// 有効な決済方法か
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <returns>有効か</returns>
	private bool IsValidPayment(CartObject[] cartList)
	{
		var paymentIds = cartList.Select(cart => cart.Payment.PaymentId).Distinct().ToArray();
		var isValid = paymentIds.All(payment => Constants.RECOMMENDOPTION_APPLICABLE_PAYMENTIDS_FOR_ORDER_COMPLETE.Contains(payment));
		return isValid;
	}

	/// <summary>
	/// Check product buy store pickup
	/// </summary>
	/// <returns>True: If can't buy store pickup, False: Can buy</returns>
	private string CheckProductBuyStorePickup(int cartIndex)
	{
		var errorMessage = string.Empty;
		var hasStorePickupUnavailableProduct = CartList.Items[cartIndex].Items
			.Any(product => product.StorePickUpFlg == Constants.FLG_PRODUCT_STOREPICKUP_FLG_INVALID);
		
		if (hasStorePickupUnavailableProduct
			|| CartList.Items[cartIndex].HasFixedPurchase)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_STOREPICKUP);
		}
		return errorMessage;
	}

	#region 注文配送先プロパティ
	/// <summary>ユーザー区分</summary>
	public string UserKbn
	{
		get { return (string)ViewState["UserKbn"]; }
		set { ViewState["UserKbn"] = value; }
	}
	/// <summary>アドレス帳情報</summary>
	public ListItemCollection UserShippingList { get; set; }
	/// <summary>都道府県情報</summary>
	public ListItemCollection Addr1List { get; set; }
	/// <summary>ユーザー国表示情報(</summary>
	public ListItemCollection UserCountryDisplayList { get; set; }
	/// <summary>ユーザー国情報(</summary>
	protected CountryLocationModel[] UserCountryList
	{
		get { return (CountryLocationModel[])Session["user_country_list"]; }
		set { Session["user_country_list"] = value; }
	}
	/// <summary>ユーザー州情報</summary>
	public ListItemCollection UserStateList { get; set; }
	/// <summary>ユーザー台湾都市情報</summary>
	public ListItemCollection UserTwCityList { get; set; }
	/// <summary>配送可能な国表示情報</summary>
	public ListItemCollection ShippingAvailableCountryDisplayList { get; set; }
	/// <summary>配送可能な国情報(</summary>
	protected CountryLocationModel[] ShippingAvailableCountryList
	{
		get { return (CountryLocationModel[])Session["shipping_available_country_list"]; }
		set { Session["shipping_available_country_list"] = value; }
	}
	/// <summary>注文者生年月日 年</summary>
	public ListItemCollection OrderOwnerBirthYear
	{
		get { return m_strOrderOwnerBirthYear; }
		set { m_strOrderOwnerBirthYear = value; }
	}
	private ListItemCollection m_strOrderOwnerBirthYear = new ListItemCollection();
	/// <summary>注文者生年月日 月</summary>
	public ListItemCollection OrderOwnerBirthMonth
	{
		get { return m_strOrderOwnerBirthMonth; }
		set { m_strOrderOwnerBirthMonth = value; }
	}
	private ListItemCollection m_strOrderOwnerBirthMonth = new ListItemCollection();
	/// <summary>注文者生年月日 日</summary>
	public ListItemCollection OrderOwnerBirthDay
	{
		get { return m_strOrderOwnerBirthDay; }
		set { m_strOrderOwnerBirthDay = value; }
	}
	private ListItemCollection m_strOrderOwnerBirthDay = new ListItemCollection();
	/// <summary>注文者性別</summary>
	public ListItemCollection OrderOwnerSex
	{
		get { return m_strOrderOwnerSex; }
		set { m_strOrderOwnerSex = value; }
	}
	private ListItemCollection m_strOrderOwnerSex = new ListItemCollection();
	/// <summary>配送方法リスト</summary>
	public List<ListItemCollection> ShippingMethodList { get; set; }
	/// <summary>メール便配送サービスリスト</summary>
	public List<List<DeliveryCompanyModel>> DeliveryCompanyListMail
	{
		get { return (List<List<DeliveryCompanyModel>>)ViewState["DeliveryCompanyListMail"]; }
		set { ViewState["DeliveryCompanyListMail"] = value; }
	}
	/// <summary>データバインド用配送種別情報</summary>
	public List<ShopShippingModel> ShopShippingList
	{
		get { return (List<ShopShippingModel>)ViewState["ShopShippingList"]; }
		set { ViewState["ShopShippingList"] = value; }
	}
	/// <summary>データバインド用配送会社情報</summary>
	public List<DeliveryCompanyModel> DeliveryCompanyList
	{
		get { return (List<DeliveryCompanyModel>)ViewState["DeliveryCompanyList"]; }
		set { ViewState["DeliveryCompanyList"] = value; }
	}
	/// <summary>データバインド用の選択可能の配送サービス情報</summary>
	protected List<Dictionary<string, Dictionary<string, string>>> SelectableDeliveryCompanyList
	{
		get
		{
			return (List<Dictionary<string, Dictionary<string, string>>>)ViewState["SelectableDeliveryCompanies"];
		}
		set { ViewState["SelectableDeliveryCompanies"] = value; }
	}
	/// <summary>最初のリピータアイテムユニークID</summary>
	public RepeaterItem FirstRpeaterItem
	{
		get { return (this.WrCartList.Items.Count != 0) ? this.WrCartList.Items[0] : null; }
	}
	/// <summary>選択された配送方法</summary>
	public string[] SelectedShippingMethod
	{
		get { return (string[])Session["SelectedShippingMethod"]; }
		set { Session["SelectedShippingMethod"] = value; }
	}
	/// <summary>ユーザーのアドレス帳情報</summary>
	public UserShippingModel[] UserShippingAddr
	{
		get { return (UserShippingModel[])ViewState["UserShippingAddr"]; }
		set { ViewState["UserShippingAddr"] = value; }
	}
	/// <summary>配送先にエラーがあるか</summary>
	protected bool HasUserShippingError
	{
		get { return (bool)ViewState["HasUserShippingError"]; }
		set { ViewState["HasUserShippingError"] = value; }
	}
	/// <summary>配送先国コード</summary>
	public string CountryIsoCode { get; set; }
	/// <summary>Convenience Store Limit Kg</summary>
	public decimal ConvenienceStoreLimitKg
	{
		get
		{
			var result = (Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG.Length > 0)
				? decimal.Parse(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0])
				: 0m;
			return result;
		}
	}
	/// <summary>Convenience Store Limit Kg 7-ELEVEN</summary>
	public decimal ConvenienceStoreLimitKg7Eleven
	{
		get
		{
			var result = (Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG.Length > 1)
				? decimal.Parse(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[1])
				: 0m;
			return result;
		}
	}
	/// <summary>Real shop info</summary>
	public RealShopModel RealShopInfo { get; set; }
	/// <summary>Can store pickup visible</summary>
	public bool CanStorePickupVisible { get; set; }
	/// <summary>Real shop area list</summary>
	public RealShopArea[] RealShopAreaList { get; set; }
	/// <summary>Real shop name list</summary>
	public RealShopModel[] RealShopNameList { get; set; }
	/// <summary>次のページが確認ページか</summary>
	public bool IsNextConfirmPage
	{
		get { return (this.CartList.CartNextPage == Constants.PAGE_FRONT_ORDER_CONFIRM); }
	}
	#region 利用していないが下位互換のため残しておく
	/// <summary>ユーザー区分がPCユーザーか判定</summary>
	[Obsolete("使用しないのであれば削除します", false)]
	protected bool IsUserKbnPCUser
	{
		get { return (this.UserKbn == Constants.FLG_USER_USER_KBN_PC_USER); }
	}
	/// <summary>ユーザー区分がスマートフォンユーザーか判定</summary>
	[Obsolete("使用しないのであれば削除します", false)]
	protected bool IsUserKbnSmartPhoneUser
	{
		get { return (this.UserKbn == Constants.FLG_USER_USER_KBN_SMARTPHONE_USER); }
	}
	/// <summary>パスワード表示フラグ</summary>
	public bool IsVisible_UserPassword
	{
		get { return (bool)ViewState["UserPassword"]; }
		set { ViewState["UserPassword"] = value; }
	}
	/// <summary>カートアイテムインデックス番号（複数カート時のグローバル情報設定に利用）</summary>
	public int CartItemIndexTmp { get; set; }
	/// <summary>カート配送先アイテムインデックス番号（複数カート時のグローバル情報設定に利用。ギフトのみで利用）</summary>
	public int CartShippingItemIndexTmp { get; set; }
	/// <summary>Is Landing Page</summary>
	public bool IsLandingPage { get; set; }
	/// <summary>Is Gift Page</summary>
	public bool IsGiftPage { get; set; }
	/// <summary>Authentication usable</summary>
	public bool AuthenticationUsable { get; set; }
	/// <summary>配送不可郵便番号</summary>
	public string UnavailableShippingZip
	{
		get
		{
			var unavailableShippingZip = new ShopShippingService().
				GetUnavailableShippingZipFromShippingDelivery(this.CartList.Items[0].ShippingType, this.CartList.Items[0].Shippings[0].DeliveryCompanyId);
			return unavailableShippingZip;
		}
	}
	/// <summary>Block error display</summary>
	public bool BlockErrorDisplay { get; set; }
	#endregion

	#endregion
	#endregion
}
