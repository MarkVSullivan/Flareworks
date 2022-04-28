


CREATE PROCEDURE [dbo].[Delete_ItemWork_Info]
	@ItemWorkID int
as
begin

	delete from ItemWork 
	where ItemWorkID=@ItemWorkID;

end;
go


alter table Users add LastQcLocationSelected varchar(30) null;
alter table Users add LastQcUserSelected varchar(25) null;
GO


ALTER procedure [dbo].[Get_User_By_ID]
	@UserID int
as
begin

	-- Return all the data from the users table
	select U.UserID, U.Username, U.Email, U.DateAdded, U.PendingApproval, U.[Disabled], 
	       U.CanQC, U.CanRunReports, U.IsSystemAdmin, U.CanAddToPullLists, U.IsPullListAdmin, U.CanAdvancedSearch, 
		   coalesce(U.LastAddedFormType,'') as LastAddedFormType,
		   coalesce(U.LastSearchFormType,'') as LastSearchFormType,
		   coalesce(U.LastSearchByCriteria, '') as LastSearchByCriteria,
		   coalesce(C1.CatalogingType,'') as LastAddedCatalogType,
		   coalesce(I1.InstitutionCode,'') as LastAddedInstitution,
		   coalesce(B1.BibliographicLevel,'') as LastAddedBiblioLevel,
		   coalesce(M1.MaterialType,'') as LastAddedMaterialType,
		   coalesce(D1.DocumentType,'') as LastAddedDocumentType,
		   coalesce(L2.LocationCode,'') as LastSearchLocation,
		   coalesce(I2.InstitutionCode,'') as LastSearchInstitution,
		   coalesce(L3.LocationID,-1) as LocationID,
		   coalesce(U.DisplayName, U.UserName) as DisplayName,
		   coalesce(U.FullName, U.DisplayName, U.UserName) as FullName,
		   isTemporary_Password, U.CatalogingSpecialist,
		   coalesce(LastQcLocationSelected,'') as LastQcLocationSelected,
		   coalesce(LastQcUserSelected,'') as LastQcUserSelected
	from Users U left outer join
	     CatalogingTypes C1 on U.LastAddedCatalogTypeID=C1.CatalogingTypeID left outer join
		 Institutions I1 on U.LastAddedInstitutionID=I1.InstitutionID left outer join
		 BibliographicLevels B1 on U.LastAddedBiblioLevelID=B1.BibliographicLevelID left outer join
		 MaterialTypes M1 on U.LastAddedMaterialTypeID=M1.MaterialTypeID left outer join
		 DocumentTypes D1 on U.LastAddedDocumentTypeID=D1.DocumentTypeID left outer join
		 Locations L2 on U.LastSearchLocationID=L2.LocationID left outer join
		 Institutions I2 on U.LastSearchInstitutionID=I2.InstitutionID left outer join
		 Locations L3 on U.LocationID=L3.LocationID
	where UserID=@UserID;

end;
GO


ALTER PROCEDURE [dbo].[Get_QC_Dashboard_Data] 
	@FilterLocation varchar(30),
	@FilterUser varchar(25),
	@UserID int
