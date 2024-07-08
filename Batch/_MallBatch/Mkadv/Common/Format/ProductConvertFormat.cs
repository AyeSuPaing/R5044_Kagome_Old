/*
=========================================================================================================
  Module      : 商品コンバータのフォーマット設定クラス(ProductConvertFormat.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.MallBatch.Mkadv.Common.Format
{
	///**************************************************************************************
	/// <summary>
	/// 商品コンバータのフォーマット設定クラス（クラス名変更：Adto.cs → ProductConvertFormat.cs）
	/// </summary>
	///**************************************************************************************
	public class ProductConvertFormat
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductConvertFormat()
		{
			// プロパティ初期化
			this.Columns = new List<string>();
			this.Formats = new List<string>();
			this.ColumnIds = new List<int>();
			this.FormatCompile = new List<FormatCompile>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="iAdtoId">商品コンバータID</param>
		/// <remarks>商品コンバータ情報を取得・保持します</remarks>
		/// =============================================================================================
		public ProductConvertFormat(SqlAccessor sqlAccessor, int iAdtoId) : this()
		{
			InitProductConvertFormat(sqlAccessor, iAdtoId);
		}

		/// <summary>
		/// 商品コンバータ情報初期化
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="iAdtoId">商品コンバータID</param>
		/// <remarks>
		/// 商品コンバータ情報を取得し、出力フォーマットをコンパイル済みフォーマットに変換した情報を保持する
		/// </remarks>
		/// =============================================================================================
		private void InitProductConvertFormat(SqlAccessor sqlAccessor, int iAdtoId)
		{
			// 商品コンバータＩＤをプロパティにセット
			this.AdtoId = iAdtoId;

			// 商品コンバータ情報を取得する
			InitAdtos(sqlAccessor);

			// 出力フォーマットを取得する
			InitAdColumns(sqlAccessor);

			// 出力フォーマットをコンパイルする（コンパイル済みフォーマット取得）
			InitFormatCompiles();
		}

		/// <summary>
		/// 商品コンバータ情報取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <remarks>商品コンバータの基本データを取得する</remarks>
		/// =============================================================================================
		private void InitAdtos(SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// 商品コンバータ情報取得
			//------------------------------------------------------
			DataView dvAdto = null;
			using (SqlStatement sqlStatement = new SqlStatement("Adto", "GetAdto"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, this.AdtoId);

				dvAdto = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// プロパティにセットする
			//------------------------------------------------------
			if (dvAdto.Count != 0)
			{
				if (StringUtility.ToEmpty(dvAdto[0][Constants.FIELD_MALLPRDCNV_FILENAME]).StartsWith("quantity"))
				{
					this.IsNeedYhoVariationInfo = true;
				}
				this.AdtoName = (string)dvAdto[0][Constants.FIELD_MALLPRDCNV_ADTO_NAME];
				this.Separater = ((string)dvAdto[0][Constants.FIELD_MALLPRDCNV_SEPARATER])[0];
				this.CharacterCodeType = StringUtility.ToEmpty(dvAdto[0][Constants.FIELD_MALLPRDCNV_CHARACTERCODETYPE]);

				if (StringUtility.ToEmpty(dvAdto[0][Constants.FIELD_MALLPRDCNV_NEWLINETYPE]) == Constants.FLG_MALLPRDCNV_NEWLINETYPE_CR)
				{
					this.NewLineType = "\r";
				}
				else if (StringUtility.ToEmpty(dvAdto[0][Constants.FIELD_MALLPRDCNV_NEWLINETYPE]) == Constants.FLG_MALLPRDCNV_NEWLINETYPE_LF)
				{
					this.NewLineType = "\n";
				}
				else
				{
					this.NewLineType = "\r\n";
				}

				this.Quote = ((string)dvAdto[0][Constants.FIELD_MALLPRDCNV_QUOTE])[0];
				this.IsHeader = (bool)dvAdto[0][Constants.FIELD_MALLPRDCNV_ISHEADER];
				this.IsQuote = (bool)dvAdto[0][Constants.FIELD_MALLPRDCNV_ISQUOTE];
				this.ExtractionPatternId = ((int)dvAdto[0][Constants.FIELD_MALLPRDCNV_EXTRACTIONPATTERN]).ToString();
			}
		}

		/// <summary>
		/// 商品コンバータの出力フォーマットを取得する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// =============================================================================================
		private void InitAdColumns(SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// 出力フォーマット情報取得
			//------------------------------------------------------
			DataView dvColumnNames = null;
			using (SqlStatement sqlStatement = new SqlStatement("AdColumns", "GetColumnName"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNVCOLUMNS_ADTO_ID, this.AdtoId);

				dvColumnNames = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// プロパティにセットする
			//------------------------------------------------------
			foreach (DataRowView drvColumnNames in dvColumnNames)
			{
				this.ColumnIds.Add((int)drvColumnNames[Constants.FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID]);
				this.Columns.Add((string)drvColumnNames[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME]);
				this.Formats.Add((string)drvColumnNames[Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT]);
			}
		}

		/// <summary>
		/// ヘッダ文字列取得
		/// </summary>
		/// <returns>ヘッダ文字列</returns>
		/// <remarks>ヘッダ文字列を生成します。</remarks>
		/// =============================================================================================
		public string GetHeaderString()
		{
			StringBuilder sbResult = new StringBuilder();

			// ヘッダ出力を行う場合（ヘッダ出力を行わない場合は空文字出力）
			if (this.IsHeader)
			{
				foreach (string strTmp in this.Columns)
				{
					// 最初のカラムでなければ区切り文字を追加する
					if (sbResult.ToString() != "")
					{
						sbResult.Append(this.Separater);
					}

					// 囲い文字を出力する
					if (this.IsQuote)
					{
						// カラムを追加（囲い文字あり）
						sbResult.Append(this.Quote).Append(strTmp).Append(this.Quote);
					}
					// 囲い文字を出力しない
					else
					{
						// カラムを追加（囲い文字なし）
						sbResult.Append(strTmp);
					}
				}
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// コンパイル済みフォーマットを取得する
		/// </summary>
		/// <remarks>出力フォーマットをコンパイル</remarks>
		/// =============================================================================================
		private void InitFormatCompiles()
		{
			List<FormatCompile> lFormats = new List<FormatCompile>();

			// 出力フォーマットを全てコンパイルする
			foreach (string strFormat in this.Formats)
			{
				// 出力フォーマットをコンパイルする
				FormatCompile formatCompile = new FormatCompile(strFormat);

				// コンパイル済み情報からモールごとのプロパティを設定する
				if (this.IsNeedYhoVariationInfo == false)
				{
					this.IsNeedYhoVariationInfo = formatCompile.IsNeedYhoVariationInfo;
				}
				if (this.IsNeedRtnCategoryInfo == false)
				{
					this.IsNeedRtnCategoryInfo = formatCompile.IsNeedRtnCategoryInfo;
				}

				lFormats.Add(formatCompile);
			}

			// 出力フォーマットを配列にする（プロパティにセットする）
			this.FormatCompile = lFormats;
		}

		/// <summary>商品コンバータID</summary>
		public int AdtoId { get; private set; }

		/// <summary>商品コンバータ名</summary>
		public string AdtoName { get; private set; }
	
		/// <summary>区切り記号</summary>
		public char Separater { get; private set; }

		/// <summary>文字コード</summary>
		public string CharacterCodeType { get; private set; }
	
		/// <summary>改行コード</summary>
		public string NewLineType { get; private set; }

		/// <summary>囲い記号</summary>
		public char Quote { get; private set; }

		/// <summary>ヘッダ出力が必要ならtrue</summary>
		public bool IsHeader { get; private set; }

		/// <summary>囲い記号が必要ならtrue</summary>
		public bool IsQuote { get; private set; }

		/// <summary>抽出条件ID</summary>
		public string ExtractionPatternId { get; private set; }

		/// <summary>行リスト</summary>
		public List<string> Columns { get; private set; }

		/// <summary>フォーマット</summary>
		public List<string> Formats { get; private set; }

		/// <summary>ID</summary>
		public List<int> ColumnIds { get; private set; }

		/// <summary>プリコンパイル済み書式</summary>
		public List<FormatCompile> FormatCompile { get; private set; }

		/// <summary>ヤフーバリエーション情報要否</summary>
		public bool IsNeedYhoVariationInfo { get; private set; }

		/// <summary>楽天カテゴリ情報要否</summary>
		public bool IsNeedRtnCategoryInfo { get; private set; }
		/// <summary>SKU管理番号のインデックス</summary>
		public int SkuControlNumber
		{
			get
			{
				return this.Columns.IndexOf(Constants.SKU_LEVEL_HEADER) -1;
			}
		}
	}
}
