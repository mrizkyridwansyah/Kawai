
create   procedure [dbo].[sp_Wms_IQCResult_SaveResult]
	@InspectionId bigint,
	@QtyNG numeric(18,2),
	@Remarks varchar(max),
	@AttachmentName varchar(max),
	@UserId varchar(25)
as
begin
	if exists (select 1 from IQC_Inspection_Header where InspectionID = @InspectionId and InspectionResultDate is not null)
	begin
		raiserror('Data QC sudah diapprove', 16,1)
		return
	end
		
	declare @qtySample numeric(18,2) = isnull((select TotalQtySample From IQC_Inspection_Header where InspectionID = @InspectionId),0)

	if @QtyNG > @qtySample
	begin
		raiserror('Qty NG tidak boleh melebihi Qty Sample', 16,1)
		return
	end

	update IQC_Inspection_Header 
	set 
		TotalQtyNG = @QtyNG, LastUpdate = getdate(), Remarks = @Remarks, StatusQC = 'INPUT'
	where InspectionID = @InspectionId

	if isnull(@AttachmentName, '') = ''
	begin
		DELETE FROM IQC_Attachment WHERE InspectionID = @InspectionId
	end
	else
	begin
		if not exists (select 1 from IQC_Attachment where InspectionID = @InspectionId)
		begin
			insert into IQC_Attachment (InspectionID, [FileName], RegisterUser, RegisterDate)
			values (@InspectionId, @AttachmentName, @UserId, getdate())
		end
		else 
		begin
			update IQC_Attachment 
			set 
				[FileName] = @AttachmentName, RegisterDate = getdate(), RegisterUser = @UserId
			where InspectionID = @InspectionId
		end
	end

end
