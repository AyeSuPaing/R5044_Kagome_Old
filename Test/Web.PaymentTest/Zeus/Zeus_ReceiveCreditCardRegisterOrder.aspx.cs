using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.Common.Util;
using w2.App.Common.Web.Page;

/// <summary>
/// カード登録向けZEUSリンク式決済処理クラス
/// </summary>
public partial class Zeus_Zeus_ReceiveCreditCardRegisterOrder : ZeusLinkPointReceiverBase
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public Zeus_Zeus_ReceiveCreditCardRegisterOrder() : base(PaymentTypes.Register)
	{
	}
}