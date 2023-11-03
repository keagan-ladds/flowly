using Flowly.Core.Builders;
using System;

namespace Flowly.Extensions.NLog.Extensions
{
    public static class RunnerBuilderExtensions
    {
        public static RunnerBuilder WithNLog(this RunnerBuilder builder)
        {
            builder.WithLoggerSource(new NLogSource());
            return builder;
        }

        public static RunnerBuilder WithNLog(this RunnerBuilder builder, NLogSource source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            builder.WithLoggerSource(source);
            return builder;
        }
    }
}
