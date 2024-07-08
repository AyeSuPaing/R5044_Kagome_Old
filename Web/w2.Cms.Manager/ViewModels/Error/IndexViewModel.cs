/*
=========================================================================================================
  Module      : エラーインデックスビューモデル(IndexViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager.ViewModels.Error
{
	/// <summary>
	/// エラーインデックスビューモデル
	/// </summary>
	public class IndexViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IndexViewModel()
		{
			this.ErrorPageLayout = Constants.LAYOUT_PATH_DEFAULT;
		}

		/// <summary>エラー区分</summary>
		public string ErrorKbn { get; set; }
		/// <summary>エラーページタイプ</summary>
		public string ErrorPageType { get; set; }
		/// <summary>エラーメッセージ（HTMLエンコード済）</summary>
		public string ErrorMessagesHtmlEncoded { get; set; }
		/// <summary>ログインボタン表示</summary>
		public bool DispLoginButton
		{
			get { return (this.ErrorPageType == Constants.REQUEST_KBN_ERRORPAGE_TYPE_DISP_GOTOLOGIN); }
		}
		/// <summary>戻るボタン表示</summary>
		public bool DispHistoryBackButton
		{
			get { return (this.DispLoginButton == false); }
		}
		/// <summary>エラーページレイアウト</summary>
		public string ErrorPageLayout { get; set; }
	}
}