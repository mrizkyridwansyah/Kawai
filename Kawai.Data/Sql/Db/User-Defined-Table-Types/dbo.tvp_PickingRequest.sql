DROP TYPE IF EXISTS [tvp_PickingRequest]
CREATE TYPE [dbo].[tvp_PickingRequest] AS TABLE(
	[PONo] [nvarchar](50) NULL,
	[POSeqNo] [int] NULL,
	[ItemCode] [nvarchar](50) NULL,
	[SerialNo] [nvarchar](50) NULL
)
GO


