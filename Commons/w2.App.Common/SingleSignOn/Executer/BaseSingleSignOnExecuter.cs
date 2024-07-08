/*
=========================================================================================================
  Module      : シングルサインオン基底実行クラス(BaseSingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.SingleSignOn.Executer
{
	/// <summary>
	/// シングルサインオン基底実行クラス
	/// </summary>
	public abstract class BaseSingleSignOnExecuter : ISingleSignOnExecuter
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal BaseSingleSignOnExecuter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">HTTPコンテンツ</param>
		internal BaseSingleSignOnExecuter(
			HttpContext context)
		{
			this.Context = context;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// シングルサインオン実行
		/// </summary>
		/// <returns>シングルサインオン結果</returns>
		public SingleSignOnResult Execute()
		{
			OnBeforeExecute();
			var result = OnExecute();
			OnAfterExecute(result);
			return result;
		}

		/// <summary>
		/// シングルサインオン実行
		/// </summary>
		/// <returns>シングルサインオン結果</returns>
		protected abstract SingleSignOnResult OnExecute();

		/// <summary>シングルサインオン実行前（ログ書いたりとかできる）</summary>
		protected virtual void OnBeforeExecute()
		{
		}

		/// <summary>
		/// シングルサインオン実行後（ログ書いたりとかできる）
		/// </summary>
		/// <param name="result">シングルサインオン結果</param>
		protected virtual void OnAfterExecute(SingleSignOnResult result)
		{
			var postParameters = (this.Context.Request.Form != null)
				? string.Join(
					"&",
					this.Context.Request.Form.AllKeys.Select(
						p => string.Format("{0}={1}", p, StringUtility.ToEmpty(this.Context.Request[p]))))
				: "";
			var getParameters = (this.Context.Request.QueryString != null)
				? string.Join(
					"&",
					this.Context.Request.QueryString.AllKeys.Select(
						p => string.Format("{0}={1}", p, StringUtility.ToEmpty(this.Context.Request[p]))))
				: "";

			var messages = string.Format(
				"result:{0},post_parameters:{1},get_parameters:{2},message:{3}",
				result.SingleSignOnDetail.ToString(),
				postParameters,
				getParameters,
				result.Messages);

			FileLogger.Write("singlesignon", messages);
		}

		/// <summary>
		/// チェックコードを作成(SHA256)
		/// </summary>
		/// <param name="stringToHash">ハッシュ化する文字列</param>
		/// <returns>SHA256でハッシュ化した文字列</returns>
		protected static string CreateCheckCode(string stringToHash)
		{
			var bytes = Encoding.UTF8.GetBytes(stringToHash);
			var numArray = new SHA256CryptoServiceProvider().ComputeHash(bytes);

			var hashedText = new StringBuilder();
			foreach (var num in numArray)
			{
				hashedText.AppendFormat("{0:X2}", num);
			}
			return hashedText.ToString();
		}

		#endregion

		#region プロパティ
		/// <summary>HTTPコンテキスト</summary>
		public HttpContext Context { get; private set; }
		#endregion
	}
}