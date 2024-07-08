/*
=========================================================================================================
  Module      : 頒布会選択可能商品入力クラス (SubscriptionBoxItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Globalization;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input;
using w2.Domain.SubscriptionBox;

namespace Input.SubscriptionBox
{
	/// <summary>
	/// 頒布会選択可能商品入力クラス
	/// </summary>
	[Serializable]
	public class SubscriptionBoxItemInput : InputBase<SubscriptionBoxItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxItemInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SubscriptionBoxItemInput(SubscriptionBoxItemModel model)
			: this()
		{
			this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
			this.BranchNo = model.BranchNo.ToString();
			this.ShopId = model.ShopId;
			this.ProductId = model.ProductId;
			this.VariationId = model.VariationId;
			this.SelectableSince = (model.SelectableSince != null)
				? model.SelectableSince.ToString()
				: null;
			this.SelectableUntil = (model.SelectableUntil != null)
				? model.SelectableUntil.ToString()
				: null;
			this.CampaignSince = (model.CampaignSince != null)
				? model.CampaignSince.ToString()
				: null;
			this.CampaignUntil = (model.CampaignUntil != null)
				? model.CampaignUntil.ToString()
				: null;
			this.CampaignPrice = (model.CampaignPrice != null)
				? model.CampaignPrice.ToString()
				: null;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// カレンダーテキストボックス用 TermSince取得<br />
		/// 日付形式はISO8601準拠
		/// </summary>
		/// <returns>yyyy-MM-dd形式</returns>
		public string GetSelectableSinceForCalendarTextBox()
		{
			var result = ParseTermsInternal(this.SelectableSince);
			return result.HasValue ? result.Value.ToString(SubscriptionBoxModel.DATE_FORMAT) : "";
		}

		/// <summary>
		/// カレンダーテキストボックス用 TermUntil取得<br />
		/// 日付形式はISO8601準拠
		/// </summary>
		/// <returns>yyyy-MM-dd形式</returns>
		public string GetSelectableUntilForCalendarTextBox()
		{
			var result = ParseTermsInternal(this.SelectableUntil);
			return result.HasValue ? result.Value.ToString(SubscriptionBoxModel.DATE_FORMAT) : "";
		}

		/// <summary>
		/// パース内部処理
		/// </summary>
		/// <returns>DateTime</returns>
		private DateTime? ParseTermsInternal(string termValue)
		{
			DateTime tmp;
			var result = DateTime.TryParse(termValue, out tmp)
				? tmp
				: (DateTime?)null;
			return result;
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override SubscriptionBoxItemModel CreateModel()
		{
			var model = new SubscriptionBoxItemModel
			{
				SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
				BranchNo = int.Parse(this.BranchNo),
				ShopId = this.ShopId,
				ProductId = this.ProductId,
				VariationId = this.VariationId,
				SelectableSince = (string.IsNullOrEmpty(this.SelectableSince) == false)
					? DateTime.Parse(this.SelectableSince + " 00:00:00.000")
					: (DateTime?)null,
				SelectableUntil = (string.IsNullOrEmpty(this.SelectableUntil) == false)
					? DateTime.Parse(this.SelectableUntil + " 23:59:59.997")
					: (DateTime?)null,
				CampaignSince = (string.IsNullOrEmpty(this.CampaignSince) == false)
					? DateTime.Parse(this.CampaignSince)
					: (DateTime?)null,
				CampaignUntil = (string.IsNullOrEmpty(this.CampaignUntil) == false)
					? DateTime.Parse(this.CampaignUntil)
					: (DateTime?)null,
				CampaignPrice = (string.IsNullOrEmpty(this.CampaignPrice) == false)
					? decimal.Parse(this.CampaignPrice)
					: (decimal?)null,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errorMessage = Validator.Validate("SubscriptionBoxItem", this.DataSource);

			// 期間の設定が未来～過去になっていないか
			if ((this.SelectableSince != null)
				&& (Validator.IsDate(this.SelectableSince))
				&& (Validator.IsDate(this.SelectableUntil))
				&& (Validator.CheckDateRange(this.SelectableSince, this.SelectableUntil) == false))
			{
				errorMessage += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "選択可能期間");
			}

			// キャンペーン期間が選択可能期間外になってないか
			if (((string.IsNullOrEmpty(this.CampaignSince) == false)
					&& ((string.IsNullOrEmpty(this.SelectableSince) == false)
						&& (DateTime.Parse(this.CampaignSince) >= DateTime.Parse(this.SelectableSince + " 00:00:00.000") == false)
						|| (string.IsNullOrEmpty(this.SelectableUntil) == false)
						&& (DateTime.Parse(this.CampaignSince) <= DateTime.Parse(this.SelectableUntil + " 23:59:59.997") == false)))
				|| ((string.IsNullOrEmpty(this.CampaignUntil) == false)
					&& ((string.IsNullOrEmpty(this.SelectableSince) == false)
						&& (DateTime.Parse(this.CampaignUntil) >= DateTime.Parse(this.SelectableSince + " 00:00:00.000") == false)
						|| (string.IsNullOrEmpty(this.SelectableUntil) == false)
						&& (DateTime.Parse(this.CampaignUntil) <= DateTime.Parse(this.SelectableUntil + " 23:59:59.997") == false))))
			{
				errorMessage += WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_CAMPAIGN_PERIOD_IS_NOT_WITHIN_THE_SELECTABLE_PERIOD);
			}

			// キャンペーン期間価格が半角数値でない
			decimal campaignPrice = 0;
			if ((string.IsNullOrEmpty(this.CampaignPrice) == false)
				&& (decimal.TryParse(this.CampaignPrice, out campaignPrice) == false))
			{
				errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_WRONG_SETTING_CAMPAIGN_PRICE);
			}
			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_BRANCH_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID] = value; }
		}
		/// <summary>選択可能期間開始（日付型）</summary>
		public DateTime? SelectableSinceDate
		{
			get
			{
				DateTime dateTmp;
				if (DateTime.TryParse(this.SelectableSince, out dateTmp) == false)
				{
					return null;
				}

				return dateTmp;
			}
		}
		/// <summary>選択可能期間開始</summary>
		public string SelectableSince
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE] = value; }
		}
		/// <summary>選択可能期間開始（日付型）</summary>
		public DateTime? SelectableUntilDate
		{
			get
			{
				DateTime dateTmp;
				if (DateTime.TryParse(this.SelectableUntil, out dateTmp) == false)
				{
					return null;
				}

				return dateTmp;
			}
		}
		/// <summary>選択可能期間終了</summary>
		public string SelectableUntil
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL] = value; }
		}
		/// <summary>キャンペーン期間開始</summary>
		public string CampaignSince
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE] = value; }
		}
		/// <summary>キャンペーン期間終了</summary>
		public string CampaignUntil
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL] = value; }
		}
		/// <summary>キャンペーン期間価格</summary>
		public string CampaignPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE] = value; }
		}

		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>配送種別</summary>
		public string ShippingType { get; set; }
		#endregion
	}
}
