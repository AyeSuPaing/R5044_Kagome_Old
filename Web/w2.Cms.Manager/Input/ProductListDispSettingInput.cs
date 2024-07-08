/*
=========================================================================================================
  Module      : 商品一覧表示設定Inputクラス(ProductListDispSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Database.Common;
using w2.Domain.NameTranslationSetting;
using w2.Domain.ProductListDispSetting;
using Validator = w2.Cms.Manager.Codes.Common.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 商品一覧表示設定Inputクラス
	/// </summary>
	public class ProductListDispSettingInput
	{
		/// <summary>件数表示個数</summary>
		public const int COUNT_LENGTH = 5;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductListDispSettingInput()
		{

		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public ProductListDispSettingModel[] CreateModel()
		{
			return this.AllSettings
				.Where(setting => (setting.IsCountSetting == false) || (string.IsNullOrEmpty(setting.DispCount) == false))
				.Select(setting => setting.CreateModel())
				.ToArray();
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			this.CountSettings = this.CountSettings.Where(
				setting => setting.IsDefaultDisp
					|| setting.IsDispEnable
					|| (string.IsNullOrEmpty(setting.DispCount) == false)).ToArray();

			// 表示時順序半角数字チェック
			var errorMessages = string.Join(
				"",
				this.AllSettings.Select(
					setting => setting.Validate().Replace(
						"@@ 1 @@",
						ValueText.GetValueText(
							Constants.TABLE_PRODUCTLISTDISPSETTING,
							Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN,
							setting.SettingKbn))));

			// 表示件数重複チェック
			if (DispCountRepeatCheck(this.CountSettings) == false)
			{
				errorMessages += WebMessages.ProductListDispSettingDispCountRepeatError;
			}

			//表示項目があるかチェック
			errorMessages += CheckSelectedIsDispEnable(this.SortSettings);
			errorMessages += CheckSelectedIsDispEnable(this.StockSettings);
			errorMessages += CheckSelectedIsDispEnable(this.ImgSettings);
			errorMessages += CheckSelectedIsDispEnable(this.CountSettings);

			// デフォルト設定が選択済みかチェック
			errorMessages += CheckSelectedDefaultDisp(this.SortSettings);
			errorMessages += CheckSelectedDefaultDisp(this.StockSettings);
			errorMessages += CheckSelectedDefaultDisp(this.ImgSettings);
			errorMessages += CheckSelectedDefaultDisp(this.CountSettings);

			return errorMessages;
		}

		/// <summary>
		/// 表示件数重複チェック
		/// </summary>
		/// <param name="productListDispSettingElementArray">商品一覧表示設定要素リスト</param>
		/// <returns></returns>
		private static bool DispCountRepeatCheck(ProductListDispSettingElement[] productListDispSettingElementArray)
		{
			var set = new HashSet<string>();
			foreach (var productListDispSettingElement in productListDispSettingElementArray)
			{
				set.Add(productListDispSettingElement.DispCount);
			}

			return (set.Count == productListDispSettingElementArray.Length);
		}

		/// <summary>
		/// 表示項目があるかチェックする
		/// </summary>
		/// <param name="targets">チェック対象</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckSelectedIsDispEnable(ProductListDispSettingElement[] targets)
		{
			if(targets.Any(setting => setting.IsDispEnable)) return string.Empty;

			var errorMsg = WebMessages.ProductListDispSettingDisplayNoSettingError.Replace(
							"@@ 1 @@",
							ValueText.GetValueText(
								Constants.TABLE_PRODUCTLISTDISPSETTING,
								Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN,
								targets.First().SettingKbn));

			return errorMsg;
		}

		/// <summary>
		/// 表示項目がある場合にデフォルトが設定されているかチェックする
		/// </summary>
		/// <param name="targets">チェック対象</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckSelectedDefaultDisp(ProductListDispSettingElement[] targets)
		{
			if (targets.Length == 0) return string.Empty;

			var errorMsg =
				targets.Any(setting => setting.IsDispEnable)
				&& (targets.Any(setting => setting.IsDispEnable && setting.IsDefaultDisp) == false)
					? WebMessages.ProductListDispSettingInconsistentError.Replace(
						"@@ 1 @@",
						ValueText.GetValueText(
							Constants.TABLE_PRODUCTLISTDISPSETTING,
							Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN,
							targets.First().SettingKbn))
					: string.Empty;
			return errorMsg;
		}
		#endregion

		#region プロパティ
		/// <summary>ソート形式</summary>
		public ProductListDispSettingElement[] SortSettings { get; set; }
		/// <summary>数量形式</summary>
		public ProductListDispSettingElement[] CountSettings { get; set; }
		/// <summary>表示形式</summary>
		public ProductListDispSettingElement[] ImgSettings { get; set; }
		/// <summary>在庫区分</summary>
		public ProductListDispSettingElement[] StockSettings { get; set; }
		/// <summary>データ全体</summary>
		public ProductListDispSettingElement[] AllSettings
		{
			get
			{
				return this.SortSettings
					.Concat(this.ImgSettings)
					.Concat(this.CountSettings)
					.Concat(this.StockSettings)
					.ToArray();
			}
		}
		/// <summary>ソート形式：デフォルト表示するID</summary>
		public string SortDefaultDisp { get; set; }
		/// <summary>件数：デフォルト表示表示するIndex</summary>
		public string CountDefaultDispIndex { get; set; }
		/// <summary>形式：デフォルト表示表示するID</summary>
		public string ImgDefaultDisp { get; set; }
		/// <summary>在庫：デフォルト表示表示するID</summary>
		public string StockDefaultDisp { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		#endregion
	}

	/// <summary>
	/// 商品一覧表示設定要素クラス
	/// </summary>
	public class ProductListDispSettingElement: InputBase<ProductListDispSettingModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductListDispSettingElement()
		{
			this.SettingId = string.Empty;
			this.IsDefaultDisp = false;
			this.IsDispEnable= false;
		}

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override ProductListDispSettingModel CreateModel()
		{
			return new ProductListDispSettingModel
			{
				SettingId = this.IsCountSetting ? this.DispCount:  this.SettingId,
				SettingKbn = this.SettingKbn,
				SettingName = this.SettingName ?? string.Empty,
				DispNo = int.Parse(this.IsCountSetting ? this.DispCount : this.DispNo),
				DispCount = string.IsNullOrEmpty(this.DispCount) ? null : (int?)int.Parse(this.DispCount),
				DateChanged = DateTime.Now,
				LastChanged = this.LastChanged,
				DefaultDispFlg =
					this.IsDefaultDisp
						? Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_ON
						: Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_OFF,
				DispEnable =
					this.IsDispEnable
						? Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON
						: Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_OFF
			};
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errorMessage = Validator.Validate("ProductListDispSetting", this.DataSource);
			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>設定ID</summary>
		public string SettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID] = value; }
		}
		/// <summary>表示名</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME] = value; }
		}
		/// <summary>表示／非表示</summary>
		public string DispEnable
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE] = value; }
		}
		/// <summary>表示順</summary>
		public string DispNo
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO] = value; }
		}
		/// <summary>デフォルト表示フラグ</summary>
		public string DefaultDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG] = value; }
		}
		/// <summary>説明</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>設定区分</summary>
		public string SettingKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN] = value; }
		}
		/// <summary>表示件数</summary>
		public string DispCount
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT] = value; }
		}
		/// <summary>設定名(翻訳前)</summary>
		public string SettingNameBeforeTranslation
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME + "_before_translation"]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME + "_before_translation"] = value; }
		}
		/// <summary>設定名翻訳設定情報</summary>
		public NameTranslationSettingModel[] SettingNameTranslationData
		{
			get { return (NameTranslationSettingModel[])this.DataSource["settingname_translation_data"]; }
			set { this.DataSource["settingname_translation_data"] = value; }
		}
		/// <summary>デフォルト表示か</summary>
		public bool IsDefaultDisp { get; set; }
		/// <summary>表示可能か</summary>
		public bool IsDispEnable{ get; set; }
		/// <summary>表示順</summary>
		public string DispNoText { get; set; }
		/// <summary>件数表示用か</summary>
		public bool IsCountSetting
		{
			get { return (this.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT); }
		}
		#endregion
	}
}
