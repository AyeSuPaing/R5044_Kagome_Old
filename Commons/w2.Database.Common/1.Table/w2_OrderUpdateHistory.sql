if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderUpdateHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderUpdateHistory]
GO
/*
=========================================================================================================
  Module      : ТНХ╢ПюХёНXРVЧЪЧЁ(w2_OrderUpdateHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderUpdateHistory](
	[order_id] [nvarchar](30) NOT NULL,
	[history_no] [int] NOT NULL  DEFAULT (1),
	[order_info] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderUpdateHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderUpdateHistory] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[history_no]
	) ON [PRIMARY]
GO