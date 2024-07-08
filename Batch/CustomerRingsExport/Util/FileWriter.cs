/*
=========================================================================================================
  Module      : ファイル書き込みクラス(FileWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace w2.Commerce.Batch.CustomerRingsExport.Util
{
	public class FileWriter : IDisposable
	{
		/// <summary>出力ストリーム</summary>
		private Stream m_outputStream;
		/// <summary>出力ライター</summary>
		private TextWriter m_writer;

		/// <summary>UTF-8 (UTF-8 with BOM)</summary>
		public static readonly Encoding ENCODING_UTF8 = new UTF8Encoding(true);
		/// <summary>UTF-8N (UTF-8 without BOM)</summary>
		public static readonly Encoding ENCODING_UTF8N = new UTF8Encoding(false);
		/// <summary>SJIS</summary>
		public static readonly Encoding ENCODING_SJIS = Encoding.GetEncoding(932);

		/// <summary>
		/// コンストラクタ（このインスタンスはDisposeする）
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="enc">エンコーディング</param>
		/// <param name="allowReadWhileProcessing">出力中に他プロセスがファイルを読み取ることを許可するか</param>
		public FileWriter(string path, Encoding enc, bool allowReadWhileProcessing)
		{
			m_outputStream = new FileStream(
				path,
				FileMode.CreateNew,
				FileAccess.Write,
				allowReadWhileProcessing ? FileShare.Read : FileShare.None);
			m_writer = new StreamWriter(m_outputStream, enc);
		}

		/// <summary>
		/// 文字列書き込み
		/// </summary>
		/// <param name="s">書き込み</param>
		public void Write(string s)
		{
			m_writer.Write(s);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			if (m_writer != null)
			{
				try { m_writer.Dispose(); }
				finally { m_writer = null; }
			}

			if (m_outputStream != null)
			{
				try { m_outputStream.Dispose(); }
				finally { m_outputStream = null; }
			}
		}
	}
}
