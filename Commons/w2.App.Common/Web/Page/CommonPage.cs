/*
=========================================================================================================
  Module      : コモンページ(CommonPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;

namespace w2.App.Common.Web.Page
{
	/// <summary>
	/// コモンページ
	/// </summary>
	public partial class CommonPage : System.Web.UI.Page
	{
		#region CommonPage共通処理
		/// <summary>
		/// タグ置換
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="countyIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
		/// <returns>置換後文字列</returns>
		public static string ReplaceTag(string targetString, string countyIsoCode = "")
		{
			return CommonPageProcess.ReplaceTag(targetString, countyIsoCode);
		}

		/// <summary>
		/// タグ置換
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>置換後文字列</returns>
		public static string ReplaceTagByLocaleId(string targetString, string languageLocaleId)
		{
			return CommonPageProcess.ReplaceTagByLocaleId(targetString, languageLocaleId);
		}

		/// <summary>
		/// ラップ済みコントロール取得
		/// </summary>
		/// <param name="controlId">コントロールID</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>WrappedControl</returns>
		public T GetWrappedControl<T>(string controlId, object defaultValue = null)
			where T : WrappedControl
		{
			return this.Process.GetWrappedControl<T>(controlId, defaultValue);
		}
		/// <summary>
		/// ラップ済みコントロール取得
		/// </summary>
		/// <param name="parentControl">親コントロール</param>
		/// <param name="controlId">コントロールID</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>WrappedControl</returns>
		public T GetWrappedControl<T>(Control parentControl, string controlId, object defaultValue = null)
			where T : WrappedControl
		{
			return this.Process.GetWrappedControl<T>(parentControl, controlId, defaultValue);
		}
		/// <summary>
		/// ラップ済みコントロール取得
		/// </summary>
		/// <param name="controlUniqueId">コントロールユニークID</param>
		/// <param name="constactorParams">コンストラクタのパラメタ</param>
		/// <returns>WrappedControl</returns>
		private T GetWrappedControl<T>(string controlUniqueId, object[] constactorParams)
			where T : WrappedControl
		{
			return this.Process.GetWrappedControl<T>(controlUniqueId, constactorParams);
		}

		/// <summary>
		/// デフォルトマスタ取得
		/// </summary>
		/// <param name="parentMaster">親マスタ</param>
		/// <returns>デフォルトマスタ</returns>
		public MasterPage GetDefaultMaster(MasterPage parentMaster = null)
		{
			return this.Process.GetDefaultMaster(parentMaster);
		}

		/// <summary>
		/// デフォルトマスタのコントロール取得
		/// </summary>
		/// <returns>デフォルトマスタ</returns>
		public Control GetDefaultMasterControl(string controlId)
		{
			return this.Process.GetDefaultMasterControl(controlId);
		}

		/// <summary>
		/// デフォルトマスタのコントロール取得
		/// </summary>
		/// <returns>デフォルトマスタ</returns>
		public Control GetDefaultMasterContentPlaceHolder()
		{
			return this.Process.GetDefaultMasterContentPlaceHolder();
		}
		#endregion

		/// <summary>ページプロセス</summary>
		public CommonPageProcess Process
		{
			get { return (CommonPageProcess)this.ProcessTemp; }
		}
		/// <summary>ページプロセス</summary>
		protected virtual IPageProcess ProcessTemp
		{
			get
			{
				if (m_processTmp == null) m_processTmp = new CommonPageProcess(this, this.ViewState);
				return m_processTmp;
			}
		}
		protected IPageProcess m_processTmp = null;
	}
}
