/*
=========================================================================================================
  Module      : Lpページ商品セットモデル (LandingPageProductSetModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページ商品セットモデル
	/// </summary>
	[Serializable]
	public partial class LandingPageProductSetModel : ModelBase<LandingPageProductSetModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageProductSetModel()
		{
			this.PageId = "";
			this.BranchNo = 0;
			this.SetName = "";
			this.ValidFlg = LandingPageConst.PRODUCT_SET_VALID_FLG_VALID;
			this.LastChanged = "";
			this.Products = new LandingPageProductModel[]{};
			this.SubscriptionBoxCourseId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageProductSetModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageProductSetModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_PAGE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_BRANCH_NO].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_BRANCH_NO] = value; }
		}
		/// <summary>セット名</summary>
		public string SetName
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SET_NAME]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SET_NAME] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_LAST_CHANGED] = value; }
		}
		/// <summary> 頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCTSET_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		#endregion
	}
}
