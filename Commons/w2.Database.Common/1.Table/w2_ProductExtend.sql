if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductExtend]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductExtend]
GO
/*
=========================================================================================================
  Module      : ���i�g�����ڃ}�X�^(w2_ProductExtend.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductExtend] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend6] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend7] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend8] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend9] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend10] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend11] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend12] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend13] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend14] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend15] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend16] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend17] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend18] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend19] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend20] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend21] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend22] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend23] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend24] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend25] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend26] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend27] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend28] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend29] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend30] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend31] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend32] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend33] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend34] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend35] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend36] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend37] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend38] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend39] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend40] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend41] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend42] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend43] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend44] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend45] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend46] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend47] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend48] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend49] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend50] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend51] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend52] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend53] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend54] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend55] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend56] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend57] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend58] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend59] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend60] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend61] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend62] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend63] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend64] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend65] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend66] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend67] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend68] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend69] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend70] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend71] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend72] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend73] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend74] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend75] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend76] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend77] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend78] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend79] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend80] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend81] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend82] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend83] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend84] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend85] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend86] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend87] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend88] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend89] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend90] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend91] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend92] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend93] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend94] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend95] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend96] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend97] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend98] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend99] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend100] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend101] [ntext] NOT NULL DEFAULT (N''),
	[extend102] [ntext] NOT NULL DEFAULT (N''),
	[extend103] [ntext] NOT NULL DEFAULT (N''),
	[extend104] [ntext] NOT NULL DEFAULT (N''),
	[extend105] [ntext] NOT NULL DEFAULT (N''),
	[extend106] [ntext] NOT NULL DEFAULT (N''),
	[extend107] [ntext] NOT NULL DEFAULT (N''),
	[extend108] [ntext] NOT NULL DEFAULT (N''),
	[extend109] [ntext] NOT NULL DEFAULT (N''),
	[extend110] [ntext] NOT NULL DEFAULT (N''),
	[extend111] [ntext] NOT NULL DEFAULT (N''),
	[extend112] [ntext] NOT NULL DEFAULT (N''),
	[extend113] [ntext] NOT NULL DEFAULT (N''),
	[extend114] [ntext] NOT NULL DEFAULT (N''),
	[extend115] [ntext] NOT NULL DEFAULT (N''),
	[extend116] [ntext] NOT NULL DEFAULT (N''),
	[extend117] [ntext] NOT NULL DEFAULT (N''),
	[extend118] [ntext] NOT NULL DEFAULT (N''),
	[extend119] [ntext] NOT NULL DEFAULT (N''),
	[extend120] [ntext] NOT NULL DEFAULT (N''),
	[extend121] [ntext] NOT NULL DEFAULT (N''),
	[extend122] [ntext] NOT NULL DEFAULT (N''),
	[extend123] [ntext] NOT NULL DEFAULT (N''),
	[extend124] [ntext] NOT NULL DEFAULT (N''),
	[extend125] [ntext] NOT NULL DEFAULT (N''),
	[extend126] [ntext] NOT NULL DEFAULT (N''),
	[extend127] [ntext] NOT NULL DEFAULT (N''),
	[extend128] [ntext] NOT NULL DEFAULT (N''),
	[extend129] [ntext] NOT NULL DEFAULT (N''),
	[extend130] [ntext] NOT NULL DEFAULT (N''),
	[extend131] [ntext] NOT NULL DEFAULT (N''),
	[extend132] [ntext] NOT NULL DEFAULT (N''),
	[extend133] [ntext] NOT NULL DEFAULT (N''),
	[extend134] [ntext] NOT NULL DEFAULT (N''),
	[extend135] [ntext] NOT NULL DEFAULT (N''),
	[extend136] [ntext] NOT NULL DEFAULT (N''),
	[extend137] [ntext] NOT NULL DEFAULT (N''),
	[extend138] [ntext] NOT NULL DEFAULT (N''),
	[extend139] [ntext] NOT NULL DEFAULT (N''),
	[extend140] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductExtend] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductExtend] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id]
	) ON [PRIMARY]
GO
