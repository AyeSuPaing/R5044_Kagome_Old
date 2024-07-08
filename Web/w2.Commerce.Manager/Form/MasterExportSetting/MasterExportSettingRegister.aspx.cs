/*
=========================================================================================================
  Module      : マスタ出力定義登録ページ処理(MasterExportSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.MasterExport;
using w2.Domain.MasterExportSetting;
using w2.Domain.ProductTaxCategory;
using w2.Domain.User.Helper;

public partial class Form_MasterExportSetting_MasterExportSettingRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// マスタ出力定義登録表示
			//------------------------------------------------------
			ViewMasterExportSettingRegister();
		}

		//Set Master field data
		SetListMasterFieldData(rblMasterKbn.SelectedValue);
	}

	#region -マスタ出力表示
	/// <summary>
	/// マスタ出力定義登録表示
	/// </summary>
	private void ViewMasterExportSettingRegister()
	{
		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		Hashtable htParam = GetParameters(Request);

		// 不正パラメータが存在した場合エラーページへ
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		if (Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] != null)
		{
			ddlSelectSetting.SelectedIndex = (int)Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID];
			Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = null;
		}
		else
		{
			btnDeleteTop.Enabled = false;
		}

		// マスタ区分取得
		string masterKbn = (string)htParam[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN];

		// マスタ出力定義情報取得
		var masterExportSettings = new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, masterKbn);

		// 該当データが存在する場合
		if (masterExportSettings.Length > 0)
		{
			var settings = masterExportSettings[ddlSelectSetting.SelectedIndex];

			// マスタ出力定義情報をビューステートに保存
			ViewState[Constants.SESSIONPARAM_KEY_MASTEREXPORTSETTING_INFO] = settings.DataSource;

			// フィールド列設定
			tbFields.Text = StringUtility.ToEmpty(settings.Fields).Replace(",", ",\n");

			// マスタ出力形式の値設定
			ddlExportFileType.SelectedValue = settings.ExportFileType;

			// 更新ボタン表示
			btnUpdateTop.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteTop.Enabled = ddlSelectSetting.SelectedIndex != 0;
		}
		else
		{
			//「（削除不可）」
			Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
				Constants.VALUETEXT_PARAM_UNDELITEABLE);
			ddlSelectSetting.Visible = false;
			tbSettingName.Enabled = false;
			tbSettingName.Text = rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE;

		}
		btnRegisterTop.Visible = true;
	}

	/// <summary>
	/// Set Master field data
	/// </summary>
	/// <param name="masterKbn"> Master classification</param>
	private void SetListMasterFieldData(string masterKbn)
	{
		//「拡張ステータス」
		var textExtendedStatus = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_STATUS);
		//「拡張ステータス更新日時」
		var textExtendedStatusDate = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
			Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_STATUS_DATE);
		// マスタフィールド取得
		List<Hashtable> masterFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(masterKbn);

		// マスタ区分がユーザーマスタの場合
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER)
		{
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			foreach (var userExtendSetting in userExtendSettingList.Items)
			{
				//「拡張項目」
				var textExtendItem = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_ITEM);
				var valueUserExtendItem = string.Format(
					"{0}：{1}",
					textExtendItem,
					((userExtendSetting.SettingName != string.Empty)
						? userExtendSetting.SettingName
						: userExtendSetting.SettingId));

				Hashtable userExtend = new Hashtable();
				userExtend.Add(Constants.MASTEREXPORTSETTING_XML_NAME, userExtendSetting.SettingId);
				userExtend.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, valueUserExtendItem);

				masterFields.Add(userExtend);
			}
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
			masterFields.AddRange(userAttributeFields);
		}

		// マスタ区分が商品タグマスタの場合
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG)
		{
			foreach (DataRowView drv in GetProductTagSetting())
			{
				Hashtable tag = new Hashtable();
				tag.Add(Constants.MASTEREXPORTSETTING_XML_NAME, drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
				tag.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, ((string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] != "") ? drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] : drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);

				masterFields.Add(tag);
			}
		}

		// マスタ区分が商品拡張項目マスタの場合、商品拡張項目取得
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND)
		{
			foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
			{
				Hashtable htExtend = new Hashtable();
				htExtend.Add(Constants.MASTEREXPORTSETTING_XML_NAME, "extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString());
				htExtend.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME].ToString());

				masterFields.Add(htExtend);
			}
		}

		// マスタ区分が受注・受注商品マスタの場合
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER) || (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM))
		{
			// 税率毎価格情報項目設定
			var taxRates = new ProductTaxCategoryService().GetAllTaxCategory()
				.Select(taxCategory => taxCategory.TaxRate)
				.Distinct()
				.OrderBy(taxRate => taxRate);
			foreach (var taxRate in taxRates.Distinct())
			{
				var taxPriceFields = new List<Hashtable>();
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「商品小計」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SUBTOTAL))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「配送料金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SHIPPING))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「決済手数料金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PAYMENT_PRICE))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「返品用金額補正」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_RETURN_PRICE))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「合計金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_TOTAL))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「消費税額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_TAX_PRICE))},
				});
				masterFields.AddRange(taxPriceFields);
			}
			// 利用上限数までを有効にする
			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				Hashtable orderExtend = new Hashtable();
				orderExtend.Add(Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString());
				orderExtend.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, textExtendedStatus + StringUtility.ToZenkaku(i));
				masterFields.Add(orderExtend);

				Hashtable orderExtendDate = new Hashtable();
				orderExtendDate.Add(Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i.ToString());
				orderExtendDate.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, textExtendedStatusDate + StringUtility.ToZenkaku(i));

				masterFields.Add(orderExtendDate);
			}
		}

		// マスタ区分が定期・定期商品マスタの場合
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE) || (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM))
		{
			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var fixedPurchaseExtend = new Hashtable();
				fixedPurchaseExtend.Add(Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString());
				fixedPurchaseExtend.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, textExtendedStatus + StringUtility.ToZenkaku(i));
				masterFields.Add(fixedPurchaseExtend);
				var fixedPurchaseExtendDate = new Hashtable();
				fixedPurchaseExtendDate.Add(
					Constants.MASTEREXPORTSETTING_XML_NAME,
					Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i.ToString());
				fixedPurchaseExtendDate.Add(
					Constants.MASTEREXPORTSETTING_XML_J_NAME,
					textExtendedStatusDate + StringUtility.ToZenkaku(i));
				masterFields.Add(fixedPurchaseExtendDate);
			}
		}


		if (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM)
			))
		{
			var textOrderExtendItem = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_ORDER_EXTENDED_ITEM);

			masterFields.AddRange(
				DataCacheControllerFacade.GetOrderExtendSettingCacheController()
					.CacheData.SettingModels.Select(
						model => new Hashtable()
						{
							{ Constants.MASTEREXPORTSETTING_XML_NAME, model.SettingId },
							{ Constants.MASTEREXPORTSETTING_XML_J_NAME, string.Format("{0} {1}", textOrderExtendItem, model.SettingName, model.SettingId) },
						}));
		}

		// マスタ区分がデータ結合マスタの場合
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING)
		{
			// ユーザーマスター
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			foreach (var userExtendSetting in userExtendSettingList.Items)
			{
				var textExtendItem = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
					Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_ITEM);
				var valueUserExtendItem = string.Format(
					"{0}：{1}",
					textExtendItem,
					string.IsNullOrEmpty(userExtendSetting.SettingName)
						? userExtendSetting.SettingId
						: userExtendSetting.SettingName);

				var userExtend = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_USER_EXTEND_CONVERTING_NAME + userExtendSetting.SettingId },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "ユーザー拡張項目：" + valueUserExtendItem },
				};

				masterFields.Add(userExtend);
			}
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING);

			masterFields.AddRange(userAttributeFields);

			// 税率毎価格情報項目設定
			var taxRates = new ProductTaxCategoryService().GetAllTaxCategory()
				.Select(taxCategory => taxCategory.TaxRate)
				.Distinct()
				.OrderBy(taxRate => taxRate);
			foreach (var taxRate in taxRates.Distinct())
			{
				var taxPriceFields = new List<Hashtable>();
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「商品小計」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SUBTOTAL))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「配送料金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SHIPPING))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「決済手数料金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PAYMENT_PRICE))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「返品用金額補正」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_RETURN_PRICE))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「合計金額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_TOTAL))},
				});
				taxPriceFields.Add(new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE + "_" + taxRate.ToString().Replace(".", "")},
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME,
						string.Format(
							"{0} %{1}",
							taxRate,
							//「消費税額」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
								Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_TAX_PRICE))},
				});
				masterFields.AddRange(taxPriceFields);
			}

			// 商品タグOPオンの場合のみ商品タグのフィールドを追加
			if (Constants.PRODUCT_TAG_OPTION_ENABLE)
			{
				foreach (DataRowView drv in GetProductTagSetting())
				{
					var jName = string.IsNullOrEmpty((string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME])
						? drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]
						: drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME];
					var tag = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_PRODUCT_TAG_CONVERTING_NAME + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "商品タグ：" + jName },
				};
					masterFields.Add(tag);
				}
			}

			// モール連携OPオンの場合のみ商品拡張項目のフィールドを追加
			if (Constants.MALLCOOPERATION_OPTION_ENABLED)
			{
				foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
				{
					var htExtend = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_PRODUCT_EXTEND_CONVERTING_NAME + "extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]) },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "商品拡張項目：" + drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME] },
				};
					masterFields.Add(htExtend);
				}
			}

			for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var orderExtend = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "受注：" + textExtendedStatus + StringUtility.ToZenkaku(i) },
				};
				masterFields.Add(orderExtend);

				var orderExtendDate = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_CONVERTING_NAME + Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "受注：" + textExtendedStatusDate + StringUtility.ToZenkaku(i) },
				};
				masterFields.Add(orderExtendDate);
			}


			// 定期OPオンの場合のみ定期拡張項目のフィールドを追加
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
				{
					var fixedPurchaseExtend = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "定期購入拡張：" + textExtendedStatus + StringUtility.ToZenkaku(i) },
				};
					masterFields.Add(fixedPurchaseExtend);
					var fixedPurchaseExtendDate = new Hashtable
				{
					{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i },
					{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "定期購入拡張：" + textExtendedStatusDate + StringUtility.ToZenkaku(i) },
				};
					masterFields.Add(fixedPurchaseExtendDate);
				}
			}

			// 注文拡張項目ON時のみ注文拡張項目のフィールドを追加
			if (Constants.ORDER_EXTEND_OPTION_ENABLED)
			{
				// 注文拡張項目マスター
				var textOrderExtendItem = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING,
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER,
				Constants.VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_ORDER_EXTENDED_ITEM);

				masterFields.AddRange(
					DataCacheControllerFacade.GetOrderExtendSettingCacheController()
						.CacheData.SettingModels.Select(
							model => new Hashtable
							{
								{ Constants.MASTEREXPORTSETTING_XML_NAME, Constants.FIELD_ORDER_EXTEND_SETTING_CONVERTING_NAME + model.SettingId },
								{ Constants.MASTEREXPORTSETTING_XML_J_NAME, "注文拡張項目：" + model.SettingName },
							}));
			}
		}

		// データソース設定
		rList.DataSource = MasterFieldSetting.RemoveMasterFields(masterFields, masterKbn);
		// データバインド
		rList.DataBind();
	}

	/// <summary>
	///　マスタ出力定義登録パラメタ取得
	/// </summary>
	/// <param name="hrRequest">マスタ出力定義登録のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable result = new Hashtable();
		string masterKbn = String.Empty;
		bool paramError = false;

		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN]))
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT: // 商品マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION: // 商品バリエーション表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE: // 会員ランク価格
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG: // 商品タグ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND: // 商品拡張項目表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK: // 商品在庫表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY: // 商品カテゴリ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER: // 注文マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM: // 注文商品マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION: // 注文セットプロモーションマスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER: // ユーザーマスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER: // 発注マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM: // 発注商品マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL: // 入荷通知メールマスタ表示（サマリー）
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL: // 入荷通知メールマスタ表示（明細）
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW: // 商品レビュー
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE: // 商品セール価格マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL: // ショートURL
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE: // 定期購入マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM: // 定期購入商品マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY: // シリアルキーマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP: // リアル店舗マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK: // リアル店舗商品在庫マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING: // ユーザ配送先マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE: // ユーザー電子発票管理マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR: // オペレータマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
					masterKbn = hrRequest[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
					break;

				case "":
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER; // ユーザーマスタがデフォルト
					break;

				default:
					paramError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN, masterKbn);
			foreach (ListItem li in rblMasterKbn.Items)
			{
				li.Selected = (li.Value == masterKbn);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// マスタ種別ラジオボタンリスト作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN))
		{
			bool enable = true;
			switch (li.Value)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND: // 商品拡張項目
					enable = Constants.MALLCOOPERATION_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE: // 商品価格
					enable = Constants.MEMBER_RANK_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION: // 注文セットプロモーション
					enable = Constants.SETPROMOTION_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW: // 商品レビュー
					enable = Constants.PRODUCTREVIEW_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE: // 商品セール価格
					enable = Constants.PRODUCT_SALE_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE: // 定期購入マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM: // 定期購入商品マスタ
					enable = Constants.FIXEDPURCHASE_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY: // シリアルキーマスタ
					enable = Constants.DIGITAL_CONTENTS_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP: // リアル店舗マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK: // リアル店舗商品在庫マスタ
					enable = Constants.REALSHOP_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER: // 発注情報
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM: // 発注商品情報
					enable = Constants.REALSTOCK_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY: // 商品カテゴリ
					enable = Constants.PRODUCT_CTEGORY_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG: // 商品タグ
					enable = Constants.PRODUCT_TAG_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK: // 商品在庫マスタ
					enable = Constants.PRODUCT_STOCK_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL: // ショートURL
					enable = Constants.SHORTURL_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
					enable = Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE;
					break;
			}

			if (enable == false) continue;

			if ((li.Text == ValueText.GetValueText(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, "Operator"))
				&& (Constants.REALSHOP_OPTION_ENABLED == false))
			{
				continue;
			}

			rblMasterKbn.Items.Add(li);
		}

		Hashtable input = GetParameters(Request);
		// 不正パラメータが存在した場合エラーページへ
		if ((bool)input[Constants.ERROR_REQUEST_PRAMETER])
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 出力ファイル形式ドロップダウン
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE))
		{
			ddlExportFileType.Items.Add(li);
		}

		foreach (var exportSetting in new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, rblMasterKbn.SelectedValue))
		{
			ddlSelectSetting.Items.Add(
				new ListItem(
					(exportSetting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
						? rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE
						: exportSetting.SettingName,
					exportSetting.SettingId));
		}

		rblMasterKbn.DataBind();
		rblMasterKbn.SelectedIndex = 0;

		// 完了画面の場合
		if (StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_COMPLETE)
		{
			divComp.Visible = true;
			Session[Constants.SESSION_KEY_ACTION_STATUS] = null;
		}
	}

	#endregion

	#region -マスタ出力 登録/更新/削除
	/// <summary>
	/// 出力設定登録・更新
	/// </summary>
	/// <param name="isRegister">登録アクション</param>
	private void InsertUpdateExportSetting(bool isRegister)
	{
		string shopId = this.LoginOperatorShopId;

		// パラメタ格納
		var input = GetExportSettingInfo(shopId);
		if (isRegister)
		{
			input.SettingName = tbSettingName.Text.Trim();
		}
		else
		{
			input.SettingId = ddlSelectSetting.SelectedValue;
		}

		// 入力チェック
		string errorMessages = input.Validate(isRegister);
		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// フィールド列チェック
		if (CheckMasterFields(shopId, input.Fields, input.MasterKbn) == false)
		{
			string errorInfo = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORTSETTING_FIELDS_ERROR);
			if (this.ErrorFieldName.Length != 0)
			{
				errorInfo += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_APPLICABLE)
					.Replace("@@ 1 @@", WebSanitizer.HtmlEncode(this.ErrorFieldName.ToString()));
			}

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorInfo;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var model = input.CreateModel();
		if (isRegister)
		{
			new MasterExportSettingService().Insert(model);
			LoadSettingList(rblMasterKbn.SelectedValue, ddlSelectSetting.Items.Count);
		}
		else
		{
			new MasterExportSettingService().Update(model);
		}

		// 完了メッセージ表示用パラメータ設定
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 登録画面へ戻る
		Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = ddlSelectSetting.SelectedIndex;
		Response.Redirect(CreateMasterExportSettingRegiestUrl(input.MasterKbn));
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

		// マスタ区分がユーザーマスタの場合、ユーザー拡張項目、ユーザー属性取得
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER)
		{
			// 名称からフィールド名を取得できるようHashtable作成
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			userExtendSettingList.Items.ForEach(userExtendSetting =>
				fields.Add(userExtendSetting.SettingId, Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId));

			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
			foreach (var key in userAttributeFields.Keys)
			{
				fields[key] = userAttributeFields[key];
			}
		}

		// マスタ区分が商品タグマスタの場合、
		// 商品タグ項目取得
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG)
		{
			foreach (DataRowView drv in GetProductTagSetting())
			{
				fields.Add(drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID], "w2_ProductTag." + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
			}
		}

		// マスタ区分が商品拡張項目マスタの場合、
		// 商品拡張項目取得
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND)
		{
			foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
			{
				// 名称からフィールド名を取得できるようHashtable作成
				fields.Add("extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString(), "w2_ProductExtend.extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString());
			}
		}

		// マスタ区分が注文系マスタの場合に変換を行う
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM))
		{
			// 注文系マスタフィールドセット
			MasterExportSettingUtility.SetFieldInfoForOrder(fields);

			// 利用不可能な税カテゴリの名称の場合はエラーページへ
			var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
			var settingTaxFieldNames = StringUtility.SplitCsvLine(namesCsv)
				.Where(name => name.Contains("_by_rate_"))
				.Distinct();
			var illegalTaxFields = settingTaxFieldNames.Where(
				settingTaxFieldName => taxFieldNames.Any(taxFieldName => settingTaxFieldName == taxFieldName) == false).ToArray();
			if (illegalTaxFields.Any())
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR).Replace("@@ 1 @@", string.Join("<br>", illegalTaxFields)).Replace("@@ 2 @@", ValueText.GetValueText(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, masterKbn));
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		// Extend status and Extend status Update Date for Fixed Purchase and Fixed Purchase item
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE) || (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM))
		{
			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var extendName = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + i.ToString();
				fields.Add(extendName, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendName));

				var extendDate = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME + i.ToString();
				fields.Add(extendDate, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendDate));
			}
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED && ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION)))
		{
			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				fields.Add(model.SettingId, string.Format("{0}.{1}", Constants.TABLE_ORDER, model.SettingId));
			}
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED && ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM)))
		{
			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				fields.Add(model.SettingId, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, model.SettingId));
			}
		}

		// データ結合マスタフィールドチェック 各フィールド名にプレフィックスをつけて判定を行う
		if (Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE
			&& (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING))
		{
			// 名称からフィールド名を取得できるようHashtable作成
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);

			foreach (var userExtendSetting in userExtendSettingList.Items)
			{
				fields.Add(
					Constants.FIELD_USER_EXTEND_CONVERTING_NAME + userExtendSetting.SettingId,
					Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId);
			}
			
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING);
			foreach (var key in userAttributeFields.Keys)
			{
				fields[key] = userAttributeFields[key];
			}

			foreach (DataRowView drv in GetProductTagSetting())
			{
				fields.Add(
					Constants.FIELD_PRODUCT_TAG_CONVERTING_NAME + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID],
					"w2_ProductTag." + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
			}

			foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
			{
				fields.Add(
					Constants.FIELD_PRODUCT_EXTEND_CONVERTING_NAME + "extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString(),
					"w2_ProductExtend.extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]));
			}

			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var extendName = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + i.ToString();
				fields.Add(Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendName, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendName));

				var extendDate = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME + i.ToString();
				fields.Add(Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendDate, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendDate));
			}

			MasterExportSettingUtility.SetFieldInfoForOrder(fields, masterKbn);

			var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();

			var settingTaxFieldNames = StringUtility.SplitCsvLine(namesCsv)
				.Where(name => name.Contains("_by_rate_"))
				.Distinct();

			var settingTaxFieldNamesArray = settingTaxFieldNames.Select(name => name.Substring(name.IndexOf('_') + 1)).ToArray();

			settingTaxFieldNamesArray = settingTaxFieldNamesArray.Select(fieldName => Constants.FIELD_ORDER_CONVERTING_NAME + fieldName).ToArray();

			var illegalTaxFields = settingTaxFieldNamesArray
				.Where(AsettingTaxFieldName => taxFieldNames.Any(taxFieldName => AsettingTaxFieldName == Constants.FIELD_ORDER_CONVERTING_NAME + taxFieldName) == false)
				.ToArray();

			if (illegalTaxFields.Any())
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR)
					.Replace("@@ 1 @@", string.Join("<br>", illegalTaxFields))
					.Replace("@@ 2 @@", ValueText.GetValueText(
						Constants.TABLE_MASTEREXPORTSETTING,
						Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
						masterKbn));
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			if (Constants.STORE_PICKUP_OPTION_ENABLED
				&& (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR)
				&& (string.IsNullOrEmpty(StringUtility.ToEmpty(fields[Constants.FIELD_REALSHOP_REAL_SHOP_ID])) == false))
			{
				for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
				{
					var extendName = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + i;
					fields.Add(
						Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendName,
						string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendName));

					var extendDate = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME + i;
					fields.Add(
						Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendDate,
						string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendDate));
				}
			}

			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				fields.Add(
					Constants.FIELD_ORDER_EXTEND_SETTING_CONVERTING_NAME + model.SettingId,
					string.Format("{0}.{1}", Constants.TABLE_ORDER, model.SettingId));
			}

			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				fields.Add(
					"order" + Constants.FIELD_ORDER_SETTING_CONVERTING_NAME + model.SettingId,
					string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, model.SettingId));
			}
			fields[Constants.FIELD_REALSHOP_REAL_SHOP_ID] = StringUtility.ToEmpty(fields[Constants.FIELD_REALSHOP_REAL_SHOP_ID])
				.Replace("@delimiter", Constants.MASTERFILEIMPORT_DELIMITER);
		}

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

	/// <summary>
	/// 出力設定情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <returns>入力設定情報</returns>
	private MasterExportSettingInput GetExportSettingInfo(string shopId)
	{
		var input = new MasterExportSettingInput
		{
			ShopId = shopId,
			MasterKbn = rblMasterKbn.SelectedValue,
			Fields = MasterExportSettingUtility.GetFieldsEscape(tbFields.Text),
			LastChanged = this.LoginOperatorName,
			ExportFileType = ddlExportFileType.SelectedValue,
		};
		return input;
	}

	#endregion

	#region イベント
	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTop_Click(object sender, EventArgs e)
	{
		InsertUpdateExportSetting(true);
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, EventArgs e)
	{
		InsertUpdateExportSetting(false);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, EventArgs e)
	{
		new MasterExportSettingService().Delete(
			this.LoginOperatorShopId,
			rblMasterKbn.SelectedValue,
			ddlSelectSetting.SelectedValue);

		LoadSettingList(rblMasterKbn.SelectedValue, ddlSelectSetting.SelectedIndex - 1);

		// 登録画面へ戻る
		Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = ddlSelectSetting.SelectedIndex;
		Response.Redirect(CreateMasterExportSettingRegiestUrl(rblMasterKbn.SelectedValue));
	}

	/// <summary>
	/// マスタ種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblMasterKbn_SelectedIndexChanged1(object sender, EventArgs e)
	{
		// マスタ種別を設定し、再度読込
		Response.Redirect(CreateMasterExportSettingRegiestUrl(rblMasterKbn.SelectedValue));
	}

	/// <summary>
	/// マスタ出力設定変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSelectSetting_SelectedIndexChanged(object sender, EventArgs e)
	{
		btnDeleteTop.Enabled = (ddlSelectSetting.SelectedItem.Value != Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID);
		tbSettingName.Text = string.Empty;
		LoadField();
	}
	#endregion

	/// <summary>
	/// 設定一覧ロード
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <param name="index">選択インデックス</param>
	private void LoadSettingList(string masterKbn, int index)
	{
		ddlSelectSetting.Items.Clear();
		foreach (var setting in new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, masterKbn))
		{
			ddlSelectSetting.Items.Add(
				new ListItem((setting.SettingId).Equals(Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID) ? rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE : setting.SettingName,
					setting.SettingId));
		}
		ddlSelectSetting.SelectedIndex = index;
	}

	/// <summary>
	/// データバインド用マスタ出力情報詳細URL作成
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>マスタ出力情報URL</returns>
	private string CreateMasterExportSettingRegiestUrl(string masterKbn)
	{
		string result = "";
		result += Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTEREXPORTSETTING_REGISTER;
		result += "?";
		result += Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN + "=" + HttpUtility.UrlEncode(masterKbn);

		return result;
	}

	/// <summary>
	/// フィールドロード
	/// </summary>
	private void LoadField()
	{
		var setting = new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, rblMasterKbn.SelectedValue)[ddlSelectSetting.SelectedIndex];
		ddlExportFileType.SelectedValue = setting.ExportFileType;
		tbFields.Text = StringUtility.ToEmpty(setting.Fields).Replace(",", ",\n");
	}

	#region プロパティ
	/// <summary>エラーがあったフィールド</summary>
	public StringBuilder ErrorFieldName
	{
		get { return m_errorFieldName; }
		set { m_errorFieldName = value; }
	}
	private StringBuilder m_errorFieldName = new StringBuilder();
	#endregion
}
