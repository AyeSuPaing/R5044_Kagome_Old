/*
=========================================================================================================
  Module      : コマンドライン引数処理クラス(CommandlineArguments.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Extension;
using w2.ExternalAPI.Common.Logging;

namespace ExternalAPI
{
	/// <summary>
	/// コマンドライン引数処理クラス
	/// </summary>
	class CommandlineArguments
	{
		/// <summary>
		/// コマンドライン引数についての説明テキスト取得
		/// </summary>
		/// <returns></returns>
		public static string GetExplanation()
		{
			return
@"以下のコマンドライン引数を設定してください。パスは相対指定が可能です。
-setArgs
-apiID:[処理キー]
-target:[処理対象パス]
-apiType:[APIタイプ（入力:import、出力:export）]
-props:[処理内部で利用できるプロパティ。key1=value1;key2=value2;のように、＝でキーと値を結び、セミコロンで区切って複数指定できる] 
-work:[不必要。作業ディレクトリパス。デフォルトでは.\Work]
-success:[不必要。成功時ディレクトリパス。デフォルトでは.\Success]
-backup:[不必要。バックアップディレクトリパス。デフォルトでは.\Backup]
-error:[不必要。エラー時ディレクトリパス。デフォルトでは.\Error]
-writeLog:[不必要。APIログを出力するか？。デフォルトではtrue]
-useftpdownload:[不必要。インポート時にFTPダウンロードを利用するかどうか？利用する場合はtrue,しない場合はfalse。デフォルトではfalse]

例： ExternalAPI.exe -setArgs -apiID:SimpleCommandLibrary_E_0017 -target:out\out.csv -apiType:export -fileType:csv -props:flag1=true;value=12 -writeLog:true -useftpdownload:false
";
		}

		#region +GetTargetFromArgs 処理対象情報をコマンドライン引数から取得する

		/// <summary>
		/// API処理対象情報をコマンドライン引数から取得する
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		/// <returns>API処理対象情報</returns>
		public static ExecuteTarget GetTargetFromArgs(string[] args)
		{
			if (CheckArgs(args) == false)
			{
				string logData = "引数情報：";

				int i = 1;
				foreach (string arg in args)
				{
					logData += string.Format("{0}番目の引数：'{1}',", i.ToString(), arg);
					i++;
				}

				// 不正な引数の情報をログに
				ApiLogger.Write(LogLevel.fatal, "引数の指定が不正です。", logData);
				throw new Exception("引数の情報が不正です。:" + logData);
			}

			if (args[0] == "-setArgs")
				return GetTargetFromArgsWithNames(args);
			else
				return GetTargetFromArgsWithoutName(args);
		}

		/// <summary>
		/// API処理対象情報をコマンドライン引数から取得する（名称指定あり）
		/// </summary>
		/// <example>-setArgs -apiID:xxxExport -target:out\out.csv -apiType:export -fileType:csv -props:flag1=true;flag2;value=123 -writeLog:true -useftpdownload:false</example>
		/// <example>-setArgs -apiID:xxxExport -target:out\out.csv -apiType:export -fileType:csv -props:flag1=true;flag2;value=123 -work:work\work.csv -success:\suc -backup:\back -error:\err -writeLog:true -useftpdownload:false</example>
		/// <param name="args">コマンドライン引数</param>
		/// <returns>API処理対象</returns>
		static ExecuteTarget GetTargetFromArgsWithNames(string[] args)
		{
			// コロン区切りのキーペアとみなして、辞書型配列に変換
			var argDict = args.Convert2Dictionary(':', true);

			// 引数を分解してTarget生成
			return new ExecuteTarget(
				argDict["-apiid"],
				argDict["-target"],
				(APIType)Enum.Parse(typeof(APIType), argDict["-apitype"], true),
				argDict["-filetype"],
				argDict.ContainsKey("-work") ? argDict["-work"] : null,
				argDict.ContainsKey("-success") ? argDict["-success"] : null,
				argDict.ContainsKey("-error") ? argDict["-error"] : null,
				argDict.ContainsKey("-backup") ? argDict["-backup"] : null,
				argDict.ContainsKey("-props") ? argDict["-props"] : null,
				argDict.ContainsKey("-writelog") ? Convert.ToBoolean(argDict["-writelog"]) : true,
				argDict.ContainsKey("-useftpdownload") ? Convert.ToBoolean(argDict["-useftpdownload"]) : false);
		}
		/// <summary>
		/// API処理対象情報をコマンドライン引数から取得する（名称指定なし）
		/// </summary>
		/// <example>xxx_Import c:\logs\Import.csv c:\logs\Import_w.csv 0 csv</example>
		/// <param name="args">コマンドライン引数</param>
		/// <returns>API処理対象</returns>
		static ExecuteTarget GetTargetFromArgsWithoutName(string[] args)
		{
			// 引数を分解してTarget生成
			return new ExecuteTarget(
				args[0],
				args[1],
				args[2],
				(APIType)Convert.ToInt32(args[3]),
				args[4]);
		}
		#endregion

		#region -CheckArgs 引数チェック処理
		/// <summary>
		///	引数チェック処理
		/// </summary>
		/// <param name="args">コマンドライン引数 </param>
		static bool CheckArgs(string[] args)
		{
			// 引数の指定がない
			if (args == null) return false;

			// 引数の指定数が不正
			if (args[0] != "-setArgs" && args.GetLength(0) != 5) return false;

			return true;
		}
		#endregion
	}
}
