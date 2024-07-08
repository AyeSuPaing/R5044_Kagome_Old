/*
=========================================================================================================
  Module      : メールテンプレートクラス(MailTemplate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.ExternalAPI.Common;

namespace ExternalAPI.Mail
{
	/// <summary>
	/// メールテンプレートクラス
	/// </summary>
	partial class MailTemplate
	{
		#region プロパティ
		/// <summary>ファイル名プロパティ</summary>
		public string FileName {get; set;}
		/// <summary>ファイルタイププロパティ</summary>
		public string FileType { get; set; }
		/// <summary>APIタイププロパティ</summary>
		public APIType ApiType { get; set; }
		/// <summary>失敗フラグプロパティ</summary>
		public bool IsFailed { get; set; }
		/// <summary>DBログテーブルプロパティ</summary>
		public DataTable Log { get; set; }
		/// <summary>実行時間プロパティ</summary>
		public DateTime ExecutedTime { get; set; }
		/// <summary>終了時間プロパティ</summary>
		public DateTime EndTime { get; set; }
		#endregion

		#region -GetApiType APIタイプ取得
		/// <summary>
		/// APIタイプ取得
		/// </summary>
		/// <param name="apiType"></param>
		/// <returns></returns>
		private static string GetApiType(APIType apiType)
		{
			return (apiType == APIType.Import) ? "入力": "出力";
		}
		#endregion
	}
}
