SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Mobile_MaterialNG_Capture]
	@InspectionId bigint
as
begin
	SELECT * fROM IQC_Inspection_Header WHERE InspectionID = @InspectionId

	SELECT * fROM IQC_SamplingBarcodeDetail WHERE InspectionID = @InspectionId

end
GO
