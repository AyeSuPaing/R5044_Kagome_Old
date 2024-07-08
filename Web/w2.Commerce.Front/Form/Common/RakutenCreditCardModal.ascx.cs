/*
=========================================================================================================
  Module      : 楽天カード入力モーダルウィンドウ(RakutenCreditCardModal.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web.UI.WebControls;

namespace Form.Common
{
	/// <summary>
	/// 楽天カード入力モーダルウィンドウ
	/// </summary>
	public partial class Form_Common_RakutenCreditCardModal : System.Web.UI.UserControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}
		/// <summary>カート枝番</summary>
		public int CartIndex { get; set; }
		/// <summary>註文か</summary>
		public bool IsOrder { get; set; }
		/// <summary>クレカ分割購入可能コードリスト</summary>
		public ListItemCollection InstallmentCodeList { get; set; }
		/// <summary>選択済みクレカ分割コード</summary>
		public string SelectedInstallmentCode { get; set; }
		/// <summary>選択済み有効期限月</summary>
		public string SelectedExpireMonth { get; set; }
		/// <summary>選択済み有効期限年</summary>
		public string SelectedExpireYear { get; set; }
		/// <summary>名義人名</summary>
		public string AuthorName { get; set; }
		/// <summary>カード会社</summary>
		public string CreditCompany { get; set; }
		/// <summary>カード番号下四桁</summary>
		public string CreditCardNo4 { get; set; }
		/// <summary>CVVトークン</summary>
		public string SecurityCode { get; set; }
	}
}