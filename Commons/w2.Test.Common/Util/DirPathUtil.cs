using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common;

namespace w2.Test.Common.Util
{
	public class DirPathUtil
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private DirPathUtil()
		{
			// 何もしない
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="appType">アプリケーションタイプ</param>
		/// <param name="appRootPath">アプリケーションROOT相対パス（C:\Inetpub\wwwroot\R5044_Kagome.Develop\を除いたパス）</param>
		/// <param name="configPath">コンフィグ(Web.Configなど)ファイルパス</param>
		public DirPathUtil(ApplicationType appType, string appRootPath, string configPath = "")
		{
			this.AppType = appType;
			this.ConfigPath = configPath;

			this.ProjectRootPath = CreateProjectRootDir();
			this.AppRootPath = this.ProjectRootPath + appRootPath;
			this.CustomerResourcePath = CreateCustomerResourceDirPath();
			this.SqlConnectionString = CreateSqlConnectionString();
			this.SqlStatementPath = GetSqlStatementDirPath();
			this.ValidatorXmlPath = GetValidatorPath();
		}

		/// <summary>
		/// ROOTディレクトリを作成する
		/// </summary>
		/// <returns>ROOTディレクトリパス</returns>
		private string CreateProjectRootDir()
		{
			return CreateProjectRootDir(System.IO.Directory.GetCurrentDirectory());
		}
		/// <summary>
		/// ROOTディレクトリを作成する
		/// </summary>
		/// <param name="currentDir">実行ディレクトリ</param>
		/// <returns>ROOTディレクトリパス</returns>
		private string CreateProjectRootDir(string currentDir)
		{
			string indexString = (this.AppType == ApplicationType.Batch ? @"Batch\" : "") + @"TestResults\";
			string rootDir = currentDir.Remove(currentDir.IndexOf(indexString));
			Regex regex = new Regex(@"(?<=Builds\\[0-9]*\\)(?<project>[^\\]*?)\\.*");
			Match match = regex.Match(rootDir);
			string projectName = match.Groups["project"].Value;
			return string.Concat(rootDir, projectName == "" ? "" : @"Sources\", projectName == "" ? "" : projectName + @"\");
		}

		/// <summary>
		/// カスタマーリソースのPATHを作成
		/// </summary>
		/// <param name="rootPath">ROOTディレクトリパス</param>
		/// <returns>カスタマーリソースPATH</returns>
		private string CreateCustomerResourceDirPath()
		{
			 return string.Concat(this.ProjectRootPath, @"Commons\w2.CustomerResources\");
		}

		/// <summary>
		/// アプリケーションのROOTパスを作成
		/// </summary>
		/// <param name="rootPath">プロジェクトROOTパス</param>
		/// <returns>アプリケーションROOTパス</returns>
		private string CreateApplicationRootDirPath(string rootPath)
		{
			return rootPath + this.AppRootPath;
		}

		/// <summary>
		/// SQL接続文字列を設定
		/// </summary>
		/// <returns></returns>
		private string CreateSqlConnectionString()
		{
			// HACK: 取り敢えず固定値
			return @"server=w2DB1;database=R5044_Kagome.Develop;uid=sa;pwd=w2Sa";
		}

		/// <summary>
		/// SQLステートメントディレクトリパス取得
		/// </summary>
		/// <returns>対象ディレクトリパス</returns>
		private string GetSqlStatementDirPath()
		{
			return string.Concat(this.AppRootPath, @"Xml\Db\");
		}

		/// <summary>
		/// ValidatorXMLパス
		/// </summary>
		/// <returns>対象ディレクトリパス</returns>
		private string GetValidatorPath()
		{
			return string.Concat(this.AppRootPath, @"Xml\Validator\");
		}

		/// <summary>アプリケーションタイプ</summary>
		public ApplicationType AppType { get; private set; }
		/// <summary>アプリケーションROOTパス</summary>
		public string AppRootPath { get; private set; }
		/// <summary>プロジェクトROOTパス</summary>
		public string ProjectRootPath { get; private set; }
		/// <summary>コンフィグファイルパス</summary>
		public string ConfigPath { get; private set; }
		/// <summary>カスタマーリソースパス</summary>
		public string CustomerResourcePath { get; private set; }
		/// <summary>SQL接続文字列</summary>
		public string SqlConnectionString { get; private set; }
		/// <summary>SQLステートメントパス</summary>
		public string SqlStatementPath { get; private set; }
		/// <summary>バリデータXMLパス</summary>
		public string ValidatorXmlPath { get; private set; }
	}
}
