SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MaterialNGClaimHeader](
	[ClaimID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClaimNo] [varchar](50) NOT NULL,
	[SupplierCode] [varchar](50) NOT NULL,
	[SupplierName] [varchar](150) NULL,
	[ClaimDate] [date] NOT NULL,
	[TotalQty] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[Status] [varchar](30) NOT NULL,
	[Notes] [varchar](max) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](50) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](50) NULL,
	[DNNumber] [varchar](50) NULL,
	[DNDate] [date] NULL,
	[BCNumber] [varchar](50) NULL,
	[BCType] [varchar](15) NULL,
	[BCDate] [date] NULL,
	[VehicleNo] [varchar](15) NULL,
	[Transport] [varchar](15) NULL,
	[Approved_User] [varchar](30) NULL,
	[Approved_Date] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
