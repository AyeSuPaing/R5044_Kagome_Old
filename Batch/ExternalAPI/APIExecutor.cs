/*
=========================================================================================================
  Module      : 連携API実行クラス(APIExecutor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using w2.App.Common;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Export;
using w2.ExternalAPI.Common.FrameWork.File;
using w2.ExternalAPI.Common.Import;

namespace ExternalAPI
{
	/// <summary>
	///	連携API実行クラス
	/// </summary>
	/// <remarks>
	/// ExecuteTargetの情報を元にCommandBuilder、ApiBaseのインスタンスを動的に生成し、
	/// ApiBaseのDoメソッドを実行する
	/// </remarks>
	public class APIExecutor
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public APIExecutor()
		{
		}
		#endregion

		#region +Excute 連携処理実行
		/// <summary>
		/// 連携処理実行
		/// </summary>
		/// <param name="target">実行対象の連携処理の情報を持つExecuteTargetクラス</param>
		public void Excute(ExecuteTarget target)
		{
			// ApiTypeによる切り分け
			switch(target.ApiType)
			{
				case APIType.Import:
					// インポート実行
					Import(target);
					break;
				case APIType.Export:
					// エクスポート実行
					Export(target);
					break;
				default :
					// エラー扱い
					throw new Exception("Apitypeの指定が不正です");
			}
		}
		#endregion

		#region +Import インポート処理実行
		/// <summary>
		/// インポート処理実行
		/// </summary>
		/// <param name="target">実行対象の連携処理の情報を持つExecuteTargetクラス</param>
		protected void Import(ExecuteTarget target)
		{
			using(ImportApiBase apiBase = GetImportApiBase(target))
			{
				apiBase.ParepareImportFile(target.TargetFilePath);

				if (File.Exists(target.TargetFilePath) == false)
				{
					throw new ArgumentException(string.Format("取り込み対象ファイルが存在しません。パス:'{0}'", target.TargetFilePath));
				}
				else
				{
					apiBase.Do();
				}
			}
		}
		#endregion

		#region +Export エクスポート処理実行
		/// <summary>
		/// エクスポート処理実行
		/// </summary>
		/// <param name="target">実行対象の連携処理の情報を持つExecuteTargetクラス</param>
		protected void Export(ExecuteTarget target)
		{
			if (File.Exists(target.TargetFilePath)) throw new ArgumentException(string.Format("出力対象ファイルがすでに存在しています。パス:'{0}'", target.TargetFilePath));

			using(ExportApiBase apiBase = GetExportApiBase(target))
			{
				apiBase.Do();
			}

			
		}
		#endregion

		#region +GetImportApiBase インポートApiベース生成
		/// <summary>
		/// インポートApiベース生成
		/// </summary>
		/// <param name="target">ApiBaseの生成に必要な情報を持つExecuteTargetクラス</param>
		/// <remarks>ファイル種別を元にインポートApiベースクラスを生成・返却</remarks>
		/// <returns>ファイル種別を元に生成した、インポートApiクラスのインスタンス</returns>
		public ImportApiBase GetImportApiBase(ExecuteTarget target)
		{
			ApiImportCommandBuilder apiInnerCommandBuilder =  GetCommandBuilder<ApiImportCommandBuilder>(target);
			switch (target.FileType)
			{
				case "csv":
					return  new CsvFileImportAPI(target, apiInnerCommandBuilder, GetCsvSetting(target));
				case "csv2":
					return new CsvFileImportAPI(target, apiInnerCommandBuilder, GetCsv2Setting(target));
				default:
					throw new Exception(string.Format("存在しないファイルタイプ'{0}'が指定されました。", target.FileType));
			}
		}
		#endregion

		#region +GetExportApiBase エクスポートApiベース生成
		/// <summary>
		/// エクスポートApiベース生成
		/// </summary>
		/// <param name="target">ApiBaseの生成に必要な情報を持つExecuteTargetクラス</param>
		/// <remarks>ファイル種別を元にエクスポートApiベースクラスを生成・返却</remarks>
		/// <returns>ファイル種別を元に生成した、エクスポートApiクラスのインスタンス</returns>
		public ExportApiBase GetExportApiBase(ExecuteTarget target)
		{
			ApiExportCommandBuilder apiExportCommandBuilder = GetCommandBuilder<ApiExportCommandBuilder>(target);
			switch (target.FileType)
			{
				case "csv":
					return new CsvFileExportApi(target, apiExportCommandBuilder, GetCsvSetting(target));
				case "csv2":
					return new CsvFileExportApi(target, apiExportCommandBuilder, GetCsv2Setting(target));
				default:
					throw new Exception(string.Format("存在しないファイルタイプ'{0}'が指定されました。", target.FileType));
			}

		}
		#endregion

		#region +GetCsvSetting CSV設定情報生成
		/// <summary>
		/// CSV情報構造体取得
		/// </summary>
		/// <param name="target">連携処理ターゲット情報</param>
		/// <returns>CSV情報構造体</returns>
		public CsvSetting GetCsvSetting(ExecuteTarget target)
		{
			return new CsvSetting
			{
				// 囲み文字
				Enclosure = "\"",
				// エンコード
				Encoding = System.Text.Encoding.GetEncoding("sjis"),
				// 行区切り文字
				LineDelimiter = "\r\n",
				// 区切り文字
				Delimiter = ",",
				// 改行エスケープ有効
				DoesEscapeLineDelimiter = true,
				// 改行エスケープ文字（改行文字をこれで置換する）
				LineDelimiterEscapeString = @""
			};
		}

		/// <summary>
		/// CSV2情報構造体取得
		/// </summary>
		/// <param name="target">連携処理ターゲット情報</param>
		/// <returns>CSV2情報構造体</returns>
		public CsvSetting GetCsv2Setting(ExecuteTarget target)
		{
			return new CsvSetting
			{
				// 囲み文字
				Enclosure = "",
				// エンコード
				Encoding = System.Text.Encoding.GetEncoding("sjis"),
				// 行区切り文字
				LineDelimiter = "\r\n",
				// 区切り文字
				Delimiter = ",",
				// 改行エスケープ有効
				DoesEscapeLineDelimiter = true,
				// 改行エスケープ文字（改行文字をこれで置換する）
				LineDelimiterEscapeString = @""
			};
		}
		#endregion

		#region +GetCommandBuilder コマンドビルダ生成
		/// <summary>
		/// コマンドビルダ生成
		/// </summary>
		/// <typeparam name="T">生成する型</typeparam>
		/// <param name="target">生成情報</param>
		/// <returns></returns>
		public T GetCommandBuilder<T>(ExecuteTarget target)
		{
			string className = typeof(T).Name + "_" + target.APIID;

			// Referディレクトリからdll読み込み
			T lastCommandBuilder = default(T);
			List<string> fileList = new List<string>(Directory.GetFiles(Constants.PHYSICALDIRPATH_EXTERNALAPI_STORAGE_LOCATION, "*.dll"));
			fileList.Sort();
			foreach (string dllFilePath in fileList)
			{
				// アセンブリロード
				Assembly assembly;
				if (TryAssemblyLoad(dllFilePath, out assembly) == false) continue;

				// ロードしたアセンブリが一致するクラスを持っていればインスタンスを作成する
				try
				{
					Type type = assembly.GetTypes().First(t => t.Name == className);
					lastCommandBuilder = (T)
						assembly.CreateInstance(
							type.FullName
							, false
							, BindingFlags.CreateInstance
							, null
							, null
							, null
							, null);

					if(lastCommandBuilder != null)
					{
						return lastCommandBuilder;
					}
				}
				// アセンブリと本体とで実装が異なる
				catch (ReflectionTypeLoadException ex)
				{
					// かなり致命的（＝これ以後継続的に失敗する可能性がある）だよね
					throw new Exception("アセンブリがバージョン違いで読み込めないようです。ApiCommandBuilder側をコンパイルしなおしてください。", ex);
				}
				// 一致するクラスが存在しないばあいは次のDLL読み込みさせたいので無視
				catch (InvalidOperationException)
				{
					// 何もせず
				}
				
			}

			// 一致するクラスが存在しない
			throw new Exception(string.Format("指定されたターゲット'{0}'に一致するアセンブリが発見できませんでした。", target.APIID));
		}
		#endregion

		#region -TryAssemblyLoad DLLの読み込みを試行
		/// <summary>
		/// DLLの読み込みを試行し、成否の結果を返します。
		/// </summary>	
		/// <param name="dllPath">DLLファイルパス</param>
		/// <param name="asm">アセンブリ</param>
		/// <returns>読込成否</returns>
		private bool TryAssemblyLoad(string dllPath, out Assembly asm)
		{
			asm = null;
			try
			{
				asm = Assembly.LoadFrom(dllPath);
				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion
	}
}
