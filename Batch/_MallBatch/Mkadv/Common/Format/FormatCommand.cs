/*
=========================================================================================================
  Module      : フォーマットコマンド(FormatCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.MallBatch.Mkadv.Common.Format
{
	///**************************************************************************************
	/// <summary>
	/// フォーマットコマンド
	/// </summary>
	///**************************************************************************************
	public class FormatCommand
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FormatCommand()
		{
			// プロパティ初期化
			this.CommandType = new CommandType();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="commandType">書式文字列（=range）</param>
		/// <param name="strKeyName">キー名</param>
		/// <param name="strArg1">引数１</param>
		/// <param name="strArg2">引数２</param>
		/// <param name="strOutput">出力内容</param>
		/// <remarks>命令引数を二つ受け取る（commandType = range の場合）</remarks>
		public FormatCommand(CommandType commandType,
							 string strKeyName,
							 string strArg1,
							 string strArg2,
							 string strOutput) : this()
		{
			this.CommandType = commandType;
			this.KeyName = strKeyName;
			this.Output = strOutput;
			this.Arg1 = strArg1;
			this.Arg2 = strArg2;
			SetIsFlgKey();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="commandType">書式文字列（bigger,biggerEqual,smaller,smallerEqual,equal,notEqual）</param>
		/// <param name="strKeyName">キー名</param>
		/// <param name="strArg">引数</param>
		/// <param name="strOutput">出力内容</param>
		/// <remarks>命令引数を一つだけ受け取る（比較演算）</remarks>
		public FormatCommand(CommandType commandType,
							 string strKeyName,
							 string strArg,
							 string strOutput) : this()
		{
			this.CommandType = commandType;
			this.KeyName = strKeyName;
			this.Output = strOutput;
			this.Arg1 = strArg;
			SetIsFlgKey();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="commandType">data（staticString）</param>
		/// <param name="strVal">キー名（データ本体）</param>
		/// <remarks>命令引数の不要</remarks>
		public FormatCommand(CommandType commandType, string strVal) : this()
		{
			if (commandType == CommandType.Data)
			{
				this.CommandType = commandType;
				this.KeyName = strVal;
			}
			else if (commandType == CommandType.Tag)
			{
				this.CommandType = commandType;
				this.KeyName = strVal.Replace("[SP:", "").Replace("]", "");
				this.Output = strVal;
				SetIsFlgKey();
			}
			else
			{
				this.CommandType = CommandType.StaticString;
				this.Output = strVal;
				SetIsFlgKey();
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>コマンドタイプを指定する（分岐終了命令用）</remarks>
		public FormatCommand(CommandType commandType) : this()
		{
			this.CommandType = commandType;
		}

		/// <summary>
		/// キー名フラグを立てる
		/// </summary>
		private void SetIsFlgKey()
		{
			// ダブルクォートを検索
			if (this.Output.IndexOf('"') != -1)
			{
				// 見つかったら固定文字列
				this.IsFlgKey = false;

				// 先頭と末尾にダブルクォートがあれば取り除く
				char[] cOutputs = this.Output.ToCharArray();
				if ((cOutputs[0] == ('"')) && (cOutputs[cOutputs.Length - 1] == ('"')) && (cOutputs.Length > 2))
				{
					this.Output = this.Output.Substring(1, this.Output.Length - 2);
				}
			}
			else
			{
				// 見つからなければキー名
				this.IsFlgKey = true;
			}
			// ダブルクォートを検索
			if ((this.Arg1 != null) && (this.Arg1.IndexOf('"') != -1))
			{
				this.Arg1 = this.Arg1.Replace("\"", "");
			}
			// ダブルクォートを検索
			if ((this.Arg2 != null) && (this.Arg2.IndexOf('"') != -1))
			{
				this.Arg2 = this.Arg2.Replace("\"", "");
			}
		}

		/// <summary>命令</summary>
		public CommandType CommandType { get; private set; }

		/// <summary>キーデータ名</summary>
		public string KeyName { get; private set; }

		/// <summary>出力内容フラグ</summary>
		public bool IsFlgKey { get; set; }

		/// <summary>出力内容（true：出力内容はデータのキー名、false：出力内容は固定値）</summary>
		public string Output { get; private set; }

		/// <summary>引数１</summary>
		public string Arg1 { get; private set; }

		/// <summary>引数２（省略可）</summary>
		public string Arg2 { get; private set; }
	}
}
