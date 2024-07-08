/*
=========================================================================================================
  Module      : マクロキオスク用連携結果(MacroKioskResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Net.SMS.MicroKiosk
{
	/// <summary>
	/// マクロキオスク用連携結果
	/// </summary>
	public class MacroKioskResult : ISendResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private MacroKioskResult()
		{
			// 隠ぺいするためprivate
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">マクロキオスクから返却されたレスポンス値</param>
		public MacroKioskResult(string result)
			: this()
		{
			// resultを解析する
			this.IsSucccess = false;
			this.SuccessMessageID = "";

			if (result != null)
			{
				var res = result.Split(',');
				if (res.Length == 3)
				{
					if (res[2] == "200")
					{
						this.IsSucccess = true;
						this.SuccessMessageID = res[1];
					}
				}
			}
			this.ResultMessage = result;
		}

		/// <summary>成功したかどうか</summary>
		public bool IsSucccess { get; private set; }
		/// <summary>結果</summary>
		public string ResultMessage { get; private set; }
		/// <summary>成功したメッセージのID</summary>
		public string SuccessMessageID
		{
			private set;
			get;
		}
	}
}
