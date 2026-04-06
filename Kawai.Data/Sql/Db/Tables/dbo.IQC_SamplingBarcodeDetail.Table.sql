SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_SamplingBarcodeDetail](
	[SamplingID] [int] IDENTITY(1,1) NOT NULL,
	[InspectionID] [int] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[CurrentStock] [numeric](18, 2) NOT NULL,
	[SampleQTY] [numeric](18, 2) NOT NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[NGCode] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[SamplingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--lect * From IQC_Inspection_Header
--select * From IQC_SamplingBarcodeDetail

create TRIGGER [trg_IQCDetail_UpdateHeaderQty]
ON [IQC_SamplingBarcodeDetail]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Update total Qty berdasarkan perubahan data di Detail
    UPDATE H
    SET H.TotalQtySample = ISNULL((
        SELECT SUM(D.SampleQTY)
        FROM IQC_SamplingBarcodeDetail D
        WHERE D.InspectionID = H.InspectionID
    ), 0),LastUpdate=getdate()
    FROM IQC_Inspection_Header H
    WHERE H.InspectionID IN (
        SELECT InspectionID FROM inserted
        UNION
        SELECT InspectionID FROM deleted
    );

  
END;
GO
ALTER TABLE [dbo].[IQC_SamplingBarcodeDetail] ENABLE TRIGGER [trg_IQCDetail_UpdateHeaderQty]
GO
