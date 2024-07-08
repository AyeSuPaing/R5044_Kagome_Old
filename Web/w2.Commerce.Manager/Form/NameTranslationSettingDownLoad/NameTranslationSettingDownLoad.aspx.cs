/*
=========================================================================================================
  Module      : 名称翻訳設定ダウンロードページ処理(NameTranslationSettingDownLoad.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Translation;
using w2.Domain.NameTranslationSetting;

public partial class Form_NameTranslationSettingDownLoad_NameTranslationSettingDownLoad : System.Web.UI.Page
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 対象データ区分チェックボックス作成
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING, Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN))
			{
				if (string.IsNullOrEmpty(li.Value)) continue;

				cblDataKbn.Items.Add(li);
			}
		}
	}
	#endregion

	#region #btnDownLoad_Click ダウンロードボタンクリック
	/// <summary>
	/// ダウンロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDownLoad_Click(object sender, EventArgs e)
	{
		// 選択された区分に応じたデータを取得
		var exportTargetData = GetExportTargetData();

		// エクスポート処理
		CsvExport(exportTargetData);
	}
	#endregion

	#region -GetExportTargetData エクスポート対象データ取得
	/// <summary>
	/// エクスポート対象データ取得
	/// </summary>
	/// <returns></returns>
	private List<NameTranslationSettingModel> GetExportTargetData()
	{
		var exportTargetData = new List<NameTranslationSettingModel>();
		var service = new NameTranslationSettingService();

		foreach (ListItem item in cblDataKbn.Items)
		{
			if (item.Selected == false) continue;

			switch (item.Value)
			{
				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT:
					var product = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT);
					if (product != null) exportTargetData.AddRange(product);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION:
					var productVariation = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION);
					if (productVariation != null) exportTargetData.AddRange(productVariation);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY:
					var productCategory = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY);
					if (productCategory != null) exportTargetData.AddRange(productCategory);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION:
					var setPromotion = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION);
					if (setPromotion != null) exportTargetData.AddRange(setPromotion);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET:
					var productSet = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET);
					if (productSet != null) exportTargetData.AddRange(productSet);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON:
					var coupon = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON);
					if (coupon != null) exportTargetData.AddRange(coupon);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK:
					var memberRank = service.GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK);
					if (memberRank != null) exportTargetData.AddRange(memberRank);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT:
					var payment = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT);
					if (payment != null) exportTargetData.AddRange(payment);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING:
					var userExtendSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING);
					if (userExtendSetting != null) exportTargetData.AddRange(userExtendSetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND:
					var productBrand = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND);
					if (productBrand != null) exportTargetData.AddRange(productBrand);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY:
					var novelty = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY);
					if (novelty != null) exportTargetData.AddRange(novelty);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS:
					var news = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS);
					if (news != null) exportTargetData.AddRange(news);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING:
					var orderMemoSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING);
					if (orderMemoSetting != null) exportTargetData.AddRange(orderMemoSetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON:
					var fixedPurchaseCancelReason = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON);
					if (fixedPurchaseCancelReason != null) exportTargetData.AddRange(fixedPurchaseCancelReason);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING:
					var productDispSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING);
					if (productDispSetting != null) exportTargetData.AddRange(productDispSetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION:
					var shopMessage = GetSiteInformationTranslationData();
					exportTargetData.AddRange(shopMessage);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE:
					var coordinateSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE);
					if (coordinateSetting != null) exportTargetData.AddRange(coordinateSetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY:
					var categorySetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY);
					if (categorySetting != null) exportTargetData.AddRange(categorySetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF:
					var staffSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF);
					if (staffSetting != null) exportTargetData.AddRange(staffSetting);
					break;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP:
					var realShopSetting = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP);
					if (realShopSetting != null) exportTargetData.AddRange(realShopSetting);
					break;
			}
		}
		return exportTargetData;
	}
	#endregion

	#region -CsvExport CSVエクスポート
	/// <summary>
	/// CSVエクスポート
	/// </summary>
	/// <param name="exportTargetData">エクスポート対象データ</param>
	private void CsvExport(List<NameTranslationSettingModel> exportTargetData)
	{
		var csvFormatters = (exportTargetData != null)
			? exportTargetData.Select(s => new NameTranslationCsvOutputFormatter(s)).ToArray()
			: new NameTranslationCsvOutputFormatter[0];

		Response.AppendHeader("Content-Disposition", "attachment; filename="
			+ ValueText.GetValueText(Constants.TABLE_NAMETRANSLATIONSETTING, "export_file_name", string.Empty)
			+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
		Response.Write((csvFormatters.FirstOrDefault() ?? new NameTranslationCsvOutputFormatter()).OutputCsvHeader() + Environment.NewLine);
		Response.Write(string.Join(Environment.NewLine, csvFormatters.Select(csvFormatter => csvFormatter.FormatCsvLine()).ToArray()));
		Response.Flush();
		Response.End();
	}
	#endregion

	#region -GetSiteInformationTranslationData サイト基本情報翻訳データ取得
	/// <summary>
	/// サイト基本情報翻訳データ取得
	/// </summary>
	/// <returns>サイト基本情報翻訳データ</returns>
	private NameTranslationSettingModel[] GetSiteInformationTranslationData()
	{
		// 翻訳情報をDBから取得
		var siteInformationTranslationSettings = new NameTranslationSettingService().GetAllTranslationSettingsByDataKbn(Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION);

		// 翻訳前情報をXMLから取得して設定
		siteInformationTranslationSettings.ToList().ForEach(setting => SetSiteInformationFromXml(setting, SiteInformationUtility.SiteInformation));
		return siteInformationTranslationSettings;
	}
	#endregion

	#region -SetSiteInformationFromXml Xmlファイルからサイト基本情報を設定
	/// <summary>
	/// Xmlファイルからサイト基本情報を設定
	/// </summary>
	/// <param name="shopMessage"></param>
	/// <param name="xmlInfo"></param>
	/// <returns></returns>
	private NameTranslationSettingModel SetSiteInformationFromXml(
		NameTranslationSettingModel shopMessage,
		IEnumerable<SiteInformationUtility.SiteInformationModel> siteInformationList)
	{
		switch (shopMessage.TranslationTargetColumn)
		{
			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME:
				shopMessage.BeforeTranslationalName = siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.ShopName)).Text;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME:
				shopMessage.BeforeTranslationalName = siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.CompanyName)).Text;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO:
				shopMessage.BeforeTranslationalName = siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.ContactCenterInfo)).Text;
				break;
		}
		return shopMessage;
	}
	#endregion
}