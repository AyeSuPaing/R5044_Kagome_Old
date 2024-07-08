using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.ReSync.Util
{
	public abstract class FileWriterBase
	{
		/// <summary>
		/// ファイル出力
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="writeStr"></param>
		protected void WriteFile(string filepath, string writeStr)
		{
			// Mutexで排他制御しながらファイル書き込み
			using (System.Threading.Mutex mtx = new System.Threading.Mutex(false,
					filepath.Replace(@"\", "_")))
			{
				try
				{
					mtx.WaitOne();
					// ファイル書き込み
					using (StreamWriter sw = new StreamWriter(filepath, true, System.Text.Encoding.Default))
					{
						StringBuilder sbMessage = new StringBuilder();
						sw.WriteLine(writeStr);
					}
				}
				catch (Exception)
				{
					// 例外の場合はなにもしない
				}

				finally
				{
					mtx.ReleaseMutex();	// Dispose()で呼ばれない模様。
				}

			}
		}
	}
}
