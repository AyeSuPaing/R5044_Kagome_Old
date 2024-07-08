/*
=========================================================================================================
  Module      : ValidatorXmlファイル作成モジュール(ValidatorXml.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace w2.Common.Util
{
	/// <summary>
	/// ValidatorXmlクラス
	/// </summary>
	[Serializable]
	public class ValidatorXml : XmlDocument
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ValidatorXml()
		{
			// カラム初期化
			this.Columns = new List<ValidatorXmlColumn>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rootNodeName">ルートノード名</param>
		public ValidatorXml(string rootNodeName)
		{
			// ルートノード名設定
			this.ValidatorName = rootNodeName;

			// カラム初期化
			this.Columns = new List<ValidatorXmlColumn>();

			// Xml宣言を追加する
			var declaration = this.CreateXmlDeclaration("1.0", "UTF-8", null);
			this.AppendChild(declaration);

			// ルートノード設定
			var rootNode = this.CreateElement(this.ValidatorName);
			this.AppendChild(rootNode);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">物理ファイルパス</param>
		/// <param name="rootNodeName">ルートノード名</param>
		public ValidatorXml(string filePath, string rootNodeName)
		{
			// ルートノード名設定
			this.ValidatorName = rootNodeName;

			// カラム初期化
			this.Columns = new List<ValidatorXmlColumn>();

			// ファイルが存在すればロード
			if (File.Exists(filePath))
			{
				this.Load(filePath);

				// プロパティにカラムデータをロード
				foreach (XmlNode columnNode in this.SelectSingleNode(this.ValidatorName).ChildNodes)
				{
					this.Columns.Add(new ValidatorXmlColumn(columnNode));
				}

				// Xml形式でロードされたカラムデータを削除
				this.SelectSingleNode(this.ValidatorName).RemoveAll();
			}
			// ファイルが存在しなければ作成
			else
			{
				// Xml宣言を追加する
				XmlDeclaration declaration = this.CreateXmlDeclaration("1.0", "UTF-8", null);
				this.AppendChild(declaration);

				// ルートノード設定
				XmlElement rootNode = this.CreateElement(this.ValidatorName);
				this.AppendChild(rootNode);
			}
		}

		/// <summary>
		/// バリデーションカラムの読み込み
		/// </summary>
		/// <param name="rootNodeName">ルートノード名</param>
		public void LoadColum(string rootNodeName)
		{
			// ルートノード名設定
			this.ValidatorName = rootNodeName;

			// カラム初期化
			this.Columns = new List<ValidatorXmlColumn>();

			// プロパティにカラムデータをロード
			foreach (XmlNode columnNode in this.SelectSingleNode(this.ValidatorName).ChildNodes)
			{
				this.Columns.Add(new ValidatorXmlColumn(columnNode));
			}

			// Xml形式でロードされたカラムデータを削除
			this.SelectSingleNode(this.ValidatorName).RemoveAll();
		}

		/// <summary>
		/// カラム追加処理
		/// </summary>
		/// <param name="columnData">追加するValidatorXmlColumnオブジェクト</param>
		public void AddColumn(ValidatorXmlColumn columnData)
		{
			this.Columns.Add(columnData);
		}

		/// <summary>
		/// ValidatorXml書き出し処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		public void WriteValidatorXml(string filePath)
		{
			// Check if not exists file then create file
			if (File.Exists(filePath) == false)
			{
				File.Create(filePath).Close();
			}

			CreateColumnXmlNode();
#if DEBUG
			FileAttributes attr = File.GetAttributes(filePath);
			bool isReadOnly = ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
			if (isReadOnly)
			{
				File.SetAttributes(filePath, attr & ~FileAttributes.ReadOnly); // 読み取り専用を解除
			}
#endif
			this.Save(filePath);
#if DEBUG
			if (isReadOnly)
			{
				File.SetAttributes(filePath, attr | FileAttributes.ReadOnly); // 読み取り専用を再設定
			}
#endif
		}

		/// <summary>
		/// カラムノード生成処理
		/// </summary>
		private void CreateColumnXmlNode()
		{
			// とりあえず一回消す
			this.SelectSingleNode(this.ValidatorName).RemoveAll();

			// 現在のカラムデータをXml形式で生成
			foreach (ValidatorXmlColumn column in this.Columns)
			{
				if ((column.IsComment == false) && (column.Name != ""))
				{
					XmlDocument columnDoc = column.CreateColumnXml();
					XmlNode columnNode = this.ImportNode(columnDoc.DocumentElement, true);
					this.SelectSingleNode(this.ValidatorName).AppendChild(columnNode);
				}
				if (column.IsComment)
				{
					XmlNode commentNode = this.ImportNode(column.Comment, true);
					this.SelectSingleNode(this.ValidatorName).AppendChild(commentNode);
				}
			}
		}

		/// <summary>
		/// 列の XML ノードを作成します。
		/// </summary>
		public void CreateColumnXml()
		{
			CreateColumnXmlNode();
		}

		/// <summary>
		/// カラムノード削除処理
		/// </summary>
		/// <param name="columnName">カラム名</param>
		public void DeleteColumnXmlNode(string columnName)
		{
			foreach (ValidatorXmlColumn column in this.Columns)
			{
				if (column.Name == columnName)
				{
					this.Columns.Remove(column);
					break;
				}
			}
		}

		/// <summary>
		/// カラム検索処理
		/// </summary>
		/// <param name="columnName">検索するカラム名</param>
		/// <returns>マッチするValidatorXmlオブジェクト</returns>
		public ValidatorXmlColumn FindColumn(string columnName)
		{
			foreach (ValidatorXmlColumn column in this.Columns)
			{
				if (column.Name == columnName)
				{
					return column;
				}
			}
			return null;
		}

		/// <summary>バリデータ名</summary>
		public string ValidatorName { get; set; }
		/// <summary>カラムリスト</summary>
		public List<ValidatorXmlColumn> Columns { get; private set; }
	}
}
