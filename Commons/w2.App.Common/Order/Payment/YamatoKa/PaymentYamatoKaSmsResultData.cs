/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証結果POST通知データクラス(PaymentYamatoKaSmsResultData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Web;

namespace w2.App.Common.Order.Payment.YamatoKa
{
	/// <summary>
	/// ヤマト後払いSMS認証結果POST通知データクラス
	/// </summary>
	public class PaymentYamatoKaSmsResultData
	{
		/// <summary>認証結果</summary>
		public enum ResultEnum
		{
			/// <summary>SMS認証、有効性OK</summary>
			Ok = 0,
			/// <summary>SMS認証、有効性NG</summary>
			Ng = 1,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="req">リクエスト</param>
		public PaymentYamatoKaSmsResultData(HttpRequest req)
		{
			this.Datasource = new Hashtable();

			foreach (var k in req.Form.AllKeys)
			{
				this.Datasource.Add(k, req.Form[k]);
			}
		}

		/// <summary>データソース</summary>
		public Hashtable Datasource { get; set; }
		/// <summary>加盟店コード</summary>
		public string YcfStrNo
		{
			get { return (string)this.Datasource["ycfStrNo"]; }
		}
		/// <summary>受注番号</summary>
		public string PaymentOrderId
		{
			get { return (string)this.Datasource["juchuNo"]; }
		}
		/// <summary>電話番号</summary>
		public string Tel
		{
			get { return (string)this.Datasource["keitaiNo"]; }
		}
		/// <summary>送り先区分 3:SMS認証　4:SMS有効性</summary>
		public PaymentYamatoKaSendDiv SendDiv
		{
			get { return (PaymentYamatoKaSendDiv)int.Parse((string)this.Datasource["giftKb"]); }
		}
		/// <summary>審査結果</summary>
		public PaymentYamatoKaEntryResponseData.ResultValue Result
		{
			get { return (PaymentYamatoKaEntryResponseData.ResultValue)int.Parse((string)this.Datasource["snsKk"]); }
		}
		/// <summary>SMS認証結果</summary>
		public ResultEnum AuthResult
		{
			get { return (ResultEnum)int.Parse((string)this.Datasource["smsNinKk"]); }
		}
		/// <summary>SMS有効性結果</summary>
		public ResultEnum AvailableResult
		{
			get { return (ResultEnum)int.Parse((string)this.Datasource["smsYuuKk"]); }
		}
	}
}
