using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;

namespace DbServer.Application.Services.Database.SqlQuery;

public class PixelQueryService : IPixelQueryService
{
    public string GetManyByPosition => "SELECT * FROM pixels WHERE PosX = @PosX AND PosY = @PosY;";

    public string GetManyByColor => "SELECT * FROM pixels WHERE Color = @Color;";

    public string GetManyByDate => "SELECT * FROM pixels WHERE CreatedAt = @CreatedAt;";

    public string DeleteManyByPosition => "DELETE FROM pixels WHERE PosX = @PosX AND PosY = @PosY;";

    public string DeleteManyByColor => "DELETE FROM pixels WHERE Color = @Color;";

    public string DeleteManyByDate => "DELETE FROM pixels WHERE CreatedAt = @CreatedAt;";

    public string GetByPositionWithCanvasId => throw new NotImplementedException();

    public string DeleteByPositionWithCanvasId => throw new NotImplementedException();

    public string GetById => "SELECT * FROM pixels WHERE Id = @Id;";

    public string DeleteById => "DELETE FROM pixels WHERE Id = @Id";

    public string Add => "INSERT INTO pixels (PosX, PosY, Color, CreatedAt) VALUES (@PosX, @PosY, @Color, @CreatedAt)";
}
