/*
=========================================================================================================
  Module      : 店舗管理者リストパラメタモデル(ListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.ShopOperator
{
	/// <summary>
	/// 店舗管理者リストパラメタモデル
	/// </summary>
	[Serializable]
	public class ListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListParamModel()
		{
			this.ValidFlg = string.Empty;
			this.ConditionMenuAccessLevel = string.Empty;
			this.SortKbn = "1";
			this.PagerNo = 1;
		}

		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>オペレータ名</summary>
		public string OperatorName { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>メニュー権限</summary>
		public string ConditionMenuAccessLevel { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}