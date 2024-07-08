/*
=========================================================================================================
  Module      : コンテンツマネージャワーカーサービス(CSSDesignWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コンテンツマネージャワーカーサービス
	/// </summary>
	public class CSSDesignWorkerService : ProgramFileDesignSettingWorkerService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CSSDesignWorkerService() : base(FileTypes.Css)
		{
		}

		/// <summary>選択中のディレクトリ</summary>
		protected override string ClickCurrentPathSession
		{
			get { return this.SessionWrapper.CssDesignMnagerClickCurrent; }
			set { this.SessionWrapper.CssDesignMnagerClickCurrent = value; }
		}
		/// <summary>選択中のパス</summary>
		protected override string SelectPathSession
		{
			get { return this.SessionWrapper.CssDesignMnagerSelectPath; }
			set { this.SessionWrapper.CssDesignMnagerSelectPath = value; }
		}
		/// <summary>現在のフロントアプリケーションのパスルート</summary>
		protected override string PathRootFrontSession
		{
			get { return this.SessionWrapper.CssDesignMnagerPathRootFront; }
			set { this.SessionWrapper.CssDesignMnagerPathRootFront = value; }
		}
	}
}