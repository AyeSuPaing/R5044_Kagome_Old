if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessageRequest]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessageRequest]
GO
/*
=========================================================================================================
  Module      : メッセージ依頼マスタ(w2_CsMessageRequest.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessageRequest] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message_no] [int] NOT NULL DEFAULT (1),
	[request_no] [int] NOT NULL DEFAULT (1),
	[request_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[request_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[request_type] [nvarchar] (10) NOT NULL,
	[urgency_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[approval_type] [nvarchar] (10) NOT NULL DEFAULT (N'CONS'),
	[comment] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[working_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessageRequest] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessageRequest] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[incident_id],
		[message_no],
		[request_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessageRequest_1] ON [dbo].[w2_CsMessageRequest]([dept_id], [request_operator_id]) ON [PRIMARY]
GO
