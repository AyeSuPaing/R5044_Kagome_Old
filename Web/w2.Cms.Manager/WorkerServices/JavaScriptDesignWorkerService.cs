/*
=========================================================================================================
  Module      : コンテンツマネージャワーカーサービス(JavaScriptDesignWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コンテンツマネージャワーカーサービス
	/// </summary>
	public class JavaScriptDesignWorkerService : ProgramFileDesignSettingWorkerService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public JavaScriptDesignWorkerService() : base(FileTypes.JavaScript)
		{
		}

		/// <summary>選択中のディレクトリ</summary>
		protected override string ClickCurrentPathSession
		{
			get { return this.SessionWrapper.JavascriptDesingMnagerClickCurrent; }
			set { this.SessionWrapper.JavascriptDesingMnagerClickCurrent = value; }
		}
		/// <summary>選択中のパス</summary>
		protected override string SelectPathSession
		{
			get { return this.SessionWrapper.JavascriptDesingMnagerSelectPath; }
			set { this.SessionWrapper.JavascriptDesingMnagerSelectPath = value; }
		}
		/// <summary>現在のフロントアプリケーションのパスルート</summary>
		protected override string PathRootFrontSession
		{
			get { return this.SessionWrapper.JavascriptDesignMnagerPathRootFront; }
			set { this.SessionWrapper.JavascriptDesignMnagerPathRootFront = value; }
		}
	}
}