SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartReceiptHeader](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceiptNo] [varchar](100) NOT NULL,
	[ReceiptDate] [date] NOT NULL,
	[SupplierCode] [varchar](15) NULL,
	[DNNumber] [varchar](50) NULL,
	[DNDate] [date] NULL,
	[BCNumber] [varchar](50) NULL,
	[BCType] [varchar](15) NULL,
	[BCDate] [date] NULL,
	[VehicleNo] [varchar](15) NULL,
	[StatusReceipt] [varchar](20) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
	[IsManual] [bit] NULL,
	[HasValid] [bit] NULL,
	[ValidDate] [datetime] NULL,
 CONSTRAINT [PK_PartReceiptHeader] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
