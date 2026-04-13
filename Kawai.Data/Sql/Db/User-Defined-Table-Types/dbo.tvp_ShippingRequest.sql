DROP TYPE IF EXISTS [tvp_ShippingRequest]
CREATE TYPE [dbo].[tvp_ShippingRequest] AS TABLE(
	[PONo] [nvarchar](50) NULL,
	[POSeqNo] [int] NULL,
	[ItemCode] [nvarchar](50) NULL,
	[SerialNoFrom] [nvarchar](50) NULL,
	[SerialNoTo] [nvarchar](50) NULL,
	[SIDate] [datetime] NULL
)
GO


