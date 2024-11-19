using System;

namespace SRWorkerServer.Application.Interfaces;

public interface ICorePixelService{
    public Task Publish<TIn, TOut>(string queue, TOut dto, string methodName, string className);
    public Task Broadcast<TDto>(string groupName, TDto dto, string methodName, string className);
    public Task PublishAndBroadcast<TIn, TOut>(string groupName, string queue, TOut dto, string methodName, string className);
}
