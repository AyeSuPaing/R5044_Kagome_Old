using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.ReSync.Util
{
	public class FileUtil
	{
		#region +LoadUploadCSV 
		/// <summary>
		/// CSVファイルロード
		/// </summary>
		/// <param name="filepath">ロードするファイルのパス</param>
		/// <returns>ファイルの内容をロードした結果をもつデータテーブル</returns>
		public DataTable LoadUploadCSV(string filepath)
		{
			DataTable dt = new DataTable();

			//ファイル読み込み
			using (FileStream fs = new FileStream(filepath,
				FileMode.Open, FileAccess.Read))
			{

				StreamReader sr = new StreamReader(fs, System.Text.Encoding.GetEncoding("shift_jis"));

				int readcnt = 0;
				string line = "";

				//一行づつ読み込み
				while ((line = sr.ReadLine()) != null)
				{

					if (readcnt == 0)
					{
						//最初の行はヘッダとして
						foreach (string headstr in line.Split(",".ToCharArray()))
						{
							dt.Columns.Add(headstr);
						}
					}
					else
					{
						DataRow dr = dt.NewRow();
						int colcnt = 0;
						//以降の行はデータとして
						foreach (string bodystr in line.Split(",".ToCharArray()))
						{
							dr[colcnt] = bodystr;

							colcnt++;
						}

						dt.Rows.Add(dr);
					}

					readcnt++;
				}

				sr.Close();
			}

			return dt;
		}
		#endregion
	}
}
