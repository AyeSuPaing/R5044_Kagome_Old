/*
=========================================================================================================
  Module      : 特集エリアバナーモデル (FeatureAreaBannerModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアバナーモデル
	/// </summary>
	public partial class FeatureAreaBannerModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>ファイルパス(拡張子含む)</summary>
		public string FilePath
		{
			get { return this.FileDirPath + this.FileName; }
		}
		/// <summary>別ウィンドウか</summary>
		public bool IsAnotherWindow
		{
			get { return this.WindowType == Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_POPUP; }
		}
		/// <summary>表示か</summary>
		public bool IsPublic
		{
			get { return (this.Publish == Constants.FLG_FEATUREAREABANNER_PUBLISH_PUBLIC); }
		}
		/// <summary>プレビュー時バイナリ</summary>
		public string PreviewBinary
		{
			get { return (string)this.DataSource["PreviewBinary"]; }
			set { this.DataSource["PreviewBinary"] = value; }
		}
		#endregion
	}
}
