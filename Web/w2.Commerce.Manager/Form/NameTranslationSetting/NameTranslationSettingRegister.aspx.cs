/*
=========================================================================================================
  Module      : 名称翻訳設定登録ページ処理(NameTranslationSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.RefreshFileManager;
using w2.Common.Web;
using w2.Domain.NameTranslationSetting;

public partial class Form_NameTranslationSetting_NameTranslationSettingRegister : NameTranslationSettingPage
{
	/// <summary>対象データ区分に応じたマスタIDのラベル制御</summary>
	protected static readonly Dictionary<string, string[]> m_controlLabelForMasterIdByDataKbn = new Dictionary<string, string[]>
	{
		{ string.Empty, new[]{ string.Empty, string.Empty }},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, new[]
			{
				//「商品ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION, new[]
			{
				//「商品ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT),
				//「商品バリエーションID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION)
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY, new[]
			{
				//「カテゴリID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION, new[]
			{
				//「セットプロモーションID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET, new[]
			{
				//「商品セットID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON, new[]
			{
				//「クーポンID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK, new[]
			{
				//「会員ランクID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT, new[]
			{
				//「決済種別ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING, new[]
			{
				//「ユーザ拡張項目ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND, new[]
			{
				//「ブランドID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY, new[]
			{
				//「ノベルティID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS, new[]
			{
				//「新着ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS),
				string.Empty 
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING, new[]
			{
				//「注文メモID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON, new[]
			{
				//「解約理由区分ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING, new[]
			{
				//「設定ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING),
				//「設定区分」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER_PRODUCT_TAG_SETTING_NAME),
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION, new[]
			{
				string.Empty,
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING, new[]
			{
				//「タグID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE, new[]
			{
				//「コーディネートID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY, new[]
			{
				//「コーディネートカテゴリID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF, new[]
			{
				//「スタッフID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF),
				string.Empty
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP, new[]
			{
				//「リアル店舗ID」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING,
					Constants.VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP),
				string.Empty
			}
		},
	};

	/// <summary>対象データ区分に応じたマスタIDの表示制御</summary>
	protected static readonly Dictionary<string, bool[]> m_controlDisplayForMasterIdByDataKbn = new Dictionary<string, bool[]>
	{
		{ string.Empty, new[]{ true, false }},
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION, new[]{ true, true } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING, new[]{ true, true } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION, new[]{ false, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF, new[]{ true, false } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP, new[]{ true, false } },
	};

	/// <summary>対象データ区分、翻訳対象項目に応じた表示HTML区分の表示制御</summary>
	protected static readonly Dictionary<string, Dictionary<string, bool[]>> m_controlDisplayForDisplayKbn = new Dictionary<string, Dictionary<string, bool[]>>
	{
		{ string.Empty, new Dictionary<string, bool[]> { { string.Empty, new[] { true, true } } } },
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME, new[]{ false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4, new[]{ true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_RETURN_EXCHANGE_MESSAGE, new[]{ false, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_CATCHCOPY, new[]{ false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME1, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME2, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME3, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTCATEGORY_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME, new[] { false , false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION, new[] { true, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_PRODUCT_SET_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_DESCRIPTION, new[] { true, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_DISCRIPTION, new[] { false, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_MEMBERRANK_MENBER_RANK_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_SETTING_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE, new[] { true, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTBRAND_BRAND_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NOVELTY_NOVELTY_DISP_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NEWS_NEWS_TEXT, new[] { true, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_ORDER_MEMO_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_DEFAULT_TEXT, new[] { false, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTLISTDISPSETTING_SETTING_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO, new[] { false, true } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTTAGSETTING_TAG_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_TITLE, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_SUMMARY, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_NAME, new[] { false, false } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_PROFILE, new[] { false, false } },
			}
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP, new Dictionary<string, bool[]>
			{
				{ string.Empty, new[] { true, true } },
				{ Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_REALSHOP_NAME, new[] { false, false } },
			}
		},
	};

	/// <summary>対象データ区分に応じたマスタIDの初期値制御</summary>
	protected static readonly Dictionary<string, string[]> m_controlDefaultValueForMasterIdByDataKbn = new Dictionary<string, string[]>
	{
		{ string.Empty, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION, new[]{ "SiteInformation", string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF, new[]{ string.Empty, string.Empty } },
		{ Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP, new[]{ string.Empty, string.Empty } },
	};

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if ((this.ActionStatus != Constants.ACTION_STATUS_INSERT)
				&& (this.ActionStatus != Constants.ACTION_STATUS_COPY_INSERT)
				&& (this.ActionStatus != Constants.ACTION_STATUS_UPDATE))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			InitializeComponents();

			// 表示制御
			switch (this.ActionStatus)
			{
				// 新規登録
				case Constants.ACTION_STATUS_INSERT:

					trRegister.Visible = true;
					trMasterId2.Visible = false;
					btnGetBeforeTranslationalName.Visible = true;
					btnInsertTop.Visible = btnInsertBottom.Visible = true;
					btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
					btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
					btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
					break;

				// コピー新規登録 OR 更新
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:

					var data = new NameTranslationSettingService().GetNameTranslationSettingForRegister(
						this.RequestDataKbn,
						this.RequestTranslationTargetColumn,
						this.RequestMasterId1,
						this.RequestMasterId2);
					if (data.Languages.Any() == false)
					{
						// 該当データ無しエラー
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					SetSiteInformationFromXml(data, SiteInformationUtility.SiteInformation);

					// コピー新規
					if (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
					{
						trRegister.Visible = true;
						btnGetBeforeTranslationalName.Visible = true;
						btnInsertTop.Visible = btnInsertBottom.Visible = true;
						btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
						btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
						btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
					}
					// 更新
					else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
					{

						ddlDataKbn.Enabled = false;
						ddlTranslationTargetColumn.Enabled = false;
						tbMasterId1.Enabled= tbMasterId2.Enabled= false;

						trEdit.Visible = true;
						btnInsertTop.Visible = btnInsertBottom.Visible = false;
						btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
						btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
						btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
					}

					// 対象データ区分選択
					ddlDataKbn.SelectedValue = data.DataKbn;
					ddlDataKbn_OnSelectedIndexChanged(ddlDataKbn, e);
					// 翻訳対象項目選択
					ddlTranslationTargetColumn.SelectedValue = data.TranslationTargetColumn;

					tbMasterId1.Text = data.MasterId1;
					tbMasterId2.Text = data.MasterId2;

					lBeforeTranslationalName.Text = WebSanitizer.HtmlEncode(data.BeforeTranslationalName);
					sBeforeTranslationalNameNotExists.Visible = (data.BeforeTranslationalName == null);

					var languages = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(
						l => new NameTranslationSettingModel()
						{
							DataKbn = data.DataKbn,
							TranslationTargetColumn = data.TranslationTargetColumn,
							LanguageCode = l.Code,
							LanguageLocaleId = l.LocaleId,
							DisplayKbn = "0",
						}).ToList();

					languages.ForEach(
						language =>
						{
							var model = data.Languages.FirstOrDefault(
								l => ((language.LanguageCode == l.LanguageCode)
									&& (language.LanguageLocaleId == l.LanguageLocaleId)));

							if (model != null)
							{
								language.AfterTranslationalName = model.AfterTranslationalName;
								language.DisplayKbn = model.DisplayKbn;
							}
						});
					rLanguages.DataSource = languages;
					rLanguages.DataBind();

					divComp.Visible = this.IsRegisterSuccess;
					break;
			}
		}

	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		var url = CreateListUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// コピー新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = CreateRegisterUrl(
			Constants.ACTION_STATUS_COPY_INSERT,
			ddlDataKbn.SelectedValue,
			ddlTranslationTargetColumn.SelectedValue,
			tbMasterId1.Text.Trim(),
			tbMasterId2.Text.Trim());
		Response.Redirect(url);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			
			var service = new NameTranslationSettingService();
			service.DeleteNameTranslationSettingForRegister(
				this.RequestDataKbn,
				this.RequestTranslationTargetColumn,
				this.RequestMasterId1,
				this.RequestMasterId2,
				accessor);
			accessor.CommitTransaction();
		}

		// フロント系サイトの表示を最新状態にする
		CreateUpdateRefreshFile();

		var url = CreateListUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 登録・更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, EventArgs e)
	{
		var dataKbn = ddlDataKbn.SelectedValue;
		var translationTargetColumn = ddlTranslationTargetColumn.SelectedValue;
		var masterId1 = tbMasterId1.Text.Trim();
		var masterId2 = tbMasterId2.Text.Trim();

		var insertModelList = new List<NameTranslationSettingModel>();
		foreach (RepeaterItem item in rLanguages.Items)
		{
			var rbDisplayKbn = (RadioButtonList)item.FindControl("rbDisplayKbn");
			var tbAfterTranslationalName = (TextBox)item.FindControl("tbAfterTranslationalName");
			var hfLanguageCode = (HiddenField)item.FindControl("hfLanguageCode");
			var hfLanguageLocaleId = (HiddenField)item.FindControl("hfLanguageLocaleId");

			var displayKbn = rbDisplayKbn.SelectedValue;
			var afterTranslationalName = tbAfterTranslationalName.Text;
			var languageCode = hfLanguageCode.Value;
			var languageLocaleId = hfLanguageLocaleId.Value;

			var input = new NameTranslationSettingInput
			{
				DataKbn = dataKbn,
				TranslationTargetColumn = translationTargetColumn,
				MasterId1 = tbMasterId1.Visible ? masterId1 : null,
				MasterId2 = tbMasterId2.Visible ? masterId2 : null,
				MasterId3 = string.Empty,
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
				DisplayKbn = displayKbn,
				AfterTranslationalName = afterTranslationalName,
			};

			// 入力チェック
			var errorMessage = input.Validate();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			input.MasterId1 = masterId1;
			input.MasterId2 = masterId2;

			var model = input.CreateModel();

			// 翻訳後名称がブランクの場合は、登録しない
			if (string.IsNullOrEmpty(model.AfterTranslationalName)) continue;

			insertModelList.Add(model);
		}

		// 翻訳後名称チェック
		if (insertModelList.Any() == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NAME_TRANSLATION_SETTING_INPUT_NAME_ONE_OR_MORE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 新規登録、コピー新規登録ではマスタ存在チェックを行う
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			var data = new NameTranslationSettingService().GetNameTranslationSettingForRegister(
				dataKbn,
				translationTargetColumn,
				masterId1,
				masterId2);
			if (data.Languages.Any())
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NAME_TRANSLATION_SETTING_DATA_EXISTS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var service = new NameTranslationSettingService();
			service.DeleteNameTranslationSettingForRegister(
				dataKbn,
				translationTargetColumn,
				masterId1,
				masterId2,
				accessor);

			insertModelList
				.ForEach(model => service.Insert(model, accessor));
			accessor.CommitTransaction();
		}

		// フロント系サイトの表示を最新状態にする
		CreateUpdateRefreshFile();

		var url = CreateRegisterUrl(
			Constants.ACTION_STATUS_UPDATE,
			dataKbn,
			translationTargetColumn,
			masterId1,
			masterId2,
			REQUEST_SUCCESS_VALUE_TRUE);
		Response.Redirect(url);
	}

	/// <summary>
	/// 取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetBeforeTranslationalName_Click(object sender, EventArgs e)
	{
		var data = new NameTranslationSettingService().GetBeforeTranslationName(
			ddlDataKbn.SelectedValue,
			ddlTranslationTargetColumn.SelectedValue,
			tbMasterId1.Text.Trim(),
			tbMasterId2.Text.Trim());

		SetSiteInformationFromXml(data, SiteInformationUtility.SiteInformation);

		lBeforeTranslationalName.Text = WebSanitizer.HtmlEncode(data.BeforeTranslationalName);
		sBeforeTranslationalNameNotExists.Visible = (data.BeforeTranslationalName == null);
	}

	/// <summary>
	/// 対象データ区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDataKbn_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = ((DropDownList)sender).SelectedValue;
		ddlTranslationTargetColumn.DataSource = m_translationColumnProducts[selectedValue];
		ddlTranslationTargetColumn.DataBind();

		var displayControlOfMasterId = m_controlDisplayForMasterIdByDataKbn[selectedValue];
		trMasterId1.Visible = displayControlOfMasterId[0];
		trMasterId2.Visible = displayControlOfMasterId[1];

		var defaultValueOfMasterId = m_controlDefaultValueForMasterIdByDataKbn[selectedValue];
		tbMasterId1.Text = defaultValueOfMasterId[0];
		tbMasterId2.Text = defaultValueOfMasterId[1];

		var labelOfMasterId = m_controlLabelForMasterIdByDataKbn[selectedValue];
		lMasterId1.Text = string.IsNullOrEmpty(labelOfMasterId[0]) ? "": string.Format("({0})", labelOfMasterId[0]);
		lMasterId2.Text = string.IsNullOrEmpty(labelOfMasterId[1]) ? "": string.Format("({0})", labelOfMasterId[1]);
	}

	/// <summary>
	/// 翻訳対象項目選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTranslationTargetColumn_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = ((DropDownList)sender).SelectedValue;
		foreach (RepeaterItem item in rLanguages.Items)
		{
			DisplayControlOfDisplayKbn(item, selectedValue);

			// 表示HTML区分が非表示なる場合は、選択値をTEXTに設定する
			if (item.FindControl("dDisplayKbn").Visible == false)
			{
				var rbDisplayKbn = (RadioButtonList)item.FindControl("rbDisplayKbn");
				rbDisplayKbn.SelectedValue = "0";
			}
		}
	}

	/// <summary>
	/// ロケール毎のリピータにバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rLanguages_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		DisplayControlOfDisplayKbn(e.Item, ddlTranslationTargetColumn.SelectedValue);
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		ddlDataKbn.DataSource = m_dataKbnListItemCollection;
		ddlDataKbn.DataBind();

		var languages = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(
			l => new NameTranslationSettingModel
			{
				LanguageCode = l.Code,
				LanguageLocaleId = l.LocaleId,
			});

		rLanguages.DataSource = languages;
		rLanguages.DataBind();
	}

	/// <summary>
	/// 一覧URL生成
	/// </summary>
	/// <returns>URL</returns>
	private string CreateListUrl()
	{
		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_LIST)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_DATA_KBN, this.SearchInfo.DataKbn)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_TRANSLATION_TARGET_COLUMN, this.SearchInfo.TranslationTargetColumn)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID1, this.SearchInfo.MasterId1)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID2, this.SearchInfo.MasterId2)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_LANGUAGE, this.SearchInfo.Language)
			.CreateUrl();
		return nextUrl;
	}
	/// <summary>
	/// 表示HTML区分の表示制御
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <param name="translationTargetColumn">翻訳対象項目</param>
	private void DisplayControlOfDisplayKbn(RepeaterItem item, string translationTargetColumn)
	{
		var tbAfterTranslationalName = ((TextBox)item.FindControl("tbAfterTranslationalName"));

		var controlDisplayForDisplayKbn = m_controlDisplayForDisplayKbn[ddlDataKbn.SelectedValue][translationTargetColumn];
		var displayHtmlFlg = controlDisplayForDisplayKbn[0];
		var isMultiLine = controlDisplayForDisplayKbn[1];

		item.FindControl("dDisplayKbn").Visible = displayHtmlFlg;

		tbAfterTranslationalName.TextMode = isMultiLine ? TextBoxMode.MultiLine : TextBoxMode.SingleLine;
		tbAfterTranslationalName.Height = isMultiLine ? 150 : 25;
	}

	/// <summary>
	/// フロント系サイトの表示を最新状態にする
	/// </summary>
	private void CreateUpdateRefreshFile()
	{
		RefreshFileType? type = null;
		switch (this.RequestDataKbn)
		{
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT:
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION:
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY:
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND:
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET:
				type = RefreshFileType.DisplayProduct;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK:
				type = RefreshFileType.MemberRank;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS:
				type = RefreshFileType.News;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY:
				type = RefreshFileType.Novelty;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON:
				type = RefreshFileType.FixedPurchaseCancelReason;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING:
				type = RefreshFileType.ProductListDispSetting;
				break;
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION:
				type = RefreshFileType.SetPromotion;
				break;
			case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING:
				type = RefreshFileType.UserExtendSetting;
				break;
		}

		if (type.HasValue == false) return;

		// フロント系サイトの表示を最新状態にする
		RefreshFileManagerProvider.GetInstance(type.Value).CreateUpdateRefreshFile();
	}
}