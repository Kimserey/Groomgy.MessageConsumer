using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Groomgy.MessageConsumer.Abstractions
{
    public interface IHost<TRaw>
    {
        IHost<TRaw> ConfigureServices(Action<IConfiguration, IServiceCollection> configureServices);

        IHost<TRaw> ConfigureLogger(Action<ILoggingBuilder> configureLogger);

        IHost<TRaw> Map<TPathFiler>(Action<IPathBuilder<TRaw>> builder) where TPathFiler: IPathFiler<TRaw>;

        void Start();
    }

    public interface IPathHandler<in TRaw>
    {
        Task<bool> Handle(TRaw message);
    }

    public interface IPathBuilder<TRaw>
    {
        IPathBuilder<TRaw> AddDecoder<TMessage, TMapper>()
            where TMapper: IDecoder<TRaw, TMessage>;

        IPathBuilder<TRaw> AddHandler<TMessage, THandler>()
            where THandler : IHandler<TMessage>;

        IPathHandler<TRaw> Build();
    }

    public interface IPathFiler<in TRaw>
    {
        Task<bool> Filter(TRaw message);
    }

    public interface IHandler<in TMessage>
    {
        Task<bool> CanHandle(Context context, TMessage message);

        Task<bool> Handle(Context context, TMessage message);
    }

    public interface IDecoder<in TRaw, TMessage>
    {
        Task<bool> CanDecode(Context context, TMessage message);

        Task<bool> Decode(Context context, TRaw raw, out TMessage mapped);
    }

    public class Context : Dictionary<string, string>
    {
        public string CorrelationId { get; set; }
    }

    public class Meta
    {
        public Type Type { get; set; }

        public MethodInfo CanPerform { get; set; }

        public MethodInfo Perform { get; set; }
    }
}