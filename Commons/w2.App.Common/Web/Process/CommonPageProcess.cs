/*
=========================================================================================================
  Module      : コモンページプロセス(CommonPageProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Web.WrappedContols;

namespace w2.App.Common.Web.Process
{
	/// <summary>
	/// コモンページプロセス
	/// </summary>
	public partial class CommonPageProcess : IPageProcess
	{
		/// <summary>呼び出し元タイプ</summary>
		public enum CallerTypes
		{
			/// <summary>ページ</summary>
			Page,
			/// <summary>ユーザーコントロール</summary>
			UserControl,
			/// <summary>マスターページ</summary>
			Master,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="caller">呼び出し元オブジェクト</param>
		/// <param name="viewState">ViewState</param>
		public CommonPageProcess(object caller, StateBag viewState)
		{
			this.Caller = caller;
			if (caller is System.Web.UI.Page)
			{
				this.Page = (System.Web.UI.Page)caller;
				this.CallerType = CallerTypes.Page;
			}
			else if (caller is MasterPage)
			{
				this.Page = ((MasterPage)caller).Page;
				this.CallerType = CallerTypes.Master;
			}
			else if (caller is UserControl)
			{
				this.Page = ((UserControl)caller).Page;
				this.CallerType = CallerTypes.UserControl;
			}
			this.ViewState = viewState;
		}

		/// <summary>
		/// タグ置換
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="countyIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
		/// <returns>置換後文字列</returns>
		public static string ReplaceTag(string targetString, string countyIsoCode = "")
		{
			var result = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				targetString,
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.CONFIGURATION_SETTING.ReadKbnList.Contains(
						ConfigurationSetting.ReadKbn.C200_CommonFront)
						? RegionManager.GetInstance().Region.LanguageLocaleId
						: Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: "",
				Constants.GLOBAL_OPTION_ENABLE ? countyIsoCode : "");
			return result;
		}

		/// <summary>
		/// タグ置換
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>置換後文字列</returns>
		public static string ReplaceTagByLocaleId(string targetString, string languageLocaleId)
		{
			var result = Constants.GLOBAL_OPTION_ENABLE
				? Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(targetString, languageLocaleId)
				: Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(targetString);
			return result;
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
			return GetWrappedControl<T>(null, controlId, defaultValue);
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
			string parentControlUniqueId = null;
			if (this.CallerType == CallerTypes.Page)
			{
				var targetMaster = GetDefaultMaster();
				parentControlUniqueId = (parentControl != null)
					? parentControl.UniqueID
					: ((targetMaster != null)
						? (GetDefaultMasterContentPlaceHolder() ?? GetDefaultMasterControl("ContentPlaceHolderBody"))
						: this.Page).UniqueID;
			}
			else if (this.CallerType == CallerTypes.UserControl)
			{
				parentControlUniqueId = (parentControl != null)
					? parentControl.UniqueID
					: ((UserControl)this.Caller).UniqueID;
			}
			else
			{
				throw new Exception("未対応：" + this.CallerType);
			}

			var control = GetWrappedControl<T>(
				parentControlUniqueId + "->" + controlId,
				(defaultValue != null)
					? new object[] { parentControlUniqueId, controlId, this.Form, this.ViewState, defaultValue }
					: new object[] { parentControlUniqueId, controlId, this.Form, this.ViewState });
			return control;
		}
		/// <summary>
		/// ラップ済みコントロール取得
		/// </summary>
		/// <param name="controlUniqueId">コントロールユニークID</param>
		/// <param name="constactorParams">コンストラクタのパラメタ</param>
		/// <returns>WrappedControl</returns>
		public T GetWrappedControl<T>(string controlUniqueId, object[] constactorParams)
			where T : WrappedControl
		{
			// コンストラクタを呼び出してラップ済みWEBコントロール作成
			if (m_dicControls.ContainsKey(controlUniqueId) == false)
			{
				m_dicControls.Add(
					controlUniqueId,
					(WrappedControl)typeof(T).InvokeMember(
						null,
						System.Reflection.BindingFlags.CreateInstance,
						null,
						null,
						constactorParams));
			}
			return (T)m_dicControls[controlUniqueId];
		}

		/// <summary>
		/// デフォルトマスタ取得
		/// </summary>
		/// <param name="parentMaster">親マスタ</param>
		/// <returns>デフォルトマスタ</returns>
		public MasterPage GetDefaultMaster(MasterPage parentMaster = null)
		{
			var targetMaster = parentMaster ?? this.Master;
			if ((targetMaster == null) || (targetMaster.Master == null)) return targetMaster;

			return GetDefaultMaster(targetMaster.Master);
		}

		/// <summary>
		/// デフォルトマスタのコントロール取得
		/// </summary>
		/// <returns>デフォルトマスタ</returns>
		public Control GetDefaultMasterControl(string controlId)
		{
			var parentControl = GetDefaultMaster().FindControl(controlId);
			if (parentControl == null) return null;

			// 最大探索階層
			const int MAX_SEARCH_COUNT = 5;
			// 最下層のコンテンツプレースホルダーを取得
			Control control = null;
			for (var i = 0; i < MAX_SEARCH_COUNT; i++)
			{
				var childControl = parentControl.FindControl(controlId);
				if (childControl == null)
				{
					control = parentControl;
					break;
				}

				parentControl = childControl;
			}
			return control;
		}

		/// <summary>
		/// デフォルトマスタのコントロール取得
		/// </summary>
		/// <returns>デフォルトマスタ</returns>
		public Control GetDefaultMasterContentPlaceHolder()
		{
			return GetDefaultMasterControl("ContentPlaceHolder1");
		}

		/// <summary>HTMLフォーム</summary>
		public HtmlForm Form { get { return this.Page.Form; } }
		/// <summary>マスタページ</summary>
		public MasterPage Master { get { return this.Page.Master; } }
		/// <summary>ページ</summary>
		public System.Web.UI.Page Page { get; private set; }
		/// <summary>呼び出し元</summary>
		public object Caller { get; private set; }
		/// <summary>呼び出し元タイプ</summary>
		public CallerTypes CallerType { get; private set; }
		/// <summary>ViewState</summary>
		public StateBag ViewState { get; private set; }
		/// <summary>コントロールキャッシュ</summary>
		private readonly Dictionary<string, WrappedControl> m_dicControls = new Dictionary<string, WrappedControl>();
	}
}
