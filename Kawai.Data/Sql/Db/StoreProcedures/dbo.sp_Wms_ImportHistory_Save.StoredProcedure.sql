SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_ImportHistory_Save]
	@Id	varchar(200),
	@Key varchar(200),
	@Template varchar(200),		
	@Status varchar(40),
	@FileName varchar(510),
	@ContentType varchar(510),
	@SizeFile bigint,
	@RowsCount int,
	@ValidRowsCount int, 
	@InvalidRowsCount int,
	@ProcessDuration bigint,
	@UserId varchar(25)
as
begin
	insert into ImportHistories 
	(
		Id
		,[Key]
		,Template
		,[Status]
		,[FileName]
		,ContentType
		,ProcessDuration
		,SizeFile
		,RowsCount
		,ValidRowsCount
		,InvalidRowsCount
		,[Date]
		,UserId
	)
	values
	(
		@Id
		,@Key
		,@Template
		,@Status
		,@FileName
		,@ContentType
		,@ProcessDuration
		,@SizeFile
		,@RowsCount
		,@ValidRowsCount
		,@InvalidRowsCount
		,getdate()
		,@UserId
	)
end
GO