AS
begin

	with pending_qc as 
	(
		select S.ItemWorkSetID, coalesce(AlephNum,'') as AlephNum, coalesce(OCLC,'') as OCLC, U.DisplayName as WorkerName, DateSubmitted, coalesce(DateRejected, '1/1/1900') as DateRejected, S.InstitutionID, S.MaterialTypeID, L.LocationCode
		from Titles T, ItemWorkSet S, Users U, Locations L
		where ( S.TitleID = T.TitleID )
		  and ( S.DateSubmitted is not null )
		  and ( S.DateApproved is null )
		  and ( S.DateRejected is null )
		  and ( S.WorkerID = U.UserID )
		  and ( U.LocationID = L.LocationID )
	),
	items_updated as
	(
		select S.ItemWorkSetID, (SUM(ItemsSentToTray) + SUM(ItemsWithdrawnDupes) + SUM(ItemsDamaged)) as ItemsProcessed, InstitutionCode, MaterialType
		from pending_qc S, ItemWork W, Institutions I, MaterialTypes M
		where S.ItemWorkSetID = W.ItemWorkSetID
		  and S.InstitutionID = I.InstitutionID
		  and S.MaterialTypeID = M.MaterialTypeID
		group by S.ItemWorkSetID, InstitutionCode, MaterialType
	)
	select Q.ItemWorkSetID, AlephNum, OCLC, WorkerName, InstitutionCode, MaterialType, SUM(coalesce(ItemsProcessed,0)) as ItemCount, DateSubmitted, DateRejected, LocationCode
	into #PendingQc
	from pending_qc Q left join
	     items_updated L on L.ItemWorkSetID = Q.ItemWorkSetID
	group by Q.ItemWorkSetID, Q.AlephNum, MaterialType, OCLC, WorkerName, InstitutionCode, MaterialType, DateSubmitted, DateRejected, LocationCode;

	-- Return the appropriate data
	if (( len(@FilterLocation) = 0 ) and ( len(@FilterUser) = 0 ))
	begin
		select * from #PendingQc;
	end
	else if ( len(@FilterUser) > 0 ) 
	begin
		select * 
		from #PendingQc
		where WorkerName=@FilterUser;		
	end 
	else
	begin
		select * 
		from #PendingQc
		where LocationCode=@FilterLocation;
	end;

	-- Get the QC rejected list
	with pending_qc as 
	(
		select S.ItemWorkSetID, coalesce(AlephNum,'') as AlephNum, coalesce(OCLC,'') as OCLC, U.DisplayName as WorkerName, DateSubmitted, coalesce(DateRejected, '1/1/1900') as DateRejected, S.InstitutionID, S.MaterialTypeID, LocationCode
		from Titles T, ItemWorkSet S, Users U, Locations L
		where ( S.TitleID = T.TitleID )
		  and ( S.DateSubmitted is not null )
		  and ( S.DateApproved is null )
		  and ( S.DateRejected is not null )
		  and ( S.WorkerID = U.UserID )
		  and ( U.LocationID = L.LocationID )
	),
	items_updated as
	(
		select S.ItemWorkSetID, (SUM(ItemsSentToTray) + SUM(ItemsWithdrawnDupes) + SUM(ItemsDamaged)) as ItemsProcessed, InstitutionCode, MaterialType
		from pending_qc S, ItemWork W, Institutions I, MaterialTypes M
		where S.ItemWorkSetID = W.ItemWorkSetID
		  and S.InstitutionID = I.InstitutionID
		  and S.MaterialTypeID = M.MaterialTypeID
		group by S.ItemWorkSetID, InstitutionCode, MaterialType
	)
	select Q.ItemWorkSetID, AlephNum, OCLC, WorkerName, InstitutionCode, MaterialType, SUM(ItemsProcessed) as ItemCount, DateSubmitted, DateRejected, LocationCode
	into #QcReject
	from pending_qc Q, items_updated L
	where Q.ItemWorkSetID=L.ItemWorkSetID
	group by Q.ItemWorkSetID, Q.AlephNum, MaterialType, OCLC, WorkerName, InstitutionCode, MaterialType, DateSubmitted, DateRejected, LocationCode
	order by DateRejected ASC;

	-- Return the appropriate data
	if (( len(@FilterLocation) = 0 ) and ( len(@FilterUser) = 0 ))
	begin
		select * from #QcReject;
	end
	else if ( len(@FilterUser) > 0 ) 
	begin
		select * 
		from #QcReject
		where WorkerName=@FilterUser;		
	end 
	else
	begin
		select * 
		from #QcReject
		where LocationCode=@FilterLocation;
	end;

	-- Save this to the user table as the last qc search
	update Users 
	set LastQcLocationSelected=@FilterLocation, LastQcUserSelected=@FilterUser 
	where UserID=@UserID;
end;
GO


update Titles set CatalogingTypeID=2 where CatalogingTypeID=3;
update ItemWorkSet set CatalogingTypeID=2 where CatalogingTypeID=3;
update Users set LastAddedCatalogTypeID=2 where LastAddedCatalogTypeID=3;
delete from CatalogingTypes where CatalogingTypeID=3;
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'Barry University', 'true', 1, 'BARRY' );
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'Florida Gulf Coast University', 'true', 1, 'FGCU' );
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'Florida International University', 'true', 1, 'FIU' );
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'Florida State University', 'true', 1, 'FSU' );
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'University of West Florida', 'true', 1, 'UWF' );
GO

insert into Institutions ( Institution, Enabled, DisplayOrder, InstitutionCode )
values ( 'Last Copy', 'true', 1, 'LAST-COPY' );
GO










