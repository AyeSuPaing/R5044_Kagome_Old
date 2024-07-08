/*
=========================================================================================================
  Module      : 注文配送先配送先選択画面処理(OrderShippingSelectShipping.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.UserShipping;

public partial class Form_Order_OrderShippingSelectShipping : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.IsGiftPage = true;
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			RefreshAll(e);
			return;
		}

		RedirectOrderShipping();
		RedirectOrderShippingSelect();

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カートチェック（カートが存在しない場合、エラーページへ）
		//------------------------------------------------------
		CheckCartExists();

		//------------------------------------------------------
		// カート注文者存在チェック
		//------------------------------------------------------
		CheckCartOwnerExists();

		// 検索結果レイヤーから住所を確定後、ポストバック発生で住所検索のエラーメッセージが再表示されてしまうためPostBack時に再度消す
		ResetAddressSearchResultErrorMessage(Constants.GIFTORDER_OPTION_ENABLED, rCartList);

		if (!IsPostBack)
		{
			//----------------------------------------------------
			// 次へボタン設定 ※配送先選択画面の次は商品選択画面固定だが、ここでは商品選択の繊維先をセットしている
			//----------------------------------------------------
			// 確認画面以外からの遷移の場合
			if ((Request.UrlReferrer != null)
				&& (Request.UrlReferrer.ToString().ToLower().IndexOf(Constants.PAGE_FRONT_ORDER_CONFIRM.ToLower()) != -1))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;
				// レコメンド商品追加時？
				if (this.IsAddRecmendItem)
				{
					this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
				}
			}
			else
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
			}

			//------------------------------------------------------
			// 注全画面リフレッシュ
			//------------------------------------------------------
			RefreshAll(e);
			// レコメンド商品投入時は、次へボタンクリックを行いエラーを表示させる
			if (this.IsAddRecmendItem)
			{
				lbNext_Click(sender, e);
			}

			// ギフト配送先エラーメッセージを表示する
			ShowShippingAreaErrorMessageForGiftOrder(rCartList);
		}
	}

	/// <summary>
	/// 全画面リフレッシュ
	/// </summary>
	/// <param name="e"></param>
	private void RefreshAll(EventArgs e)
	{
		//----------------------------------------------------
		// 配送情報入力画面初期処理（共通）
		//----------------------------------------------------
		InitComponentsOrderShipping();

		//------------------------------------------------------
		// データバインド準備
		//------------------------------------------------------
		// データバインド用配送種別情報取得
		this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();

		//------------------------------------------------------
		// データバインド後処理
		//------------------------------------------------------
		// 各種表示初期化(データバインド後に実行する必要がある)
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// 配送先リフレッシュ
			RefreshCartShippings(riCart, this.CartList.Items[riCart.ItemIndex].Shippings, e);

			// 配送先初期化
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			foreach (RepeaterItem riShipping in wrCartShippings.Items)
			{
				InitComponentsDispOrderShipping(riShipping, e);
			}
		}
	}

	/// <summary>
	/// 配送先追加リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddShipping_Click(object sender, System.EventArgs e)
	{
		LinkButton lbAddNewShipping = (LinkButton)sender;
		RepeaterItem riCart = (RepeaterItem)GetOuterControl(lbAddNewShipping, typeof(RepeaterItem));

		// 配送先作成
		CartObject cartObject = this.CartList.Items[riCart.ItemIndex];
		CartShipping cartShipping = new CartShipping(cartObject);
		cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
		cartShipping.ShippingMethod = this.CartList.Items[riCart.ItemIndex].Shippings[0].ShippingMethod;
		cartShipping.DeliveryCompanyId = this.CartList.Items[riCart.ItemIndex].Shippings[0].DeliveryCompanyId;

		// カートへ紐付け（＆メモ作成）
		List<CartShipping> lCartShippings = GetCurrentDisplayShippings(riCart);
		if (lCartShippings.Count > 0)
		{
			cartShipping.IsSameSenderAsShipping1 = true;
		}
		lCartShippings.Add(cartShipping);
		cartObject.Shippings.Add(cartShipping);

		// リフレッシュ
		RefreshCartShippings(riCart, lCartShippings, e);
	}

	/// <summary>
	/// 配送先削除リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteShipping_Click(object sender, System.EventArgs e)
	{
		LinkButton lbDeleteShipping = (LinkButton)sender;
		RepeaterItem riShipping = (RepeaterItem)GetOuterControl(lbDeleteShipping, typeof(RepeaterItem));
		RepeaterItem riCart = (RepeaterItem)GetOuterControl(riShipping, typeof(RepeaterItem));

		// 配送先削除
		int shippingIndex = int.Parse(lbDeleteShipping.CommandArgument);
		if (shippingIndex < this.CartList.Items[riCart.ItemIndex].Shippings.Count)
		{
			this.CartList.Items[riCart.ItemIndex].Shippings.RemoveAt(shippingIndex);
		}

		List<CartShipping> lCartShippings = GetCurrentDisplayShippings(riCart);
		lCartShippings.RemoveAt(shippingIndex);
		if ((shippingIndex == 0)
			&& (lCartShippings.Count > 0))
		{
			this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex].IsSameSenderAsShipping1
				= lCartShippings[shippingIndex].IsSameSenderAsShipping1
				= false;	// これをしないとのっぺらぼうの送り主表示になる
		}

		// リフレッシュ
		RefreshCartShippings(riCart, lCartShippings, e);
	}

	/// <summary>
	/// 配送先の入力情報取得
	/// </summary>
	/// <param name="riCart"></param>
	/// <returns></returns>
	protected List<CartShipping> GetCurrentDisplayShippings(RepeaterItem riCart)
	{
		List<CartShipping> lShippings = new List<CartShipping>();

		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			var rblSenderSelector = GetWrappedControl<WrappedRadioButtonList>(riShipping, "rblSenderSelector", CartShipping.AddrKbn.Owner.ToString());
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingKbnList", CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
			var wcbSameSenderAsShipping1 = GetWrappedControl<WrappedCheckBox>(riShipping, "cbSameSenderAsShipping1");
			
			CartShipping cartShipping = new CartShipping(this.CartList.Items[riCart.ItemIndex]);

			if (wcbSameSenderAsShipping1.Checked)
			{
				cartShipping.UpdateSenderAddr(lShippings[0], true);
			}
			else
			{
				// 送り主セット
				switch ((CartShipping.AddrKbn)Enum.Parse(typeof(CartShipping.AddrKbn), rblSenderSelector.SelectedValue))
				{
					case CartShipping.AddrKbn.New:

						// ラップ済みコントロール宣言
						var wtbSenderName1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderName1");
						var wtbSenderName2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderName2");
						var wtbSenderNameKana1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderNameKana1");
						var wtbSenderNameKana2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderNameKana2");
						var wtbSenderZip = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip");
						var wtbSenderZip1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip1");
						var wtbSenderZip2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip2");
						var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZipGlobal");
						var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlSenderAddr1");
						var wtbSenderAddr2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr2");
						var wtbSenderAddr3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr3");
						var wtbSenderAddr4 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr4");
						var wtbSenderAddr5 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr5");
						var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlSenderCountry");
						var wtbSenderCompanyName = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderCompanyName");
						var wtbSenderCompanyPostName = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderCompanyPostName");
						var wtbSenderTel1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1");
						var wtbSenderTel1_1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_1");
						var wtbSenderTel1_2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_2");
						var wtbSenderTel1_3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_3");
						var wtbSenderTel1Global = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1Global");

						var senderCountryIsoCode = string.Empty;
						var senderCountryName = string.Empty;
						var senderZip = wtbSenderZip.Text.Trim();
						var senderZip1 = wtbSenderZip1.HasInnerControl
							? wtbSenderZip1.Text.Trim()
							: wtbSenderZip.Text.Trim();
						var senderZip2 = wtbSenderZip2.Text.Trim();
						var senderTel = wtbSenderTel1.Text.Trim();
						var senderTel1 = wtbSenderTel1_1.HasInnerControl
							? wtbSenderTel1_1.Text.Trim()
							: wtbSenderTel1.Text.Trim();
						var senderTel2 = wtbSenderTel1_2.Text.Trim();
						var senderTel3 = wtbSenderTel1_3.Text.Trim();

						// Set value for zip
						if (wtbSenderZip2.HasInnerControl == false)
						{
							var zip = new ZipCode(StringUtility.ToHankaku(senderZip1));
							senderZip = zip.Zip;
							senderZip1 = zip.Zip1;
							senderZip2 = zip.Zip2;
						}

						// Set value for tel
						if (wtbSenderTel1_2.HasInnerControl == false)
						{
							var tel = new Tel(StringUtility.ToHankaku(senderTel1));
							senderTel = tel.TelNo;
							senderTel1 = tel.Tel1;
							senderTel2 = tel.Tel2;
							senderTel3 = tel.Tel3;
						}

						if (Constants.GLOBAL_OPTION_ENABLE)
						{
							senderCountryName = wddlSenderCountry.SelectedText;
							senderCountryIsoCode = wddlSenderCountry.SelectedValue;

							if (IsCountryJp(senderCountryIsoCode) == false)
							{
								senderZip = wtbSenderZipGlobal.Text.Trim();
								senderZip1 = string.Empty;
								senderZip2 = string.Empty;
								senderTel = wtbSenderTel1Global.Text.Trim();
								senderTel1 = string.Empty;
								senderTel2 = string.Empty;
								senderTel3 = string.Empty;
							}
						}

						cartShipping.UpdateSenderAddr(
							wtbSenderName1.Text,
							wtbSenderName2.Text,
							wtbSenderNameKana1.Text,
							wtbSenderNameKana2.Text,
							senderZip,
							senderZip1,
							senderZip2,
							wddlShippingAddr1.SelectedValue,
							wtbSenderAddr2.Text.Trim(),
							wtbSenderAddr3.Text.Trim(),
							wtbSenderAddr4.Text.Trim(),
							wtbSenderAddr5.Text.Trim(),
							senderCountryIsoCode,
							senderCountryName,
							wtbSenderCompanyName.Text,
							wtbSenderCompanyPostName.Text,
							senderTel,
							senderTel1,
							senderTel2,
							senderTel3,
							false,
							CartShipping.AddrKbn.New);
						break;

					case CartShipping.AddrKbn.Owner:
						cartShipping.UpdateSenderAddr(this.CartList.Owner, false);
						break;
				}
			}

			// 配送先セット
			switch (wddlShippingKbnList.SelectedValue)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:

					// ラップ済みコントロール宣言
					var wtbUserShippingName = GetWrappedControl<WrappedTextBox>(riShipping, "tbUserShippingName");
					var wtbShippingName1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingName1");
					var wtbShippingName2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingName2");
					var wtbShippingNameKana1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingNameKana1");
					var wtbShippingNameKana2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingNameKana2");
					var wtbShippingZip = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip");
					var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip1");
					var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip2");
					var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZipGlobal");
					var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingCountry");
					var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingAddr1");
					var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr2");
					var wtbShippingAddr3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr3");
					var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr4");
					var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingAddr5");
					var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr5");
					var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingCompanyName");
					var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingCompanyPostName");
					var wtbShippingTel = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1");
					var wtbShippingTel1_1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_1");
					var wtbShippingTel1_2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_2");
					var wtbShippingTel1_3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_3");
					var wtbShippingTel1Global = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1Global");
					var wrblSaveToUserShipping = GetWrappedControl<WrappedRadioButtonList>(riShipping, "rblSaveToUserShipping");

					var shippingCountryIsoCode = string.Empty;
					var shippingCountryName = string.Empty;
					var shippingAddr5 = string.Empty;
					var shippingZip = wtbShippingZip.Text;
					var shippingZip1 = wtbShippingZip1.HasInnerControl
						? wtbShippingZip1.Text
						: wtbShippingZip.Text;
					var shippingZip2 = wtbShippingZip2.Text;
					var shippingTel = wtbShippingTel.Text;
					var shippingTel1 = wtbShippingTel1_1.HasInnerControl
						? wtbShippingTel1_1.Text
						: wtbShippingTel.Text;
					var shippingTel2 = wtbShippingTel1_2.Text;
					var shippingTel3 = wtbShippingTel1_3.Text;

					// Set value for zip
					if (wtbShippingZip2.HasInnerControl == false)
					{
						var zip = new ZipCode(StringUtility.ToHankaku(shippingZip1));
						shippingZip = zip.Zip;
						shippingZip1 = zip.Zip1;
						shippingZip2 = zip.Zip2;
					}

					// Set value for tel
					if (wtbShippingTel1_2.HasInnerControl == false)
					{
						var tel = new Tel(StringUtility.ToHankaku(shippingTel1));
						shippingTel = tel.TelNo;
						shippingTel1 = tel.Tel1;
						shippingTel2 = tel.Tel2;
						shippingTel3 = tel.Tel3;
					}

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						shippingCountryName = wddlShippingCountry.SelectedText;
						shippingCountryIsoCode = wddlShippingCountry.SelectedValue;
						shippingAddr5 = IsCountryUs(shippingCountryIsoCode)
							? wddlShippingAddr5.SelectedText
							: wtbShippingAddr5.Text.Trim();

						if (IsCountryJp(shippingCountryIsoCode) == false)
						{
							shippingZip = wtbShippingZipGlobal.Text;
							shippingZip1 = string.Empty;
							shippingZip2 = string.Empty;
							shippingTel = wtbShippingTel1Global.Text;
							shippingTel1 = string.Empty;
							shippingTel2 = string.Empty;
							shippingTel3 = string.Empty;
						}
					}

					cartShipping.UpdateShippingAddr(
						wtbShippingName1.Text,
						wtbShippingName2.Text,
						wtbShippingNameKana1.Text,
						wtbShippingNameKana2.Text,
						shippingZip,
						shippingZip1,
						shippingZip2,
						wddlShippingAddr1.SelectedValue,
						wtbShippingAddr2.Text.Trim(),
						wtbShippingAddr3.Text.Trim(),
						wtbShippingAddr4.Text.Trim(),
						shippingAddr5,
						shippingCountryIsoCode,
						shippingCountryName,
						wtbShippingCompanyName.Text,
						wtbShippingCompanyPostName.Text,
						shippingTel,
						shippingTel1,
						shippingTel2,
						shippingTel3,
						false,
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
					cartShipping.UpdateUserShippingRegistSetting(
						wrblSaveToUserShipping.SelectedValue == "1",
						wtbUserShippingName.Text);
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					cartShipping.UpdateShippingAddr(this.CartList.Owner, false);
					break;

				default:
					cartShipping.ShippingAddrKbn = wddlShippingKbnList.SelectedValue;

					// ユーザ配送先情報画面セット
					var userShipping = new UserShippingService().Get(this.LoginUserId, int.Parse(wddlShippingKbnList.SelectedValue));
					if (userShipping != null)
					{
						cartShipping.UpdateShippingAddr(
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
							false,
							wddlShippingKbnList.SelectedValue);
					}
					break;
			}

			lShippings.Add(cartShipping);
		}

		return lShippings;
	}

	/// <summary>
	/// 配送先部分以下リフレッシュ
	/// </summary>
	/// <param name="riCart"></param>
	/// <param name="lCartShippings"></param>
	/// <param name="e"></param>
	private void RefreshCartShippings(RepeaterItem riCart, List<CartShipping> lCartShippings, EventArgs e)
	{
		InitComponentsOrderShipping();

		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		wrCartShippings.DataSource = lCartShippings;
		wrCartShippings.DataBind();

		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingKbnList");
			if (wddlShippingKbnList.InnerControl != null)
			{
				ddlShippingKbnList_OnSelectedIndexChanged(wddlShippingKbnList.InnerControl, e);
			}
		}
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, System.EventArgs e)
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
	}

	/// <summary>
	/// 次ページへ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		// 配送先存在チェック
		var wrCartList = GetWrappedControl<WrappedRepeater>("rCartList");
		foreach (RepeaterItem riCart in wrCartList.Items)
		{
			if (this.CartList.Items[riCart.ItemIndex].IsGift)
			{
				var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
				if (wrCartShippings.Items.Count == 0)
				{
					var whcErrorMessages = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "hcErrorMessages");
					whcErrorMessages.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_UNSELECTED_ERROR);
					return;
				}
			}
		}

		// 注文配送情報入力画面 送り主情報セット処理
		if (lbNext_Click_OrderShipping_ShippingSender(sender, e) == false)
		{
			return;
		}

		// 注文配送情報入力画面次へリンククリック処理（共通）
		if (lbNext_Click_OrderShipping_Shipping(sender, e) == false)
		{
			return;
		}

		// 配送先が１つなら自動紐付け
		foreach (CartObject co in this.CartList)
		{
			if (co.Shippings.Count == 1)
			{
				co.Shippings[0].ProductCounts.Clear();
				foreach (CartProduct cp in co.Items)
				{
					co.Shippings[0].ProductCounts.Add(new CartShipping.ProductCount(cp, cp.Count));
				}
			}
		}

		// 決済が使えない状態になっていたら決済入力画面へ遷移する
		if (CheckPayment() == false)
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}

		// 配送先不可エリアチェック
		CheckCartOwnerShippingArea();

		// 画面遷移		※this.CartList.CartNextPageは次のページで利用する
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT;

		// 次のページへ遷移
		var addItemFlg = this.IsAddRecmendItem ? string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG) : string.Empty;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT + addItemFlg);
	}


	/// <summary>
	/// 代引きで複数配送先を選択していないかチェック
	/// </summary>
	/// <returns>チェックOKか</returns>
	public bool CheckPayment()
	{
		if (Constants.GIFTORDER_OPTION_ENABLED == false) return true;
		return
			this.CartList.Items.All(
				cart => (cart.Shippings.Count <= 1)
					|| (cart.Payment == null)
					|| (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT));
	}

	/// <summary>戻るイベント格納用</summary>
	protected string BackEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + WlbBack.UniqueID + "', '', true, '', '', false, true))"; }
	}
	/// <summary>戻るonclick</summary>
	protected string BackOnClick
	{
		get { return "return true"; }
	}
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.lbNext.UniqueID + "', '', true, '" + this.lbNext.ValidationGroup + "', '', false, true))"; }
	}
}
