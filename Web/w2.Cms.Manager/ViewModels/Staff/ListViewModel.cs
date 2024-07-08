/*
=========================================================================================================
  Module      : スタッフListビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.Staff;
using w2.Domain.Staff.Helper;

namespace w2.Cms.Manager.ViewModels.Staff
{
	/// <summary>
	/// スタッフListビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.List = new StaffListSearchResult[0];
			this.ExportFiles = MasterExportHelper.CreateExportFilesDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF);
		}

		/// <summary>パラメタモデル</summary>
		public ListParamModel ParamModel { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>リスト一覧</summary>
		public StaffListSearchResult[] List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>選択肢群 マスタ出力</summary>
		public SelectListItem[] ExportFiles { get; private set; }
	}
}