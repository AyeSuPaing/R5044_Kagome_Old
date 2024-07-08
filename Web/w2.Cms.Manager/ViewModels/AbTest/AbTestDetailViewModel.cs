/*
=========================================================================================================
  Module      : Abテスト 詳細ビューモデル(AbTestDetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.ViewModels.AbTest
{
	/// <summary>
	/// Abテスト 詳細ビューモデル
	/// </summary>
	[Serializable]
	public class AbTestDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AbTestDetailViewModel()
		{
		}

		/// <summary>ページID</summary>
		public string AbTestId { get; set; }
		/// <summary>タイトル</summary>
		public string AbTestTitle { get; set; }
		/// <summary>公開状態</summary>
		public string PublicStatus { get; set; }
		/// <summary>公開開始日</summary>
		public string RangeStartDate { get; set; }
		/// <summary>公開開始時間</summary>
		public string RangeStartTime { get; set; }
		/// <summary>公開終了日</summary>
		public string RangeEndDate { get; set; }
		/// <summary>公開終了時間</summary>
		public string RangeEndTime { get; set; }
	}
}