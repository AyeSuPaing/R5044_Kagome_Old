/*
=========================================================================================================
  Module      : 商品グループ入力クラス (ProductGroupInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.ProductGroup;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 商品グループ入力クラス
	/// </summary>
	public class ProductGroupInput : InputBase<ProductGroupModel>
	{
		#region 列挙体
		/// <summary>入力チェック区分</summary>
		public enum EnumProductGroupInputValidationKbn
		{
			/// <summary>登録</summary>
			Register = 0,
			/// <summary>更新</summary>
			Update = 1,
			/// <summary>削除</summary>
			Delete = 2
		};
		#endregion

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductGroupInput()
		{
			this.ProductGroupId = "";
			this.ProductGroupName = "";

			this.BeginDate = "";
			this.BeginDateYear = "";
			this.BeginDateMonth = "";
			this.BeginDateDay = "";
			this.BeginDateHour = "";
			this.BeginDateMinute = "";
			this.BeginDateSecond = "";

			this.EndDate = null;
			this.EndDateYear = null;
			this.EndDateMonth = null;
			this.EndDateDay = null;
			this.EndDateHour = null;
			this.EndDateMinute = null;
			this.EndDateSecond = null;

			this.ValidFlg = false;
			this.ProductGroupPageContentsKbn = Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_TEXT;
			this.ProductGroupPageContents = "";
			this.ItemIds = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public ProductGroupInput(ProductGroupModel model)
		{
			this.ProductGroupId = model.ProductGroupId;
			this.ProductGroupName = model.ProductGroupName;

			this.BeginDate = model.BeginDate.ToString();
			this.BeginDateYear = model.BeginDate.ToString("yyyy");
			this.BeginDateMonth = model.BeginDate.ToString("MM");
			this.BeginDateDay = model.BeginDate.ToString("dd");
			this.BeginDateHour = model.BeginDate.ToString("HH");
			this.BeginDateMinute = model.BeginDate.ToString("mm");
			this.BeginDateSecond = model.BeginDate.ToString("ss");

			this.EndDate = model.EndDate.HasValue ? model.EndDate.Value.ToString() : null;
			this.EndDateYear = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy") : null;
			this.EndDateMonth = model.EndDate.HasValue ? model.EndDate.Value.ToString("MM") : null;
			this.EndDateDay = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd") : null;
			this.EndDateHour = model.EndDate.HasValue ? model.EndDate.Value.ToString("HH") : null;
			this.EndDateMinute = model.EndDate.HasValue ? model.EndDate.Value.ToString("mm") : null;
			this.EndDateSecond = model.EndDate.HasValue ? model.EndDate.Value.ToString("ss") : null;

			this.ValidFlg = (model.ValidFlg == Constants.FLG_PRODUCTGROUP_VALID_FLG_VALID);
			this.ProductGroupPageContentsKbn = model.ProductGroupPageContentsKbn;
			this.ProductGroupPageContents = model.ProductGroupPageContents;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.ItemIds = string.Join(Environment.NewLine, model.Items.Select(i => i.MasterId).ToArray());
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override ProductGroupModel CreateModel()
		{
			var i = 1;
			var model = new ProductGroupModel
			{
				ProductGroupId = this.ProductGroupId,
				ProductGroupName = this.ProductGroupName,
				BeginDate = DateTime.Parse(this.BeginDate),
				EndDate = (string.IsNullOrEmpty(this.EndDate) == false) ? DateTime.Parse(this.EndDate) : (DateTime?)null,
				ValidFlg = this.ValidFlg ? Constants.FLG_PRODUCTGROUP_VALID_FLG_VALID : Constants.FLG_PRODUCTGROUP_VALID_FLG_INVALID,
				ProductGroupPageContentsKbn = this.ProductGroupPageContentsKbn,
				ProductGroupPageContents = this.ProductGroupPageContents,
				LastChanged = this.LastChanged,
				Items = this.ItemIds
					.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
					.Select(p => p.Trim())
					.Where(p => (string.IsNullOrWhiteSpace(p) == false))
					.Select(s => new ProductGroupItemModel
					{
						ProductGroupId = this.ProductGroupId,
						ItemNo = i++,
						ItemType = Constants.FLG_PRODUCTGROUPITEM_ITEM_TYPE_PRODUCT,
						ShopId = Constants.CONST_DEFAULT_SHOP_ID,
						MasterId = s,
						LastChanged = this.LastChanged,
					})
					.ToArray()
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="validationKbn">入力チェック区分</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(EnumProductGroupInputValidationKbn validationKbn)
		{
			if (validationKbn == EnumProductGroupInputValidationKbn.Delete)
			{
				return CheckProductGroupDeleteValid();
			}

			SetDate();

			var errorMessage = Validator.Validate("ProductGroupRegister", this.DataSource);
			var productIds = this.ItemIds
				.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.Where(p => (string.IsNullOrWhiteSpace(p) == false))
				.ToArray();
			foreach (var productId in productIds)
			{
				errorMessage += CheckValidProduct(Constants.CONST_DEFAULT_SHOP_ID, productId);
			}
			// 商品ID重複チェック
			var tmpProductIds = new string[0];
			var errorProuctIds = new string[0];
			foreach (var productId in productIds)
			{
				if (errorProuctIds.Contains(productId)) continue;

				if (tmpProductIds.Contains(productId))
				{
					errorProuctIds = errorProuctIds.Concat(new[] { productId }).ToArray();
					errorMessage += WebMessages.ProductDuplication.Replace("@@ 1 @@", productId);
					continue;
				}

				tmpProductIds = tmpProductIds.Concat(new[] { productId }).ToArray();
			}

			if ((errorMessage.Length == 0) && (validationKbn == EnumProductGroupInputValidationKbn.Register))
			{
				errorMessage = CheckDupulicationProductGroupId();
			}

			// 開始日時 <= 終了日時チェック
			DateTime beginDate;
			DateTime endDate;
			if ((DateTime.TryParse(this.EndDate, out endDate))
				&& (DateTime.TryParse(this.BeginDate, out beginDate))
				&& (beginDate > endDate))
			{
				errorMessage += WebMessages.ProductGroupDaterangeError;
			}

			return errorMessage;
		}

		/// <summary>
		/// 日時セット
		/// </summary>
		public void SetDate()
		{
			this.BeginDate = string.Format(
				"{0}/{1}/{2} {3}:{4}:{5}",
				this.BeginDateYear,
				this.BeginDateMonth,
				this.BeginDateDay,
				this.BeginDateHour,
				this.BeginDateMinute,
				this.BeginDateSecond);

			if ((this.EndDateYear != "")
				|| (this.EndDateMonth != "")
				|| (this.EndDateDay != "")
				|| (this.EndDateHour != "")
				|| (this.EndDateMinute != "")
				|| (this.EndDateSecond != ""))
			{
				this.EndDate = string.Format(
					"{0}/{1}/{2} {3}:{4}:{5}",
					this.EndDateYear,
					this.EndDateMonth,
					this.EndDateDay,
					this.EndDateHour,
					this.EndDateMinute,
					this.EndDateSecond);
			}
		}

		/// <summary>
		/// 商品有効性チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckValidProduct(string shopId, string productId)
		{
			var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(shopId, productId);
			if (product.Count == 0) { return WebMessages.ProductDelete.Replace("@@ 1 @@", productId); }

			if ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				return WebMessages.ProductInvalid.Replace("@@ 1 @@", productId);
			}
			return string.Empty;
		}

		/// <summary>
		/// 商品グループID重複チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckDupulicationProductGroupId()
		{
			string errorMessage = string.Empty;

			if (new ProductGroupService().CheckDupulicationProductGroupId(this.ProductGroupId) == false)
			{
				errorMessage = WebMessages.InputCheckDuplication.Replace("@@ 1 @@", this.ProductGroupId);
			}

			return errorMessage;
		}

		/// <summary>
		/// 商品グループ削除チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckProductGroupDeleteValid()
		{
			var count = new ProductGroupService().GetUsedProductGroupCount(this.ProductGroupId);
			string errorMessage = (count > 0)
				? WebMessages.InputCheckDeleteValid.Replace("@@ 1 @@", count.ToString())
				: string.Empty;
			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>商品グループID</summary>
		public string ProductGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID] = value; }
		}
		/// <summary>商品グループ名</summary>
		public string ProductGroupName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME] = value; }
		}
		/// <summary>開始日時</summary>
		public string BeginDate
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE] = value; }
		}
		/// <summary>終了日時</summary>
		public string EndDate
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE] = value; }
		}
		/// <summary>有効フラグ</summary>
		public bool ValidFlg
		{
			get { return (bool)this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG] = value; }
		}
		/// <summary>商品グループページ表示内容HTML区分</summary>
		public string ProductGroupPageContentsKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN] = value; }
		}
		/// <summary>商品グループページ表示内容</summary>
		public string ProductGroupPageContents
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED] = value; }
		}

		/// <summary>商品ID（改行区切り）</summary>
		public string ItemIds
		{
			get { return (string)this.DataSource["itemids"]; }
			set { this.DataSource["itemids"] = value; }
		}
		/// <summary>開始日時 年 From</summary>
		public string BeginDateYear { get; set; }
		/// <summary>開始日時 月 From</summary>
		public string BeginDateMonth { get; set; }
		/// <summary>開始日時 日 From</summary>
		public string BeginDateDay { get; set; }
		/// <summary>開始日時 時 From</summary>
		public string BeginDateHour { get; set; }
		/// <summary>開始日時 分 From</summary>
		public string BeginDateMinute { get; set; }
		/// <summary>開始日時 秒 From</summary>
		public string BeginDateSecond { get; set; }
		/// <summary>終了日時 年 To</summary>
		public string EndDateYear { get; set; }
		/// <summary>終了日時 月 To</summary>
		public string EndDateMonth { get; set; }
		/// <summary>終了日時 日 To</summary>
		public string EndDateDay { get; set; }
		/// <summary>終了日時 時 To</summary>
		public string EndDateHour { get; set; }
		/// <summary>終了日時 分 To</summary>
		public string EndDateMinute { get; set; }
		/// <summary>終了日時 秒 To</summary>
		public string EndDateSecond { get; set; }
		/// <summary>入力チェック区分</summary>
		public EnumProductGroupInputValidationKbn ValidationKbn { get; set; }
		#endregion
	}
}
