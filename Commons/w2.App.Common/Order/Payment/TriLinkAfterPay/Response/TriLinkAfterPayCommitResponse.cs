/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文確定依頼レスポンスクラス(TriLinkAfterPayCommitResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文確定依頼レスポンスクラス
	/// </summary>
	public class TriLinkAfterPayCommitResponse : ResponseBase
	{
		/// <summary>ログ区分</summary>
		public override string NameForLog
		{
			get { return "Commit"; }
		}
		/// <summary>レスポンス結果</summary>
		public override bool ResponseResult
		{
			get { return this.IsHttpStatusCodeOK; }
		}
	}
}