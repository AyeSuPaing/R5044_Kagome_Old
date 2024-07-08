if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertItAdFiles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[InsertItAdFiles]
GO
/*
=========================================================================================================
  Module      : 商品コンバータ設定挿入プロシージャ(InsertItAdFiles.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertItAdFiles]
 @AdtoId int,
 @path varchar(260)
AS
	BEGIN TRANSACTION

	-- 同じパスが存在したら削除してから追加
	DELETE FROM w2_MallPrdcnvFiles WHERE path = @path
	IF @@error <> 0 GOTO CATCH

	INSERT INTO w2_MallPrdcnvFiles(adto_id,path) VALUES(@AdtoId,@path)
	IF @@error <> 0 GOTO CATCH

	UPDATE w2_MallPrdcnv SET lastCreated=CURRENT_TIMESTAMP WHERE adto_id=@AdtoId
	IF @@error <> 0 GOTO CATCH

	COMMIT TRANSACTION
	RETURN 0

	CATCH:
	ROLLBACK TRANSACTION
	RETURN -1
RETURN
