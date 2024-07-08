/*
=========================================================================================================
  Module      : マスタ出力定義ワーカーサービス(MasterExportSettingWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.App.Common.MasterExport;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.MasterExportSetting;
using w2.Common.Util;
using w2.Domain.MasterExportSetting;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// マスタ出力定義ワーカーサービス
	/// </summary>
	public class MasterExportSettingWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="masterKbn">マスタ種別</param>
		/// <param name="tempData">一時データ</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>ビューモデル</returns>
		public RegisterViewModel CreateListVm(string masterKbn, TempDataManager tempData, ActionStatus? actionStatus)
		{
			var vm = new RegisterViewModel(this.SessionWrapper.LoginShopId, masterKbn, actionStatus);

			// マスタ出力定義登録表示
			var masterExportSetting = new MasterExportSettingService().GetAllByMaster(this.SessionWrapper.LoginShopId, masterKbn);

			// マスタ出力設定の値設定
			var selectSettingIndex = 0;
			if ((tempData.MasterExportSettingInput != null)
				&& ((actionStatus == ActionStatus.SelectChange)
					|| (actionStatus == ActionStatus.Update)
					|| (actionStatus == ActionStatus.Delete)))
			{
				selectSettingIndex = tempData.MasterExportSettingInput.SelectSettingIndex;
				vm.Input.SelectSettingIndex = selectSettingIndex;
			}

			// マスタ出力形式の値設定
			vm.Input.ExportFileTypeSelectedValue = masterExportSetting[selectSettingIndex].ExportFileType;
			// 出力フォーマット設定
			var exportParam = masterExportSetting[selectSettingIndex].DataSource;
			vm.Input.Fields = StringUtility.ToEmpty(exportParam[Constants.FIELD_MASTEREXPORTSETTING_FIELDS]).Replace(",", ",\n");
			// マスタフィールド設定
			vm.MasterFieldData = SetListMasterFieldData(masterKbn);
			return vm;
		}

		/// <summary>
		/// マスタ出力定義登録パラメタ取得
		/// </summary>
		/// <param name="request">マスタ出力定義登録のパラメタが格納されたHttpRequest</param>
		/// <returns>パラメタが格納されたHashtable</returns>
		public Hashtable GetParameters(HttpRequestBase request)
		{
			var result = new Hashtable();
			var masterKbn = String.Empty;
			var paramError = false;

			try
			{
				switch (StringUtility.ToEmpty(request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN]))
				{
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL: // ショートURL
						masterKbn = request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE: // コーディネート
						masterKbn = request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM: // コーディネートアイテム
						masterKbn = request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY: // コーディネートカテゴリ
						masterKbn = request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF: // スタッフ
						masterKbn = request[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
						break;

					case "":
						masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL; // ショートURLがデフォルト
						break;

					default:
						paramError = true;
						break;
				}
				result.Add(Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN, masterKbn);
			}
			catch
			{
				paramError = true;
			}

			// パラメタ取得
			result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

			return result;
		}

		/// <summary>
		/// 出力定義設定リストを設定
		/// </summary>
		/// <param name="input">インプット</param>
		/// <param name="tempDate">一時的データ</param>
		/// <returns>インプット</returns>
		public MasterExportSettingInput SetSelectSettingList(MasterExportSettingInput input, TempDataManager tempDate)
		{
			if (tempDate.MasterExportSettingInput == null) tempDate.MasterExportSettingInput = new MasterExportSettingInput();
			tempDate.MasterExportSettingInput.SelectSettingIndex = input.SelectSettingIndex;

			return tempDate.MasterExportSettingInput;
		}

		/// <summary>
		/// マスタフィールドデータをセット
		/// </summary>
		/// <param name="masterKbn"> マスタ種別</param>
		/// <returns>マスタフィールドリスト</returns>
		public List<Hashtable> SetListMasterFieldData(string masterKbn)
		{
			// マスタフィールド取得
			var masterFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(masterKbn);

			// データソース設定
			var list = MasterFieldSetting.RemoveMasterFields(masterFields, masterKbn);

			return list;
		}

		/// <summary>
		/// 出力設定情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="input">インプット</param>
		/// <returns>入力設定情報</returns>
		private Hashtable GetExportSettingInfo(string shopId, MasterExportSettingInput input)
		{
			var inputParam = new Hashtable()
			{
				{Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, shopId},
				{Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, input.MasterKbn.Value},
				{Constants.FIELD_MASTEREXPORTSETTING_FIELDS, MasterExportSettingUtility.GetFieldsEscape(input.Fields)},
				{Constants.FIELD_MASTEREXPORTSETTING_LAST_CHANGED, this.SessionWrapper.LoginOperatorName},
				{Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE, input.ExportFileTypeSelectedValue}
			};

			return inputParam;
		}

		/// <summary>
		/// 出力設定登録・更新
		/// </summary>
		/// <param name="input">インプット</param>
		/// <returns>エラーメッセージ</returns>
		public void InsertUpdateExportSetting(MasterExportSettingInput input)
		{
			var shopId = this.SessionWrapper.LoginShopId;

			// パラメタ格納
			var inputParam = GetExportSettingInfo(shopId, input);
			if (input.IsInsert)
			{
				inputParam.Add(Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME, input.SettingName.Trim());
				MasterExportSettingUtility.InsertMasterExportSetting(inputParam);
			}
			else
			{
				inputParam.Add(Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID, input.SelectSettingValue);
				MasterExportSettingUtility.UpdateMasterExportSetting(inputParam);
			}
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="input">入力</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(MasterExportSettingInput input)
		{
			var shopId = this.SessionWrapper.LoginShopId;

			// パラメタ格納
			var inputParam = GetExportSettingInfo(shopId, input);
			if (input.IsInsert)
			{
				inputParam.Add(Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME, input.SettingName.Trim());
			}
			else
			{
				inputParam.Add(Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID, input.SelectSettingValue);
			}

			// 入力チェック
			var errorMessages = App.Common.Util.Validator.Validate(input.IsInsert ? "MasterExportSettingRegist" : "MasterExportSettingModify", inputParam);
			if (errorMessages != "")
			{
				// エラーページへ
				return errorMessages;
			}

			// フィールド列チェック
			if (CheckMasterFields(shopId, (string)inputParam[Constants.FIELD_MASTEREXPORTSETTING_FIELDS], (string)inputParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]) == false)
			{
				var errorInfo = WebMessages.MasterExportSettingIrregularFieldsError;
				if (this.ErrorFieldName.Length != 0)
				{
					errorInfo += "（該当なし：「" + this.ErrorFieldName.ToString() + "」）";
				}

				// エラーページへ
				return errorInfo;
			}

			return string.Empty;
		}

		/// <summary>
		/// 出力設定削除
		/// </summary>
		/// <param name="input">マスタ種別入力クラス</param>
		public void Delete(MasterExportSettingInput input)
		{
			var inputParam = new Hashtable()
			{
				{Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, this.SessionWrapper.LoginShopId},
				{Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, input.MasterKbn.Value},
				{Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID, input.SelectSettingValue}
			};

			MasterExportSettingUtility.DeleteMasterExportSetting(inputParam);
		}


		/// <summary>
		/// フィールドチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="namesCsv">名前列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>フィールドチェック結果</returns>
		private bool CheckMasterFields(string shopId, string namesCsv, string masterKbn)
		{
			// フィールド名変換 ※名称からフィールド名を取得できるようHashtable作成
			var fields = MasterExportSettingUtility.GetMasterExportSettingFields(masterKbn);

			// リアルフィールド作成
			var names = StringUtility.SplitCsvLine(namesCsv);
			var missedField = names.FirstOrDefault(name => (fields.ContainsKey(name) == false));
			if (missedField != null)
			{
				if (this.ErrorFieldName.Length > 0) this.ErrorFieldName.Append(", ");
				this.ErrorFieldName.Append(missedField);
				return false;
			}

			// フィールドチェック
			var sqlFieldNames = string.Join(",", names.Select(name => (string)fields[name]));
			var result = MasterExportSettingUtility.CheckFieldsExists(shopId, masterKbn, sqlFieldNames);
			return result;
		}

		/// <summary>エラーがあったフィールド</summary>
		public StringBuilder ErrorFieldName { get; set; }
	}
}