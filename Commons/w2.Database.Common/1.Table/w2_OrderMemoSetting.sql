if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderMemoSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderMemoSetting]
GO
/*
=========================================================================================================
  Module      : ТНХ╢ГБГВР▌ТшПюХё(w2_OrderMemoSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderMemoSetting] (
	[order_memo_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_memo_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[height] [int],
	[width] [int],
	[css_class] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[max_length] [int] NOT NULL DEFAULT (400),
	[default_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[display_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC'),
	[display_order] [int] NOT NULL DEFAULT (10),
	[term_bgn] [datetime],
	[term_end] [datetime],
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderMemoSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderMemoSetting] PRIMARY KEY  CLUSTERED
	(
		[order_memo_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_OrderMemoSetting_1] ON [dbo].[w2_OrderMemoSetting]([display_order]) ON [PRIMARY]
GO
