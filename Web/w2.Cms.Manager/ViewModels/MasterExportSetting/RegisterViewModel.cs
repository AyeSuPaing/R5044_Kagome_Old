/*
=========================================================================================================
  Module      : マスタ出力定義ビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Common.Util;
using w2.Domain.MasterExportSetting;

namespace w2.Cms.Manager.ViewModels.MasterExportSetting
{
	/// <summary>
	/// マスタ出力定義ビューモデル
	/// </summary>
	[Serializable]
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ種別</param>
		/// <param name="actionStatus">アクションステータス</param>
		public RegisterViewModel(string shopId, string masterKbn, ActionStatus? actionStatus)
		{
			this.Input = new MasterExportSettingInput();
			if (actionStatus != null) this.ActionStatus = (ActionStatus)actionStatus;

			InitializeComponents(shopId, masterKbn);
		}

		/// <summary>
		/// 画面制御
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ種別</param>
		public void InitializeComponents(string shopId, string masterKbn)
		{
			Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE =
				// 「（削除不可）」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
					Constants.VALUETEXT_PARAM_UNDELITEABLE);

			MasterKbnInitialization(masterKbn);

			// 出力ファイル形式ドロップダウン
			this.ExportFileType =
				ValueText.GetValueItemArray(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			// 出力設定のドロップダウン
			this.SelectSetting =
				new MasterExportSettingService().GetAllByMaster(shopId, this.MasterKbn.Value)
					.Select(es =>
						new SelectListItem
						{
							Text = (es.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
								? this.MasterKbn.Text + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE
								: es.SettingName,
							Value = es.SettingId,
						})
					.ToList();
		}

		/// <summary>
		/// マスタ種別ラジオボタンリスト
		/// </summary>
		/// <param name="masterKbn"></param>
		public void MasterKbnInitialization(string masterKbn)
		{
			var masterKbnList =
				ValueText.GetValueItemArray(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			this.MasterKbns = new List<SelectListItem>();
			foreach (var item in masterKbnList)
			{
				// ショートURLマスタ
				if ((Constants.SHORTURL_OPTION_ENABLE == false)
					&& (item.Value == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL))
				{
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE;
					continue;
				}
				this.MasterKbns.Add(item);
			}
			this.MasterKbn = this.MasterKbns.FirstOrDefault(m => m.Value == masterKbn);
		}

		/// <summary>入力値</summary>
		public MasterExportSettingInput Input { get; set; }
		/// <summary>マスタ種別データ</summary>
		public List<Hashtable> MasterFieldData { get; set; }
		/// <summary>マスタ種別</summary>
		public SelectListItem MasterKbn { get; set; }
		/// <summary>マスタ種別</summary>
		public List<SelectListItem> MasterKbns { get; set; }
		/// <summary>出力ファイル種類</summary>
		public List<SelectListItem> ExportFileType { get; set; }
		/// <summary>出力ファイル設定</summary>
		public List<SelectListItem> SelectSetting { get; set; }
	}
}