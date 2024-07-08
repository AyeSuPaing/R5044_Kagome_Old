/*
=========================================================================================================
  Module      : 引数クラス(Argument.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;

namespace w2.Commerce.Reauth
{
	/// <summary>
	/// 引数クラス
	/// </summary>
	public class Argument
	{
		/// <summary>実行モード</summary>
		protected enum ExecMode
		{
			/// <summary>再与信</summary>
			Reauth,
			/// <summary>与信のみ</summary>
			AuthOnly,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="args">引数</param>
		public Argument(string[] args)
		{
			SetArgment(args);
			CheckArgument();
		}

		/// <summary>
		/// 引数セット
		/// </summary>
		/// <param name="args">引数</param>
		private void SetArgment(string[] args)
		{
			this.Args = args;
			this.TargetDate = DateTime.Today.Date;
			this.Mode = ExecMode.Reauth;
			this.ExtendStatusNumber = "";

			// 引数なし
			if (args.Length == 0)
			{
				return;
			}

			DateTime targetDate;
			// 引数1つの場合は旧バージョンの可能性
			if ((args.Length == 1) && DateTime.TryParse(args[0], out targetDate))
			{
				this.TargetDate = targetDate.Date;
			}

			// 引数セット
			foreach (var arg in args)
			{
				var argStrings = arg.Split(':');

				switch (argStrings[0])
				{
					case "-d":
						DateTime tmpDate;
						if (DateTime.TryParse(argStrings[1], out tmpDate))
						{
							this.TargetDate = tmpDate;
						}
						break;

					case "-m":
						ExecMode tmpMode;
						if (Enum.TryParse(argStrings[1], true, out tmpMode))
						{
							this.Mode = tmpMode;
						}
						break;

					case "-e":
						this.ExtendStatusNumber = argStrings[1];
						break;
				}
			}
		}

		/// <summary>
		/// 引数チェック
		/// </summary>
		private void CheckArgument()
		{
			if (this.IsAuthOnly && string.IsNullOrEmpty(this.ExtendStatusNumber))
			{
				this.ErrorMessage = "引数エラー：与信のみの場合は拡張ステータス番号を指定してください";
			}

			if (this.IsAuthOnly && (string.IsNullOrEmpty(this.ExtendStatusNumber) == false))
			{
				int tmpNumber;
				if ((int.TryParse(this.ExtendStatusNumber, out tmpNumber) == false)
					|| (this.ExtendStatusNumber != tmpNumber.ToString())
					|| (tmpNumber <= 0)
					|| (tmpNumber > Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX))
				{
					this.ErrorMessage = "引数エラー：拡張ステータス番号は有効なものを指定してください";
				}
			}
		}

		/// <summary>引数</summary>
		public string[] Args { get; private set; }
		/// <summary>動作モード</summary>
		private ExecMode Mode { get; set; }
		/// <summary>対象日付</summary>
		public DateTime TargetDate { get; private set; }
		/// <summary>拡張ステータス番号</summary>
		public string ExtendStatusNumber { get; private set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; private set; }
		/// <summary>与信のみか？</summary>
		public bool IsAuthOnly
		{
			get { return (this.Mode == ExecMode.AuthOnly); }
		}
		/// <summary>引数文字列</summary>
		public string ArgumentString
		{
			get { return string.Join(" ", this.Args); }
		}
	}
}
