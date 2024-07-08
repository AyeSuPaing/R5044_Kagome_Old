/*
=========================================================================================================
  Module      : Atodene後払い請求書モデル (InvoiceAtodeneModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Atodene
{
	/// <summary>
	/// Atodene後払い請求書モデル
	/// </summary>
	[Serializable]
	public partial class InvoiceAtodeneModel : ModelBase<InvoiceAtodeneModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceAtodeneModel()
		{
			this.Details = new InvoiceAtodeneDetailModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtodeneModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtodeneModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_ORDER_ID] = value; }
		}
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_ZIP] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_ZIP];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_ZIP] = value; }
		}
		/// <summary>住所1</summary>
		public string Address1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS1] = value; }
		}
		/// <summary>住所2</summary>
		public string Address2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS2];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_ADDRESS2] = value; }
		}
		/// <summary>会社名</summary>
		public string Companyname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_COMPANYNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_COMPANYNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_COMPANYNAME] = value; }
		}
		/// <summary>部署名</summary>
		public string Sectionname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SECTIONNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SECTIONNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SECTIONNAME] = value; }
		}
		/// <summary>氏名</summary>
		public string Name
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_NAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_NAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_NAME] = value; }
		}
		/// <summary>加盟店名タイトル</summary>
		public string Sitenametitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAMETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAMETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAMETITLE] = value; }
		}
		/// <summary>請求書記載店舗名</summary>
		public string Sitename
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SITENAME] = value; }
		}
		/// <summary>加盟店取引IDタイトル</summary>
		public string Shoporderidtitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERIDTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERIDTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERIDTITLE] = value; }
		}
		/// <summary>ご購入店受注番号</summary>
		public string Shoporderid
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERID];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERID] = value; }
		}
		/// <summary>請求書記載事項1</summary>
		public string Descriptiontext1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT1] = value; }
		}
		/// <summary>請求書記載事項2</summary>
		public string Descriptiontext2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT2];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT2] = value; }
		}
		/// <summary>請求書記載事項3</summary>
		public string Descriptiontext3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT3];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT3] = value; }
		}
		/// <summary>請求書記載事項4</summary>
		public string Descriptiontext4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT4] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT4];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT4] = value; }
		}
		/// <summary>請求書記載事項5</summary>
		public string Descriptiontext5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT5] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT5];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DESCRIPTIONTEXT5] = value; }
		}
		/// <summary>請求書発行元企業名</summary>
		public string Billservicename
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICENAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICENAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICENAME] = value; }
		}
		/// <summary>請求書発行元情報1</summary>
		public string Billserviceinfo1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO1] = value; }
		}
		/// <summary>請求書発行元情報2</summary>
		public string Billserviceinfo2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO2];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO2] = value; }
		}
		/// <summary>請求書発行元情報3</summary>
		public string Billserviceinfo3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO3];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO3] = value; }
		}
		/// <summary>請求書発行元情報4</summary>
		public string Billserviceinfo4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO4] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO4];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSERVICEINFO4] = value; }
		}
		/// <summary>請求書ステータス</summary>
		public string Billstate1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSTATE1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSTATE1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSTATE1] = value; }
		}
		/// <summary>宛名欄挨拶文欄1</summary>
		public string Billfirstgreet1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET1] = value; }
		}
		/// <summary>宛名欄挨拶文欄2</summary>
		public string Billfirstgreet2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET2];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET2] = value; }
		}
		/// <summary>宛名欄挨拶文欄3</summary>
		public string Billfirstgreet3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET3];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET3] = value; }
		}
		/// <summary>宛名欄挨拶文欄4</summary>
		public string Billfirstgreet4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET4] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET4];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLFIRSTGREET4] = value; }
		}
		/// <summary>予備項目1</summary>
		public string Expand1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND1];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND1] = value; }
		}
		/// <summary>予備項目2</summary>
		public string Expand2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND2];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND2] = value; }
		}
		/// <summary>予備項目3</summary>
		public string Expand3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND3];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND3] = value; }
		}
		/// <summary>予備項目4</summary>
		public string Expand4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND4] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND4];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND4] = value; }
		}
		/// <summary>予備項目5</summary>
		public string Expand5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND5] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND5];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND5] = value; }
		}
		/// <summary>予備項目6</summary>
		public string Expand6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND6] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND6];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND6] = value; }
		}
		/// <summary>予備項目7</summary>
		public string Expand7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND7] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND7];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND7] = value; }
		}
		/// <summary>予備項目8</summary>
		public string Expand8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND8] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND8];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND8] = value; }
		}
		/// <summary>予備項目9</summary>
		public string Expand9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND9] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND9];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND9] = value; }
		}
		/// <summary>予備項目10</summary>
		public string Expand10
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND10] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND10];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND10] = value; }
		}
		/// <summary>請求金額タイトル</summary>
		public string Billedamounttitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNTTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNTTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNTTITLE] = value; }
		}
		/// <summary>請求金額</summary>
		public string Billedamount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNT] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNT];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDAMOUNT] = value; }
		}
		/// <summary>請求金額消費税</summary>
		public string Billedfeetax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDFEETAX] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDFEETAX];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLEDFEETAX] = value; }
		}
		/// <summary>注文日タイトル</summary>
		public string Billorderdaytitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLORDERDAYTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLORDERDAYTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLORDERDAYTITLE] = value; }
		}
		/// <summary>注文日</summary>
		public string Shoporderdate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERDATE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERDATE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_SHOPORDERDATE] = value; }
		}
		/// <summary>請求書発行日タイトル</summary>
		public string Billsenddatetitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATETITLE] = value; }
		}
		/// <summary>請求書発行日</summary>
		public string Billsenddate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLSENDDATE] = value; }
		}
		/// <summary>お支払期限日タイトル</summary>
		public string Billdeadlinedatetitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATETITLE] = value; }
		}
		/// <summary>お支払期限日</summary>
		public string Billdeadlinedate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLDEADLINEDATE] = value; }
		}
		/// <summary>お問合せ番号タイトル</summary>
		public string Transactionidtitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONIDTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONIDTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONIDTITLE] = value; }
		}
		/// <summary>お問合せ番号</summary>
		public string Transactionid
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONID];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_TRANSACTIONID] = value; }
		}
		/// <summary>銀行振込注意文言</summary>
		public string Billbankinfomation
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BILLBANKINFOMATION] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BILLBANKINFOMATION];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BILLBANKINFOMATION] = value; }
		}
		/// <summary>銀行名タイトル</summary>
		public string Banknametitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAMETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAMETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAMETITLE] = value; }
		}
		/// <summary>銀行名漢字</summary>
		public string Bankname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKNAME] = value; }
		}
		/// <summary>銀行コード</summary>
		public string Bankcode
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKCODE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKCODE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKCODE] = value; }
		}
		/// <summary>支店名タイトル</summary>
		public string Branchnametitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAMETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAMETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAMETITLE] = value; }
		}
		/// <summary>支店名漢字</summary>
		public string Branchname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHNAME] = value; }
		}
		/// <summary>支店コード</summary>
		public string Branchcode
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHCODE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHCODE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BRANCHCODE] = value; }
		}
		/// <summary>口座番号タイトル</summary>
		public string Bankaccountnumbertitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBERTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBERTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBERTITLE] = value; }
		}
		/// <summary>預金種別</summary>
		public string Bankaccountkind
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTKIND] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTKIND];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTKIND] = value; }
		}
		/// <summary>口座番号</summary>
		public string Bankaccountnumber
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBER] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBER];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNUMBER] = value; }
		}
		/// <summary>口座名義タイトル</summary>
		public string Bankaccountnametitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAMETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAMETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAMETITLE] = value; }
		}
		/// <summary>銀行口座名義</summary>
		public string Bankaccountname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_BANKACCOUNTNAME] = value; }
		}
		/// <summary>払込取扱用支払期限日</summary>
		public string Receiptbilldeadlinedate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTBILLDEADLINEDATE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTBILLDEADLINEDATE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTBILLDEADLINEDATE] = value; }
		}
		/// <summary>払込取扱用購入者氏名</summary>
		public string Receiptname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTNAME] = value; }
		}
		/// <summary>バーコード情報</summary>
		public string Invoicebarcode
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_INVOICEBARCODE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_INVOICEBARCODE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_INVOICEBARCODE] = value; }
		}
		/// <summary>収納代行会社名タイトル</summary>
		public string Receiptcompanytitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANYTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANYTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANYTITLE] = value; }
		}
		/// <summary>収納代行会社名</summary>
		public string Receiptcompany
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANY] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANY];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_RECEIPTCOMPANY] = value; }
		}
		/// <summary>請求金額</summary>
		public string Docketbilledamount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETBILLEDAMOUNT] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETBILLEDAMOUNT];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETBILLEDAMOUNT] = value; }
		}
		/// <summary>受領証用購入者会社名</summary>
		public string Docketcompanyname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETCOMPANYNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETCOMPANYNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETCOMPANYNAME] = value; }
		}
		/// <summary>受領証用購入者部署名</summary>
		public string Docketsectionname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETSECTIONNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETSECTIONNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETSECTIONNAME] = value; }
		}
		/// <summary>受領証用購入者氏名</summary>
		public string Docketname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETNAME] = value; }
		}
		/// <summary>お問い合せ番号タイトル</summary>
		public string Dockettransactionidtitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONIDTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONIDTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONIDTITLE] = value; }
		}
		/// <summary>お問い合せ番号</summary>
		public string Dockettransactionid
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONID];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DOCKETTRANSACTIONID] = value; }
		}
		/// <summary>払込受領書用購入者会社名</summary>
		public string Vouchercompanyname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCOMPANYNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCOMPANYNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCOMPANYNAME] = value; }
		}
		/// <summary>払込受領書用購入者部署名</summary>
		public string Vouchersectionname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERSECTIONNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERSECTIONNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERSECTIONNAME] = value; }
		}
		/// <summary>払込受領書用購入者氏名</summary>
		public string Vouchercustomerfullname
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCUSTOMERFULLNAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCUSTOMERFULLNAME];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERCUSTOMERFULLNAME] = value; }
		}
		/// <summary>払込受領書用お問い合せ番号タイトル</summary>
		public string Vouchertransactionidtitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONIDTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONIDTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONIDTITLE] = value; }
		}
		/// <summary>払込受領書用お問い合せ番号</summary>
		public string Vouchertransactionid
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONID];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERTRANSACTIONID] = value; }
		}
		/// <summary>払込受領書用請求金額</summary>
		public string Voucherbilledamount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDAMOUNT] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDAMOUNT];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDAMOUNT] = value; }
		}
		/// <summary>払込受領書用消費税金額</summary>
		public string Voucherbilledfeetax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDFEETAX] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDFEETAX];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_VOUCHERBILLEDFEETAX] = value; }
		}
		/// <summary>収入印紙文言</summary>
		public string Revenuestamprequired
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_REVENUESTAMPREQUIRED] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_REVENUESTAMPREQUIRED];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_REVENUESTAMPREQUIRED] = value; }
		}
		/// <summary>明細内容タイトル</summary>
		public string Goodstitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSTITLE] = value; }
		}
		/// <summary>注文数タイトル</summary>
		public string Goodsamounttitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSAMOUNTTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSAMOUNTTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSAMOUNTTITLE] = value; }
		}
		/// <summary>単価タイトル</summary>
		public string Goodspricetitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSPRICETITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSPRICETITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSPRICETITLE] = value; }
		}
		/// <summary>金額タイトル</summary>
		public string Goodssubtotaltitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSSUBTOTALTITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSSUBTOTALTITLE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_GOODSSUBTOTALTITLE] = value; }
		}
		/// <summary>明細注意事項</summary>
		public string Detailinfomation
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_DETAILINFOMATION] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_DETAILINFOMATION];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DETAILINFOMATION] = value; }
		}
		/// <summary>ゆうちょ口座番号</summary>
		public string Expand11
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND11] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND11];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND11] = value; }
		}
		/// <summary>ゆうちょ加入者名</summary>
		public string Expand12
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND12] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND12];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND12] = value; }
		}
		/// <summary>OCR-Bフォント印字項目上段情報</summary>
		public string Expand13
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND13] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND13];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND13] = value; }
		}
		/// <summary>OCR-Bフォント印字項目下段情報</summary>
		public string Expand14
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND14] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND14];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND14] = value; }
		}
		/// <summary>払込取扱用購入者住所</summary>
		public string Expand15
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND15] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND15];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND15] = value; }
		}
		/// <summary>印字ズレチェックマーク</summary>
		public string Expand16
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND16] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND16];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND16] = value; }
		}
		/// <summary>予備項目17</summary>
		public string Expand17
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND17] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND17];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND17] = value; }
		}
		/// <summary>予備項目18</summary>
		public string Expand18
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND18] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND18];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND18] = value; }
		}
		/// <summary>予備項目19</summary>
		public string Expand19
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND19] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND19];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND19] = value; }
		}
		/// <summary>予備項目20</summary>
		public string Expand20
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND20] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND20];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_EXPAND20] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICEATODENE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICEATODENE_DATE_CREATED] = value; }
		}
		#endregion
	}
}
