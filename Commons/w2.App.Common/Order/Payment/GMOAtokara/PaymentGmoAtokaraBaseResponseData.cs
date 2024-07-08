/*
=========================================================================================================
  Module      : GMOアトカラ レスポンスデータ基底クラス(PaymentGmoAtokaraBaseResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ レスポンスデータ基底クラス
	/// </summary>
	public abstract class PaymentGmoAtokaraBaseResponseData
	{
		/// <summary>レスポンスXML</summary>
		private XDocument _responseXml = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentGmoAtokaraBaseResponseData()
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public virtual void LoadXml(XDocument responseXml)
		{
			_responseXml = responseXml;

			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "result":
						this.Result = element.Value;
						break;

					case "errors":
						this.Errors = LoadXmlErrorsElement(element);
						break;
				}
			}
		}

		/// <summary>
		/// レスポンスよりエラーを取得
		/// </summary>
		/// <param name="errorsElements">エラーエレメント</param>
		/// <returns>エラーエレメント</returns>
		protected ErrorsElement LoadXmlErrorsElement(XElement errorsElements)
		{
			var result = new ErrorsElement();

			foreach (var errorsElement in errorsElements.Elements())
			{
				switch (errorsElement.Name.ToString())
				{
					case "errorCode":
						result.ErrorCode = errorsElement.Value;
						break;

					case "errorMessage":
						result.ErrorMessage = errorsElement.Value;
						break;
				}
			}
			return result;
		}

		/// <summary>処理結果ステータス</summary>
		public string Result { get; protected set; }
		/// <summary>エラー</summary>
		public ErrorsElement Errors { get; protected set; }

		/// <summary>
		/// エラーエレメント
		/// </summary>
		public class ErrorsElement
		{
			/// <summary>
			/// エラーメッセージ取得
			/// </summary>
			/// <returns>エラーメッセージ</returns>
			public string GetErrorMessage()
			{
				var result = $"{this.ErrorCode}:{this.ErrorMessage}";
				return result;
			}
			/// <summary>エラーコード</summary>
			public string ErrorCode { get; set; }
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessage { get; set; }
		}
	}
}
