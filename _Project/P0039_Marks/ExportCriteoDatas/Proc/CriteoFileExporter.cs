/*
=========================================================================================================
  Module      : Criteoファイル出力(CriteoFileExporter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteo連携用ファイル出力
	/// </summary>
	public class CriteoFileExporter : IDisposable
	{
		/// <summary>CSVヘッダ</summary>
		/// <remarks>
		/// 商品ID(必須)：id
		/// 名前(必須)：name
		/// リンク先URL(必須)：producturl
		/// 画像URL(必須)：bigimage
		/// カテゴリ名1(必須)：categoryid1
		/// 商品の詳細(必須)：description
		/// 価格(必須)：price
		/// 在庫情報：instock
		/// 小画像URL：smallimage
		/// 推奨小売価格：retailprice
		/// 値引き：discount
		/// リコメンドの可否：recommendable
		/// カテゴリ名2：categoryid2
		/// カテゴリ名3：categoryid3
		/// </remarks>
		public const string HEADER = "id,name,producturl,bigimage,categoryid1,description,price,instock,smallimage,retailprice,discount,recommendable,categoryid2,categoryid3";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportFilePath">出力ファイルパス</param>
		public CriteoFileExporter(string exportFilePath)
		{
			CreateDirectoryIfNotExists(exportFilePath);
			
			this.ExportStream = new FileStream(exportFilePath, FileMode.Create, FileAccess.Write);
			this.FileWriter = new StreamWriter(this.ExportStream);
			this.FileWriter.WriteLine(HEADER);
		}

		/// <summary>
		/// 存在しない場合にディレクトリを作成する
		/// </summary>
		/// <param name="exportFilePath"></param>
		private void CreateDirectoryIfNotExists(string exportFilePath)
		{
			string currentDirectory = new FileInfo(exportFilePath).DirectoryName;
			if (Directory.Exists(currentDirectory) == false)
			{
				Directory.CreateDirectory(new FileInfo(exportFilePath).DirectoryName);
			}
		}

		/// <summary>
		/// 出力
		/// </summary>
		/// <param name="model">Criteo連携用モデル</param>
		public void Write(CriteoModel model)
		{
			this.FileWriter.WriteLine(model.ToString());
		}

		/// <summary>
		/// メモリ解放する
		/// </summary>
		public void Dispose()
		{
			this.FileWriter.Dispose();
			this.ExportStream.Dispose();
		}

		/// <summary>出力用ストリーム</summary>
		private FileStream ExportStream;
		/// <summary>ファイルライタ</summary>
		private StreamWriter FileWriter;
	}
}