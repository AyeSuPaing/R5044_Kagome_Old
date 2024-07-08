/*
=========================================================================================================
  Module      : CrossPoint API レスポンスの基底クラス (ResponseBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using w2.App.Common.CrossPoint.Helper;
using w2.Common.Helper;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// CrossPoint API レスポンスの基底クラス
	/// </summary>
	/// <typeparam name="TResult">結果セットのモデル</typeparam>
	public abstract class ResponseBase<TResult>
	{
		/// <summary>
		/// XmlElement[]の中から指定した名前のXmlElementの値を取得する(要素が1つの場合のみ)
		/// </summary>
		/// <param name="elements">要素リスト</param>
		/// <param name="name">要素名</param>
		/// <returns>XmlElementの値</returns>
		protected static string GetElementsByName(XmlElement[] elements, string name)
		{
			var result = elements
				.Where(element => (name == element.Name))
				.Select(element => (element.FirstChild == null) ? string.Empty : element.FirstChild.Value)
				.FirstOrDefault();

			return StringUtility.ToEmpty(result);
		}

		/// <summary>
		/// 結果をセット
		/// </summary>
		/// <typeparam name="TResponse">レスポンスの型</typeparam>
		/// <param name="response">レスポンスデータ</param>
		/// <returns>結果セット</returns>
		public virtual ResultSet<TResult> GetResultSet<TResponse>(string response)
			where TResponse : ResponseBase<TResult>
		{
			this.ResultSet = SerializeHelper.Deserialize<TResponse>(response).ResultSet;
			this.ResultSet.XmlResponse = response;
			return this.ResultSet;
		}

		/// <summary>メッセージ本体</summary>
		[XmlElement("ResultSet")]
		public ResultSet<TResult> ResultSet { get; set; }
	}
}
