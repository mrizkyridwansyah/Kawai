SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MaterialNGClaimDetail](
	[DetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClaimID] [bigint] NOT NULL,
	[ItemCode] [varchar](50) NOT NULL,
	[PONumber] [varchar](50) NULL,
	[ReceiptNo] [varchar](100) NULL,
	[QtyNG] [decimal](18, 2) NOT NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NULL,
	[NGCode] [varchar](50) NOT NULL,
	[NGDescription] [varchar](max) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
