/*
=========================================================================================================
  Module      : メール送信スレッドクラス 絵文字、デコメ画像変換部分(MailSenderThread_Decome.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net.Mime;
using w2.Common.Sql;
using w2.Common.Net.Mail;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	partial class MailSenderThread : BaseThread
	{
		/// <summary>ドコモ デフォルトEncofing</summary>
		readonly static Encoding DEFAULT_ENDOCING_DOCOMO = Encoding.GetEncoding("Shift_JIS");
		/// <summary>ドコモ デフォルトTransferEncoding</summary>
		readonly static TransferEncoding DEFAULT_TRANSFERENDOCING_DOCOMO = TransferEncoding.Base64;
		/// <summary>AU デフォルトEncofing</summary>
		readonly static Encoding DEFAULT_ENDOCING_AU = Encoding.GetEncoding("Shift_JIS");
		/// <summary>AU デフォルトTransferEncoding</summary>
		readonly static TransferEncoding DEFAULT_TRANSFERENDOCING_AU = TransferEncoding.Base64;
		/// <summary>Softbank デフォルトEncofing</summary>
		readonly static Encoding DEFAULT_ENDOCING_SOFTBANK = Encoding.GetEncoding("utf-8");
		/// <summary>Softbank デフォルトTransferEncoding</summary>
		readonly static TransferEncoding DEFAULT_TRANSFERENDOCING_SOFTBANK = TransferEncoding.Base64;

		//=========================================================================================
		/// <summary>
		/// 絵文字、デコメ画像変換
		/// </summary>
		/// <param name="sbSubject">件名（値が変換されます）</param>
		/// <param name="sbBody">テキスト本文（値が変換されます）</param>
		/// <param name="sbBodyHtml">HTML本文（値が変換されます）</param>
		/// <param name="drvTargetData">抽出済ターゲットデータ</param>
		/// <param name="smsMailSender">メールセンダーインスタンス/param>
		//=========================================================================================
		public void DecomeConvert(StringBuilder sbSubject, StringBuilder sbBody, StringBuilder sbBodyHtml, DataRowView drvTargetData, SmtpMailSender smsMailSender)
		{
			//------------------------------------------------------
			// モバイルキャリア別エンコーディング設定
			//------------------------------------------------------
			string strMobileCareerId = null;
			XmlDocument xdMobileDomains = new XmlDocument();
			xdMobileDomains.Load(AppDomain.CurrentDomain.BaseDirectory + @"Xml\Setting\MobileDomainSetting.xml");
			foreach (XmlNode xnDomain in xdMobileDomains.SelectNodes("MobileDomainSetting/Domain"))
			{
				// メールアドレスからキャリアを判別
				if (((string)drvTargetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR]).Contains(xnDomain.InnerText)
					&& (xnDomain.Attributes["CareerId"] != null))
				{
					strMobileCareerId = xnDomain.Attributes["CareerId"].Value;
					break;
				}
			}

			switch (strMobileCareerId)
			{
				case CAREER_DOCOMO:
					smsMailSender.SetEncoding(DEFAULT_ENDOCING_DOCOMO, DEFAULT_TRANSFERENDOCING_DOCOMO);
					smsMailSender.Message.MultipartRelatedEnable = (sbBodyHtml.Length != 0);
					break;

				case CAREER_SOFTBANK:
					smsMailSender.SetEncoding(DEFAULT_ENDOCING_SOFTBANK, DEFAULT_TRANSFERENDOCING_SOFTBANK);
					smsMailSender.Message.MultipartRelatedEnable = (sbBodyHtml.Length != 0);
					break;

				case CAREER_AU:
					smsMailSender.SetEncoding(DEFAULT_ENDOCING_AU, DEFAULT_TRANSFERENDOCING_AU);
					smsMailSender.Message.MultipartRelatedEnable = false;
					break;

				default:
					smsMailSender.SetEncoding(Constants.MOBILE_MAIL_ENCODING, Constants.MOBILE_MAIL_TRANSFER_ENCODING);
					smsMailSender.Message.MultipartRelatedEnable = false;
					break;
			}

			//------------------------------------------------------
			// 絵文字コード取得
			//------------------------------------------------------
			MatchCollection mcPictorialSymbolSubject = Regex.Matches(sbSubject.ToString(), "<@@emoji:((?!@@>).)*@@>");
			MatchCollection mcPictorialSymbolBody = Regex.Matches(sbBody.ToString(), "<@@emoji:((?!@@>).)*@@>");
			MatchCollection mcPictorialSymbolBodyHtml = Regex.Matches(sbBodyHtml.ToString(), "<@@emoji:((?!@@>).)*@@>");
			MatchCollection mcImageTagBodyHtml = Regex.Matches(sbBodyHtml.ToString(), "<@@image:((?!@@>).)*@@>");

			if (mcPictorialSymbolSubject.Count + mcPictorialSymbolBody.Count + mcPictorialSymbolBodyHtml.Count > 0)
			{
				// SQL発行用絵文字タグリスト作成
				List<string> lSymbolTags = new List<string>();
				foreach (Match mSubject in mcPictorialSymbolSubject)
				{
					string strSymbolId = mSubject.Value.Replace("<@@emoji:", "").Replace("@@>", "");
					if (lSymbolTags.Contains(strSymbolId) == false)
					{
						lSymbolTags.Add(strSymbolId);
					}
				}
				foreach (Match mBody in mcPictorialSymbolBody)
				{
					string strSymbolId = mBody.Value.Replace("<@@emoji:", "").Replace("@@>", "");
					if (lSymbolTags.Contains(strSymbolId) == false)
					{
						lSymbolTags.Add(strSymbolId);
					}
				}
				foreach (Match mBodyHtml in mcPictorialSymbolBodyHtml)
				{
					string strSymbolId = mBodyHtml.Value.Replace("<@@emoji:", "").Replace("@@>", "");
					if (lSymbolTags.Contains(strSymbolId) == false)
					{
						lSymbolTags.Add(strSymbolId);
					}
				}

				StringBuilder sbSymbolTags = new StringBuilder();
				foreach (string strSymbolId in lSymbolTags)
				{
					if (sbSymbolTags.Length != 0)
					{
						sbSymbolTags.Append(",");
					}
					sbSymbolTags.Append("'").Append(strSymbolId.Replace("'", "''")).Append("'");
				}

				// 使用されている絵文字コードだけ取得
				DataView dvMobilePictorialSymbol = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("MobilePictorialSymbol", "GetMobilePictorialSymbolCode"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MOBILEGROUP_DEPT_ID, Constants.CONST_DEFAULT_SHOP_ID);

					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ symbol_tags @@", sbSymbolTags.ToString());
					dvMobilePictorialSymbol = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				// キャリア別絵文字コード取得
				Dictionary<string, string> dicMobilePictrialSymbol = new Dictionary<string, string>();
				switch (strMobileCareerId)
				{
					case CAREER_DOCOMO:
						foreach (DataRowView drv in dvMobilePictorialSymbol)
						{
							string strSymbol = "";
							if (((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE1]).Length >= 4)
							{
								byte[] bCode = { byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE1]).Substring(0, 2), System.Globalization.NumberStyles.HexNumber), 
																	 byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE1]).Substring(2, 2), System.Globalization.NumberStyles.HexNumber) };
								strSymbol = DEFAULT_ENDOCING_DOCOMO.GetString(bCode);
							}
							else
							{
								// コードが不完全か存在しない場合、絵文字名で変換
								strSymbol = "[" + (string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_NAME] + "]";
							}

							dicMobilePictrialSymbol.Add(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_TAG]).ToLower(), strSymbol);
						}
						break;

					case CAREER_SOFTBANK:
						foreach (DataRowView drv in dvMobilePictorialSymbol)
						{
							string strSymbol = "";
							if (((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE5]).Length >= 6)
							{
								byte[] bCode = { byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE5]).Substring(0, 2), System.Globalization.NumberStyles.HexNumber), 
																	 byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE5]).Substring(2, 2), System.Globalization.NumberStyles.HexNumber), 
																	 byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE5]).Substring(4, 2), System.Globalization.NumberStyles.HexNumber) };
								strSymbol = DEFAULT_ENDOCING_SOFTBANK.GetString(bCode);
							}
							else
							{
								// コードが不完全か存在しない場合、絵文字名で変換
								strSymbol = "[" + (string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_NAME] + "]";
							}

							dicMobilePictrialSymbol.Add(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_TAG]).ToLower(), strSymbol);
						}
						break;

					case CAREER_AU:
						foreach (DataRowView drv in dvMobilePictorialSymbol)
						{
							string strSymbol = "";
							if (((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE4]).Length >= 4)
							{
								byte[] bCode = { byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE4]).Substring(0, 2), System.Globalization.NumberStyles.HexNumber), 
																	 byte.Parse(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE4]).Substring(2, 2), System.Globalization.NumberStyles.HexNumber) };
								strSymbol = DEFAULT_ENDOCING_AU.GetString(bCode);
							}
							else
							{
								// コードが不完全か存在しない場合、絵文字名で変換
								strSymbol = "[" + (string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_NAME] + "]";
							}

							dicMobilePictrialSymbol.Add(((string)drv[Constants.FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_TAG]).ToLower(), strSymbol);
						}
						break;
				}

				//------------------------------------------------------
				// 絵文字置換処理
				//------------------------------------------------------
				foreach (Match mSubject in mcPictorialSymbolSubject)
				{
					string strKey = mSubject.Value.ToLower().Replace("<@@emoji:", "").Replace("@@>", "");
					string strValue = (dicMobilePictrialSymbol.ContainsKey(strKey)) ? dicMobilePictrialSymbol[strKey] : "";
					sbSubject.Replace(mSubject.Value, strValue);
				}
				foreach (Match mBody in mcPictorialSymbolBody)
				{
					string strKey = mBody.Value.ToLower().Replace("<@@emoji:", "").Replace("@@>", "");
					string strValue = (dicMobilePictrialSymbol.ContainsKey(strKey)) ? dicMobilePictrialSymbol[strKey] : "";
					sbBody.Replace(mBody.Value, strValue);
				}
				foreach (Match mBodyHtml in mcPictorialSymbolBodyHtml)
				{
					string strKey = mBodyHtml.Value.ToLower().Replace("<@@emoji:", "").Replace("@@>", "");
					string strValue = (dicMobilePictrialSymbol.ContainsKey(strKey)) ? dicMobilePictrialSymbol[strKey] : "";
					sbBodyHtml.Replace(mBodyHtml.Value, strValue);
				}
			}

			//------------------------------------------------------
			// デコメ用画像変換
			//------------------------------------------------------
			if (mcImageTagBodyHtml.Count > 0)
			{
				// 重複排除
				List<string> lMatchCollectionImageTags = new List<string>();
				foreach (Match mBodyHtml in mcImageTagBodyHtml)
				{
					if (lMatchCollectionImageTags.Contains(mBodyHtml.Value) == false)
					{
						lMatchCollectionImageTags.Add(mBodyHtml.Value);
					}
				}

				// DecomeAttachmentFileの中身を消去
				smsMailSender.DecomeAttachmentFile.Clear();

				foreach (string strImageTags in lMatchCollectionImageTags)
				{
					string strFileName = strImageTags.Replace("<@@image:", "").Replace("@@>", "");
					string strDecomeImageTag = "";

					if (File.Exists(Constants.PHYSICALDIRPATH_DECOMEIMAGE + strFileName))
					{
						DecomeAttachment daDecomeImage = new DecomeAttachment(Constants.PHYSICALDIRPATH_DECOMEIMAGE + strFileName);
						smsMailSender.DecomeAttachmentFile.Add(daDecomeImage);
						strDecomeImageTag = "<img src=\"cid:" + daDecomeImage.FileContentId + "\">"; // Imageタグの準備

						// AUの場合、画像のContentDispositionをAttachmentにするのが推奨されている
						if (strMobileCareerId == CAREER_AU)
						{
							daDecomeImage.FileContentDisposition = DecomeAttachment.ContentDisposition.Attachment;
						}
					}

					sbBodyHtml.Replace(strImageTags, strDecomeImageTag);
				}
			}
		}
	}
}