DROP TYPE [dbo].[tvp_ShippingInstructionPicking]
GO

/****** Object:  UserDefinedTableType [dbo].[tvp_ShippingInstructionPicking]    Script Date: 2026-08-06 12:39:59 ******/
CREATE TYPE [dbo].[tvp_ShippingInstructionPicking] AS TABLE(
	[SerialNo] [varchar](100) NULL,
	[AlreadyPicking] [bit] NULL
)
GO