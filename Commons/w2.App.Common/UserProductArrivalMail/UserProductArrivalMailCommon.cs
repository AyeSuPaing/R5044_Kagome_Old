/*
=========================================================================================================
  Module      : 入荷通知メール共通処理クラス(UserProductArrivalMailCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Order;
using w2.Common.Sql;
using w2.Domain.User;

namespace w2.App.Common.UserProductArrivalMail
{
	///*********************************************************************************************
	/// <summary>
	/// 入荷通知メール共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class UserProductArrivalMailCommon
	{
		/// <summary>
		/// フロント側入荷通知メール情報一覧取得
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="iPageNumber">表示開始記事番号</param>
		/// <param name="iPageCount">1ページの表示件数</param>
		/// <returns>入荷通知メール情報一覧データビュー</returns>
		public static DataView GetUserProdcutArrivalMailList(string strUserId, int iPageNumber, int iPageCount)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "GetUserProductArrivalmailList"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, strUserId);
				htInput.Add("bgn_row_num", iPageCount * (iPageNumber - 1) + 1);
				htInput.Add("end_row_num", iPageCount * iPageNumber);

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// フロント側商品入荷通知情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>入荷通知メール情報データビュー</returns>
		public static DataView GetUserProductArrivalMailInfo(string userId, string productId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("UserProductArrivalMail", "GetUserProductArrivalMailInfo"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, userId},
					{Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, productId},
				};
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}

		/// <summary>
		/// フロント側商品バリエーション入荷通知情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>入荷通知メール情報データビュー</returns>
		public static DataView GetUserProductArrivalMailInfo(string userId, string productId, string variationId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("UserProductArrivalMail", "GetUserProductVariationArrivalMailInfo"))
			{
				var ht = new Hashtable
			{
					{Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, userId},
					{Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, productId},
					{Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, variationId},
				};
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}

		/// <summary>
		/// 入荷通知メール区分取得
		/// </summary>
		/// <param name="blUseVariation">バリエーション有り</param>
		/// <param name="blVariationSelected">バリエーション選択状態</param>
		/// <param name="dvProductVariationList">全バリエーション情報</param>
		/// <param name="drvSelectedProductVariation">選択されたバリエーション</param>
		/// <returns>入荷通知メール区分</returns>
		public static string GetArrivalMailKbn(
			bool blUseVariation,
			bool blVariationSelected,
			DataView dvProductVariationList,
			DataRowView drvSelectedProductVariation)
		{
			string strArrivalMailKbn = null;

			// 販売期間中 かつ 在庫0以下でも表示(販売不可) かつ 在庫0以下 かつ 再入荷通知メール有効フラグがON
			if ((ProductCommon.IsSellTerm(drvSelectedProductVariation))
				&& ((string)drvSelectedProductVariation[Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG] == Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID)
				&& (OrderCommon.CheckProductStockBuyable(blUseVariation, blVariationSelected, dvProductVariationList, drvSelectedProductVariation)) == false)
			{
				// 再入荷通知
				strArrivalMailKbn = Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL;
			}
			// 販売期間開始前 かつ 販売開始通知メール有効フラグがON
			else if ((ProductCommon.IsSellBefore(drvSelectedProductVariation))
				&& ((string)drvSelectedProductVariation[Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG] == Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID))
			{
				// 販売開始通知
				strArrivalMailKbn = Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE;
			}
			// 販売期間終了後 かつ 再販売通知メール有効フラグがON
			else if ((ProductCommon.IsSellAfter(drvSelectedProductVariation))
				&& ((string)drvSelectedProductVariation[Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG] == Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID))
			{
				// 再販売通知
				strArrivalMailKbn = Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE;
			}

			return strArrivalMailKbn;
		}

		/// <summary>
		/// 入荷通知メール情報登録処理
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="strPcmobileKbn">PCモバイル区分</param>
		/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
		/// <param name="guestMailAddr">メールアドレス</param>
		public static int RegistUserProductArrivalMail(
			string strUserId,
			string strShopId,
			string strProductId,
			string strVariationId,
			string strPcmobileKbn,
			string strArrivalMailKbn,
			string guestMailAddr)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "RegistUserProductArrivalmail"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, strUserId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, strProductId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, strVariationId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN, strPcmobileKbn);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, strArrivalMailKbn);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED, GetExpiredDate(strArrivalMailKbn));
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR, guestMailAddr);

				return sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 入荷通知メール期限取得
		/// </summary>
		/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
		/// <returns></returns>
		public static DateTime GetExpiredDate(string strArrivalMailKbn)
		{
			DateTime dtDateExpired = new DateTime();
			switch (strArrivalMailKbn)
			{
				case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL:
					dtDateExpired = DateTime.Now.AddMonths(Constants.DATE_EXPIRED_ADD_MONTH_ARRIVAL);
					break;
				case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE:
					dtDateExpired = DateTime.Now.AddMonths(Constants.DATE_EXPIRED_ADD_MONTH_RELEASE);
					break;
				case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE:
					dtDateExpired = DateTime.Now.AddMonths(Constants.DATE_EXPIRED_ADD_MONTH_RESALE);
					break;
			}
			return DateTime.Parse(dtDateExpired.ToString("yyyy/MM/01")).AddSeconds(-1).AddMilliseconds(997);
		}

		/// <summary>
		/// 入荷通知メール情報更新処理
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="iMailNo">入荷通知メール枝番</param>
		/// <param name="strDateExpired">入荷通知メール通知期限</param>
		public static void UpdateUserProductArrivalMail(string strUserId, int iMailNo, string strDateExpired)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "UpdateUserProductArrivalMail"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, strUserId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO, iMailNo);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED, strDateExpired);
				
				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 入荷通知メール情報削除処理
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="iMailNo">入荷通知メール枝番</param>
		public static void DeleteUserProductArrivalMail(string strUserId, int iMailNo)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "DeleteUserProductArrivalMail"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, strUserId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO, iMailNo);

				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// ユーザのメール配信フラグチェック
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <returns>true:メール配信可 / false:メール配信不可</returns>
		public static bool CheckUserMailFlg(string strUserId)
		{
			var user = new UserService().Get(strUserId);
			return ((user != null) && (user.MailFlg == Constants.FLG_USER_MAILFLG_OK));
		}

		/// <summary>
		/// メールアドレス文字列取得
		/// </summary>
		/// <param name="strPcMobileKbn">PC・モバイル区分</param>
		/// <returns>メールアドレス文字列</returns>
		public static string CreateMailAddressNameFromTagReplacer(string strPcMobileKbn)
		{
			switch (strPcMobileKbn)
			{
				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC:
					return Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_addr.name@@");

				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE:
					return Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_addr2.name@@");
			}

			return "";
		}

		/// <summary>
		/// メールアドレス区分名文字列取得
		/// </summary>
		/// <param name="pcMobileKbn">PC・モバイル区分</param>
		/// <returns>メールアドレス区分名文字列</returns>
		public static string CreateMailAddressKbnNameFromTagReplacer(string pcMobileKbn)
		{
			switch (pcMobileKbn)
			{
				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC:
					return Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_addr.kbn_name@@");

				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE:
					return Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_addr2.kbn_name@@");
			}

			return "";
		}
	}
}
