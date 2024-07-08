if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessageRequestItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessageRequestItem]
GO
/*
=========================================================================================================
  Module      : メッセージ依頼アイテムマスタ(w2_CsMessageRequestItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessageRequestItem] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message_no] [int] NOT NULL DEFAULT (1),
	[request_no] [int] NOT NULL DEFAULT (1),
	[branch_no] [int] NOT NULL DEFAULT (1),
	[appr_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[result_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[comment] [nvarchar] (max) NOT NULL,
	[date_status_changed] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessageRequestItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessageRequestItem] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[incident_id],
		[message_no],
		[request_no],
		[branch_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessageRequestItem_1] ON [dbo].[w2_CsMessageRequestItem]([dept_id], [appr_operator_id], [result_status]) ON [PRIMARY]
GO
