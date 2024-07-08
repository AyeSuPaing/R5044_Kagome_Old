/*
=========================================================================================================
  Module      : プロセスベースクラス (BaseProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.User;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Base
{
	///**************************************************************************************
	/// <summary>
	/// ベースプロセスクラス
	/// </summary>
	///**************************************************************************************
	public class BaseProcess
	{
		/// <summary>
		/// 文字列を改行ベースで配列化する
		/// </summary>
		/// <param name="strLine">文字列（改行あり）</param>
		/// <returns>改行ごとに区切られた文字配列</returns>
		protected static string[] ToArrayLineBreak(string strLine)
		{
			return strLine.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
		}

		/// <summary>
		/// メールヘッダ用文字列取得処理
		/// </summary>
		/// <param name="strMailHeaderString">メールヘッダ文字列</param>
		/// <returns>置換済み文字列</returns>
		public static string GetMailHeaderString(string strMailHeaderString)
		{
			return strMailHeaderString.Replace(" ", "").Replace("\r", "").Replace("\t", "").Replace("\n", "");
		}

		/// <summary>
		/// エラーメールを送信する
		/// </summary>
		/// <param name="strErrorMassage">エラーメッセージ</param>
		public static void ErrorMailSender(string strErrorMassage)
		{
			using (SmtpMailSender smsMailSend = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				smsMailSend.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSend.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSend.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSend.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSend.AddBcc(mail.Address));
				smsMailSend.SetBody("[取り込みエラー]" + DateTime.Now + "\r\n" + strErrorMassage);
				if (smsMailSend.SendMail() == false)
				{
					// 送信エラーの場合ログ書き込み
					FileLogger.WriteError(smsMailSend.MailSendException.ToString());
				}
			}
		}

		/// <summary>
		/// バリエーションIDから商品を取得する
		/// </summary>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="mallCooperationSetting">モール連携設定</param>
		/// <returns>商品情報</returns>
		protected static ProductModel GetProduct(string strShopId, string strVariationId, DataRowView mallCooperationSetting)
		{
			DataView dvVariation;
			if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION)
			{
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("Product", "GetProductVariationIdFromMallExProductId"))
				{
					var htInput = new Hashtable
					{
						{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, strShopId },
						{ Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID, strVariationId }
					};

					var skuManagemrntIdFieldNameForNormal = (string)mallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL];
					var skuManagemrntIdFieldNameForVariation = (string)mallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION];

					var replaces = new []
					{
						// モール連携設定で通常商品用のSKU管理番号フィールドが設定されている場合、そのフィールドを適用する
						// 設定されていない場合は「mall_ex_product_id」を使う
						new KeyValuePair<string, string>(
							"@@ rakuten_sku_management_id_field_for_normal @@",
							(string.IsNullOrEmpty(skuManagemrntIdFieldNameForNormal) == false) && (skuManagemrntIdFieldNameForNormal.Contains("\"") == false)
								? skuManagemrntIdFieldNameForNormal
								: Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID),
						// モール連携設定でバリエーション商品用のSKU管理番号フィールドが設定されている場合、そのフィールドを適用する
						// 設定されていない場合は「mall_variation_id1」を使う
						new KeyValuePair<string, string>(
							"@@ rakuten_sku_management_id_field_for_variation @@",
							(string.IsNullOrEmpty(skuManagemrntIdFieldNameForVariation) == false) && (skuManagemrntIdFieldNameForVariation.Contains("\"") == false)
								? skuManagemrntIdFieldNameForVariation
								: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1),
					};

					foreach (var replace in replaces)
					{
						sqlStatement.Statement = sqlStatement.Statement.Replace(replace.Key, replace.Value);
					}

					dvVariation = sqlStatement.SelectSingleStatement(
						sqlAccessor,
						htInput);
				}
			}
			else
			{
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("Product", "GetProductId"))
				{
					var htInput = new Hashtable
					{
						{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, strShopId },
						{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, strVariationId }
					};

					dvVariation = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
				}
			}

			if (dvVariation.Count == 0)
			{
				FileLogger.WriteError("商品バリエーションID" + strVariationId + "の商品情報を取得できませんでした。");
				return null;
			}

			return new ProductModel(dvVariation[0]);
		}

		/// <summary>
		/// モール拡張商品IDから商品IDを取得する
		/// </summary>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strMallExProductId">モール拡張商品ID</param>
		/// <returns>商品ID</returns>
		protected static string GetVariationIdFromMallExProductId(string strShopId, string strMallExProductId)
		{
			DataView dvVariation = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlstatement = new SqlStatement("Product", "GetVariationIdFromMallExProductId"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTVARIATION_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID, strMallExProductId);

				dvVariation = sqlstatement.SelectSingleStatement(sqlAccessor, htInput);
			}
			if (dvVariation.Count == 0)
			{
				FileLogger.WriteError("モール拡張商品ID + モールバリエーションID1 + モールバリエーションID2：" + strMallExProductId + "の商品情報を取得できませんでした。");
				return "";
			}

			return (string)dvVariation[0][0];
		}

		/// <summary>
		/// 登録済みユーザチェック
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMailAddr">メールアドレス</param>
		/// <param name="strMallId">モールID</param>
		/// <returns>登録済み：最初のユーザID、未登録：空文字</returns>
		protected static string CheckRegisteredUser(SqlAccessor sqlAccessor, string strMallId, string strMailAddr)
		{
			FileLogger.WriteInfo("[SQL START]ユーザの登録確認");

			string userId = new UserService().GetUserId(StringUtility.ToEmpty(strMallId), StringUtility.ToEmpty(strMailAddr), sqlAccessor);
			FileLogger.WriteInfo("[SQL END]ユーザの登録確認");

			// ユーザーIDが取れない場合Nullってるので、Emptyで返却
			return StringUtility.ToEmpty(userId);
		}

		/// <summary>
		/// 新ユーザーID発行
		/// </summary>
		/// <returns>新ユーザーID</returns>
		protected static string GetNewUserId()
		{
			string strResult = null;

			FileLogger.WriteInfo("[SQL START]ユーザID発番");
			
			strResult = UserService.CreateNewUserId(Constants.CONST_DEFAULT_SHOP_ID,
													Constants.NUMBER_KEY_USER_ID,
													Constants.CONST_USER_ID_HEADER,
													Constants.CONST_USER_ID_LENGTH);
			FileLogger.WriteInfo("[SQL END]ユーザID発番");

			return strResult;
		}

		/// <summary>
		/// メール取得判断
		/// </summary>
		/// <param name="subject">メール件名</param>
		/// <param name="errorSubjects">不要メール件名</param>
		/// <param name="successSubjects">取込メール件名</param>
		/// <param name="filePath">対象のファイルパス</param>
		/// <returns>取込可否</returns>
		protected static bool JudgmentGetMail(
			string subject,
			string[] errorSubjects,
			string[] successSubjects,
			string filePath)
		{
			const int FLG_MAIL_ERROR = 0;	// 不要メール
			const int FLG_MAIL_SUCCESS = 1; // 取込メール
			const int FLG_MAIL_UNKNOWN = 2; // 不明メール
			int getMailFlg = FLG_MAIL_UNKNOWN;

			// 不要メール判断
			foreach (string errorSubject in errorSubjects)
			{
				if (string.IsNullOrEmpty(subject) || (subject.IndexOf(errorSubject) != -1))
				{
					FileLogger.WriteInfo("不要なので削除：[" + subject + "][" + filePath + "]");
					File.Delete(filePath);
					getMailFlg = FLG_MAIL_ERROR;
					break;
				}
			}

			// 取込メール判断
			if (getMailFlg == FLG_MAIL_UNKNOWN)
			{
				foreach (string successSubject in successSubjects)
				{
					if (subject.IndexOf(successSubject) != -1)
					{
						getMailFlg = FLG_MAIL_SUCCESS;
						break;
					}
				}
			}

			// 不明メール判断
			if (getMailFlg == FLG_MAIL_UNKNOWN)
			{
				FileLogger.WriteInfo("不要なので移動：[" + subject + "][" + filePath + "]");
				File.Move(filePath, Path.Combine(Constants.PATH_UNKNOWN + @"\", Path.GetFileName(filePath)));
			}

			return (getMailFlg == FLG_MAIL_SUCCESS);
		}
	}
}
