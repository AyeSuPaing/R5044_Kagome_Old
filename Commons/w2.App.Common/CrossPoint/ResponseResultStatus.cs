/*
=========================================================================================================
  Module      : CrossPoint API レスポンスの結果ステータスモデル (ResponseResultStatus.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// 結果ステータスモデル
	/// </summary>
	public class ResultStatus
	{
		/// <summary>エラー種別</summary>
		public enum ErrorType
		{
			Get,
			Insert,
			Update,
			Delete
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ResultStatus()
		{
			this.GetStatus = string.Empty;
			this.InsStatus = string.Empty;
			this.UpdStatus = string.Empty;
			this.DelStatus = string.Empty;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="error">エラーのタイプ</param>
		public ResultStatus(ErrorType error)
		{
			switch (error)
			{
				case ErrorType.Get:
					this.GetStatus = Constants.CROSS_POINT_RESULT_STATUS_ERROR;
					break;

				case ErrorType.Insert:
					this.InsStatus = Constants.CROSS_POINT_RESULT_STATUS_ERROR;
					break;

				case ErrorType.Update:
					this.UpdStatus = Constants.CROSS_POINT_RESULT_STATUS_ERROR;
					break;

				case ErrorType.Delete:
					this.DelStatus = Constants.CROSS_POINT_RESULT_STATUS_ERROR;
					break;
			}
		}

		/// <summary>取得結果</summary>
		[XmlElement("GetStatus")]
		public string GetStatus { get; set; }
		/// <summary>登録結果</summary>
		[XmlElement("InsStatus")]
		public string InsStatus { get; set; }
		/// <summary>更新結果</summary>
		[XmlElement("UpdStatus")]
		public string UpdStatus { get; set; }
		/// <summary>削除結果</summary>
		[XmlElement("DelStatus")]
		public string DelStatus { get; set; }
		/// <summary>エラーリスト</summary>
		[XmlElement("Error")]
		public Error[] Error { get; set; }
		/// <summary>実行結果</summary>
		public string Status
		{
			get
			{
				if (string.IsNullOrEmpty(this.GetStatus) == false) return this.GetStatus;
				if (string.IsNullOrEmpty(this.InsStatus) == false) return this.InsStatus;
				if (string.IsNullOrEmpty(this.UpdStatus) == false) return this.UpdStatus;
				if (string.IsNullOrEmpty(this.DelStatus) == false) return this.DelStatus;
				return string.Empty;
			}
		}
		/// <summary>API実行成功</summary>
		public bool IsSuccess
		{
			get
			{
				return (this.Status == Constants.CROSS_POINT_RESULT_STATUS_SUCCESS);
			}
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get
			{
				return (this.Error != null)
					? string.Join(Environment.NewLine, this.Error.Select(err => err.Message))
					: string.Empty;
			}
		}
		/// <summary>エラーコードリスト</summary>
		public string ErrorCodeList
		{
			get
			{
				return (this.Error != null)
					? "エラーコード：" + string.Join(",", this.Error.Select(error => error.Code))
					: string.Empty;
			}
		}
		/// <summary>リクエストパラメーター</summary>
		[XmlIgnore]
		public Dictionary<string, string> RequestParameter { get; set; }
		/// <summary>リクエストパラメーター(全て)</summary>
		public string RequestParameterAll
		{
			get
			{
				return string.Join(
					"\r\n",
					this.RequestParameter.Select(param => string.Format("{0} = {1}", param.Key, param.Value)));
			}
		}
		/// <summary>Error messages</summary>
		public string ErrorMessages
		{
			get
			{
				if (this.Error == null) return string.Empty;

				var result = string.Format(
					"エラーメッセージ：{0}",
					string.Join(", ", this.Error.Select(error => error.Message).ToArray()));
				return result;
			}
		}
	}
}
