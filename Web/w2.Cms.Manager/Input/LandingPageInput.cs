/*
=========================================================================================================
  Module      : LP入力モデル(LandingPageInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using w2.App.Common.Input;
using w2.App.Common.LandingPage.LandingPageDesignData;
using w2.App.Common.Order;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain.LandingPage;
using w2.Domain.Product;
using Constants = w2.Cms.Manager.Codes.Constants;
using Validator = w2.Common.Util.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// LP入力モデル
	/// </summary>
	[Serializable]
	public class LandingPageInput : InputBase<LandingPageDesignModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public LandingPageInput(LandingPageDesignModel model)
		{
			this.PageTitle = model.PageTitle;
			this.Pagedesign = new PageDesignInput();
			this.ProductSets = new LandingPageProductSetInput[] { };
			this.PageId = StringUtility.ToEmpty(model.PageId);
			this.LoginFormType = (LandingPageConst.LOGIN_FORM_TYPE_VISIBLE == model.LoginFormType);
			this.EfoCubeUseFlg = (LandingPageConst.EFO_CUBE_USE_FLG_ON == model.EfoCubeUseFlg);
			this.OrderConfirmPageSkipFlg = (LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON == model.OrderConfirmPageSkipFlg);
			this.MailAddressConfirmFormUseFlg = (LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON == model.MailAddressConfirmFormUseFlg);
			this.CopySourceId = "";
			this.SocialLoginList = "";
			this.UnpermittedPaymentIds = "";
			this.DefaultPaymentId = "";
			this.NoveltyUseFlg = (LandingPageConst.NOVELTY_USE_FLG_ON == model.NoveltyUseFlg);
			this.PersonalAuthenticationUseFlg = (model.PersonalAuthenticationUseFlg == LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_ON);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override LandingPageDesignModel CreateModel()
		{
			var model = new LandingPageDesignModel();
			model.PageId = this.PageId;
			model.PageTitle = this.PageTitle;
			model.PageFileName = this.PageUrl;
			model.PublicStatus = this.PublicStatus;
			model.PublicStartDatetime = ParseDate(this.RangeStartDate, this.RangeStartTime);
			model.PublicEndDatetime = ParseDate(this.RangeEndDate, this.RangeEndTime);
			model.ProductChooseType = this.ProductChooseType;
			model.UserRegistrationType = this.UserRegistrationType;
			model.LoginFormType = (this.LoginFormType)
				? LandingPageConst.LOGIN_FORM_TYPE_VISIBLE
				: LandingPageConst.LOGIN_FORM_TYPE_HYDE;
			model.LastChanged = "";
			model.MetadataDesc = this.MetadataDesc;
			model.ManagementTitle = this.ManagementTitle;
			model.SocialLoginUseType = this.SocialLoginUseType;
			model.SocialLoginList = this.SocialLoginList;
			model.TagSettingList = this.TagSettingList;
			model.EfoCubeUseFlg = (this.EfoCubeUseFlg) 
				? LandingPageConst.EFO_CUBE_USE_FLG_ON
				: LandingPageConst.EFO_CUBE_USE_FLG_OFF;
			model.OrderConfirmPageSkipFlg = (this.OrderConfirmPageSkipFlg) 
				? LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON
				: LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_OFF;
			model.MailAddressConfirmFormUseFlg = (this.MailAddressConfirmFormUseFlg) 
				? LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON 
				: LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_OFF;
			model.UnpermittedPaymentIds = this.UnpermittedPaymentIds;
			model.ProductSets = this.ProductSets.Select(ps => ps.CreateModel()).ToArray();
			model.PaymentChooseType = this.PaymentChooseType;
			model.DefaultPaymentId = this.DefaultPaymentId ?? "";
			model.NoveltyUseFlg = this.NoveltyUseFlg
				? LandingPageConst.NOVELTY_USE_FLG_ON
				: LandingPageConst.NOVELTY_USE_FLG_OFF;
			model.DesignMode = this.DesignMode;
			model.PersonalAuthenticationUseFlg = this.PersonalAuthenticationUseFlg
				? LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_ON
				: LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_OFF;
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージリスト</returns>
		public List<string> Validate(bool register)
		{
			// 入力チェック
			var input = this.DataSource;

			var errorMessageList = Validator.Validate(register ? "LandingPageRegister" : "LandingPageModify", input)
				.Select(kv => kv.Value)
				.ToList();

			if ((Constants.CART_LIST_LP_OPTION == false)
				&& (this.PageUrl.ToUpper() == Constants.CART_LIST_LP_PAGE_NAME))
			{
				errorMessageList.Add(WebMessages.LandingPageCanNotRegisterCartListLpError);
			}

			// 商品選択タイプ "一覧指定の商品のうち、複数の商品を選択可能" を選べるかどうか検証
			if (Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION
				&& (this.ProductChooseType == LandingPageConst.PRODUCT_CHOOSE_TYPE_CHECKBOX))
			{
				var hasAnyProductSetsOfMultipleBuyTypes = this.ProductSets
					.Where(ps=> ps.SubscriptionBoxCourseFlg == false)
					.Any(ps => (ps.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_NORMAL))
						&& ps.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_FIXEDPURCHASE))));
				if (hasAnyProductSetsOfMultipleBuyTypes)
				{
					errorMessageList.Add(WebMessages.LandingPageChcekboxProductChooseTypeConfigurationError);
				}
			}

			// 除外する決済種別リスト中にデフォルト決済種別が存在する場合、エラーになる
			var unpermittedPaymentIds = StringUtility.SplitCsvLine(this.UnpermittedPaymentIds);
			var hasDefaultPaymentId = unpermittedPaymentIds.Any(id => (id == this.DefaultPaymentId));
			if (hasDefaultPaymentId) errorMessageList.Add(WebMessages.LandingPagePaymentSettingError);

			// 商品数量のチェック
			var sv = new ProductService();
			foreach (var productSet in this.ProductSets)
			{
				// 商品セットが1つの時は、選択肢名が未入力でも通す
				if (this.ProductSets.Length == 1) productSet.SetName = "dummy";
				errorMessageList.AddRange(productSet.Validate(register));
				if (this.ProductSets.Length == 1) productSet.SetName = string.Empty;

				if ((string.IsNullOrEmpty(productSet.SubscriptionBoxCourseId)) && (productSet.SubscriptionBoxCourseFlg))
				{
					errorMessageList.Add(
						WebMessages.LandingPageSubscriptionBoxCourseNoSelectError.Replace("@@ 1 @@", productSet.BranchNo));
				}

				if (productSet.SubscriptionBoxCourseFlg) continue;

				if ((productSet.Products == null) || (productSet.Products.Length == 0))
				{
					if (Constants.CART_LIST_LP_OPTION
						&& (this.PageUrl.ToUpper() == Constants.CART_LIST_LP_PAGE_NAME))
					{
						continue;
					}

					errorMessageList.Add(
						WebMessages.LandingPageProductNoSelectError.Replace("@@ 1 @@", productSet.BranchNo));
					continue;
				}

				errorMessageList.AddRange(
					productSet.Products.Where(p => Validator.IsHalfwidthNumber(p.Quantity) == false).Select(
						p => WebMessages.LandingPageProductCountError.Replace("@@ 1 @@", productSet.BranchNo)
							.Replace("@@ 2 @@", p.VariationId)));

				// 商品購入数をチェック
				// 11個以上はNG。10個まで
				if (productSet.Products.Length > 10)
				{
					errorMessageList.Add(WebMessages.LandingPageProductOver.Replace("@@ 1 @@", productSet.BranchNo));
				}

				foreach (var product in productSet.Products)
				{
					if ((productSet.Products.Count(
						     p => ((p.ProductId == product.ProductId)　&& (p.VariationId == product.VariationId))) > 1)
						&& (errorMessageList.Contains(
							WebMessages.LandingPageDuplicationError.Replace(
								"@@ 1 @@",
								product.VariationId)) == false))
					{
						errorMessageList.Add(WebMessages.LandingPageDuplicationError.Replace("@@ 1 @@", product.VariationId));
						break;
					}

					var productModel = sv.GetProductVariation(
						product.ShopId,
						product.ProductId,
						product.VariationId,
						null);
					if (productModel == null)
					{
						errorMessageList.Add(WebMessages.ProductDelete.Replace("@@ 1 @@", product.ProductId));
						break;
					}

					var productValidationErrorMessages = Validator.Validate(
							register
								? "LandingPageProductRegister"
								: "LandingPageProductModify",
							product.DataSource)
						.Select(kv => kv.Value
							.Replace("@@ product_set @@", productSet.BranchNo)
							.Replace("@@ product_id @@", product.VariationId));
					errorMessageList.AddRange(productValidationErrorMessages);

					switch (product.BuyType)
					{
						case LandingPageConst.BUY_TYPE_FIXEDPURCHASE:
						{
							if (productModel.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
							{
								// 定期購入不可のものがあればNG
								errorMessageList.Add(
									WebMessages.ProductFixedPurchaseDisable
										.Replace("@@ 1 @@", productSet.BranchNo)
										.Replace("@@ 2 @@", ProductCommon.CreateProductJointName(productModel.DataSource)));
							}

							break;
						}
						case LandingPageConst.BUY_TYPE_NORMAL:
						{
							if (productModel.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
							{
								// 定期購入のみものがあればNG
								errorMessageList.Add(
									WebMessages.ProductFixedPurchaseOnly
										.Replace("@@ 1 @@", productSet.BranchNo)
										.Replace("@@ 2 @@", ProductCommon.CreateProductJointName(productModel.DataSource)));
							}

							break;
						}
					}
				}
			}

			if (this.ProductSets.Any(ps => ps.ValidFlg) == false)
			{
				errorMessageList.Add(WebMessages.LandingPageProductSetNoValid);
			}

			// 公開期間チェック
			if (IsValidDatetimeTerm() == false)
			{
				errorMessageList.Add(WebMessages.ReleaseRangeSettingDateRangeError);
			}

			return errorMessageList;
		}

		/// <summary>
		/// 日付が未入力、または正しい日付範囲か
		/// </summary>
		/// <returns>結果</returns>
		private bool IsValidDatetimeTerm()
		{
			// 指定なしの場合スキップ
			if (string.IsNullOrEmpty(this.RangeStartDate + this.RangeStartTime + this.RangeEndDate + this.RangeEndTime)) return true;

			// 開始日時チェック
			var startDateTime = ParseDate(this.RangeStartDate, this.RangeStartTime);
			if (IsDatetimeWithinSqldatetimeRange(startDateTime) == false) return false;

			// 終了日時チェック
			var endDateTime = ParseDate(this.RangeEndDate, this.RangeEndTime);
			if (IsDatetimeWithinSqldatetimeRange(endDateTime) == false) return false;

			// 開始日時 < 終了日時チェック
			if (startDateTime.HasValue
				&& endDateTime.HasValue
				&& (startDateTime > endDateTime))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// SqlDateTimeの範囲内チェック
		/// </summary>
		/// <param name="target">チェック対象</param>
		/// <returns>結果(targetがnullの場合もtrue　日時範囲外のみfalse)</returns>
		private bool IsDatetimeWithinSqldatetimeRange(DateTime? target)
		{
			if (target.HasValue == false) return true;

			return (((DateTime)SqlDateTime.MinValue <= target.Value)
				&& (target.Value <= (DateTime)SqlDateTime.MaxValue));
		}

		/// <summary>
		/// 日時変換
		/// </summary>
		/// <param name="date">年月日</param>
		/// <param name="time">時分秒</param>
		/// <returns>変換後DateTime（エラー時はnull）</returns>
		private DateTime? ParseDate(string date, string time)
		{
			DateTime dtTmp;

			if (DateTime.TryParse(date, out dtTmp) == false) return null;

			if (string.IsNullOrEmpty(time)) return dtTmp.Date;
			if (DateTime.TryParse(date + " " + time, out dtTmp) == false) return null;

			return dtTmp;
		}

		/// <summary>
		/// エラーメッセージリストを<br />で結合 
		/// </summary>
		/// <param name="errorMessageList">エラーメッセージリスト</param>
		/// <returns>結合したエラーメッセージ</returns>
		public string CreateErrorJoinMessage(IEnumerable<string> errorMessageList)
		{
			var result = string.Join("<br />", errorMessageList);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID] = value; }
		}
		/// <summary>ページURL</summary>
		public string PageUrl
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME] = value; }
		}
		/// <summary>公開開始日</summary>
		public string RangeStartDate
		{
			get { return StringUtility.ToEmpty(this.DataSource["start_date"]); }
			set { this.DataSource["start_date"] = value; }
		}
		/// <summary>公開開始時間</summary>
		public string RangeStartTime
		{
			get { return StringUtility.ToEmpty(this.DataSource["start_time"]); }
			set { this.DataSource["start_time"] = value; }
		}
		/// <summary>公開終了日</summary>
		public string RangeEndDate
		{
			get { return StringUtility.ToEmpty(this.DataSource["end_date"]); }
			set { this.DataSource["end_date"] = value; }
		}
		/// <summary>公開終了時間</summary>
		public string RangeEndTime
		{
			get { return StringUtility.ToEmpty(this.DataSource["end_time"]); }
			set { this.DataSource["end_time"] = value; }
		}
		/// <summary>商品選択タイプ</summary>
		public string ProductChooseType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PRODUCT_CHOOSE_TYPE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PRODUCT_CHOOSE_TYPE] = value; }
		}
		/// <summary>会員登録タイプ</summary>
		public string UserRegistrationType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_USER_REGISTRATION_TYPE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_USER_REGISTRATION_TYPE] = value; }
		}
		/// <summary>ログインフォームタイプ</summary>
		public bool LoginFormType { get; set; }
		/// <summary>利用するソーシャルログインタイプ</summary>
		public string SocialLoginUseType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_USE_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_USE_TYPE] = value; }
		}
		/// <summary>ソーシャルログインリスト</summary>
		public string SocialLoginList
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_LIST]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_LIST] = value; }
		}
		/// <summary>タグ設定リスト</summary>
		public string TagSettingList
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_TAG_SETTING_LIST]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_TAG_SETTING_LIST] = value; }
		}
		/// <summary>EFO CUBE利用</summary>
		public bool EfoCubeUseFlg { get; set; }
		/// <summary>確認画面スキップ</summary>
		public bool OrderConfirmPageSkipFlg { get; set; }
		/// <summary>メールアドレス確認フォーム利用フラグ</summary>
		public bool MailAddressConfirmFormUseFlg { get; set; }
		/// <summary>除外する決済種別IDリスト</summary>
		public string UnpermittedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_UNPERMITTED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_UNPERMITTED_PAYMENT_IDS] = value; }
		}
		/// <summary>決済種別選択タイプ</summary>
		public string PaymentChooseType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAYMENT_CHOOSE_TYPE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAYMENT_CHOOSE_TYPE] = value; }
		}
		/// <summary>デフォルト決済種別</summary>
		public string DefaultPaymentId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DEFAULT_PAYMENT_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DEFAULT_PAYMENT_ID] = value; }
		}
		/// <summary>公開状態</summary>
		public string PublicStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS] = value; }
		}
		/// <summary>ページタイトル</summary>
		public string PageTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE] = value; }
		}
		/// <summary>コピー元のID</summary>
		public string CopySourceId
		{
			get { return StringUtility.ToEmpty(this.DataSource["copy_sorce_id"]); }
			set { this.DataSource["copy_sorce_id"] = value; }
		}
		/// <summary>SEOディスクリプション</summary>
		public string MetadataDesc
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_METADATA_DESC]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_METADATA_DESC] = value; }
		}
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MANAGEMENT_TITLE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_MANAGEMENT_TITLE] = value; }
		}
		/// <summary>デザインモード</summary>
		public string DesignMode
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE]); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE] = value; }
		}
		/// <summary>ページデザイン入力値</summary>
		public PageDesignInput Pagedesign { get; set; }
		/// <summary>商品セット入力値</summary>
		public LandingPageProductSetInput[] ProductSets { get; set; }
		/// <summary>ノベルティ利用フラグ</summary>
		public bool NoveltyUseFlg { get; set; }
		/// <summary>Personal authentication use flag</summary>
		public bool PersonalAuthenticationUseFlg { get; set; }
		#endregion
	}
}