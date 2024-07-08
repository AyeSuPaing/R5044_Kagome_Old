/*
=========================================================================================================
  Module      : 商品一覧表示設定ワーカーサービス(ProductListDispSettingWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.ProductListDispSetting;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.ProductListDispSetting;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 商品一覧表示設定ワーカーサービス
	/// </summary>
	public class ProductListDispSettingWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 編集ビューモデル作成
		/// </summary>
		/// <param name="success">成功フラグ</param>
		/// <returns>ビューモデル</returns>
		internal ModifyViewModel CreateModifyVm(bool success = false)
		{
			var elements = new ProductListDispSettingService().GetAll().Select(
				setting => new ProductListDispSettingElement
				{
					SettingId = setting.SettingId,
					SettingKbn = setting.SettingKbn,
					DefaultDispFlg = setting.DefaultDispFlg,
					Description = setting.Description,
					DispCount = setting.DispCount.ToString(),
					DispEnable = setting.DispEnable,
					DispNo = setting.DispNo.ToString(),
					SettingName = setting.SettingName,
					IsDispEnable = (setting.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON),
					IsDefaultDisp = (setting.DefaultDispFlg == Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_ON),
					LastChanged = this.SessionWrapper.LoginOperatorName
				}).ToArray();
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				elements.ToList().ForEach(
					element => element.SettingNameTranslationData = GetProductListDispSettingTranslationData()
						.Where(d => (d.MasterId1 == element.SettingId)
						&& (d.MasterId2 == element.SettingKbn)).ToArray());
			}
			var input = new ProductListDispSettingInput()
			{
				SortSettings = GetProductListDispSettingDescription(
					elements.Where(m => (m.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT))
						.ToArray(),
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT),
				ImgSettings = GetProductListDispSettingDescription(
					elements.Where(m => (m.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG))
						.ToArray(),
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG),
				StockSettings = GetProductListDispSettingDescription(
					elements.Where(m => (m.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK))
						.ToArray(),
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK),
				CountSettings = elements.Where(m => (m.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT))
					.ToArray(),
			};
			input.CountSettings = input.CountSettings.Concat(
				Enumerable.Range(0, ProductListDispSettingInput.COUNT_LENGTH - input.CountSettings.Length).Select(
					data => new ProductListDispSettingElement()
					{
						SettingKbn = Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT,
						LastChanged = this.SessionWrapper.LoginOperatorName
					}))
				.ToArray();
			var model = new ModifyViewModel
			{
				Input = input,
				UpdateSuccessFlg = success
			};
			return model;
		}

		/// <summary>
		/// 商品一覧表示設定更新
		/// </summary>
		/// <param name="input">商品一覧表示設定入力</param>
		/// <returns>エラーがあればエラーメッセージ</returns>
		internal string Update(ProductListDispSettingInput input)
		{
			input.SortSettings.ToList()
				.ForEach(setting => setting.IsDefaultDisp = (setting.SettingId == input.SortDefaultDisp));
			var index = 0;
			if (int.TryParse(input.CountDefaultDispIndex, out index))
			{
				input.CountSettings[index].IsDefaultDisp = true;
			}
			input.StockSettings.ToList()
				.ForEach(setting => setting.IsDefaultDisp = (setting.SettingId == input.StockDefaultDisp));
			input.ImgSettings.ToList()
				.ForEach(setting => setting.IsDefaultDisp = (setting.SettingId == input.ImgDefaultDisp));

			var errorMessage = input.Validate();
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var service = new ProductListDispSettingService();
			var models = input.CreateModel();
			var count = models.Select(setting => service.Update(setting)).Sum();
			count += service.InsertCountSetting(models.Where(model => model.IsCountSetting).ToArray());
			return string.Empty;
		}

		/// <summary>
		/// 商品一覧表示設定翻訳情報取得
		/// </summary>
		/// <returns>商品一覧表示設定翻訳情報</returns>
		private NameTranslationSettingModel[] GetProductListDispSettingTranslationData()
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING,
			};
			var translationData = new NameTranslationSettingService().GetTranslationSettingsByDataKbn(searchCondition);
			return translationData;
		}

		/// <summary>
		/// Get Product List Disp Setting Description
		/// </summary>
		/// <param name="elements">Elements</param>
		/// <param name="settingKbn">Setting Kbn</param>
		/// <returns>Product List Disp Setting Description</returns>
		private ProductListDispSettingElement[] GetProductListDispSettingDescription(
			ProductListDispSettingElement[] elements,
			string settingKbn)
		{
			string[] descriptions;
			switch (settingKbn)
			{
				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT:
					descriptions = WebMessages.ProductListDispSettingSortDescriptions;
					break;

				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG:
					descriptions = WebMessages.ProductListDispSettingImgDescriptions;
					break;

				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK:
					descriptions = WebMessages.ProductListDispSettingStockDescriptions;
					break;

				default:
					return elements;
			}

			var index = 0;
			foreach (var item in descriptions)
			{
				if (index >= descriptions.Length) break;

				elements[index].Description = item;
				index++;
			}
			return elements;
		}
	}
}