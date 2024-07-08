/*
=========================================================================================================
  Module      : コーディネート入力クラス (CoordinateInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.Coordinate;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.Product;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// コーディネートマスタ入力クラス
	/// </summary>
	public class CoordinateInput : InputBase<CoordinateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CoordinateInput()
		{
			this.Image = new ImageInput();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override CoordinateModel CreateModel()
		{
			var productArray = (this.ProductInput != null && this.ProductInput[0].ProductList != null)
				? this.ProductInput[0].ProductList.Select(
						(product, i) => new ProductModel
						{
							ProductId = product.Id,
							VariationId = (string.IsNullOrEmpty(product.VariationId)) ? product.Id : product.VariationId,
							Name = product.Name,
							UseVariationFlg = (string.IsNullOrEmpty(product.VariationId))
								? Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE
								: Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE
						})
					.ToArray()
				: new ProductModel[0];

			var categoryList = new List<CoordinateCategoryModel>();
			if (string.IsNullOrEmpty(this.CoordinateCategoryNames) == false)
			{
				foreach (var name in this.CoordinateCategoryNames.Trim().Split(','))
				{
					var category = new CoordinateCategoryService().GetByName(name);
					if ((category != null)
						&& (categoryList.All(cat => cat.CoordinateCategoryId != category.CoordinateCategoryId)))
					{
						categoryList.Add(category);
					}
				}
			}

			var model = new CoordinateModel
			{
				CoordinateId = this.CoordinateId,
				CoordinateTitle = this.CoordinateTitle,
				CoordinateUrl = this.CoordinateUrl,
				CoordinateSummary = this.CoordinateSummary,
				InternalMemo = this.InternalMemo,
				StaffId = this.StaffId,
				RealShopId = this.RealShopId,
				DisplayKbn = this.DisplayKbn,
				HtmlTitle = this.HtmlTitle,
				MetadataDesc = this.MetadataDesc,
				DisplayDate = CreatedDisplayDate(this.DisplayDate1, this.DisplayDate2),
				LastChanged = this.LastChanged,
				CategoryList = categoryList,
				TagList =
					this.ContentsTagNames.Trim().Split(',').Select(
						name => string.IsNullOrEmpty(name)
							? new ContentsTagModel()
							: new ContentsTagModel{ ContentsTagName = name }).ToList(),
				ProductList = productArray.ToList()
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(string shopId)
		{
			var errorMessage = Validator.Validate("CoordinatePageModify", this.DataSource);

			if (this.ProductInput != null && this.ProductInput[0].ProductList != null)
			{
				// 商品重複・3軸商品チェック
				foreach (var product in this.ProductInput[0].ProductList)
				{
					if ((HasThreeAxes(product.Id, shopId)))
					{
						errorMessage += WebMessages.CoordinatePageVariationError.Replace("@@ 1 @@", product.Id) + ("<br />");
					}
					if ((this.ProductInput[0].ProductList.Count(p => p.Id == product.Id) > 1)
						&& (errorMessage.Contains(WebMessages.CoordinatePageDuplicationError.Replace("@@ 1 @@", product.Id)) == false))
					{
						errorMessage += WebMessages.CoordinatePageDuplicationError.Replace("@@ 1 @@", product.Id) + ("<br />");
					}
				}
			}

			if (string.IsNullOrEmpty(this.CoordinateCategoryNames) == false)
			{
				// カテゴリが存在するかチェック
				foreach (var name in this.CoordinateCategoryNames.Trim().Split(','))
				{
					if ((new CoordinateCategoryService().GetByName(name) == null))
					{
						errorMessage += WebMessages.CoordinatePageCategoryError.Replace("@@ 1 @@", name) + ("<br />");
					}
				}
			}

			if (CreatedDisplayDate(this.DisplayDate1, this.DisplayDate2) == null)
			{
				errorMessage += WebMessages.CoordinatePageDatetimeNoTerm + ("<br />");
			}

			return errorMessage;
		}
		#endregion

		/// <summary>
		/// 3軸あるか
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>2軸あるか</returns>
		protected bool HasThreeAxes(string productId, string shopId)
		{
			foreach (DataRowView drvProduct in ProductCommon.GetProductInfo(shopId, productId, ""))
			{
				if (string.IsNullOrEmpty((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3])) return false;
			}
			return true;
		}

		/// <summary>
		/// 公開日を作成
		/// </summary>
		/// <param name="date">日付</param>
		/// <param name="time">時間</param>
		/// <returns>公開日</returns>
		protected DateTime? CreatedDisplayDate(string date, string time)
		{
			if (string.IsNullOrEmpty(date)) return null;
			DateTime dateTime;
			var text = date;
			if (string.IsNullOrEmpty(time) == false) text += " " + time;

			if (DateTime.TryParse(text, out dateTime) == false) return null;
			if ((DateTime)SqlDateTime.MinValue > dateTime) return (DateTime)SqlDateTime.MinValue;
			if (dateTime > (DateTime)SqlDateTime.MaxValue) return (DateTime)SqlDateTime.MaxValue;

			return dateTime;
		}

		#region プロパティ
		/// <summary>画像</summary>
		public ImageInput Image { get; set; }
		/// <summary>商品一覧入力</summary>
		public ProductInputModel[] ProductInput { get; set; }
		/// <summary>コーディネートID</summary>
		public string CoordinateId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_ID] = value; }
		}
		/// <summary>コーディネートタイトル</summary>
		public string CoordinateTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_TITLE]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_TITLE] = value; }
		}
		/// <summary>コーディネートURL</summary>
		public string CoordinateUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_URL]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_URL] = value; }
		}
		/// <summary>コーディネート概要</summary>
		public string CoordinateSummary
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_SUMMARY]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_COORDINATE_SUMMARY] = value; }
		}
		/// <summary>内部用メモ</summary>
		public string InternalMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_INTERNAL_MEMO]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_INTERNAL_MEMO] = value; }
		}
		/// <summary>スタッフID</summary>
		public string StaffId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_STAFF_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_STAFF_ID] = value; }
		}
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_REAL_SHOP_ID] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_KBN] = value; }
		}
		/// <summary>タイトル</summary>
		public string HtmlTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_HTML_TITLE]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_HTML_TITLE] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_METADATA_DESC] = value; }
		}
		/// <summary>公開日（日付）</summary>
		public string DisplayDate1
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE + "1"]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE+"1"] = value; }
		}
		/// <summary>公開日（時間）</summary>
		public string DisplayDate2
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE + "2"]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DISPLAY_DATE + "2"] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATE_LAST_CHANGED] = value; }
		}
		/// <summary>カテゴリ名(カンマ区切り)</summary>
		public string CoordinateCategoryNames
		{
			get { return (string)this.DataSource[Constants.FLG_COORDINATE_COORDINATE_CATEGORY_NAMES]; }
			set { this.DataSource[Constants.FLG_COORDINATE_COORDINATE_CATEGORY_NAMES] = value; }
		}
		/// <summary>コンテンツタグ名(カンマ区切り)</summary>
		public string ContentsTagNames
		{
			get { return (string)this.DataSource[Constants.FLG_COORDINATE_CONTENTS_TAG_NAMES]; }
			set { this.DataSource[Constants.FLG_COORDINATE_CONTENTS_TAG_NAMES] = value; }
		}
		/// <summary>商品ID(カンマ区切り)</summary>
		public string ProductIds
		{
			get { return (string)this.DataSource[Constants.FLG_COORDINATE_COORDINATE_PRODUCT_IDS]; }
			set { this.DataSource[Constants.FLG_COORDINATE_COORDINATE_PRODUCT_IDS] = value; }
		}
		/// <summary>バリエーションID(カンマ区切り)</summary>
		public string VariationIds
		{
			get { return (string)this.DataSource[Constants.FLG_COORDINATE_COORDINATE_VARIATION_IDS]; }
			set { this.DataSource[Constants.FLG_COORDINATE_COORDINATE_VARIATION_IDS] = value; }
		}
		#endregion
	}
}
