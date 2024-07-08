/*
=========================================================================================================
  Module      : CrossMall Api レスポンス基底クラス (CrossMallResponseBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml;
using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.CrossMall
{
	/// <summary>
	/// CrossMall Api レスポンス基底クラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class CrossMallResponseBase<T>
	{
		/// <summary>
		/// 結果セットを取得する
		/// </summary>
		/// <typeparam name="TResponse">レスポンスの型</typeparam>
		/// <param name="response">レスポンスデータ</param>
		/// <returns>レスポンスの結果セット</returns>
		public ResultSet<T> GetResultSet<TResponse>(string response)
			where TResponse : CrossMallResponseBase<T>
		{
			this.ResultSet = SerializeHelper.Deserialize<TResponse>(response).ResultSet;
			return this.ResultSet;
		}
		
		#region プロパティ
		/// <summary> 結果セット </summary>
		[XmlElement("ResultSet")]
		public ResultSet<T> ResultSet { get; set; }
		#endregion
	}

	public class ResultSet<T>
	{
		public ResultSet()
		{
			this.TotalResult = 0;
			this.ResultStatus = new ResultStatus();
			this.Results = new T[0];
		}
		
		#region プロパティ
		/// <summary> レスポンス数 </summary>
		[XmlAttribute("TotalResult")]
		public int TotalResult { get; set; }
		/// <summary> レスポンス結果のステータス </summary>
		[XmlElement("ResultStatus")]
		public ResultStatus ResultStatus { get; set; }
		/// <summary> レスポンス結果の配列 </summary>
		[XmlElement("Result")]
		public T[] Results { get; set; }
		#endregion
	}

	/// <summary>
	/// ステータスクラス
	/// </summary>
	public class ResultStatus
	{
		public ResultStatus()
		{
			this.GetSatus = CrossMallConstants.FLG_RESULT_STATUS_ERROR_VALUE;
			this.Message = string.Empty;
		}
		
		#region プロパティ
		/// <summary> 取得ステータス </summary>
		[XmlElement("GetStatus")]
		public string GetSatus { get; set; }
		/// <summary> ステータスメッセージ </summary>
		[XmlElement("Message")]
		public string Message { get; set; }
		/// <summary> 取得ステータスの結果フラグ </summary>
		public bool IsGetSuccess { get { return GetSatus == CrossMallConstants.FLS_RESULT_STATUS_SUCCESS_VALUE; } }
		#endregion
	}
}
