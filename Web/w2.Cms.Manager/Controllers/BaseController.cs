/*
=========================================================================================================
  Module      : コントローラ基底クラス(BaseController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.ProductGroup;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コントローラ基底クラス
	/// </summary>
	[LoggedInCheckFilter]
	public abstract class BaseController : CommonController
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sessionWrapper">セッションラッパー</param>
		protected BaseController(SessionWrapper sessionWrapper = null)
		{
			this.SessionWrapper = sessionWrapper ?? new SessionWrapper();
			OnActionExecuting(null);
		}

		/// <summary>
		/// エラーページ遷移アクション
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="isDefaultLayout">デフォルトレイアウトか（falseの場合ポップアップレイアウト）</param>
		/// <param name="errorPageType">エラーページタイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult CreateErrorPageAction(string errorMessage, bool isDefaultLayout = true, string errorPageType = null)
		{
			var layout = isDefaultLayout ? Constants.LAYOUT_PATH_DEFAULT : Constants.POPUP_LAYOUT_PATH_DEFAULT;
			var url = this.Url.Action(
				"",
				Constants.CONTROLLER_W2CMS_MANAGER_ERROR,
				new
				{
					ErrorPageLayout = layout,
					ErrorPageType = errorPageType ?? "",
				});

			this.TempData.ErrorMessage = errorMessage;
			if (this.Request.IsAjaxRequest())
			{
				return JavaScript(string.Format("window.location = '{0}'", url));
			}

			return Redirect(url);
		}

		/// <summary>
		/// セッションラッパー更新（ワーカーサービスのセッションラッパーまで更新）
		/// </summary>
		/// <param name="sessionWrapper">セッションラッパー</param>
		public override void UpdateSessionWrapper(SessionWrapper sessionWrapper)
		{
			this.SessionWrapper = this.DefaultService.SessionWrapper = sessionWrapper;
		}

		/// <summary>
		/// 商品入力ビュー
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品ビューモデル</param>
		/// <returns>アクション結果</returns>
		public override ActionResult ProductInputView(string baseName, string modelNo, ProductViewModel[] list)
		{
			return null;
		}

		/// <summary>
		/// 商品入力ビュー
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品ビューモデル</param>
		/// <param name="group">商品グループモデル</param>
		/// <returns>アクション結果</returns>
		public override ActionResult ProductInputView(string baseName, string modelNo, ProductViewModel[] list, ProductGroupModel group)
		{
			return null;
		}

		/// <summary>
		/// デフォルトワーカーサービス取得
		/// </summary>
		/// <typeparam name="T">ワーカーサービスクラス</typeparam>
		/// <returns>デフォルトワーカーサービス</returns>
		protected T GetDefaultService<T>()
			where T : BaseWorkerService, new()
		{
			if (this.DefaultService == null) this.DefaultService = new T();
			return (T)this.DefaultService;
		}

		/// <summary>ワーカーサービス</summary>
		public BaseWorkerService DefaultService { get; set; }
		/// <summary>TempDataをラッパーで上書き</summary>
		public new TempDataManager TempData
		{
			get { return m_tempData ?? (m_tempData = new TempDataManager(this.SessionWrapper.Session)); }
		}
		private TempDataManager m_tempData = null;
	}
}
