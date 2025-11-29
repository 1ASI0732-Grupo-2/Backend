using System;
using workstation_backend.OfficesContext.Interface.Resources;
using workstation_backend.OfficesContext.Domain.Models.Entities;

namespace workstation_backend.OfficesContext.Interface.Transform;

public static class OfficeResourceFromEntityAssembler
{
    public static OfficeResource ToResourceFromEntity(Office office)
    {
        var serviceResources = office.Services?
            .Select(s => new OfficeServiceResource(s.Name, s.Description, s.Cost))
            .ToList() ?? new List<OfficeServiceResource>();

        return new OfficeResource(
            office.Id,
            office.Location,
            office.Description,
            office.ImageUrl,
            office.Capacity,
            office.CostPerDay,
            office.Available,
            serviceResources
        );
    }
}
