/*
=========================================================================================================
  Module      : ZIPアーカイバ(ZipArchiver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace w2.Common.Util.Archiver
{
	///**************************************************************************************
	/// <summary>
	/// Zipファイルの圧縮・展開を行う
	/// </summary>
	///**************************************************************************************
	public class ZipArchiver
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZipArchiver()
			: this(new List<string>())
		{
			// なにもしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lRefuseList">拒否リスト（ディレクトリ・ファイル）</param>
		public ZipArchiver(List<string> lRefuseList)
		{
			this.RefuseList = lRefuseList;
		}

		/// <summary>
		/// ZIP解凍
		/// </summary>
		/// <param name="targetFilePath">解凍対象ファイル</param>
		/// <param name="extractDirPath">展開先</param>
		/// <param name="workingDirectoryName">作業ディレクトリ名</param>
		/// <returns>展開ファイル</returns>
		public List<string> Decompress(string targetFilePath, string extractDirPath, string workingDirectoryName = "")
		{
			var result = new List<string>();

			var extractWorkingPath = Path.Combine(extractDirPath, workingDirectoryName);

			using (var zipFile = ZipFile.Read(targetFilePath, Encoding.GetEncoding("Shift_JIS")))
			{
				foreach (var zipEntry in zipFile)
				{
					// パス補正を行う（Webパスの「/」→[\]変換）
					// （解凍処理自体は.Net側でパス補正を行って取り込んでくれるが、拒否リストとのマッチングの際に不整合が起こる為）
					var extractFilePath = Path.Combine(extractWorkingPath, zipEntry.FileName).Replace("/", @"\");
					var fileName = Path.GetFileName(extractFilePath);
					//------------------------------------------------------
					// 解凍除外処理
					//------------------------------------------------------
					// 拒否リスト（ディレクトリ・ファイル）は書き込みを行わない
					if (this.RefuseList.Any(rl => extractFilePath.ToLower().StartsWith(rl.ToLower()))) continue;

					// 特定ファイルは書き込みを行わない
					if ((fileName == string.Empty)
						|| (fileName.ToLower() == "thumbs.db")
						|| (fileName.ToLower() == "vssver2.scc"))
					{
						continue;
					}

					//------------------------------------------------------
					// エントリ解凍
					//------------------------------------------------------
					zipEntry.Extract(extractWorkingPath, ExtractExistingFileAction.OverwriteSilently);

					result.Add(extractFilePath);
				}
			}

			return result;
		}

		/// <summary>
		/// ZIPファイル圧縮
		/// </summary>
		/// <param name="targetPath">圧縮対象（ファイル/ディレクトリ）パス</param>
		/// <param name="targetRootPath">基準ルートパス（このパスを差し引いた分の階層をZIP内に作成）</param>
		/// <param name="zipFilePath">作成ZIPファイルパス</param>
		public void CompressFile(string targetPath, string targetRootPath, string zipFilePath)
		{
			CompressFile(new string[] { targetPath }, targetRootPath, zipFilePath);
		}
		/// <summary>
		/// ZIPファイル圧縮
		/// </summary>
		/// <param name="targetPaths">圧縮対象（ファイル/ディレクトリ）パス一覧</param>
		/// <param name="targetRootPath">基準ルートパス（このパスを差し引いた分の階層をZIP内に作成）</param>
		/// <param name="zipFilePath">作成ZIPファイルパス</param>
		public void CompressFile(string[] targetPaths, string targetRootPath, string zipFilePath)
		{
			//------------------------------------------------------
			// ZIP圧縮
			//------------------------------------------------------
			using (FileStream outStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
			using (BinaryWriter writer = new BinaryWriter(outStream))
			{
				CompressToStream(targetPaths, targetRootPath, outStream);
			}
		}

		/// <summary>
		/// ZIP byte配列圧縮
		/// </summary>
		/// <param name="targetPath">圧縮対象（ファイル/ディレクトリ）パス</param>
		/// <param name="targetRootPath">基準ルートパス（このパスを差し引いた分の階層をZIP内に作成）</param>
		/// <returns>ZIPファイルバイト配列</returns>
		public void CompressToStream(string targetPath, string targetRootPath, Stream ouput)
		{
			//------------------------------------------------------
			// ZIPストリーム圧縮
			//------------------------------------------------------
			CompressToStream(new string[] { targetPath }, targetRootPath, ouput);
		}

		/// <summary>
		/// ZIPストリーム圧縮
		/// </summary>
		/// <param name="targetPaths">圧縮対象（ファイル/ディレクトリ）パス一覧</param>
		/// <param name="targetRootPath">基準ルートパス（このパスを差し引いた分の階層をZIP内に作成）</param>
		/// <returns>ZIPファイルメモリストリーム</returns>
		public void CompressToStream(string[] targetPaths, string targetRootPath, Stream output)
		{
			using (ZipFile zipFile = new ZipFile(Encoding.GetEncoding("Shift_JIS")))
			{
				// 圧縮レベル：普通
				zipFile.CompressionLevel = CompressionLevel.Default;
				zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;

				// ZIPエントリ追加
				AddZipEntry(zipFile, targetPaths, targetRootPath);

				// メモリストリームに保存
				zipFile.Save(output);
			}
		}

		/// <summary>
		/// ZIPエントリ追加
		/// </summary>
		/// <param name="zfZipFile">ZipFileオブジェクト</param>
		/// <param name="strTargetPaths">圧縮対象（ファイル/ディレクトリ）パス一覧</param>
		/// <param name="strTargetRootPath">基準ルートパス（このパスを差し引いた分の階層をZIP内に作成）</param>
		private void AddZipEntry(ZipFile zfZipFile, string[] strTargetPaths, string strTargetRootPath)
		{
			foreach (string strTargetPath in strTargetPaths)
			{
				//------------------------------------------------------
				// 拒否リストに含まれる場合、次のcontinueして次のPATHを処理する
				//------------------------------------------------------
				if (this.RefuseList.Exists(refusePath => (refusePath.ToLower() == strTargetPath.ToLower())))
				{
					continue;
				}

				//------------------------------------------------------
				// ファイル／ディレクトリ追加
				//------------------------------------------------------
				// ファイル？
				if (File.Exists(strTargetPath)
					&& (Directory.Exists(strTargetPath) == false))
				{
					// 特定ファイルは書き込みを行わない
					string strFileName = Path.GetFileName(strTargetPath).ToLower();
					if ((strFileName == "thumbs.db")
						|| (strFileName == "vssver2.scc"))
					{
						continue;
					}

					zfZipFile.AddFile(strTargetPath, (Path.GetDirectoryName(strTargetPath) + @"\").Replace(strTargetRootPath, ""));
				}
				// ディレクトリ？（再帰）
				else if ((File.Exists(strTargetPath) == false)
					&& Directory.Exists(strTargetPath))
				{
					string[] strTargetDirectoryFiles = Directory.GetFileSystemEntries(strTargetPath);
					if (strTargetDirectoryFiles.Length == 0)
					{
						//ディレクトリだけ作成
						zfZipFile.AddDirectory(strTargetPath, (strTargetPath + @"\").Replace(strTargetRootPath, ""));
					}
					else
					{
						// ディレクトリ以下のファイル追加
						AddZipEntry(zfZipFile, strTargetDirectoryFiles, strTargetRootPath);
					}
				}
			}
		}

		/// <summary>拒否リスト（扱いたくないディレクトリ/パスの一覧）</summary>
		public List<string> RefuseList { get; private set; }
	}
}