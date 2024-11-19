using System;

namespace SRWorkerServer.GridCache.Data.Entities;

public class GridEntity
{
    public int SizeX {get; set;}
    public int SizeY {get; set;}
    public byte[] Bytes {get; set;} = [];

    public int CalculatePosition(int posX, int posY){
        return (this.SizeX * posY) + posX;
    }
}
