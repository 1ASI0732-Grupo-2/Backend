namespace workstation_backend.OfficesContext.Interface.Resources;

public record class OfficeResource(Guid Id, string Location, string Description, string ImageUrl, int Capacity, int CostPerDay, bool Available, List<OfficeServiceResource> Services);
