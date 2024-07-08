/*
=========================================================================================================
  Module      : プラグインのデモ(Demo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Collections;
using w2.Plugin;

namespace w2.Plugin.Demo
{
	public class Sample :
		User.IUserRegisteredPlugin,
		Order.IOrderValidatingPlugin,
		Order.IOrderFailedPlugin,
		Order.IOrderCompletePlugin
	{

		#region IUserRegisteredPlugin メンバー

		/// <summary>
		/// 呼び出し元
		/// </summary>
		public void OnRegistered()
		{
			XDocument document = HashToXML(this.Host.Data);
			this.Host.WriteInfoLog(document.ToString());
		}

		/// <summary>
		/// ハッシュテーブルに格納されている文字列をXMLに変換する
		/// </summary>
		/// <param name="hash">ハッシュ</param>
		/// <returns>変換されたXML</returns>
		private XDocument HashToXML(Hashtable hash)
		{
			// XMLのベースを作成
			XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("root"));

			var list = from dicEntry in hash.Cast<DictionaryEntry>()
					   where dicEntry.Value.GetType() == typeof(string)
					   select new XElement(dicEntry.Key as string, dicEntry.Value as string);

			list.ToList().ForEach(element => document.Root.Add(element));
			return document;
		}

		#endregion

		#region IPlugin メンバー

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="host">プラグインホスト</param>
		public void Initialize(IPluginHost host)
		{
			this.Host = host;
		}

		/// <summary>ホスト</summary>
		public IPluginHost Host { get; private set; }

		#endregion

		#region IOrderValidatingPlugin メンバー

		/// <summary>
		/// 検証時処理
		/// </summary>
		public void OnValidating()
		{
			this.Host.WriteInfoLog("注文検証用プラグインが実行されました。");
		}

		/// <summary>成功フラグ</summary>
		public bool IsSuccess { get { return true; } }
		/// <summary>メッセージ</summary>
		public string Message { get { return ""; } }

		#endregion

		#region IOrderFailedPlugin メンバー

		/// <summary>
		/// 失敗時処理
		/// </summary>
		public void OnFailed()
		{
			this.Host.WriteInfoLog("注文失敗時用プラグインが実行されました。");
		}

		#endregion

		#region IOrderCompletePlugin メンバー

		/// <summary>
		/// 注文完了時処理
		/// </summary>
		public void OnCompleted()
		{
			this.Host.WriteInfoLog("注文完了時用プラグインが実行されました。");
		}

		#endregion
	}
}
