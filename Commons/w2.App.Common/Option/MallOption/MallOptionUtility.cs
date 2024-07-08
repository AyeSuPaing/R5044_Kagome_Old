/*
=========================================================================================================
  Module      : モール連携オプション共通処理クラス(MallOptionUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using w2.Common.Sql;
using w2.Common.Net.Mail;
using System.Text.RegularExpressions;

namespace w2.App.Common.Option
{
	///*********************************************************************************************
	/// <summary>
	/// モール連携オプション共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public class MallOptionUtility
	{
		// 列挙体（自社サイト、楽天、ヤフー）
		public enum MallKbn { OwnSite, Rakuten, Yahoo }

		// 楽天あんしんメルアドサービスドメイン部(.fwの前はPC/モバイルキャリア毎に異なる文字が入る)
		public const string RakutenMaskAddressDomain = ".fw.rakuten.ne.jp";

		/// <summary>
		/// モール区分取得
		/// </summary>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strMallId">モールID</param>
		/// <returns>モール区分</returns>
		public static MallKbn CheckMallKbn(string strShopId, string strMallId)
		{
			if (strMallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
			{
				DataView dvMallLiaise = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSetting"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, strShopId);
					htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, strMallId);

					dvMallLiaise = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				if (dvMallLiaise.Count > 0)
				{
					// 楽天
					if ((string)dvMallLiaise[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] == "R")
					{
						return MallKbn.Rakuten;
					}
					// ヤフー
					if ((string)dvMallLiaise[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] == "Y")
					{
						return MallKbn.Yahoo;
					}
				}
			}

			// モールIDが登録されていない（自社サイト）
			return MallKbn.OwnSite;
		}

		/// <summary>
		/// 自社サイトの注文？
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>自社サイト？</returns>
		public static bool IsOwnSite(string shopId, string mallId)
		{
			var mallKbn = CheckMallKbn(shopId, mallId);
			return (mallKbn == MallKbn.OwnSite);
		}

		/// <summary>
		/// SMTPサーバ設定取得
		/// </summary>
		/// <param name="strOrderId">モールID</param>
		/// <returns>SMTPサーバ設定XML</returns>
		public static string GetSmtpServerSetting(string strMallId)
		{
			string strSettingXml;
			//------------------------------------------------------
			// SMTPサーバ設定取得
			//------------------------------------------------------
			DataView dvSmtpServerSetting = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetSmtpServerSetting"))
			{
				Hashtable htSqlParam = new Hashtable();
				htSqlParam.Add(Constants.FIELD_ORDER_MALL_ID, strMallId);
				htSqlParam.Add(Constants.FIELD_ORDER_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID);

				dvSmtpServerSetting = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
			}
			if (dvSmtpServerSetting.Count == 0)
			{
				throw new ApplicationException("Smtpサーバ設定が取得できませんでした");
			}

			// サーバ情報を格納
			strSettingXml = (string)dvSmtpServerSetting[0][Constants.FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING];

			return strSettingXml;
		}

		/// <summary>
		/// 楽天マスクアドレス存在チェック
		/// </summary>
		/// <param name="mmMessage">MailMessage</param>
		public static bool CheckRakutenMaskAddress(MailMessage mmMessage)
		{
			// 楽天マスクアドレスドメイン部で文字列が終了しているチェックする（チェック対象はTo)
			foreach (MailAddress maAddressTo in mmMessage.To)
			{
				if (Regex.IsMatch(maAddressTo.Address, RakutenMaskAddressDomain + "$"))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 個別:商品設定のカラム名取得
		/// </summary>
		/// <param name="exhibitsConfig">商品設定番号</param>
		/// <returns>カラム名</returns>
		public static string GetExhibitsConfigField(string exhibitsConfig)
		{
			switch (exhibitsConfig)
			{
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_1:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_2:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_3:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_4:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_5:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_6:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_7:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_8:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_9:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_10:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_11:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_12:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_13:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_14:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_15:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_16:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_17:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_18:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_19:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19;
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_20:
					return Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20;
				default:
					return string.Empty;
			}
		}
	}
}