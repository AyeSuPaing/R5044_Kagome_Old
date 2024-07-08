/*
=========================================================================================================
  Module      : Lpページデザインモデル (LandingPageDesignModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページデザインモデル
	/// </summary>
	public partial class LandingPageDesignModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>ブロック</summary>
		public LandingPageDesignBlockModel[] Blocks { get; set; }
		/// <summary>商品セット</summary>
		public LandingPageProductSetModel[] ProductSets { get; set; }
		/// <summary>
		/// 表示できるかどうかをチェック
		/// </summary>
		/// <returns>
		/// TRUE：表示OK
		/// FALSE：表示NG
		/// </returns>
		public bool DisplayCheck()
		{
			if (this.PublicStatus == LandingPageConst.PUBLIC_STATUS_UNPUBLISHED) { return false; }
			if (this.PublicStartDatetime.HasValue && DateTime.Now < this.PublicStartDatetime.Value) { return false; }
			if (this.PublicEndDatetime.HasValue && DateTime.Now > this.PublicEndDatetime.Value) { return false; }
			return true;
		}
		/// <summary>Is cart list landing page</summary>
		public bool IsCartListLp
		{
			get { return (this.PageFileName.ToUpper() == Constants.CART_LIST_LP_PAGE_NAME); }
		}
		/// <summary>公開済みか</summary>
		public bool IsPublished
		{
			get { return (this.PublicStatus == LandingPageConst.PUBLIC_STATUS_PUBLISHED); }
		}
		/// <summary> EFO CUBE利用するか </summary>
		public bool IsEfoEnabled
		{
			get { return this.EfoCubeUseFlg == LandingPageConst.EFO_CUBE_USE_FLG_ON; }
		}
		/// <summary>検索結果の全件数</summary>
		public int RowCount
		{
			get
			{
				if ((this.DataSource.ContainsKey("row_count") == false)
					|| this.DataSource["row_count"] == DBNull.Value) return 0;
				return (int)this.DataSource["row_count"];
			}
			set { this.DataSource["row_count"] = value; }
		}
		#endregion
	}
}
