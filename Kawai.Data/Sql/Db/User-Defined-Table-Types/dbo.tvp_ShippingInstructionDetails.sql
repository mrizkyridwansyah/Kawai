DROP TYPE IF EXISTS [tvp_ShippingInstructionDetails]
CREATE TYPE [dbo].[tvp_ShippingInstructionDetails] AS TABLE(
	[Item_Code] [varchar](50) NULL,
	[PO_SeqNo] [int] NULL,
	[Qty] [numeric](18, 9) NULL,
	[SerialNo_From] [varchar](100) NULL,
	[SerialNo_To] [varchar](100) NULL
)