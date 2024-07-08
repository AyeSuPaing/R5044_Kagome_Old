/*
=========================================================================================================
  Module      : スタッフリストパラメタモデル(ListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.Staff
{
	/// <summary>
	/// スタッフリストパラメタモデル
	/// </summary>
	public class ListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListParamModel()
		{
			this.PagerNo = 1;
			this.HeightLowerLimit = string.Empty;
			this.HeightUpperLimit = string.Empty;
		}

		/// <summary>スタッフID</summary>
		public string StaffId { get; set; }
		/// <summary>スタッフ名</summary>
		public string StaffName { get; set; }
		/// <summary>身長下限</summary>
		public string HeightLowerLimit { get; set; }
		/// <summary>身長上限</summary>
		public string HeightUpperLimit { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
		/// <summary>データ出力タイプ</summary>
		public string DataExportType { get; set; }
	}
}