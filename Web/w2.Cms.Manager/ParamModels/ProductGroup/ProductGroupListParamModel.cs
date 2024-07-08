/*
=========================================================================================================
  Module      : 商品グループ一覧リストパラメタモデル(ProductGroupListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Cms;

namespace w2.Cms.Manager.ParamModels.ProductGroup
{
	/// <summary>
	/// 商品グループ一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class ProductGroupListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductGroupListParamModel()
		{
			this.PagerNo = 1;
			this.ProductGroupId = "";
			this.ProductGroupName = "";
			this.ProductId = "";
			this.Dates = new SearchParamDateModel();
			this.ValidFlg = "";
		}

		/// <summary>商品グループID</summary>
		public string ProductGroupId { get; set; }
		/// <summary>商品グループ名</summary>
		public string ProductGroupName { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>検索パラメータ日付</summary>
		public SearchParamDateModel Dates { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}