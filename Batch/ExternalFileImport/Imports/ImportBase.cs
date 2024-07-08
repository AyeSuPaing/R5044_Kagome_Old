/*
=========================================================================================================
  Module      : データ取込処理基底クラス(ImportBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Util;

namespace w2.Commerce.Batch.ExternalFileImport.Imports
{
	abstract class ImportBase
	{
		/// <summary>値定義</summary>
		Hashtable m_htParamDefine = new Hashtable();

		/// <summary>店舗ID</summary>
		protected string m_strShopId = null;

		/// <summary>ファイルタイプ</summary>
		protected string m_strFileType = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected ImportBase(string strShopId, string strFileType)
		{
			m_strShopId = strShopId;
			m_strFileType = strFileType;

			//------------------------------------------------------
			// 値定義読み込み
			//------------------------------------------------------
			XmlDocument xdParamDefine = new XmlDocument();
			var path = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + Constants.FILE_XML_PARAMDEFINE;
			xdParamDefine.Load(path);

			XmlNodeList xnlColumns = xdParamDefine.SelectSingleNode("/ParamDefineSetting/" + m_strFileType).ChildNodes;
			foreach (XmlNode xnColumn in xnlColumns)
			{
				if (xnColumn.NodeType == XmlNodeType.Comment)
				{
					continue;
				}

				Hashtable htParam = new Hashtable();
				foreach (XmlNode xnDefine in xnColumn.ChildNodes)
				{
					if (xnDefine.NodeType == XmlNodeType.Comment)
					{
						continue;
					}

					Dictionary<string, string> dicAttributes = new Dictionary<string, string>();
					dicAttributes.Add("value", (xnDefine.Attributes["value"] != null) ? xnDefine.Attributes["value"].Value : "");
					dicAttributes.Add("payment_kbn", (xnDefine.Attributes["payment_kbn"] != null) ? xnDefine.Attributes["payment_kbn"].Value : "");
					dicAttributes.Add("payment_status", (xnDefine.Attributes["payment_status"] != null) ? xnDefine.Attributes["payment_status"].Value : "");

					var nameAttr = xnDefine.Attributes["name"];
					htParam.Add(nameAttr != null ? nameAttr.Value : "", dicAttributes);
				}

				m_htParamDefine.Add(xnColumn.Name, htParam);
			}
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="strActiveFilePath">取込ファイルパス</param>
		/// <returns>取込件数</returns>
		abstract public int Import(string strActiveFilePath);

		/// <summary>
		/// 値コンバート
		/// </summary>
		/// <param name="strFieldName">フィールド名</param>
		/// <param name="strValue">コンバートする値</param>
		/// <param name="strAttribute">アトリビュートの値</param>
		/// <returns>コンバート後の値</returns>
		public string ConvertValue(string strFieldName, string strValue, string strAttribute)
		{
			string strResult = null;

			Hashtable htDefine = (Hashtable)m_htParamDefine[strFieldName];
			if ((htDefine != null) && (htDefine.Count != 0))
			{
				strResult = ((Dictionary<string, string>)htDefine[strValue])[strAttribute];
			}
			else
			{
				throw new ApplicationException("値のコンバートに失敗しました。(フィールド名：" + strFieldName + "、値：" + strValue + "、属性：" + strAttribute + "）");
			}

			return StringUtility.ToEmpty(strResult);
		}

		/// <summary>
		/// 郵便番号加工
		/// </summary>
		/// <param name="orderId">>注文ID</param>
		/// <param name="baseZip">加工前郵便番号</param>
		/// <returns>加工後郵便番号</returns>
		protected string ProcessingZip(string orderId, string baseZip)
		{
			if (Regex.IsMatch(baseZip, "[0-9]{3}-[0-9]{4}"))
			{
				return baseZip;
			}
			else if (baseZip.Length > 4)
			{
				baseZip = baseZip.Replace("-", string.Empty);

				return string.Format(
					"{0}-{1}",
					baseZip.Substring(0, 3),
					baseZip.Substring(3));
			}
			else
			{
				var errorMessage = string.Format("郵便番号が正しくありません。(order_id:{0})", orderId);
				throw new ApplicationException(errorMessage);
			}
		}

		/// <summary>
		/// 電話番号加工
		/// </summary>
		/// <param name="orderId">>注文ID</param>
		/// <param name="baseTel">加工前電話番号</param>
		/// <returns>加工後電話番号</returns>
		protected string[] ProcessingTel(string orderId, string baseTel)
		{
			return ProcessingTelFormated(orderId, baseTel).Split("-".ToCharArray());
		}

		/// <summary>
		/// 電話番号加工
		/// </summary>
		/// <param name="orderId">>注文ID</param>
		/// <param name="baseTel">加工前電話番号</param>
		/// <returns>加工後電話番号</returns>
		protected string ProcessingTelFormated(string orderId, string baseTel)
		{
			if (Regex.IsMatch(baseTel, "[0-9]+-[0-9]+-[0-9]+"))
			{
				return baseTel;
			}
			else if (baseTel.Replace("-", string.Empty).Length > 7)
			{
				baseTel = baseTel.Replace("-", string.Empty);

				return string.Format(
					"{0}-{1}-{2}",
					baseTel.Substring(0, baseTel.Length - 8),
					baseTel.Substring(baseTel.Length - 8, 4),
					baseTel.Substring(baseTel.Length - 4, 4));
			}
			else
			{
				var errorMessage = string.Format("電話番号が正しくありません。(order_id:{0})", orderId);
				throw new ApplicationException(errorMessage);
			}
		}

		/// <summary>
		/// 日時型変換加工
		/// </summary>
		/// <param name="baseDateTime">加工前日時</param>
		/// <returns>加工後日時</returns>
		protected string ProcessingDateTime(string baseDateTime)
		{
			return baseDateTime
				.Replace("年", "/")
				.Replace("月", "/")
				.Replace("日", string.Empty);
		}
	}
}
