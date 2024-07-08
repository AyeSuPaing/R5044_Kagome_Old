/*
=========================================================================================================
  Module      : Lpページ商品セット入力クラス (LandingPageProductSetInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.LandingPage;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// Lpページ商品セット入力クラス
	/// </summary>
	public class LandingPageProductSetInput : InputBase<LandingPageProductSetModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageProductSetInput()
		{
			this.BranchNo = "0";
			this.SubscriptionBoxCourseFlg = false;
			this.SubscriptionBoxCourseId = string.Empty;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public LandingPageProductSetInput(LandingPageProductSetModel model) : this()
		{
			this.PageId = model.PageId;
			this.BranchNo = model.BranchNo.ToString();
			this.SetName = model.SetName;
			this.ValidFlg = (model.ValidFlg == LandingPageConst.PRODUCT_SET_VALID_FLG_VALID);
			this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
			this.SubscriptionBoxCourseFlg = (string.IsNullOrEmpty(model.SubscriptionBoxCourseId) == false);
			this.Products = new LandingPageProductInput[] { };
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override LandingPageProductSetModel CreateModel()
		{
			var model = new LandingPageProductSetModel
			{
				PageId = this.PageId,
				BranchNo = int.Parse(this.BranchNo),
				SetName = this.SetName,
				ValidFlg = (this.ValidFlg)
					? LandingPageConst.PRODUCT_SET_VALID_FLG_VALID
					: LandingPageConst.PRODUCT_SET_VALID_FLG_INVALID,
				SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
				LastChanged = "",
				DateChanged = DateTime.Now,
				DateCreated = DateTime.Now,
			};

			model.Products = (this.Products != null)
				? this.Products.Select(product =>
					{
						var pm = product.CreateModel();
						pm.PageId = model.PageId;
						pm.BranchNo = model.BranchNo;
						return pm;
					}).ToArray()
				: new LandingPageProductModel[] { };

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public List<string> Validate(bool register)
		{
			// 入力チェック
			var errorMessageList = Validator
				.Validate(register ? "LandingPageProductSetRegister" : "LandingPageProductSetModify", this.DataSource)
				.Select(kv => kv.Value.Replace("@@ 1 @@", this.BranchNo)).ToList();

			return errorMessageList;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>枝番</summary>
		public string BranchNo { get; set; }
		/// <summary>セット名</summary>
		public string SetName
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SET_NAME]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SET_NAME] = value; }
		}
		/// <summary>有効フラグ</summary>
		public bool ValidFlg { get; set; }
		/// <summary>頒布会フラグ</summary>
		public bool SubscriptionBoxCourseFlg { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>商品入力値</summary>
		public LandingPageProductInput[] Products { get; set; }
		#endregion
	}
}