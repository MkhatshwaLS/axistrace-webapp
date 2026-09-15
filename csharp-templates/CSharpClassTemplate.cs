// C# Class Template
// Usage: copy this file and replace the placeholders (NamespaceName, ClassName)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NamespaceName
{
    /// <summary>
    /// Summary description for ClassName.
    /// </summary>
    public class ClassName
    {
        // Example private field
        private readonly ILogger<ClassName>? _logger;

        // Public properties
        public int Id { get; set; }
        public string? Name { get; set; }

        // Parameterless constructor
        public ClassName()
        {
        }

        // Constructor with dependencies
        public ClassName(ILogger<ClassName> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Example async initialization method.
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Example method.
        /// </summary>
        public void DoWork()
        {
            _logger?.LogDebug("Doing work in {ClassName}", nameof(ClassName));
        }

        public override string ToString() => $"{nameof(ClassName)}: {Name ?? "(null)"}";
    }
}
