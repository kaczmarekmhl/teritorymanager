select
      p.[Name]
      ,p.[PostCode]
      /*,p.[AddedByUserId]*/
      ,p.[Selected]
      ,p.[District_Id]
      ,p.[Manual]
      ,p.[SearchUpdate]
      ,p.[Remarks]
      ,p.[DoNotVisit]
      ,p.[DoNotVisitReportDate]
      ,p.[IsVisitedByOtherPublisher]
      ,p.[VisitingPublisher]
      ,p.[Lastname]
      ,p.[StreetAddress]
      ,p.[TelephoneNumber]
      ,p.[Longitude]
      ,p.[Latitude]
	  , count(0)
from People p
join Districts d on p.District_Id = d.id
where d.Congregation_Id = 1
group by
p.[Name]
      ,p.[PostCode]
      /*,p.[AddedByUserId]*/
      ,p.[Selected]
      ,p.[District_Id]
      ,p.[Manual]
      ,p.[SearchUpdate]
      ,p.[Remarks]
      ,p.[DoNotVisit]
      ,p.[DoNotVisitReportDate]
      ,p.[IsVisitedByOtherPublisher]
      ,p.[VisitingPublisher]
      ,p.[Lastname]
      ,p.[StreetAddress]
      ,p.[TelephoneNumber]
      ,p.[Longitude]
      ,p.[Latitude]
having count(0) > 1


begin tran
commit tran
delete pd 
from People pd
join Districts dd on pd.District_Id = dd.Id
where exists(
	select *
	from People p
	where pd.District_Id = p.District_Id
	and pd.PostCode = p.PostCode
	and pd.Manual = p.Manual
	and pd.Lastname = p.Lastname
	and pd.StreetAddress = p.StreetAddress
	and pd.TelephoneNumber = p.TelephoneNumber
	and pd.Longitude = p.Longitude
	and pd.Latitude = p.Latitude
	and pd.Id != p.Id
	and p.AddedByUserId = dd.AssignedToUserId
	)
	and  pd.AddedByUserId != dd.AssignedToUserId
	and dd.Congregation_Id = 1
	and Remarks is null
	and pd.DoNotVisit = 0
	and pd.IsVisitedByOtherPublisher = 0
	and pd.Manual = 0

select pd.name, pd.Lastname, pd.District_Id
from People pd
join Districts dd on pd.District_Id = dd.Id
where exists(
	select *
	from People p
	where pd.District_Id = p.District_Id
	and pd.PostCode = p.PostCode
	and pd.Manual = p.Manual
	and pd.Lastname = p.Lastname
	and pd.StreetAddress = p.StreetAddress
	and pd.TelephoneNumber = p.TelephoneNumber
	and pd.Longitude = p.Longitude
	and pd.Latitude = p.Latitude
	and pd.Id != p.Id
	and p.AddedByUserId = dd.AssignedToUserId
	)
	and  pd.AddedByUserId != dd.AssignedToUserId
	and dd.Congregation_Id = 1
	and Remarks is null
	and pd.DoNotVisit = 0
	and pd.IsVisitedByOtherPublisher = 0
	and pd.Manual = 0
order by District_Id